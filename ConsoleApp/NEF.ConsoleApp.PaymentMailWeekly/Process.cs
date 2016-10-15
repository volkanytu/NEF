using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.PaymentMailWeekly
{
    class Process
    {
        internal static void ExecuteFilterUser(SqlDataAccess sda, IOrganizationService service)
        {
            #region SQL QUERY
            string sqlQuery = @"SELECT
                              Q.new_projectidName [İlgili Proje]
							   ,Q.new_productidName [İlgili Konut]
							   ,Q.CustomerIdName [Müşteri]
                               ,P.new_quoteidName [İlgili Satış]
                               ,CONVERT(varchar, CAST(P.new_paymentamount AS money), 1) AS [Ödenmesi Gereken Tutar]
							   , 
							   CONVERT(VARCHAR(10),
							   CAST(CONVERT(datetime, 
                               SWITCHOFFSET(CONVERT(datetimeoffset, 
                               P.new_date), 
                               DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as date),104) AS [Vade Tarihi]
							   ,CASE Q.CustomerIdType WHEN '1' THEN A.OwnerIdName WHEN '2' THEN C.OwnerIdName END AS [Müşteri Satış Temsilcisi]
							   ,CASE Q.CustomerIdType WHEN '1' THEN '-' WHEN '2' THEN C.MobilePhone END AS [Cep Telefonu]
							   ,CONVERT(varchar, CAST(Q.TotalAmountLessFreight AS money), 1) AS [İndirimli Konut Fiyatı]
							   ,ISNULL((SELECT Value FROM StringMap WHERE ObjectTypeCode=10040 AND AttributeName='new_itype' AND AttributeValue=P.new_itype) ,'-') AS [Ödeme Tipi]
							   ,ISNULL(CASE Q.CustomerIdType WHEN '1' THEN '-' WHEN '2' THEN C.new_cellphonenumber END,'-') AS [Tel. No. Cep]
							   ,ISNULL(CASE Q.CustomerIdType WHEN '1' THEN '-' WHEN '2' THEN C.new_telephone2number END,'-') AS [Tel. No. 2]
							   ,ISNULL((SELECT Value FROM StringMap WHERE ObjectTypeCode=10040 AND AttributeName='new_type' AND AttributeValue=P.new_type) ,'-') AS [Ödeme Türü]                            
                               ,Q.OwnerIdName [Satış Temsilcisi]
                               ,P.TransactionCurrencyIdName [Para Birimi]
							   ,P.new_paymentamount AS [Ödenmesi Gereken Tutar Decimnal]							   							   
							   ,CASE Q.CustomerIdType WHEN '1' THEN A.OwnerId WHEN '2' THEN C.OwnerId END AS [Müşteri Satış Temsilcisi Id]
                               FROM
                               new_payment P (NOLOCK)
                               JOIN
                               Quote Q (NOLOCK)
                               ON
                               P.new_quoteid=Q.QuoteId
							   LEFT JOIN
							   CONTACT C (NOLOCK)
							   ON
							   C.ContactId=Q.CustomerId
							   LEFT JOIN
							   ACCOUNT A (NOLOCK)
							   ON
							   A.AccountId=Q.CustomerId
							   WHERE
                               P.new_isvoucher=1
                               AND
                               Q.StatusCode!=6
                               AND
							   CAST(CONVERT(datetime, 
                               SWITCHOFFSET(CONVERT(datetimeoffset, 
                               P.new_date), 
                               DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as date)					   
                               BETWEEN							   
                              @date1
							   AND
                              @date2							
							 order by [Vade Tarihi]
                               ";
            sqlQuery = string.Format(sqlQuery);
            DataTable dt = sda.getDataTable(sqlQuery, new SqlParameter[] { new SqlParameter("date1", DateTime.Now.Date), new SqlParameter("date2", DateTime.Now.AddDays(6).Date) });

            #endregion SQL QUERY

            if (dt.Rows.Count > 0)
            {
                List<string> userList = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (!userList.Contains(dr["Müşteri Satış Temsilcisi Id"].ToString()))
                    {
                        userList.Add(dr["Müşteri Satış Temsilcisi Id"].ToString());
                    }
                }
                foreach (string userId in userList)
                {
                    DataTable tblFiltered = dt.AsEnumerable()
         .Where(row => row.Field<Guid>("Müşteri Satış Temsilcisi Id").ToString() == userId)
         .OrderByDescending(row => row.Field<String>("Vade Tarihi"))
         .CopyToDataTable();
                    string totalCount = tblFiltered.Rows.Count.ToString();
                    decimal TL = 0;
                    decimal Dolar = 0;
                    decimal Euro = 0;

                    foreach (DataRow dr in tblFiltered.Rows)
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
                    tblFiltered.Columns.Remove("Müşteri Satış Temsilcisi Id");
                    tblFiltered.Columns.Remove("Ödenmesi Gereken Tutar Decimnal");
                    SendMailToResourceImpoter(tblFiltered, new Guid(userId), totalCount, TL, Dolar,Euro, service);
                    
                }



            }
        }
        public static void SendMailToResourceImpoter(DataTable dt_unprocessed, Guid userId, string totalCount, decimal TL, decimal Dolar,decimal Euro, IOrganizationService service)
        {
            try
            {
                string errorHtml = string.Empty;
                StringBuilder sb = new StringBuilder();
                if (dt_unprocessed.Rows.Count != 0)
                {
                    errorHtml = ConvertDtToHtmlTable(dt_unprocessed);
                }

                sb.AppendLine("Bu hafta Toplam " + totalCount + " Adet Senet " + String.Format("{0:0,0.00}", TL) + " TL ve " + String.Format("{0:0,0.00}", Dolar) + " $ ve " + String.Format("{0:0,0.00}", Euro) + " € senet bulunmaktadır.");
                sb.AppendLine(errorHtml);


                #region Create Email

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["partyid"] = new EntityReference("systemuser", userId);

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["subject"] = "Ödeme Vadesi " + DateTime.Now.Date.ToString("dd/MM/yyyy") + " - " + DateTime.Now.AddDays(6).Date.ToString("dd/MM/yyyy") + " Tarihleri Arasında Olan Senetler";
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

                sb.AppendLine("Bu hafta Toplam " + totalCount + " Adet Senet " + String.Format("{0:0,0.00}", TL) + " TL ve " + String.Format("{0:0,0.00}", Dolar) + " $ ve " + String.Format("{0:0,0.00}", Euro) + " € senet bulunmaktadır.");
                sb.AppendLine(errorHtml);


                #region Create Email

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["addressused"] = "Mail_KurumsalFinansman@nef.com.tr";

                EntityCollection ccCollection = new EntityCollection();
                Entity ccParty1 = new Entity("activityparty");
                Entity ccParty2 = new Entity("activityparty");
                Entity ccParty3 = new Entity("activityparty");
                ccParty1["addressused"] = "mete.altin@nef.com.tr";
                ccParty2["addressused"] = "Hakan.kocer@nef.com.tr";
                ccParty3["addressused"] = "Mail_Satis_Yonetim@nef.com.tr";
                ccCollection.Entities.Add(ccParty1);
                ccCollection.Entities.Add(ccParty2);
                ccCollection.Entities.Add(ccParty3);



                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["cc"] = ccCollection;
                email["subject"] = "Ödeme Vadesi " + DateTime.Now.Date.ToString("dd/MM/yyyy") + " - " + DateTime.Now.AddDays(6).Date.ToString("dd/MM/yyyy") + " Tarihleri Arasında Olan Senetler";
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

        internal static void ExecuteMailGroup(SqlDataAccess sda, IOrganizationService service)
        {
            #region SQL QUERY
            string sqlQuery = @"SELECT
                               Q.new_projectidName [İlgili Proje]
							   ,Q.new_productidName [İlgili Konut]
							   ,Q.CustomerIdName [Müşteri]
                               ,P.new_quoteidName [İlgili Satış]
                               ,CONVERT(varchar, CAST(P.new_paymentamount AS money), 1) AS [Ödenmesi Gereken Tutar]
							   , 
							   CONVERT(VARCHAR(10),
							   CAST(CONVERT(datetime, 
                               SWITCHOFFSET(CONVERT(datetimeoffset, 
                               P.new_date), 
                               DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as date),104) AS [Vade Tarihi]
							   ,CASE Q.CustomerIdType WHEN '1' THEN A.OwnerIdName WHEN '2' THEN C.OwnerIdName END AS [Müşteri Satış Temsilcisi]
							   ,CASE Q.CustomerIdType WHEN '1' THEN '-' WHEN '2' THEN C.MobilePhone END AS [Cep Telefonu]
							   ,CONVERT(varchar, CAST(Q.TotalAmountLessFreight AS money), 1) AS [İndirimli Konut Fiyatı]
							   ,ISNULL((SELECT Value FROM StringMap WHERE ObjectTypeCode=10040 AND AttributeName='new_itype' AND AttributeValue=P.new_itype) ,'-') AS [Ödeme Tipi]
							   ,ISNULL(CASE Q.CustomerIdType WHEN '1' THEN '-' WHEN '2' THEN C.new_cellphonenumber END,'-') AS [Tel. No. Cep]
							   ,ISNULL(CASE Q.CustomerIdType WHEN '1' THEN '-' WHEN '2' THEN C.new_telephone2number END,'-') AS [Tel. No. 2]
							   ,ISNULL((SELECT Value FROM StringMap WHERE ObjectTypeCode=10040 AND AttributeName='new_type' AND AttributeValue=P.new_type) ,'-') AS [Ödeme Türü]                            
                               ,Q.OwnerIdName [Satış Temsilcisi]
                               ,P.TransactionCurrencyIdName [Para Birimi]
							   ,P.new_paymentamount AS [Ödenmesi Gereken Tutar Decimnal]							   							   
							   ,CASE Q.CustomerIdType WHEN '1' THEN A.OwnerId WHEN '2' THEN C.OwnerId END AS [Müşteri Satış Temsilcisi Id]
                               FROM
                               new_payment P (NOLOCK)
                               JOIN
                               Quote Q (NOLOCK)
                               ON
                               P.new_quoteid=Q.QuoteId
							   LEFT JOIN
							   CONTACT C (NOLOCK)
							   ON
							   C.ContactId=Q.CustomerId
							   LEFT JOIN
							   ACCOUNT A (NOLOCK)
							   ON
							   A.AccountId=Q.CustomerId
							   WHERE
                               P.new_isvoucher=1
                               AND
                               Q.StatusCode!=6
                               AND
							   CAST(CONVERT(datetime, 
                               SWITCHOFFSET(CONVERT(datetimeoffset, 
                               P.new_date), 
                               DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as date)					   
                               BETWEEN							   
                              @date1
							   AND
                              @date2							
							 order by [Vade Tarihi]
                               ";
            sqlQuery = string.Format(sqlQuery);
            DataTable dt = sda.getDataTable(sqlQuery, new SqlParameter[] { new SqlParameter("date1", DateTime.Now.Date), new SqlParameter("date2", DateTime.Now.AddDays(6).Date) });

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
                dt.Columns.Remove("Müşteri Satış Temsilcisi Id");
                dt.Columns.Remove("Ödenmesi Gereken Tutar Decimnal");
                SendMailToResourceImpoter(dt, totalCount, TL, Dolar,Euro, service);





            }
        }
    }
}
