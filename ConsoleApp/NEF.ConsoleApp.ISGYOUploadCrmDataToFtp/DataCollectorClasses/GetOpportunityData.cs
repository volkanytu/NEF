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
    public class GetOpportunityData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                    opp.OpportunityId
	                                    ,opp.Name
	                                    ,opp.CustomerId
	                                    ,opp.CustomerIdName
	                                    ,opp.OwnerIdName
	                                    ,sm.Value AS Status
	                                    ,opp.new_relatedactivitystatusidName AS ActivityStatus
	                                    ,opp.CreatedOn
	                                    ,opp.ActualCloseDate
                                    FROM
                                    Opportunity AS opp (NOLOCK)
	                                    JOIN
		                                    StringMap AS sm (NOLOCK)
			                                    ON
			                                    sm.ObjectTypeCode=3
			                                    AND
			                                    sm.AttributeName='statuscode'
			                                    AND
			                                    sm.AttributeValue=opp.StatusCode
                                    WHERE
                                    opp.OwningBusinessUnit='4D2ABB3A-C2B1-E411-80C7-005056A60603'";

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

        public GetOpportunityData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }

    }
}
