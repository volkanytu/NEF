using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivitySms
{
    public class UpdateCampaignActivity
    {
        IOrganizationService orgService;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public UpdateCampaignActivity()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivitySms", sdaCustom);
        }

        public void Execute()
        {
            try
            {
                logMe.Log("UpdateCampaignActivity_SMS - Execute", "UpdateCampaignActivity Uygulaması Başladı.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

                #region |   Get Campaign Activity   |

                #region |   Query   |

                string queryCA = @"SELECT	
                                    E.ActivityId CAID,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageSmsErrorTable WHERE Error = 1 AND ActivityId = e.ActivityId
                                    )FailureCount,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageSmsTempTable WHERE Error = 0 AND ActivityId = e.ActivityId AND StatusCode!=5
                                    )EmailCount,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageSmsTempTable WHERE Error = 0 AND StatusCode = 100000003 AND ActivityId = e.ActivityId
                                    )NoSendEmailCount,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageSmsTempTable WHERE ActivityId = e.ActivityId AND Error = 0 AND ( StatusCode = 100000001 OR StatusCode = 1 )
                                    )ResponseWaitingCount
                                FROM
                                    NEFCUSTOM_MSCRM..EuroMessageSmsTempTable E (NOLOCK),
                                    CampaignActivity Ca (NOLOCK)
                                WHERE
	                                ca.ActivityId = E.ActivityId 
--and 
	                               -- GETDATE() > Ca.new_euromessagereportlimit
                                GROUP BY
                                    E.ActivityId";

                #endregion |    Query   |

                sda.openConnection(Globals.ConnectionString); ;
                DataTable dtCA = sda.getDataTable(queryCA);
                sda.closeConnection();
                SmsFunctions sFunc = new SmsFunctions();

                foreach (DataRow dr in dtCA.Rows)
                {
                    if ((int)dr["ResponseWaitingcount"] == 0)
                    {
                        try
                        {
                            Guid campaignActivityId = (Guid)dr["CAID"];

                            UpdateCampaignActivityStatus(campaignActivityId);
                        }
                        catch (Exception ex)
                        {
                            logMe.Log("UpdateCampaignActivity_SMS - Execute", ex, TEMPEventLog.EventType.Exception);
                        }
                    }
                }

                #endregion

                logMe.Log("UpdateCampaignActivity_SMS - Execute", "UpdateCampaignActivity Uygulaması Bitti.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);
            }
            catch (Exception ex)
            {
                logMe.Log("UpdateCampaignActivity_SMS - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        //Kampanya aktivitesi altındaki tüm gönderimler tamamlandıktan sonra Kampanya Aktivitesinin statüsü Tamamlandı olarak setlenir.
        //Kampanya aktivitesine ait bütün response istekleri crmTempSmsResponseTable tablosundan silinir.
        //Kampanya maliyeti hesaplanır.
        private void UpdateCampaignActivityStatus(Guid campaignActivityID)
        {
            try
            {
                SetStateRequest stateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference("campaignactivity", campaignActivityID),
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(100000001)
                };
                SetStateResponse stateResponse = (SetStateResponse)orgService.Execute(stateRequest);

                //Kampanya aktivitesine ait bütün response istekleri crmTempSmsResponseTable tablosundan silinir.
                DeleteRequestFromTempTable(campaignActivityID);

                //Kampanya aktivitesine ait bütün smsler EuroMessageSmsTempTable tablosundan silinir.
                DeleteSmsFromTempTable(campaignActivityID);
            }
            catch (Exception ex)
            {
                logMe.Log("UpdateCampaignActivityStatus_SMS - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        //Bütün smslerin responseları alındıktan sonra temp tablodan bu kampanya aktivitesine ait kayıtlar silinir
        private void DeleteRequestFromTempTable(Guid campaignActivityID)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString); 
                string query = "DELETE FROM NEFCUSTOM_MSCRM..crmTempSmsResponseTable WHERE CampaignActivityID = @CAID";
                sda.ExecuteNonQuery(query, new SqlParameter("@CAID", campaignActivityID));
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("Execute - DeleteRequestFromTempTable_SMS", ex, TEMPEventLog.EventType.Exception);
            }
        }

        //Bütün smslerin işlemleri bittikten sonra temp tablodan bu kampanya aktivitesine ait kayıtlar silinir
        private void DeleteSmsFromTempTable(Guid campaignActivityID)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);
                string query = "DELETE FROM NEFCUSTOM_MSCRM..EuroMessageSmsTempTable WHERE ActivityId = @CAID";
                sda.ExecuteNonQuery(query, new SqlParameter("@CAID", campaignActivityID));
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("Execute - DeleteSmsFromTempTable", ex, TEMPEventLog.EventType.Exception);
            }
        }
    }
}
