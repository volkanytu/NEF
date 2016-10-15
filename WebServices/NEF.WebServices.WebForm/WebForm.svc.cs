using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NEF.WebServices.WebForm
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class WebForm : IWebForm
    {

        public MsCrmResult CreateWebForm(Webform form)
        {
            IOrganizationService service = MSCRM.AdminOrgService;
            MsCrmResult result = new MsCrmResult();
            Guid contactId = Guid.Empty;
            Guid channelOfAwarenessId = Guid.Empty;
            Guid subParticipationSourceId = Guid.Empty;
            Guid projectId = Guid.Empty;
            Guid utm_mediumid = Guid.Empty;
            Guid utm_campaignid = Guid.Empty;

            #region | FIND CONTACT |

            if (!string.IsNullOrEmpty(form.CustomerEmail))
            {
                contactId = GetContact("emailaddress1", form.CustomerEmail, service);
            }
            else
            {
                result.Success = false;
                result.Message = "Lütfen e-posta adresinizi yazınız";
                return result;
            }
            if (!string.IsNullOrEmpty(form.CustomerMobilePhone) && contactId == Guid.Empty)
            {
                contactId = GetContact("mobilephone", form.CustomerMobilePhone, service);
            }
            else if (string.IsNullOrEmpty(form.CustomerMobilePhone))
            {
                result.Success = false;
                result.Message = "Lütfen cep telefonu numaranızı yazınız";
                return result;
            }
            #endregion
            
            #region | FORMAT CHECK MOBILE |
            TelephoneNumber number = GeneralHelper.CheckTelephoneNumber(form.CustomerMobilePhone);
            if (!number.isFormatOK)
            {
                result.Success = false;
                result.Message = "Lütfen cep telefonu numaranızı geçerli formatta yazınız";
                return result;

            }

            #endregion

            #region | Haberdar olma kanalı|
            if (!string.IsNullOrEmpty(form.ChannelOfAwareness))
            {
                channelOfAwarenessId = GetChannelOfAwareness(form.ChannelOfAwareness, service);
            }
            else
            {
                result.Success = false;
                result.Message = "Lütfen haberdar olma kanalını seçiniz";
                return result;
            }
            #endregion

            #region | Alt Katılım Kaynağı |
            if (!string.IsNullOrEmpty(form.SubParticipationSource))
            {
                subParticipationSourceId = GetSubParticipationSource(form.SubParticipationSource, service);

            }
            #endregion

            #region | İletişim Tercihi |
            if (form.ContactPreferences == null || !form.ContactPreferences.HasValue)
            {
                result.Success = false;
                result.Message = "Lütfen iletişim tercihinizi seçiniz";
                return result;
            }
            #endregion

            #region | İlgilendiği Proje |
            if (!string.IsNullOrEmpty(form.InterestOfProjectCode))
            {
                projectId = GetProject(form.InterestOfProjectCode, service);

            }
            #endregion

            #region | UPDATE-CREATE CONTACT |
            if (contactId != Guid.Empty)
            {
                Entity contactRecord = service.Retrieve("contact", contactId, new ColumnSet(true));
                Entity c = new Entity("contact");
                c.Id = contactId;
                if (!contactRecord.Contains("new_subsourceofparticipationid"))//Yoksa Güncelle
                {
                    if (subParticipationSourceId != Guid.Empty)
                    {
                        c.Attributes["new_subsourceofparticipationid"] = new EntityReference("new_subsourceofparticipation", subParticipationSourceId);//Alt katılım kaynağı
                    }
                }
                if (!contactRecord.Contains("new_sourceofparticipationid"))//Yoksa Güncelle
                {
                    c.Attributes["new_sourceofparticipationid"] = new EntityReference("new_sourceofparticipation", new Guid(Globals.WebFormParticipationId));//Katılım kaynağı
                }
                if (!contactRecord.Contains("new_channelofawarenessid"))//Yoksa Güncelle
                {
                    c.Attributes["new_channelofawarenessid"] = new EntityReference("new_channelofawareness", channelOfAwarenessId);//Haber Olma Kanalı
                }

                if (!contactRecord.Contains("new_investmentrange"))
                {
                    if (form.CustomerInvestmentRange.HasValue)
                    {
                        if (form.CustomerInvestmentRange.Value == InvestmentRange.optionOne)
                        {
                            c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionOne);
                        }
                    }
                    if (form.CustomerInvestmentRange.HasValue)
                    {
                        if (form.CustomerInvestmentRange.Value == InvestmentRange.optionTwo)
                        {
                            c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionTwo);
                        }
                    }
                    if (form.CustomerInvestmentRange.HasValue)
                    {
                        if (form.CustomerInvestmentRange.Value == InvestmentRange.optionThree)
                        {
                            c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionThree);
                        }
                    }
                    if (form.CustomerInvestmentRange.HasValue)
                    {
                        if (form.CustomerInvestmentRange.Value == InvestmentRange.optionFour)
                        {
                            c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFour);
                        }
                    }
                    if (form.CustomerInvestmentRange.HasValue)
                    {
                        if (form.CustomerInvestmentRange.Value == InvestmentRange.optionFive)
                        {
                            c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFive);
                        }
                    }
                    if (form.CustomerInvestmentRange.HasValue)
                    {
                        if (form.CustomerInvestmentRange.Value == InvestmentRange.optionSix)
                        {
                            c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionSix);
                        }
                    }
                }

                if (!contactRecord.Contains("new_preferenceoffice"))
                {
                    if (form.FlatTypeChoose.HasValue)
                    {
                        if (form.FlatTypeChoose.Value == FlatType.optionFive)
                        {
                            c.Attributes["new_preferenceoffice"] = true;
                        }
                    }
                }

                if (!contactRecord.Contains("new_preferencestore"))
                {
                    if (form.FlatTypeChoose.HasValue)
                    {
                        if (form.FlatTypeChoose.Value == FlatType.optionSix)
                        {
                            c.Attributes["new_preferencestore"] = true;
                        }
                    }
                }

                if (!contactRecord.Contains("new_1plus1"))
                {
                    if (form.FlatTypeChoose.HasValue)
                    {
                        if (form.FlatTypeChoose.Value == FlatType.optionOne)
                        {
                            c.Attributes["new_1plus1"] = true;
                        }
                    }
                }

                if (!contactRecord.Contains("new_2plus1"))
                {
                    if (form.FlatTypeChoose.HasValue)
                    {
                        if (form.FlatTypeChoose.Value == FlatType.optionTwo)
                        {
                            c.Attributes["new_2plus1"] = true;
                        }
                    }
                }
                if (!contactRecord.Contains("new_3plus1"))
                {
                    if (form.FlatTypeChoose.HasValue)
                    {
                        if (form.FlatTypeChoose.Value == FlatType.optionThree)
                        {
                            c.Attributes["new_3plus1"] = true;
                        }
                    }
                }
                if (!contactRecord.Contains("new_4plus1"))
                {
                    if (form.FlatTypeChoose.HasValue)
                    {
                        if (form.FlatTypeChoose.Value == FlatType.optionFour)
                        {
                            c.Attributes["new_4plus1"] = true;
                        }
                    }
                }
                try
                {
                    service.Update(c);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;
                }

            }
            else if (contactId == Guid.Empty)
            {
                Entity c = new Entity("contact");
                if (string.IsNullOrEmpty(form.CustomerName))
                {
                    result.Success = false;
                    result.Message = "Lütfen adınızı yazınız";
                    return result;
                }
                if (string.IsNullOrEmpty(form.CustomerSurname))
                {
                    result.Success = false;
                    result.Message = "Lütfen soyadınızı yanız";
                    return result;
                }
                c.Attributes["firstname"] = form.CustomerName;
                c.Attributes["lastname"] = form.CustomerSurname;
                c.Attributes["mobilephone"] = form.CustomerMobilePhone;
                c.Attributes["emailaddress1"] = form.CustomerEmail;
                c.Attributes["new_countrycodemobilephone"] = number.countryCode;
                c.Attributes["new_cellphonenumber"] = number.phoneNo;
                c.Attributes["new_customertype"] = new OptionSetValue(100000002);//Potansiyel Müşteri

                if (form.CustomerInvestmentRange == InvestmentRange.optionOne)
                {
                    c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionOne);
                }
                if (form.CustomerInvestmentRange == InvestmentRange.optionTwo)
                {
                    c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionTwo);
                }
                if (form.CustomerInvestmentRange == InvestmentRange.optionThree)
                {
                    c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionThree);
                }
                if (form.CustomerInvestmentRange == InvestmentRange.optionFour)
                {
                    c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFour);
                }
                if (form.CustomerInvestmentRange == InvestmentRange.optionFive)
                {
                    c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFive);
                }
                if (form.CustomerInvestmentRange == InvestmentRange.optionSix)
                {
                    c.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionSix);
                }
                if (form.FlatTypeChoose == FlatType.optionFive)
                {
                    c.Attributes["new_preferenceoffice"] = true;
                }

                if (form.FlatTypeChoose == FlatType.optionSix)
                {
                    c.Attributes["new_preferencestore"] = true;
                }
                if (form.FlatTypeChoose == FlatType.optionOne)
                {
                    c.Attributes["new_1plus1"] = true;
                }
                if (form.FlatTypeChoose == FlatType.optionTwo)
                {
                    c.Attributes["new_2plus1"] = true;
                }
                if (form.FlatTypeChoose == FlatType.optionThree)
                {
                    c.Attributes["new_3plus1"] = true;
                }
                if (form.FlatTypeChoose == FlatType.optionFour)
                {
                    c.Attributes["new_4plus1"] = true;
                }
                if (subParticipationSourceId != Guid.Empty)
                {
                    c.Attributes["new_subsourceofparticipationid"] = new EntityReference("new_subsourceofparticipation", subParticipationSourceId);//Alt katılım kaynağı
                }
                c.Attributes["new_sourceofparticipationid"] = new EntityReference("new_sourceofparticipation", new Guid(Globals.WebFormParticipationId));//Katılım kaynağı
                c.Attributes["new_channelofawarenessid"] = new EntityReference("new_channelofawareness", channelOfAwarenessId);//Haber Olma Kanalı


                try
                {
                    c.Attributes["ownerid"] = new EntityReference("systemuser", new Guid(Globals.WebFormSytemUserId));//Web Form Sahibi
                    contactId = service.Create(c);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;
                }
            }

            #endregion

            #region | AssociateEntitiesRequest To Project |
            if (projectId != Guid.Empty && contactId != Guid.Empty)
            {
                try
                {
                    AssociateEntitiesRequest request = new AssociateEntitiesRequest();
                    request.Moniker1 = new EntityReference("contact", contactId);
                    request.Moniker2 = new EntityReference("new_project", projectId);
                    request.RelationshipName = "new_contact_new_project";
                    service.Execute(request);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            #endregion

            #region |Get UTMS|
            if (!string.IsNullOrEmpty(form.UTM_Campaign))
            {
                utm_campaignid = GetUtmCampaign("new_name", form.UTM_Campaign, service);
            }
            if (!string.IsNullOrEmpty(form.UTM_Medium))
            {
                utm_mediumid = GetUtmMedium("new_name", form.UTM_Medium, service);
            }
            #endregion

            #region | CREATE WEB FORM |
            if (contactId != Guid.Empty)
            {
                Entity w = new Entity("new_webform");
                w.Attributes["new_name"] = form.CustomerName + " " + form.CustomerSurname + " - " + DateTime.Now.ToString("dd/MM/yyyy");
                w.Attributes["new_firstname"] = form.CustomerName;
                w.Attributes["new_lastname"] = form.CustomerSurname;
                w.Attributes["new_mobilephone"] = form.CustomerMobilePhone;
                w.Attributes["new_emailadress"] = form.CustomerEmail;
                w.Attributes["new_sourceofparticipationid"] = new EntityReference("new_sourceofparticipation", new Guid(Globals.WebFormParticipationId));

                if (utm_mediumid != Guid.Empty)
                {
                    w.Attributes["new_utmmediumid"] = new EntityReference("new_utmmedium", utm_mediumid);
                }
                if (utm_campaignid != Guid.Empty)
                {
                    w.Attributes["new_utmcampaignid"] = new EntityReference("new_utmcampaign", utm_campaignid);
                }
                if (subParticipationSourceId != Guid.Empty)
                {
                    w.Attributes["new_subsourceofparticipationid"] = new EntityReference("new_subsourceofparticipation", subParticipationSourceId);
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionOne)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionOne);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionTwo)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionTwo);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionThree)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionThree);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionFour)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFour);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionFive)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFive);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionSix)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionSix);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionSeven)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionSeven);
                    }
                }
                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionEight)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionEight);
                    }
                }

                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionNine)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionNine);
                    }
                }

                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionTen)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionTen);
                    }
                }

                if (form.CustomerInvestmentRange.HasValue)
                {
                    if (form.CustomerInvestmentRange.Value == InvestmentRange.optionEleven)
                    {
                        w.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionEleven);
                    }
                }

                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionOne)
                    {
                        w.Attributes["new_1plus1"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionTwo)
                    {
                        w.Attributes["new_2plus1"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionThree)
                    {
                        w.Attributes["new_3plus1"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionFour)
                    {
                        w.Attributes["new_4plus1"] = true;
                    }
                }


                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionSeven)
                    {
                        w.Attributes["new_2plus1r"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionEight)
                    {
                        w.Attributes["new_3plus1r"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionNine)
                    {
                        w.Attributes["new_4plus1r"] = true;
                    }
                }

                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionTen)
                    {
                        w.Attributes["new_2plus1v"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionEleven)
                    {
                        w.Attributes["new_3plus1v"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionTwelve)
                    {
                        w.Attributes["new_4plus1v"] = true;
                    }
                }


                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionFive)
                    {
                        w.Attributes["new_preferenceoffice"] = true;
                    }
                }
                if (form.FlatTypeChoose.HasValue)
                {
                    if (form.FlatTypeChoose.Value == FlatType.optionSix)
                    {
                        w.Attributes["new_preferencestore"] = true;
                    }
                }
                w.Attributes["new_contactid"] = new EntityReference("contact", contactId);
                if (channelOfAwarenessId != Guid.Empty)
                {
                    w.Attributes["new_channelofawarenessid"] = new EntityReference("new_channelofawareness", channelOfAwarenessId);//Haber Olma Kanalı
                }
                if (form.ContactPreferences == ContactPreferences.Phone)
                {
                    w.Attributes["new_onphonecall"] = true;
                }
                else if (form.ContactPreferences == ContactPreferences.EMail)
                {
                    w.Attributes["new_isemailadress"] = true;
                }
                if (form.NefInformation)
                {
                    w.Attributes["new_istakeinfo"] = true;
                }
                if (!string.IsNullOrEmpty(form.Message))
                {
                    w.Attributes["new_message"] = form.Message;
                }
                if (projectId != Guid.Empty)
                {
                    w.Attributes["new_projectid"] = new EntityReference("new_project", projectId);
                }
                if (form.Location.HasValue)
                {
                    w.Attributes["new_location"] = new OptionSetValue((int)form.Location.Value);
                }
                if (form.ProcessType.HasValue)
                {
                    w.Attributes["new_processtype"] = new OptionSetValue((int)form.ProcessType.Value);
                }
                try
                {
                    w.Attributes["ownerid"] = new EntityReference("systemuser", new Guid(Globals.WebFormSytemUserId));//Web Form Sahibi
                    service.Create(w);
                    result.Success = true;
                    result.Message = "Kaydınız başarıyla alınmıştır. Teşekkür ederiz";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }

            }
            return result;

            #endregion

        }

        private Guid GetContact(string attributeName, string attributeValue, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = attributeName;
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(attributeValue);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("contact");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        private Guid GetUtmMedium(string attributeName, string attributeValue, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = attributeName;
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(attributeValue);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_utmmedium");
            Query.ColumnSet = new ColumnSet("new_utmmediumid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        private Guid GetUtmCampaign(string attributeName, string attributeValue, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = attributeName;
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(attributeValue);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_utmcampaign");
            Query.ColumnSet = new ColumnSet("new_utmcampaignid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        private Guid GetProject(string projectCode, IOrganizationService service)//İlgilendiği Proje
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_projectcode";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(projectCode);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("new_project");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        private Guid GetSubParticipationSource(string code, IOrganizationService service)//Alt katılım kaynağı bul
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_code";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(code);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_subsourceofparticipation");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        private Guid GetChannelOfAwareness(string code, IOrganizationService service)//haberdar olma kanalı bul
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_code";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(code);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_channelofawareness");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public void GetPriceRange()
        {

        }
    }
}
