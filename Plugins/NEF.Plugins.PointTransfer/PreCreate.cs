using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.PointTransferPlugin
{
    public class PreCreate : IPlugin
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

            if (!entity.Contains("new_name") || entity["new_name"] == null)
            {
                entity["new_name"] = string.Format("TRANSFER|{0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            }
        }
    }
}
