using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using NEF.Library.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace NEF.Plugins.AppointmentPlugIn
{
    public class PreState : IPlugin
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
                if (!context.InputParameters.Contains("EntityMoniker") || !(context.InputParameters["EntityMoniker"] is EntityReference))
                {
                    return;
                }
                #endregion

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                Guid appointmentId = ((EntityReference)context.InputParameters["EntityMoniker"]).Id;
                Entity postImage = service.Retrieve("appointment", appointmentId, new ColumnSet(true));
                #endregion

                #region | RANDEVU SONUCU İLGİLİ İSE YAPILACAK İŞLEMLER |
                if (!postImage.Attributes.Contains("requiredattendees") && postImage["requiredattendees"] != null)
                {
                    throw new Exception("Lütfen randevu kişi bilgisini doldurunuz!");
                }

                EntityReference customer = (EntityReference)((EntityCollection)(postImage["requiredattendees"])).Entities[0]["partyid"];


                if (postImage.Attributes.Contains("new_activitystatus") && postImage["new_activitystatus"] != null)
                {
                    MsCrmResultObject activityStatusResult = ActivityHelper.GetActivityStatusDetail(((EntityReference)postImage["new_activitystatus"]).Id, sda);
                    if (activityStatusResult.Success)
                    {
                        ActivityStatus activityStatus = (ActivityStatus)activityStatusResult.ReturnObject;

                        if (activityStatus.ActivityStatusCode == ActivityStatusCodes.Ilgili)
                        {
                            bool hasOpenOpportunity = false;
                            Guid ownerId = ((EntityReference)postImage["ownerid"]).Id;
                            Guid opportunityId = Guid.Empty;

                            #region | OPPORTUNITY PROCESS |
                            MsCrmResult opportunityControlResult = OpportunityHelper.HasOpenOpportunityToSalesConsultantAndContact(customer.Id, ownerId, sda);
                            if (opportunityControlResult.Success)
                            {
                                hasOpenOpportunity = true;
                                opportunityId = opportunityControlResult.CrmId;

                                Entity oppEnt = new Entity("opportunity");
                                oppEnt["opportunityid"] = opportunityId;
                                oppEnt["new_relatedactivitystatusid"] = (EntityReference)postImage["new_activitystatusdetail"];
                                service.Update(oppEnt);
                            }
                            else
                            {

                                Opportunity opportunity = new Opportunity();
                                opportunity.Contact = customer;
                                opportunity.ActivityStatusDetail = (EntityReference)postImage["new_activitystatusdetail"];
                                opportunity.Owner = ((EntityReference)postImage["ownerid"]);

                                MsCrmResult opportunityResult = OpportunityHelper.CreateOrUpdateOpportunity(opportunity, service);
                                if (opportunityResult.Success)
                                {
                                    opportunityId = opportunityResult.CrmId;
                                }
                                else
                                {
                                    throw new Exception(opportunityResult.Result);
                                }
                            }
                            #endregion

                            #region | SEY ACTIVITY REGARDING OBJECT AS OPPORTUNITY |
                            Entity activity = new Entity("appointment");
                            activity.Id = postImage.Id;
                            activity["regardingobjectid"] = new EntityReference("opportunity", opportunityId);
                            service.Update(activity);
                            #endregion

                            #region | OPPORTUNITY PRODUCT PROCESS |
                            MsCrmResultObject activityProductsResult = InterestProductHelper.GetActivityInterestedProjects(postImage.Id, sda);

                            if (activityProductsResult.Success)
                            {
                                List<InterestProduct> relatedProducts = (List<InterestProduct>)activityProductsResult.ReturnObject;

                                #region | FIRSATIN FİYAT LİSTESİ ÜRÜNÜN FİYAT LİSTESİ İLE SET EDİLİR |
                                Entity ent = new Entity("opportunity");
                                ent.Id = opportunityId;
                                ent["pricelevelid"] = relatedProducts[0].InterestedProduct.PriceList;
                                service.Update(ent);
                                #endregion

                                for (int i = 0; i < relatedProducts.Count; i++)
                                {
                                    Product product = relatedProducts[i].InterestedProduct;
                                    if (hasOpenOpportunity)
                                    {
                                        MsCrmResult hasProductAddedOppProc = OpportunityHelper.HasOpportunityProduct(opportunityId, product.ProductId, sda);
                                        if (!hasProductAddedOppProc.Success)
                                        {
                                            OpportunityHelper.CreateOpportunityProduct(opportunityId, product, service);
                                        }
                                    }
                                    else
                                    {
                                        OpportunityHelper.CreateOpportunityProduct(opportunityId, product, service);
                                    }
                                }

                            }
                            #endregion
                        }
                    }
                    else
                    {
                        throw new Exception(activityStatusResult.Result);
                    }
                }

                #endregion

                #region | SAHİPLİK İLE İLGİLİ İŞLEMLER |
                //Eğer kişi ve firma resepsiyonist tarafından oluşturulmuş ise, ilk aktivenin sahibi atanır.
                Entity ownerEntity = service.Retrieve(customer.LogicalName, customer.Id, new ColumnSet("ownerid"));

                EntityReference owner = (EntityReference)ownerEntity["ownerid"];

                SystemUser su = SystemUserHelper.GetSystemUserInfo(owner.Id, sda);
                if (su.SystemUserId != null)
                {
                    if (su.UserType == UserTypes.Resepsiyonist)
                    {

                        AssignRequest assign = new AssignRequest
                        {
                            Assignee = (EntityReference)postImage["ownerid"],
                            Target = customer
                        };


                        // Execute the Request
                        service.Execute(assign);
                    }
                }

                #endregion

                #region | WEBFORM KAYDI VAR İSE YAPILACAKLAR |

                //Birden fazla web form kaydı var ise 1. si "Tamamlandı - Temas Kuruldu" olarak diğerleri "Tamamlandı - Başka Bir Temas Nedeniyle" olarak kapatılır.
                MsCrmResultObject webFormResult = WebFormHelper.GetContactWebForms(customer.Id, sda);
                if (webFormResult.Success && webFormResult.ReturnObject != null)
                {
                    List<WebForm> webFormList = (List<WebForm>)webFormResult.ReturnObject;

                    if (webFormList.Count > 0)
                    {
                        WebForm firstForm = webFormList[0];
                        WebFormHelper.WebFormClose(firstForm.WebFormId, 1, 2, service); //Tamamlandı - Temas Kuruldu

                        if (webFormList.Count > 1)
                        {
                            for (int i = 0; i < webFormList.Count; i++)
                            {
                                WebForm form = webFormList[i];
                                WebFormHelper.WebFormClose(form.WebFormId, 1, 100000000, service); //Tamamlandı - Başka Bir Temas Nedeniyle
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

        }
    }
}
