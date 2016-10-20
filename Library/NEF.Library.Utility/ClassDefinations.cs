using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NEF.Library.Utility
{
    public class UserHeaderInfo
    {
        public int OpenActivityCount { get; set; }
        public int OpenOppCount { get; set; }
        public int SalesCount { get; set; }
        public int PlannedSalesCount { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal PlannedSalesAmount { get; set; }
        public string SalesAmountString { get; set; }
        public string PlannedSalesAmountString { get; set; }
    }

    public class HomeOption
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Account { get; set; }
        public EntityReference Product { get; set; }
        public DateTime? OptionDate { get; set; }
        public string OptionDateString { get; set; }
        public EntityReference Owner { get; set; }
        public EntityReference Oppotunity { get; set; }
        public bool AdministratorOption { get; set; }
        public StringMap Status { get; set; }
    }

    public class HomeRentOption
    {
        public EntityReference Product { get; set; }
        public EntityReference Currency { get; set; }
        public int? UsedRenatAndSalesStatus { get; set; }
        public int? GoodsStatus { get; set; }
        public int? RentalMonths { get; set; }
        public string RentalNot { get; set; }
        public decimal? PaymentOfHire { get; set; }
    }


    public class UserFeed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public EntityReference User { get; set; }
        public StringMap Status { get; set; }
        public StringMap FeedType { get; set; }
    }

    #region | LOGO CLASSES |

    public class SatisOdemePlanlari
    {
        public string AlacakCariHesapKodu { get; set; }
        public string CariHesapKodu { get; set; }
        public int IslemTipi { get; set; }
        public string OnayKodu { get; set; }
        public string OnayNO { get; set; }
        public Guid SatisId { get; set; }
        public int SatisRef { get; set; }
        public Guid SenetId { get; set; }
        public string SenetNo { get; set; }
        public decimal Tutar { get; set; }
        public DateTime? VadeTarihi { get; set; }
    }

    public class Cariler
    {
        public string Adres1 { get; set; }
        public string Adres2 { get; set; }
        public Guid CariID { get; set; }
        public string CariKod { get; set; }
        public string CepTel1 { get; set; }
        public string CepTel2 { get; set; }
        public string DaireKimlikNo { get; set; }
        public string Eposta1 { get; set; }
        public string Eposta2 { get; set; }
        public string Fax { get; set; }
        public string GrupSirketKodu { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public int IslemTipi { get; set; }
        public string Semt { get; set; }
        public string SozlesmeDurumu { get; set; }
        public string TeslimSekli { get; set; }
        public string Ulke { get; set; }
        public string Unvan { get; set; }
        public string VatandaslikNo { get; set; }
        public string VergiDairesi { get; set; }
        public string ProjeKodu { get; set; }
    }
    public class Satislar
    {
        public string BolumKodu { get; set; }
        public string CariHesapKodu { get; set; }
        public Guid CariId { get; set; }
        public decimal DovizKuru { get; set; }
        public Guid GrupSirketId { get; set; }
        public string GrupSirketKodu { get; set; }
        public decimal IndirimliTutar { get; set; }
        public int IslemTipi { get; set; }
        public string IsYeri { get; set; }
        public decimal KdvOran { get; set; }
        public decimal KdvTutar { get; set; }
        public string ParaBirimi { get; set; }
        public Guid ProjeID { get; set; }
        public string ProjeKimligi { get; set; }
        public string ProjeKodu { get; set; }
        public string SatisDurumu { get; set; }
        public Guid SatisID { get; set; }
        public string SatisNo { get; set; }
        public int SatisRef { get; set; }
        public DateTime? SozlesmeTarih { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public string UniteKodu { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class LogoAccount
    {
        public string DonemNo { get; set; }
        public string FirmaNo { get; set; }
    }

    public class ExpenseCenter
    {
        public string DaireKimlikNo { get; set; }
        public decimal Fiyat { get; set; }
        public int IslemTipi { get; set; }
        public string MasrafAd { get; set; }
        public Guid MasrafMerkezId { get; set; }
        public string ParaBirimi { get; set; }
        public Guid ProjeID { get; set; }
        public string ProjeKodu { get; set; }
    }

    public class PaymentLogo
    {
        public string VoucherNumber { get; set; }//Senet No
        public decimal VoucherAmount { get; set; }//Ödenmesi Gereken Tutar
        public decimal Amount { get; set; } //Ödenen Tutar
        public decimal BalanceAmount { get; set; } //Kalan Tutar
        public string Status { get; set; }
        public string TransactionCurrencyName { get; set; }
        public string CrmId { get; set; }

    }
    #endregion

    public class ExchangeRate
    {
        public Guid Id { get; set; }
        public EntityReference Currency { get; set; }
        public DateTime RateDate { get; set; }
        public decimal SaleRate { get; set; }
        public decimal BuyRate { get; set; }
    }

    public class PriceLevel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EntityReference Currency { get; set; }
    }

    public class ProductPriceLevel
    {
        public Guid Id { get; set; }
        public EntityReference PriceLevel { get; set; }
        public EntityReference Product { get; set; }
        public EntityReference Currency { get; set; }
        public decimal Price { get; set; }
    }


    public class HouseImport
    {
        public string Name { get; set; }
        public Guid HouseImportId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class ChartKeyValues
    {
        public int SortNo { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string ValueText
        {
            get
            {
                return this.Value.ToString("N0", CultureInfo.CurrentCulture);
            }
            set
            {
                value = this.Value.ToString("N0", CultureInfo.CurrentCulture);
            }
        }
    }

    public class Test
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Participation
    {
        public Guid ParticipationId { get; set; }
        public string Name { get; set; }
    }

    public class SubParticipation
    {
        public Guid SubParticipationId { get; set; }
        public string Name { get; set; }
    }

    public class Channel
    {
        public Guid ChannelId { get; set; }
        public string Name { get; set; }
    }

    public class SystemUser
    {
        public Guid SystemUserId { get; set; }
        public string FullName { get; set; }
        public byte[] Image { get; set; }
        public string ImageBase64 { get; set; }
        public UserTypes? UserType { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid TeamId { get; set; }
    }

    [DataContract]
    public class Contact
    {
        [DataMember]
        public Guid ContactId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string IdentityNumber { get; set; }
        [DataMember]
        public string PassportNumber { get; set; }
        [DataMember]
        public string EmailAddress1 { get; set; }
        [DataMember]
        public string MobilePhone { get; set; }
        [DataMember]
        public string Telephone { get; set; }
        [DataMember]
        public string AddressDetail { get; set; }
        [DataMember]
        public string OverAddressDetail { get; set; }
        [DataMember]
        public string EntityType { get; set; }
        [DataMember]
        public ContactTypes ContactType { get; set; }
        [DataMember]
        public GenderCodes GenderCode { get; set; }
        [DataMember]
        public FamilyStatusCodes FamilyStatusCode { get; set; }
        [DataMember]
        public EntityReference Participation { get; set; }
        [DataMember]
        public EntityReference SubParticipation { get; set; }
        [DataMember]
        public EntityReference Channel { get; set; }
        [DataMember]
        public EntityReference Country { get; set; }
        [DataMember]
        public EntityReference City { get; set; }
        [DataMember]
        public EntityReference Town { get; set; }
        [DataMember]
        public EntityReference Owner { get; set; }
        [DataMember]
        public EntityReference District { get; set; }
        [DataMember]
        public EntityReference OverCountry { get; set; }
        [DataMember]
        public EntityReference OverCity { get; set; }
        [DataMember]
        public EntityReference Nationality { get; set; }
        [DataMember]
        public List<Activity> Activities { get; set; }
        [DataMember]
        public List<Opportunity> Opportunities { get; set; }
        [DataMember]
        public List<Quote> Quotes { get; set; }
        [DataMember]
        public List<Product> Projects { get; set; }
        [DataMember]
        public JobTitles? JobTitle { get; set; }
        [DataMember]
        public InvestmentType? InvestmentType { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool IsVip { get; set; }
        [DataMember]
        public bool HasCreditProblem { get; set; }
        [DataMember]
        public bool HasPaymentProblem { get; set; }
        [DataMember]
        public bool IsBlackList { get; set; }
        [DataMember]
        public EntityReference RefContact { get; set; }

        [DataMember]
        public EntityReference GuaContact { get; set; }

        [DataMember]
        public EntityReference CrOwner { get; set; }
        [DataMember]
        public EntityReference CustomerSpecialist { get; set; }
        [DataMember]
        public int MarketingGrantValue { get; set; }
        [DataMember]
        public EntityReference GrantUpdateUser { get; set; }
        [DataMember]
        public string SecondryPersonName { get; set; }
        [DataMember]
        public string SecondryPersonLastName { get; set; }
        [DataMember]
        public string SecondryPersonPhone { get; set; }
        [DataMember]
        public bool sendSMS { get; set; }
        [DataMember]
        public bool sendFax { get; set; }
        [DataMember]
        public bool sendEmail { get; set; }
        [DataMember]
        public bool sendMail { get; set; }
        [DataMember]
        public bool contactTelephone { get; set; }
        [DataMember]
        public EntityReference Annotation { get; set; }
        [DataMember]
        public string GuarantorName { get; set; }
        [DataMember]
        public string GuarantorPhone { get; set; }
    }

    public class Account
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string EmailAddress1 { get; set; }
        public string Telephone1 { get; set; }
        public string TaxNumber { get; set; }
        public string AddressDetail { get; set; }
        public string Description { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference TaxOffice { get; set; }
        public EntityReference Country { get; set; }
        public EntityReference City { get; set; }
        public EntityReference Town { get; set; }
        public EntityReference Owner { get; set; }
        public EntityReference District { get; set; }
        public EntityReference CrOwner { get; set; }
    }

    public class Activity
    {
        public Guid ActivityId { get; set; }
        public EntityReference ActivityParty { get; set; }
        public EntityReference Owner { get; set; }
        public EntityReference SalesOffice { get; set; }
        public EntityReference CreatedBy { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string ActivityType { get; set; }
        public string SubjectString { get; set; }
        public string PresentationTypeString { get; set; }
        public string ScheduledStartString { get; set; }
        public string ScheduledEndString { get; set; }
        public string CreatedOnString { get; set; }
        public ActivityStateCodes? StateCode { get; set; }
        public Directions? Direction { get; set; }
        public ObjectTypeCodes? ObjectTypeCode { get; set; }
        public string EntityType { get; set; }
        public int? Subject { get; set; }
        public int? PresentationType { get; set; }
        public int? CallCenterAgent { get; set; }
        public EntityReference ActivityStatus { get; set; }
        public EntityReference ActivityStatusDetail { get; set; }
        public string Note { get; set; }
        public EntityReference Contact { get; set; }
        public ActivityStatusCodes ActivityStatusCode { get; set; }
        public PriorityValues? Priority { get; set; }
        public string PriorityString { get; set; }
        public EntityReference ActivityTopic { get; set; }
        public DateTime? NextCallDate { get; set; }
        public DateTime? NextAppointmentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public string ProjectName { get; set; }
        public string ContactMessage { get; set; }

        public EntityReference Retailer { get; set; }
    }

    public class FinancialAccount
    {
        public Guid FinancialAccountId { get; set; }
        public string Name { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Account { get; set; }
    }

    public class WebForm
    {
        public Guid WebFormId { get; set; }
        public EntityReference Contact { get; set; }
    }

    public class TaxOffice
    {
        public Guid TaxOfficeId { get; set; }
        public string Name { get; set; }
    }

    public class ActivityStatus
    {
        public string Required { get; set; }
        public Guid ActivityStatusId { get; set; }
        public string Name { get; set; }
        public ActivityStatusCodes? ActivityStatusCode { get; set; }
    }

    public class ActivityStatusDetail
    {
        public Guid ActivityStatusDetailId { get; set; }
        public Entity ActivityStatus { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class ActivityTopic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class SalesOffice
    {
        public Guid SalesOfficeId { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
    }

    public class Annotation
    {
        public string Subject { get; set; }
        public string DocumentBody { get; set; }
        public string MimeType { get; set; }
        public Guid AnnotationId { get; set; }
        public string FileName { get; set; }
    }

    public class Quote
    {
        public Guid QuoteId { get; set; }
        public string Name { get; set; }
        public string QuoteNumber { get; set; }
        public int RevisionNumber { get; set; }
        public DateTime? SalesDate { get; set; }
        public DateTime? ContratDate { get; set; }
        public decimal HouseListPrice { get; set; }
        public string HouseListPriceString { get; set; }
        public decimal? HouseSalePrice { get; set; }
        public string HouseSalePriceString { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string DiscountPriceString { get; set; }
        public decimal? HouseDiscountPrice { get; set; }
        public string HouseDiscountPriceString { get; set; }
        public decimal? PrePaymentPrice { get; set; }
        public string PrePaymentPriceString { get; set; }
        public decimal? SumIntervalPrice { get; set; }
        public string SumIntervalPriceString { get; set; }
        public decimal? PaymentPlanDiscountRate { get; set; }
        public string PaymentPlanDiscountRateString { get; set; }
        public decimal? PerSquareMeterPrice { get; set; }
        public string PerSquareMeterPriceString { get; set; }
        public int? PaymentTerm { get; set; }
        public int? InstNumber { get; set; }
        public decimal? SumInstPrice { get; set; }
        public string SumInstPriceString { get; set; }
        public StringMap StateCode { get; set; }
        public StringMap StatusCode { get; set; }
        public EntityReference Opportunity { get; set; }
        public bool PaymentPlan { get; set; }
        public EntityReference Currency { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Owner { get; set; }
        public string SalesDateString { get; set; }
        public string SalesDateStringHour { get; set; }
        public string Status { get; set; }
        public List<Product> Products { get; set; }
        public SalesTypes? SalesType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string ContratDateString { get; set; }
        public string RevisionDescription { get; set; }
        public bool IsRevision { get; set; }
        public string EntityType { get; set; }
        public DateTime? PrePaymentDate { get; set; }
        public string UserComment { get; set; }
        public FallingApprovalTypes? ApprovalType { get; set; }
        public FallingApprovalTypes? FallingApprovalType { get; set; }
        public bool HasSecondCustomer { get; set; }
        public EntityReference SecondCustomer { get; set; }
        public bool IsProjectGyo { get; set; }
        public EntityReference Annotation { get; set; }
        public EntityReference ReferenceContact { get; set; }
        public int? UsageType { get; set; }

        public EntityReference Retailer { get; set; }
    }

    public class Rental
    {
        public Guid? RentalId { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference Quantor { get; set; }
        public EntityReference Account { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Product { get; set; }
        public EntityReference Owner { get; set; }
        public string Name { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string ContractStartDateStr { get; set; }
        public string ContractEndDateStr { get; set; }
        public decimal? ProductAmount { get; set; }
        public string ProductAmountStr { get; set; }
        public decimal? RentalAmount { get; set; }
        public string RentalAmountStr { get; set; }
        public EntityReference Currency { get; set; }
        public EntityReference pCurrency { get; set; }
        public StringMap StateCode { get; set; }
        public StringMap StatusCode { get; set; }
        public StringMap pStatusCode { get; set; }
        public decimal? CommissionAmount { get; set; }
        public string CommissionAmountStr { get; set; }

        public decimal? DepositAmount { get; set; }
        public string DepositAmountStr { get; set; }

        public string GuarantorName { get; set; }
        public string GuarantorPhone { get; set; }
    }



    public class SecondHand
    {
        public Guid? SecondHandId { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference Account { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Product { get; set; }
        public EntityReference Owner { get; set; }
        public string Name { get; set; }
        public decimal? ProductAmount { get; set; }
        public string ProductAmountStr { get; set; }
        public decimal? SecondHandAmount { get; set; }
        public string SecondHandAmountStr { get; set; }
        public EntityReference Currency { get; set; }
        public EntityReference pCurrency { get; set; }
        public StringMap StateCode { get; set; }
        public StringMap StatusCode { get; set; }
        public StringMap pStatusCode { get; set; }
        public decimal? CommissionAmount { get; set; }
        public string CommissionAmountStr { get; set; }
        public decimal? PrePayment { get; set; }
        public string PrePaymentStr { get; set; }
        public DateTime? PrePaymentDate { get; set; }
        public string PrePaymentDateStr { get; set; }
        public int? PrePaymentType { get; set; }
    }

    public class AuthorityDocument
    {
        public string Name { get; set; }
        public Guid? AuthorityDocumentId { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDateStr { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndDateStr { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Product { get; set; }
        public EntityReference Project { get; set; }
        public string BlockName { get; set; }
        public string HomeNumber { get; set; }
    }

    public class Payment
    {
        public Guid PaymentId { get; set; }
        public string Name { get; set; }
        public EntityReference Contact { get; set; }
        public EntityReference Account { get; set; }
        public EntityReference Quote { get; set; }
        public EntityReference Currency { get; set; }
        public EntityReference Owner { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public PaymentTypes? PaymentType { get; set; }
        public PaymentCashTypes? PaymentCashType { get; set; }
        public PaymentStatuses? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string PaymentDateString { get; set; }
        public string CreatedOnString { get; set; }
        public PaymentAccountingTypes? PaymentAccountingTypes { get; set; }
        public EntityReference FinancialAccount { get; set; }
    }

    public class InterestProduct
    {
        public Guid InterestProductId { get; set; }
        public EntityReference PhoneCall { get; set; }
        public EntityReference Appointment { get; set; }
        public string Name { get; set; }
        public Product InterestedProduct { get; set; }
    }

    public class InterestProject
    {
        public Guid InterestProjectId { get; set; }
        public EntityReference PhoneCall { get; set; }
        public EntityReference Appointment { get; set; }
        public string Name { get; set; }
        public EntityReference InterestedProject { get; set; }
    }

    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int? FloorNumber { get; set; }
        public string HomeNumber { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference Block { get; set; }
        public EntityReference GeneralHomeType { get; set; }
        public EntityReference HomeType { get; set; }
        public EntityReference Location { get; set; }
        public EntityReference PriceList { get; set; }
        public StringMap StatusCode { get; set; }
        public StringMap StateCode { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? Price { get; set; }
        public string PriceString { get; set; }
        public decimal? TLPrice { get; set; }
        public string TLPriceString { get; set; }
        public decimal? EuroPrice { get; set; }
        public string EuroPriceString { get; set; }
        public decimal? DollarPrice { get; set; }
        public string DollarPriceString { get; set; }
        public decimal? KdvRatio { get; set; }
        public decimal? TaxofStampRatio { get; set; }
        public EntityReference Currency { get; set; }
        public EntityReference Uom { get; set; }
        public decimal? Brut { get; set; }
        public decimal? Net { get; set; }
        public decimal? BBNetArea { get; set; }
        public decimal? BBGeneralGrossArea { get; set; }
        public int? Aks { get; set; }
        public int? Direction { get; set; }
        public string DirectionName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string UnitPriceString { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool HasTerrace { get; set; }
        public bool HasBalcony { get; set; }
        public bool HasKitchen { get; set; }
        public bool HasAuthority { get; set; }
        public decimal? Terrace { get; set; }
        public decimal? Balcony { get; set; }
        public decimal? Kitchen { get; set; }
        public decimal? Garden { get; set; }

        public decimal? Deck { get; set; }
        public decimal? Court { get; set; }

        public string LicenceNumber { get; set; }
        public bool IsProjectGyo { get; set; }
        public EntityReference Etap { get; set; }
        public EntityReference BlockType { get; set; }
        public decimal? Pool { get; set; }
        public int? GoodsStatus { get; set; }
        public int? UsedRentalSalesStatus { get; set; }
        public decimal? PaymentOfHire { get; set; }
        public string RentalNotes { get; set; }
        public int? RentalMonths { get; set; }
        public Guid? City { get; set; }
        public Guid? County { get; set; }
        public EntityReference PhoneCall { get; set; }
        public EntityReference Appointment { get; set; }
        public StringMap RentalStatus { get; set; }
        public string BlockTypeName { get; set; }
    }

    public class QuoteControlSetting
    {
        public Guid QuoteControlSettingId { get; set; }
        public string Name { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference TransactionCurrency { get; set; }
        public decimal? ConsultantRate { get; set; }
        public int? ConsultantOptionDay { get; set; }
        public decimal? ConsultantUnitPrice { get; set; }
        public decimal? ManagerRate { get; set; }
        public int? ManagerOptionDay { get; set; }
        public decimal? ManagerUnitPrice { get; set; }
        public decimal? DirectorRate { get; set; }
        public int? DirectorOptionDay { get; set; }
        public decimal? DirectorUnitPrice { get; set; }
    }

    public class RentalControlSetting
    {
        public Guid RentalControlSettingId { get; set; }
        public string Name { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference TransactionCurrency { get; set; }
        public decimal? ConsultantRate { get; set; }
        public decimal? DirectorRate { get; set; }
    }

    public class SecondHandControlSetting
    {
        public Guid SecondHandControlSettingId { get; set; }
        public string Name { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference TransactionCurrency { get; set; }
        public decimal? ConsultantRate { get; set; }
        public decimal? DirectorRate { get; set; }
    }

    public class Project
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public bool IsProjectGyo { get; set; }
        public decimal? Ratio { get; set; }
        public DateTime? ExpireDate { get; set; }
    }

    public class Block
    {
        public Guid BlockId { get; set; }
        public string Name { get; set; }
        public string BlockNumber { get; set; }
        public int? FloorNumber { get; set; }
        public EntityReference Project { get; set; }
        public EntityReference Parcel { get; set; }

    }
    public class BlockType
    {
        public Guid BlockTypeId { get; set; }
        public string Name { get; set; }
    }
    public class Etap
    {
        public Guid EtapId { get; set; }
        public string Name { get; set; }
    }

    public class Location
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public EntityReference Project { get; set; }
    }

    public class GeneralHomeType
    {
        public Guid GeneralHomeTypeId { get; set; }
        public string Name { get; set; }
    }

    public class HomeType
    {
        public Guid HomeTypeId { get; set; }
        public EntityReference GeneralHomeType { get; set; }
        public string Name { get; set; }
    }

    public class Country
    {
        public Guid CountryId { get; set; }
        public string Name { get; set; }
    }

    public class City
    {
        public Guid CityId { get; set; }
        public EntityReference Country { get; set; }
        public string Name { get; set; }
    }

    public class Town
    {
        public Guid TownId { get; set; }
        public EntityReference City { get; set; }
        public string Name { get; set; }
    }

    public class Retailer
    {
        public Guid RetailerId { get; set; }
        public string Name { get; set; }
    }

    public class District
    {
        public Guid DistrictId { get; set; }
        public EntityReference Town { get; set; }
        public string Name { get; set; }
    }

    public class Nationality
    {
        public Guid NationalityId { get; set; }
        public string Name { get; set; }
    }

    public class TelephoneNumber
    {
        public Guid PhoneNumberID { get; set; }
        public string TelephoneNo { get; set; }
        public string countryCode { get; set; }
        public string phoneCode { get; set; }
        public string phoneNo { get; set; }
        public bool isFormatOK { get; set; }
    }

    public class Queue
    {
        public Guid QueueId { get; set; }
        public string Name { get; set; }
    }

    public class TransactionCurrency
    {
        public Guid TransactionCurrencyId { get; set; }
        public string Name { get; set; }
    }

    public class Opportunity
    {
        public Guid OpportunityId { get; set; }
        public EntityReference Contact { get; set; } //Bu alan aslında customer alanıdır. Hem Account hem Contact olabilir. Account sonradan eklenmiştir.
        public EntityReference Owner { get; set; }
        public EntityReference ActivityStatusDetail { get; set; }
        public string Status { get; set; }
        public string CreatedOnString { get; set; }
        public StringMap StatusCode { get; set; }
        public StringMap StateCode { get; set; }
        public string OpportunityCode { get; set; }
        public EntityReference ActivityStatus { get; set; }
        public List<Product> OppProducts { get; set; }
        public int? OppProductCount { get; set; }
        public int? ContactTypeCode { get; set; }
        public int? CustomerType { get; set; }
        public bool IsProjectGYO { get; set; }
    }

    public class StringMap
    {
        public string Name { get; set; }
        public int? Value { get; set; }
    }

    public class MsCrmResult
    {
        public Guid CrmId { get; set; }
        public bool Success { get; set; }
        public string Result { get; set; }
    }

    public class MsCrmResult<TReturnObject>
    {
        public Guid CrmId { get; set; }
        public bool Success { get; set; }
        public string Result { get; set; }
        public TReturnObject ReturnObject { get; set; }
    }

    [Serializable]
    public class MsCrmResultObject
    {
        public Guid CrmId { get; set; }
        public bool Success { get; set; }
        public string Result { get; set; }
        public string ExtraInformation { get; set; }
        public object ReturnObject { get; set; }
    }
}
