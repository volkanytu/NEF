using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
//using NEF.ConsoleApp.CreateCampaignActivityAndList.com.euromsg.ws.auth;
//using NEF.ConsoleApp.CreateCampaignActivityAndList.com.euromsg.ws.campaign;
//using NEF.ConsoleApp.CreateCampaignActivityAndList.sendlist;
using NEF.ConsoleApp.CreateCampaignActivityAndList.com.euromsg.ws.auth.live;
using NEF.ConsoleApp.CreateCampaignActivityAndList.com.euromsg.ws.campaign.live;
using NEF.ConsoleApp.CreateCampaignActivityAndList.sendlist.live;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.CreateCampaignActivityAndList
{
    public class SmsFunctions
    {
        #region | Members |

        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        Campaign campaign;
        IOrganizationService orgService;
        TEMPEventLog logMe;

        #endregion | Members |

        public SmsFunctions()
        { 
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            campaign = new Campaign();
            orgService = MSCRM.AdminOrgService;
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.CreateCampaignActivityAndList", sdaCustom);
        }

        public ProcessResult CreateSendListOnEuroMessage(string listName, string AuthenticationServiceKey)
        {
            try
            {
                SendList sendList = new SendList();
                EmSendListResult emResult = sendList.CreateSendList(AuthenticationServiceKey, "CRM", listName);

                if (emResult.Code == "00")
                    return new ProcessResult(true, "", "");
                else
                    return new ProcessResult(false, "", emResult.Code);
            }
            catch (Exception ex)
            {
                return new ProcessResult(false, "", ex.Message);
            }
        }

        public ProcessResult AddToSendLists(Guid campaignActivityId, string AuthenticationServiceKey, string listName, IOrganizationService service)
        {
            try
            {
                bool sonuc = false;

                #region |   Query Contact   |

                string queryList = @"SELECT
	                                C.FirstName [FirstName],
	                                C.LastName [LastName],
	                                C.mobilephone [MobilePhone],
                                    C.emailaddress1 [EmailAddress],
	                                C.ContactId [ContactId]
                                FROM
	                                Contact C WITH(NOLOCK)
                                WHERE
	                                C.StateCode = 0
                                AND
	                                C.birthdate IS NOT NULL
                                AND
	                                LEFT((RIGHT(CONVERT(VARCHAR(10), dbo.fn_UTCToTzSpecificLocalTime(C.birthdate,-120,-60,0,3,5,3,0,0,0,1,0,0,1,5,4,0,0,0,0), 103), 10)), 5) = LEFT((RIGHT(CONVERT(VARCHAR(10), GETDATE(), 103), 10)), 5)
                                AND
	                                C.donotphone = 0
                                AND
	                                C.mobilephone IS NOT NULL
                                AND
	                                C.new_invalidmobilephone = 0 
                                AND
	                                C.emailaddress1 IS NOT NULL";
                sda.openConnection(Globals.ConnectionString);
                DataTable dt = sda.getDataTable(queryList, new SqlParameter("@campaignActivityId", campaignActivityId));
                sda.closeConnection();
                #endregion |   Query Contact   |

                List<sendlist.live.EmKeyValue[]> demographicDatas = new List<sendlist.live.EmKeyValue[]>();
                List<SmsDetail> smsDetails = new List<SmsDetail>();

                logMe.Log("SmsFunctions - AddToSendLists", "Listedeki kişi sayısı: " + dt.Rows.Count.ToString() + ", Aktivite ID : " + campaignActivityId, TEMPEventLog.EventType.Info);

                int counter = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    Console.WriteLine("Sayaç:" + counter.ToString());

                    string firstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : "";
                    string lastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : "";
                    string phoneNumber = dr["MobilePhone"] != DBNull.Value ? dr["MobilePhone"].ToString() : "";
                    string customerId = dr["ContactId"] != DBNull.Value ? dr["ContactId"].ToString() : "";
                    string customermailaddress = dr["EmailAddress"] != DBNull.Value ? dr["EmailAddress"].ToString() : "";

                    //Eğer Contact üzerindeki tüm EPosta alanları boş ise..
                    if (phoneNumber == "-1")
                        continue;

                    sendlist.live.EmKeyValue[] demographicData = {
                        new sendlist.live.EmKeyValue(){ Key ="EMAIL", Value = customermailaddress },
                        new sendlist.live.EmKeyValue(){ Key ="Adınız", Value = firstName },
                        new sendlist.live.EmKeyValue(){ Key ="Soyadınız", Value = lastName},
                        new sendlist.live.EmKeyValue(){ Key ="Cep_Telefonunuz", Value = phoneNumber },                        
                        new sendlist.live.EmKeyValue(){ Key ="CustomerId", Value = customerId }                       
                    };

                    demographicDatas.Add(demographicData);
                    counter++;
                }

                int packetCount = 0;

                Console.WriteLine("Kişi Sayısı:" + demographicDatas.Count + " bir tuşa basınız");

                if (demographicDatas.Count > 0)
                {
                    packetCount = demographicDatas.Count / 2000;
                }

                Console.WriteLine("Paket Sayısı:" + packetCount.ToString() + " Devam etmek için tuşa basınız");

                List<BulkDetailedResult> resultList = new List<BulkDetailedResult>();

                for (int i = 0; i <= packetCount; i++)
                {
                    Console.WriteLine("Paket Sayısı:" + demographicDatas.Skip(2000 * i).Take(2000).ToArray().Length.ToString());
                    BulkDetailedResult[] results;
                    SendList sendListService = new SendList();
                    sendListService.Timeout = 10000000;
                    Console.WriteLine("Toplu gönderim yapılacak.Bir tuşa basınız.");

                    EmSendListResult result = sendListService.AddBulk(AuthenticationServiceKey, "CRM", listName, "EMAIL", demographicDatas.Skip(2000 * i).Take(2000).ToArray(), true, out results);

                    if (result.Code == "00")
                    {
                        resultList.AddRange(results);
                        Console.WriteLine(i.ToString() + ". Paket Gönderildi.Bir Tuşa Basınız.");
                    }
                    else
                        Console.WriteLine("Hata:" + result.DetailedMessage);
                }

                if (resultList.Count > 0)
                {
                    sonuc = true;
                    Console.WriteLine("Kampanya Gönderildi. Adet:" + resultList.Count.ToString());
                }
                else
                    return new ProcessResult(false, "Hata", "Liste EuroMsg tarafına iletilemedi.");

                if (sonuc == true)
                    return new ProcessResult(true, "", "00");
                else
                    return new ProcessResult(false, "", "99");
            }
            catch (Exception ex)
            {
                return new ProcessResult(false, "", ex.StackTrace);
            }
        }

        public string AuthenticationEM()
        {
            try
            {
                string AuthenticationServiceKey = string.Empty;

                Auth auth = new Auth();
                EmAuthResult authResult = auth.Login(Globals.EuroMessageUserNameLive, Globals.EuroMessagePasswordLive);
                if (authResult.Code == "00")
                {
                    AuthenticationServiceKey = authResult.ServiceTicket;
                    return AuthenticationServiceKey;
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void LogoutEM(string AuthenticationServiceKey)
        {
            if (AuthenticationServiceKey != "")
            {
                Auth auth = new Auth();
                auth.Logout(AuthenticationServiceKey);
            }
        }
    }
}
