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

namespace NEF.ConsoleApp.SendMarketingListEmail
{
    public class CreateMail
    {
        IOrganizationService orgService;
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public CreateMail()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
        }

        public void Execute()
        {
            DateTime startDate;
            DateTime endDate;
            Guid campaignActivityId;
            Guid ownerId;
            Guid marketingListId;
            EMTempProcess emt;

            try
            {
                Console.WriteLine("Mail gönderim uygulaması çalıştı.");
                logMe.Log("CreateMail - Execute", "Mail oluşturma uygulaması başladı.", TEMPEventLog.EventType.Info);

                #region Periyodik gönderimler dağıtıldı durumuna çekilir.
                EmailFunctions eFunc = new EmailFunctions();
                    //eFunc.TestCampaignActivity();

                    //eFunc.SetCampaignActivityDistributed();
                #endregion

                //                #region |   Get Campaign Activity   |
                //                // StatusCode değeri 'Dağıtıldı' ve Kanal Tipi 'Email' olan kampanya aktiviteleri alınıyor.
                //                string queryCActivity = @"
                //                                    SELECT
                //	                                    CA.ActivityId CAID,
                //                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledend) CASEND,
                //                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledstart) CAS,
                //	                                    CA.[Subject] CS,
                //                                        CA.CreatedBy 'owner'                                                 
                //                                    FROM
                //	                                    CampaignActivity CA (NOLOCK)
                //                                    WHERE
                //	                                    CA.StatusCode = 6
                //                                        AND
                //	                                    CA.ChannelTypeCode = 7";

                //                DataTable dtCActivity = sda.GetDataTable(queryCActivity);
                //                Console.WriteLine("Gönderilmeyi bekleyen kampanya aktiviteleri alındı.");
                //                logMe.Log("CreateMail - Execute", "Gönderilmeyi bekleyen kampanya aktiviteleri alındı. Adet: " + dtCActivity.Rows.Count, TEMPEventLog.EventType.Info);
                //                #endregion |   Get Campaign Activity   |

                Console.WriteLine("Email oluşturma işlemleri başladı.");

                try
                {
                    //startDate = (DateTime)dr["CAS"];
                    //endDate = (DateTime)dr["CASEND"];
                    //campaignActivityId = (Guid)dr["CAID"];
                    //ownerId = (Guid)dr["owner"];

                    #region |   Process Marketing Lists    |
                    //Kampanya aktivitesine ait listeler çekilir.
                    DataTable dtMarketingLists = GetMarketingList();

                    if (dtMarketingLists != null && dtMarketingLists.Rows.Count > 0)
                    {
                        foreach (DataRow drList in dtMarketingLists.Rows)
                        {
                            marketingListId = new Guid(drList["ListId"].ToString());
                            try
                            {
                               
                                emt = new EMTempProcess();
                                bool result = emt.Process( marketingListId.ToString());

                                // Result true dönerse kampanya aktivitesinin durumu değiştirilir.
                                //if (result)
                                //{
                                    //SetStateRequest stateRequest = new SetStateRequest()
                                    //{
                                    //    EntityMoniker = new EntityReference("campaignactivity", campaignActivityId),
                                    //    State = new OptionSetValue(0),
                                    //    Status = new OptionSetValue(100000003)
                                    //};
                                    //SetStateResponse stateResponse = (SetStateResponse)orgService.Execute(stateRequest);
                                //}
                            }
                            catch (Exception ex)
                            {
                                logMe.Log("CreateMail", ex, TEMPEventLog.EventType.Exception, "List", marketingListId.ToString());
                                continue;
                            }
                        }
                    }
                    #endregion |   Process Marketing Lists    |
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable GetMarketingList()
        {
            DataTable dt = null;
            try
            {
                #region |   Get Marketing List    |
                string query = @"
                                SELECT 
                                    L.ListId,
                                    L.ListName
                                FROM 
	                                List AS L
                                WHERE
                                    L.new_sendingtype = 2
	                                AND
	                                L.new_sendingstatus = 2
                                ";
                sda.openConnection(Globals.ConnectionString);
                dt = sda.getDataTable(query);
                sda.closeConnection();
                #endregion |   Get Marketing List    |
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}
