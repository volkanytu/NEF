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

namespace NEF.ConsoleApp.SendMarketingList
{
    public class SendSmsIntegration
    {
        IOrganizationService orgService;
        SqlConnection conn;
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

                SmsFunctions sFunc = new SmsFunctions();

                Console.WriteLine("Kampanya aktivitesi işlemleri başladı.");

                //if ((DateTime.Now.Hour == startDate.Hour && DateTime.Now.Minute >= startDate.Minute) || (DateTime.Now.Hour > startDate.Hour))
                //{
                //Kampanya aktivitesine ait listeler çekilir.
                DataTable marketingLists = GetMarketingList(sda);

                //Eğer kampanya aktivitesi altında pazarlama listesi yoksa herhangi bir işlem yapılmaz.
                if (marketingLists != null && marketingLists.Rows.Count > 0)
                {
                    foreach (DataRow drList in marketingLists.Rows)
                    {
                        listId = new Guid(drList["ListId"].ToString());

                        if (listId != Guid.Empty)
                        {
                            //Pazarlama listesine eklenen müsteriler çekilir.
                            DataTable customerList = sFunc.GetCustomerList(sda, listId);
                            Console.WriteLine("Data Alındı");
                            if (customerList != null && customerList.Rows.Count > 0)
                            {
                                smsList += sFunc.BuildSmsList(customerList);
                            }
                            Console.WriteLine("Sms Listsi oluşturuldu.");
                            if (!string.IsNullOrEmpty(smsList))
                            {
                                dataFile = sFunc.BuildDataFile(smsList, drList["ListName"].ToString());
                                smsList = string.Empty;
                                Console.WriteLine(customerList.Rows.Count);

                                if (!string.IsNullOrEmpty(dataFile))
                                {
                                    zipFileName = sFunc.WriteZip(dataFile, drList["ListName"].ToString());
                                    sFunc.UploadViaSftp(zipFileName, sftpUrl, sftpUsername, sftpPassword);


                                    UpdateMarkengListStatus(listId);

                                }
                                else
                                {
                                    Console.WriteLine("Marketing list için .txt oluşturulamadı.");
                                    logMe.Log("SendSmsIntegration - Execute", "Marketing list için .txt oluşturulamadı.", TEMPEventLog.EventType.Info, "List", listId.ToString());
                                }
                            }
                        }
                    }


                }
                else
                {
                    Console.WriteLine("Kampanya aktivitesine ait pazarlama listesi bulunmamaktadır.");
                    logMe.Log("SendSmsIntegration - Execute", "Kampanya aktivitesine ait pazarlama listesi bulunmamaktadır.", TEMPEventLog.EventType.Info, "new_campaignactivity", listId.ToString());
                }
                //}
                //else
                //{
                //    Console.WriteLine("Başlangıç saati gelmediğinden dolayı kampanya aktivitesi Euromsg'a gönderilmedi.");
                //    logMe.Log("SendSmsIntegration - Execute", "Başlangıç saati gelmediğinden dolayı kampanya aktivitesi Euromsg'a gönderilmedi. Kampanya Aktivitesi ID: " + dr["CAID"].ToString(), TEMPEventLog.EventType.Info);
                //}

                Console.WriteLine("Kampanya aktivitesi işlemleri bitti.");
                Console.WriteLine("Gönderilmeyi bekleyen kampanya aktivitelerinin alınma işlemi sona erdi.");
                logMe.Log("SendSmsIntegration - Execute", "Gönderilmeyi bekleyen kampanya aktivitelerinin alınma işlemi sona erdi.", TEMPEventLog.EventType.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("İşlemler sırasında bir hata ile karşılaşıldı.");
                logMe.Log("SendSmsIntegration - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        private DataTable GetMarketingList(SqlDataAccess sda)
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
	                                List AS L
                                WHERE
                                    L.new_sendingtype = 1
	                                AND
	                                L.new_sendingstatus = 2
                                ";
                sda.openConnection(Globals.ConnectionString);
                dt = sda.getDataTable(query);
                sda.closeConnection();
                #endregion |   Get Query    |
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private void UpdateMarkengListStatus(Guid _listId)
        {
            Entity e = new Entity("list");
            e.Id = _listId;
            e["new_sendingstatus"] = new OptionSetValue(3);//Send
            MSCRM.AdminOrgService.Update(e);

        }
    }
}
