using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.ExchangeRateMail
{
    class Process
    {
        internal static void Execute()
        {
          
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                string sql = @"SELECT
                                Q.QuoteId
                                ,Q.new_contractdate
                                ,Q.new_amountwithtax                              
                            FROM
                            QUOTE AS Q (NOLOCK) where new_contractdate is not null and new_amountwithtax is not null";

                DataTable dt = sda.getDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            string sql2 = @"SELECT
                                P.new_date                              
                                ,P.new_paymentamount
                            FROM
                            new_payment AS P (NOLOCK) WHERE P.new_quoteid='{0}' and new_type !=7 and new_isvoucher=1 and new_date is not null order by new_date asc";
                            sql2 = string.Format(sql2, dt.Rows[i]["QuoteId"].ToString());
                            DataTable dt2 = sda.getDataTable(sql2);
                            int days = 0;
                            if (dt2.Rows.Count > 0)
                            {
                                DateTime firstdate = ((DateTime)dt2.Rows[0]["new_date"]).ToLocalTime();
                                DateTime lastdate = ((DateTime)dt2.Rows[dt2.Rows.Count - 1]["new_date"]).ToLocalTime();
                                TimeSpan difference = lastdate.Date - firstdate.Date;
                                days = (int)difference.TotalDays;//new_paymentterm
                            }

                            string sql3 = @"SELECT
                                P.new_date                              
                                ,P.new_paymentamount
                            FROM
                            new_payment AS P (NOLOCK) WHERE P.new_quoteid='{0}' and new_type !=7 and new_date is not null order by new_date desc";
                            sql3 = string.Format(sql3, dt.Rows[i]["QuoteId"].ToString());
                            DataTable dt3 = sda.getDataTable(sql3);
                            if (dt3.Rows.Count > 0)
                            {
                                DateTime salesTermDate = ((DateTime)dt3.Rows[0]["new_date"]);//new_salestermdate
                                DateTime contractDate = ((DateTime)dt.Rows[i]["new_contractdate"]);
                                TimeSpan difference = salesTermDate.Date - contractDate.Date;
                                int salesTermDay = (int)difference.TotalDays + 1;//new_salestermday
                                decimal amountwithtax = (decimal)dt.Rows[i]["new_amountwithtax"];
                                decimal total = 0;

                                for (int j = 0; j < dt3.Rows.Count; j++)
                                {
                                    TimeSpan difference2 = ((DateTime)dt3.Rows[j]["new_date"]).ToLocalTime().Date - contractDate.ToLocalTime().Date;
                                    int diff = (int)difference2.TotalDays + 1;
                                    total += diff * (decimal)dt3.Rows[j]["new_paymentamount"];
                                }

                                string effectiveTermDay = ((int)Math.Round(total / amountwithtax, 0, MidpointRounding.AwayFromZero)).ToString();//new_effectivetermday
                                DateTime effectiveTermDate = contractDate.AddDays((int)Math.Round(total / amountwithtax, 0, MidpointRounding.AwayFromZero) - 1);//new_effectivetermdate


                                #region | QUERY UPDATE STATUS|
                                string sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   new_paymentterm=@new_paymentterm,
                                       new_salestermdate=@new_salestermdate,
                                       new_salestermday=@new_salestermday,
                                       new_effectivetermday=@new_effectivetermday,
                                       new_effectivetermdate=@new_effectivetermdate                                      
                                    WHERE
	                                    QuoteId='{0}'";
                                #endregion

                                sda.ExecuteNonQuery(string.Format(sqlQuery, dt.Rows[i]["QuoteId"].ToString()), new SqlParameter[] { 
                         new SqlParameter("new_paymentterm", days.ToString())
                        ,new SqlParameter("new_salestermdate", salesTermDate)
                        ,new SqlParameter("new_salestermday", salesTermDay.ToString())
                        ,new SqlParameter("new_effectivetermday",effectiveTermDay)
                        ,new SqlParameter("new_effectivetermdate", effectiveTermDate)});


                            }
                            Console.Clear();
                            Console.Write(i + 1 + "/" + dt.Rows.Count);
                        }
                        catch (Exception ex)
                        {

                            continue;
                        }
                    }

                }
          
          
        }
    }
}
