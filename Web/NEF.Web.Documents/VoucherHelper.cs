using MessagingToolkit.Barcode.QRCode;
using Microsoft.Office.Interop.Word;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace NEF.Web.Documents
{
    public class VoucherHelper
    {
        IOrganizationService service;
        public static string quoteNumber { get; set; }
        public string ExecuteVoucher(Guid QuoteId, string Path, string CollaborateAccountName)
        {
            service = MSCRM.AdminOrgService;
            Entity quote = service.Retrieve("quote", QuoteId, new ColumnSet("new_salesshareaccountid", "quotenumber", "new_contractdate", "new_secondcontactid", "new_financialaccountid"));
            string salesAccountName = string.Empty;
            string secondCustomerName = string.Empty;
            string secondCustomerTc = string.Empty;
            string financialAccount = string.Empty;
            if (quote.Contains("new_financialaccountid"))
            {
                financialAccount = quote.GetAttributeValue<EntityReference>("new_financialaccountid").Name;
            }
            quoteNumber = (string)quote.Attributes["quotenumber"];
            if (quote.Contains("new_salesshareaccountid"))//Satışı Yapan Firma
            {
                Entity SalesAccount = service.Retrieve("new_share", ((EntityReference)quote.Attributes["new_salesshareaccountid"]).Id, new ColumnSet(true));
                salesAccountName = SalesAccount.Contains("new_name") ? SalesAccount.Attributes["new_name"].ToString() : string.Empty;
            }
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
            string projectName = string.Empty;
            string apartmentIdentificationNumber = string.Empty;
            string str1 = QuoteId.ToString();
            string str2 = string.Empty;
            if (!Directory.Exists(Path + "\\DocumentMerge"))
                Directory.CreateDirectory(Path + "\\DocumentMerge");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document\\" + str1))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document\\" + str1);

            ConditionExpression con = new ConditionExpression();
            con.AttributeName = "quoteid";
            con.Operator = ConditionOperator.Equal;
            con.Values.Add(QuoteId);

            FilterExpression _filter = new FilterExpression();
            _filter.FilterOperator = LogicalOperator.And;
            _filter.Conditions.Add(con);

            QueryExpression _Query = new QueryExpression("quotedetail");
            _Query.ColumnSet = new ColumnSet("productid");
            _Query.Criteria.FilterOperator = LogicalOperator.And;
            _Query.Criteria.Filters.Add(_filter);

            EntityCollection _Result = service.RetrieveMultiple(_Query);
            if (_Result.Entities.Count > 0)
            {
                Entity product = service.Retrieve("product", ((EntityReference)_Result.Entities[0].Attributes["productid"]).Id, new ColumnSet(true));
                projectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
                apartmentIdentificationNumber = product.Contains("productnumber") ? product.Attributes["productnumber"].ToString() : string.Empty;
            }




            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_vstatus";
            con2.Values.AddRange((object)2, (object)7);
            con2.Operator = ConditionOperator.NotIn;

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_isvoucher";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(true);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            List<byte[]> list = new List<byte[]>();
            List<string> keywordList = new List<string>();

            foreach (Entity p in Result.Entities)
            {
                if (CollaborateAccountName == "Timur Gayrimenkul")
                {
                    if (!((EntityReference)p.Attributes["new_collaborateaccountid"]).Name.Contains(CollaborateAccountName) && 
                        !((EntityReference)p.Attributes["new_collaborateaccountid"]).Name.Contains("BTE") &&
                        !((EntityReference)p.Attributes["new_collaborateaccountid"]).Name.Contains("TT Gayrimenkul ve Ticaret A.Ş."))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!((EntityReference)p.Attributes["new_collaborateaccountid"]).Name.Contains(CollaborateAccountName))
                    {
                        continue;
                    }
                }


                Dictionary<string, string> fields = new Dictionary<string, string>();
                keywordList.Add((string)p["new_vnumber"]);
                fields.Add("qrCode", "[" + (string)p["new_vnumber"] + "]");
                fields.Add("voucherno", (string)p["new_vnumber"]);
                fields.Add("düzenleme yeri", string.Empty);
                fields.Add("ödemeyeri", string.Empty);
                fields.Add("kefil", string.Empty);
                fields.Add("no", (Result.Entities.IndexOf(p) + 1).ToString());
                fields.Add("day", DateTime.Now.ToString("dd"));
                fields.Add("month", DateTime.Now.ToString("MM"));
                fields.Add("year", DateTime.Now.ToString("yyyy"));
                fields.Add("duzenlemetarihi", quote.Contains("new_contractdate") ? ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty);
                fields.Add("ProjeAdi", projectName);
                fields.Add("DaireKimlikNo", apartmentIdentificationNumber);





                if (p.Contains("new_date"))
                {
                    fields.Add("vade", ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"));
                    fields.Add("date", YaziyaCevir(((DateTime)p.Attributes["new_date"]).ToLocalTime()));
                }
                if (p.Contains("new_paymentamount"))
                {
                    Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                    currencyDetail["currencysymbol"].ToString();

                    Entity kusuratEtiketi = this.GetKusuratEtiketi(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "new_name" });

                    fields.Add("tutar", ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    fields.Add("yaziylatutar", YaziyaCevir(((Money)p.Attributes["new_paymentamount"]).Value, currencyDetail["currencysymbol"].ToString(), kusuratEtiketi["new_name"].ToString()));

                }
                if (p.Contains("new_contactid"))
                {
                    Entity contact = service.Retrieve("contact", ((EntityReference)p.Attributes["new_contactid"]).Id, new ColumnSet(true));
                    if (secondCustomerName != string.Empty)
                    {
                        fields.Add("fullname", contact.Contains("fullname") ? contact.Attributes["fullname"].ToString() + " - " + secondCustomerName : string.Empty);
                    }
                    else
                    {
                        fields.Add("fullname", contact.Contains("fullname") ? contact.Attributes["fullname"].ToString() : string.Empty);
                    }

                    string address = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() + " " : string.Empty;
                    address += contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                    address += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                    address += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;
                    fields.Add("adresi", address);
                    fields.Add("phone", contact.Contains("mobilephone") ? contact.Attributes["mobilephone"].ToString() : string.Empty);
                    string tc = contact.Contains("new_tcidentitynumber") ? contact.Attributes["new_tcidentitynumber"].ToString() + " " : string.Empty;
                    tc += contact.Contains("new_passportnumber") ? "Pasaport No:" + contact.Attributes["new_passportnumber"].ToString() : string.Empty;
                    if (secondCustomerTc != string.Empty)
                    {
                        tc = tc + " - " + secondCustomerTc;
                    }
                    fields.Add("tc", tc);
                    fields.Add("vergino", string.Empty);
                    fields.Add("vergidairesi", string.Empty);
                }
                else if (p.Contains("new_accountid"))
                {
                    Entity account = service.Retrieve("account", ((EntityReference)p.Attributes["new_accountid"]).Id, new ColumnSet(true));
                    fields.Add("fullname", account.Contains("name") ? account.Attributes["name"].ToString() : string.Empty);
                    string address = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() + " " : string.Empty;
                    address += account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                    address += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                    address += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;
                    fields.Add("adresi", address);
                    fields.Add("phone", account.Contains("telephone1") ? account.Attributes["telephone1"].ToString() : string.Empty);
                    fields.Add("vergino", account.Contains("new_taxnumber") ? account.Attributes["new_taxnumber"].ToString() : string.Empty);
                    fields.Add("vergidairesi", ((EntityReference)account.Attributes["new_taxofficeid"]).Name.ToString());
                    fields.Add("tc", string.Empty);

                }
                if (CollaborateAccountName.Contains("Timur Gayrimenkul"))
                {
                    if (salesAccountName != string.Empty && projectName != "827 Inistanbul Topkapı")
                    {
                        fields.Add("accountname", salesAccountName);
                        list.Add(DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\senet2.docx", (DataSet)null, fields));
                        Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                        currencyDetail["currencysymbol"].ToString();


                        CreateQRCodes(Path + "\\DocumentMerge\\Document\\" + str1, (string)p["new_vnumber"], financialAccount, ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"),
                            ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    }
                    else if (salesAccountName == string.Empty && projectName != "827 Inistanbul Topkapı")
                    {
                        list.Add(DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\senet.docx", (DataSet)null, fields));
                        Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                        currencyDetail["currencysymbol"].ToString();


                        CreateQRCodes(Path + "\\DocumentMerge\\Document\\" + str1, (string)p["new_vnumber"], financialAccount, ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"),
                            ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    }
                    else
                    {
                        list.Add(DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\topkapinefsenet.docx", (DataSet)null, fields));
                        Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                        currencyDetail["currencysymbol"].ToString();


                        CreateQRCodes(Path + "\\DocumentMerge\\Document\\" + str1, (string)p["new_vnumber"], financialAccount, ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"),
                            ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    }

                }
                else
                {
                    if (projectName != "827 Inistanbul Topkapı")
                    {
                        list.Add(DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\senetgyo.docx", (DataSet)null, fields));
                        //CreateQRCodes(Path + "\\DocumentMerge\\Document\\" + str1, (string)p["new_vnumber"]);

                        Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                        currencyDetail["currencysymbol"].ToString();


                        CreateQRCodes(Path + "\\DocumentMerge\\Document\\" + str1, (string)p["new_vnumber"], financialAccount, ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"),
                            ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    }
                    else
                    {
                        list.Add(DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\topkapiisgyosenet.docx", (DataSet)null, fields));

                        Entity currencyDetail = this.GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                        currencyDetail["currencysymbol"].ToString();

                        CreateQRCodes(Path + "\\DocumentMerge\\Document\\" + str1, (string)p["new_vnumber"], financialAccount, ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"),
                            ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    }

                }
            }

            if (list.Count > 0)
            {
                if (CollaborateAccountName.Contains("Timur Gayrimenkul"))
                {
                    System.IO.File.WriteAllBytes(Path + "DocumentMerge\\Document\\" + str1 + "\\senet.docx", DocumentMerge.WordDokumanBirlestir((IList<byte[]>)list));
                    str2 = Path + "DocumentMerge\\Document\\" + str1 + "\\senet.docx";
                }
                else
                {
                    System.IO.File.WriteAllBytes(Path + "DocumentMerge\\Document\\" + str1 + "\\senetisgyo.docx", DocumentMerge.WordDokumanBirlestir((IList<byte[]>)list));
                    str2 = Path + "DocumentMerge\\Document\\" + str1 + "\\senetisgyo.docx";
                }

            }

            var app = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = app.Documents.Add(str2, Visible: true);
            try
            {
                // app.Documents.Open(str2, Visible: false);
                //app.Documents.Add(str2, Visible: false);
                //app.Documents[0].Activate();
                wordDocument.Activate();

                foreach (string keyword in keywordList)
                {
                    var sel = app.Selection;
                    sel.Find.Text = string.Format("[{0}]", keyword);
                    sel.Find.Execute(Replace: WdReplace.wdReplaceNone);
                    sel.Range.Select();
                    var imgPath = Path + "DocumentMerge\\Document\\" + str1 + "\\" + keyword + ".jpg";
                    sel.InlineShapes.AddPicture(
                        FileName: imgPath,
                        LinkToFile: false,
                        SaveWithDocument: true);

                }
                wordDocument.SaveAs(str2);

                //wordDocument.Close(false); // Close the Word Document.
                //app.Quit(false); // Close Word Application.      

                ((Microsoft.Office.Interop.Word._Document)wordDocument).Close(Type.Missing, Type.Missing, Type.Missing);
                ((Microsoft.Office.Interop.Word._Application)app).Quit(Type.Missing, Type.Missing, Type.Missing);
                ((Microsoft.Office.Interop.Word._Application)app).Application.Quit(Type.Missing, Type.Missing, Type.Missing);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Release all Interop objects.
                if (wordDocument != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDocument);
                if (wordDocument != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                wordDocument = null;
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();


            }
            return str2;
        }

        private void CreateQRCodes(string path, string voucherNumber)
        {
            QRCodeEncoder encoder = new QRCodeEncoder();
            MessagingToolkit.Barcode.Common.BitMatrix a = encoder.Encode(voucherNumber, MessagingToolkit.Barcode.BarcodeFormat.QRCode, 75, 75);
            Bitmap mBitmap = new Bitmap(a.GetWidth(), a.GetHeight());
            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    mBitmap.SetPixel(i, j, a.Get(i, j) ? Color.Black : Color.White);
                }
            }
            mBitmap.Save(path + "\\" + voucherNumber + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void CreateQRCodes(string path, string voucherNumber, string financialAccount, string paymentDate, string paymentAmount)
        {
            string content = voucherNumber + Environment.NewLine + financialAccount + Environment.NewLine + paymentDate + Environment.NewLine + paymentAmount;
            QRCodeEncoder encoder = new QRCodeEncoder();
            MessagingToolkit.Barcode.Common.BitMatrix a = encoder.Encode(content, MessagingToolkit.Barcode.BarcodeFormat.QRCode, 150, 150);
            Bitmap mBitmap = new Bitmap(a.GetWidth(), a.GetHeight());
            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    mBitmap.SetPixel(i, j, a.Get(i, j) ? Color.Black : Color.White);
                }
            }
            mBitmap.Save(path + "\\" + voucherNumber + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
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

        private List<Entity> GetSatisSenetleri(Guid QuoteId)
        {
            ConditionExpression conditionExpression1 = new ConditionExpression();
            conditionExpression1.AttributeName = "new_quoteid";
            conditionExpression1.Operator = ConditionOperator.Equal;
            conditionExpression1.Values.Add((object)QuoteId);
            ConditionExpression conditionExpression2 = new ConditionExpression();
            conditionExpression2.AttributeName = "statuscode";
            conditionExpression2.Values.AddRange((object)2, (object)7);
            conditionExpression2.Operator = ConditionOperator.NotIn;
            ConditionExpression conditionExpression3 = new ConditionExpression();
            conditionExpression3.AttributeName = "new_isvoucher";
            conditionExpression3.Operator = ConditionOperator.Equal;
            conditionExpression3.Values.Add(true);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression1);
            filterExpression.Conditions.Add(conditionExpression2);
            filterExpression.Conditions.Add(conditionExpression3);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            columnSet.AddColumn("new_paymentid");
            columnSet.AddColumn("new_date");
            columnSet.AddColumn("new_paymentamount");
            OrderExpression orderExpression = new OrderExpression();
            orderExpression.AttributeName = "new_date";
            orderExpression.OrderType = OrderType.Ascending;
            QueryExpression queryExpression = new QueryExpression();
            queryExpression.ColumnSet = columnSet;
            queryExpression.Criteria = filterExpression;
            queryExpression.EntityName = "new_payment";
            queryExpression.Orders.Add(orderExpression);
            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)this.service.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = (QueryBase)queryExpression
            });
            if (multipleResponse.EntityCollection.Entities != null || multipleResponse.EntityCollection.Entities.Count > 0)
                return Enumerable.ToList<Entity>((IEnumerable<Entity>)multipleResponse.EntityCollection.Entities);
            else
                return (List<Entity>)null;
        }
        private Entity GetContactDetail(Guid id, string[] columns)
        {
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = "contactid";
            conditionExpression.Operator = ConditionOperator.Equal;
            conditionExpression.Values.Add((object)id);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            foreach (string column in columns)
                columnSet.AddColumn(column);
            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)this.service.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = (QueryBase)new QueryExpression()
                {
                    ColumnSet = columnSet,
                    Criteria = filterExpression,
                    EntityName = "contact"
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
        private string YaziyaCevir(DateTime tarih)
        {
            switch (tarih.Month)
            {
                case 1:
                    int num1 = tarih.Day;
                    string str1 = num1.ToString();
                    string str2 = " Ocak ";
                    num1 = tarih.Year;
                    string str3 = num1.ToString();
                    return str1 + str2 + str3;
                case 2:
                    int num2 = tarih.Day;
                    string str4 = num2.ToString();
                    string str5 = " Şubat ";
                    num2 = tarih.Year;
                    string str6 = num2.ToString();
                    return str4 + str5 + str6;
                case 3:
                    int num3 = tarih.Day;
                    string str7 = num3.ToString();
                    string str8 = " Mart ";
                    num3 = tarih.Year;
                    string str9 = num3.ToString();
                    return str7 + str8 + str9;
                case 4:
                    int num4 = tarih.Day;
                    string str10 = num4.ToString();
                    string str11 = " Nisan ";
                    num4 = tarih.Year;
                    string str12 = num4.ToString();
                    return str10 + str11 + str12;
                case 5:
                    int num5 = tarih.Day;
                    string str13 = num5.ToString();
                    string str14 = " Mayıs ";
                    num5 = tarih.Year;
                    string str15 = num5.ToString();
                    return str13 + str14 + str15;
                case 6:
                    int num6 = tarih.Day;
                    string str16 = num6.ToString();
                    string str17 = " Haziran ";
                    num6 = tarih.Year;
                    string str18 = num6.ToString();
                    return str16 + str17 + str18;
                case 7:
                    int num7 = tarih.Day;
                    string str19 = num7.ToString();
                    string str20 = " Temmuz ";
                    num7 = tarih.Year;
                    string str21 = num7.ToString();
                    return str19 + str20 + str21;
                case 8:
                    int num8 = tarih.Day;
                    string str22 = num8.ToString();
                    string str23 = " Ağustos ";
                    num8 = tarih.Year;
                    string str24 = num8.ToString();
                    return str22 + str23 + str24;
                case 9:
                    int num9 = tarih.Day;
                    string str25 = num9.ToString();
                    string str26 = " Eylül ";
                    num9 = tarih.Year;
                    string str27 = num9.ToString();
                    return str25 + str26 + str27;
                case 10:
                    int num10 = tarih.Day;
                    string str28 = num10.ToString();
                    string str29 = " Ekim ";
                    num10 = tarih.Year;
                    string str30 = num10.ToString();
                    return str28 + str29 + str30;
                case 11:
                    int num11 = tarih.Day;
                    string str31 = num11.ToString();
                    string str32 = " Kasım ";
                    num11 = tarih.Year;
                    string str33 = num11.ToString();
                    return str31 + str32 + str33;
                case 12:
                    int num12 = tarih.Day;
                    string str34 = num12.ToString();
                    string str35 = " Aralık ";
                    num12 = tarih.Year;
                    string str36 = num12.ToString();
                    return str34 + str35 + str36;
                default:
                    return tarih.ToShortDateString();
            }
        }

    }
    public class VoucherDetail
    {
        public string ProjectName { get; set; }
        public string ApartmentNo { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string PlaceOfIssue { get; set; }
        public string PlaceOfPayment { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerTc { get; set; }
        public string CustomerTaxOffice { get; set; }
        public string CustomerTaxNo { get; set; }


    }
}