using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class EMTempProcess
    {
        IOrganizationService orgService;
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        DateTime startDate;
        TEMPEventLog logMe;

        string caId;
        string listId;
        string ownerId;
        DateTime start;
        DateTime end;

        public EMTempProcess()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
        }

        public bool Process(string _caId, string _listId, string _ownerId, DateTime _start, DateTime _end)
        {
            bool flag = false;

            try
            {
                caId = _caId;
                ownerId = _ownerId;
                start = _start;
                end = _end;
                listId = _listId;
                startDate = DateTime.Now;

                Console.WriteLine("EM Process Uygulaması Başladı..." + startDate.ToString());
                logMe.Log("EMTempProcess - Process", "EM Process Başladı:" + startDate.ToString(), TEMPEventLog.EventType.Info);

                if (IsListDynamic(listId))
                {
                    ColumnSet cols = new ColumnSet(new string[] { "query" });

                    Entity list = orgService.Retrieve("list", new Guid(_listId), cols);
                    string dynamicQuery = list.Attributes["query"].ToString();
                    var countQuery = dynamicQuery;

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Fetch XML alındı. Zaman:" + (DateTime.Now - startDate).ToString());
                    logMe.Log("EMTempProcess - Process", "Fetch XML Alındı:" + (DateTime.Now - startDate).ToString(), TEMPEventLog.EventType.Info);
                    var memberCountResult = FetchAll(countQuery);
                }
                else
                {
                    InsertStaticListToTempTable();
                }


                sda.openConnection(Globals.ConnectionString);
                string sqlClearTemp = @"exec NEFCUSTOM_MSCRM..sp_ClearTempEMailTable '{0}'";
                Console.WriteLine("Hata durumları güncelleniyor...");
                sda.ExecuteNonQuery(string.Format(sqlClearTemp, caId.ToString()));
                Console.WriteLine("Hata durumları güncellendi...    ");
                Console.WriteLine("İşlem Tamamlandı.Zaman:" + (DateTime.Now - startDate).ToString());
                logMe.Log("EMTempProcess - Process", "İşlem Tamamlandı :" + (DateTime.Now - startDate).ToString(), TEMPEventLog.EventType.Info);
                flag = true;
                sda.closeConnection();
            }
            catch (Exception)
            {            
                throw;
            }

            return flag;
        }

        public RetrieveMultipleResponse Fetch(string fetchQuery)
        {
            RetrieveMultipleRequest req = new RetrieveMultipleRequest();
            FetchExpression fetch = new FetchExpression(fetchQuery);
            req.Query = fetch;

            RetrieveMultipleResponse resp = (RetrieveMultipleResponse)orgService.Execute(req);

            return resp;
        }

        public RetrieveMultipleResponse FetchAll(string sFetchXml)
        {
            RetrieveMultipleResponse allRecords = null;
            RetrieveMultipleResponse currentPage = null;

            int page = 1;
            bool bComplete = false;
            Thread[] th = new Thread[3];

            while (!bComplete)
            {
                XmlDocument oFetchXml = new XmlDocument();
                oFetchXml.LoadXml(sFetchXml);
                XmlNode oFetchNode = oFetchXml.SelectSingleNode("/fetch");
                if (oFetchNode.Attributes["page"] == null)
                {
                    XmlAttribute oPageAttribute = oFetchXml.CreateAttribute("page");
                    oPageAttribute.Value = page.ToString();
                    oFetchNode.Attributes.Append(oPageAttribute);
                }
                else
                    oFetchNode.Attributes["page"].Value = page.ToString();

                currentPage = Fetch(oFetchXml.InnerXml);

                if (page == 1)
                    allRecords = currentPage;
                else
                {
                    foreach (Entity current in currentPage.EntityCollection.Entities)
                        allRecords.EntityCollection.Entities.Add(current);
                }

                if (currentPage.EntityCollection.MoreRecords)
                {
                    //if (page == 5) { bComplete = true; }
                    page++;
                }
                else
                    bComplete = true;

                TempProcess tp = new TempProcess(ref sda, caId, ownerId, start, end, listId, currentPage, (page + 4));
                tp.Process();
                //page++;
                Console.SetCursorPosition(0, 4);
                Console.WriteLine("Sayfa Sayısı:" + page.ToString());
            }
            return allRecords;
        }

        bool IsListDynamic(string listId)
        {
            bool returnValue = false;

            try
            {
                sda.openConnection(Globals.ConnectionString);

                string sqlQuery = @"SELECT
	                                l.Type
                                FROM
	                                CampaignActivity AS ca
		                                JOIN
			                                CampaignActivityItem AS cai
				                                ON
				                                ca.ActivityId=cai.CampaignActivityId
				                                AND
				                                cai.ItemObjectTypeCode=4300 --List
		                                JOIN
			                                List AS l
				                                ON
				                                cai.ItemId=l.ListId
                                WHERE
	                                cai.CampaignActivityId = @CAID and
                                    l.listId = @LID";


                SqlParameter[] parameters = new SqlParameter[]{
                                                new SqlParameter("@CAID",caId),
                                                new SqlParameter("@LID",listId)
                                             };
                returnValue = (bool)sda.ExecuteScalar(sqlQuery, parameters);

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnValue;
        }

        void InsertStaticListToTempTable()
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);

                #region |   SQL QUERY   |
                string sqlQuery = @"INSERT 
                                    INTO 
                                        NEFCUSTOM_MSCRM..EuroMessageMailTempTable
		                                    (CustomerId
		                                    ,ListId
		                                    ,ActivityId
		                                    ,OwnerId
		                                    ,EMStatusCode
		                                    ,StatusCode
		                                    ,StateCode
		                                    ,ScheduledStart
		                                    ,ScheduledEnd
		                                    ,Subject
		                                    ,Error
                                    )
                                    SELECT
	                                    l.EntityId AS CustomerId
	                                    ,l.ListId
	                                    ,'" + caId.ToString() + @"' AS ActivityId
	                                    ,'" + ownerId.ToString() + @"' AS OwnerId
	                                    ,2 AS EMStatusCode
	                                    ,1 AS StatusCode
	                                    ,0 AS StateCode
	                                    ,@startdate AS ScheduledStart
	                                    ,@enddate AS ScheduledEnd
	                                    ,'' AS Subject
	                                    ,0 AS Error
                                    FROM
	                                    ListMember AS l (NOLOCK)
                                    WHERE
	                                    l.ListId='" + listId.ToString() + @"'
                                    AND
	                                    l.EntityType=2";
                #endregion |   SQL QUERY   |

                sda.ExecuteScalar(sqlQuery, new SqlParameter("@startdate", Convert.ToDateTime(start.ToString("yyyy-MM-dd HH:mm:ss"))), new SqlParameter("@enddate", Convert.ToDateTime(end.ToString("yyyy-MM-dd HH:mm:ss"))));

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
