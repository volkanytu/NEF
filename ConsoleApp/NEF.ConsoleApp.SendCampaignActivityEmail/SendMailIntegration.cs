using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class SendMailIntegration
    {
        IOrganizationService orgService;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public static string sftpUsername = Globals.SftpUsernameLive;
        public static string sftpPassword = Globals.SftpPasswordLive;
        public static string sftpUrl = "file.euromsg.com";

        public SendMailIntegration()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
        }

        public void Execute()
        {
            string campaignXmlFile = string.Empty;
            string mailList = string.Empty;
            string dataFile = string.Empty;
            string zipFileName = string.Empty;
            Guid listId = Guid.Empty;

            Console.WriteLine("Mail gönderim uygulaması çalıştı.");

            try
            {
                logMe.Log("SendMailIntegration - Execute", "Mail gönderim uygulaması başladı.", TEMPEventLog.EventType.Info);

                #region |   Get Campaign Activity   |
                //Euromessage' a gönderilmeyi bekleyen Kampanya Aktiviteleri çekiliyor.
                string queryCActivity = @"
                                    SELECT
	                                    CA.ActivityId CAID,
                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledstart) CAS,
	                                    CA.[Subject] CS
                                    FROM
	                                    CampaignActivity CA (NOLOCK)
                                    WHERE
	                                    CA.StatusCode = 100000000 AND CA.StateCode=0 AND CA.ChannelTypeCode=7";
                sda.openConnection(Globals.ConnectionString);
                DataTable dtCActivity = sda.getDataTable(queryCActivity);
                sda.closeConnection();
                Console.WriteLine("Euromessage' a iletilmeyi bekleyen kampanya aktiviteleri alındı.");
                logMe.Log("SendMailIntegration - Execute", "Euromessage' a iletilmeyi bekleyen kampanya aktiviteleri alındı. Adet: " + dtCActivity.Rows.Count, TEMPEventLog.EventType.Info);
                #endregion |   Get Campaign Activity   |

                EmailFunctions eFunc = new EmailFunctions();

                foreach (DataRow dr in dtCActivity.Rows)
                {
                    Console.WriteLine("Kampanya aktivitesi işlemleri başladı.");
                    DateTime startDate = (DateTime)dr["CAS"];

                    if ((DateTime.Now.Hour == startDate.Hour && DateTime.Now.Minute >= startDate.Minute) || (DateTime.Now.Hour > startDate.Hour))
                    {
                        Guid campaignActivityID = new Guid(dr["CAID"].ToString());

                        //Kampanya aktivitesine ait listeler çekilir.
                        DataTable marketingLists = GetMarketingList(sda, campaignActivityID);

                        //Eğer kampanya aktivitesi altında pazarlama listesi yoksa herhangi bir işlem yapılmaz.
                        if (marketingLists != null && marketingLists.Rows.Count > 0)
                        {
                            campaignXmlFile = eFunc.BuildXml(dr);

                            if (!string.IsNullOrEmpty(campaignXmlFile))
                            {
                                foreach (DataRow drList in marketingLists.Rows)
                                {
                                    listId = new Guid(drList["ListId"].ToString());

                                    if (listId != Guid.Empty)
                                    {
                                        //Pazarlama listesine eklenen müsteriler çekilir.
                                        DataTable customerList = eFunc.GetCustomerList(sda, campaignActivityID, listId);

                                        if (customerList != null && customerList.Rows.Count > 0)
                                            mailList += eFunc.BuildMailList(customerList);
                                    }
                                }

                                if (!string.IsNullOrEmpty(mailList))
                                {
                                    dataFile = eFunc.BuildDataFile(mailList);

                                    if (!string.IsNullOrEmpty(dataFile))
                                    {
                                        zipFileName = eFunc.WriteZip(dataFile, campaignXmlFile);
                                        eFunc.UploadViaSftp(zipFileName, sftpUrl, sftpUsername, sftpPassword);

                                        //Kampanya aktivitesi 100000004->EuroMsg Gönderim Yapıldı olarak UPDATE Ediliyor.
                                        SetStateRequest stateRequest = new SetStateRequest()
                                        {
                                            EntityMoniker = new EntityReference("campaignactivity", campaignActivityID),
                                            State = new OptionSetValue(1),
                                            Status = new OptionSetValue(100000001)
                                        };
                                        SetStateResponse stateResponse = (SetStateResponse)orgService.Execute(stateRequest);

                                        Console.WriteLine("Kampanya aktivitesi Euromsg'a başarıyla gönderildi. Aktivite Id : " + campaignActivityID.ToString());
                                    }
                                    else
                                    {
                                        Console.WriteLine("Marketing list için .txt oluşturulamadı.");
                                        logMe.Log("SendMailIntegration - Execute", "Marketing list için .txt oluşturulamadı.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Kampanya aktivitesi için XML oluşturulamadı.");
                                logMe.Log("SendMailIntegration - Execute", "Kampanya aktivitesi için XML oluşturulamadı.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Kampanya aktivitesine ait pazarlama listesi bulunmamaktadır.");
                            logMe.Log("SendMailIntegration - Execute", "Kampanya aktivitesine ait pazarlama listesi bulunmamaktadır.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Başlangıç saati gelmediğinden dolayı kampanya aktivitesi Euromsg'a gönderilmedi.");
                        logMe.Log("SendMailIntegration - Execute", "Başlangıç saati gelmediğinden dolayı kampanya aktivitesi Euromsg'a gönderilmedi.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                    }

                    Console.WriteLine("Kampanya aktivitesi işlemleri bitti.");
                }

                Console.WriteLine("Gönderilmeyi bekleyen kampanya aktivitelerinin alınma işlemi sona erdi.");
                logMe.Log("SendMailIntegration - Execute", "Gönderilmeyi bekleyen kampanya aktivitelerinin alınma işlemi sona erdi.", TEMPEventLog.EventType.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("İşlemler sırasında bir hata ile karşılaşıldı.");
                logMe.Log("SendMailIntegration - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        private DataTable GetMarketingList(SqlDataAccess sda, Guid campaignActivityID)
        {
            DataTable dt = null;
            try
            {
                #region |   Get Query    |

                string query = @"
                                SELECT 
                                    L.ListId,
                                    L.ListName
                                FROM 
	                                CampaignActivityItemBase AS CIB (NOLOCK)
                                JOIN
	                                List AS L
                                ON
	                                CIB.ItemId = L.ListId
                                WHERE 
                                    CIB.CampaignActivityId = @CAID
                                    AND
	                                CIB.ItemObjectTypeCode = 4300
	                                AND
	                                L.StatusCode = 0
                                ";
                sda.openConnection(Globals.ConnectionString);
                dt = sda.getDataTable(query, new SqlParameter("@CAID", campaignActivityID));
                sda.closeConnection();
                #endregion |   Get Query    |
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}
