using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NEF.Plugins.AccountPlugIn
{
    public class PreCreate : IPlugin
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
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                {
                    return;
                }
                #endregion

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                #endregion

                Entity entity = (Entity)context.InputParameters["Target"];

                #region | CHECK DUPLICATE |
                if (entity.Attributes.Contains("name") && entity["name"] != null)
                {
                    if (entity["name"] != null)
                    {
                        
                        string name = GeneralHelper.ToTitleCase(entity["name"].ToString());

                        MsCrmResult nameResult = AccountHelper.CheckDuplicateName(name, sda);
                        if (!nameResult.Success)
                            throw new Exception(nameResult.Result);

                        entity["name"] = name;
                    }
                }

                if (entity.Attributes.Contains("new_taxnumber") && entity["new_taxnumber"] != null)
                {
                    string taxNumber = entity["new_taxnumber"].ToString();

                    MsCrmResult taxResult = AccountHelper.CheckDuplicateTaxNumber(taxNumber, sda);
                    if (!taxResult.Success)
                        throw new Exception(taxResult.Result);
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
                    sda.closeConnection();
            }
        }
    }
}
