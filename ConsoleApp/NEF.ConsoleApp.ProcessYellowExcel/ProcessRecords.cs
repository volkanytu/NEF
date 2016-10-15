using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using NEF.Library.Business;
using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
namespace NEF.ConsoleApp.ProcessYellowExcel
{
    public static class ProcessRecords
    {
        public static MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            int totalCount = 0;
            int errorCount = 0;
            int successCount = 0;

            IOrganizationService service = MSCRM.GetOrgService(true);

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            DataTable lstImports = QuoteHelper.GetNotProcessedYellowExcelQuotes(sda);

            Console.SetCursorPosition(0, 1);
            Console.WriteLine("Listeler çekildi. Adet:" + lstImports.Rows.Count.ToString());

            if (lstImports.Rows.Count > 0)
            {
                for (int i = 0; i < lstImports.Rows.Count; i++)
                {
                    Guid quoteId = new Guid(Convert.ToString(lstImports.Rows[i]["QuoteId"]));

                    try
                    {
                        Console.SetCursorPosition(0, 2);
                        Console.WriteLine((i + 1).ToString() + ". Liste İşleniyor.");
                        //HATALI KAYITLARI SİLER
                        PaymentHelper.DeletePaymentsIfExist(quoteId, service, sda);
                        totalCount = 0;
                        errorCount = 0;
                        successCount = 0;

                        int customerIdType = Convert.ToInt32(lstImports.Rows[i]["CustomerIdType"]);
                        Guid customerId = new Guid(Convert.ToString(lstImports.Rows[i]["CustomerId"]));
                        Guid financialAccount = new Guid(Convert.ToString(lstImports.Rows[i]["new_financialaccountid"]));
                        Decimal totalAmountQuote = Convert.ToDecimal(lstImports.Rows[i]["totalamountlessfreight"]);
                        Decimal totalLineItemAmount = Convert.ToDecimal(lstImports.Rows[i]["TotalLineItemAmount"]);

                        MsCrmResult resultProcessing = QuoteHelper.UpdateYellowExcelState(quoteId, 2, service); //İşleniyor

                        if (resultProcessing.Success)
                        {
                            DataTable dt = QuoteHelper.GetNotProcessedYellowExcelData(quoteId, "Sheet1", sda);

                            Console.SetCursorPosition(0, 3);
                            Console.WriteLine((i + 1).ToString() + ". Liste Kayıt sayısı: " + dt.Rows.Count.ToString());

                            if (dt.Rows.Count > 0)
                            {
                                totalCount = dt.Rows.Count;
                                decimal totalAmountExcel = 0;
                                foreach (DataRow currentPayment in dt.Rows)
                                {
                                    if (DBNull.Value != currentPayment["Tutar"])
                                    {
                                        totalAmountExcel += Convert.ToDecimal(currentPayment["Tutar"]);
                                    }
                                }
                                if (totalAmountExcel != totalAmountQuote)
                                {
                                    string message = "İndirimli Konut Fiyatı, Sarı Excel tutar toplamı ile aynı değil. Lütfen kontrol edin.";
                                    message += Environment.NewLine;
                                    message += "Excel Tutar Toplamı : " + totalAmountExcel.ToString("#.##");
                                    message += Environment.NewLine;
                                    message += "İndirimli Konut Fiyatı : " + totalAmountQuote.ToString("#.##");
                                    QuoteHelper.LogYellowExcelError(quoteId, message, service);
                                    SendInformationMail(quoteId, 0, service);
                                    break;
                                }
                                for (int j = 0; j < totalCount; j++)
                                {
                                    Console.SetCursorPosition(0, 4);
                                    Console.WriteLine("Sayaç:" + (j + 1).ToString());

                                    //Satıs tutarı kontrolü geçici bir süre kapatıldı.
                                    //if (j == 0)
                                    //{
                                    //    if (totalLineItemAmount == Convert.ToDecimal(dt.Rows[j]["Satıs Tutarı"]))
                                    //    {

                                    //    }
                                    //}


                                    if ((DBNull.Value == dt.Rows[j]["Tutar"] || Convert.ToDecimal(dt.Rows[j]["Tutar"]) == 0) ||
                                        DBNull.Value == dt.Rows[j]["Vade Gün"] || dt.Rows[j]["Vade Gün"].ToString().Contains("1900"))
                                        continue;

                                    if (DBNull.Value == dt.Rows[j]["Ödeme Tipi"] || Convert.ToString(dt.Rows[j]["Ödeme Tipi"]).Trim().ToUpper().Equals("KAPARO"))
                                        continue;


                                    #region | PROCESS ROW |
                                    try
                                    {
                                        SetPayment(dt.Rows[j], quoteId, customerIdType, customerId, financialAccount, service);

                                    }
                                    catch (Exception ex)
                                    {
                                        //Console.WriteLine("hata 1" + ex.Message);
                                        //Console.ReadLine();

                                        QuoteHelper.UpdateYellowExcelState(quoteId, 4, service, ex.Message); //Hata alındı
                                        SendInformationMail(quoteId, 0, service);
                                    }
                                    #endregion

                                    Console.SetCursorPosition(0, 5);
                                    Console.WriteLine("Hata:" + errorCount.ToString());

                                    Console.SetCursorPosition(0, 6);
                                    Console.WriteLine("Başarılı:" + successCount.ToString());
                                }
                            }

                        }
                        QuoteHelper.UpdateYellowExcelState(quoteId, 3, service, "Excel başarılı bir şekilde işlendi."); //İşlendi
                        SendInformationMail(quoteId, 1, service);

                    }
                    catch (Exception ex)
                    {
                        QuoteHelper.LogYellowExcelError(quoteId, ex.Message, service);
                        returnValue.Result = ex.Message;
                        //Console.WriteLine("hata 1" + ex.Message);
                        //Console.ReadLine();
                    }
                }
                returnValue.Success = true;
                returnValue.Result = "Listeler işlendi...";
            }
            return returnValue;
        }
        public static void SendInformationMail(Guid quoteId, int status, IOrganizationService service)
        {
            try
            {
                #region Create Email

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["addressused"] = "Mail_SatisDestek@nef.com.tr";


                Entity ccParty = new Entity("activityparty");
                ccParty["addressused"] = "erhan.serter@nef.com.tr";

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };

                email["cc"] = new Entity[] { ccParty };
                email["from"] = new Entity[] { fromParty };
                email["subject"] = DateTime.Now.AddDays(-1).Date.ToString("dd/MM/yyyy") + " Tarihli Satışlar";
                email["description"] = GetMailBody(quoteId, status);
                email["directioncode"] = true;
                Guid id = service.Create(email);

                #endregion

                #region Send Email
                var req = new SendEmailRequest
                {
                    EmailId = id,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };

                try
                {
                    var res = (SendEmailResponse)service.Execute(req);

                }
                catch (Exception ex)
                {

                }
                #endregion
            }
            catch (Exception ex)
            {

            }
        }
        private static void SetPayment(DataRow dr, Guid quoteId, int customerIdType, Guid customerId, Guid financialAccount, IOrganizationService service)
        {
            string paymentType = Convert.ToString(dr["Ödeme Tipi"]);
            string currencyType = Convert.ToString(dr["ParaBirimi"]);
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            Payment payment = new Payment();
            payment.PaymentAmount = Convert.ToDecimal(dr["Tutar"]);
            switch (paymentType)
            {
                case "Banka Kredisi"://Banka Ödemesi
                    payment.PaymentType = PaymentTypes.BankaOdemesi;
                    payment.PaymentAccountingTypes = PaymentAccountingTypes.MusteriPesinat_BankaKredisi;
                    break;
                case "Peşinat"://Pesin Odeme
                    payment.PaymentType = PaymentTypes.PesinOdeme;
                    payment.PaymentAccountingTypes = PaymentAccountingTypes.MusteriPesinat_Nakit;
                    break;
                case "Taksit"://Düzenli taksit
                    payment.PaymentType = PaymentTypes.DuzenliTaksit;
                    payment.PaymentAccountingTypes = PaymentAccountingTypes.MusteriTaksit;
                    break;
                case "Ara Ödeme":
                    payment.PaymentType = PaymentTypes.AraOdeme;
                    payment.PaymentAccountingTypes = PaymentAccountingTypes.MusteriTaksit;
                    break;
                default:
                    break;
            }
            if (customerIdType == 1)// account
                payment.Account = new EntityReference("account", customerId);
            else if (customerIdType == 2)//contact
                payment.Contact = new EntityReference("contact", customerId);

            payment.PaymentDate = Convert.ToDateTime(dr["Vade Gün"]);
            payment.FinancialAccount = new EntityReference("new_financialaccount", financialAccount);
            TransactionCurrency tcurrency = CurrencyHelper.GetCurrencyByName(currencyType, sda);
            payment.Currency = new EntityReference("transactioncurrency", tcurrency.TransactionCurrencyId);
            payment.Quote = new EntityReference("quote", quoteId);
            switch (payment.PaymentType)
            {
                case PaymentTypes.BankaOdemesi: payment.Name = "Banka Ödemesi - " + DateTime.Now.ToShortDateString(); break;
                case PaymentTypes.DuzenliTaksit: payment.Name = "Düzenli Taksit - " + DateTime.Now.ToShortDateString(); break;
                case PaymentTypes.PesinOdeme: payment.Name = "Pesin Ödeme - " + DateTime.Now.ToShortDateString(); break;
                case PaymentTypes.AraOdeme: payment.Name = "Ara Ödeme - " + DateTime.Now.ToShortDateString(); break;
                default:
                    break;
            }
            PaymentHelper.CreateOrUpdatePayment(payment, service);
            sda.closeConnection();

        }

        private static string GetMailBody(Guid quoteId, int status)
        {
            string retVal = string.Empty;
            string url = "http://fenixcrm.nef.com.tr/FENiX/main.aspx?etc=1084&extraqs=&histKey=708831980&id=%7b";
            url += quoteId.ToString().ToUpper();
            url += "%7d&newWindow=true&pagetype=entityrecord#779082638";

            if (status == 1)
            {
                retVal += "Merhaba,";
                retVal += "<br>";
                retVal += "Satış üzerinde ödeme kayıtlarınız başarıyla oluşturulmuştur. Aşağıdaki linkten satışa ulaşabilirsiniz.";
                retVal += "<br>";
                retVal += "<a href=\"" + url + "\"> Satış </a>";
                retVal += "<br>";
                retVal += "Saygılarımızla";

            }
            else if (status == 0)
            {
                retVal += "Merhaba,";
                retVal += "<br>";
                retVal += "Satış üzerinde ödeme kayıtları oluştururken bir hata ile karşılaşıldı. Lütfen kontrol edebilirsiniz.";
                retVal += "<br>";
                retVal += "<a href=\"" + url + "\"> Satış </a>";
                retVal += "<br>";
                retVal += "Saygılarımızla";
            }
            return retVal;
        }
    }
}
