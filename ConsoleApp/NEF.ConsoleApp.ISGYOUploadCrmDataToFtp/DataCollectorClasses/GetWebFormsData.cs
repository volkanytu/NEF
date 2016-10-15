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
    class GetWebFormsData : ICollaborateData
    {
        CollaborateDataType _dataType;
         public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                             p.new_webformId AS Id
	                             
	                            ,p.new_name As Name
	                            ,pro.new_projectId AS ProjectId
	                            ,pro.new_name AS ProjectIdName
	                            ,sm.Value AS Status,
	                            new_contactid AS ContactId,
								new_contactidname AS ContactIdName,
								new_emailadress AS EmailAddress,
								new_firstname AS FirstName,
								new_lastname AS LastName,
								new_message AS Message,
								new_mobilephone AS MobilePhone,
								new_channelofawarenessid AS ChannelOfAwarenessId,
								new_channelofawarenessidname AS ChannelOfAwarenessIdName,
								new_isemailadress AS IsEmailAddress,
								new_istakeinfo AS IsTakeInfo
                            FROM
                            new_webform AS p (NOLOCK)
	                            JOIN
		                            new_project AS pro (NOLOCK)
			                            ON
			                            p.new_projectid=pro.new_projectId
	                            JOIN
		                            new_projectsalescollaborate AS pcol (NOLOCK)
			                            ON
			                            pro.new_projectId=pcol.new_projectid
	                            JOIN
		                            new_collaborateaccount AS col (NOLOCK)
			                            ON
			                            pcol.new_accountid=col.new_collaborateaccountId
	                            JOIN
		                            StringMap AS sm (NOLOCK)
			                            ON
			                            sm.ObjectTypeCode=10058
			                            AND
			                            sm.AttributeName='statuscode'
			                            AND
			                            sm.AttributeValue=p.StatusCode
                            WHERE
                            pro.new_projectId!='CB9CC4EF-1117-E011-817F-00123F4DA0F7'
                            AND
                            col.new_collaborateaccountId='B3A17FFD-C5B1-E411-80C7-005056A60603'";

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

         public GetWebFormsData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
