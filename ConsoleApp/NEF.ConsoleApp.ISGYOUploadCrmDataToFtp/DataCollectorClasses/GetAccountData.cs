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
    public class GetAccountData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |


            string sqlQuery = @"SELECT DISTINCT
	                                c.AccountId
	                                ,c.Name
	                                ,c.new_taxnumber AS TaxNumber
	                                ,c.Telephone1
	                                ,c.EMailAddress1 AS EmailAddress
	                                ,c.new_addresscountryidName AS Country
	                                ,c.new_addresscityidName AS City
	                                ,c.new_addresstownidName AS Town
	                                ,c.new_addressdistrictidName AS District
	                                ,c.new_addressdetail AS AddressDetail
                                FROM
									Account AS c (NOLOCK)
                                JOIN
									Quote AS q (NOLOCK)
								ON 
									q.CustomerId = c.AccountId
								JOIN
		                            new_project AS pro (NOLOCK)
			                    ON
			                        q.new_projectid=pro.new_projectId
			                    JOIN
		                            new_projectsalescollaborate AS pcol (NOLOCK)
			                    ON
			                        pro.new_projectId=pcol.new_projectid
			                    JOIN
		                            new_collaborateaccount AS col (NOLOCK)
			                    ON
			                        pcol.new_accountid=col.new_collaborateaccountId
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

        public GetAccountData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
