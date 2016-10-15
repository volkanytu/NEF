
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace NEF.WebServices.Integration
{

    [ServiceContract]

    public interface IIntegration
    {

        [OperationContract]
        MsCrmResult GetQuote(string QuoteNumber);

        [OperationContract]
        MsCrmResult CancellationConfirmed(string QuoteNumber);

        [OperationContract]
        MsCrmResult CancellationDenied(string QuoteNumber);


        [OperationContract]
        Contact GetContactDetail(Guid contactId);

        [OperationContract]
        MsCrmResult CancelledAndTransfered(string QuoteNumber);

        [OperationContract]
        List<Country> GetCountries();

        [OperationContract]
        List<City> GetCities(Guid countryId);

        [OperationContract]
        List<Town> GetTowns(Guid cityId);

        [OperationContract]
        List<District> GetDistricts(Guid townId);

        [OperationContract]
        List<Nationality> GetNationalities();

        [OperationContract]
        List<Participation> GetParticipations();

        [OperationContract]
        List<SubParticipation> GetSubParticipations(Guid participationId);

        [OperationContract]
        List<StringMap> GetJobs();

        [OperationContract]
        List<StringMap> GetMaritalStatus();




    }

    [DataContract]
    public class MsCrmResult
    {

        [DataMember]
        public bool Success { get; set; } //İşlemin başarı durumu
        [DataMember]
        public QuoteDetail Result { get; set; } //İşlem sonucu açıklaması
        [DataMember]
        public string Message { get; set; } //İşlem sonucu açıklaması
    }

    public class QuoteDetail
    {
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string HomeNumber { get; set; }
        [DataMember]
        public string ApartmentIdentificationNumber { get; set; }
        [DataMember]
        public string Block { get; set; }
        [DataMember]
        public string PerQquareMeterAmount { get; set; }
        [DataMember]
        public string SalesConsultant { get; set; }
        [DataMember]
        public string SalesConsultantEmail { get; set; }
        [DataMember]
        public DateTime? SalesProcessDate { get; set; }
        [DataMember]
        public DateTime? ContractDate { get; set; }
        [DataMember]
        public DateTime? CanceledDate { get; set; }
        [DataMember]
        public string CanceledReason { get; set; }
        [DataMember]
        public string SubCanceledReason { get; set; }
        [DataMember]
        public string CanceledDescription { get; set; }
        [DataMember]
        public string PrePaymentAmount { get; set; }
        [DataMember]
        public string TotalAmountLessFreight { get; set; }
        [DataMember]
        public string IBAN { get; set; }
        [DataMember]
        public string BankOffice { get; set; }
        [DataMember]
        public string Bank { get; set; }
        [DataMember]
        public string CustomerAddress { get; set; }
        [DataMember]
        public string EMail { get; set; }
        [DataMember]
        public string TcTaxNo { get; set; }
        [DataMember]
        public string MobilePhone { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string TransactionCurrency { get; set; }

        [DataMember]
        public string ContractAccessOffice { get; set; } //Sözleşmenin Onaylandığı Noter
        [DataMember]
        public DateTime? contractAccessDate { get; set; } //Sözleşmenin Onaylandığı Tarih

        [DataMember]
        public string WageNumber { get; set; } //Yevmiye Numarası

        [DataMember]
        public string ContractCanceledOffice { get; set; } //	Dönme Hakkının Bildirildiği Noter
        [DataMember]
        public DateTime? ProtestDate { get; set; } //İhtarname Tarihi

        [DataMember]
        public string NotaryStatus { get; set; } //	Noter onaylı /Noter onaysız 

        [DataMember]
        public string SalesStatus { get; set; } //	Satışın Durumu
        [DataMember]
        public string KonutStatus { get; set; } //	Konutun Durumu
        [DataMember]
        public string SalesId { get; set; } //	QuoteId

        [DataMember]
        public string CalcelReasonId { get; set; } //	Satış İptal Detayı Id

        [DataMember]
        public string HouseCrmLink { get; set; } //	Konut Crm Link







    }



}
