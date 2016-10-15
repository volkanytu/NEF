using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.QuotePlugIn
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {

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

                EntityReference customer = entity.Attributes.Contains("customerid") && entity["customerid"] != null ? (EntityReference)entity["customerid"] : preImage.Attributes.Contains("customerid") && preImage["customerid"] != null ? (EntityReference)preImage["customerid"] : null;
                EntityReference financialAccount = preImage.Attributes.Contains("new_financialaccountid") && preImage["new_financialaccountid"] != null ? (EntityReference)preImage["new_financialaccountid"] : null;
                EntityReference currency = preImage.Attributes.Contains("transactioncurrencyid") && preImage["transactioncurrencyid"] != null ? (EntityReference)preImage["transactioncurrencyid"] : null;
                EntityReference secondCustomer = entity.Attributes.Contains("new_secondcontactid") && entity["new_secondcontactid"] != null ? (EntityReference)entity["new_secondcontactid"] : preImage.Attributes.Contains("new_secondcontactid") && preImage["new_secondcontactid"] != null ? (EntityReference)preImage["new_secondcontactid"] : null;
                EntityReference projectId = entity.Attributes.Contains("new_projectid") && entity["new_projectid"] != null ? (EntityReference)entity["new_projectid"] : preImage.Attributes.Contains("new_projectid") && preImage["new_projectid"] != null ? (EntityReference)preImage["new_projectid"] : null;

                #region | QUOTE STATUS PROCESS |
                //Developed By Kemal Burak YILMAZ
                if (entity.Attributes.Contains("statuscode") && customer != null)
                {
                    QuoteStatus _oldQuoteStatus = QuoteStatus.DevamEdiyor;

                    #region | OLD STATUS CODE |
                    if (preImage.Contains("statuscode") && preImage["statuscode"] != null)
                    {
                        _oldQuoteStatus = (QuoteStatus)((OptionSetValue)preImage["statuscode"]).Value;
                    }
                    #endregion


                    QuoteStatus _quoteStatus = (QuoteStatus)((OptionSetValue)entity["statuscode"]).Value;
                    Guid ownerId = ((EntityReference)preImage["ownerid"]).Id;
                    bool isRevized = false;
                    if (preImage.Attributes.Contains("revisionnumber"))
                    {
                        if (Convert.ToInt32(preImage["revisionnumber"]) != 0)
                        {
                            isRevized = true;
                        }
                    }
                    string ownerName = ((EntityReference)preImage["ownerid"]).Name;
                    Guid contactId = customer.Id;
                    string customerName = customer.Name;

                    if (_quoteStatus == QuoteStatus.OnayBekleniyor) //Satış onaya gönderildi ise
                    {
                        #region | PRODUCT STATUS PROCESS |
                        //Onaya gönderildi ise konutun durumu opsiyonlu olarak değiştirilir. 20 dk içerisinde onaylanmazsa boş olarak düzeltilir bir job yardımıyla
                        MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(entity.Id, sda);
                        if (productResult.Success)
                        {
                            #region | SET IS GYO |
                            //Developed by Kemal Burak YILMAZ
                            //Developed on 06.04.2015
                            //İş GYO Projelerinde; iş ünvanı "Satış Müdürü İş GYO" olan kullanıcılara da mail gönderilir.

                            bool isIsGyo = false;
                            if (projectId != null)
                            {
                                Entity project = service.Retrieve("new_project", projectId.Id, new ColumnSet(true));
                                isIsGyo = project.Attributes.Contains("new_iswithisgyo") ? project["new_iswithisgyo"] != null ? (bool)project["new_iswithisgyo"] : false : false;
                            }


                            #endregion

                            List<Product> returnList = (List<Product>)productResult.ReturnObject;

                            SystemUser userInfo = SystemUserHelper.GetSystemUserInfo(ownerId, sda);
                            if (userInfo.BusinessUnitId != null && userInfo.BusinessUnitId != Guid.Empty)
                            {
                                if (userInfo.BusinessUnitId.ToString().ToUpper() == Globals.AlternatifBusinessUnitId.ToString().ToUpper())
                                {
                                    #region | YENİ ONAY SÜRECİ |
                                    Product _product = returnList[0];
                                    decimal discountRate = preImage.Attributes.Contains("new_paymentplandiscountrate") ? (decimal)preImage["new_paymentplandiscountrate"] : decimal.MaxValue;
                                    decimal m2Price = preImage.Attributes.Contains("new_persquaremeter") ? (decimal)((Money)preImage["new_persquaremeter"]).Value : decimal.MaxValue;

                                    QuoteControlSetting control = ProductHelper.GetControlSettingByProject(_product.Project.Id, sda);
                                    if (control.QuoteControlSettingId != Guid.Empty)
                                    {

                                        if (m2Price != decimal.MaxValue)
                                        {
                                            if (m2Price < control.ConsultantUnitPrice && m2Price >= control.ManagerUnitPrice)
                                            {
                                                #region | QUOTE SEND CONFIRM PROCESS |
                                                MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, sda, service);
                                                if (!mailResult.Success)
                                                {
                                                    throw new Exception(mailResult.Result);
                                                }
                                                #endregion

                                                #region | SET PRODUCT STATUS |
                                                HomeOption hOpt = new HomeOption();
                                                hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                hOpt.AdministratorOption = false;
                                                hOpt.Product = new EntityReference("product", _product.ProductId);
                                                hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                if (!productStatusResult.Success)
                                                    throw new Exception(productStatusResult.Result);
                                                #endregion

                                                entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaya Düşme Nedeni
                                                entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                            }
                                            else if (m2Price < control.ManagerUnitPrice && m2Price >= control.DirectorUnitPrice)
                                            {
                                                #region | QUOTE SEND CONFIRM PROCESS |
                                                MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, sda, service);
                                                if (!mailResult.Success)
                                                {
                                                    throw new Exception(mailResult.Result);
                                                }
                                                #endregion

                                                #region | SET PRODUCT STATUS |
                                                HomeOption hOpt = new HomeOption();
                                                hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                hOpt.AdministratorOption = false;
                                                hOpt.Product = new EntityReference("product", _product.ProductId);
                                                hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                if (!productStatusResult.Success)
                                                    throw new Exception(productStatusResult.Result);
                                                #endregion

                                                entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor); //Satış Durumu
                                                entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaya Düşme Nedeni
                                                entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                            }
                                            else if (m2Price < control.DirectorUnitPrice)
                                            {
                                                throw new Exception("Girmiş olduğunuzbirim metre kare satış fiyatı bu proje için çok düşük!");
                                            }
                                            else
                                            {
                                                if (discountRate != decimal.MaxValue)
                                                {
                                                    if (discountRate <= control.ConsultantRate)
                                                    {
                                                        entity["statuscode"] = new OptionSetValue((int)QuoteStatus.Onaylandı);
                                                        entity["new_approvaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaylanma Nedeni
                                                        entity["new_fallingapprovaltype"] = null; //Onaya Düşme Nedeni Boşaltılır

                                                        #region | SET SALES OFFICE |
                                                        MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                                                        if (lastActivityResult.Success)
                                                        {
                                                            Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                                            entity["new_salesofficeid"] = lastActivity.SalesOffice;
                                                        }
                                                        #endregion
                                                    }
                                                    else if (discountRate <= control.ManagerRate)
                                                    {
                                                        #region | QUOTE SEND CONFIRM PROCESS |
                                                        MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, sda, service);
                                                        if (!mailResult.Success)
                                                        {
                                                            throw new Exception(mailResult.Result);
                                                        }
                                                        #endregion

                                                        #region | SET PRODUCT STATUS |
                                                        HomeOption hOpt = new HomeOption();
                                                        hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                        hOpt.AdministratorOption = false;
                                                        hOpt.Product = new EntityReference("product", _product.ProductId);
                                                        hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                        //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                        MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                        if (!productStatusResult.Success)
                                                            throw new Exception(productStatusResult.Result);
                                                        #endregion

                                                        entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaya Düşme Nedeni
                                                        entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                                    }
                                                    else if (discountRate <= control.DirectorRate)
                                                    {
                                                        entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor);

                                                        #region | QUOTE SEND CONFIRM PROCESS |
                                                        MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, sda, service);
                                                        if (!mailResult.Success)
                                                        {
                                                            throw new Exception(mailResult.Result);
                                                        }
                                                        #endregion

                                                        #region | SET PRODUCT STATUS |
                                                        HomeOption hOpt = new HomeOption();
                                                        hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                        hOpt.AdministratorOption = false;
                                                        hOpt.Product = new EntityReference("product", _product.ProductId);
                                                        hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                        //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                        MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                        if (!productStatusResult.Success)
                                                            throw new Exception(productStatusResult.Result);
                                                        #endregion

                                                        entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor); //Satış Durumu
                                                        entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaya Düşme Nedeni
                                                        entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Girmiş olduğunuz indirim oranı bu proje için çok fazla!");
                                                    }
                                                }
                                                else
                                                {
                                                    entity["statuscode"] = new OptionSetValue((int)QuoteStatus.Onaylandı);


                                                    #region | SET SALES OFFICE |
                                                    MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                                                    if (lastActivityResult.Success)
                                                    {
                                                        Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                                        entity["new_salesofficeid"] = lastActivity.SalesOffice;
                                                    }
                                                    #endregion

                                                    entity["new_approvaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaylanma Nedeni
                                                    entity["new_fallingapprovaltype"] = null; //Onaya Düşme Nedeni Boşaltılır
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region | YENİ ONAY SÜRECİ |
                                    Product _product = returnList[0];
                                    decimal discountRate = preImage.Attributes.Contains("new_paymentplandiscountrate") ? (decimal)preImage["new_paymentplandiscountrate"] : decimal.MaxValue;
                                    decimal m2Price = preImage.Attributes.Contains("new_persquaremeter") ? (decimal)((Money)preImage["new_persquaremeter"]).Value : decimal.MaxValue;

                                    QuoteControlSetting control = ProductHelper.GetControlSettingByProject(_product.Project.Id, sda);
                                    if (control.QuoteControlSettingId != Guid.Empty)
                                    {

                                        if (m2Price != decimal.MaxValue)
                                        {
                                            if (m2Price < control.ConsultantUnitPrice && m2Price >= control.ManagerUnitPrice)
                                            {
                                                #region | QUOTE SEND CONFIRM PROCESS |
                                                MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisMuduru, sda, service, isIsGyo);
                                                if (!mailResult.Success)
                                                {
                                                    throw new Exception(mailResult.Result);
                                                }
                                                #endregion

                                                #region | SET PRODUCT STATUS |
                                                HomeOption hOpt = new HomeOption();
                                                hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                hOpt.AdministratorOption = false;
                                                hOpt.Product = new EntityReference("product", _product.ProductId);
                                                hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                if (!productStatusResult.Success)
                                                    throw new Exception(productStatusResult.Result);
                                                #endregion

                                                entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaya Düşme Nedeni
                                                entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                            }
                                            else if (m2Price < control.ManagerUnitPrice && m2Price >= control.DirectorUnitPrice)
                                            {
                                                #region | QUOTE SEND CONFIRM PROCESS |
                                                MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisDirektoru, sda, service, isIsGyo);
                                                if (!mailResult.Success)
                                                {
                                                    throw new Exception(mailResult.Result);
                                                }
                                                #endregion

                                                #region | SET PRODUCT STATUS |
                                                HomeOption hOpt = new HomeOption();
                                                hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                hOpt.AdministratorOption = false;
                                                hOpt.Product = new EntityReference("product", _product.ProductId);
                                                hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                if (!productStatusResult.Success)
                                                    throw new Exception(productStatusResult.Result);
                                                #endregion

                                                entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor); //Satış Durumu
                                                entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaya Düşme Nedeni
                                                entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                            }
                                            else if (m2Price < control.DirectorUnitPrice)
                                            {
                                                throw new Exception("Girmiş olduğunuzbirim metre kare satış fiyatı bu proje için çok düşük!");
                                            }
                                            else
                                            {
                                                if (discountRate != decimal.MaxValue)
                                                {
                                                    if (discountRate <= control.ConsultantRate)
                                                    {
                                                        entity["statuscode"] = new OptionSetValue((int)QuoteStatus.Onaylandı);
                                                        entity["new_approvaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaylanma Nedeni
                                                        entity["new_fallingapprovaltype"] = null; //Onaya Düşme Nedeni Boşaltılır

                                                        #region | SET SALES OFFICE |
                                                        MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                                                        if (lastActivityResult.Success)
                                                        {
                                                            Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                                            entity["new_salesofficeid"] = lastActivity.SalesOffice;
                                                        }
                                                        #endregion
                                                    }
                                                    else if (discountRate <= control.ManagerRate)
                                                    {
                                                        #region | QUOTE SEND CONFIRM PROCESS |
                                                        MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisMuduru, sda, service, isIsGyo);
                                                        if (!mailResult.Success)
                                                        {
                                                            throw new Exception(mailResult.Result);
                                                        }
                                                        #endregion

                                                        #region | SET PRODUCT STATUS |
                                                        HomeOption hOpt = new HomeOption();
                                                        hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                        hOpt.AdministratorOption = false;
                                                        hOpt.Product = new EntityReference("product", _product.ProductId);
                                                        hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                        //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                        MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                        if (!productStatusResult.Success)
                                                            throw new Exception(productStatusResult.Result);
                                                        #endregion

                                                        entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaya Düşme Nedeni
                                                        entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                                    }
                                                    else if (discountRate <= control.DirectorRate)
                                                    {
                                                        entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor);

                                                        #region | QUOTE SEND CONFIRM PROCESS |
                                                        MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisDirektoru, sda, service, isIsGyo);
                                                        if (!mailResult.Success)
                                                        {
                                                            throw new Exception(mailResult.Result);
                                                        }
                                                        #endregion

                                                        #region | SET PRODUCT STATUS |
                                                        HomeOption hOpt = new HomeOption();
                                                        hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                        hOpt.AdministratorOption = false;
                                                        hOpt.Product = new EntityReference("product", _product.ProductId);
                                                        hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                        //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                        MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                        if (!productStatusResult.Success)
                                                            throw new Exception(productStatusResult.Result);
                                                        #endregion

                                                        entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor); //Satış Durumu
                                                        entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaya Düşme Nedeni
                                                        entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Girmiş olduğunuz indirim oranı bu proje için çok fazla!");
                                                    }
                                                }
                                                else
                                                {
                                                    entity["statuscode"] = new OptionSetValue((int)QuoteStatus.Onaylandı);


                                                    #region | SET SALES OFFICE |
                                                    MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                                                    if (lastActivityResult.Success)
                                                    {
                                                        Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                                        entity["new_salesofficeid"] = lastActivity.SalesOffice;
                                                    }
                                                    #endregion

                                                    entity["new_approvaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaylanma Nedeni
                                                    entity["new_fallingapprovaltype"] = null; //Onaya Düşme Nedeni Boşaltılır
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                #region | YENİ ONAY SÜRECİ |
                                Product _product = returnList[0];
                                decimal discountRate = preImage.Attributes.Contains("new_paymentplandiscountrate") ? (decimal)preImage["new_paymentplandiscountrate"] : decimal.MaxValue;
                                decimal m2Price = preImage.Attributes.Contains("new_persquaremeter") ? (decimal)((Money)preImage["new_persquaremeter"]).Value : decimal.MaxValue;

                                QuoteControlSetting control = ProductHelper.GetControlSettingByProject(_product.Project.Id, sda);
                                if (control.QuoteControlSettingId != Guid.Empty)
                                {

                                    if (m2Price != decimal.MaxValue)
                                    {
                                        if (m2Price < control.ConsultantUnitPrice && m2Price >= control.ManagerUnitPrice)
                                        {
                                            #region | QUOTE SEND CONFIRM PROCESS |
                                            MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisMuduru, sda, service, isIsGyo);
                                            if (!mailResult.Success)
                                            {
                                                throw new Exception(mailResult.Result);
                                            }
                                            #endregion

                                            #region | SET PRODUCT STATUS |
                                            HomeOption hOpt = new HomeOption();
                                            hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                            hOpt.AdministratorOption = false;
                                            hOpt.Product = new EntityReference("product", _product.ProductId);
                                            hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                            //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                            MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                            if (!productStatusResult.Success)
                                                throw new Exception(productStatusResult.Result);
                                            #endregion

                                            entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaya Düşme Nedeni
                                            entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                        }
                                        else if (m2Price < control.ManagerUnitPrice && m2Price >= control.DirectorUnitPrice)
                                        {
                                            #region | QUOTE SEND CONFIRM PROCESS |
                                            MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisDirektoru, sda, service, isIsGyo);
                                            if (!mailResult.Success)
                                            {
                                                throw new Exception(mailResult.Result);
                                            }
                                            #endregion

                                            #region | SET PRODUCT STATUS |
                                            HomeOption hOpt = new HomeOption();
                                            hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                            hOpt.AdministratorOption = false;
                                            hOpt.Product = new EntityReference("product", _product.ProductId);
                                            hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                            //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                            MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                            if (!productStatusResult.Success)
                                                throw new Exception(productStatusResult.Result);
                                            #endregion

                                            entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor); //Satış Durumu
                                            entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaya Düşme Nedeni
                                            entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                        }
                                        else if (m2Price < control.DirectorUnitPrice)
                                        {
                                            throw new Exception("Girmiş olduğunuzbirim metre kare satış fiyatı bu proje için çok düşük!");
                                        }
                                        else
                                        {
                                            if (discountRate != decimal.MaxValue)
                                            {
                                                if (discountRate <= control.ConsultantRate)
                                                {
                                                    entity["statuscode"] = new OptionSetValue((int)QuoteStatus.Onaylandı);
                                                    entity["new_approvaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaylanma Nedeni
                                                    entity["new_fallingapprovaltype"] = null; //Onaya Düşme Nedeni Boşaltılır

                                                    #region | SET SALES OFFICE |
                                                    MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                                                    if (lastActivityResult.Success)
                                                    {
                                                        Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                                        entity["new_salesofficeid"] = lastActivity.SalesOffice;
                                                    }
                                                    #endregion
                                                }
                                                else if (discountRate <= control.ManagerRate)
                                                {
                                                    #region | QUOTE SEND CONFIRM PROCESS |
                                                    MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisMuduru, sda, service, isIsGyo);
                                                    if (!mailResult.Success)
                                                    {
                                                        throw new Exception(mailResult.Result);
                                                    }
                                                    #endregion

                                                    #region | SET PRODUCT STATUS |
                                                    HomeOption hOpt = new HomeOption();
                                                    hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                    hOpt.AdministratorOption = false;
                                                    hOpt.Product = new EntityReference("product", _product.ProductId);
                                                    hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                    //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                    MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                    if (!productStatusResult.Success)
                                                        throw new Exception(productStatusResult.Result);
                                                    #endregion

                                                    entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaya Düşme Nedeni
                                                    entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                                }
                                                else if (discountRate <= control.DirectorRate)
                                                {
                                                    entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor);

                                                    #region | QUOTE SEND CONFIRM PROCESS |
                                                    MsCrmResult mailResult = QuoteHelper.SendMailQuoteToApproval(entity.Id, UserTypes.SatisDirektoru, sda, service, isIsGyo);
                                                    if (!mailResult.Success)
                                                    {
                                                        throw new Exception(mailResult.Result);
                                                    }
                                                    #endregion

                                                    #region | SET PRODUCT STATUS |
                                                    HomeOption hOpt = new HomeOption();
                                                    hOpt.OptionDate = DateTime.Now.AddMinutes(20);
                                                    hOpt.AdministratorOption = false;
                                                    hOpt.Product = new EntityReference("product", _product.ProductId);
                                                    hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                                                    //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                                                    MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                                                    if (!productStatusResult.Success)
                                                        throw new Exception(productStatusResult.Result);
                                                    #endregion

                                                    entity["statuscode"] = new OptionSetValue((int)QuoteStatus.DirektorOnayiBekleniyor); //Satış Durumu
                                                    entity["new_fallingapprovaltype"] = new OptionSetValue((int)FallingApprovalTypes.SariExcel); //Onaya Düşme Nedeni
                                                    entity["new_approvaltype"] = null; //Onaylanma Nedeni Boşaltılır
                                                }
                                                else
                                                {
                                                    throw new Exception("Girmiş olduğunuz indirim oranı bu proje için çok fazla!");
                                                }
                                            }
                                            else
                                            {
                                                entity["statuscode"] = new OptionSetValue((int)QuoteStatus.Onaylandı);


                                                #region | SET SALES OFFICE |
                                                MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                                                if (lastActivityResult.Success)
                                                {
                                                    Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                                    entity["new_salesofficeid"] = lastActivity.SalesOffice;
                                                }
                                                #endregion

                                                entity["new_approvaltype"] = new OptionSetValue((int)FallingApprovalTypes.m2); //Onaylanma Nedeni
                                                entity["new_fallingapprovaltype"] = null; //Onaya Düşme Nedeni Boşaltılır
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else if (_quoteStatus == QuoteStatus.Onaylandı || _quoteStatus == QuoteStatus.Reddedildi)
                    {
                        #region | QUOTE CONFIRM/REFUSE PROCESS |

                        DateTime createdOn = (DateTime)preImage["createdon"];
                        EntityReference confirmUser = entity.Attributes.Contains("new_confirmuserid") && entity["new_confirmuserid"] != null ? (EntityReference)entity["new_confirmuserid"] : preImage.Attributes.Contains("new_confirmuserid") && preImage["new_confirmuserid"] != null ? (EntityReference)preImage["new_confirmuserid"] : null;
                        SystemUser su = null;

                        if (confirmUser != null && confirmUser.Id != Guid.Empty)
                        {
                            su = SystemUserHelper.GetSystemUserInfo(confirmUser.Id, sda);
                        }
                        DateTime salesDate = entity.Attributes.Contains("new_salesprocessdate") && entity["new_salesprocessdate"] != null ? (DateTime)entity["new_salesprocessdate"] : preImage.Attributes.Contains("new_salesprocessdate") && preImage["new_salesprocessdate"] != null ? (DateTime)preImage["new_salesprocessdate"] : DateTime.Now;

                        #region | SET SALES OFFICE |
                        if (_quoteStatus == QuoteStatus.Onaylandı)
                        {
                            MsCrmResultObject lastActivityResult = ActivityHelper.GetCustomerLastAppointment(customer.Id, sda);
                            if (lastActivityResult.Success)
                            {
                                Activity lastActivity = (Activity)lastActivityResult.ReturnObject;
                                entity["new_salesofficeid"] = lastActivity.SalesOffice;
                            }
                        }

                        #endregion

                        if (su != null)
                        {
                            MsCrmResult mailResult = QuoteHelper.SendMailUserApprovalResult(entity.Id, ownerId, ownerName, createdOn, customerName, su.FullName, salesDate, _quoteStatus, service, sda);
                            if (!mailResult.Success)
                            {
                                throw new Exception(mailResult.Result);
                            }
                        }
                        if (_quoteStatus == QuoteStatus.Reddedildi) //Satış reddedildi ise kapora ödemesi de etkin değil olarak kapatılır.
                        {
                            MsCrmResultObject paymentResult = PaymentHelper.GetQuotePrePayments(entity.Id, sda);
                            if (paymentResult.Success)
                            {
                                List<Payment> prePayments = (List<Payment>)paymentResult.ReturnObject;
                                for (int i = 0; i < prePayments.Count; i++)
                                {
                                    SetStateRequest setStateReq = new SetStateRequest();
                                    setStateReq.EntityMoniker = new EntityReference("new_payment", prePayments[i].PaymentId);

                                    setStateReq.State = new OptionSetValue(1);
                                    setStateReq.Status = new OptionSetValue(2);

                                    SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
                                }

                                if (secondCustomer != null)//satış onaylandığında 2.Kişi potansiyel müşteri yapılır
                                {
                                    Entity secondContactEntity = new Entity("contact");
                                    secondContactEntity.Id = secondCustomer.Id;
                                    secondContactEntity["new_customertype"] = new OptionSetValue((int)ContactTypes.Potansiyel);
                                    service.Update(secondContactEntity);
                                }
                            }
                            else
                            {
                                throw new Exception(paymentResult.Result);
                            }
                        }
                        #endregion

                        #region | CREATE USER FEED PROCESS |

                        UserFeed uFeed = new UserFeed();
                        uFeed.Name = "Satış Onay Durumu";
                        uFeed.FeedType = new StringMap()
                        {
                            Value = (_quoteStatus == QuoteStatus.Onaylandı ? 1 : 4),
                            Name = _quoteStatus.ToString()
                        };

                        uFeed.User = new EntityReference("systemuser", ownerId);
                        uFeed.Url = "editsale.html?quoteid=" + entity.Id;

                        uFeed.Description = customerName + " müşterisine ait satışınız " + (_quoteStatus == QuoteStatus.Onaylandı ? "onaylanmıştır." : "reddedilmiştir.");

                        FeedsHelper.CreateFeedEtity(uFeed, service);

                        #endregion

                        #region | CREATE OPTION RECORD |

                        if (_quoteStatus == QuoteStatus.Onaylandı)
                        {
                            MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(entity.Id, sda);

                            List<Product> returnList = (List<Product>)productResult.ReturnObject;
                            Product _product = returnList[0];

                            HomeOption hOpt = new HomeOption();
                            hOpt.OptionDate = DateTime.Now.AddMinutes(10);
                            hOpt.AdministratorOption = false;
                            hOpt.Product = new EntityReference("product", _product.ProductId);
                            hOpt.Owner = new EntityReference("systemuser", Globals.AdministratorId);

                            //MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(_product.ProductId, ProductStatuses.Opsiyonlu, service);
                            MsCrmResult productStatusResult = ProductHelper.UpdateProductOptionInfo(hOpt, service);
                            if (secondCustomer != null)//satış onaylandığında 2.Kişi gerçek müşteri yapılır
                            {
                                Entity secondContactEntity = new Entity("contact");
                                secondContactEntity.Id = secondCustomer.Id;
                                secondContactEntity["new_customertype"] = new OptionSetValue((int)ContactTypes.Gercek);
                                service.Update(secondContactEntity);
                            }
                            //HomeOption hOpt = new HomeOption();
                            //hOpt.Name = customerName;

                            //if (customer.LogicalName == "contact")
                            //{
                            //    hOpt.Contact = customer;
                            //}
                            //else
                            //{
                            //    hOpt.Account = customer;
                            //}

                            //hOpt.Product = new EntityReference("product", _product.ProductId);
                            //hOpt.OptionDate = DateTime.Now.AddMinutes(10);

                            //ProductHelper.CreateHomeOption(hOpt, service);
                        }

                        #endregion

                    }
                    else if (_quoteStatus == QuoteStatus.KaporaAlındı)
                    {
                        #region | FINANCIAL ACCOUNT PROCESS AND UPDATE PRODUCT PROCESS |
                        //Developed By Kemal Burak YILMAZ
                        if (customer != null) //Kapora alındı ise cari kaydı oluşturulur ve konutun durumu değiştirilir
                        {
                            #region | SET GROUP CODE |
                            if (customer.LogicalName == "contact")
                            {
                                if (!string.IsNullOrEmpty(customerName))
                                {
                                    char character = customerName[0];
                                    MsCrmResultObject groupCodeResult = ContactHelper.GetContactGroupCodeByCharacter(character, sda);
                                    if (groupCodeResult.Success)
                                    {
                                        int groupCode = (int)groupCodeResult.ReturnObject;
                                        groupCode++;
                                        string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                                        Entity entContact = new Entity("contact");
                                        entContact["contactid"] = customer.Id;
                                        entContact["new_groupcodenumber"] = groupCode;
                                        entContact["new_groupcodecharacter"] = character + "";
                                        entContact["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                                        service.Update(entContact);
                                    }
                                    else
                                    {
                                        throw new Exception(groupCodeResult.Result);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Kişi isminin baş harfi boş olamaz!");
                                }
                            }

                            if (customer.LogicalName == "account")
                            {
                                if (!string.IsNullOrEmpty(customerName))
                                {
                                    char character = customerName[0];
                                    MsCrmResultObject groupCodeResult = AccountHelper.GetAccountGroupCodeByCharacter(character, sda);
                                    if (groupCodeResult.Success)
                                    {
                                        int groupCode = (int)groupCodeResult.ReturnObject;
                                        groupCode++;
                                        string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                                        Entity entAccount = new Entity("account");
                                        entAccount["accountid"] = customer.Id;
                                        entAccount["new_groupcodenumber"] = groupCode;
                                        entAccount["new_groupcodecharacter"] = character + "";
                                        entAccount["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                                        service.Update(entAccount);
                                    }
                                    else
                                    {
                                        throw new Exception(groupCodeResult.Result);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Firma isminin baş harfi boş olamaz!");
                                }
                            }
                            #endregion

                            #region | FINANCIAL ACCOUNT PROCESS |
                            FinancialAccount financial = new FinancialAccount();
                            if (customer.LogicalName == "contact")
                                financial.Contact = customer;
                            else
                                financial.Account = customer;

                            financial.Name = customer.Name;

                            MsCrmResult financialAccountResult = FinancialAccountHelper.CreateOrUpdateFinancialAccount(financial, service);

                            if (financialAccountResult.Success) // Satış(Teklif) üzerindeki ilgili alana set edilir.
                            {
                                entity["new_financialaccountid"] = new EntityReference("new_financialaccount", financialAccountResult.CrmId);
                            }
                            else
                            {
                                throw new Exception(financialAccountResult.Result);
                            }
                            #endregion

                            #region | PRODUCT STATUS PROCESS |
                            //Kapora alındı ise konutun durumu ön satış yapıldı olarak değiştirilir.
                            MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(entity.Id, sda);
                            if (productResult.Success)
                            {
                                List<Product> returnList = (List<Product>)productResult.ReturnObject;
                                for (int i = 0; i < returnList.Count; i++)
                                {
                                    MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(returnList[i].ProductId, ProductStatuses.OnSatisYapildi, service);
                                    if (!productStatusResult.Success)
                                        throw new Exception(productStatusResult.Result);
                                }
                            }
                            #endregion

                            #region | PRODUCT UPDATE |
                            QuoteHelper.UpdateProduct(entity.Id, service, _quoteStatus);

                            #endregion


                        }
                        #endregion

                        //entity["new_yellowexcelstate"] = new OptionSetValue(1); //Yüklendi
                    }
                    else if (_quoteStatus == QuoteStatus.İptalEdildi)//Satış iptal ise ödeme kayıtları satış iptal olarak kapatılır.
                    {
                        #region | PAYMENT CLOSED |
                        PaymentHelper.UpdatePayments(entity.Id, service);
                        #endregion | PAYMENT CLOSED |

                        #region | CONTACT TYPE UPDATE |
                        if (customer.LogicalName == "contact")
                        {
                            MsCrmResult contactResult = ContactHelper.ContactHasMuhasebeAndImzaliSozlesme(entity.Id, customer.Id, sda);
                            Entity ent = new Entity("contact");
                            ent["contactid"] = customer.Id;
                            ent["new_customertype"] = contactResult.Success ? new OptionSetValue((int)ContactTypes.Gercek) : new OptionSetValue((int)ContactTypes.Potansiyel);
                            service.Update(ent);

                            if (secondCustomer != null)//satış onaylandığında 2.Kişi gerçek müşteri yapılır
                            {
                                Entity secondContactEntity = new Entity("contact");
                                secondContactEntity.Id = secondCustomer.Id;
                                secondContactEntity["new_customertype"] = new OptionSetValue((int)ContactTypes.Potansiyel);
                                service.Update(secondContactEntity);
                            }

                        }

                        #endregion

                        #region | PRODUCT STATUS PROCESS |

                        MsCrmResultObject productResult = QuoteHelper.GetSalesQuoteProducts(entity.Id, sda);
                        if (productResult.Success)
                        {
                            List<Product> returnList = (List<Product>)productResult.ReturnObject;
                            for (int i = 0; i < returnList.Count; i++)
                            {
                                MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(returnList[i].ProductId, ProductStatuses.Bos, service);
                                if (!productStatusResult.Success)
                                    throw new Exception(productStatusResult.Result);
                            }
                        }
                        #endregion

                        #region | CREATE USER FEED PROCESS |

                        UserFeed uFeed = new UserFeed();
                        uFeed.Name = "Satış İptal";
                        uFeed.FeedType = new StringMap()
                        {
                            Value = 2,
                            Name = _quoteStatus.ToString()
                        };

                        uFeed.User = new EntityReference("systemuser", ownerId);
                        uFeed.Url = "editsale.html?quoteid=" + entity.Id;

                        uFeed.Description = customerName + " müşterisine ait satış kaydınız iptal edilmiştir.";

                        FeedsHelper.CreateFeedEtity(uFeed, service);

                        #endregion

                        #region | PRODUCT UPDATE |
                        QuoteHelper.UpdateProduct(entity.Id, service, _quoteStatus);

                        #endregion

                        #region | Deactive Discount Protocols
                        DiscountProtocolHelper.SetStatusDeactiveDiscountProtocol(entity.Id, service, sda);
                        #endregion

                        if (context.Depth < 2 && _oldQuoteStatus == QuoteStatus.İptalAktarıldı)
                            QuoteHelper.SetLogoTransmission(entity, service);

                    }
                    else if (_quoteStatus == QuoteStatus.SozlesmeHazirlandi)
                    {
                        #region | CONTACT TYPE UPDATE |
                        if (customer.LogicalName == "contact")
                        {
                            Entity ent = new Entity("contact");
                            ent["contactid"] = customer.Id;
                            ent["new_customertype"] = new OptionSetValue((int)ContactTypes.Gercek);
                            service.Update(ent);
                        }
                        #endregion

                        #region | PRODUCT STATUS PROCESS |
                        MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(entity.Id, sda);
                        if (productResult.Success)
                        {
                            List<Product> returnList = (List<Product>)productResult.ReturnObject;
                            for (int i = 0; i < returnList.Count; i++)
                            {
                                MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(returnList[i].ProductId, ProductStatuses.Satildi, service);
                                if (!productStatusResult.Success)
                                    throw new Exception(productStatusResult.Result);
                            }
                        }
                        #endregion

                        #region | OPPORTUNITY CLOSED |
                        OpportunityHelper.OpportunityClosed(entity.Id, service);
                        #endregion | OPPORTUNITY CLOSED |

                        #region | UPDATE FINANCIAL ACCOUNT CODE |
                        if (financialAccount == null)
                        {
                            #region | SET GROUP CODE |
                            if (customer.LogicalName == "contact")
                            {
                                if (!string.IsNullOrEmpty(customerName))
                                {
                                    char character = customerName[0];
                                    MsCrmResultObject groupCodeResult = ContactHelper.GetContactGroupCodeByCharacter(character, sda);
                                    if (groupCodeResult.Success)
                                    {
                                        int groupCode = (int)groupCodeResult.ReturnObject;
                                        groupCode++;
                                        string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                                        Entity entContact = new Entity("contact");
                                        entContact["contactid"] = customer.Id;
                                        entContact["new_groupcodenumber"] = groupCode;
                                        entContact["new_groupcodecharacter"] = character + "";
                                        entContact["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                                        service.Update(entContact);
                                    }
                                    else
                                    {
                                        throw new Exception(groupCodeResult.Result);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Kişi isminin baş harfi boş olamaz!");
                                }
                            }

                            if (customer.LogicalName == "account")
                            {
                                if (!string.IsNullOrEmpty(customerName))
                                {
                                    char character = customerName[0];
                                    MsCrmResultObject groupCodeResult = AccountHelper.GetAccountGroupCodeByCharacter(character, sda);
                                    if (groupCodeResult.Success)
                                    {
                                        int groupCode = (int)groupCodeResult.ReturnObject;
                                        groupCode++;
                                        string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                                        Entity entAccount = new Entity("account");
                                        entAccount["accountid"] = customer.Id;
                                        entAccount["new_groupcodenumber"] = groupCode;
                                        entAccount["new_groupcodecharacter"] = character + "";
                                        entAccount["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                                        service.Update(entAccount);
                                    }
                                    else
                                    {
                                        throw new Exception(groupCodeResult.Result);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Firma isminin baş harfi boş olamaz!");
                                }
                            }
                            #endregion

                            #region | FINANCIAL ACCOUNT PROCESS |
                            if (!isRevized)
                            {
                                FinancialAccount financial = new FinancialAccount();
                                if (customer.LogicalName == "contact")
                                    financial.Contact = customer;
                                else
                                    financial.Account = customer;

                                financial.Name = customer.Name;

                                MsCrmResult financialAccountResult = FinancialAccountHelper.CreateOrUpdateFinancialAccount(financial, service);

                                if (financialAccountResult.Success) // Satış(Teklif) üzerindeki ilgili alana set edilir.
                                {
                                    entity["new_financialaccountid"] = new EntityReference("new_financialaccount", financialAccountResult.CrmId);
                                    financialAccount = new EntityReference("new_financialaccount", financialAccountResult.CrmId);
                                    PaymentHelper.UpdateFinancialAccount(entity.Id, financialAccountResult.CrmId, service);
                                }
                                else
                                {
                                    throw new Exception(financialAccountResult.Result);
                                }
                            }
                            #endregion
                        }

                        if (!isRevized)
                        {
                            if (!string.IsNullOrEmpty(customerName) && financialAccount != null && currency != null)
                            {
                                char character = customerName[0];
                                MsCrmResultObject fAccountResult = FinancialAccountHelper.GetFinancialAccountNumberByCharacter(character, sda);
                                if (fAccountResult.Success)
                                {
                                    int fAccountCode = (int)fAccountResult.ReturnObject;
                                    fAccountCode++;

                                    string fAccountCodeLeft = fAccountCode.ToString().PadLeft(5, '0');

                                    Entity fAccount = new Entity("new_financialaccount");
                                    fAccount["new_financialaccountid"] = financialAccount.Id;
                                    fAccount["new_financialaccountnumber"] = fAccountCode;
                                    fAccount["new_financialaccountcharacter"] = character + "";
                                    fAccount["new_name"] = "329CA20" + (currency.Name == "TL" ? "TRL" : (currency.Name == "Euro" ? "EUR" : currency.Name)) + character + fAccountCodeLeft;
                                    service.Update(fAccount);
                                }
                                else
                                {
                                    throw new Exception(fAccountResult.Result);
                                }
                            }
                            else
                            {
                                throw new Exception("Kişi isminin baş harfi, Cari kodu, Para Birimi boş olamaz!");
                            }
                        }


                        #endregion

                        #region | SÖZLEŞME NUMARASI OLUŞTUR |
                        string contractNumber = QuoteHelper.SetContractNumber(entity, service, sda);

                        #endregion | SÖZLEŞME NUMARASI OLUŞTUR |

                        #region | SENET NUMARASI OLUŞTUR |
                        PaymentHelper.SetVoucherNumber(entity.Id, contractNumber, service, sda);

                        #endregion | SENET NUMARASI OLUŞTUR |
                    }
                    else if (_quoteStatus == QuoteStatus.MuhasebeyeAktarıldı)
                    {
                        //SQL UPDATE YAPILDI. COMMON:SVC KULLANILARAK
                        //if (context.Depth < 2)
                        //    QuoteHelper.SetLogoTransmission(entity, service);

                        #region | UPDATE FINANCIAL ACCOUNT CODE -VOLKAN 08.03.2015 |

                        if (!string.IsNullOrEmpty(customerName) && financialAccount != null && !financialAccount.Name.Contains("329") && currency != null)
                        {
                            char character = customerName[0];
                            MsCrmResultObject fAccountResult = FinancialAccountHelper.GetFinancialAccountNumberByCharacter(character, sda);
                            if (fAccountResult.Success)
                            {
                                int fAccountCode = (int)fAccountResult.ReturnObject;
                                fAccountCode++;

                                string fAccountCodeLeft = fAccountCode.ToString().PadLeft(5, '0');

                                Entity fAccount = new Entity("new_financialaccount");
                                fAccount["new_financialaccountid"] = financialAccount.Id;
                                fAccount["new_financialaccountnumber"] = fAccountCode;
                                fAccount["new_financialaccountcharacter"] = character + "";
                                fAccount["new_name"] = "329CA20" + (currency.Name == "TL" ? "TRL" : (currency.Name == "Euro" ? "EUR" : currency.Name)) + character + fAccountCodeLeft;
                                service.Update(fAccount);
                            }
                            else
                            {
                                throw new Exception(fAccountResult.Result);
                            }
                        }
                        else
                        {
                            throw new Exception("Kişi isminin baş harfi, Cari kodu, Para Birimi boş olamaz!");
                        }

                        #endregion

                        #region | SET GROUP CODE |
                        if (customer.LogicalName == "contact")
                        {
                            if (!string.IsNullOrEmpty(customerName) && !ContactHelper.CheckContactHasGroupCode(customer.Id, sda).Success)
                            {
                                char character = customerName[0];
                                MsCrmResultObject groupCodeResult = ContactHelper.GetContactGroupCodeByCharacter(character, sda);
                                if (groupCodeResult.Success)
                                {
                                    int groupCode = (int)groupCodeResult.ReturnObject;
                                    groupCode++;
                                    string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                                    Entity entContact = new Entity("contact");
                                    entContact["contactid"] = customer.Id;
                                    entContact["new_groupcodenumber"] = groupCode;
                                    entContact["new_groupcodecharacter"] = character + "";
                                    entContact["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                                    service.Update(entContact);
                                }
                                else
                                {
                                    throw new Exception(groupCodeResult.Result);
                                }
                            }
                            else
                            {
                                throw new Exception("Kişi isminin baş harfi boş olamaz!");
                            }
                        }

                        if (customer.LogicalName == "account")
                        {
                            if (!string.IsNullOrEmpty(customerName) && !AccountHelper.CheckAccountHasGroupCode(customer.Id, sda).Success)
                            {
                                char character = customerName[0];
                                MsCrmResultObject groupCodeResult = AccountHelper.GetAccountGroupCodeByCharacter(character, sda);
                                if (groupCodeResult.Success)
                                {
                                    int groupCode = (int)groupCodeResult.ReturnObject;
                                    groupCode++;
                                    string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                                    Entity entAccount = new Entity("account");
                                    entAccount["accountid"] = customer.Id;
                                    entAccount["new_groupcodenumber"] = groupCode;
                                    entAccount["new_groupcodecharacter"] = character + "";
                                    entAccount["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                                    service.Update(entAccount);
                                }
                                else
                                {
                                    throw new Exception(groupCodeResult.Result);
                                }
                            }
                            else
                            {
                                throw new Exception("Firma isminin baş harfi boş olamaz!");
                            }
                        }
                        #endregion
                    }

                    else if (_quoteStatus == QuoteStatus.Düzeltilmiş)
                    {
                        if (context.Depth < 2 && _oldQuoteStatus == QuoteStatus.MuhasebeyeAktarıldı)
                            QuoteHelper.SetLogoTransmission(entity, service);
                    }
                    if (_quoteStatus != QuoteStatus.DevamEdiyor && _quoteStatus != QuoteStatus.İptalAktarıldı && _quoteStatus != QuoteStatus.İptalEdildi)
                    {
                        QuoteHelper.SetPreStatus(entity, service, _quoteStatus);
                    }

                    #region | QUOTE PRODUCT UPDATE |
                    if (_quoteStatus == QuoteStatus.OnayBekleniyor || _quoteStatus == QuoteStatus.Onaylandı || _quoteStatus == QuoteStatus.KaporaAlındı || _quoteStatus == QuoteStatus.DirektorOnayiBekleniyor)
                    {
                        QuoteHelper.UpdateQuoteProduct(entity.Id, service, true);
                    }
                    else if (_quoteStatus == QuoteStatus.Reddedildi || (_quoteStatus == QuoteStatus.DevamEdiyor))
                    {
                        QuoteHelper.UpdateQuoteProduct(entity.Id, service, false);
                    }
                    #endregion

                }
                #endregion

                if (entity.Contains("new_containstax"))
                {
                    if ((bool)entity["new_containstax"])
                    {
                        entity["new_taxamount"] = new Money(Convert.ToDecimal(0));
                    }
                }

                if (entity.Contains("new_isnotarizedsales") && context.Depth == 1)
                {
                    QuoteHelper.UpdateTaxOfStamp(entity, service);
                }
                if (entity.Contains("totalamount") && context.Depth == 1)
                {
                    QuoteHelper.UpdateTaxAmount(entity, service);
                }



                if (entity.Contains("new_salesprocessdate") || entity.Contains("transactioncurrencyid"))
                {
                    QuoteHelper.SetExchangeRateOnQuotePreUpdate(entity, preImage, service);
                }

                if (entity.Contains("new_secondcontactid"))
                {
                    #region | UPDATE QUOTE NAME|
                    ConditionExpression con1 = new ConditionExpression();
                    con1.AttributeName = "quoteid";
                    con1.Operator = ConditionOperator.Equal;
                    con1.Values.Add(entity.Id);

                    FilterExpression filter = new FilterExpression();
                    filter.FilterOperator = LogicalOperator.And;
                    filter.Conditions.Add(con1);

                    QueryExpression Query = new QueryExpression("quotedetail");
                    Query.ColumnSet = new ColumnSet("productid");
                    Query.Criteria.FilterOperator = LogicalOperator.And;
                    Query.Criteria.Filters.Add(filter);

                    EntityCollection Result = service.RetrieveMultiple(Query);
                    if (Result.Entities.Count > 0)
                    {
                        Entity product = service.Retrieve("product", ((EntityReference)Result.Entities[0].Attributes["productid"]).Id, new ColumnSet("name"));
                        entity["name"] = customer.Name + " - " + secondCustomer.Name + " - " + (product.Contains("name") ? (string)product.Attributes["name"] : string.Empty);
                    }
                    #endregion | UPDATE QUOTE NAME|
                }
                PaymentHelper.UpdatePaymentCustomers(entity.GetAttributeValue<Guid>("quoteid"), customer, service);

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
