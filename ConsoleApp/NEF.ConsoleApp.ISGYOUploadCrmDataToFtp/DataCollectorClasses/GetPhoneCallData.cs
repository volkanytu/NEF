using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using NEF.Library.Business;
using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;

namespace NEF.ConsoleApp.ISGYOUploadCrmDataToFtp
{
    public class GetPhoneCallData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                pc.ActivityId
	                                ,pc.new_activitytopicidName AS ActivityTopic
	                                ,pc.new_activitystatusName AS ActivityStatus
	                                ,pc.new_activitystatusdetailName AS ActivityStatusDetail
	                                ,pc.PhoneNumber
	                                ,pc.Description
	                                ,pc.OwnerIdName
	                                ,pc.ScheduledStart
	                                ,pc.ActualEnd
	                                ,ap.PartyIdName AS CustomerIdName
	                                ,apFrom.PartyIdName AS CallerName
	                                ,smState.Value AS State
                                FROM
                                PhoneCall AS pc (NOLOCK)
	                                JOIN
		                                ActivityParty AS ap (NOLOCK)
			                                ON
			                                ap.ActivityId=pc.ActivityId
			                                AND
			                                ap.ParticipationTypeMask=2 --TO
	                                JOIN
		                                ActivityParty AS apFrom (NOLOCK)
			                                ON
			                                apFrom.ActivityId=pc.ActivityId
			                                AND
			                                apFrom.ParticipationTypeMask=1 --FROM
	                                JOIN
		                                StringMap AS smState (NOLOCK)
			                                ON
			                                smState.ObjectTypeCode=4210
			                                AND
			                                smState.AttributeName='statecode'
			                                AND
			                                smState.AttributeValue=pc.StateCode
                                WHERE
                                pc.OwningBusinessUnit='4D2ABB3A-C2B1-E411-80C7-005056A60603'";

            #endregion

            try
            {
                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    XLWorkbook wb = new XLWorkbook();

                    IXLWorksheet ws = wb.Worksheets.Add(dt, _dataType.ToString());

                    wb.SaveAs(@Environment.CurrentDirectory + @"\files\" + _dataType.ToString() + ".xlsx");

                }
                returnValue.Success = true;
                returnValue.Result = string.Format("[{0}] adet data gönderildi.[{1}]", dt.Rows.Count.ToString(), _dataType.ToString());
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public GetPhoneCallData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
