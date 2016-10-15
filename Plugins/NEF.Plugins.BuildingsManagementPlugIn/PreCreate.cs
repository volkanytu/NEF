using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace NEF.Plugins.BuildingsManagementPlugIn
{
    public class PreCreate : IPlugin
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

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    BuildingsManagementHelper.CheckRecordDublicate(entity, sda, adminService);
                    BuildingsManagementHelper.SetNumber(entity, sda, adminService);
                    BuildingsManagementHelper.CreatePortal(entity, sda, adminService);


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
