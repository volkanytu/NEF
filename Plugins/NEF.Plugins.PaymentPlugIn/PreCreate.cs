using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.PaymentPlugIn
{
    public class PreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                #region | Service |
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService adminService = serviceFactory.CreateOrganizationService(Globals.AdministratorId);
                OrganizationServiceContext orgContext = new OrganizationServiceContext(adminService);

                #endregion

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    PaymentTypes type = (PaymentTypes)((OptionSetValue)entity["new_type"]).Value;

                    if (entity.Attributes.Contains("new_quoteid") && entity["new_quoteid"] != null)
                    {
                        EntityReference quote = (EntityReference)entity["new_quoteid"];

                        #region | PRE PAYMENT PROCESS |
                        if (type == PaymentTypes.KaporaOdemesi)
                        {
                            MsCrmResult prePaymentResult = PaymentHelper.QuoteHasPrePayment(quote.Id, sda);
                            if (prePaymentResult.Success) // Satışa ait önceden kapora ödemesi varsa
                                throw new Exception(prePaymentResult.Result);
                            else
                            {
                                #region | MÜŞTERİ İLGİLİ KİŞİ İSE |
                                if (entity.Attributes.Contains("new_contactid") && entity["new_contactid"] != null)
                                {
                                    EntityReference _c = (EntityReference)entity["new_contactid"];
                                    MsCrmResultObject contactResult = ContactHelper.GetContactDetail(_c.Id, sda);
                                    if (contactResult.Success)
                                    {
                                        Contact _contact = (Contact)contactResult.ReturnObject;

                                        if (_contact.Nationality.Name == "TC")
                                        {
                                            MsCrmResult identityControl = GeneralHelper.CheckIdentityNumber(_contact.IdentityNumber);
                                            if (!identityControl.Success)
                                            {
                                                throw new Exception(identityControl.Result);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(contactResult.Result);
                                    }
                                }
                                #endregion

                                #region | MÜŞTERİ FİRMA İSE |
                                if (entity.Attributes.Contains("new_accountid") && entity["new_accountid"] != null)
                                {
                                    EntityReference _c = (EntityReference)entity["new_accountid"];
                                    MsCrmResultObject accountResult = AccountHelper.GetAccountDetail(_c.Id, sda);
                                    if (accountResult.Success)
                                    {
                                        Account _account = (Account)accountResult.ReturnObject;
                                        if (string.IsNullOrEmpty(_account.TaxNumber))
                                        {
                                            throw new Exception("Firma vergi numarası boş olamaz!");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(accountResult.Result);
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                       

                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }

        }
    }
}
