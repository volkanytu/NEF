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
    public class GetSalesData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT 
	                                q.QuoteId
	                                ,q.Name
	                                ,CASE 
		                                WHEN q.new_paymentplan=0 THEN 'Kredili Ödeme Planı'
		                                WHEN q.new_paymentplan=1 THEN 'Kredisiz Ödeme Planı'
	                                END AS PaymentPlan
	                                ,q.TotalLineItemAmount AS ItemAmount
	                                ,Sm.Value SalesStatus
	                                ,q.new_contractprocessdate AS ContractProcessDate
	                                ,q.new_contractdate AS ContractDate
	                                ,q.CreatedOn
	                                ,q.new_persquaremeter AS PerSquareMeter
	                                ,q.new_taxamount AS KDVAmount
	                                ,q.new_taxofstampamount AS TaxOfStampAmount
	                                ,q.new_amountwithtax AS AmountWithKdv
	                                ,q.new_totalsalesamountbytax AS TotalSalesAmount
	                                ,q.new_prepaymentamount AS PrePaymentAmount
	                                ,smPrePayment.Value AS PrePaymentType
	                                ,q.new_financialaccountidName AS FinancialAccountCode
	                                ,q.new_contractnumber AS ContractNumber
	                                ,q.CustomerId
	                                ,q.CustomerIdName
	                                ,q.OwnerId
	                                ,q.OwnerIdName
	                                ,q.new_productid
	                                ,q.new_productidName
                                    ,CASE WHEN q.CustomerIdType = 1 THEN 'Account'
	                                WHEN q.CustomerIdType = 2 THEN 'Contact'
	                                END AS 'CustomerType'
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
	                                LEFT JOIN
		                                StringMap AS smPrePayment (NOLOCK)
			                                ON
			                                smPrePayment.ObjectTypeCode=1084
			                                AND
			                                smPrePayment.AttributeName='new_prepaymenttype'
			                                AND
			                                smPrePayment.AttributeValue=q.new_prepaymenttype
	                                JOIN
		                                StringMap AS sm (NOLOCK)
			                                ON
			                                sm.ObjectTypeCode=1084
			                                AND
			                                sm.AttributeName='statuscode'
			                                AND
			                                sm.AttributeValue=q.StatusCode
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

        public GetSalesData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
