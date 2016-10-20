using Microsoft.Crm.Sdk.Messages;
using Microsoft.Office.Interop.Excel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using CrmEntity = NEF.Library.Entities.CrmEntities;
using NEF.Library.Entities.CustomEntities;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace NEF.WebServices.SalesPortal
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SalesPortal : ISalesPortal
    {
        SqlDataAccess sda = null;
        IOrganizationService service = null;

        public SalesPortal()
        {
            Initializer.Init();
        }

        public string MakeContactSearch(string searchedWord)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(searchedWord))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ContactHelper.ContactSearch(searchedWord, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen aranacak müşteri bilgisini giriniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string MakeContactSearchWithOwner(string searchedWord, string ownerId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(searchedWord))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ContactHelper.ContactSearch(searchedWord, ownerId, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen aranacak müşteri bilgisini giriniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetContactDetail(string contactId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(contactId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ContactHelper.GetContactDetail(new Guid(contactId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kişi seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCustomerQuotes(string customerId, string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                //IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                if (!string.IsNullOrEmpty(customerId))
                {
                    SystemUser userDetail = SystemUserHelper.GetSystemUserInfo(new Guid(systemUserId), sda);
                    if (userDetail.SystemUserId != null)
                    {
                        if (userDetail.UserType != UserTypes.IsGyoSatisMuduru && userDetail.UserType != UserTypes.IsGyoSatisDanismani)
                        {
                            returnValue = QuoteHelper.GetCustomerQuotes(new Guid(customerId), sda);
                        }
                        else
                        {
                            returnValue = QuoteHelper.GetCustomerQuotes(new Guid(customerId), sda);
                            if (returnValue.Success)
                            {
                                List<Quote> lstQuotes = (List<Quote>)returnValue.ReturnObject;

                                lstQuotes = (from a in lstQuotes
                                             where
                                             a.IsProjectGyo == true
                                             select a).ToList();

                                returnValue.ReturnObject = lstQuotes;
                            }

                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşteri seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCustomerRentals(string contactId, string accountId, string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                //IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                if (!string.IsNullOrEmpty(contactId))
                {
                    //SystemUser userDetail = SystemUserHelper.GetSystemUserInfo(new Guid(systemUserId), sda);
                    returnValue = RentalHelper.GetCustomerRentals(new Guid(contactId), null, sda);
                }
                else if (!string.IsNullOrEmpty(accountId))
                {
                    returnValue = RentalHelper.GetCustomerRentals(null, new Guid(accountId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşteri seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCustomerSecondHands(string contactId, string accountId, string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(contactId))
                {
                    returnValue = SecondHandHelper.GetCustomerSecondHands(new Guid(contactId), null, sda);
                }
                else if (!string.IsNullOrEmpty(accountId))
                {
                    returnValue = SecondHandHelper.GetCustomerSecondHands(null, new Guid(accountId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşteri seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCustomerOpportunities(string customerId, string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                //IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                if (!string.IsNullOrEmpty(customerId))
                {
                    SystemUser userDetail = SystemUserHelper.GetSystemUserInfo(new Guid(systemUserId), sda);
                    if (userDetail.SystemUserId != null)
                    {
                        if (userDetail.UserType != UserTypes.IsGyoSatisMuduru && userDetail.UserType != UserTypes.IsGyoSatisDanismani)
                        {
                            returnValue = OpportunityHelper.GetCustomerOpportunities(new Guid(customerId), sda);
                        }
                        else
                        {
                            returnValue = OpportunityHelper.GetCustomerOpportunities(new Guid(customerId), sda);
                            if (returnValue.Success)
                            {
                                List<Opportunity> lstOpp = (List<Opportunity>)returnValue.ReturnObject;

                                lstOpp = (from a in lstOpp
                                          where
                                          a.IsProjectGYO == true
                                          select a).ToList();

                                returnValue.ReturnObject = lstOpp;
                            }

                        }

                    }

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşteri seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCustomerActivities(string customerId, string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                //IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                if (!string.IsNullOrEmpty(customerId))
                {
                    returnValue = ActivityHelper.GetCustomerActivities(new Guid(customerId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşteri seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetParticipations()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ParticipationHelper.GetParticipations(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSubParticipations(string participationId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(participationId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ParticipationHelper.GetSubParticipations(new Guid(participationId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Önce kaynağı seçmelisiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetChannels()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ParticipationHelper.GetChannels(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult UpdateProductRentInfo(HomeRentOption rentInfo)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (rentInfo != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = ProductHelper.UpdateProductRentInfo(rentInfo, service);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata oluştu";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult CreateProductForRent(Product product)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (product != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = ProductHelper.CreateProductForRent(product, service);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata oluştu";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult UpdateContact(Contact contact)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (contact != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = ContactHelper.CreateOrUpdateContact(contact, service);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public MsCrmResult CreateContact(Contact contact)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (contact != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = ContactHelper.CreateOrUpdateContact(contact, service);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public MsCrmResult DeleteAnnotionByContactId(string contactId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(contactId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    returnValue = ContactHelper.DeleteAnnotionByContactId(contactId, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public MsCrmResult DeleteAnnotionByRentalId(string rentalId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(rentalId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    returnValue = ContactHelper.DeleteAnnotionByRentalId(rentalId, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public MsCrmResult IsContactExist(Contact contact)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (contact != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    returnValue = ContactHelper.IsContactExist(contact, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public MsCrmResult IsAccountExist(Account account)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (account != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    returnValue = ContactHelper.IsAccountExist(account, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public string GetSalesConsultants()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                List<UserTypes> userTypes = new List<UserTypes>();
                userTypes.Add(UserTypes.SatisDanismani);
                userTypes.Add(UserTypes.IsGyoSatisDanismani);
                returnValue = SystemUserHelper.GetSalesConsultants(userTypes, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCustomerLastActivity(string customerId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(customerId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ActivityHelper.GetCustomerLastActivity(new Guid(customerId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CreateActivity(Activity activity, string systemUserId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (activity != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);
                    IOrganizationService serviceAdmin = MSCRM.GetOrgService(true);

                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    if (activity.ObjectTypeCode == ObjectTypeCodes.Telefon)
                    {
                        try
                        {
                            SystemUser sInfo = SystemUserHelper.GetSystemUserInfo(new Guid(systemUserId), sda);

                            if (sInfo.UserType == UserTypes.IsGyoSatisDanismani || sInfo.UserType == UserTypes.IsGyoSatisMuduru || sInfo.UserType == UserTypes.IsGyoCallCenter)
                            {

                                var grantAccessRequest = new GrantAccessRequest
                                {
                                    PrincipalAccess = new PrincipalAccess
                                    {
                                        AccessMask = AccessRights.ReadAccess,
                                        Principal = new EntityReference("systemuser", new Guid(systemUserId))
                                    },
                                    Target = activity.ActivityParty
                                };

                                serviceAdmin.Execute(grantAccessRequest);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        returnValue = ActivityHelper.CreatePhoneCall(activity, service);
                    }
                    else
                    {
                        returnValue = ActivityHelper.CreateAppointment(activity, service);

                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public string GetProjects(string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);
                OrganizationServiceContext orgServiceContext = new OrganizationServiceContext(service);

                returnValue = ProductHelper.GetProjects(orgServiceContext);

                SystemUser userDetail = SystemUserHelper.GetSystemUserInfo(new Guid(systemUserId), sda);
                if (userDetail.SystemUserId != null)
                {

                    if (userDetail.UserType == UserTypes.IsGyoSatisMuduru || userDetail.UserType == UserTypes.IsGyoSatisDanismani)
                    {
                        List<Project> lstProcs = (List<Project>)returnValue.ReturnObject;

                        lstProcs = (from a in lstProcs
                                    where
                                    a.IsProjectGyo == true
                                    select a).ToList();

                        returnValue.ReturnObject = lstProcs;
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProjectBlocks(string projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(projectId) && projectId != "-1")
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ProductHelper.GetProjectBlocks(new Guid(projectId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir proje seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProjectBlockTypes(string projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(projectId) && projectId != "-1")
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    returnValue = ProductHelper.GetBlockTypes(sda, new Guid(projectId));
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir proje seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetEtaps(string projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(projectId) && projectId != "-1")
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ProductHelper.GetEtaps(sda, new Guid(projectId));
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir proje seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProjectLocations(string projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(projectId) && projectId != "-1")
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ProductHelper.GetProjectLocations(new Guid(projectId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir proje seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetGeneralHomeTypes(string projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ProductHelper.GetGeneralHomeTypes(new Guid(projectId), sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetGeneralHomeTypesForRent()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ProductHelper.GetGeneralHomeTypesForRent(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetHomeTypesByGeneralType(string generalHomeTypeId, string pId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(generalHomeTypeId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ProductHelper.GetHomeTypesByGeneralTypeAndProject(new Guid(generalHomeTypeId), new Guid(pId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir genel daire tipi seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetHomeTypesByGeneralTypeForRent(string generalHomeTypeId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(generalHomeTypeId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ProductHelper.GetHomeTypesByGeneralTypeForRent(new Guid(generalHomeTypeId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir genel daire tipi seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProductStates()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetProductActiveStateCodes(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProductStatus(int state)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetProductActiveStatusCodes(state, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetRentalProductStatus()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetRentalProductActiveStatusCodes(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string MakeHouseSearch(Product product, string systemUserId)
        {
            //Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                returnValue = ProductHelper.MakeHouseSearch(product, service, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string MakeRentalHouseSearch(Product product, string systemUserId)
        {
            //Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                returnValue = ProductHelper.MakeRentalHouseSearch(product, service, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string MakeHouseSearchForActivity(Product product, Guid phonecallId, Guid appointmentId, string systemUserId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                returnValue = ProductHelper.MakeHouseSearchForActivity(product, phonecallId, appointmentId, service, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string MakeRentalHouseSearchForActivity(Product product, Guid phonecallId, Guid appointmentId, string systemUserId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                returnValue = ProductHelper.MakeRentalHouseSearchForActivity(product, phonecallId, appointmentId, service, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string MakeAuthorityDocSearch(Guid? projectId, DateTime? startDate, DateTime? endDate, string systemUserId)
        {
            //Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                IOrganizationService service = MSCRM.GetOrgService(true, systemUserId);

                returnValue = SecondHandHelper.MakeAuthorityDocSearch(projectId, startDate, endDate, service, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCountries()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = AddressHelper.GetCountries(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCities(string countryId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(countryId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = AddressHelper.GetCities(new Guid(countryId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen ülke seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetTowns(string cityId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(cityId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = AddressHelper.GetTowns(new Guid(cityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen şehir seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetDistricts(string townId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(townId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = AddressHelper.GetDistricts(new Guid(townId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen ilçe seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetTeamMembers(string teamId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(teamId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ContactHelper.GetTeamMembers(new Guid(teamId), sda);
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetActivityStatuses(string subjectId, string userTypeCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(subjectId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ActivityHelper.GetActivityStatuses(new Guid(subjectId), userTypeCode, sda);
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetActivityStatusDetails(string activityStatusId, string userTypeCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(activityStatusId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = ActivityHelper.GetActivityStatusDetails(new Guid(activityStatusId), userTypeCode, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen bir görüşme sonucu seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetActivityTopics(string userTypeCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ActivityHelper.GetActivityTopics(userTypeCode, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSalesOffices()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ActivityHelper.GetSalesOffices(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult UpdateActivity(Activity activity)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (activity != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);

                    if (activity.ObjectTypeCode == ObjectTypeCodes.Telefon)
                    {
                        returnValue = ActivityHelper.UpdatePhoneCall(activity, service);
                    }
                    else
                    {
                        returnValue = ActivityHelper.UpdateAppointment(activity, service);
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public string GetActivityInterestedHouses(string activityId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(activityId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = InterestProductHelper.GetActivityInterestedProjects(new Guid(activityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetActivityInterestedHousesForSR(string activityId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(activityId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = InterestProductHelper.GetActivityInterestedProjectsForSR(new Guid(activityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }



        public MsCrmResult RemoveActivityInterestedHouses(string[] interestedHouseIds)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (interestedHouseIds != null && interestedHouseIds.Length > 0)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    for (int i = 0; i < interestedHouseIds.Length; i++)
                    {
                        InterestProductHelper.RemoveInterestedHouse(new Guid(interestedHouseIds[i]), service);
                    }

                    returnValue.Success = true;
                    returnValue.Result = "İlgilendiği konutlar başarıyla kaldırıldı.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public MsCrmResult CreateActivityInterestedHouses(InterestProduct interestProduct)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (interestProduct != null)
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    IOrganizationService service = MSCRM.GetOrgService(true);

                    Guid activityId = interestProduct.PhoneCall != null ? interestProduct.PhoneCall.Id : interestProduct.Appointment.Id;

                    MsCrmResult hasAdded = InterestProductHelper.InterestHouseHasSameProduct(interestProduct.InterestedProduct.ProductId, activityId, sda);
                    if (hasAdded.Success) // Daha önceden eklenmediyse
                    {
                        returnValue = InterestProductHelper.CreateInterestHouse(interestProduct, service);
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Konut aynı aktiviteye yalnızca bir kez eklenebilir!";
                    }


                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            return returnValue;
        }

        public string GetUserMonthlySalesAmountData(string userId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;
                DateTime before12Month = DateTime.Now.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);
                MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotesBySalesDateRange(new Guid(userId), startOfMonth.ToUniversalTime(), DateTime.UtcNow, sda);

                if (resultQuotes.Success)
                {
                    List<Quote> lstQuotes = new List<Quote>();

                    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                    List<string[]> lstTest = new List<string[]>();



                    string[] months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

                    //for (int i = 0; i < (months.Length - 1); i++)
                    for (int i = 0; i < 13; i++)
                    {
                        var refDate = before12Month.AddMonths(i);

                        decimal salesAmount = (from bs in lstQuotes
                                               where
                                               bs.SalesDate != null
                                               &&
                                               (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı
                                               || bs.StatusCode.Value == (int)QuoteStatus.KaporaAlındı || bs.StatusCode.Value == (int)QuoteStatus.SozlesmeHazirlandi)
                                               &&
                                               ((DateTime)bs.SalesDate).Year == refDate.Year
                                               &&
                                               ((DateTime)bs.SalesDate).Month == refDate.Month
                                               select
                                                 bs.HouseListPrice
                                   ).Sum();

                        lstTest.Add(new string[] { months[refDate.Month - 1] + refDate.ToString("yy"), Convert.ToInt32(salesAmount).ToString() });

                    }

                    returnValue.ReturnObject = lstTest.ToList();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserMonthlySalesQuantityData(string userId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;
                DateTime before12Month = DateTime.Now.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);
                MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotesBySalesDateRange(new Guid(userId), startOfMonth.ToUniversalTime(), DateTime.UtcNow, sda);

                if (resultQuotes.Success)
                {
                    List<Quote> lstQuotes = new List<Quote>();

                    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                    List<string[]> lstTest = new List<string[]>();



                    string[] months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

                    //for (int i = 0; i < (months.Length - 1); i++)
                    for (int i = 0; i < 13; i++)
                    {
                        var refDate = before12Month.AddMonths(i);

                        decimal salesAmount = (from bs in lstQuotes
                                               where
                                               bs.SalesDate != null
                                               &&
                                              (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı
                                               || bs.StatusCode.Value == (int)QuoteStatus.KaporaAlındı || bs.StatusCode.Value == (int)QuoteStatus.SozlesmeHazirlandi)
                                               &&
                                               ((DateTime)bs.SalesDate).Year == refDate.Year
                                               &&
                                               ((DateTime)bs.SalesDate).Month == refDate.Month
                                               select
                                                 bs.HouseListPrice
                                   ).Count();

                        lstTest.Add(new string[] { months[refDate.Month - 1] + refDate.ToString("yy"), Convert.ToInt32(salesAmount).ToString() });

                    }

                    returnValue.ReturnObject = lstTest.ToList();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserSalesAmountByProject(string userId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;

                DateTime before12Month = DateTime.Now.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                List<ChartKeyValues> lstData = QuoteHelper.GetUserSalesAmountByProjectData(startOfMonth, DateTime.Now.ToUniversalTime(), new Guid(userId), sda);

                returnValue.ReturnObject = lstData;
                returnValue.Success = true;

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);

                //if (resultQuotes.Success)
                //{
                //    List<Quote> lstQuotes = new List<Quote>();

                //    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                //    List<ChartKeyValues> lstTest = (from bs in lstQuotes
                //                                    where
                //                                    bs.SalesDate != null
                //                                    &&
                //                                    (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                //                                    &&
                //                                    ((DateTime)bs.SalesDate).Year == thisYear
                //                                    &&
                //                                    bs.Products != null && bs.Products.Count > 0
                //                                    group bs by bs.Products[0].Project.Name into g
                //                                    orderby g.Sum(x => x.HouseListPrice)
                //                                    select new ChartKeyValues
                //                                    {
                //                                        Name = g.First().Products[0].Project.Name,
                //                                        Value = Convert.ToInt32(g.Sum(x => x.HouseListPrice)),
                //                                        ValueText = (g.Sum(x => x.HouseListPrice)).ToString("N0", CultureInfo.CurrentCulture)
                //                                    }).ToList();

                //    returnValue.ReturnObject = lstTest;
                //}
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserSalesQuantityByProject(string userId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;

                DateTime before12Month = DateTime.Now.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                List<ChartKeyValues> lstData = QuoteHelper.GetUserSalesQuantityByProjectData(startOfMonth, DateTime.Now.ToUniversalTime(), new Guid(userId), sda);

                returnValue.ReturnObject = lstData;
                returnValue.Success = true;

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);

                //if (resultQuotes.Success)
                //{
                //    List<Quote> lstQuotes = new List<Quote>();

                //    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                //    List<ChartKeyValues> lstTest = (from bs in lstQuotes
                //                                    where
                //                                    bs.SalesDate != null
                //                                    &&
                //                                    (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                //                                    &&
                //                                    ((DateTime)bs.SalesDate).Year == thisYear
                //                                    &&
                //                                    bs.Products != null && bs.Products.Count > 0
                //                                    group bs by bs.Products[0].Project.Name into g
                //                                    orderby g.Sum(x => x.HouseListPrice)
                //                                    select new ChartKeyValues
                //                                    {
                //                                        Name = g.First().Products[0].Project.Name,
                //                                        Value = Convert.ToInt32(g.Count()),
                //                                        ValueText = (g.Count().ToString("N0", CultureInfo.CurrentCulture))
                //                                    }).ToList();

                //    returnValue.ReturnObject = lstTest;
                //}
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserOpenActivities(string userId, int stateCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;

                returnValue = ActivityHelper.GetUserActivities(new Guid(userId), sda);

                if (returnValue.Success)
                {
                    List<Activity> lstActivities = (List<Activity>)returnValue.ReturnObject;

                    if (stateCode == (int)ActivityStateCodes.Acik)
                    {
                        lstActivities = (from a in lstActivities
                                         where
                                         a.StateCode == ActivityStateCodes.Acik
                                         ||
                                         a.StateCode == ActivityStateCodes.Zamanlanmis
                                         select a).ToList();
                    }
                    else if (stateCode == (int)ActivityStateCodes.Tamamlandi)
                    {
                        lstActivities = (from a in lstActivities
                                         where
                                         a.StateCode == ActivityStateCodes.Tamamlandi
                                         select a).ToList();
                    }
                    else
                    {
                        lstActivities = (from a in lstActivities
                                         where
                                         a.StateCode != ActivityStateCodes.IptalEdildi
                                         select a).ToList();
                    }


                    returnValue.ReturnObject = lstActivities;
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserOpenOpportunities(string userId, int stateCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = OpportunityHelper.GetUserOpportunities(new Guid(userId), sda);

                if (returnValue.Success)
                {
                    List<Opportunity> lstOpportunities = (List<Opportunity>)returnValue.ReturnObject;

                    if (stateCode != -1)
                    {
                        lstOpportunities = (from a in lstOpportunities
                                            where
                                            a.StateCode.Value == stateCode //Açık
                                            select a).ToList();
                    }
                    else //Hepsi
                    {
                        lstOpportunities = (from a in lstOpportunities
                                            select a).ToList();
                    }


                    returnValue.ReturnObject = lstOpportunities;
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserSales(string userId, int stateCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            int thisYear = DateTime.Now.Year;

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);

                if (returnValue.Success)
                {
                    List<Quote> lstQuotes = (List<Quote>)returnValue.ReturnObject;

                    if (stateCode == -1)
                    {
                        lstQuotes = (from a in lstQuotes
                                     where
                                     a.Products != null && a.Products.Count > 0
                                     &&
                                     ((DateTime)a.CreatedOn).Year == thisYear
                                     &&
                                     (a.StatusCode.Value == (int)QuoteStatus.DevamEdiyor || a.StatusCode.Value == (int)QuoteStatus.Onaylandı)
                                     select a).ToList();

                        //lstQuotes = (from a in lstQuotes
                        //             where
                        //             (a.StatusCode.Value == (int)QuoteStatus.DevamEdiyor || a.StatusCode.Value == (int)QuoteStatus.Onaylandı)
                        //             select a).ToList();
                    }
                    else
                    {
                        lstQuotes = (from a in lstQuotes
                                     where
                                     a.Products != null && a.Products.Count > 0
                                     &&
                                     ((DateTime)a.CreatedOn).Year == thisYear
                                     &&
                                     a.StatusCode.Value == stateCode
                                     select a).ToList();

                        //lstQuotes = (from a in lstQuotes
                        //             where
                        //             a.StatusCode.Value == stateCode
                        //             select a).ToList();
                    }


                    returnValue.ReturnObject = lstQuotes;
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserCustomers(string userId, int stateCode)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ContactHelper.GetUserCustomers(new Guid(userId), sda);

                if (returnValue.Success)
                {
                    List<Contact> lstContacts = (List<Contact>)returnValue.ReturnObject;

                    if (stateCode == -1)
                    {
                        lstContacts = (from a in lstContacts
                                       select a).ToList();
                    }
                    else
                    {
                        lstContacts = (from a in lstContacts
                                       where
                                           a.ContactType == (ContactTypes)stateCode
                                       select a).ToList();
                    }


                    returnValue.ReturnObject = lstContacts;
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            string json = jSer.Serialize(returnValue);

            return json;
        }

        /// <summary>
        /// Emrah Eroğlu
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="owner"></param>
        /// <param name="interestedHouses"></param>
        /// <returns></returns>
        public MsCrmResult CreateRental(EntityReference account, EntityReference contact, EntityReference owner, string[] interestedHouses)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (contact != null || account != null)
                {
                    if (interestedHouses != null && interestedHouses.Length > 0)
                    {
                        IOrganizationService service = MSCRM.GetOrgService(true);
                        sda = new SqlDataAccess();
                        sda.openConnection(Globals.ConnectionString);

                        //for (int i = 0; i < interestedHouses.Length; i++)
                        //{
                        //    MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                        //    if (interestedResult.Success && interestedResult.ReturnObject != null)
                        //    {
                        //        Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                        //        if (_proc.UsedRentalSalesStatus != 2 && _proc.UsedRentalSalesStatus != 3)
                        //        {
                        //            returnValue.Success = false;
                        //            returnValue.Result = "Lütfen Kiralama kayıdı seçiniz!";
                        //        }
                        //    }
                        //}
                        for (int i = 0; i < interestedHouses.Length; i++)
                        {
                            MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                            if (interestedResult.Success && interestedResult.ReturnObject != null)
                            {
                                Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                                if (_proc.StatusCode.Value == 1)
                                {
                                    Rental _rental = new Rental();
                                    if (contact != null)
                                    {
                                        _rental.Contact = contact;
                                    }
                                    if (account != null)
                                    {
                                        _rental.Account = account;
                                    }
                                    _rental.Name = _proc.Name;
                                    _rental.Owner = owner;
                                    _rental.Product = new EntityReference("product", _proc.ProductId);
                                    returnValue = RentalHelper.UpdateOrCreateRental(_rental, service, sda);
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen konut seçiniz!";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult CreateSecondHand(EntityReference account, EntityReference contact, EntityReference owner, string[] interestedHouses)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (contact != null || account != null)
                {
                    if (interestedHouses != null && interestedHouses.Length > 0)
                    {
                        IOrganizationService service = MSCRM.GetOrgService(true);
                        sda = new SqlDataAccess();
                        sda.openConnection(Globals.ConnectionString);

                        //for (int i = 0; i < interestedHouses.Length; i++)
                        //{
                        //    MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                        //    if (interestedResult.Success && interestedResult.ReturnObject != null)
                        //    {
                        //        Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                        //        if (_proc.UsedRentalSalesStatus != 5 && _proc.UsedRentalSalesStatus != 6)
                        //        {
                        //            returnValue.Success = false;
                        //            returnValue.Result = "Lütfen 2.El Satış kayıdı seçiniz!";
                        //            return returnValue;
                        //        }
                        //    }
                        //}
                        for (int i = 0; i < interestedHouses.Length; i++)
                        {
                            MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                            if (interestedResult.Success && interestedResult.ReturnObject != null)
                            {
                                Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                                if (_proc.StatusCode.Value == 1)
                                {
                                    SecondHand _secondHand = new SecondHand();
                                    if (contact != null)
                                    {
                                        _secondHand.Contact = contact;
                                    }
                                    if (account != null)
                                    {
                                        _secondHand.Account = account;
                                    }
                                    _secondHand.Name = _proc.Name;
                                    _secondHand.Owner = owner;
                                    _secondHand.Product = new EntityReference("product", _proc.ProductId);
                                    returnValue = SecondHandHelper.UpdateOrCreateSecondHand(_secondHand, service, sda);
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen konut seçiniz!";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult CreateSalesRetail(EntityReference customer, EntityReference owner, string[] interestedHouses, string activityId)
        {
            //Quote alıyordum parametre olarak şimdilik böyle yaptım
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (customer != null)
                {
                    if (interestedHouses != null && interestedHouses.Length > 0)
                    {
                        IOrganizationService service = MSCRM.GetOrgService(true);

                        sda = new SqlDataAccess();
                        sda.openConnection(Globals.ConnectionString);

                        for (int i = 0; i < interestedHouses.Length; i++)
                        {
                            MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                            if (interestedResult.Success && interestedResult.ReturnObject != null)
                            {
                                Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                                if (_proc.StatusCode.Value == 1)
                                {
                                    Quote _quote = new Quote();
                                    _quote.Contact = customer;
                                    _quote.Owner = owner;

                                    _quote.Products = new List<Product>();
                                    _quote.Products.Add(_proc);

                                    MsCrmResultObject retailerResult = ActivityHelper.GetRetailUserFromActivity(activityId, sda);
                                    _quote.Retailer = new EntityReference("new_retailer", retailerResult.CrmId);
                                    returnValue = QuoteHelper.UpdateOrCreateQuote(_quote, service, sda);

                                    if (returnValue.Success)
                                    {
                                        MsCrmResult quoteDetailResult = QuoteHelper.CreateQuoteDetail(returnValue.CrmId, _proc, service);

                                        returnValue.Success = quoteDetailResult.Success;
                                        returnValue.Result = quoteDetailResult.Result;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen konut seçiniz!";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public MsCrmResult CreateQuote(EntityReference customer, EntityReference owner, string[] interestedHouses)
        {
            //Quote alıyordum parametre olarak şimdilik böyle yaptım
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (customer != null)
                {
                    if (interestedHouses != null && interestedHouses.Length > 0)
                    {
                        IOrganizationService service = MSCRM.GetOrgService(true);

                        sda = new SqlDataAccess();
                        sda.openConnection(Globals.ConnectionString);

                        for (int i = 0; i < interestedHouses.Length; i++)
                        {
                            MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                            if (interestedResult.Success && interestedResult.ReturnObject != null)
                            {
                                Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                                if (_proc.StatusCode.Value == 1)
                                {
                                    Quote _quote = new Quote();
                                    _quote.Contact = customer;
                                    _quote.Owner = owner;

                                    _quote.Products = new List<Product>();
                                    _quote.Products.Add(_proc);
                                    returnValue = QuoteHelper.UpdateOrCreateQuote(_quote, service, sda);

                                    if (returnValue.Success)
                                    {
                                        MsCrmResult quoteDetailResult = QuoteHelper.CreateQuoteDetail(returnValue.CrmId, _proc, service);

                                        returnValue.Success = quoteDetailResult.Success;
                                        returnValue.Result = quoteDetailResult.Result;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen konut seçiniz!";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public string GetOpportunityInfo(string oppId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                Opportunity oppInfo = OpportunityHelper.GetOpportunityDetail(new Guid(oppId), sda);

                if (oppInfo != null && oppInfo.OpportunityId != Guid.Empty)
                {
                    returnValue.ReturnObject = oppInfo;
                    returnValue.Success = true;
                    returnValue.Result = "Fırsat bilgisi çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProductInfo(string productId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                Product pInfo = ProductHelper.GetProductDetail(new Guid(productId), sda);

                if (pInfo != null && pInfo.ProductId != Guid.Empty)
                {
                    returnValue.ReturnObject = pInfo;
                    returnValue.Success = true;
                    returnValue.Result = "Ürün bilgisi çekildi.";
                }
                else
                {
                    returnValue.Result = "Konut bilgisi çekilemedi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSystemUserInfo(string userId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                SystemUser sInfo = SystemUserHelper.GetSystemUserInfo(new Guid(userId), sda);


                if (sInfo != null && sInfo.Image != null && sInfo.Image.Length > 0)
                {
                    sInfo.ImageBase64 = System.Convert.ToBase64String(sInfo.Image,
                                    0,
                                    sInfo.Image.Length);
                }

                if (sInfo != null && sInfo.SystemUserId != Guid.Empty)
                {
                    returnValue.ReturnObject = sInfo;
                    returnValue.Success = true;
                    returnValue.Result = "Kullanıcı bilgisi çekildi.";
                }
                else
                {
                    returnValue.Result = "Kullanıcı bilgisi çekilemedi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSystemUserInfoRetailer(string userId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                SystemUser sInfo = SystemUserHelper.GetSystemUserInfoRetailer(new Guid(userId), sda);


                if (sInfo != null && sInfo.Image != null && sInfo.Image.Length > 0)
                {
                    sInfo.ImageBase64 = System.Convert.ToBase64String(sInfo.Image,
                                    0,
                                    sInfo.Image.Length);
                }

                if (sInfo != null && sInfo.SystemUserId != Guid.Empty)
                {
                    returnValue.ReturnObject = sInfo;
                    returnValue.Success = true;
                    returnValue.Result = "Kullanıcı bilgisi çekildi.";
                }
                else
                {
                    returnValue.Result = "Kullanıcı bilgisi çekilemedi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetQuoteDetail(string quoteId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(quoteId))
                {
                    returnValue = QuoteHelper.GetQuoteDetail(new Guid(quoteId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen satış seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetRentalDetail(string rentalid)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(rentalid))
                {
                    returnValue = RentalHelper.GetRentalDetail(new Guid(rentalid), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen Kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSecondHandDetail(string secondhandid)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(secondhandid))
                {
                    returnValue = SecondHandHelper.GetSecondHandDetail(new Guid(secondhandid), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen 2.El Seçiniz seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult UpdateQuote(Quote quote)
        {
            MsCrmResult returnValue = new MsCrmResult();

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {
                if (quote != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = QuoteHelper.UpdateOrCreateQuote(quote, service, sda);
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult UpdateRental(Rental rental)
        {
            MsCrmResult returnValue = new MsCrmResult();

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {
                if (rental != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = RentalHelper.UpdateOrCreateRental(rental, service, sda);
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult UpdateSecondHand(SecondHand secondhand)
        {
            MsCrmResult returnValue = new MsCrmResult();

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            try
            {
                if (secondhand != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = SecondHandHelper.UpdateOrCreateSecondHand(secondhand, service, sda);
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult CreateOrUpdatePayment(Payment payment)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (payment != null)
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);
                    if (payment.Contact != null || payment.Account != null)
                    {
                        if (payment.Contact != null)
                            returnValue = ContactHelper.ContactHasAddress(payment.Contact.Id, sda);
                        else
                            returnValue = AccountHelper.AccountHasAddress(payment.Account.Id, sda);


                        if (returnValue.Success)
                        {
                            #region | KAPORA TUTAR KONTROLÜ |
                            if (payment.PaymentType == PaymentTypes.KaporaOdemesi)
                            {
                                if (payment.PaymentDate != null)
                                {
                                    decimal total = (decimal)payment.PaymentAmount;
                                    decimal minPrePaymentValue = 500;

                                    MsCrmResultObject quoteDetail = QuoteHelper.GetQuoteDetail(payment.Quote.Id, sda);
                                    if (quoteDetail.Success)
                                    {
                                        Quote _quote = (Quote)quoteDetail.ReturnObject;
                                        Product _product = ProductHelper.GetProductDetail(_quote.Products[0].ProductId, sda);
                                        MsCrmResultObject projectDetail = QuoteHelper.GetProjectDetail(_product.Project.Id, sda);
                                        if (projectDetail.Success)
                                        {
                                            minPrePaymentValue = (decimal)projectDetail.ReturnObject;
                                        }
                                    }

                                    if (payment.Currency.Id != Globals.CurrencyIdTL)
                                    {
                                        MsCrmResultObject currencyResult = CurrencyHelper.GetExchangeRateByCurrency((DateTime)payment.PaymentDate, payment.Currency.Id, sda);
                                        if (currencyResult.Success)
                                        {
                                            ExchangeRate currency = (ExchangeRate)currencyResult.ReturnObject;
                                            total = total * currency.SaleRate;
                                            if (total < minPrePaymentValue)
                                            {
                                                returnValue.Success = false;
                                                returnValue.Result = "Kapora bu proje için en az " + minPrePaymentValue.ToString("N0", CultureInfo.CurrentCulture) + " TL değerinde olmalıdır!";
                                            }
                                        }
                                        else
                                        {
                                            returnValue.Success = false;
                                            returnValue.Result = currencyResult.Result;
                                        }
                                    }
                                    else
                                    {
                                        if (total < minPrePaymentValue)
                                        {
                                            returnValue.Success = false;
                                            returnValue.Result = "Kapora bu proje için en az " + minPrePaymentValue.ToString("N0", CultureInfo.CurrentCulture) + " TL değerinde olmalıdır!";
                                        }
                                    }
                                }
                                else
                                {
                                    returnValue.Success = false;
                                    returnValue.Result = "Lütfen kapora tarihini seçiniz!";
                                }
                            }
                            #endregion

                            if (returnValue.Success)
                            {
                                MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(payment.Quote.Id, sda);
                                if (productResult.Success)
                                {
                                    List<Product> products = (List<Product>)productResult.ReturnObject;

                                    Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                                    if (_proc.StatusCode.Value == (int)ProductStatuses.Bos || _proc.StatusCode.Value == (int)ProductStatuses.YoneticiOpsiyonlu || _proc.StatusCode.Value == (int)ProductStatuses.Opsiyonlu)
                                    {
                                        returnValue = PaymentHelper.CreateOrUpdatePayment(payment, service);

                                        if (returnValue.Success && payment.PaymentType == PaymentTypes.KaporaOdemesi && payment.PaymentStatus == PaymentStatuses.KaporaAlindi)
                                        {
                                            #region | UPDATE QUOTE PREPAYMENT AMOUNT |
                                            Entity ent = new Entity("quote");
                                            ent["quoteid"] = payment.Quote.Id;
                                            ent["statuscode"] = new OptionSetValue((int)QuoteStatus.KaporaAlındı);
                                            ent["new_prepaymentamount"] = new Money((decimal)payment.PaymentAmount);
                                            ent["new_isprepaymenttaken"] = true;
                                            ent["new_prepaymentdate"] = payment.PaymentDate;
                                            ent["new_prepaymenttype"] = new OptionSetValue((int)payment.PaymentCashType);
                                            service.Update(ent);
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        returnValue.Success = false;
                                        returnValue.Result = "Konut durumu boş veya yönetici opsiyonlu olmalıdır!";
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen müşteriyi seçiniz!";
                    }

                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
            return returnValue;
        }

        public string GetQuotePrePayment(string quoteId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(quoteId))
                {
                    returnValue = QuoteHelper.GetQuotePayments(new Guid(quoteId), PaymentTypes.KaporaOdemesi, sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen satış seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult SendQuoteToApproval(string quoteId, string comment)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(quoteId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);


                    MsCrmResult prePaymentResult = PaymentHelper.QuoteHasPrePayment(new Guid(quoteId), sda);
                    if (prePaymentResult.Success)
                    {
                        MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(new Guid(quoteId), sda);
                        if (productResult.Success)
                        {
                            List<Product> products = (List<Product>)productResult.ReturnObject;

                            for (int i = 0; i < products.Count; i++)
                            {
                                Product _proc = ProductHelper.GetProductDetail(products[i].ProductId, sda);
                                if (_proc.StatusCode.Value == (int)ProductStatuses.Bos)
                                {
                                    if (!string.IsNullOrEmpty(comment))  //Onaya gönderme için herhangi bir yorum var ise Satış üzerindeki kullanıcı yorumu alanı update edilir.
                                    {
                                        QuoteHelper.UpdateQuoteUserComment(new Guid(quoteId), comment, service);
                                    }

                                    returnValue = QuoteHelper.UpdateQuoteStatus(new Guid(quoteId), QuoteStatus.OnayBekleniyor, Guid.Empty, service);
                                }
                                else
                                {
                                    returnValue.Success = false;
                                    returnValue.Result = "Konut durumu boş olmalıdır!";
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Kapora almadan satışı onaya gönderemezsiniz!";
                    }

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen satış seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult SendRentalToApproval(string rentalId, string comment)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(rentalId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject rentalResult = RentalHelper.GetRentalDetail(new Guid(rentalId), sda);
                    if (rentalResult.Success)
                    {
                        Rental _rental = (Rental)rentalResult.ReturnObject;
                        Product productResult = ProductHelper.GetProductDetail(_rental.Product.Id, sda);
                        if (productResult.UsedRentalSalesStatus.Value == 2)
                        {
                            returnValue = RentalHelper.SendToApproval(_rental.RentalId.Value, service);
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu kiralamaya uygun olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult SendSecondHandToApproval(string secondHandId, string comment)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(secondHandId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject rentalResult = SecondHandHelper.GetSecondHandDetail(new Guid(secondHandId), sda);
                    if (rentalResult.Success)
                    {
                        SecondHand _rental = (SecondHand)rentalResult.ReturnObject;
                        Product productResult = ProductHelper.GetProductDetail(_rental.Product.Id, sda);
                        if (productResult.UsedRentalSalesStatus.Value == 5)
                        {
                            returnValue = SecondHandHelper.SendToApproval(_rental.SecondHandId.Value, service);
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu 2.El Satışa uygun olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult ConfirmQuote(string quoteId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(quoteId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(new Guid(quoteId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.StatusCode.Value == (int)ProductStatuses.Bos || _proc.StatusCode.Value == (int)ProductStatuses.YoneticiOpsiyonlu || _proc.StatusCode.Value == (int)ProductStatuses.Opsiyonlu)
                        {
                            returnValue = QuoteHelper.UpdateQuoteStatus(new Guid(quoteId), QuoteStatus.Onaylandı, new Guid(userId), service);
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu boş veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen satış seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }
            return returnValue;
        }

        public MsCrmResult RefuseQuote(string quoteId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(quoteId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(new Guid(quoteId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.StatusCode.Value == (int)ProductStatuses.Bos || _proc.StatusCode.Value == (int)ProductStatuses.YoneticiOpsiyonlu || _proc.StatusCode.Value == (int)ProductStatuses.Opsiyonlu)
                        {
                            returnValue = QuoteHelper.UpdateQuoteStatus(new Guid(quoteId), QuoteStatus.Reddedildi, new Guid(userId), service);
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu boş veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen satış seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult ConfirmRental(string rentalId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(rentalId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = RentalHelper.GetRentalProducts(new Guid(rentalId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.UsedRentalSalesStatus == 2 ||
                             _proc.UsedRentalSalesStatus.Value == 3)
                        {
                            Entity ent = new Entity("product");
                            ent.Id = _proc.ProductId;
                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(4);
                            service.Update(ent);
                            returnValue = RentalHelper.UpdateRentalStatus(new Guid(rentalId), RentalStatuses.Tamamlandi, new Guid(userId), service);

                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu kiralamaya uygun veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }
            return returnValue;
        }

        public MsCrmResult RefuseRental(string rentalId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(rentalId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = RentalHelper.GetRentalProducts(new Guid(rentalId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.UsedRentalSalesStatus == 2 ||
                            _proc.UsedRentalSalesStatus.Value == 3)
                        {
                            Entity ent = new Entity("product");
                            ent.Id = _proc.ProductId;
                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(2);
                            service.Update(ent);

                            returnValue = RentalHelper.UpdateRentalStatus(new Guid(rentalId), RentalStatuses.IptalEdildi, new Guid(userId), service);
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu kiralamaya uygun veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult ConfirmSecondHand(string secondHandId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(secondHandId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = SecondHandHelper.GetSecondHandProducts(new Guid(secondHandId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.UsedRentalSalesStatus == 5 ||
                             _proc.UsedRentalSalesStatus.Value == 6)
                        {
                            returnValue = SecondHandHelper.UpdateSecondHandStatus(new Guid(secondHandId), SecondHandStatuses.Onaylandi, new Guid(userId), service);

                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu satışa uygun veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }
            return returnValue;
        }

        public MsCrmResult ConfirmSecondHandKaporaTaken(string secondHandId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(secondHandId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = SecondHandHelper.GetSecondHandProducts(new Guid(secondHandId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.UsedRentalSalesStatus == 5 ||
                             _proc.UsedRentalSalesStatus.Value == 6)
                        {
                            Entity ent = new Entity("product");
                            ent.Id = _proc.ProductId;
                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(7);
                            service.Update(ent);

                            returnValue = SecondHandHelper.UpdateSecondHandStatus(new Guid(secondHandId), SecondHandStatuses.OnOdemeAlindi, new Guid(userId), service);

                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu satışa uygun veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }
            return returnValue;
        }

        public MsCrmResult RefuseSecondHand(string secondHandId, string userId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(secondHandId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    IOrganizationService service = MSCRM.GetOrgService(true);

                    MsCrmResultObject productResult = SecondHandHelper.GetSecondHandProducts(new Guid(secondHandId), sda);
                    if (productResult.Success)
                    {
                        List<Product> products = (List<Product>)productResult.ReturnObject;

                        Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
                        if (_proc.UsedRentalSalesStatus == 5 ||
                            _proc.UsedRentalSalesStatus.Value == 6)
                        {
                            Entity ent = new Entity("product");
                            ent.Id = _proc.ProductId;
                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(5);
                            service.Update(ent);

                            returnValue = RentalHelper.UpdateRentalStatus(new Guid(secondHandId), RentalStatuses.IptalEdildi, new Guid(userId), service);
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = "Konut durumu 2.El satışa uygun veya yönetici opsiyonlu olmalıdır!";
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen kiralama seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public string GetCurrencies()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = CurrencyHelper.GetCurrencies(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CustomerHasCallCenterCall(string customerId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (!string.IsNullOrEmpty(customerId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    MsCrmResultObject callCenterResult = ActivityHelper.GetCallCenterPhoneCallsByCustomer(new Guid(customerId), sda);
                    if (callCenterResult.Success)
                    {
                        List<Activity> returnList = (List<Activity>)callCenterResult.ReturnObject;
                        returnValue.Success = true;
                        returnValue.CrmId = returnList[0].ActivityId;
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = callCenterResult.Result;
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşteri seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
            return returnValue;
        }

        public string GetCallCenterPhoneCalls()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ActivityHelper.GetPhonecallsByQueue(QueueTypes.CallCenter, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            jSer.MaxJsonLength = Int32.MaxValue;
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetCallCenterPhoneCallsTopkapi()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                //TOPKAPI GUID
                Guid queueId = new Guid("96936B64-51E7-E411-80D0-005056A60603");
                returnValue = ActivityHelper.GetPhonecallsByQueueId(queueId, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            jSer.MaxJsonLength = Int32.MaxValue;
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetNationalities()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = AddressHelper.GetNationalities(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CreateOrUpdateAccount(Account account)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (account != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = AccountHelper.CreateOrUpdateAccount(account, service);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public string GetAccountDetail(string accountId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(accountId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = AccountHelper.GetAccountDetail(new Guid(accountId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen firma seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetTaxOffices()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = AccountHelper.GetTaxOffices(sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public List<UserFeed> GetOldFeeds(string userId)
        {
            List<UserFeed> returnValue = new List<UserFeed>();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(userId))
                {
                    returnValue = FeedsHelper.GetUserOldFeeds(new Guid(userId), sda);
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public List<UserFeed> GetNotPostedFeeds()
        {
            List<UserFeed> returnValue = new List<UserFeed>();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                service = MSCRM.GetOrgService(true);

                returnValue = FeedsHelper.GetNotPostedFeeds(sda);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult UpdateFeedAsPosted(string feedId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                service = MSCRM.GetOrgService(true);

                returnValue = FeedsHelper.UpdateFeedAsPosted(new Guid(feedId), service);
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public MsCrmResult UpdateFeedAsRead(string feedId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                service = MSCRM.GetOrgService(true);

                returnValue = FeedsHelper.UpdateFeedAsRead(new Guid(feedId), service);
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {

            }

            return returnValue;
        }

        public string GetIpAddress()
        {
            string returnValue = string.Empty;

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                returnValue = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                returnValue = HttpContext.Current.Request.UserHostAddress;
            }

            return returnValue;
        }

        public MsCrmResult CheckEntityExists(string entityId, string entityName, string attributeName)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetEntityIdByAttributeName(entityName, attributeName, entityId, sda);
            }
            catch (Exception ex)
            {

                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public MsCrmResult CheckSaleCustomerIdentityAndAddress(string quoteId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                MsCrmResultObject resultQuote = QuoteHelper.GetQuoteDetail(new Guid(quoteId), sda);

                if (resultQuote.Success)
                {
                    Quote qd = (Quote)resultQuote.ReturnObject;

                    if (qd.Contact.LogicalName == "contact")
                        returnValue = ContactHelper.ContactHasAddressAndIdentity(qd.Contact.Id, sda);
                    else
                        returnValue = AccountHelper.AccountHasAddressAndTaxNo(qd.Contact.Id, sda);
                }

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public UserHeaderInfo GetUserHeaderInfo(string userId)
        {
            UserHeaderInfo returnValue = new UserHeaderInfo();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = SystemUserHelper.GetUserHeaderInfo(new Guid(userId), sda);

                returnValue.PlannedSalesAmountString = returnValue.PlannedSalesAmount.ToString("N0", CultureInfo.CurrentCulture);
                returnValue.SalesAmountString = returnValue.SalesAmount.ToString("N0", CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public string GetActivityInfo(string activityId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(activityId))
                {
                    returnValue = ActivityHelper.GetActivityInfo(new Guid(activityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen aktivite seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetPhoneCallActivityInfo(string activityId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(activityId))
                {
                    returnValue = ActivityHelper.GetPhoneCallActivityInfo(new Guid(activityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen aktivite seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetAppointmentActivityInfo(string activityId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(activityId))
                {
                    returnValue = ActivityHelper.GetAppointmentActivityInfo(new Guid(activityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen aktivite seçiniz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CheckActivityOwnership(string userId, string activityId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ActivityHelper.CheckActivityOwnership(new Guid(userId), new Guid(activityId), sda);

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public string GetUserAllIntrestedHouses(string customerId, string systemUserId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(customerId))
                {
                    SystemUser userDetail = SystemUserHelper.GetSystemUserInfo(new Guid(systemUserId), sda);
                    if (userDetail.SystemUserId != null)
                    {
                        if (userDetail.UserType != UserTypes.IsGyoSatisMuduru && userDetail.UserType != UserTypes.IsGyoSatisDanismani)
                        {
                            returnValue = OpportunityHelper.GetUserAllIntrestedHouses(new Guid(customerId), sda);
                        }
                        else
                        {
                            returnValue = OpportunityHelper.GetUserAllIntrestedHouses(new Guid(customerId), sda);
                            if (returnValue.Success)
                            {
                                List<Product> lstProcs = (List<Product>)returnValue.ReturnObject;

                                lstProcs = (from a in lstProcs
                                            where
                                            a.IsProjectGyo == true
                                            select a).ToList();

                                returnValue.ReturnObject = lstProcs;
                            }

                        }

                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteri bilgisi gönderilmedi!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetUserAllIntrestedEmptyHouses(string customerId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                if (!string.IsNullOrEmpty(customerId))
                {
                    if (returnValue.Success)
                    {
                        List<Product> lstPro = (List<Product>)returnValue.ReturnObject;

                        List<Product> query = (from a in lstPro
                                               where
                                               (int)a.StatusCode.Value == 1 //Boş
                                               select a).ToList();
                        if (query.Count > 0)
                        {
                            returnValue.ReturnObject = query;
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.ReturnObject = null;
                            returnValue.Result = "Boş konut bilgisi bulunmamaktadır.";
                        }


                    }

                    returnValue = OpportunityHelper.GetUserAllIntrestedHouses(new Guid(customerId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteri bilgisi gönderilmedi!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult ExportPhoneCall(List<Activity> phoneCallList)
        {
            MsCrmResult result = new MsCrmResult();
            System.Data.DataTable dt = new System.Data.DataTable("PhoneCall");
            dt.Columns.Add("Aktivite ID");
            dt.Columns.Add("Müşteri");
            dt.Columns.Add("Kullanıcı");
            dt.Columns.Add("Öncelik");
            dt.Columns.Add("Oluşturulma Tarihi");
            dt.Columns.Add("Telefon Numarası");

            foreach (Activity ac in phoneCallList)
            {
                DataRow dr = dt.NewRow();
                dr[0] = ac.ActivityId.ToString();
                dr[1] = ac.Contact.Name.ToString();
                dr[2] = ac.Owner.Name.ToString();
                dr[3] = ac.PriorityString.ToString();
                dr[4] = ac.CreatedOnString.ToString();
                dr[5] = string.IsNullOrEmpty(ac.PhoneNumber) ? string.Empty : ac.PhoneNumber.Replace("+90", "").Replace("-", "");
                dt.Rows.Add(dr);
            }

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            try
            {

                Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Worksheet ws = (Worksheet)wb.Worksheets[1];
                excelApp.Visible = false;
                ws.Name = dt.TableName;
                for (int i = 1; i < dt.Columns.Count + 1; i++)
                {
                    ws.Cells[1, i] = dt.Columns[i - 1].ColumnName;
                }

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        ws.Cells[j + 2, k + 1] = dt.Rows[j].ItemArray[k].ToString();
                    }
                }
                if (File.Exists(@Globals.PortalAttachmentFolder + "data.xlsx"))
                {
                    File.Delete(@Globals.PortalAttachmentFolder + "data.xlsx");
                }


                wb.SaveAs(@Globals.PortalAttachmentFolder + "data.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                wb.Close();
                result.Success = true;
                result.Result = @Globals.PortalAttachmentFolder + "data.xlsx";

            }
            catch (Exception)
            {

                result.Success = false;
                result.Result = string.Empty;
            }
            finally
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
            }


            return result;

        }

        public string GetCallCenterAgents()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetOptionSetValues(4210, "new_callcenteragent", sda);


            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetQuoteStatuses()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetOptionSetValues(1084, "statuscode", sda);

                if (returnValue.Success)
                {
                    List<StringMap> returnList = (List<StringMap>)returnValue.ReturnObject;

                    List<StringMap> newReturnList = new List<StringMap>();

                    for (int i = 0; i < returnList.Count; i++)
                    {
                        QuoteStatus status = (QuoteStatus)returnList[i].Value;
                        switch (status)
                        {
                            case QuoteStatus.SozlesmeHazirlandi:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.DevamEdiyor:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.Sözleşmeİmzalandı:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.KaporaAlındı:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.Kazanıldı:
                                break;
                            case QuoteStatus.Kaybedildi:
                                break;
                            case QuoteStatus.İptalEdildi:
                                break;
                            case QuoteStatus.Düzeltilmiş:
                                break;
                            case QuoteStatus.İptalAktarıldı:
                                break;
                            case QuoteStatus.MuhasebeyeAktarıldı:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.TeslimEdildi:
                                break;
                            case QuoteStatus.BittiSatıldı:
                                break;
                            case QuoteStatus.OnayBekleniyor:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.Onaylandı:
                                newReturnList.Add(returnList[i]);
                                break;
                            case QuoteStatus.Reddedildi:
                                break;
                            case QuoteStatus.DirektorOnayiBekleniyor:
                                newReturnList.Add(returnList[i]);
                                break;
                            default:
                                break;
                        }
                    }

                    returnValue.ReturnObject = newReturnList;
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProjectsForActivity(Guid phonecallId, Guid appointmentId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = InterestedProjectHelper.GetProjectsForActivity(phonecallId, appointmentId, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetActivityInterestedProjects(string activityId)
        {
            Thread.Sleep(1000);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                if (!string.IsNullOrEmpty(activityId))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);

                    returnValue = InterestedProjectHelper.GetActivityInterestedProjects(new Guid(activityId), sda);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CreateInterestedProject(string[] interestedProjects, Guid phonecallId, Guid appointmentId)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (interestedProjects != null && interestedProjects.Length > 0)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);

                    for (int i = 0; i < interestedProjects.Length; i++)
                    {
                        returnValue = InterestedProjectHelper.CreateInterestedProject(new Guid(interestedProjects[i]), phonecallId, appointmentId, service);
                        if (!returnValue.Success)
                            throw new Exception(returnValue.Result);
                    }
                }

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult RemoveActivityInterestedProjects(string[] interestedProjectIds)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (interestedProjectIds != null && interestedProjectIds.Length > 0)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    for (int i = 0; i < interestedProjectIds.Length; i++)
                    {
                        InterestedProjectHelper.RemoveInterestedProject(new Guid(interestedProjectIds[i]), service);
                    }

                    returnValue.Success = true;
                    returnValue.Result = "İlgilendiği projeler başarıyla kaldırıldı.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public string GetCustomerRelationSpecialists()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = SystemUserHelper.GetSalesConsultants(UserTypes.MusteriIliskileri, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetProductOptionInfo(string productId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ProductHelper.GetProductOptionInfo(new Guid(productId), sda);


            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetOpportunityLostStatuses()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = GeneralHelper.GetStatusCodesByState(3, 2, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CloseOpportunityAsLost(Guid opportunityId, int statusCode)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                IOrganizationService service = MSCRM.GetOrgService(true);

                returnValue = OpportunityHelper.CloseOppAsLost(opportunityId, statusCode, DateTime.Now, service);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public MsCrmResult CheckCustomerApartmentOwner(string customerId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = QuoteHelper.CustomerCheckApartmentOwner(new Guid(customerId), sda);

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        //All Team Chart Data
        public string GetMonthlySalesAmountData()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;
                DateTime before12Month = DateTime.UtcNow.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);
                MsCrmResultObject resultQuotes = QuoteHelper.GetQuotesBySalesDateRange(startOfMonth.ToUniversalTime(), DateTime.Now, sda);

                if (resultQuotes.Success)
                {
                    List<Quote> lstQuotes = new List<Quote>();

                    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                    List<string[]> lstTest = new List<string[]>();



                    string[] months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

                    //for (int i = 0; i < (months.Length - 1); i++)
                    for (int i = 0; i < 13; i++)
                    {
                        var refDate = before12Month.AddMonths(i);

                        decimal salesAmount = (from bs in lstQuotes
                                               where
                                               bs.SalesDate != null
                                               &&
                                               (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                                               &&
                                               ((DateTime)bs.SalesDate).Year == refDate.Year
                                               &&
                                               ((DateTime)bs.SalesDate).Month == refDate.Month
                                               select
                                                 bs.HouseListPrice
                                   ).Sum();

                        lstTest.Add(new string[] { months[refDate.Month - 1] + refDate.ToString("yy"), Convert.ToInt32(salesAmount).ToString() });

                    }

                    returnValue.ReturnObject = lstTest.ToList();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetMonthlySalesQuantityData()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.UtcNow.Year;
                DateTime before12Month = DateTime.Now.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);
                MsCrmResultObject resultQuotes = QuoteHelper.GetQuotesBySalesDateRange(startOfMonth.ToUniversalTime(), DateTime.UtcNow, sda);

                if (resultQuotes.Success)
                {
                    List<Quote> lstQuotes = new List<Quote>();

                    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                    List<string[]> lstTest = new List<string[]>();



                    string[] months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

                    //for (int i = 0; i < (months.Length - 1); i++)
                    for (int i = 0; i < 13; i++)
                    {
                        var refDate = before12Month.AddMonths(i);

                        decimal salesAmount = (from bs in lstQuotes
                                               where
                                               bs.SalesDate != null
                                               &&
                                               (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                                               &&
                                               ((DateTime)bs.SalesDate).Year == refDate.Year
                                               &&
                                               ((DateTime)bs.SalesDate).Month == refDate.Month
                                               select
                                                 bs.HouseListPrice
                                   ).Count();

                        lstTest.Add(new string[] { months[refDate.Month - 1] + refDate.ToString("yy"), Convert.ToInt32(salesAmount).ToString() });

                    }

                    returnValue.ReturnObject = lstTest.ToList();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSalesAmountByProject()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.UtcNow.Year;

                //DateTime before12Month = DateTime.Now.AddMonths(-1);
                //DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                DateTime startdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);



                List<ChartKeyValues> lstData = QuoteHelper.GetSalesAmountByProjectData(startdate, DateTime.UtcNow.ToUniversalTime(), sda);

                returnValue.ReturnObject = lstData;
                returnValue.Success = true;


                //MsCrmResultObject resultQuotes = QuoteHelper.GetAllQuotes(sda);

                //if (resultQuotes.Success)
                //{
                //    List<Quote> lstQuotes = new List<Quote>();

                //    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                //    List<ChartKeyValues> lstTest = (from bs in lstQuotes
                //                                    where
                //                                    bs.SalesDate != null
                //                                    &&
                //                                    (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                //                                    &&
                //                                    ((DateTime)bs.SalesDate).Year == thisYear
                //                                    &&
                //                                    bs.Products != null && bs.Products.Count > 0
                //                                    group bs by bs.Products[0].Project.Name into g
                //                                    orderby g.Sum(x => x.HouseListPrice)
                //                                    select new ChartKeyValues
                //                                    {
                //                                        Name = g.First().Products[0].Project.Name,
                //                                        Value = Convert.ToInt32(g.Sum(x => x.HouseListPrice)),
                //                                        ValueText = (g.Sum(x => x.HouseListPrice)).ToString("N0", CultureInfo.CurrentCulture)
                //                                    }).ToList();

                //    returnValue.ReturnObject = lstTest;
                //}
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSalesQuantityByProject()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.UtcNow.Year;

                //DateTime before12Month = DateTime.Now.AddMonths(-1);
                //DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);
                DateTime startdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                List<ChartKeyValues> lstData = QuoteHelper.GetSalesQuantityByProjectData(startdate, DateTime.UtcNow.ToUniversalTime(), sda);

                returnValue.ReturnObject = lstData;
                returnValue.Success = true;

                //MsCrmResultObject resultQuotes = QuoteHelper.GetAllQuotes(sda);

                //if (resultQuotes.Success)
                //{
                //    List<Quote> lstQuotes = new List<Quote>();

                //    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                //    List<ChartKeyValues> lstTest = (from bs in lstQuotes
                //                                    where
                //                                    bs.SalesDate != null
                //                                    &&
                //                                    (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                //                                    &&
                //                                    ((DateTime)bs.SalesDate).Year == thisYear
                //                                    &&
                //                                    bs.Products != null && bs.Products.Count > 0
                //                                    group bs by bs.Products[0].Project.Name into g
                //                                    orderby g.Sum(x => x.HouseListPrice)
                //                                    select new ChartKeyValues
                //                                    {
                //                                        Name = g.First().Products[0].Project.Name,
                //                                        Value = Convert.ToInt32(g.Count()),
                //                                        ValueText = (g.Count().ToString("N0", CultureInfo.CurrentCulture))
                //                                    }).ToList();

                //    returnValue.ReturnObject = lstTest;
                //}
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        //All Team data general chart data 
        public string GetMonthlySalesGeneralAmountData()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;
                DateTime before12Month = DateTime.UtcNow.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);
                MsCrmResultObject resultQuotes = QuoteHelper.GetQuotesBySalesDateRange(startOfMonth.ToUniversalTime(), DateTime.Now, sda);

                if (resultQuotes.Success)
                {
                    List<Quote> lstQuotes = new List<Quote>();

                    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                    List<string[]> lstTest = new List<string[]>();



                    string[] months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

                    //for (int i = 0; i < (months.Length - 1); i++)
                    for (int i = 0; i < 13; i++)
                    {
                        var refDate = before12Month.AddMonths(i);

                        decimal salesAmount = (from bs in lstQuotes
                                               where
                                               bs.SalesDate != null
                                               &&
                                               (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı ||
                                                bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı ||
                                                bs.StatusCode.Value == (int)QuoteStatus.KaporaAlındı ||
                                                bs.StatusCode.Value == (int)QuoteStatus.SozlesmeHazirlandi)
                                               &&
                                               ((DateTime)bs.SalesDate).Year == refDate.Year
                                               &&
                                               ((DateTime)bs.SalesDate).Month == refDate.Month
                                               select
                                                 bs.HouseListPrice
                                   ).Sum();

                        lstTest.Add(new string[] { months[refDate.Month - 1] + refDate.ToString("yy"), Convert.ToInt32(salesAmount).ToString() });

                    }

                    returnValue.ReturnObject = lstTest.ToList();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetMonthlySalesGeneralQuantityData()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.Now.Year;
                DateTime before12Month = DateTime.UtcNow.AddMonths(-12);
                DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);

                //MsCrmResultObject resultQuotes = QuoteHelper.GetSystemUserQuotes(new Guid(userId), sda);
                MsCrmResultObject resultQuotes = QuoteHelper.GetQuotesBySalesDateRange(startOfMonth.ToUniversalTime(), DateTime.Now, sda);

                if (resultQuotes.Success)
                {
                    List<Quote> lstQuotes = new List<Quote>();

                    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                    List<string[]> lstTest = new List<string[]>();



                    string[] months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;

                    //for (int i = 0; i < (months.Length - 1); i++)
                    for (int i = 0; i < 13; i++)
                    {
                        var refDate = before12Month.AddMonths(i);

                        decimal salesAmount = (from bs in lstQuotes
                                               where
                                               bs.SalesDate != null
                                               &&
                                               (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı
                                               || bs.StatusCode.Value == (int)QuoteStatus.KaporaAlındı || bs.StatusCode.Value == (int)QuoteStatus.SozlesmeHazirlandi)
                                               &&
                                               ((DateTime)bs.SalesDate).Year == refDate.Year
                                               &&
                                               ((DateTime)bs.SalesDate).Month == refDate.Month
                                               select
                                                 bs.HouseListPrice
                                   ).Count();

                        lstTest.Add(new string[] { months[refDate.Month - 1] + refDate.ToString("yy"), Convert.ToInt32(salesAmount).ToString() });

                    }

                    returnValue.ReturnObject = lstTest.ToList();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetSalesGeneralAmountByProject()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.UtcNow.Year;

                //DateTime before12Month = DateTime.Now.AddMonths(-1);
                //DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);
                DateTime startdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                List<ChartKeyValues> lstData = QuoteHelper.GetSalesGeneralAmountByProjectData(startdate, DateTime.UtcNow.ToUniversalTime(), sda);

                returnValue.ReturnObject = lstData;
                returnValue.Success = true;


                //MsCrmResultObject resultQuotes = QuoteHelper.GetAllQuotes(sda);

                //if (resultQuotes.Success)
                //{
                //    List<Quote> lstQuotes = new List<Quote>();

                //    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                //    List<ChartKeyValues> lstTest = (from bs in lstQuotes
                //                                    where
                //                                    bs.SalesDate != null
                //                                    &&
                //                                    (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                //                                    &&
                //                                    ((DateTime)bs.SalesDate).Year == thisYear
                //                                    &&
                //                                    bs.Products != null && bs.Products.Count > 0
                //                                    group bs by bs.Products[0].Project.Name into g
                //                                    orderby g.Sum(x => x.HouseListPrice)
                //                                    select new ChartKeyValues
                //                                    {
                //                                        Name = g.First().Products[0].Project.Name,
                //                                        Value = Convert.ToInt32(g.Sum(x => x.HouseListPrice)),
                //                                        ValueText = (g.Sum(x => x.HouseListPrice)).ToString("N0", CultureInfo.CurrentCulture)
                //                                    }).ToList();

                //    returnValue.ReturnObject = lstTest;
                //}
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetGeneralSalesQuantityByProject()
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                int thisYear = DateTime.UtcNow.Year;

                //DateTime before12Month = DateTime.Now.AddMonths(-1);
                //DateTime startOfMonth = new DateTime(before12Month.Year, before12Month.Month, 1);
                DateTime startdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                List<ChartKeyValues> lstData = QuoteHelper.GetSalesGeneralQuantityByProjectData(startdate, DateTime.UtcNow.ToUniversalTime(), sda);

                returnValue.ReturnObject = lstData;
                returnValue.Success = true;

                //MsCrmResultObject resultQuotes = QuoteHelper.GetAllQuotes(sda);

                //if (resultQuotes.Success)
                //{
                //    List<Quote> lstQuotes = new List<Quote>();

                //    lstQuotes = (List<Quote>)resultQuotes.ReturnObject;

                //    List<ChartKeyValues> lstTest = (from bs in lstQuotes
                //                                    where
                //                                    bs.SalesDate != null
                //                                    &&
                //                                    (bs.StatusCode.Value == (int)QuoteStatus.MuhasebeyeAktarıldı || bs.StatusCode.Value == (int)QuoteStatus.Sözleşmeİmzalandı)
                //                                    &&
                //                                    ((DateTime)bs.SalesDate).Year == thisYear
                //                                    &&
                //                                    bs.Products != null && bs.Products.Count > 0
                //                                    group bs by bs.Products[0].Project.Name into g
                //                                    orderby g.Sum(x => x.HouseListPrice)
                //                                    select new ChartKeyValues
                //                                    {
                //                                        Name = g.First().Products[0].Project.Name,
                //                                        Value = Convert.ToInt32(g.Count()),
                //                                        ValueText = (g.Count().ToString("N0", CultureInfo.CurrentCulture))
                //                                    }).ToList();

                //    returnValue.ReturnObject = lstTest;
                //}
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }


            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }


        public UserHeaderInfo GetAllHeaderInfo()
        {
            UserHeaderInfo returnValue = new UserHeaderInfo();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = SystemUserHelper.GetAllHeaderInfo(sda);

                returnValue.PlannedSalesAmountString = returnValue.PlannedSalesAmount.ToString("N0", CultureInfo.CurrentCulture);
                returnValue.SalesAmountString = returnValue.SalesAmount.ToString("N0", CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

            return returnValue;
        }

        public MsCrmResult SecondHandAttachment()
        {
            MsCrmResult retVal = new MsCrmResult();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                //retVal = QuoteHelper.CustomerCheckApartmentOwner(new Guid(customerId), sda);

            }
            catch (Exception ex)
            {
                retVal.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
            return retVal;
        }

        public string GetContactAttachment(string contactId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                returnValue.Success = true;
                returnValue.Result = "Kisi bilgileri alındı";
                returnValue.ReturnObject = ContactHelper.GetContactAttachment(new Guid(contactId), sda);

            }
            catch (Exception ex)
            {

                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetRentalAttachment(string rentalId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);
                returnValue.Success = true;
                returnValue.Result = "kiralama bilgileri alındı";
                returnValue.ReturnObject = QuoteHelper.GetRentralAttachment(new Guid(rentalId), sda);

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string GetDisplayLocationAttachment(string activityId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                EntityReference ann = QuoteHelper.GetDisplayLocationAttachment(new Guid(activityId), sda);
                if (ann != null)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Yer gösterme belgeleri alındı.";
                    returnValue.ReturnObject = ann;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Yer gösterme belgesi bulunamadı.";
                    returnValue.ReturnObject = null;
                }


            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public MsCrmResult CreateAuthorityDocument(AuthorityDocument document)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                returnValue = SecondHandHelper.CreateOrUpdateAuthorityDocument(document, service);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        #region | LOYALTY |
        public MsCrmResult ConfirmPointUsage(string loyaltyPointId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                if (!string.IsNullOrWhiteSpace(loyaltyPointId))
                {
                    Guid loyaltyPoint = new Guid(loyaltyPointId);
                    Initializer.LoyatyPointBusiness.ConfirmPointUsage(loyaltyPoint);

                    returnValue.Success = true;
                }
                else
                {
                    throw new Exception("INVALID_PARAMETER:loyaltyPointId");
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public MsCrmResult RefusePointUsage(string loyaltyPointId)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                if (!string.IsNullOrWhiteSpace(loyaltyPointId))
                {
                    Guid loyaltyPoint = new Guid(loyaltyPointId);
                    Initializer.LoyatyPointBusiness.RefusePointUsage(loyaltyPoint);

                    returnValue.Success = true;
                }
                else
                {
                    throw new Exception("INVALID_PARAMETER:loyaltyPointId");
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public MsCrmResult<LoyaltyPointSummary> GetContactPointSummary(string contactId)
        {
            MsCrmResult<LoyaltyPointSummary> returnValue = new MsCrmResult<LoyaltyPointSummary>();

            try
            {
                if (!string.IsNullOrWhiteSpace(contactId))
                {
                    Guid contact = new Guid(contactId);
                    returnValue.ReturnObject = Initializer.LoyatyPointBusiness.GetContactPointSummary(contact);

                    returnValue.Success = true;
                }
                else
                {
                    throw new Exception("INVALID_PARAMETER:contactId");
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public MsCrmResult<CrmEntity.LoyaltyPoint> GetLoyaltyPoint(string loyaltyPointId)
        {
            MsCrmResult<CrmEntity.LoyaltyPoint> returnValue = new MsCrmResult<CrmEntity.LoyaltyPoint>();

            try
            {
                if (!string.IsNullOrWhiteSpace(loyaltyPointId))
                {
                    Guid loyaltyPoint = new Guid(loyaltyPointId);
                    returnValue.ReturnObject = Initializer.LoyatyPointBusiness.Get(loyaltyPoint);

                    returnValue.Success = true;
                }
                else
                {
                    throw new Exception("INVALID_PARAMETER:loyaltyPointId");
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }
        #endregion
    }
}
