using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.HomeOption
{
    public class PostState : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {
                #region | SERVICE |
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                #region | Validate Request |
                //Target yoksa veya Entity tipinde değilse, devam etme.
                if (!context.InputParameters.Contains("EntityMoniker") || !(context.InputParameters["EntityMoniker"] is EntityReference))
                {
                    return;
                }
                #endregion


                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


                Guid homeOptionId = ((EntityReference)context.InputParameters["EntityMoniker"]).Id;
                Entity entity = service.Retrieve("new_optionofhome", homeOptionId, new ColumnSet(true));
                int stateCode = ((OptionSetValue)entity["statecode"]).Value;
                int statusCode = ((OptionSetValue)entity["statuscode"]).Value;

                if (entity.Attributes.Contains("new_productid") && entity.Attributes["new_productid"] != null)
                {
                    if (stateCode == 1) //Etkin Değil
                    {
                        if (statusCode == 2) //İptal
                        {
                            Entity proc = new Entity("product");
                            proc["productid"] = ((EntityReference)entity["new_productid"]).Id;
                            proc["statuscode"] = new OptionSetValue((int)ProductStatuses.Bos);
                            proc["new_optionlastvaliditydate"] = null;

                            service.Update(proc);
                        }
                    }
                }
             
                #endregion


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
