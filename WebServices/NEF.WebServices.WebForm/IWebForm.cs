using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NEF.WebServices.WebForm
{
    [ServiceContract]
    public interface IWebForm
    {

        [WebInvoke(Method = "POST", UriTemplate = "/CreateWebForm", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        MsCrmResult CreateWebForm(Webform form);
    }

    [DataContract]
    public class MsCrmResult
    {

        [DataMember]
        public bool Success { get; set; } //İşlemin başarı durumu

        [DataMember]
        public string Message { get; set; } //İşlem sonucu açıklaması
    }

    public class Webform
    {
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string CustomerSurname { get; set; }
        [DataMember]
        public string CustomerEmail { get; set; }
        [DataMember]
        public string CustomerMobilePhone { get; set; }
        [DataMember]
        public string ChannelOfAwareness { get; set; }//Haberdar olma kanalı
        [DataMember]
        public ContactPreferences? ContactPreferences { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool NefInformation { get; set; }
        [DataMember]
        public string SubParticipationSource { get; set; }//Alt katılım kaynağı
        [DataMember]
        public string InterestOfProjectCode { get; set; }//İlgilendiği Proje Kodu
        [DataMember]
        public InvestmentRange? CustomerInvestmentRange { get; set; }
        [DataMember]
        public FlatType? FlatTypeChoose { get; set; }
        [DataMember]
        public string UTM_Medium { get; set; }
        [DataMember]
        public string UTM_Campaign { get; set; }
        [DataMember]
        public Location? Location { get; set; }
        [DataMember]
        public ProcessType? ProcessType { get; set; }
    }

    public enum Location
    {
        Merter = 1,
        Kağıthane = 2
    }

    public enum ProcessType
    {
        ForSale = 1,
        ForRent = 2
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

    public enum FlatType
    {
        optionOne,
        optionTwo,
        optionThree,
        optionFour,
        optionFive,
        optionSix,

        optionSeven,
        optionEight,
        optionNine,
        optionTen,
        optionEleven,
        optionTwelve
    }

    public enum ContactPreferences
    {
        Phone,
        EMail
    }
}
