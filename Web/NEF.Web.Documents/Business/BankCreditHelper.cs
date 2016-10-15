using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NEF.Web.Documents.Business
{
    public class BankCreditHelper
    {

        IOrganizationService service;
        public static string projectNameGlobal { get; set; }

        public static string bankNameGlobal { get; set; }
        public string ExecuteContractCover(Guid bankCreditId, string Path)
        {
            string parsel = string.Empty;
            string ada = string.Empty;
            string pafta = string.Empty;
            string discrict = string.Empty;
            string deliveryDate = string.Empty;
            string bankName = string.Empty;
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
            string city = string.Empty;
            string county = string.Empty;
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
            Guid projectId = Guid.Empty;
            Guid QuoteId = Guid.Empty;
            Guid bankId = Guid.Empty;
            Entity contact = null;
            Entity account = null;
            Entity SalesAccount = null;
            service = MSCRM.AdminOrgService;
            Entity bankCreditRecord = service.Retrieve("new_bankcreditstatus", bankCreditId, new ColumnSet(true));
            if (bankCreditRecord != null)
            {
                QuoteId = bankCreditRecord.GetAttributeValue<EntityReference>("new_quoteid").Id;
                bankId = bankCreditRecord.GetAttributeValue<EntityReference>("new_bankid").Id;
            }
            else
            {
                return string.Empty;
            }

            Entity bank = service.Retrieve("new_bank", bankId, new ColumnSet(true));
            if (bank == null)
                return string.Empty;
            Entity quote = service.Retrieve("quote", QuoteId, new ColumnSet(true));
            if (quote == null)
                return string.Empty;

            bankName = bank.GetAttributeValue<string>("new_name");
            bankNameGlobal = bankName;

            Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
            if (currencyDetail != null && currencyDetail.Attributes.Contains("currencysymbol"))
                currencySymbol = currencyDetail["currencysymbol"].ToString();

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

                parsel = product.Contains("new_parcelid") ? ((EntityReference)product.Attributes["new_parcelid"]).Name : string.Empty;
                ada = product.Contains("new_blockofbuildingid") ? ((EntityReference)product.Attributes["new_blockofbuildingid"]).Name : string.Empty;
                pafta = product.Contains("new_threaderid") ? ((EntityReference)product.Attributes["new_threaderid"]).Name : string.Empty;
                discrict = product.Contains("new_district") ? (string)product.Attributes["new_district"] : string.Empty;
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
                deliveryDate = product.Contains("new_deliverydate") ? ((DateTime)product.Attributes["new_deliverydate"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
            }
            Entity project = service.Retrieve("new_project", projectId, new ColumnSet(true));
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            dictionary1.Add("TeslimTarihi", string.Empty);
            dictionary1.Add("SatışBrüt", grossm2.ToString("N2"));

            decimal sumOfPrePaymentAndVoucher = 0;

            if (contact != null)
            {
                if (!string.IsNullOrEmpty(contact.Attributes["fullname"].ToString()))
                    dictionary1.Add("İlgiliKişi", contact.Attributes["fullname"].ToString());
                else
                    dictionary1.Add("İlgiliKişi", string.Empty);
            }
            else if (account != null)
            {
                if (!string.IsNullOrEmpty(account.Attributes["name"].ToString()))
                    dictionary1.Add("İlgiliKişi", account.Attributes["name"].ToString());
                else
                    dictionary1.Add("İlgiliKişi", string.Empty);
            }

            if (bankCreditRecord.Contains("new_bankofficeid"))
                dictionary1.Add("BankaŞubesi", bankCreditRecord.GetAttributeValue<EntityReference>("new_bankofficeid").Name);
            else
                dictionary1.Add("BankaŞubesi", string.Empty);

            if (bankCreditRecord.Contains("new_appcreditamount"))
                dictionary1.Add("BaşvurulanKrediTutarı", ((Money)bankCreditRecord.Attributes["new_appcreditamount"]).Value.ToString("N2"));
            else
                dictionary1.Add("BaşvurulanKrediTutarı", string.Empty);

            if (!string.IsNullOrEmpty(bankName))
                dictionary1.Add("Banka", bankName);
            else
                dictionary1.Add("Banka", string.Empty);


            if (quote.Contains("new_prepaymentamount"))
            {
                dictionary1.Add("ÖnÖdemeTutarı", ((Money)quote.Attributes["new_prepaymentamount"]).Value.ToString("N2"));

                Entity kusuratEtiketi = this.GetKusuratEtiketi(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "new_name" });
                string yaziylaTutar = YaziyaCevir(((Money)quote.Attributes["new_prepaymentamount"]).Value,
                currencyDetail["currencysymbol"].ToString(), kusuratEtiketi["new_name"].ToString());
                dictionary1.Add("YazıylaÖnÖdemeTutarı", yaziylaTutar);
                sumOfPrePaymentAndVoucher += ((Money)quote.Attributes["new_prepaymentamount"]).Value;
            }

            if (quote.Contains("new_totalvoucheramount"))
            {
                dictionary1.Add("PeşinÖdemeTutarı", ((Money)quote.Attributes["new_totalvoucheramount"]).Value.ToString("N2") + " " + currencySymbol);
                sumOfPrePaymentAndVoucher += ((Money)quote.Attributes["new_totalvoucheramount"]).Value;
            }

            if (!string.IsNullOrEmpty(sumOfPrePaymentAndVoucher.ToString("N2")))
            {
                Entity kusuratEtiketi = this.GetKusuratEtiketi(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "new_name" });
                string yaziylaTutar = YaziyaCevir(sumOfPrePaymentAndVoucher, currencyDetail["currencysymbol"].ToString(), kusuratEtiketi["new_name"].ToString());
                dictionary1.Add("ÖnÖdemeTutarıPeşinÖdemeTutarı", sumOfPrePaymentAndVoucher.ToString("N2"));
                dictionary1.Add("YazıylaÖnÖdemeTutarıPeşinÖdemeTutarı", yaziylaTutar);

            }
            else
                dictionary1.Add("ÖnÖdemeTutarıPeşinÖdemeTutarı", string.Empty);

            if (!string.IsNullOrEmpty(projectName))
                dictionary1.Add("Proje", projectName);
            else
                dictionary1.Add("Proje", string.Empty);

            if (!string.IsNullOrEmpty(pafta))
                dictionary1.Add("Pafta", pafta);
            else
                dictionary1.Add("Pafta", string.Empty);

            if (!string.IsNullOrEmpty(ada))
                dictionary1.Add("Ada", ada);
            else
                dictionary1.Add("Ada", string.Empty);

            if (!string.IsNullOrEmpty(parsel))
                dictionary1.Add("Parsel", parsel);
            else
                dictionary1.Add("Parsel", string.Empty);

            if (!string.IsNullOrEmpty(apartmentNo))
                dictionary1.Add("DaireNo", apartmentNo);
            else
                dictionary1.Add("DaireNo", string.Empty);

            if (!string.IsNullOrEmpty(blok))
                dictionary1.Add("Blok", blok);
            else
                dictionary1.Add("Blok", string.Empty);

            if (quote.Contains("new_contractdate"))
                dictionary1.Add("SözleşmeTarihi", ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
            else
                dictionary1.Add("SözleşmeTarihi", string.Empty);





            if (bankCreditRecord.Contains("new_approvedcreditamount"))
                dictionary1.Add("OnaylananKrediTutarı", ((Money)bankCreditRecord.Attributes["new_approvedcreditamount"]).Value.ToString("N2"));
            else
                dictionary1.Add("OnaylananKrediTutarı", string.Empty);

            if (!string.IsNullOrEmpty(address))
                dictionary1.Add("AdresDetayı", address);
            else
                dictionary1.Add("AdresDetayı", string.Empty);

            if (!string.IsNullOrEmpty(discrict))
                dictionary1.Add("İlçe", discrict);
            else
                dictionary1.Add("İlçe", string.Empty);

            if (!string.IsNullOrEmpty(floor))
                dictionary1.Add("Kat", floor);
            else
                dictionary1.Add("Kat", string.Empty);

            if (!string.IsNullOrEmpty(apartmentCity))
                dictionary1.Add("İl", apartmentCity);
            else
                dictionary1.Add("İl", string.Empty);

            decimal totalAmountlessFreight = 0;
            decimal creditAmount = 0;

            if (quote.Contains("totalamountlessfreight"))
            {
                totalAmountlessFreight = ((Money)quote.Attributes["totalamountlessfreight"]).Value;
                dictionary1.Add("İndirimliKonutFiyatı", totalAmountlessFreight.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("İndirimliKonutFiyatı", string.Empty);
            }

            if (quote.Contains("new_creditamount"))
            {
                creditAmount = ((Money)quote.Attributes["new_creditamount"]).Value;

                dictionary1.Add("KrediTutarınınYarısı", (creditAmount / 2).ToString("N2"));


                decimal diff = totalAmountlessFreight - creditAmount;
                Entity kusuratEtiketi = this.GetKusuratEtiketi(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "new_name" });
                string yaziylaTutar = YaziyaCevir(diff,
                currencyDetail["currencysymbol"].ToString(), kusuratEtiketi["new_name"].ToString());

                dictionary1.Add("PeşinÖdemelerToplamı", diff.ToString("N2"));
                dictionary1.Add("PeşinÖdemelerToplamıYarısı", (diff / 2).ToString());
                dictionary1.Add("YazıylaPeşinÖdemelerToplamı", yaziylaTutar);
            }
            else
            {
                dictionary1.Add("YazıylaPeşinÖdemelerToplamı", string.Empty);
            }

            dictionary1.Add("today", DateTime.Now.ToShortDateString());
            dictionary1.Add("day", DateTime.Now.Day.ToString());
            dictionary1.Add("month", DateTime.Now.Month.ToString());
            dictionary1.Add("year", DateTime.Now.Year.ToString());
            dictionary1.Add("ParaBirimi", currencySymbol);
            dictionary1.Add("Köy", "..............");
            dictionary1.Add("Sokak", "..............");
            dictionary1.Add("Cadde", "..............");
            dictionary1.Add("Mahalle", "..............");
            dictionary1.Add("Mevki", "..............");
            DataSet dataSetSorted = null;

            if (projectName.Equals("853 NEF 03 Kağıthane") ||
                                         projectName.Equals("857 NEF 12 Merter") ||
                                         projectName.Equals("855 NEF 13 Merter"))
            {
                // akbank işbank garanti
                if (bankName.Equals("Akbank T.A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\NEF_03_12_13\\AKBANK\\AKBANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\AKBANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else if (bankName.Equals("Türkiye İş Bankası A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\NEF_03_12_13\\ISBANK\\ISBANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\ISBANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else if (bankName.Equals("Türkiye Garanti Bankası A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\NEF_03_12_13\\GARANTI\\GARANTI.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\GARANTI.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
            }
            else if (projectName.Equals("827 Inistanbul Topkapı"))
            {
                if (bankName.Equals("Türkiye İş Bankası A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\TOPKAPI\\ISBANK\\ISBANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\ISBANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else if (bankName.Equals("Yapı ve Kredi Bankası A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\TOPKAPI\\YAPIKREDI\\YAPIKREDI.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\YAPIKREDI.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else if (bankName.Equals("Türkiye Garanti Bankası A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\TOPKAPI\\GARANTI\\GARANTI.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\GARANTI.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else if (bankName.Equals("Türkiye Vakıflar Bankası T.A.O."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\TOPKAPI\\VAKIFBANK\\VAKIFBANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\VAKIFBANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }

            }
            else if (projectName.Equals("847 NEF 08 Kağıthane") ||
                                              projectName.Equals("837 NEF 06 Points") ||
                                              projectName.Equals("843 NEF 04 Points") ||
                                              projectName.Equals("841 NEF 25 Şişli") ||
                                              projectName.Equals("831 NEF 14 Kağıthane"))
            {
                if (bankName.Equals("Denizbank A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\NEF_08_06_04_25_14\\DENIZBANK\\DENIZBANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\DENIZBANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
            }
            else if (projectName.Equals("833 NEF 22 Ataköy"))
            {
                if (bankName.Equals("Türkiye İş Bankası A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\NEF_22\\ISBANK\\ISBANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\ISBANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
                else if (bankName.Equals("Odea Bank A.Ş."))
                {
                    string folder = BankCreditHelper.CreateFolder(bankCreditId, Path);
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\BankTemplates\\NEF_22\\ODEABANK\\ODEABANK.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\ODEABANK.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;
                }
            }
            else
            {
                return string.Empty;
            }
            return string.Empty;
        }
        private static string CreateFolder(Guid bankCreditId, string Path)
        {
            string str1 = bankCreditId.ToString();
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
            return TextToTranslateMoney.ParaYaziya(para, yaziyaGosterimTipi, paraBirim, kurusBirim);
        }
    }

}