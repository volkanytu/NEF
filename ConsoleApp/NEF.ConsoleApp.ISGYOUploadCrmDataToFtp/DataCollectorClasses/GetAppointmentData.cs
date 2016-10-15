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
    public class GetAppointmentData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                    app.ActivityId
	                                    ,app.new_activitytopicidName AS ActivityTopic
	                                    ,app.new_activitystatusName AS ActivityStatus
	                                    ,app.new_activitystatusdetailName AS ActivityStatusDetail
	                                    ,sm.Value AS PresentationType
	                                    ,app.new_salesofficeidName AS SalesOffice
	                                    ,app.OwnerIdName
	                                    ,app.ScheduledStart
	                                    ,app.ActualEnd
	                                    ,ap.PartyIdName AS CustomerIdName
	                                    ,smState.Value AS State
	                                    ,ap.PartyId AS CustomerId
                                    FROM
                                    Appointment AS app (NOLOCK)
	                                    JOIN
		                                    ActivityParty AS ap (NOLOCK)
			                                    ON
			                                    ap.ActivityId=app.ActivityId
			                                    AND
			                                    ap.ParticipationTypeMask=5 --Required Attendies
	                                    LEFT JOIN
		                                    StringMap AS sm (NOLOCK)
			                                    ON
			                                    sm.ObjectTypeCode=4201
			                                    AND
			                                    sm.AttributeName='new_presentationtype'
			                                    AND
			                                    sm.AttributeValue=app.new_presentationtype
	                                    JOIN
		                                    StringMap AS smState (NOLOCK)
			                                    ON
			                                    smState.ObjectTypeCode=4201
			                                    AND
			                                    smState.AttributeName='statecode'
			                                    AND
			                                    smState.AttributeValue=app.StateCode
                                    WHERE
                                    app.OwningBusinessUnit='4D2ABB3A-C2B1-E411-80C7-005056A60603'";

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

        public GetAppointmentData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
