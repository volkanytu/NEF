using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.ProductPlugIn
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
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

            Entity preImage = null;
            if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
            {
                preImage = (Entity)context.PreEntityImages["PreImage"];
            }
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (context.Depth < 2)
                    ProductHelper.SetLogoTransmission(entity, adminService);

                if (entity.Contains("price"))
                {
                    ProductHelper.CreateHomePriceChanging(entity,preImage, adminService);
                }

            }

        }
    }
}
