 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using NEF.WebService.MobilApp;

namespace NEF.ConsoleApp.MobilWebServisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FenixMobilService.MobilServiceClient client = new FenixMobilService.MobilServiceClient();
            string json=client.GetCustomer(1,40, CustomerType.Contact);

            string json2 = client.GetMobilAppCustomer();
            Console.ReadLine();




            MobilService client2 = new MobilService();

            GetProject(client2);


            string emailContact = "uguromay1@gmail.com";
            CustomerDetailResult customerDetailReturn = Login(client2, emailContact);
            CustomerDetailResult customerDetailReturn2 = Login(client2, "cihanhanci@yandex.com.tr");
            string customerId = customerDetailReturn.Customer.CustomerId;
            string customerId2 = customerDetailReturn2.Customer.CustomerId;
            CustomerType customerType = customerDetailReturn.Customer.CustomerType.Value;
            CustomerType customerType2 = customerDetailReturn2.Customer.CustomerType.Value;
            string returnValue= client2.GetFoldhome(customerId);
            PaymentResult paymentReturn2 = GetPaymentList(client2, customerId2, customerType2);
            PaymentResult paymentReturn = GetPaymentList(client2, customerId, customerType);

            MsCrmResult foldhomeCreateReturn = CreateFoldhome(client2, customerDetailReturn, customerId, customerType);
            string foldhomeId = foldhomeCreateReturn.Message;

            MsCrmResult foldhomeUpdateReturn = UpdateFoldhome(client2, customerDetailReturn, customerId, customerType, foldhomeId);
            MsCrmResult foldhomeUpdate2Return = UpdateFoldhome(client2, customerDetailReturn, customerId, customerType, foldhomeId);
            MsCrmResult foldhomeCancelReturn = CancelFoldhome(client2, customerDetailReturn, customerId, customerType, foldhomeId);

            LeadInfo lead = new LeadInfo();
            lead.Customer = new CustomerInfo();
            lead.Customer.Email = "cihanhanci@yandex.com.tr";
            lead.Customer.Name = "Cihan";
            lead.Customer.Surname = "Hancı";
            lead.Customer.Phone = "+90-531-9657717";

            client2.CreateLead(lead);

            WebCustomerInfo webInfo = new WebCustomerInfo();
            webInfo.Customer = new CustomerInfo();
            webInfo.CustomerInvestmentRange = new InvestmentRange();
            webInfo.Quote = new Quote();
            webInfo.Customer.CustomerId = customerId2;
            webInfo.Customer.Email = customerDetailReturn2.Customer.Email;
            webInfo.Customer.CustomerType = CustomerType.Contact;
            webInfo.CustomerInvestmentRange = InvestmentRange.optionTwo;
            webInfo.Quote.ProductId = customerDetailReturn.QuoteList[0].ProductId;
            webInfo.Quote.ProductName = customerDetailReturn.QuoteList[0].ProductName;
            webInfo.Quote.ProjectCode = customerDetailReturn.QuoteList[0].ProjectCode;
            webInfo.Quote.ProjectName = customerDetailReturn.QuoteList[0].ProjectName;
            client2.CreateWebForm(webInfo);
         }

        private static ProjectResult GetProject(MobilService client)
        {
            
            string result = client.GetProject();

            JavaScriptSerializer set = new JavaScriptSerializer();
            set.MaxJsonLength = Int32.MaxValue;
            ProjectResult returnValue = set.Deserialize<ProjectResult>(result);
            return returnValue;
        }


        private static MsCrmResult CancelFoldhome(MobilService client, CustomerDetailResult customerDetailReturn, string customerId, CustomerType customerType, string foldhomeId)
        {
            Foldhome foldhomeActivity = new Foldhome();
            foldhomeActivity.FoldhomeId = foldhomeId;
            string result = client.UpdateStatusFoldhome(foldhomeActivity);

            JavaScriptSerializer set = new JavaScriptSerializer();
            set.MaxJsonLength = Int32.MaxValue;
            MsCrmResult returnValue = set.Deserialize<MsCrmResult>(result);
            return returnValue;
        }

        private static MsCrmResult UpdateFoldhome(MobilService client, CustomerDetailResult customerDetailReturn, string customerId, CustomerType customerType, string foldhomeId)
        {
            Foldhome foldhomeActivity = new Foldhome();
            foldhomeActivity.Amount = Convert.ToDecimal(90);
            foldhomeActivity.StartDate = new DateTime(2016, 07, 30, 10, 00, 00);
            foldhomeActivity.EndDate = new DateTime(2016, 07, 30, 18, 00, 00);
            foldhomeActivity.FoldhomeId = foldhomeId;
            foldhomeActivity.SurveyResult = 5;

            string result = client.UpdateFoldhome(foldhomeActivity);

            JavaScriptSerializer set = new JavaScriptSerializer();
            set.MaxJsonLength = Int32.MaxValue;
            MsCrmResult returnValue = set.Deserialize<MsCrmResult>(result);
            return returnValue;
        }

        private static MsCrmResult CreateFoldhome(MobilService client, CustomerDetailResult customerDetailReturn, string customerId, CustomerType customerType)
        {
            Foldhome foldhomeActivity = new Foldhome();
            foldhomeActivity.Amount = Convert.ToDecimal(60);
            foldhomeActivity.CustomerId = customerId;
            foldhomeActivity.CustomerType = customerType;
            foldhomeActivity.StartDate = new DateTime(2016, 07, 30, 10, 00, 00);
            foldhomeActivity.EndDate = new DateTime(2016, 07, 30, 12, 00, 00);
            foldhomeActivity.ProjectCode = customerDetailReturn.QuoteList[0].ProjectCode;
            foldhomeActivity.RoomType = RoomType.MiniFutbol;

            string result = client.CreateFoldhome(foldhomeActivity);

            JavaScriptSerializer set = new JavaScriptSerializer();
            set.MaxJsonLength = Int32.MaxValue;
            MsCrmResult returnValue = set.Deserialize<MsCrmResult>(result);
            return returnValue;
        }


        private static CustomerDetailResult Login(MobilService client, string emailContact)
        {
            string result = client.Login(emailContact);
            JavaScriptSerializer set = new JavaScriptSerializer();
            set.MaxJsonLength = Int32.MaxValue;
            CustomerDetailResult returnValue = set.Deserialize<CustomerDetailResult>(result);
            return returnValue;
        }

        private static PaymentResult GetPaymentList(MobilService client, string customerId, CustomerType customerType)
        {
            string result = client.GetPaymentList(customerId, customerType);
            JavaScriptSerializer set = new JavaScriptSerializer();
            set.MaxJsonLength = Int32.MaxValue;
            PaymentResult returnValue = set.Deserialize<PaymentResult>(result);
            return returnValue;
        }
    }
}
