using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NEF.WebService.CompanyInternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICompanyInternal
    {

        [WebInvoke(Method = "POST", UriTemplate = "/GetQuote", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetQuote(string productNumber);
    }


    [DataContract]
    public class QuoteInfo
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string QuoteNumber { get; set; }
        [DataMember]
        public string LicenceNumber { get; set; }

    }

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
    public class QuoteInfoResult
    {
        [DataMember]
        public MsCrmResult Result { get; set; }
        [DataMember]
        public QuoteInfo QuoteInfo { get; set; }

    }


}
