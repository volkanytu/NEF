using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk;

namespace NEF.ConsoleApp.ExchangeRateProcess
{
    public static class ExchangeRateProcess
    {
        static SqlDataAccess sda = null;
        static IOrganizationService service = null;

        public static MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                service = MSCRM.GetOrgService(true);

                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                XElement CurrencyRate = XElement.Load("http://www.tcmb.gov.tr/kurlar/today.xml");
                string currentDate = DateTime.Now.ToShortDateString();
                XAttribute dateRate = (from p in CurrencyRate.Attributes()
                                       where p.Name.LocalName == "Tarih"
                                       select p).Single();

                if (string.Format("dd/MM/yyyy", dateRate.Value) == string.Format("dd/MM/yyyy", currentDate))
                {
                    #region | USD |
                    XElement dollar = (from p in CurrencyRate.Elements()
                                       where p.Attribute("CurrencyCode").Value == "USD"
                                       select p).Single();

                    ExchangeRate erDollar = new ExchangeRate();
                    erDollar.BuyRate = Convert.ToDecimal(dollar.Element("BanknoteBuying").Value.Replace('.', ','));
                    erDollar.SaleRate = Convert.ToDecimal(dollar.Element("BanknoteSelling").Value.Replace('.', ','));
                    erDollar.RateDate = DateTime.Now.Date;

                    TransactionCurrency currUsd = CurrencyHelper.GetCurrencyByName("USD", sda);

                    erDollar.Currency = new EntityReference()
                    {
                        Id = currUsd.TransactionCurrencyId,
                        Name = "US Dollar",
                        LogicalName = "transactioncurrency"
                    };

                    MsCrmResultObject resultDollar = CurrencyHelper.GetExchangeRateByCurrency(DateTime.Now, erDollar.Currency.Id, sda);

                    if (resultDollar.Success)
                    {
                        #region | TOMORROW |
                        erDollar.RateDate = erDollar.RateDate.AddDays(1);
                        CurrencyHelper.CreateOrUpdateExchangeRate(erDollar, service);

                        #endregion

                        #region | UPDATE TODAY |
                        ExchangeRate eRateDollar = (ExchangeRate)resultDollar.ReturnObject;
                        erDollar.Id = eRateDollar.Id;

                        erDollar.RateDate = erDollar.RateDate.AddDays(-1);
                        CurrencyHelper.CreateOrUpdateExchangeRate(erDollar, service);

                        #endregion

                    }
                    else
                    {
                        #region | CREATE TODAY |
                        CurrencyHelper.CreateOrUpdateExchangeRate(erDollar, service);

                        #endregion

                        #region | TOMORROW |
                        erDollar.RateDate = erDollar.RateDate.AddDays(1);
                        CurrencyHelper.CreateOrUpdateExchangeRate(erDollar, service);

                        #endregion
                    }

                    #endregion

                    #region | EUR |
                    XElement euro = (from p in CurrencyRate.Elements()
                                     where p.Attribute("CurrencyCode").Value == "EUR"
                                     select p).Single();

                    ExchangeRate erEuro = new ExchangeRate();
                    erEuro.BuyRate = Convert.ToDecimal(euro.Element("BanknoteBuying").Value.Replace('.', ','));
                    erEuro.SaleRate = Convert.ToDecimal(euro.Element("BanknoteSelling").Value.Replace('.', ','));
                    erEuro.RateDate = DateTime.Now.Date;

                    TransactionCurrency currEur = CurrencyHelper.GetCurrencyByName("Euro", sda);

                    erEuro.Currency = new EntityReference()
                    {
                        Id = currEur.TransactionCurrencyId,
                        Name = "Euro",
                        LogicalName = "transactioncurrency"
                    };

                    MsCrmResultObject resultEuro = CurrencyHelper.GetExchangeRateByCurrency(DateTime.Now, erEuro.Currency.Id, sda);

                    if (resultEuro.Success)
                    {
                        #region | TOMORROW |
                        erEuro.RateDate = erEuro.RateDate.AddDays(1);
                        CurrencyHelper.CreateOrUpdateExchangeRate(erEuro, service);

                        #endregion

                        #region | UPDATE TODAY |
                        ExchangeRate eRateEuro = (ExchangeRate)resultEuro.ReturnObject;
                        erEuro.Id = eRateEuro.Id;

                        erEuro.RateDate = erEuro.RateDate.AddDays(-1);
                        CurrencyHelper.CreateOrUpdateExchangeRate(erEuro, service);

                        #endregion

                    }
                    else
                    {
                        #region | CREATE TODAY |
                        CurrencyHelper.CreateOrUpdateExchangeRate(erEuro, service);

                        #endregion

                        #region | TOMORROW |
                        erEuro.RateDate = erEuro.RateDate.AddDays(1);
                        CurrencyHelper.CreateOrUpdateExchangeRate(erEuro, service);

                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }
    }
}
