using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivitySms
{
    public class SendSmsIntegration
    {
        IOrganizationService orgService;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public static string sftpUsername = Globals.SftpUsernameLive;
        public static string sftpPassword = Globals.SftpPasswordLive;
        public static string sftpUrl = "file.euromsg.com";

        public SendSmsIntegration()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivitySms", sdaCustom);
        }

        public void Execute()
        {
            string campaignXmlFile = string.Empty;
            string smsList = string.Empty;
            string dataFile = string.Empty;
            string zipFileName = string.Empty;
            Guid listId = Guid.Empty;

            Console.WriteLine("SMS gönderim uygulaması çalıştı.");
            try
            {
               



                logMe.Log("SendSmsIntegration - Execute", "Mail gönderim uygulaması başladı.", TEMPEventLog.EventType.Info);

                #region |   Get Campaign Activity   |
                //Euromessage' a gönderilmeyi bekleyen Kampanya Aktiviteleri çekiliyor.
                string queryCActivity = @"
                                    SELECT
	                                    CA.ActivityId CAID,
                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledstart) CAS,
                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledend) CAE,
	                                    CA.[Subject] CS,
                                        CA.new_smstext MSG
                                    FROM
	                                    CampaignActivity CA (NOLOCK)
                                    WHERE
	                                    CA.StatusCode = 100000000 AND CA.StateCode=0 AND CA.ChannelTypeCode=3";
                sda.openConnection(Globals.ConnectionString);
                DataTable dtCActivity = sda.getDataTable(queryCActivity);
                sda.closeConnection();
                Console.WriteLine("Euromessage' a iletilmeyi bekleyen kampanya aktiviteleri alındı.");
                logMe.Log("SendSmsIntegration - Execute", "Euromessage' a iletilmeyi bekleyen kampanya aktiviteleri alındı. Adet: " + dtCActivity.Rows.Count, TEMPEventLog.EventType.Info);
                #endregion |   Get Campaign Activity   |

                SmsFunctions sFunc = new SmsFunctions();

                foreach (DataRow dr in dtCActivity.Rows)
                {
                    Console.WriteLine("Kampanya aktivitesi işlemleri başladı.");
                    DateTime startDate = (DateTime)dr["CAS"];

                    //if ((DateTime.Now.Hour == startDate.Hour && DateTime.Now.Minute >= startDate.Minute) || (DateTime.Now.Hour > startDate.Hour))
                    //{
                    Guid campaignActivityID = new Guid(dr["CAID"].ToString());

                    //Kampanya aktivitesine ait listeler çekilir.
                    DataTable marketingLists = GetMarketingList(sda, campaignActivityID);

                    //Eğer kampanya aktivitesi altında pazarlama listesi yoksa herhangi bir işlem yapılmaz.
                    if (marketingLists != null && marketingLists.Rows.Count > 0)
                    {
                        campaignXmlFile = sFunc.BuildXml(dr);

                        if (!string.IsNullOrEmpty(campaignXmlFile))
                        {
                            foreach (DataRow drList in marketingLists.Rows)
                            {
                                listId = new Guid(drList["ListId"].ToString());

                                if (listId != Guid.Empty)
                                {
                                    //Pazarlama listesine eklenen müsteriler çekilir.
                                    DataTable customerList = sFunc.GetCustomerList(sda, campaignActivityID, listId);

                                    if (customerList != null && customerList.Rows.Count > 0)
                                        smsList += sFunc.BuildSmsList(customerList);
                                }
                            }

                            if (!string.IsNullOrEmpty(smsList))
                            {
                                dataFile = sFunc.BuildDataFile(smsList);

                                if (!string.IsNullOrEmpty(dataFile))
                                {
                                    zipFileName = sFunc.WriteZip(dataFile, campaignXmlFile);
                                    sFunc.UploadViaSftp(zipFileName, sftpUrl, sftpUsername, sftpPassword);

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
                                    logMe.Log("SendSmsIntegration - Execute", "Marketing list için .txt oluşturulamadı.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Kampanya aktivitesi için XML oluşturulamadı.");
                            logMe.Log("SendSmsIntegration - Execute", "Kampanya aktivitesi için XML oluşturulamadı.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Kampanya aktivitesine ait pazarlama listesi bulunmamaktadır.");
                        logMe.Log("SendSmsIntegration - Execute", "Kampanya aktivitesine ait pazarlama listesi bulunmamaktadır.", TEMPEventLog.EventType.Info, "new_campaignactivity", dr["CAID"].ToString());
                    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Başlangıç saati gelmediğinden dolayı kampanya aktivitesi Euromsg'a gönderilmedi.");
                    //    logMe.Log("SendSmsIntegration - Execute", "Başlangıç saati gelmediğinden dolayı kampanya aktivitesi Euromsg'a gönderilmedi. Kampanya Aktivitesi ID: " + dr["CAID"].ToString(), TEMPEventLog.EventType.Info);
                    //}

                    Console.WriteLine("Kampanya aktivitesi işlemleri bitti.");
                }
                Console.WriteLine("Gönderilmeyi bekleyen kampanya aktivitelerinin alınma işlemi sona erdi.");
                logMe.Log("SendSmsIntegration - Execute", "Gönderilmeyi bekleyen kampanya aktivitelerinin alınma işlemi sona erdi.", TEMPEventLog.EventType.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("İşlemler sırasında bir hata ile karşılaşıldı.");
                logMe.Log("SendSmsIntegration - Execute", ex, TEMPEventLog.EventType.Exception);
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
