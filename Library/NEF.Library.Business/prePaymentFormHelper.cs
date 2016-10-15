using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public class prePaymentFormHelper
    {
        IOrganizationService service;
        internal string ExecutePrePaymentForm(Guid QuoteId, string Path)
        {
            string folder = prePaymentFormHelper.CreateFolder(QuoteId, Path);
            string projectName = string.Empty;
            string referans = string.Empty;
            string blok = string.Empty;
            string floor = string.Empty;
            string apartmentNo = string.Empty;
            decimal m2 = 0;
            decimal grossm2 = 0;
            string currencySymbol = string.Empty;
            Guid projectId = Guid.Empty;
            string city = string.Empty;
            string address = string.Empty;
            string passportNumber = string.Empty;
            string foreignAddress = string.Empty;
            string secondaryPersonLastName = string.Empty;
            string secondaryPersonName = string.Empty;
            string secondaryPersonPhone = string.Empty;
            string blocktype = string.Empty;

            Entity contact = null;
            Entity account = null;
            service = MSCRM.AdminOrgService;
            Entity quote = service.Retrieve("quote", QuoteId, new ColumnSet(true));
            Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
            if (currencyDetail != null && currencyDetail.Attributes.Contains("currencysymbol"))
                currencySymbol = currencyDetail["currencysymbol"].ToString();
            string secondCustomerName = string.Empty;
            string secondCustomerTc = string.Empty;
            if (quote.Contains("new_secondcontactid"))
            {
                Entity secondContact = service.Retrieve("contact", ((EntityReference)quote.Attributes["new_secondcontactid"]).Id, new ColumnSet(true));
                secondCustomerName = secondContact.Contains("fullname") ? (string)secondContact.Attributes["fullname"] : string.Empty;
                secondCustomerTc = secondContact.Contains("new_tcidentitynumber") ? (string)secondContact.Attributes["new_tcidentitynumber"] : string.Empty;
                if (secondContact.Contains("new_passportnumber"))
                {
                    secondCustomerTc = secondCustomerTc + " / " + (string)secondContact.Attributes["new_passportnumber"];
                }
            }
            if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "contact")
            {
                contact = service.Retrieve("contact", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                city = contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                city += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                city += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;
                address = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() : string.Empty;
                address += " " + city;
                passportNumber = contact.Contains("new_passportnumber") ? (string)contact.Attributes["new_passportnumber"] : string.Empty;
                if (contact.Contains("new_address3countryid"))
                {
                    foreignAddress = contact.Contains("new_nontcidentityaddress") ? contact.Attributes["new_nontcidentityaddress"].ToString() : string.Empty;
                    foreignAddress += " " + ((EntityReference)contact.Attributes["new_address3cityid"]).Name + "/" + ((EntityReference)contact.Attributes["new_address3countryid"]).Name;
                }
                referans = contact.Contains("new_referencecontactid") ? ((EntityReference)contact.Attributes["new_referencecontactid"]).Name : string.Empty;
                secondaryPersonName = contact.Contains("new_secondrypersonname") ? contact.Attributes["new_secondrypersonname"].ToString() : string.Empty;
                secondaryPersonLastName = contact.Contains("new_secondrypersonlastname") ? contact.Attributes["new_secondrypersonlastname"].ToString() : string.Empty;
                secondaryPersonPhone = contact.Contains("new_secondrypersonphone") ? contact.Attributes["new_secondrypersonphone"].ToString() : string.Empty;
            }
            else if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "account")
            {
                account = service.Retrieve("account", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                city = account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                city += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                city += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;
                address = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() : string.Empty;
                address += " " + city;
                referans = account.Contains("primarycontactid") ? ((EntityReference)account.Attributes["primarycontactid"]).Name : string.Empty;

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
                blok = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
                floor = product.Contains("new_floornumber") ? product.Attributes["new_floornumber"].ToString() : string.Empty;
                apartmentNo = product.Contains("new_homenumber") ? (string)product.Attributes["new_homenumber"] : string.Empty;
                m2 = product.Contains("new_netm2") ? (decimal)product.Attributes["new_netm2"] : 0;
                grossm2 = product.Contains("new_grossm2") ? (decimal)product.Attributes["new_grossm2"] : 0;
                blocktype = product.Contains("new_blocktypeid") ? ((EntityReference)product.Attributes["new_blocktypeid"]).Name : string.Empty;

            }
            Entity project = service.Retrieve("new_project", projectId, new ColumnSet(true));

            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();

            if (projectName == "853 NEF 03 Kağıthane")
            {
                dictionary1.Add("Proje", "NEF KAĞITHANE 03");
            }
            else
            {
                dictionary1.Add("proje", projectName);
            }
           
            dictionary1.Add("satisdanismani", ((EntityReference)quote.Attributes["ownerid"]).Name);
            if (quote.Contains("new_salesprocessdate"))
            {
                dictionary1.Add("satistarihi", ((DateTime)quote.Attributes["new_salesprocessdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
            }
            else
            {
                dictionary1.Add("satistarihi", string.Empty);
            }
            dictionary1.Add("referans", referans);
            dictionary1.Add("blok", blok);
            dictionary1.Add("daireno", apartmentNo);
            dictionary1.Add("kat", floor);
            dictionary1.Add("brutnet", grossm2.ToString("N2") + "/" + m2.ToString("N2"));
            dictionary1.Add("satisaesasbrutm2", grossm2.ToString("N2"));
            dictionary1.Add("adres1", address);

            if (foreignAddress != string.Empty)
            {

                dictionary1.Add("adres2", foreignAddress);
            }
            else
            {
                dictionary1.Add("adres2", string.Empty);
            }

            if (contact != null)
            {
                if (contact.Contains("fullname"))
                {
                    if (secondCustomerName != string.Empty)
                    {
                        dictionary1.Add("musterifirmaadsoyad", contact.Attributes["fullname"].ToString() + " - " + secondCustomerName);
                    }
                    else
                    {
                        dictionary1.Add("musterifirmaadsoyad", contact.Attributes["fullname"].ToString());
                    }
                }
                else
                {
                    dictionary1.Add("musterifirmaadsoyad", string.Empty);

                }
                if (contact.Contains("new_tcidentitynumber"))
                {
                    if (secondCustomerTc != string.Empty)
                    {
                        if (passportNumber != string.Empty)
                        {
                            dictionary1.Add("tcnovergino", contact.Attributes["new_tcidentitynumber"].ToString() + "/" + passportNumber + " - " + secondCustomerTc);
                        }
                        else
                        {
                            dictionary1.Add("tcnovergino", contact.Attributes["new_tcidentitynumber"].ToString() + " - " + secondCustomerTc);
                        }
                    }
                    else
                    {
                        if (passportNumber != string.Empty)
                        {
                            dictionary1.Add("tcnovergino", contact.Attributes["new_tcidentitynumber"].ToString() + "/" + passportNumber);
                        }
                        else
                        {
                            dictionary1.Add("tcnovergino", contact.Attributes["new_tcidentitynumber"].ToString());
                        }
                    }
                }
                else
                {
                    dictionary1.Add("tcnovergino", passportNumber);
                }
                if (contact.Contains("mobilephone"))
                {
                    dictionary1.Add("telefon", contact.Attributes["mobilephone"].ToString());
                }
                else
                {
                    dictionary1.Add("telefon", string.Empty);
                }
                if (contact.Contains("emailaddress1"))
                {
                    dictionary1.Add("eposta", contact.Attributes["emailaddress1"].ToString());
                }
                else
                {
                    dictionary1.Add("eposta", string.Empty);
                }

                if (!string.IsNullOrEmpty(secondaryPersonName) && !string.IsNullOrEmpty(secondaryPersonLastName))
                {
                     dictionary1.Add("ikincilkisiadsoyad", string.Format(" {0} {1}", secondaryPersonName, secondaryPersonLastName));
                }
                else
                {
                    dictionary1.Add("ikincilkisiadsoyad", string.Empty);
                }

                if (!string.IsNullOrEmpty(secondaryPersonPhone))
                {
                    dictionary1.Add("ikincilkisitelefon", secondaryPersonPhone);
                }
                else
                {
                    dictionary1.Add("ikincilkisitelefon", string.Empty);
                }

            }
            else if (account != null)
            {
                if (account.Contains("name"))
                {
                    dictionary1.Add("musterifirmaadsoyad", account.Attributes["name"].ToString());
                }
                else
                {
                    dictionary1.Add("musterifirmaadsoyad", string.Empty);

                }
                if (account.Contains("new_taxnumber"))
                {
                    dictionary1.Add("tcnovergino", account.Attributes["new_taxnumber"].ToString());
                }
                if (account.Contains("telephone1"))
                {
                    dictionary1.Add("telefon", account.Attributes["telephone1"].ToString());
                }
                else
                {
                    dictionary1.Add("telefon", string.Empty);
                }
                if (account.Contains("emailaddress1"))
                {
                    dictionary1.Add("eposta", account.Attributes["emailaddress1"].ToString());
                }
                else
                {
                    dictionary1.Add("eposta", string.Empty);
                }
            }
            if (quote.Contains("totalamount"))
            {
                dictionary1.Add("dairesatisfiyati", ((Money)quote.Attributes["totalamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("dairesatisfiyati", string.Empty);
            }

            if (quote.Contains("totallineitemamount"))
            {
                dictionary1.Add("listefiyati", ((Money)quote.Attributes["totallineitemamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("listefiyati", string.Empty);
            }



            if (quote.Contains("new_prepaymentamount"))
            {
                dictionary1.Add("kaporatutar", ((Money)quote.Attributes["new_prepaymentamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("kaporatutar", string.Empty);
            }
            if (quote.Contains("new_prepaymentdate"))
            {
                dictionary1.Add("kaporatarih", ((DateTime)quote.Attributes["new_prepaymentdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
            }
            else
            {
                dictionary1.Add("kaporatarih", string.Empty);
            }
            if (quote.Contains("new_taxamount"))
            {
                dictionary1.Add("kdvtutari", ((Money)quote.Attributes["new_taxamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("kdvtutari", string.Empty);
            }
            if (quote.Contains("new_taxofstampamount"))
            {
                dictionary1.Add("damgavergisitutari", ((Money)quote.Attributes["new_taxofstampamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("damgavergisitutari", string.Empty);
            }

            if (!string.IsNullOrEmpty(blocktype))
                dictionary1.Add("blocktype", blocktype);
            else
                dictionary1.Add("blocktype", string.Empty);


            if (projectName == "895 NEF Yalıkavak")
            {
                byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\YalikavakOnSatisFormu.docx", (DataSet)null, dictionary1);
                string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\YalikavakOnSatisFormu.docx";
                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
                return path1;
            }
            else if (projectId == Globals.TopkapiProjectId)
            {
                byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\TopkapiOnSatisFormu.docx", (DataSet)null, dictionary1);
                string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\TopkapiOnSatisFormu.docx";
                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
                return path1;
            }
            else
            {
                byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\PrePayment.docx", (DataSet)null, dictionary1);
                string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\PrePayment.docx";
                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
                return path1;

                
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
    }
}
