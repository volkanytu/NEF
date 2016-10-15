using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public class ContractCoverEngHelper
    {
        IOrganizationService service;
        public static string projectNameGlobal { get; set; }
        public string ExecuteContractCover(Guid QuoteId, string Path)
        {
            DateTime deliveryDate;
            string typeOfHome = string.Empty;
            string generalTypeOfHome = string.Empty;

            string deliveryDateString = string.Empty;
            string SalesAccountName = string.Empty;
            string SalesAccountAddress = string.Empty;
            string SalesAccountEmail = string.Empty;
            string SalesAccountMersisno = string.Empty;
            string SalesAccountTel = string.Empty;
            string secondCustomerFirstName = string.Empty;
            string secondCustomerLastName = string.Empty;
            string secondCustomerTc = string.Empty;
            string secondCustomerNumber = string.Empty;
            string projectName = string.Empty;
            string blok = string.Empty;
            string floor = string.Empty;
            string apartmentNo = string.Empty;
            string apartmentCity = string.Empty;
            decimal m2 = 0;
            decimal grossm2 = 0;
            string currencySymbol = string.Empty;
            Guid projectId = Guid.Empty;
            string city = string.Empty;
            string adaPaftaParsel = string.Empty;
            string unitType = string.Empty;
            string apartmentType = string.Empty;
            string location = string.Empty;
            string freeSectionIdNumber = string.Empty;
            string address = string.Empty;
            string passportNumber = string.Empty;
            string foreignAddress = string.Empty;
            string Nationality = string.Empty;
            string CustomerNumber = string.Empty;
            string bbnetalan = string.Empty;
            string bbbrutalan = string.Empty;
            string satisesasalan = string.Empty;
            string bahce = string.Empty;
            string teras = string.Empty;
            string balkon = string.Empty;
            string satisesasalanm2 = string.Empty;
            string bbgenelbrutalan = string.Empty;
            string totalPaymentString = string.Empty;
            Entity contact = null;
            Entity account = null;
            Entity SalesAccount = null;
            service = MSCRM.AdminOrgService;
            Entity quote = service.Retrieve("quote", QuoteId, new ColumnSet(true));

            Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
            if (currencyDetail != null && currencyDetail.Attributes.Contains("currencysymbol"))
                currencySymbol = currencyDetail["currencysymbol"].ToString();

            string folder = ContractCoverEngHelper.CreateFolder(QuoteId, Path);


            if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "contact")
            {
                if (quote.Contains("new_secondcontactid"))
                {
                    Entity secondContact = service.Retrieve("contact", ((EntityReference)quote.Attributes["new_secondcontactid"]).Id, new ColumnSet(true));
                    secondCustomerFirstName = secondContact.Contains("firstname") ? (string)secondContact.Attributes["firstname"] : string.Empty;
                    secondCustomerLastName = secondContact.Contains("lastname") ? (string)secondContact.Attributes["lastname"] : string.Empty;
                    secondCustomerTc = secondContact.Contains("new_tcidentitynumber") ? (string)secondContact.Attributes["new_tcidentitynumber"] : string.Empty;
                    if (secondContact.Contains("new_passportnumber"))
                    {
                        secondCustomerTc = secondCustomerTc + " / " + (string)secondContact.Attributes["new_passportnumber"];
                    }
                    secondCustomerNumber = secondContact.Contains("new_number") ? (string)secondContact.Attributes["new_number"] : string.Empty;
                }


                contact = service.Retrieve("contact", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                city = contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                city += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                city += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;
                address = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() : string.Empty;
                address = address + " " + city;
                passportNumber = contact.Contains("new_passportnumber") ? (string)contact.Attributes["new_passportnumber"] : string.Empty;
                if (contact.Contains("new_address3countryid"))
                {
                    foreignAddress = contact.Contains("new_nontcidentityaddress") ? contact.Attributes["new_nontcidentityaddress"].ToString() : string.Empty;
                    foreignAddress += " " + ((EntityReference)contact.Attributes["new_address3cityid"]).Name + "/" + ((EntityReference)contact.Attributes["new_address3countryid"]).Name;
                }
                if (contact.Contains("new_nationalityid"))
                {
                    Nationality = ((EntityReference)contact.Attributes["new_nationalityid"]).Name;
                }
                CustomerNumber = contact.Contains("new_number") ? contact.Attributes["new_number"].ToString() : string.Empty;
                CustomerNumber += secondCustomerNumber != string.Empty ? " - " + secondCustomerNumber : string.Empty;

            }
            else if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "account")
            {
                account = service.Retrieve("account", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                city = account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                city += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                city += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;
                address = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() : string.Empty;
                address = address + " " + city;
            }




            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet("productid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                Entity product = service.Retrieve("product", ((EntityReference)Result.Entities[0].Attributes["productid"]).Id, new ColumnSet(true));
                projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
                projectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
                projectNameGlobal = projectName;
                blok = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
                floor = product.Contains("new_floornumber") ? product.Attributes["new_floornumber"].ToString() : string.Empty;
                apartmentNo = product.Contains("new_homenumber") ? (string)product.Attributes["new_homenumber"] : string.Empty;
                m2 = product.Contains("new_netm2") ? (decimal)product.Attributes["new_netm2"] : 0;
                grossm2 = product.Contains("new_grossm2") ? (decimal)product.Attributes["new_grossm2"] : 0;
                adaPaftaParsel = product.Contains("new_blockofbuildingid") ? ((EntityReference)product.Attributes["new_blockofbuildingid"]).Name + "/" : string.Empty + "/";
                adaPaftaParsel += product.Contains("new_threaderid") ? ((EntityReference)product.Attributes["new_threaderid"]).Name + "/" : string.Empty + "/";
                adaPaftaParsel += product.Contains("new_parcelid") ? ((EntityReference)product.Attributes["new_parcelid"]).Name : string.Empty;
                apartmentCity = product.Contains("new_city") ? (string)product.Attributes["new_city"] + "/" : string.Empty + "/";
                apartmentCity += product.Contains("new_district") ? (string)product.Attributes["new_district"] + "/" : string.Empty + "/";
                apartmentCity += product.Contains("new_quarter") ? (string)product.Attributes["new_quarter"] : string.Empty;
                unitType = product.Contains("new_unittypeid") ? ((EntityReference)product.Attributes["new_unittypeid"]).Name : string.Empty;
                apartmentType = product.Contains("new_generaltypeofhomeid") ? ((EntityReference)product.Attributes["new_generaltypeofhomeid"]).Name : string.Empty;
                location = product.Contains("new_locationid") ? ((EntityReference)product.Attributes["new_locationid"]).Name : string.Empty;
                freeSectionIdNumber = product.Contains("new_freesectionidnumber") ? (string)product.Attributes["new_freesectionidnumber"] : string.Empty;
                bbnetalan = product.Contains("new_bbnetarea") ? ((decimal)product.Attributes["new_bbnetarea"]).ToString("N2") : string.Empty;
                bbbrutalan = product.Contains("new_netm2") ? ((decimal)product.Attributes["new_netm2"]).ToString("N2") : string.Empty;
                satisesasalan = product.Contains("new_satisaesasalan") ? ((decimal)product.Attributes["new_satisaesasalan"]).ToString("N2") : string.Empty;
                bahce = product.Contains("new_garden") ? ((decimal)product.Attributes["new_garden"]).ToString("N2") : " - ";
                teras = product.Contains("new_terracegross") ? ((decimal)product.Attributes["new_terracegross"]).ToString("N2") : " - ";
                balkon = product.Contains("new_balconym2") ? ((decimal)product.Attributes["new_balconym2"]).ToString("N2") : " - ";
                satisesasalanm2 = product.Contains("new_grossm2") ? ((decimal)product.Attributes["new_grossm2"]).ToString("N2") : " - ";
                bbgenelbrutalan = product.Contains("new_bbgeneralgrossarea") ? ((decimal)product.Attributes["new_bbgeneralgrossarea"]).ToString("N2") : string.Empty;
                deliveryDateString = product.Contains("new_deliverydate") ? ((DateTime)product.Attributes["new_deliverydate"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                typeOfHome = product.Contains("new_typeofhomeid") ? ((EntityReference)product.Attributes["new_typeofhomeid"]).Name : string.Empty;
                generalTypeOfHome = product.Contains("new_generaltypeofhomeid") ? ((EntityReference)product.Attributes["new_generaltypeofhomeid"]).Name : string.Empty;

                if (product.Contains("new_deliverydate"))
                {
                    deliveryDate = product.GetAttributeValue<DateTime>("new_deliverydate");
                    decimal totalPayment = GetTotalPaymentOnOrBeforeProjectDate(QuoteId, deliveryDate);
                    totalPaymentString = totalPayment.ToString("N2");
                }
            }
            Entity project = service.Retrieve("new_project", projectId, new ColumnSet(true));





            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(bbnetalan))
                dictionary1.Add("bbnetalan", bbnetalan);
            else
                dictionary1.Add("bbnetalan", string.Empty);

            if (!string.IsNullOrEmpty(bbbrutalan))
                dictionary1.Add("bbbrutalan", bbbrutalan);
            else
                dictionary1.Add("bbbrutalan", string.Empty);

            if (!string.IsNullOrEmpty(satisesasalan))
                dictionary1.Add("satisesasalan", satisesasalan);
            else
                dictionary1.Add("satisesasalan", string.Empty);

            if (!string.IsNullOrEmpty(bahce))
                dictionary1.Add("bahce", bahce);
            else
                dictionary1.Add("bahce", string.Empty);

            if (!string.IsNullOrEmpty(teras))
                dictionary1.Add("teras", teras);
            else
                dictionary1.Add("teras", string.Empty);

            if (!string.IsNullOrEmpty(balkon))
                dictionary1.Add("balkon", balkon);
            else
                dictionary1.Add("balkon", string.Empty);

            if (!string.IsNullOrEmpty(satisesasalanm2))
                dictionary1.Add("satisesasalanm2", satisesasalanm2);
            else
                dictionary1.Add("satisesasalanm2", string.Empty);

            if (!string.IsNullOrEmpty(bbgenelbrutalan))
                dictionary1.Add("bbgenelbrutalan", bbgenelbrutalan);
            else
                dictionary1.Add("bbgenelbrutalan", string.Empty);


            if (quote.Contains("new_salesshareaccountid"))//Satışı Yapan Firma
            {
                SalesAccount = service.Retrieve("new_share", ((EntityReference)quote.Attributes["new_salesshareaccountid"]).Id, new ColumnSet(true));
                SalesAccountName = SalesAccount.Contains("new_name") ? SalesAccount.Attributes["new_name"].ToString() : string.Empty;
                SalesAccountAddress = SalesAccount.Contains("new_adressdetail") ? SalesAccount.Attributes["new_adressdetail"].ToString() : string.Empty;
                SalesAccountEmail = SalesAccount.Contains("new_emailaddress") ? SalesAccount.Attributes["new_emailaddress"].ToString() : string.Empty;
                SalesAccountMersisno = SalesAccount.Contains("new_mersisnumber") ? SalesAccount.Attributes["new_mersisnumber"].ToString() : string.Empty;
                SalesAccountTel = SalesAccount.Contains("new_phonenumber") ? SalesAccount.Attributes["new_phonenumber"].ToString() : string.Empty;
            }
            if (SalesAccount != null)
            {
                if (SalesAccountName != string.Empty)
                    dictionary1.Add("accountname", SalesAccountName);
                else
                    dictionary1.Add("accountname", string.Empty);
                if (SalesAccountAddress != string.Empty)
                    dictionary1.Add("accountaddress", SalesAccountAddress);
                else
                    dictionary1.Add("accountaddress", string.Empty);
                if (SalesAccountTel != string.Empty)
                    dictionary1.Add("accounttelephone", SalesAccountTel);
                else
                    dictionary1.Add("accounttelephone", string.Empty);
                if (SalesAccountEmail != string.Empty)
                    dictionary1.Add("accountemail", SalesAccountEmail);
                else
                    dictionary1.Add("accountemail", string.Empty);
                if (SalesAccountMersisno != string.Empty)
                    dictionary1.Add("accountmersisno", SalesAccountMersisno);
                else
                    dictionary1.Add("accountmersisno", string.Empty);
            }

            dictionary1.Add("teslimtarihi", deliveryDateString);
            dictionary1.Add("fiyatlabel", "Satış Fiyatı");
            dictionary1.Add("kdvlabel", "KDV Tutarı");
            dictionary1.Add("parabirimi", currencySymbol);
            dictionary1.Add("projeadi", projectName);
            dictionary1.Add("konutkimlik", projectName);
            dictionary1.Add("blok", blok);
            dictionary1.Add("kat", floor);
            dictionary1.Add("bulundugukat", floor);
            dictionary1.Add("no", apartmentNo);
            dictionary1.Add("daireno", apartmentNo);
            dictionary1.Add("m", grossm2.ToString("N2"));
            dictionary1.Add("satisdanismani", ((EntityReference)quote.Attributes["ownerid"]).Name.ToString());
            dictionary1.Add("musterino", CustomerNumber);
            dictionary1.Add("il", apartmentCity);
            dictionary1.Add("adapaftaparsel", adaPaftaParsel);
            if (unitType.Equals("Konut"))
            {
                dictionary1.Add("unitetipi", "Dwelling");
            }
            else if (unitType.Equals("Mağaza"))
            {
                dictionary1.Add("unitetipi", "Warehouse");
            }
            else
            {
                dictionary1.Add("oda", unitType);
            }

            if (apartmentType.Equals("Mağaza"))
            {
                dictionary1.Add("oda", "Warehouse");
            }
            else
            {
                dictionary1.Add("oda", apartmentType);
            }
            dictionary1.Add("konum", location);
            dictionary1.Add("bolumno", freeSectionIdNumber);
            dictionary1.Add("net", m2.ToString("N2"));
            dictionary1.Add("brut", grossm2.ToString("N2"));
            dictionary1.Add("onodemetutaritoplami", totalPaymentString);
            if (foreignAddress != string.Empty)
            {
                dictionary1.Add("evadresi", foreignAddress);
                dictionary1.Add("adres", foreignAddress);
                dictionary1.Add("isadresi", string.Empty);
            }
            else
            {
                dictionary1.Add("evadresi", address);
                dictionary1.Add("isadresi", string.Empty);
                dictionary1.Add("adres", address);
            }

            if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(Nationality) && Nationality != "TC")
            {
                dictionary1.Add("tcadresi", address);
                dictionary1.Add("tebligatadresi2", string.Empty);
            }
            else
            {
                dictionary1.Add("tcadresi", string.Empty);
                dictionary1.Add("tebligatadresi2", address);
            }







            if (quote.Contains("new_contractnumber"))
            {
                dictionary1.Add("sozlesmeno", quote.Attributes["new_contractnumber"].ToString());
            }
            else
            {
                dictionary1.Add("sozlesmeno", string.Empty);
            }
            if (contact != null)
            {
                dictionary1.Add("alicino", "T.C. Kimlik / Pasaport No:");
                if (contact.Contains("firstname"))
                {
                    if (secondCustomerFirstName != string.Empty)
                    {
                        dictionary1.Add("ad", contact.Attributes["firstname"].ToString() + " - " + secondCustomerFirstName);
                    }
                    else
                    {
                        dictionary1.Add("ad", contact.Attributes["firstname"].ToString());
                    }

                }
                else
                {
                    dictionary1.Add("ad", string.Empty);
                }
                if (contact.Contains("lastname"))
                {
                    if (secondCustomerLastName != string.Empty)
                    {
                        dictionary1.Add("soyad", contact.Attributes["lastname"].ToString() + " - " + secondCustomerLastName);
                    }
                    else
                    {
                        dictionary1.Add("soyad", contact.Attributes["lastname"].ToString());
                    }

                }
                else
                {
                    dictionary1.Add("soyad", string.Empty);
                }
                if (contact.Contains("new_tcidentitynumber"))
                {
                    if (secondCustomerTc != string.Empty)
                    {
                        if (passportNumber != string.Empty)
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString() + "/" + passportNumber + " - " + secondCustomerTc);
                        }
                        else
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString() + " - " + secondCustomerTc);
                        }
                    }
                    else
                    {
                        if (passportNumber != string.Empty)
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString() + "/" + passportNumber);
                        }
                        else
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString());
                        }
                    }

                }
                else
                {
                    dictionary1.Add("tckimlik", passportNumber);
                }

                if (contact.Contains("fullname"))
                {
                    if (secondCustomerFirstName != string.Empty)
                    {
                        dictionary1.Add("adsoyad", contact.Attributes["fullname"].ToString() + "- " + secondCustomerFirstName + " " + secondCustomerLastName);
                        dictionary1.Add("cari", contact.Attributes["fullname"].ToString() + "- " + secondCustomerFirstName + " " + secondCustomerLastName);
                    }
                    else
                    {
                        dictionary1.Add("adsoyad", contact.Attributes["fullname"].ToString());
                        dictionary1.Add("cari", contact.Attributes["fullname"].ToString());
                    }

                }
                else
                {
                    dictionary1.Add("adsoyad", string.Empty);
                    dictionary1.Add("cari", string.Empty);

                }
                if (contact.Contains("mobilephone"))
                {
                    dictionary1.Add("ceptelefonu", contact.Attributes["mobilephone"].ToString());
                }
                else
                {
                    dictionary1.Add("ceptelefonu", string.Empty);
                }
                if (contact.Contains("emailaddress1"))
                {
                    dictionary1.Add("epostaadresi3", contact.Attributes["emailaddress1"].ToString());
                }
                else
                {
                    dictionary1.Add("epostaadresi3", string.Empty);
                }
            }
            else if (account != null)
            {
                dictionary1.Add("alicino", "Vergi Dairesi / Vergi No:");
                dictionary1.Add("soyad", string.Empty);
                string taxOfficeNumber = string.Empty;
                if (account.Contains("telephone1"))
                {
                    dictionary1.Add("ceptelefonu", account.Attributes["telephone1"].ToString());
                }
                else
                {
                    dictionary1.Add("ceptelefonu", string.Empty);
                }
                if (account.Contains("emailaddress1"))
                {
                    dictionary1.Add("epostaadresi3", account.Attributes["emailaddress1"].ToString());
                }
                else
                {
                    dictionary1.Add("epostaadresi3", string.Empty);
                }
                if (account.Contains("name"))
                {
                    dictionary1.Add("ad", account.Attributes["name"].ToString());
                    dictionary1.Add("cari", account.Attributes["name"].ToString());
                    dictionary1.Add("adsoyad", account.Attributes["name"].ToString());
                }
                else
                {
                    dictionary1.Add("ad", string.Empty);
                    dictionary1.Add("cari", string.Empty);
                    dictionary1.Add("adsoyad", string.Empty);
                }

                if (account.Contains("new_taxofficeid"))
                {
                    taxOfficeNumber = ((EntityReference)account.Attributes["new_taxofficeid"]).Name.ToString() + "/";
                }
                if (account.Contains("new_taxnumber"))
                {
                    taxOfficeNumber += account.Attributes["new_taxnumber"].ToString();
                }
                dictionary1.Add("tckimlik", taxOfficeNumber);
            }

            if (quote.Contains("new_contractdate"))
            {
                dictionary1.Add("sozlemetarihi", ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
                dictionary1.Add("tarih", ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToString("dd/MM/yyyy"));

            }
            else
            {
                dictionary1.Add("sozlemetarihi", string.Empty);
            }
            if (quote.Contains("totallineitemamount"))
            {
                dictionary1.Add("dairefiyat", ((Money)quote.Attributes["totallineitemamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("dairefiyat", string.Empty);
            }
            if (quote.Contains("totalamountlessfreight"))
            {
                dictionary1.Add("satisfiyati2", ((Money)quote.Attributes["totalamountlessfreight"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("satisfiyati2", string.Empty);
            }
            if (grossm2 > 0)
            {
                dictionary1.Add("satisfiyat", (((Money)quote.Attributes["totalamountlessfreight"]).Value / grossm2).ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("satisfiyat", string.Empty);
            }
            if (quote.Contains("discountpercentage"))
            {
                decimal totalAmount = ((Money)quote.Attributes["totallineitemamount"]).Value;
                dictionary1.Add("oran", ((decimal)quote.Attributes["discountpercentage"]).ToString("N2"));
                dictionary1.Add("miktar", ((totalAmount * (decimal)quote.Attributes["discountpercentage"]) / 100).ToString("N2") + " " + currencySymbol);
            }
            else if (quote.Contains("discountamount"))
            {
                decimal totalAmount = ((Money)quote.Attributes["totallineitemamount"]).Value;
                dictionary1.Add("miktar", ((Money)quote.Attributes["discountamount"]).Value.ToString("N2") + " " + currencySymbol);
                dictionary1.Add("oran", ((((Money)quote.Attributes["discountamount"]).Value / totalAmount) * 100).ToString("N2"));
            }
            else
            {
                dictionary1.Add("oran", string.Empty);
                dictionary1.Add("miktar", string.Empty);
            }

            if (quote.Contains("new_paymentplan") && (bool)quote.Attributes["new_paymentplan"])
            {
                dictionary1.Add("satissekli", "Scheme of Payment without Loan");
            }
            else
            {
                dictionary1.Add("satissekli", "Scheme of Payment with Loan");
            }
            if (quote.Contains("new_taxrate") && quote.Contains("new_containstax"))
            {
                if (!(bool)quote["new_containstax"])
                {
                    dictionary1.Add("kdvyuzde", ((decimal)quote.Attributes["new_taxrate"]).ToString("N2"));
                }
                else
                {
                    dictionary1.Add("kdvyuzde", string.Empty);
                }
            }
            else
            {
                dictionary1.Add("kdvyuzde", string.Empty);
            }
            if (quote.Contains("new_taxamount"))
            {
                dictionary1.Add("kdvtutari", ((Money)quote.Attributes["new_taxamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("kdvtutari", string.Empty);
            }

            //Topkapı projesinde çıktı ne olursa olsun yazması gerekiyor. Bu nedenle kontrolden çıkarıldı.
            if (projectName == "827 Inistanbul Topkapı")
            {
                if (quote.Contains("new_taxofstamp"))
                {
                    dictionary1.Add("damgavergisiyuzde", ((decimal)quote.Attributes["new_taxofstamp"]).ToString("N2"));
                }
                else
                {
                    dictionary1.Add("damgavergisiyuzde", string.Empty);
                }
                if (quote.Contains("new_taxofstamp"))
                {
                    decimal taxRate = ((decimal)quote.Attributes["new_taxofstamp"]);
                    decimal totalAmount = ((Money)quote.Attributes["totalamount"]).Value;

                    dictionary1.Add("damgavergisi", ((totalAmount * taxRate) / 100).ToString("N2") + " " + currencySymbol);
                }
                else
                {
                    dictionary1.Add("damgavergisi", "0.00" + " " + currencySymbol);
                }
            }
            else
            {
                if (quote.Contains("new_taxofstamp") && (!quote.Contains("new_isnotarizedsales") || !(bool)quote.Attributes["new_isnotarizedsales"]))
                {
                    dictionary1.Add("damgavergisiyuzde", ((decimal)quote.Attributes["new_taxofstamp"]).ToString("N2"));
                }
                else
                {
                    dictionary1.Add("damgavergisiyuzde", string.Empty);
                }
                if (quote.Contains("new_taxofstamp") && (!quote.Contains("new_isnotarizedsales") || !(bool)quote.Attributes["new_isnotarizedsales"]))
                {
                    decimal taxRate = ((decimal)quote.Attributes["new_taxofstamp"]);
                    decimal totalAmount = ((Money)quote.Attributes["totalamount"]).Value;

                    dictionary1.Add("damgavergisi", ((totalAmount * taxRate) / 100).ToString("N2") + " " + currencySymbol);
                }
                else
                {
                    dictionary1.Add("damgavergisi", "0.00" + " " + currencySymbol);
                }
            }

            decimal _taxAmount = quote.Contains("new_taxamount") ? ((Money)quote.Attributes["new_taxamount"]).Value : 0;
            decimal _totalAmount = quote.Contains("totalamount") ? ((Money)quote.Attributes["totalamount"]).Value : 0;
            decimal _taxStampRate = (!quote.Contains("new_isnotarizedsales") || !(bool)quote.Attributes["new_isnotarizedsales"]) ? quote.Contains("new_taxofstamp") ? ((decimal)quote.Attributes["new_taxofstamp"]) : 0 : 0;
            decimal _taxStampAmount = ((_totalAmount * _taxStampRate) / 100);

            Entity kusuratEtiketi = this.GetKusuratEtiketi(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "new_name" });
            string yaziylaTutar = YaziyaCevir((_totalAmount + _taxAmount + _taxStampAmount), currencyDetail["currencysymbol"].ToString(), kusuratEtiketi["new_name"].ToString());


            dictionary1.Add("toplamtutar", (_totalAmount + _taxAmount + _taxStampAmount).ToString("N2") + " " + currencySymbol + " (" + yaziylaTutar + ") ");


            if (project.Contains("new_annualratio"))
            {
                dictionary1.Add("yillikfaizoran", "%" + ((decimal)project.Attributes["new_annualratio"]).ToString("N2"));
            }
            else
            {
                dictionary1.Add("yillikfaizoran", "%0");
            }
            if (project.Contains("new_buildingpermitsdate"))
            {
                dictionary1.Add("yapiruhsattarihi", ((DateTime)project.Attributes["new_buildingpermitsdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
            }
            else
            {
                dictionary1.Add("yapiruhsattarihi", string.Empty);
            }
            if (project.Contains("new_guaranteeinfo"))
            {
                dictionary1.Add("teminatbilgisi", project.Attributes["new_guaranteeinfo"].ToString());
            }
            else
            {
                dictionary1.Add("teminatbilgisi", string.Empty);
            }
            decimal totalBankCredit = GetPaymentTotalByType(QuoteId, 9);//Banka Kredisi
            dictionary1.Add("krediodemetutari", totalBankCredit.ToString("N2") + " " + currencySymbol);

            DataTable dtPaymentPlan = new DataTable();
            dtPaymentPlan.TableName = "snt";
            dtPaymentPlan.Columns.Add("tarih", System.Type.GetType("System.DateTime"));
            dtPaymentPlan.Columns.Add("text");
            dtPaymentPlan.Columns.Add("tip");
            dtPaymentPlan.Columns.Add("tutar");

            // EntityCollection Kapora = GetCollectionPaymentTotalByType(QuoteId, 4);//Kapora Ödemesi
            decimal _totalKapora = 0;
            if (quote.Contains("new_prepaymentdate") && quote.Contains("new_prepaymentamount"))
            {
                for (int i = 0; i < 1; i++)
                {
                    DataRow row = dtPaymentPlan.NewRow();
                    row["tarih"] = quote.Contains("new_prepaymentdate") ? Convert.ToDateTime(((DateTime)quote.Attributes["new_prepaymentdate"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                    row["text"] = "Date Of";
                    row["tip"] = "Pre Payment Amount";
                    row["tutar"] = quote.Contains("new_prepaymentamount") ? ((Money)quote.Attributes["new_prepaymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                    _totalKapora = quote.Contains("new_prepaymentamount") ? ((Money)quote.Attributes["new_prepaymentamount"]).Value : 0;
                    dtPaymentPlan.Rows.Add(row);
                }
            }


            EntityCollection KDV = GetCollectionPaymentTotalByType(QuoteId, 6);//KDV
            decimal _totalKDV = 0;
            foreach (Entity p in KDV.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Date Of";
                row["tip"] = "Payable VAT Amount";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalKDV += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection DamgaVergisi = GetCollectionPaymentTotalByType(QuoteId, 7);//Damga Vergisi
            decimal _totalDamgaVergisi = 0;
            foreach (Entity p in DamgaVergisi.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Date Of";
                row["tip"] = "Payable Stamp Tax Amount";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalDamgaVergisi += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection PesinOdeme = GetCollectionPaymentTotalByType(QuoteId, 3);//Peşin Ödeme
            decimal _totalPesinOdeme = 0;
            foreach (Entity p in PesinOdeme.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Date Of";
                row["tip"] = "Cash Payment Amount";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalPesinOdeme += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection BankaKredisi = GetCollectionPaymentTotalByType(QuoteId, 9);//Banka Kredisi
            decimal _totalBankaKredisi = 0;
            foreach (Entity p in BankaKredisi.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Date Of";
                row["tip"] = "Payable Bank Credit";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalBankaKredisi += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection AraOdeme = GetCollectionPaymentTotalByType(QuoteId, 1);//Ara Ödeme
            decimal _totalAraOdeme = 0;
            string _dateAraOdeme = string.Empty;
            foreach (Entity p in AraOdeme.Entities)
            {
                _totalAraOdeme += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                //if (AraOdeme.Entities.IndexOf(p) == 0)
                //{
                //    _dateAraOdeme = p.Contains("new_date") ? ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                //}
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Date Of";
                row["tip"] = "Payable Interim Payments";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                dtPaymentPlan.Rows.Add(row);
            }
            //if (_totalAraOdeme > 0)
            //{
            //    DataRow row = dtPaymentPlan.NewRow();
            //    row["tarih"] = _dateAraOdeme;
            //    row["text"] = "Tarihinde";
            //    row["tip"] = "Ödenecek Ara Ödeme Tutarı";
            //    row["tutar"] = _totalAraOdeme.ToString("N2") + " " + currencySymbol;
            //    dtPaymentPlan.Rows.Add(row);
            //}

            EntityCollection DuzenliTaksit = GetCollectionPaymentTotalByType(QuoteId, 2);//Düzenli Taksit
            decimal _totalDuzenliTaksit = 0;
            string _dateDuzenliTaksit = string.Empty;
            foreach (Entity p in DuzenliTaksit.Entities)
            {
                _totalDuzenliTaksit += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                //if (DuzenliTaksit.Entities.IndexOf(p) == 0)
                //{
                //    _dateDuzenliTaksit = p.Contains("new_date") ? ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                //}
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Date Of";
                row["tip"] = "Installments Payable Amount";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                dtPaymentPlan.Rows.Add(row);
            }
            //if (_totalDuzenliTaksit > 0)
            //{
            //    DataRow row = dtPaymentPlan.NewRow();
            //    row["tarih"] = _dateDuzenliTaksit;
            //    row["text"] = "Tarihinde";
            //    row["tip"] = "Ödenecek Düzenli Taksit Tutarı";
            //    row["tutar"] = _totalDuzenliTaksit.ToString("N2") + " " + currencySymbol;
            //    dtPaymentPlan.Rows.Add(row);
            //}

            dictionary1.Add("odemelertoplami", (_totalDuzenliTaksit + _totalAraOdeme + _totalBankaKredisi + _totalPesinOdeme + _totalDamgaVergisi + _totalKDV + _totalKapora).ToString("N2"));





            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dtPaymentPlan);
            dataSet.Tables[0].DefaultView.Sort = "tarih ASC";

            DataTable dt = dataSet.Tables[0].DefaultView.ToTable();
            dataSet.Tables[0].Rows.Clear();

            DataTable dtPaymentPlanSorted = new DataTable();
            dtPaymentPlanSorted.TableName = "snt";
            dtPaymentPlanSorted.Columns.Add("tarih", System.Type.GetType("System.String"));
            dtPaymentPlanSorted.Columns.Add("text");
            dtPaymentPlanSorted.Columns.Add("tip");
            dtPaymentPlanSorted.Columns.Add("tutar");
            DataSet dataSetSorted = new DataSet();
            dataSetSorted.Tables.Add(dtPaymentPlanSorted);

            foreach (DataRow row in dt.Rows)
            {
                dataSetSorted.Tables[0].Rows.Add(row.ItemArray);
            }
            foreach (DataRow row in dtPaymentPlanSorted.Rows)
            {
                dataSetSorted.Tables[0].Rows[dtPaymentPlanSorted.Rows.IndexOf(row)][0] = Convert.ToDateTime(row.ItemArray[0]).ToString("dd/MM/yyyy");
            }


            if (SalesAccount != null && projectName != "827 Inistanbul Topkapı")
            {
                byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\SozlesmeKapagiIng2.docx", dataSetSorted, dictionary1);
                string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\SozlesmeKapagi2.docx";
                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
                return path1;

            }
            else if (SalesAccount == null && projectName != "827 Inistanbul Topkapı")
            {
                byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\SozlesmeKapagiIng.docx", dataSetSorted, dictionary1);
                string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\SozlesmeKapagiIng.docx";
                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
                return path1;

            }
            else if (projectName == "827 Inistanbul Topkapı")//TOPKAPI PROJESİ
            {
                if (typeOfHome.Equals("G") && generalTypeOfHome.Equals("2+1"))
                {
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\TopkapiSozlesmeKapagiIngG.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\TopkapiSozlesmeKapagiIngG.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else
                {
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\TopkapiSozlesmeKapagiIng.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\TopkapiSozlesmeKapagiIng.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
            }
            else
            {
                return string.Empty;
            }


        }
        private static string CreateFolder(Guid QuoteId, string Path)
        {
            string str1 = QuoteId.ToString();
            if (!Directory.Exists(Path + "\\DocumentMerge"))
                Directory.CreateDirectory(Path + "\\DocumentMerge");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document"))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document\\" + str1))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document\\" + str1);
            return str1;
        }
        private Entity GetCurrencyDetail(Guid id, string[] Columns)
        {
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = "transactioncurrencyid";
            conditionExpression.Operator = ConditionOperator.Equal;
            conditionExpression.Values.Add((object)id);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            columnSet.AddColumns(Columns);
            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)this.service.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = (QueryBase)new QueryExpression()
                {
                    ColumnSet = columnSet,
                    Criteria = filterExpression,
                    EntityName = "transactioncurrency"
                }
            });
            if (multipleResponse.EntityCollection.Entities != null && multipleResponse.EntityCollection.Entities.Count > 0)
                return Enumerable.First<Entity>((IEnumerable<Entity>)multipleResponse.EntityCollection.Entities);
            else
                return (Entity)null;
        }
        private decimal GetPaymentTotalByType(Guid QuoteId, int Type)
        {
            decimal bankCredit = 0;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);


            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_type";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(Type);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    bankCredit += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return bankCredit;
        }


        private decimal GetTotalPaymentOnOrBeforeProjectDate(Guid QuoteId, DateTime date)
        {
            decimal bankCredit = 0;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_date";
            con2.Operator = ConditionOperator.OnOrBefore;
            con2.Values.Add(date);

            //ConditionExpression con3 = new ConditionExpression();
            //con3.AttributeName = "new_type";
            //con3.Operator = ConditionOperator.NotEqual;
            //con3.Values.Add(6);

            //ConditionExpression con4 = new ConditionExpression();
            //con4.AttributeName = "new_type";
            //con4.Operator = ConditionOperator.NotEqual;
            //con4.Values.Add(7);


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            //filter.Conditions.Add(con3);
            //filter.Conditions.Add(con4);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    bankCredit += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return bankCredit;
        }


        private EntityCollection GetCollectionPaymentTotalByType(Guid QuoteId, int Type)
        {
            EntityCollection col = new EntityCollection();

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);


            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_type";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(Type);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount", "new_date");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            Query.AddOrder("new_date", OrderType.Descending);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    col.Entities.Add(p);
                }
            }
            return col;
        }

        private Entity GetKusuratEtiketi(Guid id, string[] Columns)
        {
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = "new_currencyid";
            conditionExpression.Operator = ConditionOperator.Equal;
            conditionExpression.Values.Add((object)id);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            columnSet.AddColumns(Columns);
            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)this.service.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = (QueryBase)new QueryExpression()
                {
                    ColumnSet = columnSet,
                    Criteria = filterExpression,
                    EntityName = "new_currencylabel"
                }
            });
            if (multipleResponse.EntityCollection.Entities != null && multipleResponse.EntityCollection.Entities.Count > 0)
                return Enumerable.First<Entity>((IEnumerable<Entity>)multipleResponse.EntityCollection.Entities);
            else
                return (Entity)null;
        }

        private string YaziyaCevir(Decimal para, string paraBirim, string kurusBirim)
        {
            TextToTranslateMoney.ParaYaziyaGosterimTipi yaziyaGosterimTipi = (TextToTranslateMoney.ParaYaziyaGosterimTipi)(int)(byte)((TextToTranslateMoney.ParaYaziyaGosterimTipi)(int)(byte)((TextToTranslateMoney.ParaYaziyaGosterimTipi)(int)(byte)((TextToTranslateMoney.ParaYaziyaGosterimTipi)(int)(byte)((TextToTranslateMoney.ParaYaziyaGosterimTipi)0 + 1) + 16) + 4) + 64);
            return TextToTranslateMoney.ParaYaziyaEng(para, yaziyaGosterimTipi, paraBirim, kurusBirim);
        }
    }
}
