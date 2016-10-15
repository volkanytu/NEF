using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
//using NEF.Plugins.CampaignActivityPlugin.com.euromsg.auth.live;
//using NEF.Plugins.CampaignActivityPlugin.com.euromsg.campaign.live;
using NEF.Plugins.CampaignActivityPlugin.com.euromsg.ws.Auth;
using NEF.Plugins.CampaignActivityPlugin.com.euromsg.campaign;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace NEF.Plugins.CampaignActivityPlugin
{
    public class CampaignActivity
    {
        SqlDataAccess sda = new SqlDataAccess();
        Campaign campaign;

        public CampaignActivity()
        {
            campaign = new Campaign();
        }

        public void EuroMessage(Entity entity, IOrganizationService orgService)
        {
            try
            {
                if (entity.Contains("channeltypecode"))
                {
                    if (((OptionSetValue)entity["channeltypecode"]).Value == 7)//Mail Value
                    {
                        string authentication = AuthenticationEM(orgService);

                        if (!string.IsNullOrEmpty(authentication))
                            createCampaignActivityEM(entity, authentication, orgService);
                    }

                    //if (((OptionSetValue)entity["channeltypecode"]).Value == 3)//Sms Value
                    //{
                    //    string authentication = AuthenticationEM(orgService);

                    //    if (!string.IsNullOrEmpty(authentication))
                    //        createCampaignActivitySMS(entity, authentication, orgService);
                    //}
                    //else if (((OptionSetValue)entity["channeltypecode"]).Value == 7)//Mail Value
                    //{
                    //    string authentication = AuthenticationEM(orgService);

                    //    if (!string.IsNullOrEmpty(authentication))
                    //        createCampaignActivityEM(entity, authentication, orgService);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void createCampaignActivitySMS(Entity entity, string authentication, IOrganizationService service)
        {
            Guid campaignActivityId = entity.Id;
            EmSmsCampaign smsCampaign;
            OrganizationServiceContext orgcontext = new OrganizationServiceContext(service);
            int sendingType = 1;

            try
            {
                if (entity.Contains("new_sendingtype"))
                    sendingType = ((OptionSetValue)entity["new_sendingtype"]).Value;

                var query = (from r in orgcontext.CreateQuery("campaignactivity")
                             where (Guid)r["activityid"] == campaignActivityId
                             select new
                             {
                                 campaign = r.Attributes.Contains("regardingobjectid") ? r["regardingobjectid"] : new EntityReference("campaign", Guid.Empty),
                                 CampaignSubject = r.Attributes.Contains("subject") ? r["subject"] : string.Empty

                             }).FirstOrDefault();

                if (query != null)
                {
                    smsCampaign = new EmSmsCampaign();
                    smsCampaign.CampaignID = campaignActivityId.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();
                    smsCampaign.Name = query.CampaignSubject.ToString();
                    smsCampaign.CampaignType = EmSmsCampaignType.SingleShot;
                    smsCampaign.Originator = "EUROMSG";
                    smsCampaign.Locked = false;
                    smsCampaign.SmsMessage = entity.Contains("new_smstext") ? entity["new_smstext"].ToString() : string.Empty;
                    smsCampaign.AlternateSmsMessage = "";
                    smsCampaign.UniqueSmsFlag = true;
                    
                    EmCampaignFuncRes emResponse = campaign.CreateSmsCampaign(authentication, ref smsCampaign);

                    if (emResponse.Code != "00")
                    {
                        throw new ApplicationException(emResponse.Code + " -- " + emResponse.Message + " -- " + emResponse.DetailedMessage);
                    }

                    LogoutEM(authentication);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }

        private void createCampaignActivityEM(Entity entity, string authentication, IOrganizationService service)
        {
            Guid campaignActivityId = entity.Id;
            EmEmailCampaign eCampaign;
            OrganizationServiceContext orgcontext = new OrganizationServiceContext(service);
            int sendingType = 1;

            try
            {
                if (entity.Contains("new_sendingtype"))
                    sendingType = ((OptionSetValue)entity["new_sendingtype"]).Value;

                var query = (from r in orgcontext.CreateQuery("campaignactivity")
                             where (Guid)r["activityid"] == campaignActivityId
                             select new
                             {
                                 campaign = r.Attributes.Contains("regardingobjectid") ? r["regardingobjectid"] : new EntityReference("campaign", Guid.Empty),
                                 CampaignSubject = r.Attributes.Contains("subject") ? r["subject"] : string.Empty
                             }).FirstOrDefault();

                if (query != null)
                {
                    eCampaign = new EmEmailCampaign();

                    // Periyodik
                    if (sendingType == 2)
                    {
                        eCampaign.CampaignType = EmEmailCampaignType.SingleShot;
                        eCampaign.HtmlMessage = GetHtmlAttachment(Globals.BirthdayCampaignId, service);

                    }
                    else // Standart
                    {
                        eCampaign.CampaignType = EmEmailCampaignType.Template;
                        eCampaign.HtmlMessage = "<html><head></head><body></body></html>";
                    }

                    eCampaign.CampaignID = campaignActivityId.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();
                    eCampaign.ClickThroughFlag = true;
                    eCampaign.FromAddress = Globals.EuromsgFromAddressLive;
                    eCampaign.FromName = Globals.EuromsgFromNameLive;
                    eCampaign.GoogleAnalytics = false;
                    eCampaign.GoogleUtmCampaign = "";
                    eCampaign.GoogleUtmContent = "";
                    eCampaign.GoogleUtmMedium = "";
                    eCampaign.GoogleUtmSource = "";
                    eCampaign.GoogleUtmTerm = "";
                    eCampaign.Locked = false;
                    eCampaign.Name = ((EntityReference)query.campaign).Name;
                    eCampaign.OmnitureFlag = false;
                    eCampaign.RateFlag = false;
                    eCampaign.ReadFlag = true;
                    eCampaign.ReplyAddress = Globals.EuromsgReplyAddressLive;
                    eCampaign.Speed = 1;
                    eCampaign.Subject = new string[1];
                    eCampaign.Subject[0] = query.CampaignSubject.ToString();
                    eCampaign.UniqueEmailFlag = false;
                    eCampaign.ShareThisFlag = false;

                    EmCampaignFuncRes emResult = campaign.CreateEmailCampaign(authentication, ref eCampaign);

                    if (emResult.Code != "00") // Başarısız ise
                    {
                        if (emResult.Message == "Email message has a high spam score. This e-mail campaign can't be created." ||
                            emResult.Message == "Not a valid from name!")
                            throw new InvalidPluginExecutionException(emResult.Message);
                        else
                            throw new InvalidPluginExecutionException(emResult.Message + "\n" + emResult.DetailedMessage);
                    }

                    LogoutEM(authentication);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }

        public string AuthenticationEM(IOrganizationService service)
        {
            try
            {
                string AuthenticationServiceKey = string.Empty;
                Auth auth = new Auth();
                EmAuthResult authResult = auth.Login(Globals.EuroMessageUserNameLive, Globals.EuroMessagePasswordLive);
                if (authResult.Code == "00")
                {
                    AuthenticationServiceKey = authResult.ServiceTicket;
                    return AuthenticationServiceKey;
                }
                else
                {

                    throw new InvalidPluginExecutionException("authentication exception CODE" + authResult.Code + "authResult.Message =" + authResult.Message);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }

        public void LogoutEM(string AuthenticationServiceKey)
        {
            if (AuthenticationServiceKey != "")
            {
                Auth auth = new Auth();
                auth.Logout(AuthenticationServiceKey);
            }
        }

        public string GetHtmlAttachment(string stCampaignId, IOrganizationService orgService)
        {
            string htmlTemplate = string.Empty;
            Guid campaignId = new Guid(stCampaignId);

            QueryExpression getDocuments = new QueryExpression("annotation")
            {
                //ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("objectid", ConditionOperator.Equal, campaignId),
                        new ConditionExpression("isdocument", ConditionOperator.Equal, true)
                    }
                }
            };

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "objectid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(campaignId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "isdocument";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(true);



            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);


            QueryExpression Query = new QueryExpression("annotation");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = MSCRM.AdminOrgService.RetrieveMultiple(Query);



            var incidentDocumentList = orgService.RetrieveMultiple(getDocuments);

            foreach (var annotation in incidentDocumentList.Entities)
            {
                var documentBody = annotation.Contains("documentbody") ? (string)(annotation["documentbody"]) : string.Empty;
                var fileName = annotation.Contains("filename") ? (string)(annotation["filename"]) : string.Empty;

                htmlTemplate = Encoding.GetEncoding("windows-1254").GetString(Convert.FromBase64String(documentBody));
            }

            return htmlTemplate;
        }
    }
}
