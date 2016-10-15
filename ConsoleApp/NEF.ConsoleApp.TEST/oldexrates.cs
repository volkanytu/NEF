using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NEF.Library.Business;
using NEF.Library.Utility;
using System.Data;
using Microsoft.Xrm.Sdk.Query;
using System.Data.SqlClient;

namespace NEF.ConsoleApp.TEST
{
    public static class oldexrates
    {
        public static void Process()
        {
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            IOrganizationService service = MSCRM.GetOrgService(true);

            string sql = @"SELECT
                                d.new_name AS Name
                                ,d.new_doviz AS Currency
                                ,d.new_tarih AS ExDate
                                ,d.new_kur AS SalesRate
                                ,d.new_aliskuru AS BuyingRate
                            FROM
                            IDEFIX2.KonutSatis_MSCRM.dbo.new_dovizkuru AS d
                            WHERE
                            d.new_name IS NOT NULL
                            AND
                            d.new_doviz IS NOT NULL
                            AND
                            d.new_tarih IS NOT NULL
                            AND
                            d.new_kur IS NOT NULL
                            AND
                            d.new_aliskuru IS NOT NULL ORDER BY d.new_tarih DESC";

            DataTable dt = sda.getDataTable(sql);

            if (dt.Rows.Count > 0)
            {
                int error = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    try
                    {
                        Entity ent = new Entity("new_exchangerate");

                        ent["new_name"] = dt.Rows[i]["Name"].ToString();
                        ent["new_currencydate"] = ((DateTime)dt.Rows[i]["ExDate"]).ToLocalTime().Date;

                        if ((int)dt.Rows[i]["Currency"] == 1) //Euro
                            ent["new_currencyid"] = new EntityReference("transactioncurrency", Globals.CurrencyIdEUR);
                        else if ((int)dt.Rows[i]["Currency"] == 2) //Usd
                            ent["new_currencyid"] = new EntityReference("transactioncurrency", Globals.CurrencyIdUSD);

                        ent["new_salesrate"] = (decimal)dt.Rows[i]["SalesRate"];
                        ent["new_buyingrate"] = (decimal)dt.Rows[i]["BuyingRate"];

                        service.Create(ent);

                        Console.WriteLine((i + 1).ToString() + "/" + dt.Rows.Count.ToString());
                    }
                    catch (Exception ex)
                    {
                        error++;
                        Console.SetCursorPosition(0, 2);

                        Console.WriteLine("HATA:" + error.ToString());
                    }
                }
            }

            Console.WriteLine("Bitti!");
            Console.ReadKey();
        }

        public static string Muhasebelestir(string quoteId)
        {
            string result = "false";
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                Entity quote = service.Retrieve("quote", new Guid(quoteId), new ColumnSet("customerid", "new_financialaccountid", "transactioncurrencyid"));
                EntityReference customer = quote.Attributes.Contains("customerid") ? (EntityReference)quote["customerid"] : null;
                EntityReference financialAccount = quote.Attributes.Contains("new_financialaccountid") ? (EntityReference)quote["new_financialaccountid"] : null;
                EntityReference currency = quote.Attributes.Contains("transactioncurrencyid") ? (EntityReference)quote["transactioncurrencyid"] : null;
                string customerName = customer.Name;

                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                #region | UPDATE FINANCIAL ACCOUNT CODE -VOLKAN 08.03.2015 |

                if (!string.IsNullOrEmpty(customerName) && financialAccount != null && !financialAccount.Name.Contains("329") && currency != null)
                {
                    char character = customerName[0];
                    MsCrmResultObject fAccountResult = FinancialAccountHelper.GetFinancialAccountNumberByCharacter(character, sda);
                    if (fAccountResult.Success)
                    {
                        int fAccountCode = (int)fAccountResult.ReturnObject;
                        fAccountCode++;

                        string fAccountCodeLeft = fAccountCode.ToString().PadLeft(5, '0');

                        Entity fAccount = new Entity("new_financialaccount");
                        fAccount["new_financialaccountid"] = financialAccount.Id;
                        fAccount["new_financialaccountnumber"] = fAccountCode;
                        fAccount["new_financialaccountcharacter"] = character + "";
                        fAccount["new_name"] = "329CA20" + (currency.Name == "TL" ? "TRL" : (currency.Name == "Euro" ? "EUR" : currency.Name)) + character + fAccountCodeLeft;
                        service.Update(fAccount);
                    }
                    else
                    {
                        throw new Exception(fAccountResult.Result);
                    }
                }


                #endregion

                #region | SET GROUP CODE |
                if (customer.LogicalName == "contact")
                {
                    if (!string.IsNullOrEmpty(customerName) && !ContactHelper.CheckContactHasGroupCode(customer.Id, sda).Success)
                    {
                        char character = customerName[0];
                        MsCrmResultObject groupCodeResult = ContactHelper.GetContactGroupCodeByCharacter(character, sda);
                        if (groupCodeResult.Success)
                        {
                            int groupCode = (int)groupCodeResult.ReturnObject;
                            groupCode++;
                            string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                            Entity entContact = new Entity("contact");
                            entContact["contactid"] = customer.Id;
                            entContact["new_groupcodenumber"] = groupCode;
                            entContact["new_groupcodecharacter"] = character + "";
                            entContact["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                            service.Update(entContact);
                        }
                        else
                        {
                            throw new Exception(groupCodeResult.Result);
                        }
                    }
                }

                if (customer.LogicalName == "account")
                {
                    if (!string.IsNullOrEmpty(customerName) && !AccountHelper.CheckAccountHasGroupCode(customer.Id, sda).Success)
                    {
                        char character = customerName[0];
                        MsCrmResultObject groupCodeResult = AccountHelper.GetAccountGroupCodeByCharacter(character, sda);
                        if (groupCodeResult.Success)
                        {
                            int groupCode = (int)groupCodeResult.ReturnObject;
                            groupCode++;
                            string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                            Entity entAccount = new Entity("account");
                            entAccount["accountid"] = customer.Id;
                            entAccount["new_groupcodenumber"] = groupCode;
                            entAccount["new_groupcodecharacter"] = character + "";
                            entAccount["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                            service.Update(entAccount);
                        }
                        else
                        {
                            throw new Exception(groupCodeResult.Result);
                        }
                    }
                }
                #endregion

                #region | QUERY UPDATE STATUS|
                string sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   StatusCode=@StatusCode,
                                       ModifiedOn=GETUTCDATE()
                                    WHERE
	                                    QuoteId='{0}'";
                #endregion

                sda.ExecuteNonQuery(string.Format(sqlQuery, quoteId), new SqlParameter[] { new SqlParameter("StatusCode", (int)QuoteStatus.MuhasebeyeAktarıldı) });



                #region | QUERY UPDATE LOGO TRANSFER |
                sqlQuery = string.Empty;
                sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   new_islogotransferred=@Islogotransferred
                                    WHERE
	                                    QuoteId='{0}'";
                #endregion

                sda.ExecuteNonQuery(string.Format(sqlQuery, quoteId), new SqlParameter[] { new SqlParameter("Islogotransferred", 1) });
                result = "true";
            }
            catch (Exception)
            {

                result = "false";
            }
            return result;
        }
    }
}
