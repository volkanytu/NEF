using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.LoyaltyPointPlugIn
{
    public class PostUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

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

                if (entity.Attributes.Contains("statuscode") && entity["statuscode"] != null)
                {
                    if (((OptionSetValue)entity["statuscode"]).Value == 3) //CONFIRMED ise
                    {
                        //TODO: Mail Gönder
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
                    sda.closeConnection();
            }
        }
    }
}
