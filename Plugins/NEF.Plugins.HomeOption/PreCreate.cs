using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.HomeOption
{
    public class PreCreate : IPlugin
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
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                #endregion

                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.Attributes.Contains("new_productid"))
                {
                    EntityReference createdUser = (EntityReference)entity["ownerid"];
                    EntityReference proc = (EntityReference)entity["new_productid"];
                    DateTime optionDate = (DateTime)entity["new_optiondate"];

                    TimeSpan fark = optionDate - DateTime.Now;

                    Product product = ProductHelper.GetProductDetail(proc.Id, sda);

                    if (product.StatusCode.Value != (int)ProductStatuses.Bos)
                    {
                        throw new Exception("Opsiyon ekleyebilme için konutun durumu BOŞ olmalıdır.");
                    }

                    if (product.Project != null)
                    {
                        QuoteControlSetting control = ProductHelper.GetControlSettingByProject(product.Project.Id, sda);
                        if (control.QuoteControlSettingId != Guid.Empty)
                        {
                            SystemUser su = SystemUserHelper.GetSystemUserInfo(createdUser.Id, sda);

                            if (su.UserType == UserTypes.SatisDanismani)
                            {
                                if (fark.Days > control.ConsultantOptionDay)
                                    throw new Exception("En fazla " + control.ConsultantOptionDay + " gün opsiyonlayabilirsiniz!");
                                else
                                {
                                    #region | SET PRODUCT STATUS |
                                    MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(product.ProductId, ProductStatuses.YoneticiOpsiyonlu, service, optionDate);
                                    if (!productStatusResult.Success)
                                        throw new Exception(productStatusResult.Result);
                                    #endregion
                                }
                            }
                            else if (su.UserType == UserTypes.SatisMuduru || su.UserType == UserTypes.IsGyoSatisMuduru)
                            {
                                if (fark.Days > control.ManagerOptionDay)
                                    throw new Exception("En fazla " + control.ManagerOptionDay + " gün opsiyonlayabilirsiniz!");
                                else
                                {
                                    #region | SET PRODUCT STATUS |
                                    MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(product.ProductId, ProductStatuses.YoneticiOpsiyonlu, service, optionDate);
                                    if (!productStatusResult.Success)
                                        throw new Exception(productStatusResult.Result);
                                    #endregion
                                }
                            }
                            else if (su.UserType == UserTypes.SatisDirektoru)
                            {
                                if (fark.Days > control.DirectorOptionDay)
                                    throw new Exception("En fazla " + control.DirectorOptionDay + " gün opsiyonlayabilirsiniz!");
                                else
                                {
                                    #region | SET PRODUCT STATUS |
                                    MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(product.ProductId, ProductStatuses.YoneticiOpsiyonlu, service, optionDate);
                                    if (!productStatusResult.Success)
                                        throw new Exception(productStatusResult.Result);
                                    #endregion
                                }
                            }
                            else
                            {
                                throw new Exception("Konut opsiyonlama yetkiniz bulunmamaktadır!");
                            }
                        }
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
