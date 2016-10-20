using Microsoft.Xrm.Sdk;
using NEF.Library.Entities.CustomEntities;
using CrmEntity = NEF.Library.Entities.CrmEntities;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NEF.WebServices.SalesPortal
{
    [ServiceContract]
    public interface ISalesPortal
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/MakeContactSearchWithOwner")]
        string MakeContactSearchWithOwner(string searchedWord, string ownerId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/MakeContactSearch")]
        string MakeContactSearch(string searchedWord);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetContactDetail")]
        string GetContactDetail(string contactId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCustomerQuotes")]
        string GetCustomerQuotes(string customerId, string systemUserId);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
           , UriTemplate = "/GetCustomerRentals")]
        string GetCustomerRentals(string contactId, string accountId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/GetCustomerSecondHands")]
        string GetCustomerSecondHands(string contactId, string accountId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCustomerOpportunities")]
        string GetCustomerOpportunities(string customerId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCustomerActivities")]
        string GetCustomerActivities(string customerId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetParticipations")]
        string GetParticipations();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSubParticipations")]
        string GetSubParticipations(string participationId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetChannels")]
        string GetChannels();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/UpdateContact")]
        MsCrmResult UpdateContact(Contact contact);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateContact")]
        MsCrmResult CreateContact(Contact contact);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/DeleteAnnotionByContactId")]
        MsCrmResult DeleteAnnotionByContactId(string contactId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/DeleteAnnotionByRentalId")]
        MsCrmResult DeleteAnnotionByRentalId(string rentalId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/IsContactExist")]
        MsCrmResult IsContactExist(Contact contact);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/IsAccountExist")]
        MsCrmResult IsAccountExist(Account account);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSalesConsultants")]
        string GetSalesConsultants();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCustomerLastActivity")]
        string GetCustomerLastActivity(string customerId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateActivity")]
        MsCrmResult CreateActivity(Activity activity, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProjects")]
        string GetProjects(string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProjectBlocks")]
        string GetProjectBlocks(string projectId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
         , UriTemplate = "/GetProjectBlockTypes")]
        string GetProjectBlockTypes(string projectId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
        , UriTemplate = "/GetEtaps")]
        string GetEtaps(string projectId);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProjectLocations")]
        string GetProjectLocations(string projectId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetGeneralHomeTypes")]
        string GetGeneralHomeTypes(string projectId);



        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetGeneralHomeTypesForRent")]
        string GetGeneralHomeTypesForRent();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetHomeTypesByGeneralTypeForRent")]
        string GetHomeTypesByGeneralTypeForRent(string generalHomeTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetHomeTypesByGeneralType")]
        string GetHomeTypesByGeneralType(string generalHomeTypeId, string pId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProductStates")]
        string GetProductStates();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProductStatus")]
        string GetProductStatus(int state);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
           , UriTemplate = "/GetRentalProductStatus")]
        string GetRentalProductStatus();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/MakeHouseSearch")]
        string MakeHouseSearch(Product product, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/MakeRentalHouseSearch")]
        string MakeRentalHouseSearch(Product product, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/MakeHouseSearchForActivity")]
        string MakeHouseSearchForActivity(Product product, Guid phonecallId, Guid appointmentId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/MakeRentalHouseSearchForActivity")]
        string MakeRentalHouseSearchForActivity(Product product, Guid phonecallId, Guid appointmentId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
         , UriTemplate = "/MakeAuthorityDocSearch")]
        string MakeAuthorityDocSearch(Guid? projectId, DateTime? startDate, DateTime? endDate, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCountries")]
        string GetCountries();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCities")]
        string GetCities(string countryId);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetTeamMembers")]
        string GetTeamMembers(string teamId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetTowns")]
        string GetTowns(string cityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetDistricts")]
        string GetDistricts(string townId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetActivityStatuses")]
        string GetActivityStatuses(string subjectId, string userTypeCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetActivityStatusDetails")]
        string GetActivityStatusDetails(string activityStatusId, string userTypeCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSalesOffices")]
        string GetSalesOffices();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/UpdateActivity")]
        MsCrmResult UpdateActivity(Activity activity);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetActivityInterestedHousesForSR")]
        string GetActivityInterestedHousesForSR(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
           , UriTemplate = "/GetActivityInterestedHouses")]
        string GetActivityInterestedHouses(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/RemoveActivityInterestedHouses")]
        MsCrmResult RemoveActivityInterestedHouses(string[] interestedHouseIds);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateActivityInterestedHouses")]
        MsCrmResult CreateActivityInterestedHouses(InterestProduct interestProduct);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserMonthlySalesAmountData")]
        string GetUserMonthlySalesAmountData(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserMonthlySalesQuantityData")]
        string GetUserMonthlySalesQuantityData(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserSalesAmountByProject")]
        string GetUserSalesAmountByProject(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserOpenActivities")]
        string GetUserOpenActivities(string userId, int stateCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserOpenOpportunities")]
        string GetUserOpenOpportunities(string userId, int stateCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserSales")]
        string GetUserSales(string userId, int stateCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserCustomers")]
        string GetUserCustomers(string userId, int stateCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetOpportunityInfo")]
        string GetOpportunityInfo(string oppId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProductInfo")]
        string GetProductInfo(string productId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSystemUserInfo")]
        string GetSystemUserInfo(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateQuote")]
        MsCrmResult CreateQuote(EntityReference customer, EntityReference owner, string[] interestedHouses);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
           , UriTemplate = "/CreateRental")]
        MsCrmResult CreateRental(EntityReference account, EntityReference contact, EntityReference owner, string[] interestedHouses);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/CreateSecondHand")]
        MsCrmResult CreateSecondHand(EntityReference account, EntityReference contact, EntityReference owner, string[] interestedHouses);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateSalesRetail")]
        MsCrmResult CreateSalesRetail(EntityReference customer, EntityReference owner, string[] interestedHouses, string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetQuoteDetail")]
        string GetQuoteDetail(string quoteId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetRentalDetail")]
        string GetRentalDetail(string rentalid);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSecondHandDetail")]
        string GetSecondHandDetail(string secondhandid);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/UpdateQuote")]
        MsCrmResult UpdateQuote(Quote quote);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/UpdateRental")]
        MsCrmResult UpdateRental(Rental rental);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
         , UriTemplate = "/UpdateSecondHand")]
        MsCrmResult UpdateSecondHand(SecondHand secondhand);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateOrUpdatePayment")]
        MsCrmResult CreateOrUpdatePayment(Payment payment);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetQuotePrePayment")]
        string GetQuotePrePayment(string quoteId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/SendQuoteToApproval")]
        MsCrmResult SendQuoteToApproval(string quoteId, string comment);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/SendRentalToApproval")]
        MsCrmResult SendRentalToApproval(string rentalId, string comment);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
           , UriTemplate = "/SendSecondHandToApproval")]
        MsCrmResult SendSecondHandToApproval(string secondHandId, string comment);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/ConfirmQuote")]
        MsCrmResult ConfirmQuote(string quoteId, string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/RefuseQuote")]
        MsCrmResult RefuseQuote(string quoteId, string userId);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
           , UriTemplate = "/ConfirmRental")]
        MsCrmResult ConfirmRental(string rentalId, string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/RefuseRental")]
        MsCrmResult RefuseRental(string rentalId, string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
         , UriTemplate = "/ConfirmSecondHandKaporaTaken")]
        MsCrmResult ConfirmSecondHandKaporaTaken(string secondHandId, string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
        , UriTemplate = "/ConfirmSecondHand")]
        MsCrmResult ConfirmSecondHand(string secondHandId, string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/RefuseSecondHand")]
        MsCrmResult RefuseSecondHand(string secondHandId, string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCurrencies")]
        string GetCurrencies();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CustomerHasCallCenterCall")]
        MsCrmResult CustomerHasCallCenterCall(string customerId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCallCenterPhoneCalls")]
        string GetCallCenterPhoneCalls();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCallCenterPhoneCallsTopkapi")]
        string GetCallCenterPhoneCallsTopkapi();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetNationalities")]
        string GetNationalities();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateOrUpdateAccount")]
        MsCrmResult CreateOrUpdateAccount(Account account);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateAuthorityDocument")]
        MsCrmResult CreateAuthorityDocument(AuthorityDocument document);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetAccountDetail")]
        string GetAccountDetail(string accountId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetTaxOffices")]
        string GetTaxOffices();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetOldFeeds")]
        List<UserFeed> GetOldFeeds(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetNotPostedFeeds")]
        List<UserFeed> GetNotPostedFeeds();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/UpdateFeedAsPosted")]
        MsCrmResult UpdateFeedAsPosted(string feedId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/UpdateFeedAsRead")]
        MsCrmResult UpdateFeedAsRead(string feedId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetIpAddress")]
        string GetIpAddress();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CheckEntityExists")]
        MsCrmResult CheckEntityExists(string entityId, string entityName, string attributeName);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CheckSaleCustomerIdentityAndAddress")]
        MsCrmResult CheckSaleCustomerIdentityAndAddress(string quoteId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserHeaderInfo")]
        UserHeaderInfo GetUserHeaderInfo(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetActivityInfo")]
        string GetActivityInfo(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetPhoneCallActivityInfo")]
        string GetPhoneCallActivityInfo(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetAppointmentActivityInfo")]
        string GetAppointmentActivityInfo(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetActivityTopics")]
        string GetActivityTopics(string userTypeCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CheckActivityOwnership")]
        MsCrmResult CheckActivityOwnership(string userId, string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserAllIntrestedHouses")]
        string GetUserAllIntrestedHouses(string customerId, string systemUserId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserAllIntrestedEmptyHouses")]
        string GetUserAllIntrestedEmptyHouses(string customerId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/ExportPhoneCall")]
        MsCrmResult ExportPhoneCall(List<Activity> phoneCallList);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetUserSalesQuantityByProject")]
        string GetUserSalesQuantityByProject(string userId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/GetContactAttachment")]
        string GetContactAttachment(string contactId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
          , UriTemplate = "/GetRentalAttachment")]
        string GetRentalAttachment(string rentalId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
         , UriTemplate = "/GetDisplayLocationAttachment")]
        string GetDisplayLocationAttachment(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCallCenterAgents")]
        string GetCallCenterAgents();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetQuoteStatuses")]
        string GetQuoteStatuses();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProjectsForActivity")]
        string GetProjectsForActivity(Guid phonecallId, Guid appointmentId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetActivityInterestedProjects")]
        string GetActivityInterestedProjects(string activityId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateInterestedProject")]
        MsCrmResult CreateInterestedProject(string[] interestedProjects, Guid phonecallId, Guid appointmentId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/RemoveActivityInterestedProjects")]
        MsCrmResult RemoveActivityInterestedProjects(string[] interestedProjectIds);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetCustomerRelationSpecialists")]
        string GetCustomerRelationSpecialists();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetProductOptionInfo")]
        string GetProductOptionInfo(string productId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetOpportunityLostStatuses")]
        string GetOpportunityLostStatuses();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CloseOpportunityAsLost")]
        MsCrmResult CloseOpportunityAsLost(Guid opportunityId, int statusCode);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CheckCustomerApartmentOwner")]
        MsCrmResult CheckCustomerApartmentOwner(string customerId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetMonthlySalesAmountData")]
        string GetMonthlySalesAmountData();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetMonthlySalesQuantityData")]
        string GetMonthlySalesQuantityData();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSalesAmountByProject")]
        string GetSalesAmountByProject();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSalesQuantityByProject")]
        string GetSalesQuantityByProject();


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetMonthlySalesGeneralAmountData")]
        string GetMonthlySalesGeneralAmountData();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetMonthlySalesGeneralQuantityData")]
        string GetMonthlySalesGeneralQuantityData();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetSalesGeneralAmountByProject")]
        string GetSalesGeneralAmountByProject();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetGeneralSalesQuantityByProject")]
        string GetGeneralSalesQuantityByProject();



        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetAllHeaderInfo")]
        UserHeaderInfo GetAllHeaderInfo();


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/UpdateProductRentInfo")]
        MsCrmResult UpdateProductRentInfo(HomeRentOption rentInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/CreateProductForRent")]
        MsCrmResult CreateProductForRent(Product product);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/ConfirmPointUsage")]
        MsCrmResult ConfirmPointUsage(string loyaltyPointId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/RefusePointUsage")]
        MsCrmResult RefusePointUsage(string loyaltyPointId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetContactPointSummary")]
        MsCrmResult<LoyaltyPointSummary> GetContactPointSummary(string contactId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "/GetLoyaltyPoint")]
        MsCrmResult<CrmEntity.LoyaltyPoint> GetLoyaltyPoint(string loyaltyPointId);
    }
}
