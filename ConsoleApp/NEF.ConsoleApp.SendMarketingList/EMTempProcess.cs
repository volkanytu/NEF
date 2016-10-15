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

namespace NEF.ConsoleApp.SendMarketingList
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
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivitySms", sdaCustom);
        }

        public bool Process(string _listId)
        {
            bool flag = false;

            try
            {
                listId = _listId;
                startDate = DateTime.Now;

                Console.WriteLine("EM-SMS Process Uygulaması Başladı..." + startDate.ToString());
                logMe.Log("EMTempProcess_SMS - Process", "EM Process Başladı:" + startDate.ToString(), TEMPEventLog.EventType.Info);

                if (IsListDynamic(listId))
                {
                    ColumnSet cols = new ColumnSet(new string[] { "query" });

                    Entity list = orgService.Retrieve("list", new Guid(_listId), cols);
                    string dynamicQuery = list.Attributes["query"].ToString();
                    var countQuery = dynamicQuery;

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Fetch XML alındı. Zaman:" + (DateTime.Now - startDate).ToString());
                    logMe.Log("EMTempProcess_SMS - Process", "Fetch XML Alındı:" + (DateTime.Now - startDate).ToString(), TEMPEventLog.EventType.Info);
                    var memberCountResult = FetchAll(countQuery);
                }
                else
                {
                    logMe.Log("EMTempProcess_SMS - Process", "EM Process Başladı1:" + startDate.ToString(), TEMPEventLog.EventType.Info);
                    InsertStaticListToTempTable();
                    logMe.Log("EMTempProcess_SMS - Process", "EM Process Başladı:2" + startDate.ToString(), TEMPEventLog.EventType.Info);
                }

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string sqlClearTemp = @"exec NEFCUSTOM_MSCRM..SP_clearTempSmsTable '{0}'";
                Console.WriteLine("Hata durumları güncelleniyor...");
                //sda.ExecuteNonQuery(string.Format(sqlClearTemp, caId.ToString()));
                //Console.WriteLine("Hata durumları güncellendi...    ");
                Console.WriteLine("İşlem Tamamlandı.Zaman:" + (DateTime.Now - startDate).ToString());
                logMe.Log("EMTempProcess_SMS - Process", "İşlem Tamamlandı :" + (DateTime.Now - startDate).ToString(), TEMPEventLog.EventType.Info);
                flag = true;

                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
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
            start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);
            end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0);
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
			                        List AS l
				                              
                                WHERE
                                    l.listId = @LID";


                SqlParameter[] parameters = new SqlParameter[]{
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
            start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);
            end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0);

            try
            {
                sda.openConnection(Globals.ConnectionString);
                logMe.Log("EMTempProcess_SMS - Process", "İşlem Tamamlandı1 :" + start.ToString("yyyy-MM-dd HH:mm:ss") + end.ToString("yyyy-MM-dd HH:mm:ss"), TEMPEventLog.EventType.Info);
                #region |   SQL QUERY   |
                string sqlQuery = @"INSERT 
                                    INTO 
                                        NEFCUSTOM_MSCRM..EuroMessageSmsTempTable
		                                    (CustomerId
		                                    ,ListId
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
