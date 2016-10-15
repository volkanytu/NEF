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
    public class CreateSms
    {
        IOrganizationService orgService;
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public CreateSms()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivitySms", sdaCustom);
        }

        internal void Execute()
        {
            DateTime startDate;
            DateTime endDate;
            Guid campaignActivityId;
            Guid ownerId;
            Guid marketingListId;
            EMTempProcess emt;

            try
            {
                Console.WriteLine("Sms gönderim uygulaması çalıştı.");
                logMe.Log("CreateSms - Execute", "Sms oluşturma uygulaması başladı.", TEMPEventLog.EventType.Info);

                #region Periyodik gönderimler dağıtıldı durumuna çekilir.
                SmsFunctions sFunc = new SmsFunctions();
                sFunc.TestCampaignActivity();
                // sFunc.SetCampaignActivityDistributed();
                #endregion

                #region |   Get Campaign Activity   |
                // StatusCode değeri 'Dağıtıldı' ve Kanal Tipi 'SMS' olan kampanya aktiviteleri alınıyor.
                string queryCActivity = @"
                                    SELECT
	                                    CA.ActivityId CAID,
                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledend) CASEND,
                                        DATEADD(HOUR,DATEDIFF(HOUR,GETUTCDATE(),GETDATE()),CA.scheduledstart) CAS,
	                                    CA.[Subject] CS,
                                        CA.CreatedBy 'owner'                                                 
                                    FROM
	                                    CampaignActivity CA (NOLOCK)
                                    WHERE
	                                    CA.StatusCode = 6
                                        AND
	                                    CA.ChannelTypeCode = 3";
                sda.openConnection(Globals.ConnectionString);
                DataTable dtCActivity = sda.getDataTable(queryCActivity);
                sda.closeConnection();
                Console.WriteLine("Gönderilmeyi bekleyen kampanya aktiviteleri alındı.");
                logMe.Log("CreateSms - Execute", "Gönderilmeyi bekleyen kampanya aktiviteleri alındı. Adet: " + dtCActivity.Rows.Count, TEMPEventLog.EventType.Info);
                #endregion |   Get Campaign Activity   |

                foreach (DataRow dr in dtCActivity.Rows)
                {
                    Console.WriteLine("SMS oluşturma işlemleri başladı.");

                    try
                    {
                        startDate = (DateTime)dr["CAS"];
                        endDate = (DateTime)dr["CASEND"];
                        campaignActivityId = (Guid)dr["CAID"];
                        ownerId = (Guid)dr["owner"];

                        #region |   Process Marketing Lists    |
                        //Kampanya aktivitesine ait listeler çekilir.
                        DataTable dtMarketingLists = GetMarketingList(campaignActivityId);

                        if (dtMarketingLists != null && dtMarketingLists.Rows.Count > 0)
                        {
                            foreach (DataRow drList in dtMarketingLists.Rows)
                            {
                                try
                                {
                                    marketingListId = new Guid(drList["ListId"].ToString());
                                    emt = new EMTempProcess();
                                    bool result = emt.Process(campaignActivityId.ToString(), marketingListId.ToString(), ownerId.ToString(), startDate, endDate);

                                    // Result true dönerse kampanya aktivitesinin durumu değiştirilir.
                                    if (result)
                                    {
                                        SetStateRequest stateRequest = new SetStateRequest()
                                        {
                                            EntityMoniker = new EntityReference("campaignactivity", campaignActivityId),
                                            State = new OptionSetValue(0),
                                            Status = new OptionSetValue(100000000)
                                        };
                                        SetStateResponse stateResponse = (SetStateResponse)orgService.Execute(stateRequest);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logMe.Log("CreateSMS", ex, TEMPEventLog.EventType.Exception, "CampaignActivity", campaignActivityId.ToString());
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable GetMarketingList(Guid campaignActivityID)
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
