using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NEF.Library.Business
{
    public class PreSalesFormHelper
    {
        IOrganizationService service;
        public static string projectNameGlobal { get; set; }
        public string ExecutePreSalesForm(Guid QuoteId, string Path)
        {
            string folder = PreSalesFormHelper.CreateFolder(QuoteId, Path);
            QuoteDetail quote = new QuoteDetail();
            quote = GetQuoteDetail(QuoteId);
            QuoteInformation quoteInformation = this.CreateQuoteInformation(quote);
            return CreateDocument(Path, quoteInformation, folder);
        }

        private QuoteDetail GetQuoteDetail(Guid QuoteId)
        {
            service = MSCRM.AdminOrgService;
            QuoteDetail quote = new QuoteDetail();
            Entity q = service.Retrieve("quote", QuoteId, new ColumnSet(true));
            quote.NameSurname = q.Contains("customerid") ? ((EntityReference)q.Attributes["customerid"]).Name : string.Empty;
            string currencySymbol = string.Empty;
            Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)q["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
            if (currencyDetail != null && currencyDetail.Attributes.Contains("currencysymbol"))
                currencySymbol = currencyDetail["currencysymbol"].ToString();
            Entity contact = null;
            Entity account = null;
            string secondCustomerName = string.Empty;
            string secondCustomerTc = string.Empty;
            if (q.Contains("customerid") && ((EntityReference)q.Attributes["customerid"]).LogicalName.ToLower() == "contact")
            {
                if (q.Contains("new_secondcontactid"))
                {
                    Entity secondContact = service.Retrieve("contact", ((EntityReference)q.Attributes["new_secondcontactid"]).Id, new ColumnSet(true));
                    secondCustomerName = secondContact.Contains("fullname") ? (string)secondContact.Attributes["fullname"] : string.Empty;
                    secondCustomerTc = secondContact.Contains("new_tcidentitynumber") ? (string)secondContact.Attributes["new_tcidentitynumber"] : string.Empty;
                    if (secondContact.Contains("new_passportnumber"))
                    {
                        secondCustomerTc = secondCustomerTc + " / " + (string)secondContact.Attributes["new_passportnumber"];
                    }
                }
                contact = service.Retrieve("contact", ((EntityReference)q.Attributes["customerid"]).Id, new ColumnSet(true));
                quote.Address = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() + "/" : string.Empty;
                quote.Address += contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                quote.Address += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                quote.Address += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;

                quote.Phone = contact.Contains("mobilephone") ? contact.Attributes["mobilephone"].ToString() : string.Empty;
                quote.TcTaxNo = contact.Contains("new_tcidentitynumber") ? contact.Attributes["new_tcidentitynumber"].ToString() : string.Empty;
                quote.Email = contact.Contains("emailaddress1") ? contact.Attributes["emailaddress1"].ToString() : string.Empty;
                quote.PassportNumber = contact.Contains("new_passportnumber") ? (string)contact.Attributes["new_passportnumber"] : string.Empty;
                if (quote.PassportNumber != string.Empty)
                {
                    if (quote.TcTaxNo == string.Empty)
                    {
                        quote.TcTaxNo = quote.PassportNumber;
                    }
                    else
                    {
                        quote.TcTaxNo = quote.TcTaxNo + " / Pasaport No:" + quote.PassportNumber;
                    }
                }
                if (secondCustomerName != string.Empty)
                {
                    quote.NameSurname = quote.NameSurname + " - " + secondCustomerName;
                }
                if (secondCustomerTc != string.Empty)
                {
                    quote.TcTaxNo = quote.TcTaxNo + " - " + secondCustomerTc;
                }
                if (contact.Contains("new_address3countryid"))
                {
                    quote.ForeignAddress = contact.Contains("new_nontcidentityaddress") ? contact.Attributes["new_nontcidentityaddress"].ToString() : string.Empty;
                    quote.ForeignAddress += " " + ((EntityReference)contact.Attributes["new_address3cityid"]).Name + "/" + ((EntityReference)contact.Attributes["new_address3countryid"]).Name;
                }
                if (contact.Contains("new_nationalityid"))
                {
                    quote.Nationality = ((EntityReference)contact.Attributes["new_nationalityid"]).Name;
                }



            }
            else if (q.Contains("customerid") && ((EntityReference)q.Attributes["customerid"]).LogicalName.ToLower() == "account")
            {
                account = service.Retrieve("account", ((EntityReference)q.Attributes["customerid"]).Id, new ColumnSet(true));
                quote.Address = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() + "/" : string.Empty;
                quote.Address += account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                quote.Address += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                quote.Address += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;

                quote.Phone = account.Contains("telephone1") ? account.Attributes["telephone1"].ToString() : string.Empty;
                quote.TcTaxNo = account.Contains("new_taxnumber") ? account.Attributes["new_taxnumber"].ToString() : string.Empty;
                if (account.Contains("new_taxofficeid"))
                {
                    quote.TcTaxNo += " / " + ((EntityReference)account.Attributes["new_taxofficeid"]).Name.ToString();
                }

                quote.Email = account.Contains("emailaddress1") ? account.Attributes["emailaddress1"].ToString() : string.Empty;
            }
            if (q.Contains("new_amountwithtax"))
            {
                quote.AdvanceAmount = ((Money)q.Attributes["new_amountwithtax"]).Value.ToString("N2") + " " + currencySymbol;
            }
            if (q.Contains("new_contractdate"))
            {
                quote.ContractDate = ((DateTime)q.Attributes["new_contractdate"]).ToLocalTime().AddDays(-1).ToString("dd/MM/yyyy");
            }

            if (q.Contains("new_salesshareaccountid"))//Satışı Yapan Firma
            {
                Entity SalesAccount = service.Retrieve("new_share", ((EntityReference)q.Attributes["new_salesshareaccountid"]).Id, new ColumnSet(true));
                quote.SalesAccountName = SalesAccount.Contains("new_name") ? SalesAccount.Attributes["new_name"].ToString() : string.Empty;
                quote.SalesAccountAddress = SalesAccount.Contains("new_adressdetail") ? SalesAccount.Attributes["new_adressdetail"].ToString() : string.Empty;
                quote.SalesAccountEmail = SalesAccount.Contains("new_emailaddress") ? SalesAccount.Attributes["new_emailaddress"].ToString() : string.Empty;
                quote.SalesAccountMersisno = SalesAccount.Contains("new_mersisnumber") ? SalesAccount.Attributes["new_mersisnumber"].ToString() : string.Empty;
                quote.SalesAccountTel = SalesAccount.Contains("new_phonenumber") ? SalesAccount.Attributes["new_phonenumber"].ToString() : string.Empty;
                quote.SalesAccountFax = SalesAccount.Contains("new_faxnumber") ? SalesAccount.Attributes["new_faxnumber"].ToString() : string.Empty;
            }


            SetProjectDetail(QuoteId, quote, service);


            return quote;
        }

        private void SetProjectDetail(Guid QuoteId, QuoteDetail quote, IOrganizationService service)
        {
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
                quote.ProjectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
                projectNameGlobal = quote.ProjectName;
                quote.AdaParsel = product.Contains("new_blockofbuildingid") ? ((EntityReference)product.Attributes["new_blockofbuildingid"]).Name + "/" : string.Empty + "/";
                quote.AdaParsel += product.Contains("new_parcelid") ? ((EntityReference)product.Attributes["new_parcelid"]).Name : string.Empty;
                quote.Blok = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
                quote.Konum = product.Contains("new_locationid") ? ((EntityReference)product.Attributes["new_locationid"]).Name : string.Empty;
                quote.Floor = product.Contains("new_floornumber") ? product.Attributes["new_floornumber"].ToString() : string.Empty;
                quote.ApartmentNo = product.Contains("new_homenumber") ? (string)product.Attributes["new_homenumber"] : string.Empty;
                quote.ApartmentType = product.Contains("new_typeofhomeid") ? ((EntityReference)product.Attributes["new_typeofhomeid"]).Name : string.Empty;
                quote.m2 = product.Contains("new_grossm2") ? ((decimal)product.Attributes["new_grossm2"]).ToString("N2") + "/" : string.Empty + "/";
                quote.m2 += product.Contains("new_netm2") ? ((decimal)product.Attributes["new_netm2"]).ToString("N2") : string.Empty;
                quote.CityCounty = product.Contains("new_city") ? (string)product.Attributes["new_city"] + "/" : string.Empty + "/";
                quote.CityCounty += product.Contains("new_district") ? (string)product.Attributes["new_district"] + "/" : string.Empty + "/";
                quote.CityCounty += product.Contains("new_quarter") ? (string)product.Attributes["new_quarter"] : string.Empty;
                quote.garden = product.Contains("new_garden") ? ((decimal)product.Attributes["new_garden"]).ToString("N2") : " - ";
                quote.terracegross = product.Contains("new_terracegross") ? ((decimal)product.Attributes["new_terracegross"]).ToString("N2") : " - ";
                quote.balconym2 = product.Contains("new_balconym2") ? ((decimal)product.Attributes["new_balconym2"]).ToString("N2") : " - ";
                quote.DeliveryDate = product.Contains("new_deliverydate") ? ((DateTime)product.Attributes["new_deliverydate"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;




                Guid projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
                if (projectId != Guid.Empty)
                {
                    Entity project = service.Retrieve("new_project", projectId, new ColumnSet("new_deliverydate", "new_revisiondeliverydate", "new_buildingpermitsdate", "new_guaranteeinfo"));

                    if (project.Contains("new_buildingpermitsdate"))
                    {
                        quote.RegistrationDate = ((DateTime)project.Attributes["new_buildingpermitsdate"]).ToLocalTime().ToString("dd/MM/yyyy");
                    }
                    if (project.Contains("new_guaranteeinfo"))
                    {
                        quote.Guarantees = project.Attributes["new_guaranteeinfo"].ToString();
                    }
                }
                quote.bbnetalan = product.Contains("new_bbnetarea") ? ((decimal)product.Attributes["new_bbnetarea"]).ToString("N2") : string.Empty;
                quote.bbbrutalan = product.Contains("new_netm2") ? ((decimal)product.Attributes["new_netm2"]).ToString("N2") : string.Empty;
                quote.satisaesasalanm2 = product.Contains("new_grossm2") ? ((decimal)product.Attributes["new_grossm2"]).ToString("N2") : string.Empty;
                quote.satisesasalan = product.Contains("new_satisaesasalan") ? ((decimal)product.Attributes["new_satisaesasalan"]).ToString("N2") : string.Empty;
                quote.bbgenelbrutalan = product.Contains("new_bbgeneralgrossarea") ? ((decimal)product.Attributes["new_bbgeneralgrossarea"]).ToString("N2") : string.Empty;
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
        private static string CreateDocument(string path, QuoteInformation satisBilgileri, string folderName)
        {
            string path1 = string.Empty;
            {
                byte[] bytes = (byte[])null;
                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = satisBilgileri.DataSet;
                
                if (satisBilgileri.Fields.ContainsKey("accountname") && projectNameGlobal != "827 Inistanbul Topkapı")//Satış Yapan Firma Var ise ve Topkapı projesi değil ise
                {
                    bytes = DocumentMerge.WordDokumanOlustur(path + "DocumentMerge\\Templates\\presalesform2.docx", dataSet2, satisBilgileri.Fields);
                    path1 = path + "DocumentMerge\\Document\\" + folderName + "\\presalesform2.docx";
                }
                else if (!satisBilgileri.Fields.ContainsKey("accountname") && projectNameGlobal != "827 Inistanbul Topkapı")//Satış Yapan Firma yok ise ve Topkapı projesi değil ise
                {
                    bytes = DocumentMerge.WordDokumanOlustur(path + "DocumentMerge\\Templates\\presalesform.docx", dataSet2, satisBilgileri.Fields);
                    path1 = path + "DocumentMerge\\Document\\" + folderName + "\\presalesform.docx";
                }
                else
                {
                    bytes = DocumentMerge.WordDokumanOlustur(path + "DocumentMerge\\Templates\\Topkapipresalesform.docx", dataSet2, satisBilgileri.Fields);
                    path1 = path + "DocumentMerge\\Document\\" + folderName + "\\Topkapipresalesform.docx";
                }



                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
            }

            return path1;
        }
        private QuoteInformation CreateQuoteInformation(QuoteDetail quote)
        {

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(quote.ContractDate))
                dictionary.Add("tarih", quote.ContractDate);//Sözleşme Tarihinden 1 gün önce istendi
            else
                dictionary.Add("tarih", string.Empty);

            if (!string.IsNullOrEmpty(quote.NameSurname))
                dictionary.Add("adsoyad", quote.NameSurname);
            else
                dictionary.Add("adsoyad", string.Empty);

            if (!string.IsNullOrEmpty(quote.Address))
                dictionary.Add("adres", quote.Address);
            else
                dictionary.Add("adres", string.Empty);

            if (!string.IsNullOrEmpty(quote.ForeignAddress))
            {
                dictionary.Remove("adres");
                dictionary.Add("adres", quote.ForeignAddress);
            }


            if (!string.IsNullOrEmpty(quote.Address) && !string.IsNullOrEmpty(quote.Nationality) && quote.Nationality != "TC")
                dictionary.Add("tcadresi", quote.Address);
            else
                dictionary.Add("tcadresi", string.Empty);

            if (!string.IsNullOrEmpty(quote.Phone))
                dictionary.Add("telefon", quote.Phone);
            else
                dictionary.Add("telefon", string.Empty);

            if (!string.IsNullOrEmpty(quote.TcTaxNo))
                dictionary.Add("tcno", quote.TcTaxNo);
            else
                dictionary.Add("tcno", string.Empty);

            if (!string.IsNullOrEmpty(quote.Email))
                dictionary.Add("eposta", quote.Email);
            else
                dictionary.Add("eposta", string.Empty);

            if (quote.ProjectName == "853 NEF 03 Kağıthane")
            {
                dictionary.Add("projeadi", "NEF KAĞITHANE 03");
            }
            else
            {
                dictionary.Add("projeadi", string.Empty);
            }

            if (!string.IsNullOrEmpty(quote.garden))
                dictionary.Add("bahce", quote.garden);
            else
                dictionary.Add("bahce", string.Empty);

            if (!string.IsNullOrEmpty(quote.terracegross))
                dictionary.Add("teras", quote.terracegross);
            else
                dictionary.Add("teras", string.Empty);

            if (!string.IsNullOrEmpty(quote.balconym2))
                dictionary.Add("balkon", quote.balconym2);
            else
                dictionary.Add("balkon", string.Empty);

            if (!string.IsNullOrEmpty(quote.satisaesasalanm2))
                dictionary.Add("satisesasalanm2", quote.satisaesasalanm2);
            else
                dictionary.Add("satisesasalanm2", string.Empty);






            if (!string.IsNullOrEmpty(quote.CityCounty))
                dictionary.Add("ililcemahalle", quote.CityCounty);
            else
                dictionary.Add("ililcemahalle", string.Empty);

            if (!string.IsNullOrEmpty(quote.AdaParsel))
                dictionary.Add("adaparsel", quote.AdaParsel);
            else
                dictionary.Add("adaparsel", string.Empty);

            if (!string.IsNullOrEmpty(quote.Blok))
                dictionary.Add("blokno", quote.Blok);
            else
                dictionary.Add("blokno", string.Empty);

            if (!string.IsNullOrEmpty(quote.Konum))
                dictionary.Add("konum", quote.Konum);
            else
                dictionary.Add("konum", string.Empty);

            if (!string.IsNullOrEmpty(quote.Floor))
                dictionary.Add("kat", quote.Floor);
            else
                dictionary.Add("kat", string.Empty);

            if (!string.IsNullOrEmpty(quote.ApartmentNo))
                dictionary.Add("daireno", quote.ApartmentNo);
            else
                dictionary.Add("daireno", string.Empty);

            if (!string.IsNullOrEmpty(quote.ApartmentType))
                dictionary.Add("dairetipi", quote.ApartmentType);
            else
                dictionary.Add("dairetipi", string.Empty);

            if (!string.IsNullOrEmpty(quote.m2))
                dictionary.Add("satisaesasbrutm2", quote.m2);
            else
                dictionary.Add("satisaesasbrutm2", string.Empty);

            if (!string.IsNullOrEmpty(quote.AdvanceAmount))
                dictionary.Add("toplamtaksitlisatisbedeli", quote.AdvanceAmount);
            else
                dictionary.Add("toplamtaksitlisatisbedeli", string.Empty);

            dictionary.Add("toplampesinsatisbedeli", "0");//Sabit 0 basıyor


            if (!string.IsNullOrEmpty(quote.DeliveryDate))
                dictionary.Add("teslimtarihi", quote.DeliveryDate);
            else
                dictionary.Add("teslimtarihi", string.Empty);

            if (!string.IsNullOrEmpty(quote.RegistrationDate))
                dictionary.Add("yapiruhsatininalinistarihi", quote.RegistrationDate);
            else
                dictionary.Add("yapiruhsatininalinistarihi", string.Empty);

            if (!string.IsNullOrEmpty(quote.Guarantees))
                dictionary.Add("teminat", quote.Guarantees);
            else
                dictionary.Add("teminat", string.Empty);

            if (!string.IsNullOrEmpty(quote.SalesAccountName))//Satış Yapan Firma Var ise
            {
                if (!string.IsNullOrEmpty(quote.SalesAccountName))
                    dictionary.Add("accountname", quote.SalesAccountName);
                else
                    dictionary.Add("accountname", string.Empty);
                if (!string.IsNullOrEmpty(quote.SalesAccountAddress))
                    dictionary.Add("accountaddress", quote.SalesAccountAddress);
                else
                    dictionary.Add("accountaddress", string.Empty);
                if (!string.IsNullOrEmpty(quote.SalesAccountEmail))
                    dictionary.Add("accountemail", quote.SalesAccountEmail);
                else
                    dictionary.Add("accountemail", string.Empty);
                if (!string.IsNullOrEmpty(quote.SalesAccountMersisno))
                    dictionary.Add("accountmersisno", quote.SalesAccountMersisno);
                else
                    dictionary.Add("accountmersisno", string.Empty);
                if (!string.IsNullOrEmpty(quote.SalesAccountTel))
                    dictionary.Add("accounttelephone", quote.SalesAccountTel);
                else
                    dictionary.Add("accounttelephone", string.Empty);
                if (!string.IsNullOrEmpty(quote.SalesAccountFax))
                    dictionary.Add("accountfax", quote.SalesAccountFax);
                else
                    dictionary.Add("accountfax", string.Empty);
            }
            if (!string.IsNullOrEmpty(quote.bbnetalan))
                dictionary.Add("bbnetalan", quote.bbnetalan);
            else
                dictionary.Add("bbnetalan", string.Empty);

            if (!string.IsNullOrEmpty(quote.bbbrutalan))
                dictionary.Add("bbbrutalan", quote.bbbrutalan);
            else
                dictionary.Add("bbbrutalan", string.Empty);

            if (!string.IsNullOrEmpty(quote.satisesasalan))
                dictionary.Add("satisesasalan", quote.satisesasalan);
            else
                dictionary.Add("satisesasalan", string.Empty);

            if (!string.IsNullOrEmpty(quote.bbgenelbrutalan))
                dictionary.Add("bbgenelbrutalan", quote.bbgenelbrutalan);
            else
                dictionary.Add("bbgenelbrutalan", string.Empty);


            QuoteInformation quoteInformation = new QuoteInformation();
            DataSet dataSet = new DataSet();
            quoteInformation.DataSet = dataSet;
            quoteInformation.Fields = dictionary;
            return quoteInformation;

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
    public class QuoteDetail
    {
        public string NameSurname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string TcTaxNo { get; set; }
        public string Email { get; set; }
        public string ProjectName { get; set; }
        public string CityCounty { get; set; }
        public string AdaParsel { get; set; }
        public string Blok { get; set; }
        public string Konum { get; set; }
        public string Floor { get; set; }
        public string ApartmentNo { get; set; }
        public string ApartmentType { get; set; }
        public string m2 { get; set; }
        public string AdvanceAmount { get; set; }//Peşin Satış Bedeli        
        public string DeliveryDate { get; set; }//Teslim Tarihi
        public string RegistrationDate { get; set; }//Yapı Ruhsat Tarihi
        public string Guarantees { get; set; }//Teminat
        public string ContractDate { get; set; }//Sözleşme Tarihi
        public string ForeignAddress { get; set; }//Yabancı ülkedeki adresi
        public string PassportNumber { get; set; }//Pasaport No
        public string Nationality { get; set; }//Pasaport No
        public string SalesAccountName { get; set; }//satışı yapan Firma
        public string SalesAccountAddress { get; set; }//satışı yapan Firma
        public string SalesAccountEmail { get; set; }//satışı yapan Firma
        public string SalesAccountTel { get; set; }//satışı yapan Firma
        public string SalesAccountFax { get; set; }//satışı yapan Firma
        public string SalesAccountMersisno { get; set; }//satışı yapan Firma

        public string bbnetalan { get; set; }//BB Net Alanı (m2)*
        public string bbbrutalan { get; set; }//BB Brüt Alanı (m2)**
        public string satisesasalan { get; set; }//Satış Esas alan(m2) ***
        public string bbgenelbrutalan { get; set; }// BB Genel Brüt Alanı (m2) ****  
        public string garden { get; set; }
        public string terracegross { get; set; }
        public string balconym2 { get; set; }
        public string satisaesasalan { get; set; }

        public string satisaesasalanm2 { get; set; }
    }
    public class QuoteInformation
    {
        public Dictionary<string, string> Fields { get; set; }

        public DataSet DataSet { get; set; }
    }

}
