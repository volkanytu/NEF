﻿using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.SecondHandPlugIn
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SERVICE |
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            #region | Validate Request |
            //Target yoksa veya Entity tipinde değilse, devam etme.
            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
            {
                return;
            }
            #endregion

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(Globals.AdministratorId);

            #region |DEFINE IMAGE IF EXISTS|
            Entity preImage = null;

            if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
            {
                preImage = (Entity)context.PreEntityImages["PreImage"];
            }
            #endregion

            #endregion

            Entity entity = (Entity)context.InputParameters["Target"];

            try
            {
                if (entity.Attributes.Contains("new_issendingapproval"))
                {
                    if (entity.GetAttributeValue<bool>("new_issendingapproval"))
                    {
                        EntityReference currency = preImage.Attributes.Contains("transactioncurrencyid") && preImage["transactioncurrencyid"] != null ? (EntityReference)preImage["transactioncurrencyid"] : null;
                        EntityReference projectId = preImage.Attributes.Contains("new_projectid") && preImage["new_projectid"] != null ? (EntityReference)preImage["new_projectid"] : null;


                        MsCrmResultObject productResult = SecondHandHelper.GetSecondHandProducts(entity.Id, sda);
                        if (productResult.Success)
                        {
                            //Ürün alındı
                            Product product = ((List<Product>)productResult.ReturnObject)[0];
                            //Rule alındı
                            SecondHandControlSetting control = ProductHelper.GetSecondHandControlSettingByProject(product.Project.Id, sda);
                            //Kiralama tutarı alındı
                            decimal secondHandAmount = preImage.GetAttributeValue<Money>("new_salesfee").Value;
                            if (control.SecondHandControlSettingId != Guid.Empty)
                            {
                                if (control.ConsultantRate != null)
                                {
                                    if (control.ConsultantRate != decimal.MaxValue)
                                    {
                                        decimal rate = (product.PaymentOfHire.Value * (control.ConsultantRate.Value / 100));
                                        decimal minRate = product.PaymentOfHire.Value - rate;
                                        decimal maxRate = product.PaymentOfHire.Value + rate;

                                        if (secondHandAmount >= minRate && secondHandAmount <= maxRate)
                                        {
                                            //Opsiyona takılmaz.
                                            Entity ent = new Entity("product");
                                            ent.Id = product.ProductId;
                                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(7);//Satıldı.
                                            service.Update(ent);
                                            entity["statuscode"] = new OptionSetValue((int)SecondHandStatuses.Onaylandi); //Kiralama Durumu
                                        }
                                        else
                                        {
                                            //Ürün kiralama opsiyonlu
                                            //Onaya gönder
                                            Entity ent = new Entity("product");
                                            ent.Id = product.ProductId;
                                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(6);//konut durumu 2.el satış opsiyonlu
                                            service.Update(ent);
                                            entity["statuscode"] = new OptionSetValue((int)SecondHandStatuses.OnayBekleniyor); //Kiralama Durumu
                                            MsCrmResult mailResult = SecondHandHelper.SendMailSecondHandToApproval(product, preImage, UserTypes.IkinciElSatisDirektoru, sda, service);
                                            if (!mailResult.Success)
                                            {
                                                throw new Exception(mailResult.Result);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
        }
    }
}
