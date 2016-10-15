using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NEF.Library.Utility;
using NEF.Library.Business;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace NEF.Library.Business
{
    public static class LogoHelper
    {

        public static List<ExpenseCenter> GetExpenseCenterList(SqlDataAccess sda)
        {
            List<ExpenseCenter> returnValue = new List<ExpenseCenter>();
            try
            {
                List<ExpenseCenter> masrafMerkezList = new List<ExpenseCenter>();

                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    p.ProductId
	                                    ,pro.new_projectcode AS ProjectCode
                                    FROM
	                                    Product AS p (NOLOCK)
		                                    JOIN
			                                    new_project AS pro (NOLOCK)
				                                    ON
				                                    p.new_projectid=pro.new_projectId
                                    WHERE
	                                    p.new_islogointegration=1 AND p.StateCode=0
                                    AND
	                                    CAST( p.ModifiedOn AS DATE)=CAST(GETUTCDATE() AS DATE)";

                #endregion

                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product pInfo = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);

                        if (pInfo != null && pInfo.ProductId != Guid.Empty)
                        {
                            ExpenseCenter eCent = new ExpenseCenter()
                                            {
                                                MasrafMerkezId = pInfo.ProductId
                                            };

                            if (dt.Rows[i]["ProjectCode"] != DBNull.Value)
                            {
                                eCent.ProjeKodu = dt.Rows[i]["ProjectCode"].ToString();
                            }

                            if (!string.IsNullOrEmpty(pInfo.Name))
                            {
                                eCent.DaireKimlikNo = pInfo.Name;
                            }

                            if (pInfo.Currency != null && pInfo.Currency.Id != Guid.Empty)
                            {
                                eCent.ParaBirimi = pInfo.Currency.Name;
                            }

                            if (pInfo.Project != null && pInfo.Project.Id != Guid.Empty)
                            {
                                eCent.ProjeID = pInfo.Project.Id;
                            }
                            if (pInfo.Price != null)
                            {
                                eCent.Fiyat = (decimal)pInfo.Price;
                            }

                            //if ((pInfo.Project == null || pInfo.Project.Id == Guid.Empty || pInfo.Brut == null || pInfo.FloorNumber == null || string.IsNullOrEmpty(pInfo.HomeNumber) || pInfo.Block == null || pInfo.Block.Id == Guid.Empty ? false : pInfo.Aks != null))
                            if ((pInfo.Project.Name == "895 NEF Yalıkavak" || pInfo.Project.Id == Guid.Empty || pInfo.Brut == null || string.IsNullOrEmpty(pInfo.HomeNumber) || pInfo.Block == null || (pInfo.Block.Id == Guid.Empty ? false : true)))
                            {
                                string kelime = string.Empty;
                                kelime = pInfo.Project.Name.Substring(4, 7);

                                if (kelime != string.Empty)
                                {
                                    if (kelime.Substring(kelime.Length - 1) == " ")
                                    {
                                        kelime = kelime.Substring(0, kelime.Length - 1);
                                    }
                                }

                                eCent.MasrafAd = string.Concat(kelime, " ");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, pInfo.Block.Name, " Bl D:");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, pInfo.HomeNumber, " (Aks");
                       
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, (pInfo.Aks != null ? ((int)pInfo.Aks).ToString() : "---"), ") M2:");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, ((decimal)pInfo.Brut).ToString("N2").Replace(',', '.'), " ");

                                eCent.IslemTipi = 1;
                                masrafMerkezList.Add(eCent);
                            }
                            else if ((pInfo.Project == null || pInfo.Project.Id == Guid.Empty || pInfo.Brut == null || pInfo.FloorNumber == null || string.IsNullOrEmpty(pInfo.HomeNumber) || pInfo.Block == null || pInfo.Block.Id == Guid.Empty ? false : true))
                            {
                                string kelime = string.Empty;
                                kelime = pInfo.Project.Name.Substring(4, 7);

                                if (kelime != string.Empty)
                                {
                                    if (kelime.Substring(kelime.Length - 1) == " ")
                                    {
                                        kelime = kelime.Substring(0, kelime.Length - 1);
                                    }
                                }

                                eCent.MasrafAd = string.Concat(kelime, " ");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, pInfo.Block.Name, " Bl D:");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, pInfo.HomeNumber, " K:");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, ((int)pInfo.FloorNumber).ToString(), " (Aks");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, (pInfo.Aks != null ? ((int)pInfo.Aks).ToString() : "---"), ") M2:");
                                eCent.MasrafAd = string.Concat(eCent.MasrafAd, ((decimal)pInfo.Brut).ToString("N2").Replace(',', '.'), " ");

                                eCent.IslemTipi = 1;
                                masrafMerkezList.Add(eCent);
                            }
                            
                        }
                    }
                }


                returnValue = masrafMerkezList;
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static List<LogoAccount> GetLogoFirmalar(Guid projectId, SqlDataAccess sda)
        {
            List<LogoAccount> returnValue = new List<LogoAccount>();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    lap.new_logoaccountid AS LogoAccountId
	                                    ,lap.new_projectid AS ProjectId
	                                    ,la.new_name LogoAccountName
	                                    ,la.new_periodnumber AS PeriodNo
                                    FROM
	                                    new_new_project_new_logoaccount AS  lap
		                                    JOIN
			                                    new_logoaccount AS la (NOLOCK)
				                                    ON
				                                    lap.new_logoaccountid=la.new_logoaccountId
                                    WHERE
                                    lap.new_projectid='{0}'
                                    AND
                                    la.new_name IS NOT NULL
                                    AND
                                    la.new_periodnumber IS NOT NULL
                                    AND
                                    la.statecode=0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, projectId));

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        LogoAccount la = new LogoAccount();

                        la.FirmaNo = dt.Rows[i]["LogoAccountName"].ToString();
                        la.DonemNo = dt.Rows[i]["PeriodNo"].ToString();

                        returnValue.Add(la);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static bool CheckExpenseCenter(ExpenseCenter expenceCenter)
        {
            bool returnValue = false;
            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"SELECT CRMID FROM MASRAFMERKEZI WHERE CRMID=@CRMID";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@CRMID", expenceCenter.MasrafMerkezId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static void CreateExpenseCenter(ExpenseCenter expenseCenter, LogoAccount logoAccount)
        {
            try
            {
                int status = 0;
                int kayitstatusu = 0;
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionStringLogo); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"INSERT INTO MASRAFMERKEZI (ISLEMTIPI,KAYITTARIHI,KAYITSTATUSU,CRMID,MASRAFKODU,MASRAFADI,DAIRELISTEFIYAT,PARABIRIMI,STATUS,FIRMANO,DONEMNO,PROJEKODU) VALUES (@ISLEMTIPI,@KAYITTARIHI,@KAYITSTATUSU,@CRMID,@MASRAFKODU,@MASRAFADI,@DAIRELISTEFIYAT,@PARABIRIMI,@STATUS,@FIRMANO,@DONEMNO,@PROJEKODU)";

                #endregion

                List<SqlParameter> lstParameters = new List<SqlParameter>();

                lstParameters.Add(new SqlParameter("@ISLEMTIPI", expenseCenter.IslemTipi));
                lstParameters.Add(new SqlParameter("@KAYITTARIHI", DateTime.Now));
                lstParameters.Add(new SqlParameter("@KAYITSTATUSU", kayitstatusu));
                lstParameters.Add(new SqlParameter("@CRMID", expenseCenter.MasrafMerkezId));
                lstParameters.Add(new SqlParameter("@MASRAFKODU", expenseCenter.DaireKimlikNo == null ? string.Empty : expenseCenter.DaireKimlikNo));
                lstParameters.Add(new SqlParameter("@MASRAFADI", expenseCenter.MasrafAd == null ? string.Empty : expenseCenter.MasrafAd));
                lstParameters.Add(new SqlParameter("@DAIRELISTEFIYAT", expenseCenter.Fiyat));
                lstParameters.Add(new SqlParameter("@PARABIRIMI", expenseCenter.ParaBirimi));
                lstParameters.Add(new SqlParameter("@STATUS", status));

                if (!string.IsNullOrEmpty(expenseCenter.ProjeKodu))
                    lstParameters.Add(new SqlParameter("@PROJEKODU", expenseCenter.ProjeKodu));
                else
                    lstParameters.Add(new SqlParameter("@PROJEKODU", DBNull.Value));

                if (logoAccount == null)
                {
                    lstParameters.Add(new SqlParameter("@FIRMANO", ""));
                    lstParameters.Add(new SqlParameter("@DONEMNO", ""));
                }
                else
                {
                    lstParameters.Add(new SqlParameter("@FIRMANO", (logoAccount.FirmaNo == null ? string.Empty : logoAccount.FirmaNo)));
                    lstParameters.Add(new SqlParameter("@DONEMNO", (logoAccount.DonemNo == null ? string.Empty : logoAccount.DonemNo)));
                }

                sda.ExecuteNonQuery(sqlQuery, lstParameters.ToArray());

            }
            catch (Exception ex)
            {

            }
        }

        public static List<Satislar> GetSatisList(SqlDataAccess sda)
        {

            List<Satislar> satislars;
            DateTime? sozlesmeTarih;
            DateTime value;
            bool hour;
            bool flag;
            try
            {
                List<Satislar> SatisList = new List<Satislar>();

                #region | SQL QUERY |
                string sqlQuery = @"SELECT
                                         q.QuoteId
                                         ,q.Name
                                         ,q.new_contractdate AS ContactDate
                                         ,q.new_contractnumber AS ContractNumber
                                         ,q.StatusCode
                                         ,p.new_projectid AS ProjectId
                                         ,p.new_projectidName AS ProjectIdName
                                         ,q.TransactionCurrencyId
                                         ,q.TransactionCurrencyIdName
                                         ,q.new_taxamount AS KDVAmount
                                         ,q.new_taxrate AS KDVRatio
                                         ,q.TotalAmountLessFreight AS DiscountAmount
                                         ,c.new_groupcode AS GroupAccountCode
                                            ,c.ContactId
                                         ,fa.new_name AS FinancialAccountCode
                                            ,q.new_financialaccountid AS FinancailAccountId
                                            ,q.new_financialaccountidName AS FinancailAccountIdName
                                         ,pro.new_deliverydate AS DeliveryDate
                                         ,pro.new_workplace AS WorkPlace
                                         ,p.new_generaltypeofhomeid AS GeneralHomeTypeId
                                         ,p.new_generaltypeofhomeidName AS GeneralHomeTypeIdName
                                         ,p.new_unittypeid AS UnitTypeId
                                         ,p.new_unittypeidName AS UnitTypeIdName
                                            ,ut.new_unitcode AS UnitTypeCode
                                            ,ISNULL(exr.new_salesrate,1) AS CurrencySalesRate
                                            ,pro.new_projectcode AS ProjectNo
                                            ,q.ModifiedOn
                                        FROM
                                        Quote AS q (NOLOCK)
                                         JOIN
                                          QuoteDetail AS qd (NOLOCK)
                                           ON
                                           q.QuoteId=qd.QuoteId
                                         JOIN
                                          Product AS p (NOLOCK)
                                           ON
                                           qd.ProductId=p.ProductId
                                         JOIN
                                          new_project AS pro (NOLOCK)
                                           ON
                                           p.new_projectid =pro.new_projectId
                                         JOIN
                                          new_financialaccount fa (NOLOCK)
                                           ON
                                           q.new_financialaccountid=fa.new_financialaccountId
                                         JOIN
                                          Contact AS c (NOLOCK)
                                           ON
                                           c.ContactId=q.CustomerId
                                         JOIN
                                          new_unittype AS ut (NOLOCK)
                                           ON
                                           ut.new_unittypeId=p.new_unittypeid
                                            LEFT JOIN
                                          new_exchangerate AS exr (NOLOCK)
                                           ON
                                           CAST(exr.new_currencydate AS DATE)=CAST(q.new_contractdate AS DATE)
                                           AND
                                           exr.new_currencyid=q.TransactionCurrencyId
                                        WHERE
                                        q.new_islogotransferred=1
                                        --AND
                                        --q.new_prestatus = 100000001
                                        AND
                                        q.new_financialaccountidName!=q.CustomerIdName
                                        AND
                                        q.new_financialaccountid is not null
                                        AND
                                     CAST( q.ModifiedOn AS DATE)=CAST(GETUTCDATE() AS DATE)

                UNION

                SELECT
                                         q.QuoteId
                                         ,q.Name
                                         ,q.new_contractdate AS ContactDate
                                         ,q.new_contractnumber AS ContractNumber
                                         ,q.StatusCode
                                         ,p.new_projectid AS ProjectId
                                         ,p.new_projectidName AS ProjectIdName
                                         ,q.TransactionCurrencyId
                                         ,q.TransactionCurrencyIdName
                                         ,q.new_taxamount AS KDVAmount
                                         ,q.new_taxrate AS KDVRatio
                                         ,q.TotalAmountLessFreight AS DiscountAmount
                                         ,a.new_groupcode AS GroupAccountCode
                                            ,a.AccountId
                                         ,fa.new_name AS FinancialAccountCode
                                            ,q.new_financialaccountid AS FinancailAccountId
                                            ,q.new_financialaccountidName AS FinancailAccountIdName
                                         ,pro.new_deliverydate AS DeliveryDate
                                         ,pro.new_workplace AS WorkPlace
                                         ,p.new_generaltypeofhomeid AS GeneralHomeTypeId
                                         ,p.new_generaltypeofhomeidName AS GeneralHomeTypeIdName
                                         ,p.new_unittypeid AS UnitTypeId
                                         ,p.new_unittypeidName AS UnitTypeIdName
                                            ,ut.new_unitcode AS UnitTypeCode
                                            ,ISNULL(exr.new_salesrate,1) AS CurrencySalesRate
                                            ,pro.new_projectcode AS ProjectNo
                                            ,q.ModifiedOn
                                        FROM
                                        Quote AS q (NOLOCK)
                                         JOIN
                                          QuoteDetail AS qd (NOLOCK)
                                           ON
                                           q.QuoteId=qd.QuoteId
                                         JOIN
                                          Product AS p (NOLOCK)
                                           ON
                                           qd.ProductId=p.ProductId
                                         JOIN
                                          new_project AS pro (NOLOCK)
                                           ON
                                           p.new_projectid =pro.new_projectId
                                         JOIN
                                          new_financialaccount fa (NOLOCK)
                                           ON
                                           q.new_financialaccountid=fa.new_financialaccountId
                                         JOIN
                                          Account AS a (NOLOCK)
                                           ON
                                           a.AccountId=q.CustomerId
                                         JOIN
                                          new_unittype AS ut (NOLOCK)
                                           ON
                                           ut.new_unittypeId=p.new_unittypeid
                                            LEFT JOIN
                                          new_exchangerate AS exr (NOLOCK)
                                           ON
                                           CAST(exr.new_currencydate AS DATE)=CAST(q.new_contractdate AS DATE)
                                           AND
                                           exr.new_currencyid=q.TransactionCurrencyId
                                        WHERE
                                        q.new_islogotransferred=1
                                        --AND
                                        --q.new_prestatus = 100000001
                                        AND
                                        q.new_financialaccountidName!=q.CustomerIdName
                                        AND
                                        q.new_financialaccountid is not null
                                        AND
                                     CAST( q.ModifiedOn AS DATE)=CAST(GETUTCDATE() AS DATE)";



                #endregion

                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Satislar satis = new Satislar()
                        {
                            SatisID = (Guid)dt.Rows[i]["QuoteId"]
                        };

                        if (dt.Rows[i]["ModifiedOn"] != DBNull.Value)
                        {
                            satis.ModifiedOn = (DateTime)dt.Rows[i]["ModifiedOn"];
                        }

                        if (dt.Rows[i]["ContractNumber"] == DBNull.Value)
                        {
                            satis.SatisNo = "";
                        }
                        else
                        {
                            satis.SatisNo = dt.Rows[i]["ContractNumber"].ToString();
                        }

                        if (dt.Rows[i]["ContactDate"] == DBNull.Value)
                        {
                            sozlesmeTarih = null;
                            satis.SozlesmeTarih = sozlesmeTarih;
                        }
                        else
                        {
                            satis.SozlesmeTarih = (DateTime)dt.Rows[i]["ContactDate"];
                            sozlesmeTarih = satis.SozlesmeTarih;
                            if (sozlesmeTarih.Value.Hour > 21)
                            {
                                flag = false;
                            }
                            else
                            {
                                sozlesmeTarih = satis.SozlesmeTarih;
                                value = sozlesmeTarih.Value;
                                flag = value.Hour != 21;
                            }
                            if (!flag)
                            {
                                sozlesmeTarih = satis.SozlesmeTarih;
                                value = sozlesmeTarih.Value;
                                satis.SozlesmeTarih = new DateTime?(value.AddHours(3));
                            }
                        }

                        if (dt.Rows[i]["FinancailAccountId"] != DBNull.Value)
                        {
                            if (dt.Rows[i]["FinancailAccountIdName"].ToString().Substring(0, 1) != "3")
                            {
                                satis.CariHesapKodu = dt.Rows[i]["FinancailAccountIdName"].ToString();
                            }
                            else
                            {
                                satis.CariHesapKodu = string.Concat("340", dt.Rows[i]["FinancailAccountIdName"].ToString().Substring(3));
                            }

                            satis.CariId = (Guid)dt.Rows[i]["FinancailAccountId"];

                        }

                        if (dt.Rows[i]["GroupAccountCode"] != DBNull.Value && dt.Rows[i]["ContactId"] != DBNull.Value)
                        {
                            satis.GrupSirketKodu = dt.Rows[i]["GroupAccountCode"].ToString();
                            satis.GrupSirketId = (Guid)dt.Rows[i]["ContactId"];
                        }

                        if (dt.Rows[i]["ProjectId"] != DBNull.Value)
                        {
                            satis.ProjeID = (Guid)dt.Rows[i]["ProjectId"];
                        }

                        if (dt.Rows[i]["ProjectNo"] != DBNull.Value)
                        {
                            satis.ProjeKimligi = dt.Rows[i]["ProjectNo"].ToString();
                        }
                        else
                            satis.ProjeKimligi = "";

                        //if (!item.Attributes.Contains("a.new_navisionprojeno"))
                        //{
                        //    satis.ProjeKimligi = "";
                        //}
                        //else
                        //{
                        //    satis.ProjeKimligi = ((AliasedValue)item.Attributes["a.new_navisionprojeno"]).Value.ToString();
                        //}


                        if (dt.Rows[i]["ProjectIdName"] == DBNull.Value)
                        {
                            satis.ProjeKodu = "";
                        }
                        else
                        {
                            satis.ProjeKodu = dt.Rows[i]["ProjectIdName"].ToString();
                        }

                        if (dt.Rows[i]["UnitTypeIdName"] == DBNull.Value)
                        {
                            satis.UniteKodu = "";
                        }
                        else
                        {
                            satis.UniteKodu = dt.Rows[i]["UnitTypeIdName"].ToString();
                        }

                        if (dt.Rows[i]["WorkPlace"] == DBNull.Value)
                        {
                            satis.IsYeri = "";
                        }
                        else
                        {
                            satis.IsYeri = dt.Rows[i]["WorkPlace"].ToString();
                        }

                        if (dt.Rows[i]["TransactionCurrencyId"] == DBNull.Value)
                        {
                            satis.ParaBirimi = "";
                        }
                        else
                        {
                            satis.ParaBirimi = dt.Rows[i]["TransactionCurrencyIdName"].ToString();
                        }


                        if (satis.ParaBirimi == "TL")
                        {
                            satis.DovizKuru = new decimal(1);
                        }
                        else if (dt.Rows[i]["CurrencySalesRate"] != DBNull.Value)
                        {
                            satis.DovizKuru = (decimal)dt.Rows[i]["CurrencySalesRate"];
                        }

                        //Topkapı projesi ise Satışın yarı tutarı logo ara tabloya aktarılır. aksi halde tamamı.
                        if (dt.Rows[i]["WorkPlace"] != DBNull.Value)
                        {
                            if (Convert.ToString(dt.Rows[i]["WorkPlace"]).Equals("827"))
                            {
                                if (dt.Rows[i]["DiscountAmount"] != DBNull.Value)
                                {
                                    satis.IndirimliTutar = ((decimal)dt.Rows[i]["DiscountAmount"] / 2);
                                }
                                if (dt.Rows[i]["KDVAmount"] != DBNull.Value)
                                {
                                    satis.KdvTutar = ((decimal)dt.Rows[i]["KDVAmount"]/ 2);
                                }
                            }
                            else
                            {
                                if (dt.Rows[i]["DiscountAmount"] != DBNull.Value)
                                {
                                    satis.IndirimliTutar = (decimal)dt.Rows[i]["DiscountAmount"];
                                }
                                if (dt.Rows[i]["KDVAmount"] != DBNull.Value)
                                {
                                    satis.KdvTutar = (decimal)dt.Rows[i]["KDVAmount"];
                                }
                            }
                        }

                        if (dt.Rows[i]["DeliveryDate"] == DBNull.Value)
                        {
                            sozlesmeTarih = null;
                            satis.TeslimTarihi = sozlesmeTarih;
                        }
                        else
                        {
                            satis.TeslimTarihi = (DateTime)dt.Rows[i]["DeliveryDate"];
                            sozlesmeTarih = satis.TeslimTarihi;
                            if (sozlesmeTarih.Value.Hour > 21)
                            {
                                hour = false;
                            }
                            else
                            {
                                sozlesmeTarih = satis.TeslimTarihi;
                                value = sozlesmeTarih.Value;
                                hour = value.Hour != 21;
                            }
                            if (!hour)
                            {
                                sozlesmeTarih = satis.TeslimTarihi;
                                value = sozlesmeTarih.Value;
                                satis.TeslimTarihi = new DateTime?(value.AddHours(3));
                            }
                        }

                        if (dt.Rows[i]["KDVRatio"] != DBNull.Value)
                        {
                            satis.KdvOran = (decimal)dt.Rows[i]["KDVRatio"];
                        }

                        

                        if (dt.Rows[i]["UnitTypeId"] != DBNull.Value)
                        {
                            satis.BolumKodu = dt.Rows[i]["UnitTypeCode"].ToString();
                        }

                        if (dt.Rows[i]["StatusCode"] != DBNull.Value)
                        {
                            int durum = (int)dt.Rows[i]["StatusCode"];

                            if (durum == 1)
                            {
                                satis.SatisDurumu = "ÖNSATIŞ";
                                satis.UniteKodu = string.Concat("Gayrimenkul Satış-", satis.UniteKodu, "-Satış");
                            }
                            if (durum == 6 || durum == 7 ) //İptal veya düzeltilmiş
                            {
                                satis.SatisDurumu = "İPTAL";
                                satis.UniteKodu = string.Concat("Gayrimenkul Satış-", satis.UniteKodu, "-İptal");
                            }
                            if (durum == 100000001)
                            {
                                satis.SatisDurumu = "SÖZLEŞME";
                                satis.UniteKodu = string.Concat("Gayrimenkul Satış-", satis.UniteKodu, "-Satış");
                            }
                        }

                        SatisList.Add(satis);
                    }
                }
                satislars = SatisList;
                return satislars;
            }
            catch (Exception exception)
            {
               
            }

            satislars = null;
            return satislars;

        }

        public static bool ControlQuotePayment(Guid quoteId)
        {
            bool retVal = true;

            string sqlQuery = @"
                                    SELECT 
										*
								FROM 
									new_payment AS pay WITH(NOLOCK) 
									
								WHERE 
									pay.new_quoteid = @CRMID
								and	
									pay.new_sign = 0 
								and 
									pay.new_isvoucher = 1";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@CRMID", quoteId.ToString()) };

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

                DataTable dt = sda.getDataTable(sqlQuery, parameters);
                if (dt.Rows.Count > 0)
                {
                    retVal = false;
                }

                return retVal;
        }

        public static bool ExecuteCari(Satislar satis, LogoAccount firma, SqlDataAccess sda)
        {
            bool flag;
            bool kontrol = true;
            try
            {
                List<Cariler> cariList = LogoHelper.GetCariList(satis.SatisID, sda);

                if ((cariList == null ? false : cariList.Count > 0))
                {
                    foreach (Cariler item in cariList)
                    {
                        item.SozlesmeDurumu = satis.SatisDurumu;
                        kontrol = (!LogoHelper.CariKontrol(item) ? LogoHelper.CreateCari(item, firma) : LogoHelper.CreateCari(item, firma));
                    }
                }
                if (!kontrol)
                {
                    flag = false;
                    return flag;
                }
            }
            catch (Exception ex)
            {

            }

            flag = true;
            return flag;
        }

        public static List<Cariler> GetCariList(Guid satisID, SqlDataAccess sda)
        {
            List<Cariler> carilers;
            try
            {
                List<Cariler> CariList = new List<Cariler>();

                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    q.QuoteId
	                                    ,q.CustomerId
                                        ,q.CustomerIdName
	                                    ,qd.ProductId
                                        ,qd.ProductIdName
	                                    ,c.new_tcidentitynumber AS TcNo
	                                    ,c.new_addressdistrictid AS DistrictId
	                                    ,c.new_addressdistrictidName AS DistrictIdName
	                                    ,c.JobTitle
	                                    ,c.new_countryid AS CountryId
	                                    ,c.new_countryidName AS CountryIdName
	                                    ,c.new_addresscityid AS CityId
	                                    ,c.new_addresscityidName AS CityIdName
	                                    ,c.new_addresstownid AS TownId
	                                    ,c.new_addresstownidName AS TownIdName
	                                    ,c.EMailAddress1
	                                    ,c.MobilePhone
	                                    ,c.Telephone2
	                                    ,c.new_addressdetail AS AddressDetail
	                                    ,c.new_groupcode AS GroupAccountCode
	                                    ,fa.new_name AS FinancialAccountCode
                                        ,fa.new_financialaccountId AS FinancialAccountId
	                                    ,p.new_homenumber AS HomeNumber
                                        ,p.new_projectid AS ProjectId
                                        ,p.new_projectidName AS ProjectIdName
										,NULL AS TaxOffice
                                    FROM
                                    Quote AS q
	                                    JOIN
		                                    QuoteDetail AS qd (NOLOCK)
			                                    ON
			                                    qd.QuoteId=q.QuoteId
	                                    JOIN
		                                    Product AS p (NOLOCK)
			                                    ON
			                                    qd.ProductId=p.ProductId
	                                    JOIN
		                                    new_financialaccount AS fa (NOLOCK)
			                                    ON
			                                    fa.new_financialaccountId=q.new_financialaccountid
	                                    JOIN
		                                    Contact AS c (NOLOCK)
			                                    ON
			                                    c.ContactId=fa.new_contactid
                                    WHERE
                                     q.QuoteId='{0}'
									UNION 
									SELECT
									 q.QuoteId
	                                    ,q.CustomerId
                                        ,q.CustomerIdName
	                                    ,qd.ProductId
                                        ,qd.ProductIdName
	                                    ,a.new_taxnumber AS TcNo
	                                    ,a.new_addressdistrictid AS DistrictId
	                                    ,a.new_addressdistrictidName AS DistrictIdName
	                                    ,NULL AS JobTitle
	                                    ,a.new_addresscountryid AS CountryId
	                                    ,a.new_addresscountryidName AS CountryIdName
	                                    ,a.new_addresscityid AS CityId
	                                    ,a.new_addresscityidName AS CityIdName
	                                    ,a.new_addresstownid AS TownId
	                                    ,a.new_addresstownidName AS TownIdName
	                                    ,a.EMailAddress1
	                                    ,a.Telephone1
	                                    ,a.Telephone2
	                                    ,a.new_addressdetail AS AddressDetail
	                                    ,a.new_groupcode AS GroupAccountCode
	                                    ,fa.new_name AS FinancialAccountCode
                                        ,fa.new_financialaccountId AS FinancialAccountId
	                                    ,p.new_homenumber AS HomeNumber
                                        ,p.new_projectid AS ProjectId
                                        ,p.new_projectidName AS ProjectIdName
										,a.new_taxofficeidName AS TaxOffice
                                    FROM
                                    Quote AS q
	                                    JOIN
		                                    QuoteDetail AS qd (NOLOCK)
			                                    ON
			                                    qd.QuoteId=q.QuoteId
	                                    JOIN
		                                    Product AS p (NOLOCK)
			                                    ON
			                                    qd.ProductId=p.ProductId
	                                    JOIN
		                                    new_financialaccount AS fa (NOLOCK)
			                                    ON
			                                    fa.new_financialaccountId=q.new_financialaccountid
	                                    JOIN
		                                    Account AS a (NOLOCK)
			                                    ON
			                                    a.AccountId=fa.new_accountid
                                    WHERE
                                     q.QuoteId='{0}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, satisID));

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string projeKodu = string.Empty;

                        if (dt.Rows[i]["ProjectIdName"] != DBNull.Value)
                        {
                            projeKodu = dt.Rows[i]["ProjectIdName"].ToString();
                        }

                        if (dt.Rows[i]["FinancialAccountCode"] != DBNull.Value && dt.Rows[i]["FinancialAccountCode"].ToString().Substring(0, 1) == "3")
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                Cariler cari = new Cariler()
                                {
                                    CariID = (Guid)dt.Rows[i]["FinancialAccountId"]
                                };

                                cari.ProjeKodu = projeKodu;

                                if (dt.Rows[i]["FinancialAccountCode"] == DBNull.Value)
                                {
                                    cari.CariKod = "";
                                }
                                else
                                {
                                    cari.CariKod = dt.Rows[i]["FinancialAccountCode"].ToString();

                                    if (j == 0)
                                    {
                                        cari.CariKod = dt.Rows[i]["FinancialAccountCode"].ToString();
                                    }
                                    else if (j == 1)
                                    {
                                        if (cari.CariKod.Substring(0, 3) == "340")
                                        {
                                            cari.CariKod = string.Concat("329", cari.CariKod.Substring(3, cari.CariKod.Length - 3));
                                        }
                                        else if (cari.CariKod.Substring(0, 3) == "329")
                                        {
                                            cari.CariKod = string.Concat("340", cari.CariKod.Substring(3, cari.CariKod.Length - 3));
                                        }
                                    }
                                    else if (j == 2)
                                    {
                                        cari.CariKod = string.Concat("120", cari.CariKod.Substring(3, cari.CariKod.Length - 3));
                                    }
                                    else if (j == 3)
                                    {
                                        cari.CariKod = string.Concat("127", cari.CariKod.Substring(3, cari.CariKod.Length - 3));
                                    }
                                }
                                cari.TeslimSekli = "GM Satış";
                                cari.SozlesmeDurumu = "SÖZLEŞME";

                                #region | FILL SALE INFO |

                                if (dt.Rows[i]["CustomerId"] != DBNull.Value)
                                {
                                    cari.Unvan = dt.Rows[i]["CustomerIdName"].ToString();
                                }

                                if (dt.Rows[i]["AddressDetail"] != DBNull.Value)
                                {
                                    cari.Adres1 = dt.Rows[i]["AddressDetail"].ToString();
                                }
                                else
                                {
                                    cari.Adres1 = "";
                                }
                                //else if (!item.Attributes.Contains("account.address1_line1"))
                                //{
                                //    cari.Adres1 = "";
                                //}
                                //else
                                //{
                                //    cari.Adres1 = ((AliasedValue)item.Attributes["account.address1_line1"]).Value.ToString();
                                //}

                                cari.Adres2 = "";
                                //if (item.Attributes.Contains("contact.address1_line2"))
                                //{
                                //    cari.Adres2 = ((AliasedValue)item.Attributes["contact.address1_line2"]).Value.ToString();
                                //}
                                //else if (!item.Attributes.Contains("account.address1_line2"))
                                //{
                                //    cari.Adres2 = "";
                                //}
                                //else
                                //{
                                //    cari.Adres2 = ((AliasedValue)item.Attributes["account.address1_line2"]).Value.ToString();
                                //}

                                if (dt.Rows[i]["CityId"] != DBNull.Value)
                                {
                                    cari.Il = dt.Rows[i]["CityIdName"].ToString();
                                }
                                else
                                {
                                    cari.Il = "";
                                }
                                //else if (!item.Attributes.Contains("account.new_ilid"))
                                //{
                                //    cari.Il = "";
                                //}
                                //else
                                //{
                                //    cari.Il = ((EntityReference)((AliasedValue)item.Attributes["account.new_ilid"]).Value).Name;
                                //}

                                if (dt.Rows[i]["CountryId"] != DBNull.Value)
                                {
                                    cari.Ulke = dt.Rows[i]["CountryIdName"].ToString();
                                }
                                else
                                {
                                    cari.Ulke = "";
                                }
                                //else if (!item.Attributes.Contains("account.new_ulke"))
                                //{
                                //    cari.Ulke = "";
                                //}
                                //else
                                //{
                                //    cari.Ulke = ((EntityReference)((AliasedValue)item.Attributes["account.new_ulke"]).Value).Name;
                                //}

                                if (dt.Rows[i]["GroupAccountCode"] == DBNull.Value)
                                {
                                    cari.GrupSirketKodu = "";
                                }
                                else
                                {
                                    cari.GrupSirketKodu = dt.Rows[i]["GroupAccountCode"].ToString();
                                }

                                if (dt.Rows[i]["TownId"] != DBNull.Value)
                                {
                                    cari.Ilce = dt.Rows[i]["TownIdName"].ToString();
                                }
                                else
                                {
                                    cari.Ilce = "";
                                }
                                //else if (!item.Attributes.Contains("account.new_ilceid"))
                                //{
                                //    cari.Ilce = "";
                                //}
                                //else
                                //{
                                //    cari.Ilce = ((EntityReference)((AliasedValue)item.Attributes["account.new_ilceid"]).Value).Name;
                                //}

                                if (dt.Rows[i]["DistrictId"] != DBNull.Value)
                                {
                                    cari.Semt = dt.Rows[i]["DistrictIdName"].ToString();
                                }
                                else
                                {
                                    cari.Semt = "";
                                }
                                //else if (!item.Attributes.Contains("account.new_semtid"))
                                //{
                                //    cari.Semt = "";
                                //}
                                //else
                                //{
                                //    cari.Semt = ((EntityReference)((AliasedValue)item.Attributes["account.new_semtid"]).Value).Name;
                                //}

                                if (dt.Rows[i]["MobilePhone"] != DBNull.Value)
                                {
                                    cari.CepTel1 = dt.Rows[i]["MobilePhone"].ToString();
                                }
                                else
                                {
                                    cari.CepTel1 = "";
                                }
                                //else if (!item.Attributes.Contains("account.telephone1"))
                                //{
                                //    cari.CepTel1 = "";
                                //}
                                //else
                                //{
                                //    cari.CepTel1 = ((AliasedValue)item.Attributes["account.telephone1"]).Value.ToString();
                                //}

                                if (dt.Rows[i]["Telephone2"] != DBNull.Value)
                                {
                                    cari.CepTel2 = dt.Rows[i]["Telephone2"].ToString();
                                }
                                else
                                {
                                    cari.CepTel2 = "";
                                }
                                //else if (!item.Attributes.Contains("account.telephone2"))
                                //{
                                //    cari.CepTel2 = "";
                                //}
                                //else
                                //{
                                //    cari.CepTel2 = ((AliasedValue)item.Attributes["account.telephone2"]).Value.ToString();
                                //}

                                cari.Fax = "";
                                //if (item.Attributes.Contains("contact.fax"))
                                //{
                                //    cari.Fax = ((AliasedValue)item.Attributes["contact.fax"]).Value.ToString();
                                //}
                                //else if (!item.Attributes.Contains("account.fax"))
                                //{
                                //    cari.Fax = "";
                                //}
                                //else
                                //{
                                //    cari.Fax = ((AliasedValue)item.Attributes["account.fax"]).Value.ToString();
                                //}

                                if (dt.Rows[i]["EMailAddress1"] != DBNull.Value)
                                {
                                    cari.Eposta1 = dt.Rows[i]["EMailAddress1"].ToString();
                                }
                                else
                                {
                                    cari.Eposta1 = "";
                                }
                                //else if (!item.Attributes.Contains("account.emailaddress1"))
                                //{
                                //    cari.Eposta1 = "";
                                //}
                                //else
                                //{
                                //    cari.Eposta1 = ((AliasedValue)item.Attributes["account.emailaddress1"]).Value.ToString();
                                //}

                                cari.Eposta2 = "";
                                //if (item.Attributes.Contains("contact.emailaddress2"))
                                //{
                                //    cari.Eposta2 = ((AliasedValue)item.Attributes["contact.emailaddress2"]).Value.ToString();
                                //}
                                //else if (!item.Attributes.Contains("account.emailaddress2"))
                                //{
                                //    cari.Eposta2 = "";
                                //}
                                //else
                                //{
                                //    cari.Eposta2 = ((AliasedValue)item.Attributes["account.emailaddress2"]).Value.ToString();
                                //}

                                cari.VergiDairesi = "";
                                //if (!item.Attributes.Contains("account.new_vergidairesiid"))
                                //{
                                //    cari.VergiDairesi = "";
                                //}
                                //else
                                //{
                                //    cari.VergiDairesi = ((EntityReference)((AliasedValue)item.Attributes["account.new_vergidairesiid"]).Value).Name;
                                //}

                                if (dt.Rows[i]["TcNo"] != DBNull.Value)
                                {
                                    cari.VatandaslikNo = dt.Rows[i]["TcNo"].ToString();
                                }
                                else
                                {
                                    cari.VatandaslikNo = "";
                                }
                                //else if (!item.Attributes.Contains("account.new_verginumarasi"))
                                //{
                                //    cari.VatandaslikNo = "";
                                //}
                                //else
                                //{
                                //    cari.VatandaslikNo = ((AliasedValue)item.Attributes["account.new_verginumarasi"]).Value.ToString();
                                //}

                                if (dt.Rows[i]["ProductId"] == DBNull.Value)
                                {
                                    cari.DaireKimlikNo = "";
                                }
                                else
                                {
                                    cari.DaireKimlikNo = dt.Rows[i]["ProductIdName"].ToString();
                                }

                                #endregion

                                //cari = this.GetSatisBilgileri(satisID, ref cari);

                                if (cari != null)
                                {
                                    CariList.Add(cari);
                                }
                            }
                        }
                    }
                }
                carilers = CariList;
                return carilers;
            }
            catch (Exception ex)
            {

            }
            carilers = null;
            return carilers;
        }

        public static bool CariKontrol(Cariler item)
        {
            bool retVal = false;
            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionStringLogo); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"SELECT CRMID FROM CARI WHERE CRMID=@CRMID";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@CRMID", item.CariID.ToString()) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count == 0)
                {
                    item.IslemTipi = 1;
                }
                else
                {
                    retVal = true;
                    item.IslemTipi = 2;
                }
            }
            catch (Exception ex)
            {

            }

            return retVal;
        }

        public static bool CreateCari(Cariler item, LogoAccount firma)
        {
            bool durum = true;

            int kayitStatusu = 0;

            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionStringLogo); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"INSERT INTO CARI (ISLEMTIPI,KAYITTARIHI,KAYITSTATUSU,CRMID,CARIKOD,UNVAN,ADRES1,ADRES2,ULKE,IL,ILCE,SEMT,CEPTEL1,CEPTEL2,FAX,EPOSTA1,EPOSTA2,VATANDASLIKNO,DAIREKIMLIKNO,GRUPSIRKETKODU,VERGIDAIRESI,TESLIMSEKLI,SOZLESMEDURUMU,FIRMANO,DONEMNO,PROJEKODU) VALUES (@ISLEMTIPI,@KAYITTARIHI,@KAYITSTATUSU,@CRMID,@CARIKOD,@UNVAN,@ADRES1,@ADRES2,@ULKE,@IL,@ILCE,@SEMT,@CEPTEL1,@CEPTEL2,@FAX,@EPOSTA1,@EPOSTA2,@VATANDASLIKNO,@DAIREKIMLIKNO,@GRUPSIRKETKODU,@VERGIDAIRESI,@TESLIMSEKLI,@SOZLESMEDURUMU,@FIRMANO,@DONEMNO,@PROJEKODU)";

                #endregion

                if (firma.FirmaNo == "111")
                {
                    List<SqlParameter> lstParameters111 = new List<SqlParameter>();

                    lstParameters111.Add(new SqlParameter("@ISLEMTIPI", item.IslemTipi));
                    lstParameters111.Add(new SqlParameter("@KAYITTARIHI", DateTime.Now));
                    lstParameters111.Add(new SqlParameter("@KAYITSTATUSU", kayitStatusu));
                    lstParameters111.Add(new SqlParameter("@CRMID", item.CariID));
                    lstParameters111.Add(new SqlParameter("@CARIKOD", (item.CariKod == null ? string.Empty : item.CariKod)));
                    lstParameters111.Add(new SqlParameter("@UNVAN", (item.Unvan == null ? string.Empty : item.Unvan)));
                    lstParameters111.Add(new SqlParameter("@ADRES1", (item.Adres1 == null ? string.Empty : item.Adres1)));
                    lstParameters111.Add(new SqlParameter("@GRUPSIRKETKODU", (item.GrupSirketKodu == null ? string.Empty : item.GrupSirketKodu)));
                    lstParameters111.Add(new SqlParameter("@ADRES2", (item.Adres2 == null ? string.Empty : item.Adres2)));
                    lstParameters111.Add(new SqlParameter("@ULKE", (item.Ulke == string.Empty ? string.Empty : item.Ulke)));
                    lstParameters111.Add(new SqlParameter("@IL", (item.Il == null ? string.Empty : item.Il)));
                    lstParameters111.Add(new SqlParameter("@ILCE", (item.Ilce == null ? string.Empty : item.Ilce)));
                    lstParameters111.Add(new SqlParameter("@SEMT", (item.Semt == null ? string.Empty : item.Semt)));
                    lstParameters111.Add(new SqlParameter("@CEPTEL1", (item.CepTel1 == null ? string.Empty : item.CepTel1)));
                    lstParameters111.Add(new SqlParameter("@CEPTEL2", (item.CepTel2 == null ? string.Empty : item.CepTel2)));
                    lstParameters111.Add(new SqlParameter("@FAX", (item.Fax == null ? string.Empty : item.Fax)));
                    lstParameters111.Add(new SqlParameter("@EPOSTA1", (item.Eposta1 == null ? string.Empty : item.Eposta1)));
                    lstParameters111.Add(new SqlParameter("@EPOSTA2", (item.Eposta2 == null ? string.Empty : item.Eposta2)));
                    lstParameters111.Add(new SqlParameter("@VATANDASLIKNO", (item.VatandaslikNo == null ? string.Empty : item.VatandaslikNo)));
                    lstParameters111.Add(new SqlParameter("@DAIREKIMLIKNO", (item.DaireKimlikNo == null ? string.Empty : item.DaireKimlikNo)));
                    lstParameters111.Add(new SqlParameter("@VERGIDAIRESI", (item.VergiDairesi == null ? string.Empty : item.VergiDairesi)));
                    lstParameters111.Add(new SqlParameter("@TESLIMSEKLI", (item.TeslimSekli == null ? string.Empty : item.TeslimSekli)));
                    lstParameters111.Add(new SqlParameter("@SOZLESMEDURUMU", (item.SozlesmeDurumu == null ? string.Empty : item.SozlesmeDurumu)));
                    lstParameters111.Add(new SqlParameter("@PROJEKODU", item.ProjeKodu));

                    if (firma == null)
                    {
                        lstParameters111.Add(new SqlParameter("@FIRMANO", ""));
                        lstParameters111.Add(new SqlParameter("@DONEMNO", ""));
                    }
                    else
                    {
                        lstParameters111.Add(new SqlParameter("@FIRMANO", (firma.FirmaNo == null ? string.Empty : "101")));
                        lstParameters111.Add(new SqlParameter("@DONEMNO", (firma.DonemNo == null ? string.Empty : firma.DonemNo)));
                    }

                    sda.ExecuteNonQuery(sqlQuery, lstParameters111.ToArray());
                }

                List<SqlParameter> lstParameters = new List<SqlParameter>();

                lstParameters.Add(new SqlParameter("@ISLEMTIPI", item.IslemTipi));
                lstParameters.Add(new SqlParameter("@KAYITTARIHI", DateTime.Now));
                lstParameters.Add(new SqlParameter("@KAYITSTATUSU", kayitStatusu));
                lstParameters.Add(new SqlParameter("@CRMID", item.CariID));
                lstParameters.Add(new SqlParameter("@CARIKOD", (item.CariKod == null ? string.Empty : item.CariKod)));
                lstParameters.Add(new SqlParameter("@UNVAN", (item.Unvan == null ? string.Empty : item.Unvan)));
                lstParameters.Add(new SqlParameter("@ADRES1", (item.Adres1 == null ? string.Empty : item.Adres1)));
                lstParameters.Add(new SqlParameter("@GRUPSIRKETKODU", (item.GrupSirketKodu == null ? string.Empty : item.GrupSirketKodu)));
                lstParameters.Add(new SqlParameter("@ADRES2", (item.Adres2 == null ? string.Empty : item.Adres2)));
                lstParameters.Add(new SqlParameter("@ULKE", (item.Ulke == string.Empty ? string.Empty : item.Ulke)));
                lstParameters.Add(new SqlParameter("@IL", (item.Il == null ? string.Empty : item.Il)));
                lstParameters.Add(new SqlParameter("@ILCE", (item.Ilce == null ? string.Empty : item.Ilce)));
                lstParameters.Add(new SqlParameter("@SEMT", (item.Semt == null ? string.Empty : item.Semt)));
                lstParameters.Add(new SqlParameter("@CEPTEL1", (item.CepTel1 == null ? string.Empty : item.CepTel1)));
                lstParameters.Add(new SqlParameter("@CEPTEL2", (item.CepTel2 == null ? string.Empty : item.CepTel2)));
                lstParameters.Add(new SqlParameter("@FAX", (item.Fax == null ? string.Empty : item.Fax)));
                lstParameters.Add(new SqlParameter("@EPOSTA1", (item.Eposta1 == null ? string.Empty : item.Eposta1)));
                lstParameters.Add(new SqlParameter("@EPOSTA2", (item.Eposta2 == null ? string.Empty : item.Eposta2)));
                lstParameters.Add(new SqlParameter("@VATANDASLIKNO", (item.VatandaslikNo == null ? string.Empty : item.VatandaslikNo)));
                lstParameters.Add(new SqlParameter("@DAIREKIMLIKNO", (item.DaireKimlikNo == null ? string.Empty : item.DaireKimlikNo)));
                lstParameters.Add(new SqlParameter("@VERGIDAIRESI", (item.VergiDairesi == null ? string.Empty : item.VergiDairesi)));
                lstParameters.Add(new SqlParameter("@TESLIMSEKLI", (item.TeslimSekli == null ? string.Empty : item.TeslimSekli)));
                lstParameters.Add(new SqlParameter("@SOZLESMEDURUMU", (item.SozlesmeDurumu == null ? string.Empty : item.SozlesmeDurumu)));
                lstParameters.Add(new SqlParameter("@PROJEKODU", item.ProjeKodu));

                if (firma == null)
                {
                    lstParameters.Add(new SqlParameter("@FIRMANO", ""));
                    lstParameters.Add(new SqlParameter("@DONEMNO", ""));
                }
                else
                {
                    lstParameters.Add(new SqlParameter("@FIRMANO", (firma.FirmaNo == null ? string.Empty : firma.FirmaNo)));
                    lstParameters.Add(new SqlParameter("@DONEMNO", (firma.DonemNo == null ? string.Empty : firma.DonemNo)));
                }

                sda.ExecuteNonQuery(sqlQuery, lstParameters.ToArray());
                

            }
            catch (Exception ex)
            {
                durum = false;
            }

            return durum;
        }

        public static bool SatisKontrol(Satislar item)
        {
            bool retVal = false;

            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionStringLogo); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"SELECT CRMID FROM SATIS WHERE CRMID=@CRMID";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@CRMID", item.SatisID.ToString()) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);


                if (dt.Rows.Count == 0)
                {
                    item.IslemTipi = 1;
                }
                else
                {
                    retVal = true;
                    item.IslemTipi = 2;
                }

            }
            catch (Exception ex)
            {

            }

            return retVal;
        }

        public static void CreateSatis(Satislar item, LogoAccount firma)
        {
            int kayitStatusu = 0;

            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionStringLogo); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"INSERT INTO SATIS (ISLEMTIPI,KAYITTARIHI,CRMID,KAYITSTATUSU,SATISNO,SOZLESMETARIH,CARIHESAPKODU,PROJEKIMLIGI,PROJEKODU,UNITEKODU,ISYERI,BOLUMKODU,PARABIRIMI,DOVIZKURU,INDIRIMLITUTAR,KDVORAN,KDVTUTAR,TESLIMTARIHI,SATISDURUMU,FIRMANO,DONEMNO,IPTALTARIHI) VALUES (@ISLEMTIPI,@KAYITTARIHI,@CRMID,@KAYITSTATUSU,@SATISNO,@SOZLESMETARIH,@CARIHESAPKODU,@PROJEKIMLIGI,@PROJEKODU,@UNITEKODU,@ISYERI,@BOLUMKODU,@PARABIRIMI,@DOVIZKURU,@INDIRIMLITUTAR,@KDVORAN,@KDVTUTAR,@TESLIMTARIHI,@SATISDURUMU,@FIRMANO,@DONEMNO,@IPTALTARIHI)SET @ID = SCOPE_IDENTITY();";

                #endregion

                List<SqlParameter> lstParameters = new List<SqlParameter>();

                lstParameters.Add(new SqlParameter("@ISLEMTIPI", item.IslemTipi));
                lstParameters.Add(new SqlParameter("@KAYITTARIHI", DateTime.Now));
                lstParameters.Add(new SqlParameter("@KAYITSTATUSU", kayitStatusu));
                lstParameters.Add(new SqlParameter("@CRMID", item.SatisID));
                lstParameters.Add(new SqlParameter("@SATISNO", (item.SatisNo == null ? string.Empty : item.SatisNo)));
                lstParameters.Add(new SqlParameter("@SOZLESMETARIH", item.SozlesmeTarih));
                lstParameters.Add(new SqlParameter("@CARIHESAPKODU", (item.CariHesapKodu == null ? string.Empty : item.CariHesapKodu)));
                lstParameters.Add(new SqlParameter("@PROJEKIMLIGI", (item.ProjeKimligi == null ? string.Empty : item.ProjeKimligi)));
                lstParameters.Add(new SqlParameter("@PROJEKODU", (item.ProjeKodu == null ? string.Empty : item.ProjeKodu)));
                lstParameters.Add(new SqlParameter("@UNITEKODU", (item.UniteKodu == null ? string.Empty : item.UniteKodu)));
                lstParameters.Add(new SqlParameter("@ISYERI", (item.IsYeri == null ? string.Empty : item.IsYeri)));
                lstParameters.Add(new SqlParameter("@BOLUMKODU", (item.BolumKodu == null ? string.Empty : item.BolumKodu)));
                lstParameters.Add(new SqlParameter("@PARABIRIMI", (item.ParaBirimi == null ? string.Empty : item.ParaBirimi)));
                lstParameters.Add(new SqlParameter("@DOVIZKURU", item.DovizKuru));
                lstParameters.Add(new SqlParameter("@INDIRIMLITUTAR", item.IndirimliTutar));
                lstParameters.Add(new SqlParameter("@KDVORAN", item.KdvOran));
                lstParameters.Add(new SqlParameter("@KDVTUTAR", item.KdvTutar));
                lstParameters.Add(new SqlParameter("@TESLIMTARIHI", (!item.TeslimTarihi.HasValue ? new DateTime?(DateTime.MinValue) : item.TeslimTarihi)));
                lstParameters.Add(new SqlParameter("@SATISDURUMU", (item.SatisDurumu == null ? string.Empty : item.SatisDurumu)));
                lstParameters.Add(new SqlParameter("@IPTALTARIHI", item.ModifiedOn));

                if (firma == null)
                {
                    lstParameters.Add(new SqlParameter("@FIRMANO", ""));
                    lstParameters.Add(new SqlParameter("@DONEMNO", ""));
                }
                else
                {
                    lstParameters.Add(new SqlParameter("@FIRMANO", (firma.FirmaNo == null ? string.Empty : firma.FirmaNo)));
                    lstParameters.Add(new SqlParameter("@DONEMNO", (firma.DonemNo == null ? string.Empty : firma.DonemNo)));
                }

                SqlParameter parOut = new SqlParameter("@ID", SqlDbType.Int);
                parOut.Direction = ParameterDirection.Output;

                lstParameters.Add(parOut);


                SqlCommand returnCmd = sda.ExecuteNonQueryReturnsCommand(sqlQuery, lstParameters.ToArray());

                item.SatisRef = (int)returnCmd.Parameters["@ID"].Value;
            }
            catch (Exception ex)
            {

            }

        }

        public static void LogoAktarimTrue(Guid Id, string EntityName, IOrganizationService service)
        {
            try
            {
                if (Id != Guid.Empty && !string.IsNullOrEmpty(EntityName))
                {
                    Entity ent = new Entity(EntityName)
                    {
                        Id = Id
                    };

                    ent.Attributes.Add("new_islogotransferred", true);

                    service.Update(ent);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void LogoAktarimFalse(Guid Id, string EntityName, IOrganizationService service)
        {
            try
            {
                if (Id != Guid.Empty && !string.IsNullOrEmpty(EntityName))
                {
                    Entity ent = new Entity(EntityName)
                    {
                        Id = Id
                    };

                    ent.Attributes.Add("new_islogointegration", false);

                    service.Update(ent);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void UpdateLogoAktarimStatus(Guid Id, string value)
        {
            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                #region | SQL QUERY |

                string sqlQuery = @"UPDATE Quote SET new_islogotransferred=" + value + " WHERE QuoteId='" + Id.ToString() + "'";

                #endregion


                sda.ExecuteNonQuery(sqlQuery);

                //if (Id != Guid.Empty && !string.IsNullOrEmpty(EntityName))
                //{
                //    Entity ent = new Entity(EntityName)
                //    {
                //        Id = Id
                //    };

                //    ent.Attributes.Add(attributeName, value);

                //    service.Update(ent);
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public static void ExecuteSatisOdemePlani(Guid Id, int SatisRef, SqlDataAccess sda)
        {
            //SatisOdemePlanlari item = null;

            try
            {
                List<SatisOdemePlanlari> satisOdemePlanList = LogoHelper.GetSatisOdemePlaniList(Id, SatisRef, sda);
                List<SatisOdemePlanlari> satisKaporaPlanList = LogoHelper.GetSatisKaporaList(Id, SatisRef, sda);

                if ((satisOdemePlanList == null ? false : satisOdemePlanList.Count > 0))
                {
                    foreach (SatisOdemePlanlari item in satisOdemePlanList)
                    {
                        if (!LogoHelper.SatisOdemePlanKontrol(item))
                        {
                            LogoHelper.CreateSatisOdemePlani(item);
                        }
                        else
                        {
                            LogoHelper.CreateSatisOdemePlani(item);
                        }
                    }
                }

                if ((satisKaporaPlanList == null ? false : satisKaporaPlanList.Count > 0))
                {
                    foreach (SatisOdemePlanlari satisOdemePlanlari in satisKaporaPlanList)
                    {
                        if (!LogoHelper.SatisOdemePlanKontrol(satisOdemePlanlari))
                        {
                            LogoHelper.CreateSatisOdemePlani(satisOdemePlanlari);
                        }
                        else
                        {
                            LogoHelper.CreateSatisOdemePlani(satisOdemePlanlari);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static bool SatisOdemePlanKontrol(SatisOdemePlanlari item)
        {
            bool retVal = false;

            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"SELECT CRMID FROM SATISODEMEPLANI WHERE CRMID=@CRMID";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@CRMID", item.SenetId.ToString()) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count == 0)
                {
                    item.IslemTipi = 1;
                }
                else
                {
                    retVal = true;
                    item.IslemTipi = 2;
                }

            }
            catch (Exception ex)
            {

            }

            return retVal;
        }

        public static void CreateSatisOdemePlani(SatisOdemePlanlari item)
        {
            try
            {
                int kayitStatus = 0;

                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionStringLogo); //LOGODB CONNSTR

                #region | SQL QUERY |

                string sqlQuery = @"INSERT INTO SATISODEMEPLANI (ISLEMTIPI,KAYITTARIHI,KAYITSTATUSU,CRMID,SATISID,VADETARIHI,SATISREF,ODEMETUTARI,ONAYNO,CARIHESAPKODU,ALACAKCARIHESAPKODU,MAKBUZNO,SENETNO)VALUES (@ISLEMTIPI,@KAYITTARIHI,@KAYITSTATUSU,@CRMID,@SATISID,@VADETARIHI,@SATISREF,@ODEMETUTARI,@ONAYNO,@CARIHESAPKODU,@ALACAKCARIHESAPKODU,@MAKBUZNO,@SENETNO)";

                #endregion

                List<SqlParameter> lstParameters = new List<SqlParameter>();

                lstParameters.Add(new SqlParameter("@ISLEMTIPI", item.IslemTipi));
                lstParameters.Add(new SqlParameter("@KAYITTARIHI", DateTime.Now));
                lstParameters.Add(new SqlParameter("@KAYITSTATUSU", kayitStatus));
                lstParameters.Add(new SqlParameter("@CRMID", item.SenetId));
                lstParameters.Add(new SqlParameter("@SATISID", item.SatisId));
                lstParameters.Add(new SqlParameter("@VADETARIHI", item.VadeTarihi));
                lstParameters.Add(new SqlParameter("@SATISREF", item.SatisRef));
                lstParameters.Add(new SqlParameter("@ODEMETUTARI", item.Tutar));
                lstParameters.Add(new SqlParameter("@ONAYNO", item.OnayNO));
                lstParameters.Add(new SqlParameter("@CARIHESAPKODU", item.CariHesapKodu));
                lstParameters.Add(new SqlParameter("@ALACAKCARIHESAPKODU", item.AlacakCariHesapKodu));
                lstParameters.Add(new SqlParameter("@MAKBUZNO", item.OnayKodu));
                lstParameters.Add(new SqlParameter("@SENETNO", item.SenetNo));

                sda.ExecuteNonQuery(sqlQuery, lstParameters.ToArray());

            }
            catch (Exception ex)
            {

            }
        }

        public static List<SatisOdemePlanlari> GetSatisOdemePlaniList(Guid Id, int SatisRef, SqlDataAccess sda)
        {
            List<SatisOdemePlanlari> satisOdemePlanlaris;
            DateTime? vadeTarihi;
            DateTime value;
            bool hour;

            try
            {
                List<SatisOdemePlanlari> SatisOdemeList = new List<SatisOdemePlanlari>();

                #region | SQL QUERY |

                string sqlQuery = @"DECLARE @paymentTypeCode INT
                                    SELECT TOP 1
	                                    @paymentTypeCode=e.ObjectTypeCode
                                    FROM
	                                    Entity AS e (NOLOCK)
                                    WHERE
	                                    e.Name='new_payment'

                                    SELECT
	                                    pay.new_paymentId AS Id
	                                    ,pay.new_date AS PaymentDate
	                                    ,pay.new_quoteid AS QouteId
	                                    ,pay.new_quoteidName AS QuoteIdName
	                                    ,pay.new_vtype AS SenetTipi
	                                    ,pay.new_vnumber AS SenetNo
	                                    ,pay.new_type AS VType
	                                    ,pay.new_itype AS PaymentType
                                        ,sm.Value AS PaymentTypeValue
	                                    ,pay.new_paymentamount AS WaitedPayment
	                                    ,q.new_financialaccountid AS FinancialAccountId
	                                    ,q.new_financialaccountidName AS FinancialAccountIdName
	                                    ,p.new_projectid AS ProjectId
	                                    ,p.new_projectidName AS ProjectIdName
                                        ,pro.new_creditcontactaccountcode AS AlacakCariHesapKodu
                                    FROM
	                                    new_payment AS pay (NOLOCK)
		                                    JOIN
			                                    Quote AS q (NOLOCK)
				                                    ON
				                                    pay.new_quoteid=q.QuoteId
		                                    JOIN
			                                    QuoteDetail AS qd (NOLOCK)
				                                    ON
				                                    q.QuoteId=qd.QuoteId
		                                    JOIN
			                                    Product AS p (NOLOCK)
				                                    ON
				                                    qd.ProductId=p.ProductId
		                                    JOIN
			                                    new_project AS pro (NOLOCK)
				                                    ON
				                                    p.new_projectid=pro.new_projectId
                                            LEFT JOIN
			                                    StringMap AS sm (NOLOCK)
				                                    ON
				                                    sm.ObjectTypeCode=@paymentTypeCode
				                                    AND
				                                    sm.AttributeName='new_itype'
				                                    AND
				                                    sm.AttributeValue=pay.new_itype

                                    WHERE
	                                    pay.new_quoteid='{0}'
                                    AND
										pay.new_collaborateaccountid != 'B3A17FFD-C5B1-E411-80C7-005056A60603'--iş gyo ortaklıkları alınmayacak
                                    --AND
	                                    --pay.new_isvoucher=1
                                    --AND
	                                    --pay.new_sign=1";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, Id));

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SatisOdemePlanlari odeme = new SatisOdemePlanlari()
                        {
                            SenetId = (Guid)dt.Rows[i]["Id"]
                        };

                        string cari = string.Empty;
                        if (dt.Rows[i]["FinancialAccountId"] != DBNull.Value)
                        {
                            cari = dt.Rows[i]["FinancialAccountIdName"].ToString();
                            cari = cari.Substring(3);
                        }

                        if (dt.Rows[i]["QouteId"] == DBNull.Value)
                        {
                            odeme.SatisId = Guid.Empty;
                        }
                        else
                        {
                            odeme.SatisId = (Guid)dt.Rows[i]["QouteId"];
                        }

                        if (dt.Rows[i]["PaymentDate"] == DBNull.Value)
                        {
                            vadeTarihi = null;
                            odeme.VadeTarihi = vadeTarihi;
                        }
                        else
                        {
                            odeme.VadeTarihi = (DateTime)dt.Rows[i]["PaymentDate"];
                            vadeTarihi = odeme.VadeTarihi;
                            if (vadeTarihi.Value.Hour > 21)
                            {
                                hour = false;
                            }
                            else
                            {
                                vadeTarihi = odeme.VadeTarihi;
                                value = vadeTarihi.Value;
                                hour = value.Hour != 21;
                            }
                            if (!hour)
                            {
                                vadeTarihi = odeme.VadeTarihi;
                                value = vadeTarihi.Value;
                                odeme.VadeTarihi = new DateTime?(value.AddHours(3));
                            }
                        }

                        Guid satisId = odeme.SatisId;
                        odeme.SatisRef = SatisRef;
                        if (dt.Rows[i]["WaitedPayment"] == DBNull.Value)
                        {
                            odeme.Tutar = new decimal(0);
                        }
                        else
                        {
                            odeme.Tutar = (decimal)dt.Rows[i]["WaitedPayment"];
                        }

                        if (dt.Rows[i]["SenetNo"] == DBNull.Value)
                        {
                            odeme.SenetNo = "";
                        }
                        else
                        {
                            odeme.SenetNo = dt.Rows[i]["SenetNo"].ToString();
                        }

                        if (dt.Rows[i]["ProjectId"] != DBNull.Value)
                        {
                            if (dt.Rows[i]["AlacakCariHesapKodu"] == DBNull.Value)
                            {
                                odeme.AlacakCariHesapKodu = "";
                            }
                            else
                            {
                                odeme.AlacakCariHesapKodu = dt.Rows[i]["AlacakCariHesapKodu"].ToString();
                            }
                        }

                        if (dt.Rows[i]["PaymentType"] == DBNull.Value)
                        {
                            odeme.OnayNO = "";
                            odeme.OnayKodu = "";
                        }
                        else
                        {
                            odeme.OnayNO = dt.Rows[i]["PaymentTypeValue"].ToString();

                            if (dt.Rows[i]["SenetTipi"] != DBNull.Value)
                            {
                                switch ((int)dt.Rows[i]["SenetTipi"])
                                {
                                    case 1:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO81";
                                            break;
                                        }
                                    case 2:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO82";
                                            break;
                                        }
                                    case 3:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO83";
                                            break;
                                        }
                                    case 4:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO85";
                                            break;
                                        }
                                    case 5:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO86";
                                            break;
                                        }
                                    case 6:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO84";
                                            break;
                                        }
                                    case 7:
                                        {
                                            odeme.OnayKodu = "ZMH00TAO76";
                                            odeme.CariHesapKodu = string.Concat("127", cari);

                                            if (dt.Rows[i]["PaymentType"] == DBNull.Value)
                                            {
                                                odeme.OnayNO = "";
                                            }
                                            else if ((int)dt.Rows[i]["PaymentType"] == 6)
                                            {
                                                odeme.OnayNO = "Damga Vergisi - Senet";
                                            }
                                            else if ((int)dt.Rows[i]["PaymentType"] != 7)
                                            {
                                                odeme.OnayNO = "";
                                            }
                                            else
                                            {
                                                odeme.OnayNO = "Damga Vergisi - Virman";
                                            }
                                            break;
                                        }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(odeme.CariHesapKodu))
                        {
                            odeme.CariHesapKodu = string.Concat("340", cari);
                        }

                        SatisOdemeList.Add(odeme);
                    }
                }

                satisOdemePlanlaris = SatisOdemeList;
                return satisOdemePlanlaris;
            }
            catch (Exception ex)
            {

            }

            satisOdemePlanlaris = null;
            return satisOdemePlanlaris;
        }

        public static List<SatisOdemePlanlari> GetSatisKaporaList(Guid Id, int SatisRef, SqlDataAccess sda)
        {
            List<SatisOdemePlanlari> satisOdemePlanlaris;
            DateTime? vadeTarihi;
            DateTime value;
            bool hour;

            try
            {

                List<SatisOdemePlanlari> SatisOdemeList = new List<SatisOdemePlanlari>();

                #region | SQL QUERY |

                string sqlQuery = @"DECLARE @paymentTypeCode INT
                                    SELECT TOP 1
	                                    @paymentTypeCode=e.ObjectTypeCode
                                    FROM
	                                    Entity AS e (NOLOCK)
                                    WHERE
	                                    e.Name='new_payment'

                                    SELECT
	                                    pay.new_paymentId AS Id
	                                    ,pay.new_date AS PaymentDate
	                                    ,pay.new_quoteid AS QouteId
	                                    ,pay.new_quoteidName AS QuoteIdName
	                                    ,pay.new_vtype AS SenetTipi
	                                    ,pay.new_vnumber AS SenetNo
	                                    ,pay.new_type AS VType
	                                    ,pay.new_itype AS PaymentType
                                        ,sm.Value AS PaymentTypeValue
	                                    ,pay.new_paymentamount AS WaitedPayment
	                                    ,q.new_financialaccountid AS FinancialAccountId
	                                    ,q.new_financialaccountidName AS FinancialAccountIdName
	                                    ,p.new_projectid AS ProjectId
	                                    ,p.new_projectidName AS ProjectIdName
                                        ,pro.new_creditcontactaccountcode AS AlacakCariHesapKodu
                                        ,pay.new_paymentamount AS Amount
                                    FROM
	                                    new_payment AS pay (NOLOCK)
		                                    JOIN
			                                    Quote AS q (NOLOCK)
				                                    ON
				                                    pay.new_quoteid=q.QuoteId
		                                    JOIN
			                                    QuoteDetail AS qd (NOLOCK)
				                                    ON
				                                    q.QuoteId=qd.QuoteId
		                                    JOIN
			                                    Product AS p (NOLOCK)
				                                    ON
				                                    qd.ProductId=p.ProductId
		                                    JOIN
			                                    new_project AS pro (NOLOCK)
				                                    ON
				                                    p.new_projectid=pro.new_projectId
                                           LEFT JOIN
			                                    StringMap AS sm (NOLOCK)
				                                    ON
				                                    sm.ObjectTypeCode=@paymentTypeCode
				                                    AND
				                                    sm.AttributeName='new_itype'
				                                    AND
				                                    sm.AttributeValue=pay.new_itype

                                    WHERE
	                                    pay.new_quoteid='{0}'
                                    AND 
                                        pay.new_type=4 --Kapora 
                                    AND 
                                        pay.StatusCode=100000000 --Kapora Alındı
                                    AND
										pay.new_collaborateaccountid != 'B3A17FFD-C5B1-E411-80C7-005056A60603' -- işgyo";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, Id));

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SatisOdemePlanlari odeme = new SatisOdemePlanlari()
                        {
                            SenetId = (Guid)dt.Rows[i]["Id"]
                        };

                        if (dt.Rows[i]["QouteId"] == DBNull.Value)
                        {
                            odeme.SatisId = Guid.Empty;
                        }
                        else
                        {
                            odeme.SatisId = (Guid)dt.Rows[i]["QouteId"];
                        }

                        if (dt.Rows[i]["PaymentDate"] == DBNull.Value)
                        {
                            vadeTarihi = null;
                            odeme.VadeTarihi = vadeTarihi;
                        }
                        else
                        {
                            odeme.VadeTarihi = (DateTime)dt.Rows[i]["PaymentDate"];
                            vadeTarihi = odeme.VadeTarihi;
                            if (vadeTarihi.Value.Hour > 21)
                            {
                                hour = false;
                            }
                            else
                            {
                                vadeTarihi = odeme.VadeTarihi;
                                value = vadeTarihi.Value;
                                hour = value.Hour != 21;
                            }
                            if (!hour)
                            {
                                vadeTarihi = odeme.VadeTarihi;
                                value = vadeTarihi.Value;
                                odeme.VadeTarihi = new DateTime?(value.AddHours(3));
                            }
                        }

                        if (dt.Rows[i]["Amount"] == DBNull.Value)
                        {
                            odeme.Tutar = new decimal(0);
                        }
                        else
                        {
                            odeme.Tutar = (decimal)dt.Rows[i]["Amount"];
                        }

                        Guid satisId = odeme.SatisId;
                        odeme.SatisRef = SatisRef;
                        if (dt.Rows[i]["SenetNo"] == DBNull.Value)
                        {
                            odeme.SenetNo = "";
                        }
                        else
                        {
                            odeme.SenetNo = dt.Rows[i]["SenetNo"].ToString();
                        }


                        if (dt.Rows[i]["PaymentType"] == DBNull.Value)
                        {
                            odeme.OnayNO = "";
                        }
                        else
                        {
                            odeme.OnayNO = dt.Rows[i]["PaymentTypeValue"].ToString();
                        }

                        odeme.OnayKodu = "ZMH00TAO80";

                        if (dt.Rows[i]["ProjectId"] != DBNull.Value)
                        {
                            if (dt.Rows[i]["AlacakCariHesapKodu"] == DBNull.Value)
                            {
                                odeme.AlacakCariHesapKodu = "";
                            }
                            else
                            {
                                odeme.AlacakCariHesapKodu = dt.Rows[i]["AlacakCariHesapKodu"].ToString();
                            }
                        }

                        if (dt.Rows[i]["FinancialAccountId"] == DBNull.Value)
                        {
                            odeme.CariHesapKodu = "";
                        }
                        else
                        {
                            odeme.CariHesapKodu = string.Concat("340", dt.Rows[i]["FinancialAccountIdName"].ToString().Substring(3));
                        }

                        SatisOdemeList.Add(odeme);
                    }
                }

                satisOdemePlanlaris = SatisOdemeList;
                return satisOdemePlanlaris;
            }
            catch (Exception ex)
            {

            }

            satisOdemePlanlaris = null;
            return satisOdemePlanlaris;
        }

        public static List<PaymentLogo> GetPaymentsFromLogo(string condiotion)
        {
            

            List<PaymentLogo> paymentList = new List<PaymentLogo>();

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionStringLogo2); //LOGODB ÖDENEN SENETLER CONNSTR

            #region | SQL QUERY |

            string sqlQuery = @"SELECT 
	                                SERI_NO
	                                ,DOVIZSTR
	                                ,SENET_TUTAR
	                                ,KAPANAN_TUTAR
	                                ,KALAN_TUTAR
	                                ,STATU
	                                ,ISLEM_TARIHI
                                FROM 
	                                LG_101_SENET_KAPAMA_DURUMU
                                WHERE 				
	                                SERI_NO IS NOT NULL 
                                AND 
	                                SERI_NO != ''
	                            AND 
								    SERI_NO IN ({0})";

            
            #endregion | SQL QUERY |
            DataTable dt = sda.getDataTable(string.Format(sqlQuery,condiotion));

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PaymentLogo pl = new PaymentLogo();
                    pl.Amount = dr["KAPANAN_TUTAR"] != DBNull.Value ? Convert.ToDecimal(dr["KAPANAN_TUTAR"]) : 0;
                    pl.BalanceAmount = dr["KALAN_TUTAR"] != DBNull.Value ? Convert.ToDecimal(dr["KALAN_TUTAR"]) : 0;
                    pl.Status = dr["STATU"] != DBNull.Value ? dr["STATU"].ToString() : string.Empty;
                    pl.VoucherAmount = dr["SENET_TUTAR"] != DBNull.Value ? Convert.ToDecimal(dr["SENET_TUTAR"]) : 0;
                    pl.VoucherNumber = dr["SERI_NO"] != DBNull.Value ? dr["SERI_NO"].ToString() : string.Empty;
                    pl.TransactionCurrencyName = dr["DOVIZSTR"] != DBNull.Value ? dr["DOVIZSTR"].ToString() : string.Empty;
                    Console.WriteLine(pl.VoucherNumber);
                    paymentList.Add(pl);

                }

            }
            return paymentList;
        }


        public static List<PaymentLogo> GetPaymentsFromLogo()
        {
            string filePath = AppDomain.CurrentDomain + "Logo.txt";
           
            List<PaymentLogo> paymentList = new List<PaymentLogo>();

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionStringLogo2); //LOGODB ÖDENEN SENETLER CONNSTR

            #region | SQL QUERY |

            string sqlQuery = @"CREATE TABLE  #ciftTemp (SERI_NO NVARCHAR(50)
						                                ,DOVIZSTR NVARCHAR(50)
						                                ,SENET_TUTAR DECIMAL(18,2)
						                                ,KAPANAN_TUTAR DECIMAL(18,2)
						                                ,KALAN_TUTAR DECIMAL(18,2)
						                                ,STATU NVARCHAR(50)
						                                ,ISLEM_TARIHI DATETIME)
						
                                CREATE TABLE  #tekTemp (SERI_NO NVARCHAR(50)
						                                ,DOVIZSTR NVARCHAR(50)
						                                ,SENET_TUTAR DECIMAL(18,2)
						                                ,KAPANAN_TUTAR DECIMAL(18,2)
						                                ,KALAN_TUTAR DECIMAL(18,2)
						                                ,STATU NVARCHAR(50)
						                                ,ISLEM_TARIHI DATETIME) 

                                CREATE TABLE  #kaTemp (SERI_NO NVARCHAR(50)
						                                ,DOVIZSTR NVARCHAR(50)
						                                ,SENET_TUTAR DECIMAL(18,2)
						                                ,KAPANAN_TUTAR DECIMAL(18,2)
						                                ,KALAN_TUTAR DECIMAL(18,2)
						                                ,STATU NVARCHAR(50)
						                                ,ISLEM_TARIHI DATETIME) 
		
                                CREATE TABLE #notKaTemp (SERI_NO NVARCHAR(50)
						                                ,DOVIZSTR NVARCHAR(50)
						                                ,SENET_TUTAR DECIMAL(18,2)
						                                ,KAPANAN_TUTAR DECIMAL(18,2)
						                                ,KALAN_TUTAR DECIMAL(18,2)
						                                ,STATU NVARCHAR(50)
						                                ,ISLEM_TARIHI DATETIME) 
	
                                CREATE TABLE #allTemp (SERI_NO NVARCHAR(50)
						                                ,DOVIZSTR NVARCHAR(50)
						                                ,SENET_TUTAR DECIMAL(18,2)
						                                ,KAPANAN_TUTAR DECIMAL(18,2)
						                                ,KALAN_TUTAR DECIMAL(18,2)
						                                ,STATU NVARCHAR(50)
						                                ,ISLEM_TARIHI DATETIME) 					
				
                                --ÇİFT OLMAYAN KAYITLAR,
                                INSERT INTO #tekTemp (SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR,STATU,ISLEM_TARIHI)
                                SELECT 
	                                SERI_NO
	                                ,DOVIZSTR
	                                ,SENET_TUTAR
	                                ,KAPANAN_TUTAR
	                                ,KALAN_TUTAR
	                                ,STATU
	                                ,ISLEM_TARIHI
                                FROM 
	                                LG_101_SENET_KAPAMA_DURUMU
                                WHERE 				
	                                SERI_NO IS NOT NULL 
                                AND 
	                                SERI_NO != ''
                                AND
	                                SERI_NO NOT IN (SELECT
							                                SERI_NO
						                                FROM 
							                                LG_101_SENET_KAPAMA_DURUMU AS LG
						                                WHERE
							
							                                LG.SERI_NO IS NOT NULL 
						                                AND 
							                                LG.SERI_NO != ''
						                                 GROUP BY 
							                                LG.SERI_NO 
						                                HAVING 
							                                COUNT(*) > 1)
                                --ÇİFT OLAN KAYITLAR
                                INSERT INTO #ciftTemp(SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR,STATU,ISLEM_TARIHI)
                                SELECT 
	                                SERI_NO
	                                ,DOVIZSTR
	                                ,SENET_TUTAR
	                                ,KAPANAN_TUTAR
	                                ,KALAN_TUTAR
	                                ,STATU
	                                ,ISLEM_TARIHI
                                FROM 
	                                LG_101_SENET_KAPAMA_DURUMU
                                WHERE 				
	                                SERI_NO IS NOT NULL 
                                AND 
	                                SERI_NO != ''
                                AND
	                                SERI_NO  IN (SELECT
							                                SERI_NO
						                                FROM 
							                                LG_101_SENET_KAPAMA_DURUMU AS LG
						                                WHERE
							
							                                LG.SERI_NO IS NOT NULL 
						                                AND 
							                                LG.SERI_NO != ''
						                                 GROUP BY 
							                                LG.SERI_NO 
						                                HAVING 
							                                COUNT(*) > 1)
                                ORDER BY SERI_NO                           
                                --ÇİFT OLAN KAYITLARDAN KA OLANLAR 
                                INSERT INTO #kaTemp (SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR,STATU,ISLEM_TARIHI)
	                                SELECT 
	                                SERI_NO
	                                ,DOVIZSTR
	                                ,SENET_TUTAR
	                                ,KAPANAN_TUTAR
	                                ,KALAN_TUTAR
	                                ,STATU
	                                ,ISLEM_TARIHI
                                FROM 
	                                LG_101_SENET_KAPAMA_DURUMU
                                WHERE 				
	                                SERI_NO IS NOT NULL 
                                AND 
	                                SERI_NO != ''
                                AND
	                                STATU = 'KA' OR  STATU = 'PK'
                                AND
	                                SERI_NO collate Turkish_CI_AS  IN (SELECT CT.SERI_NO FROM #ciftTemp AS CT )

                                INSERT INTO #notKaTemp (SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR	,STATU,ISLEM_TARIHI)
                                select * from #ciftTemp AS CIFT WHERE CIFT.SERI_NO NOT IN (SELECT KA.SERI_NO FROM #kaTemp AS KA)

                                --UNIQUE
                                INSERT INTO #allTemp (SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR,STATU,ISLEM_TARIHI)
                                SELECT * FROM #tekTemp

                                INSERT INTO #allTemp (SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR,STATU,ISLEM_TARIHI)
                                SELECT * FROM #kaTemp

                                INSERT INTO #allTemp (SERI_NO,DOVIZSTR,SENET_TUTAR,KAPANAN_TUTAR,KALAN_TUTAR,STATU,ISLEM_TARIHI)
                                SELECT * FROM #notKaTemp

                                select * from #allTemp order by SERI_NO ";

            #endregion | SQL QUERY |
            DataTable dt = sda.getDataTable(string.Format(sqlQuery));

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PaymentLogo pl = new PaymentLogo();
                    pl.Amount = dr["KAPANAN_TUTAR"] != DBNull.Value ? Convert.ToDecimal(dr["KAPANAN_TUTAR"]) : 0;
                    pl.BalanceAmount = dr["KALAN_TUTAR"] != DBNull.Value ? Convert.ToDecimal(dr["KALAN_TUTAR"]) : 0;
                    pl.Status = dr["STATU"] != DBNull.Value ? dr["STATU"].ToString() : string.Empty;
                    pl.VoucherAmount = dr["SENET_TUTAR"] != DBNull.Value ? Convert.ToDecimal(dr["SENET_TUTAR"]) : 0;
                    pl.VoucherNumber = dr["SERI_NO"] != DBNull.Value ? dr["SERI_NO"].ToString() : string.Empty;
                    pl.TransactionCurrencyName = dr["DOVIZSTR"] != DBNull.Value ? dr["DOVIZSTR"].ToString() : string.Empty;
                    Console.WriteLine(pl.VoucherNumber);
                    paymentList.Add(pl);

                }

            }
            return paymentList;
        }

        public static PaymentLogo GetPaymentFromCrm(SqlDataAccess sda, string VoucherNumber)
        {
            PaymentLogo crm = new PaymentLogo();
            if (VoucherNumber == string.Empty)
                return crm;

            #region SQL Query
            string sqlQuery = @"SELECT
                                P.new_paymentId AS CrmId
                                ,P.new_vnumber AS VoucherNumber
                                ,P.new_paymentamount AS VoucherAmount
                                ,P.new_amount AS Amount
                                ,P.new_balanceamount AS BalanceAmount
                                ,P.new_vstatus AS Status
                                ,P.TransactionCurrencyIdName AS TransactionCurrencyName
                                ,P.new_type
                                FROM
                                new_payment P WITH (NOLOCK) 
                                JOIN 
                                quote Q WITH (NOLOCK) 
                                ON 
                                Q.quoteId = P.new_quoteid
                                where 
                                 Q.StatusCode <> 7 and P.StateCode = 0 AND
                                P.new_vnumber='{0}'";

            sqlQuery = string.Format(sqlQuery, VoucherNumber);
            DataTable dt = sda.getDataTable(sqlQuery);

            #endregion SQL Query

            if (dt.Rows.Count == 1)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    crm.CrmId = dr["CrmId"] != DBNull.Value ? dr["CrmId"].ToString() : string.Empty;
                    crm.VoucherNumber = VoucherNumber;
                    crm.VoucherAmount = dr["VoucherAmount"] != DBNull.Value ? Convert.ToDecimal(dr["VoucherAmount"]) : 0;
                    crm.Amount = dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0;
                    crm.BalanceAmount = dr["BalanceAmount"] != DBNull.Value ? Convert.ToDecimal(dr["BalanceAmount"]) : 0;
                    crm.Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : string.Empty;
                    crm.TransactionCurrencyName = dr["TransactionCurrencyName"] != DBNull.Value ? dr["TransactionCurrencyName"].ToString() : string.Empty;
                }
            }
            return crm;
        }

        public static void UpdatePaymentCrm(SqlDataAccess sda, PaymentLogo item)
        {

            #region SQL QUERY
            string sqlQuery = @"UPDATE
	                                   new_payment
                                    SET
	                                   new_vstatus=@Status,new_balanceamount=@BalanceAmount,new_balanceamount_base=@BalanceAmount,new_amount=@Amount,new_amount_base=@Amount
                                    WHERE
	                                    new_paymentId='{0}'";
            #endregion
            SqlParameter[] parameters = new SqlParameter[3];
            switch (item.Status)
            {
                case "AC": parameters[0] = new SqlParameter("Status", 23);
                    break;

                case "KA": parameters[0] = new SqlParameter("Status", 24);
                    break;

                case "PK": parameters[0] = new SqlParameter("Status", 25);
                    break;
            }
            parameters[1] = new SqlParameter("BalanceAmount", item.BalanceAmount);
            parameters[2] = new SqlParameter("Amount", item.Amount);
            sda.ExecuteNonQuery(string.Format(sqlQuery, item.CrmId), parameters);

        }
    }
}
