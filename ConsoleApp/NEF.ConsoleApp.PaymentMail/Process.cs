using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.PaymentMail
{
    class Process
    {
        internal static void Execute(SqlDataAccess sda, IOrganizationService service)
        {
            #region SQL QUERY
            string sqlQuery = @"SELECT
                               P.new_quoteidName [İlgili Satış]
                               ,Q.new_projectidName [İlgili Proje]
                               ,Q.new_productidName [İlgili Konut]
                               ,Q.CustomerIdName [Müşteri]                              
                               ,CONVERT(VARCHAR(10),
							   CAST(CONVERT(datetime, 
                               SWITCHOFFSET(CONVERT(datetimeoffset, 
                               P.new_date), 
                               DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as date),104) AS [Vade Tarihi]
                               ,CONVERT(varchar, CAST(P.new_paymentamount AS money), 1) AS [Ödenmesi Gereken Tutar]
                               ,P.new_paymentamount AS [Ödenmesi Gereken Tutar Decimnal]	
                               ,P.TransactionCurrencyIdName [Para Birimi]                              
                              ,SM.Value AS [Satışın Durumu]
                               FROM
                               new_payment P (NOLOCK)
                               JOIN
                               Quote Q (NOLOCK)
                               ON
                               P.new_quoteid=Q.QuoteId
                               JOIN
                               StringMap AS SM (NOLOCK)
                               ON
                               SM.ObjectTypeCode=1084
                               AND
                               SM.AttributeName='statuscode'
                               AND
                               SM.AttributeValue=Q.StatusCode
                               WHERE
                               P.new_isvoucher=1 AND P.StateCode=0
                               AND
                               Q.StatusCode IN (2,100000001,100000002,100000009)
                               AND
                                P.new_collaborateaccountid != @CollaborateAccountId
                               AND
                               CAST(CONVERT(datetime, 
                               SWITCHOFFSET(CONVERT(datetimeoffset, 
                               P.new_date), 
                               DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as date)					   
                               BETWEEN							   
                              @date1
							   AND
                              @date2

                               ";
            sqlQuery = string.Format(sqlQuery);
            DataTable dt;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                dt = sda.getDataTable(sqlQuery, new SqlParameter[] { new SqlParameter("date1", DateTime.Now.AddDays(-2).Date), new SqlParameter("date2", DateTime.Now.Date),
                                                                     new SqlParameter("CollaborateAccountId", Globals.IsGayrimenkulYatirimOrtakligiId)});
            }
            else
            {
                dt = sda.getDataTable(sqlQuery, new SqlParameter[] { new SqlParameter("date1", DateTime.Now.Date), new SqlParameter("date2", DateTime.Now.Date),
                                                                     new SqlParameter("CollaborateAccountId", Globals.IsGayrimenkulYatirimOrtakligiId)});
            }

            #endregion SQL QUERY

            if (dt.Rows.Count > 0)
            {
                string totalCount = dt.Rows.Count.ToString();
                decimal TL = 0;
                decimal Dolar = 0;
                decimal Euro = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Para Birimi"].ToString() == "TL")
                    {
                        TL += (decimal)dr["Ödenmesi Gereken Tutar Decimnal"];
                    }
                    else if (dr["Para Birimi"].ToString() == "USD")
                    {
                        Dolar += (decimal)dr["Ödenmesi Gereken Tutar Decimnal"];
                    }
                    else if (dr["Para Birimi"].ToString() == "Euro")
                    {
                        Euro += (decimal)dr["Ödenmesi Gereken Tutar Decimnal"];
                    }
                }
                dt.Columns.Remove("Ödenmesi Gereken Tutar Decimnal");
                SendMailToResourceImpoter(dt, totalCount, TL, Dolar, Euro, service);

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

                sb.AppendLine("Toplam " + totalCount + " Adet Senet " + String.Format("{0:0,0.00}", TL) + " TL ve " + String.Format("{0:0,0.00}", Dolar) + " $ ve " + String.Format("{0:0,0.00}", Euro) + " € senet bulunmaktadır.");
                sb.AppendLine(errorHtml);

                #region Create Email

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["addressused"] = "Mail_Finansman@nef.com.tr";

                Entity ccParty = new Entity("activityparty");
                ccParty["addressused"] = "mete.altin@nef.com.tr";

                EntityCollection bccCollection = new EntityCollection();
                Entity bccParty1 = new Entity("activityparty");
                Entity bccParty2 = new Entity("activityparty");
                Entity bccParty3 = new Entity("activityparty");
                bccParty1["addressused"] = "erkan.ozvar@nef.com.tr";
                bccParty2["addressused"] = "erhan.serter@nef.com.tr";
                //bccParty3["addressused"] = "aleksi.komorosano@nef.com.tr";
                bccCollection.Entities.Add(bccParty1);
                bccCollection.Entities.Add(bccParty2);
                //bccCollection.Entities.Add(bccParty3);

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["cc"] = new Entity[] { ccParty };
                email["bcc"] = bccCollection;
                email["subject"] = "Ödeme Vadesi Bugün Olan Senetler";
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
            myBuilder.Append("<table border='1' cellpadding='5' cellspacing='0' ");
            myBuilder.Append("style='border: solid 1px Silver; font-size: x-small;'>");

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
