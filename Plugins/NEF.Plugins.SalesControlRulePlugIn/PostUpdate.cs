using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.SalesControlRulePlugIn
{
    public class PostUpdate:IPlugin
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

                if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity &&
                   context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                {
                    Entity preImage = (Entity)context.PreEntityImages["PreImage"];
                    Entity postImage = (Entity)context.PostEntityImages["PostImage"];

                    if (context.Depth < 2)
                    {
                        SalesControlHelper.ChangeFieldsMail(preImage, postImage, adminService);
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
                {
                    sda.closeConnection();
                }

            }
        }
    }
}
