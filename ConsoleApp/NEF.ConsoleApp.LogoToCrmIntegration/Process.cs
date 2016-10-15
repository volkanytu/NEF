using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.LogoToCrmIntegration
{
    class Process
    {
        internal static void ExecutePayment(SqlDataAccess crmSda, IOrganizationService service)
        {

            List<PaymentLogo> paymentList = LogoHelper.GetPaymentsFromLogo();

            if (paymentList.Count > 0)
            {


                foreach (PaymentLogo item in paymentList)
                {
                    Console.Clear();
                    Console.WriteLine(paymentList.IndexOf(item) + 1 + " / " + paymentList.Count);
                    Console.WriteLine(item.VoucherNumber);
                    PaymentLogo crm = LogoHelper.GetPaymentFromCrm(crmSda, item.VoucherNumber);
                    try
                    {

                        if (!string.IsNullOrEmpty(crm.CrmId))
                        {
                            item.CrmId = crm.CrmId;
                            if (item.VoucherAmount == 0 || item.VoucherAmount != crm.VoucherAmount)//crm virgül 
                                continue;
                            if (item.TransactionCurrencyName == string.Empty || (item.TransactionCurrencyName == "EUR" ? "Euro" : item.TransactionCurrencyName) != crm.TransactionCurrencyName)
                                continue;
                            if (item.BalanceAmount != crm.VoucherAmount - item.Amount)
                                continue;
                            LogoHelper.UpdatePaymentCrm(crmSda, item);
                        }
                    }
                    catch (Exception ex)
                    {
                        StreamWriter file2 = new StreamWriter(@"C:\Nef\LogoAktarimLog.txt", true);
                        file2.WriteLine("-----------------" + DateTime.Now.ToShortDateString() + "----------------------");
                        file2.WriteLine("SERİ NO:" + item.VoucherNumber);
                        file2.WriteLine(ex.Message);
                        file2.Close();
                    }
                }
            }
        }
    }
}
