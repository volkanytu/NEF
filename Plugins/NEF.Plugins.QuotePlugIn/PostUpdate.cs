using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.QuotePlugIn
{
    public class PostUpdate:IPlugin
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
            Entity entity = (Entity)context.InputParameters["Target"];
            if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
            {
                Entity postImage = (Entity)context.PostEntityImages["PostImage"];

                //if (context.Depth < 2)
                //{
                //    QuoteHelper.UpdateTaxAmount(postImage, adminService);
                //}
            }
           

        }
    }
}
