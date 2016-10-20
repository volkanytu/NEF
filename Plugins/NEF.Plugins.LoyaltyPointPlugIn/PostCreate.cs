using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.LoyaltyPointPlugIn
{
    public class PostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
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

            if (entity.Contains("new_pointtype") && entity["new_pointtype"] != null)
            {
                int pointType = ((OptionSetValue)entity["new_pointtype"]).Value;

                if (pointType == 2) //Harcama
                {
                    SendApprovalMail(entity, context.UserId, service);
                }
            }
        }

        private void SendApprovalMail(Entity entity, Guid senderId, IOrganizationService service)
        {
            #region | FROM |
            Entity fromParty = new Entity("activityparty");
            fromParty["partyid"] = new EntityReference("systemuser", senderId);
            Entity[] fromPartyColl = new Entity[] { fromParty };
            #endregion

            #region | SET TO |

            List<Entity> toPartyColl = new List<Entity>();

            Entity toParty = new Entity("activityparty");
            toParty["partyid"] = new EntityReference("systemuser", Globals.AlternatifDirectorSystemUserId);
            toPartyColl.Add(toParty);

            #endregion

            #region | BODY |
            string projectName = "---";
            string salesName = "---";
            string contactName = "---";

            Guid contactId = entity.Contains("new_contactid") && entity["new_contactid"] != null ? ((EntityReference)entity["new_contactid"]).Id : Guid.Empty;
            Guid projectId = entity.Contains("new_projectid") && entity["new_projectid"] != null ? ((EntityReference)entity["new_projectid"]).Id : Guid.Empty;
            Guid salesId = entity.Contains("new_quoteid") && entity["new_quoteid"] != null ? ((EntityReference)entity["new_quoteid"]).Id : Guid.Empty;

            if (contactId != Guid.Empty)
            {
                contactName = service.Retrieve("contact", contactId, new ColumnSet("fullname")).Attributes["fullname"].ToString();
            }

            if (projectId != Guid.Empty)
            {
                projectName = service.Retrieve("new_project", projectId, new ColumnSet("new_name")).Attributes["new_name"].ToString();
            }

            if (salesId != Guid.Empty)
            {
                salesName = service.Retrieve("quote", salesId, new ColumnSet("name")).Attributes["name"].ToString();
            }

            string amount = entity.Contains("new_amount") && entity["new_amount"] != null ? ((decimal)entity["new_amount"]).ToString() : string.Empty;
            string description = entity.Contains("new_description") && entity["new_description"] != null ? entity["new_description"].ToString() : string.Empty;

            string body = "<table>";
            body += "<tr><td>İlgili Kişi : </td><td>" + contactName + "</td></tr>";
            body += "<tr><td>Proje : </td><td>" + projectName + "</td></tr>";
            body += "<tr><td>İlgili Satış : </td><td>" + salesName + "</td></tr>";
            body += "<tr><td>Tutar : </td><td>" + amount + "</td></tr>";
            body += "<tr><td>Açıklama  : </td><td>" + description + "</td></tr>";
            body += "</table>";
            body += "<br/>";
            body += "<br/>";
            body += "<a href='{0}' target='_blank'>Puan kullanımını onaylamak/reddetmek için lütfen tıklayınız.</a>";

            string url = Globals.PortalUrl + "index.aspx?page=confirmpoint&name=pointid&pageid=" + entity.Id.ToString();
            body = string.Format(body, url);
            #endregion

            EmailHelper.SendMail(null, fromParty, toParty, "Puan Kullanım Onayı", body, service);
        }
    }
}
