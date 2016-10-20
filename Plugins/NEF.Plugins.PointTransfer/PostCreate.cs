using Microsoft.Xrm.Sdk;
using NEF.Library.Entities.CrmEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.PointTransferPlugin
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

            if (entity.Contains(PointTransfer.SOURCE_CONTACT_ID) && entity[PointTransfer.SOURCE_CONTACT_ID] != null)
            {
                var erSourceContact = (EntityReference)entity[PointTransfer.SOURCE_CONTACT_ID];
            }

            if (entity.Contains(PointTransfer.TARGET_CONTACT_ID) && entity[PointTransfer.TARGET_CONTACT_ID] != null)
            {
                var erTargetContact = (EntityReference)entity[PointTransfer.TARGET_CONTACT_ID];
            }
        }
    }
}
