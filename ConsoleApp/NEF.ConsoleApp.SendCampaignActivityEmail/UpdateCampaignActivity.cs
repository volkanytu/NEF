using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class UpdateCampaignActivity
    {
        IOrganizationService orgService;
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public UpdateCampaignActivity()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "NEF.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
        }

        public void Execute()
        {
            try
            {
                logMe.Log("UpdateCampaignActivity - Execute", "UpdateCampaignActivity Uygulaması Başladı.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

                #region |   Get Campaign Activity   |

                #region |   Query   |

                string queryCA = @"SELECT	
                                    E.ActivityId CAID,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageMailErrorTable WHERE Error = 1 AND ActivityId = e.ActivityId
                                    )FailureCount,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageMailTempTable WHERE Error = 0 AND ActivityId = e.ActivityId AND StatusCode!=5
                                    )EmailCount,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageMailTempTable WHERE Error = 0 AND StatusCode = 100000003 AND ActivityId = e.ActivityId
                                    )NoSendEmailCount,
                                    (
                                        SELECT COUNT(0) FROM NEFCUSTOM_MSCRM..EuroMessageMailTempTable WHERE ActivityId = e.ActivityId AND Error = 0 AND ( StatusCode = 100000001 OR StatusCode = 1 )
                                    )ResponseWaitingCount
                                FROM
                                    NEFCUSTOM_MSCRM..EuroMessageMailTempTable E (NOLOCK),
                                    CampaignActivity Ca (NOLOCK)
                                WHERE
	                                ca.ActivityId = E.ActivityId 
                                --and 
	                               -- GETDATE() > Ca.new_euromessagereportlimit
                                GROUP BY
                                    E.ActivityId";

                #endregion |    Query   |

                sda.openConnection(Globals.ConnectionString);
                DataTable dtCA = sda.getDataTable(queryCA);
                sda.closeConnection();
                EmailFunctions eFunc = new EmailFunctions();

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
                            logMe.Log("UpdateCampaignActivity - Execute", ex, TEMPEventLog.EventType.Exception);
                        }
                    }
                }

                #endregion

                logMe.Log("UpdateCampaignActivity - Execute", "UpdateCampaignActivity Uygulaması Bitti.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);
            }
            catch (Exception ex)
            {
                logMe.Log("UpdateCampaignActivity - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        //Kampanya aktivitesi altındaki tüm gönderimler tamamlandıktan sonra Kampanya Aktivitesinin statüsü Tamamlandı olarak setlenir.
        //Kampanya aktivitesine ait bütün response istekleri crmTempMailResponseTable tablosundan silinir.
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

                //Kampanya aktivitesine ait bütün response istekleri crmTempMailResponseTable tablosundan silinir.
                DeleteRequestFromTempTable(campaignActivityID);

                //Kampanya aktivitesine ait bütün emailler EuroMessageTempTable tablosundan silinir.
                DeleteEmailFromTempTable(campaignActivityID);
            }
            catch (Exception ex)
            {
                logMe.Log("UpdateCampaignActivity - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        //Bütün emaillerin responseları alındıktan sonra temp tablodan bu kampanya aktivitesine ait kayıtlar silinir
        private void DeleteRequestFromTempTable(Guid campaignActivityID)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);
                string query = "DELETE FROM NEFCUSTOM_MSCRM..crmTempMailResponseTable WHERE CampaignActivityID = @CAID";
                sda.ExecuteNonQuery(query, new SqlParameter("@CAID", campaignActivityID));
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("UpdateCampaignActivity - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }

        //Bütün emaillerin işlemleri bittikten sonra temp tablodan bu kampanya aktivitesine ait kayıtlar silinir
        private void DeleteEmailFromTempTable(Guid campaignActivityID)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);
                string query = "DELETE FROM NEFCUSTOM_MSCRM..EuroMessageMailTempTable WHERE ActivityId = @CAID";
                sda.ExecuteNonQuery(query, new SqlParameter("@CAID", campaignActivityID));
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("UpdateCampaignActivity - Execute", ex, TEMPEventLog.EventType.Exception);
            }
        }
    }
}
