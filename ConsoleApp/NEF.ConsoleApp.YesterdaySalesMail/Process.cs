using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.YesterdaySalesMail
{
    class Process
    {
        internal static void Execute(SqlDataAccess sda, IOrganizationService service)
        {
            #region SQL QUERY
            string sqlQuery = @"SELECT
                                    PRJ.new_name AS Proje,
                                    PRD.new_blockidName AS Blok,
                                    PRD.new_homenumber AS [Daire No],
                                    PRD.new_unittypeidName AS [Ünite Tipi],
                                    PRD.ProductNumber AS [Daire Kimlik No],
                                    Q.CustomerIdName AS [Müşteri],
                                    CONVERT(varchar, CAST(Q.TotalAmount AS money), 1) AS [Satış Fiyatı],
                                    CONVERT(varchar, CAST(Q.new_prepaymentamount AS money), 1) AS [Kapora Tutarı],
                                    (SELECT
                                    CONVERT(varchar, CAST(SUM(
                                    PAY.new_paymentamount
                                    ) AS money), 1)
                                    
                                    FROM
                                    new_payment AS PAY
                                    WHERE
                                    PAY.new_quoteid=Q.QuoteId
                                    AND
                                    PAY.new_type=3
                                    ) AS [Peşinat],
                                    (SELECT
                                    CONVERT(varchar, CAST(SUM(
                                    PAY.new_paymentamount
                                    )AS money), 1)
                                    
                                    FROM
                                    new_payment AS PAY
                                    WHERE
                                    PAY.new_quoteid=Q.QuoteId
                                    AND
                                    PAY.new_type=1
                                    ) AS [Ara Ödeme],
                                    (SELECT
                                    CONVERT(varchar, CAST(SUM(
                                    PAY.new_paymentamount
                                    ) AS money), 1)
                                    
                                    FROM
                                    new_payment AS PAY
                                    WHERE
                                    PAY.new_quoteid=Q.QuoteId
                                    AND
                                    PAY.new_type=9
                                    ) AS [Kredi],
                                    
                                    Q.TransactionCurrencyIdName AS [Para Birimi],
                                    CONVERT(VARCHAR(10),Q.new_contractdate,104) AS [Sözleşme Tarihi],
                                    CONVERT(VARCHAR(10),Q.new_salesprocessdate,104) AS [Satış İşlem Tarihi],                                   
                                    SM.Value AS [Satışın Durumu]
                                    FROM
                                    Quote AS Q (NOLOCK)
                                    INNER JOIN
                                    QuoteDetail AS QD (NOLOCK)
                                    ON
                                    Q.QuoteId=QD.QuoteId
                                    INNER JOIN
                                    Product AS PRD (NOLOCK)
                                    
                                    ON
                                    QD.ProductId=PRD.ProductId
                                    INNER JOIN
                                    new_project AS PRJ (NOLOCK)
                                    ON
                                    PRD.new_projectid=PRJ.new_projectId
                                    INNER JOIN
                                    StringMap AS SM (NOLOCK)
                                    ON
                                    SM.ObjectTypeCode=1084
                                    AND
                                    SM.AttributeName='statuscode'
                                    AND
                                    SM.AttributeValue=q.StatusCode
                                    WHERE
                                    CAST(Q.new_salesprocessdate as date) = @date
                                    AND
                                    PRJ.new_name NOT LIKE '%Omerd%'
                                    AND
                                    PRJ.new_name NOT LIKE '%FENIX%'
                                    AND
                                    (
									Q.StatusCode=100000007--Kapora Alındı
									OR
									Q.StatusCode=100000009--Sözleşme Hazırlandı
									OR
									Q.StatusCode=2--Sözleşme İmzalandı
									OR
									Q.StatusCode=100000001--Muhasebeye Aktarıldı
									--OR
									--Q.StatusCode=6--İptal Edildi
									)
                                    ORDER BY Q.new_contractdate DESC";
            sqlQuery = string.Format(sqlQuery);
            DataTable dt = sda.getDataTable(sqlQuery, new SqlParameter[] { new SqlParameter("date", DateTime.Now.AddDays(-1).ToUniversalTime().Date) });
            #endregion SQL QUERY

            //if (dt.Rows.Count > 0)
            //{
            //    SendMailToResourceImpoter(dt, service);
            //}
            string totalCount = dt.Rows.Count.ToString();
            decimal TL = 0;
            decimal Dolar = 0;
            decimal Euro = 0;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Para Birimi"].ToString() == "TL")
                {
                    TL += Convert.ToDecimal(dr["Satış Fiyatı"], CultureInfo.InvariantCulture);

                }
                else if (dr["Para Birimi"].ToString() == "USD")
                {
                    Dolar += Convert.ToDecimal(dr["Satış Fiyatı"], CultureInfo.InvariantCulture);
                }
                else if (dr["Para Birimi"].ToString() == "Euro")
                {
                    Euro += Convert.ToDecimal(dr["Satış Fiyatı"], CultureInfo.InvariantCulture);
                }
            }

            SendMailToResourceImpoter(dt, totalCount, TL, Dolar, Euro, service);
        }
        public static void SendMailToResourceImpoter(DataTable dt_unprocessed, IOrganizationService service)
        {
            try
            {
                string errorHtml = string.Empty;
                StringBuilder sb = new StringBuilder();
                if (dt_unprocessed.Rows.Count != 0)
                {
                    errorHtml = ConvertDtToHtmlTable(dt_unprocessed);
                }

                sb.AppendLine(errorHtml);

                #region Create Email

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["addressused"] = "Mail_APP_Crm_Muhasebe@nef.com.tr";

                //Entity toParty = new Entity("activityparty");
                //toParty["addressused"] = "emrah.eroglu@innthebox.com";

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["subject"] = DateTime.Now.AddDays(-1).Date.ToString("dd/MM/yyyy") + " Tarihli Satışlar";
                email["description"] = sb.ToString();
                email["directioncode"] = true;
                Guid id = service.Create(email);

                #endregion

                #region Send Email
                var req = new SendEmailRequest
                {
                    EmailId = id,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };

                try
                {
                    var res = (SendEmailResponse)service.Execute(req);

                }
                catch (Exception ex)
                {

                }
                #endregion
            }
            catch (Exception ex)
            {


            }
        }

        public static void SendMailToResourceImpoter(DataTable dt_unprocessed, string totalCount, decimal TL, decimal Dolar, decimal Euro, IOrganizationService service)
        {
            try
            {
                string errorHtml = string.Empty;
                StringBuilder sb = new StringBuilder();
                if (dt_unprocessed.Rows.Count != 0)
                {
                    errorHtml = ConvertDtToHtmlTable(dt_unprocessed);
                }

                sb.AppendLine("Toplam " + totalCount + " Adet Satış " + String.Format("{0:0,0.00}", TL) + " TL ve " + String.Format("{0:0,0.00}", Dolar) + " $ ve " + String.Format("{0:0,0.00}", Euro) + " € 'dur");
                sb.AppendLine(errorHtml);

                #region Create Email

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["addressused"] = "Mail_APP_Crm_Muhasebe@nef.com.tr";

                //Entity toParty = new Entity("activityparty");
                //toParty["addressused"] = "emrah.eroglu@innthebox.com";

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["subject"] = DateTime.Now.AddDays(-1).Date.ToString("dd/MM/yyyy") + " Tarihli Satışlar";
                email["description"] = sb.ToString();
                email["directioncode"] = true;
                Guid id = service.Create(email);

                #endregion

                #region Send Email
                var req = new SendEmailRequest
                {
                    EmailId = id,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };

                try
                {
                    var res = (SendEmailResponse)service.Execute(req);

                }
                catch (Exception ex)
                {

                }
                #endregion
            }
            catch (Exception ex)
            {


            }
        }

        public static string ConvertDtToHtmlTable(DataTable targetTable)
        {
            string myHtmlFile = "";

            if (targetTable == null)
            {
                throw new System.ArgumentNullException("targetTable");
            }
            else
            {
                //Continue.
            }

            //Get a worker object.
            StringBuilder myBuilder = new StringBuilder();

            //Open tags and write the top portion.            
            myBuilder.Append("<table border='1' cellpadding='5' border-collapse='collapse'; cellspacing='0' ");
            myBuilder.Append("style='border: solid 1px black; font-size: x-small;'>");

            //Add the headings row.

            myBuilder.Append("<tr align='left' valign='top'>");

            foreach (DataColumn myColumn in targetTable.Columns)
            {
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append(myColumn.ColumnName);
                myBuilder.Append("</td>");
            }

            myBuilder.Append("</tr>");

            //Add the data rows.
            foreach (DataRow myRow in targetTable.Rows)
            {
                myBuilder.Append("<tr align='left' valign='top'>");

                foreach (DataColumn myColumn in targetTable.Columns)
                {
                    myBuilder.Append("<td align='left' valign='top'>");
                    myBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    myBuilder.Append("</td>");
                }

                myBuilder.Append("</tr>");
            }

            //Close tags.
            myBuilder.Append("</table>");

            //Get the string for return.
            myHtmlFile = myBuilder.ToString();

            return myHtmlFile;
        }

    }
}
