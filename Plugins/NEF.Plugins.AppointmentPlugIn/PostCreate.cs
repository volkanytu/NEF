using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using System.Net.Sockets;
using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.Plugins.NotificationPlugIn
{
    public class PostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //SqlDataAccess sda = null;

            //sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);

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

                EntityReference owner = (EntityReference)entity["ownerid"];
                EntityReference customer = (EntityReference)((EntityCollection)(entity["requiredattendees"])).Entities[0]["partyid"];

                #region | CREATE USER FEED PROCESS |

                UserFeed uFeed = new UserFeed();
                uFeed.Name = "Yeni Randevu";
                uFeed.FeedType = new StringMap()
                {
                    Value = 2,
                    Name = "info"
                };

                uFeed.User = owner;

                if (customer.LogicalName == "contact")
                    uFeed.Url = "editcontact.html?contactid=" + customer.Id.ToString() + "&appointmentid=" + entity.Id.ToString();
                else
                    uFeed.Url = "editaccount.html?accountid=" + customer.Id.ToString() + "&appointmentid=" + entity.Id.ToString();

                uFeed.Description = customer.Name + " müşterisine ait bir randevu kaydı tarafınıza atanmıştır.";

                FeedsHelper.CreateFeedEtity(uFeed, service);

                #endregion

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            finally
            {
                //if (sda != null)
                //sda.closeConnection();
            }
        }
    }
}
