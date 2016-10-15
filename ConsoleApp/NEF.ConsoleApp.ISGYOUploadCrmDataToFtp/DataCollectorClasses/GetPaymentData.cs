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
    public class GetPaymentData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                pay.new_paymentId AS PaymentId
	                                ,pay.new_name AS Name
	                                ,pay.TransactionCurrencyIdName
	                                ,sm.Value AS PaymentStatus
	                                ,pay.new_quoteid AS SalesId
	                                ,pay.new_quoteidName AS SalesName
	                                ,pay.new_amount AS PaidAmount
	                                ,pay.new_paymentamount AS PaymentAmount
	                                ,pay.new_balanceamount AS BalanceAmount
	                                ,pay.new_vnumber AS VoucherNo
	                                ,smVStatus.Value AS VoucherStatus
	                                ,pay.new_date AS VoucherDate
	                                ,pay.new_isvoucher AS IsVoucher
	                                ,smVType.Value AS VoucherType
	                                ,smType.Value AS PayementType
                                FROM
	                                new_payment AS pay (NOLOCK)
		                                JOIN
			                                StringMap AS sm (NOLOCK)
				                                ON
				                                sm.ObjectTypeCode=10040
				                                AND
				                                sm.AttributeName='statuscode'
				                                AND
				                                sm.AttributeValue=pay.StatusCode
		                                LEFT JOIN
			                                StringMap AS smType (NOLOCK)
				                                ON
				                                smType.ObjectTypeCode=10040
				                                AND
				                                smType.AttributeName='new_type'
				                                AND
				                                smType.AttributeValue=pay.new_type
		                                LEFT JOIN
			                                StringMap AS smVStatus (NOLOCK)
				                                ON
				                                smVStatus.ObjectTypeCode=10040
				                                AND
				                                smVStatus.AttributeName='new_vstatus'
				                                AND
				                                smVStatus.AttributeValue=pay.new_vstatus
		                                LEFT JOIN
			                                StringMap AS smVType (NOLOCK)
				                                ON
				                                smVType.ObjectTypeCode=10040
				                                AND
				                                smVType.AttributeName='new_vtype'
				                                AND
				                                smVType.AttributeValue=pay.new_vtype
                                WHERE
                                pay.new_quoteid 
                                IN
                                (
	                                SELECT 
		                                q.QuoteId
	                                FROM
	                                Quote AS q (NOLOCK)
		                                JOIN
			                                Product AS p (NOLOCK)
			                                ON
			                                q.new_productid=p.ProductId
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
	                                WHERE
	                                pro.new_projectId!='CB9CC4EF-1117-E011-817F-00123F4DA0F7'
	                                AND
	                                col.new_collaborateaccountId='B3A17FFD-C5B1-E411-80C7-005056A60603'
                                )
                                AND
	                                pay.new_collaborateaccountid='B3A17FFD-C5B1-E411-80C7-005056A60603'";

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

        public GetPaymentData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
