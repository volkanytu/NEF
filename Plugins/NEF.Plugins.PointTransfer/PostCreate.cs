using Microsoft.Xrm.Sdk;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.IocManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.DataLibrary.SqlDataLayer;

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

            EntityReference erSourceContact = null;
            EntityReference erTargetContact = null;

            EntityReferenceWrapper erPointTransfer = entity.ToEntityReference().ToEntityReferenceWrapper();

            Initializer.Init(service);

            if (entity.Contains(PointTransfer.SOURCE_CONTACT_ID) && entity[PointTransfer.SOURCE_CONTACT_ID] != null)
            {
                erSourceContact = (EntityReference)entity[PointTransfer.SOURCE_CONTACT_ID];
            }

            if (entity.Contains(PointTransfer.TARGET_CONTACT_ID) && entity[PointTransfer.TARGET_CONTACT_ID] != null)
            {
                erTargetContact = (EntityReference)entity[PointTransfer.TARGET_CONTACT_ID];
            }

            if (erSourceContact != null && erTargetContact != null)
            {
                Initializer.LoyatyPointBusiness.TransferPoints(erSourceContact.Id, erTargetContact.Id, erPointTransfer);
            }
        }
    }
}
