using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivitySms
{
    public class TempProcess
    {
        IOrganizationService orgService;
        SqlDataAccess sda;
        string listId;
        string caId;
        string ownerId;
        DateTime ScheduledStart;
        DateTime ScheduledEnd;
        RetrieveMultipleResponse resp;
        int rowNo;
        bool no24hour;

        public TempProcess(ref SqlDataAccess _sda, string _caId, string _ownerId, DateTime _start, DateTime _end, string _listId, RetrieveMultipleResponse _resp, int _rowNo)
        {
            orgService = MSCRM.AdminOrgService;
            sda = _sda;
            listId = _listId;
            resp = _resp;
            rowNo = _rowNo;
            caId = _caId;
            ownerId = _ownerId;
            ScheduledStart = _start;
            ScheduledEnd = _end;
        }

        public void Process()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            DataCollection<Entity> dataCollection = ((RetrieveMultipleResponse)resp).EntityCollection.Entities;

            Console.SetCursorPosition(0, rowNo);

            if (dataCollection != null && dataCollection.Count > 0)
            {
                int i = 0;
                Guid activityId = new Guid(caId);
                string query = string.Empty;
                bool is24HourControl = no24hour;

                sda.openConnection(Globals.ConnectionString);

                foreach (Entity entityVal in dataCollection)
                {
                    bool error = false;

                    string sqlQuery = string.Empty;

                    #region |INSERT TO TEMP TABLE|
                    query += @" INSERT 
	                    INTO 
                            NEFCUSTOM_MSCRM..EuroMessageSmsTempTable
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
	                    ) ";
                    query += "VALUES( "
                    + "'" + entityVal["contactid"].ToString() + @"',"
                    + "'" + listId + @"',"
                    + "'" + activityId.ToString() + "',"
                    + "'" + ownerId.ToString() + "',"
                    + "2,"
                    + "1,"
                    + "0,"
                    + "'" + ScheduledStart.ToString("yyyy-MM-dd HH:mm:ss") + "',"
                    + "'" + ScheduledEnd.ToString("yyyy-MM-dd HH:mm:ss") + "',"
                    + "'',"
                        //+ (error ? "1" : "0") + (dataCollection.Count == (i + 1) ? "" : " UNION ALL ");
                    + (error ? "1" : "0)");

                    #endregion

                    Console.WriteLine("Sayaç: " + i.ToString() + " / " + dataCollection.Count.ToString());
                    i++;
                }
                sda.ExecuteNonQuery(query);
            }
        }
    }
}
