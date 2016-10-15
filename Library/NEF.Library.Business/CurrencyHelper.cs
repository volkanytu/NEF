using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class CurrencyHelper
    {
        public static MsCrmResultObject GetExchangeRate(DateTime exchangeDate, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    er.new_exchangerateId AS Id
	                                    ,er.new_currencyid AS CurrencyId
	                                    ,er.new_currencyidName AS CurrencyIdName
	                                    ,er.new_salesrate AS SaleRate
	                                    ,er.new_buyingrate AS BuyRate
	                                    ,er.new_currencydate
                                    FROM
                                        new_exchangerate AS er (NOLOCK)
                                    WHERE
                                        CAST(DATEADD(HH,3,er.new_currencydate) AS DATE)=CAST(@ratedate AS DATE)";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ratedate", exchangeDate) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    List<ExchangeRate> lstRates = new List<ExchangeRate>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ExchangeRate eRate = new ExchangeRate()
                        {
                            Id = (Guid)dt.Rows[i]["Id"],
                            Currency = new EntityReference()
                            {
                                Id = (Guid)dt.Rows[i]["CurrencyId"],
                                Name = dt.Rows[i]["CurrencyIdName"].ToString(),
                                LogicalName = "transactioncurrency"
                            },
                            RateDate = exchangeDate,
                            SaleRate = (decimal)dt.Rows[i]["SaleRate"],
                            BuyRate = (decimal)dt.Rows[i]["BuyRate"]
                        };

                        lstRates.Add(eRate);
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lstRates;
                    returnValue.Result = "Döviz kurları çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetExchangeRateByCurrency(DateTime exchangeDate, Guid currencyId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    er.new_exchangerateId AS Id
	                                    ,er.new_currencyid AS CurrencyId
	                                    ,er.new_currencyidName AS CurrencyIdName
	                                    ,er.new_salesrate AS SaleRate
	                                    ,er.new_buyingrate AS BuyRate
	                                    ,er.new_currencydate
                                    FROM
                                        new_exchangerate AS er (NOLOCK)
                                    WHERE
                                        CAST(DATEADD(HH,3,er.new_currencydate) AS DATE)=CAST(@ratedate AS DATE) AND er.new_currencyid=@currencyId ";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ratedate", exchangeDate), new SqlParameter("@currencyId", currencyId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    ExchangeRate eRate = new ExchangeRate()
                    {
                        Id = (Guid)dt.Rows[0]["Id"],
                        Currency = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["CurrencyId"],
                            Name = dt.Rows[0]["CurrencyIdName"].ToString(),
                            LogicalName = "transactioncurrency"
                        },
                        RateDate = exchangeDate,
                        SaleRate = (decimal)dt.Rows[0]["SaleRate"],
                        BuyRate = (decimal)dt.Rows[0]["BuyRate"]
                    };

                    returnValue.Success = true;
                    returnValue.ReturnObject = eRate;
                    returnValue.Result = "Döviz kurları çekildi.";
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "İstediğiniz tarihe ait kur bilgileri alınamadı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject ConvertCurrency(Guid sourceCurrencyId, decimal sourceValue, Guid targetCurrencyId, DateTime exchangeDate, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            returnValue.ReturnObject = sourceValue;

            MsCrmResultObject resultRates = CurrencyHelper.GetExchangeRate(exchangeDate, sda);

            if (resultRates.Success)
            {
                List<ExchangeRate> lstRates = (List<ExchangeRate>)resultRates.ReturnObject;

                if (sourceCurrencyId != targetCurrencyId)
                {
                    if (sourceCurrencyId == Globals.CurrencyIdTL) //TL'den Dövize ise
                    {
                        decimal targetRate = lstRates.Find(x => x.Currency.Id == targetCurrencyId).SaleRate;
                        returnValue.ReturnObject = sourceValue / targetRate;

                        returnValue.Success = true;
                    }
                    else if (targetCurrencyId == Globals.CurrencyIdTL) //Döziden TL'ye ise
                    {
                        decimal sourceRate = lstRates.Find(x => x.Currency.Id == sourceCurrencyId).SaleRate;
                        returnValue.ReturnObject = sourceValue * sourceRate;

                        returnValue.Success = true;
                    }
                    else //Dövizden dövize ise
                    {
                        decimal sourceRate = lstRates.Find(x => x.Currency.Id == sourceCurrencyId).SaleRate;
                        decimal targetRate = lstRates.Find(x => x.Currency.Id == targetCurrencyId).SaleRate;

                        returnValue.ReturnObject = sourceValue * (sourceRate / targetRate);

                        returnValue.Success = true;
                    }
                }
                else
                {
                    returnValue.Success = true;
                    returnValue.Result = "Aynı para birimi.";
                }
            }

            return returnValue;
        }

        public static MsCrmResult CreateOrUpdateExchangeRate(ExchangeRate eRate, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            Entity ent = new Entity("new_exchangerate");

            ent["new_name"] = eRate.Currency.Name + "-" + eRate.RateDate.ToString("dd.MM.yyyy");
            ent["new_currencydate"] = eRate.RateDate;
            ent["new_currencyid"] = eRate.Currency;
            ent["new_salesrate"] = eRate.SaleRate;
            ent["new_buyingrate"] = eRate.BuyRate;

            if (eRate.Id != null && eRate.Id != Guid.Empty)
            {
                ent["new_exchangerateid"] = eRate.Id;

                returnValue.CrmId = eRate.Id;
                service.Update(ent);
            }
            else
            {
                returnValue.CrmId = service.Create(ent);
            }


            return returnValue;
        }

        public static MsCrmResultObject GetCurrencies(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
									*
								INTO
									#TempTable
								FROM
								(
									SELECT
												o.BaseCurrencyId AS Id
												,o.BaseCurrencyIdName Name
												,50 SortNo
											FROM
												Organization AS o

											UNION

											SELECT
											t.TransactionCurrencyId AS Id
											,CurrencyName AS Name
											,1 SortNo
											FROM
											TransactionCurrency AS t
											WHERE
											t.StateCode=0
								)A	
								
								
								SELECT
								B.*
								FROM
								(
									SELECT
										T.*
										,DENSE_RANK() OVER (PARTITION BY T.Id ORDER BY T.SortNo DESC) AS RankPoint
									FROM
										#TempTable T
								)B
								WHERE
									B.RankPoint = 1
								ORDER BY
									B.SortNo DESC

								DROP TABLE #TempTable
										";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET CURRENCIES |
                    List<TransactionCurrency> returnList = new List<TransactionCurrency>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        TransactionCurrency _currency = new TransactionCurrency();
                        _currency.TransactionCurrencyId = (Guid)dt.Rows[i]["Id"];
                        _currency.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_currency);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin para birimi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult UpdateProductPriceLevelPricesByExchange(Guid productCurrencyId, Guid priceLevelCurrencyId, decimal exRate, bool isMultiply, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {

                #region | SQL QUERY|

                string sqlQuery = @"--DECLARE @rate DECIMAL
                                    --DECLARE @isMultiply BIT

                                    --SET @rate=1.5
                                    --SET @isMultiply=1

                                    SELECT
	                                    *
                                    INTO
	                                    #temp
                                    FROM
                                    (
	                                    SELECT
		                                    ppl.ProductPriceLevelId
		                                    ,ppl.Amount
                                            ,p.Price
	                                    FROM
	                                    ProductPriceLevel AS ppl (NOLOCK)
		                                    JOIN
			                                    Product AS p (NOLOCK)
				                                    ON
				                                    ppl.ProductId=p.ProductId
		                                    JOIN
			                                    PriceLevel AS pl (NOLOCK)
				                                    ON
				                                    ppl.PriceLevelId=pl.PriceLevelId
	                                    WHERE
		                                    p.TransactionCurrencyId=@productCurrencyId
	                                    AND
		                                    ppl.TransactionCurrencyId=@priceLevelCurrencyId
                                    ) AS A


                                    UPDATE
	                                    ppl
                                    SET
	                                    ppl.Amount=CASE 
					                                    WHEN
						                                    @isMultiply=1
					                                    THEN
					                                    t.Price*@rate					
					                                    ELSE
					                                    t.Price/@rate
			                                       END
                                    FROM
                                    ProductPriceLevel AS ppl
	                                    JOIN
		                                    #temp AS t (NOLOCK)
			                                    ON
			                                    ppl.ProductPriceLevelId=t.ProductPriceLevelId


                                    DROP TABLE #temp";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@productCurrencyId", productCurrencyId), new SqlParameter("@priceLevelCurrencyId", priceLevelCurrencyId), new SqlParameter("@rate", exRate), new SqlParameter("@isMultiply", isMultiply) };

                sda.ExecuteNonQuery(sqlQuery, parameters);

                returnValue.Success = true;
                returnValue.Result = "Fiyat listesi kalemleri fiyatları güncellendi.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static TransactionCurrency GetCurrencyByName(string currencyName, SqlDataAccess sda)
        {
            TransactionCurrency returnValue = new TransactionCurrency();

            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TC.TransactionCurrencyId
	                                ,TC.CurrencyName
                                FROM
	                                TransactionCurrency TC
                                WHERE
	                                TC.CurrencyName = '{0}'
                                ORDER BY
	                                TC.CurrencyName ASC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, currencyName));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET CURRENCY |

                    returnValue.TransactionCurrencyId = (Guid)dt.Rows[0]["TransactionCurrencyId"];
                    returnValue.Name = dt.Rows[0]["CurrencyName"] != DBNull.Value ? dt.Rows[0]["CurrencyName"].ToString() : string.Empty;

                    #endregion
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static TransactionCurrency GetBaseCurrency(string currencyName, SqlDataAccess sda)
        {
            TransactionCurrency returnValue = new TransactionCurrency();

            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
		                            o.BaseCurrencyId AS Id
		                            ,o.BaseCurrencyIdName Name
	                            FROM
		                            Organization AS o";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET CURRENCY |

                    returnValue.TransactionCurrencyId = (Guid)dt.Rows[0]["Id"];
                    returnValue.Name = dt.Rows[0]["CurrencyName"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;

                    #endregion
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }
    }
}
