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
            conn = new SqlConnection();
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

                Console.WriteLine("SMS oluşturma işlemleri başladı.");

                try
                {

                    #region |   Process Marketing Lists    |
                    //Kampanya aktivitesine ait listeler çekilir.
                    DataTable dtMarketingLists = GetMarketingList();

                    if (dtMarketingLists != null && dtMarketingLists.Rows.Count > 0)
                    {
                        foreach (DataRow drList in dtMarketingLists.Rows)
                        {
                            try
                            {
                                marketingListId = new Guid(drList["ListId"].ToString());
                                emt = new EMTempProcess();
                                bool result = emt.Process( marketingListId.ToString());

                                //// Result true dönerse kampanya aktivitesinin durumu değiştirilir.
                                //if (result)
                                //{
                                //    SetStateRequest stateRequest = new SetStateRequest()
                                //    {
                                //        EntityMoniker = new EntityReference("campaignactivity", campaignActivityId),
                                //        State = new OptionSetValue(0),
                                //        Status = new OptionSetValue(100000003)
                                //    };
                                //    SetStateResponse stateResponse = (SetStateResponse)orgService.Execute(stateRequest);
                                //}
                            }
                            catch (Exception ex)
                            {
                                logMe.Log("CreateSMS", ex, TEMPEventLog.EventType.Exception, ":List", drList["ListId"].ToString());
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
                sda.openConnection(Globals.ConnectionString); 
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
