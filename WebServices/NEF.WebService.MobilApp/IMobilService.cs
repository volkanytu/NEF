using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NEF.WebService.MobilApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IMobilService
    {

        [WebInvoke(Method = "POST", UriTemplate = "/Login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string Login(string emailAddress);


        [WebInvoke(Method = "POST", UriTemplate = "/GetPaymentList", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetPaymentList(string customerId, CustomerType customerType);


        [WebInvoke(Method = "POST", UriTemplate = "/GetProject", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetProject();


        [WebInvoke(Method = "POST", UriTemplate = "/GetCustomer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetCustomer(int page, int rowCount, CustomerType customerType);


        [WebInvoke(Method = "POST", UriTemplate = "/GetFoldhome", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetFoldhome(string customerId);


        [WebInvoke(Method = "POST", UriTemplate = "/CreateFoldhome", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CreateFoldhome(Foldhome foldhome);


        [WebInvoke(Method = "POST", UriTemplate = "/UpdateFoldhome", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string UpdateFoldhome(Foldhome foldhome);


        [WebInvoke(Method = "POST", UriTemplate = "/CreateWebForm", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CreateWebForm(WebCustomerInfo WebCustomerInfo);


        [WebInvoke(Method = "POST", UriTemplate = "/CreateLead", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CreateLead(LeadInfo LeadInfo);

        [WebInvoke(Method = "POST", UriTemplate = "/GetMobilAppCustomer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetMobilAppCustomer();
    }

    [DataContract]
    public class CustomerInfo
    {
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string IdentityNumber { get; set; }
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public CustomerType? CustomerType { get; set; }
    }


    [DataContract]
    public class WebCustomerInfo
    {
        [DataMember]
        public CustomerInfo Customer { get; set; }
        [DataMember]
        public InvestmentRange? CustomerInvestmentRange { get; set; }
        [DataMember]
        public Quote Quote { get; set; }
    }

    [DataContract]
    public class LeadInfo
    {
        [DataMember]
        public CustomerInfo Customer { get; set; }
    }

    [DataContract]
    public class Quote
    {

        [DataMember]
        public string ProjectCode { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProductId { get; set; }
        [DataMember]
        public string ProductName { get; set; }
    }

    [DataContract]
    public class PaymentDetail
    {
        [DataMember]
        public string PaymentId { get; set; }
        [DataMember]
        public string VNumber { get; set; }
        [DataMember]
        public string PaymentName { get; set; }
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public decimal PaymentAmount { get; set; }
        [DataMember]
        public decimal BalanceAmount { get; set; }
        [DataMember]
        public string TransactionName { get; set; }
        [DataMember]
        public string VStatus { get; set; }
        [DataMember]
        public string PaymentType { get; set; }
        [DataMember]
        public DateTime VDate { get; set; }
        [DataMember]
        public DateTime PaymentDate { get; set; }
        [DataMember]
        public string ProjectCode { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProductId { get; set; }
        [DataMember]
        public string ProductName { get; set; }
    }

    [DataContract]
    public class Foldhome
    {
        [DataMember]
        public string FoldhomeId { get; set; }
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public CustomerType CustomerType { get; set; }
        [DataMember]
        public string ProjectCode { get; set; }
        [DataMember]
        public RoomType RoomType { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public FoldhomeStatus? StatusCode { get; set; }
        [DataMember]
        public decimal? Amount { get; set; }
        [DataMember]
        public int? SurveyResult { get; set; }
        [DataMember]
        public PaymentStatus? PaymentStatusCode { get; set; }
    }

    [DataContract]
    public class Project
    {
        [DataMember]
        public string ProjectCode { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
    }

    [DataContract]
    public class CustomerProject
    {
        [DataMember]
        public CustomerInfo Customer { get; set; }
        [DataMember]
        public List<Quote> QuoteList { get; set; }

    }

    [DataContract]
    public class MsCrmResult
    {

        [DataMember]
        public bool Success { get; set; } //İşlemin başarı durumu

        [DataMember]
        public string Message { get; set; } //İşlem sonucu açıklaması
        [DataMember]
        public string CRMId { get; set; }
    }

    [DataContract]
    public class CustomerDetailResult
    {
        [DataMember]
        public MsCrmResult Result { get; set; }
        [DataMember]
        public CustomerInfo Customer { get; set; }
        [DataMember]
        public List<Quote> QuoteList { get; set; }

    }

    [DataContract]
    public class PaymentResult
    {
        [DataMember]
        public MsCrmResult Result { get; set; }

        [DataMember]
        public List<PaymentDetail> CustomerPayment { get; set; }
    }

    [DataContract]
    public class FoldhomeResult
    {
        [DataMember]
        public MsCrmResult Result { get; set; }

        [DataMember]
        public List<Foldhome> FoldhomeList { get; set; }
    }

    [DataContract]
    public class ProjectResult
    {
        [DataMember]
        public MsCrmResult Result { get; set; }

        [DataMember]
        public List<Project> ProjectList { get; set; }
    }

    [DataContract]
    public class CustomerProjectListResult
    {
        [DataMember]
        public MsCrmResult Result { get; set; }

        [DataMember]
        public List<CustomerProject> CustomerProjectList { get; set; }
    }

    public enum FoldhomeStatus
    {
        Revervasyon = 1,
        Tamamlandi = 2,
        Iptal = 3
    }

    public enum FoldhomeState
    {
        Etkin = 0,
        Tamamlandi = 1,
        Iptal = 2
    }

    public enum CustomerType
    {
        Account = 1,
        Contact = 2
    }


    public enum InvestmentRange
    {
        optionOne = 100000000,
        optionTwo = 100000001,
        optionThree = 100000002,
        optionFour = 100000003,
        optionFive = 100000004,
        optionSix = 100000005,
        optionSeven = 100000006,
        optionEight = 100000007,
        optionNine = 100000008,
        optionTen = 100000009,
        optionEleven = 1000000010
    }

    public enum RoomType
    {
        MisafirEvi = 1,
        DavetOdasi = 2,
        MiniFutbol = 3,
        Squash = 4,
        BasketbolSahasi = 5,
        Barbeku = 6,
        KatToplantiOdasi = 7,
        OzelStudyo = 8,
        Classroom = 9,
        Lounge = 10,
        PlaystationOdasi =11,
        PartyKaraokeOdasi=12,
        SinemaOdasi=13,
        MuzikOdasi=14,
        SefinOdasi=15,
        ToplantiOdasi=16
    }

    public enum PaymentStatus
    {
        OdemeBekliyor = 1,
        OdemeAlindi = 2
    }
}
