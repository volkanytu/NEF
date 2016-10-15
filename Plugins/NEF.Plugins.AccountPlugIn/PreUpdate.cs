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
    public class PreUpdate : IPlugin
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

                #region |DEFINE IMAGE IF EXISTS|
                Entity preImage = null;
                if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
                {
                    preImage = (Entity)context.PreEntityImages["PreImage"];
                }
                #endregion

                #endregion

                Entity entity = (Entity)context.InputParameters["Target"];

                #region | CHECK DUPLICATE |
                if (entity.Attributes.Contains("name"))
                {
                    if (entity["name"] != null)
                    {
                        string oldName = preImage.Attributes.Contains("name") ? preImage["name"].ToString() : string.Empty;
                        string name = GeneralHelper.ToTitleCase(entity["name"].ToString());

                        if (name != oldName)
                        {
                            MsCrmResult nameResult = AccountHelper.CheckDuplicateName(name, sda);
                            if (!nameResult.Success)
                                throw new Exception(nameResult.Result);

                            entity["name"] = name;
                        }
                    }
                }

                if (entity.Attributes.Contains("new_taxnumber"))
                {
                    if (entity["new_taxnumber"] != null)
                    {
                        string oldTaxNumber = preImage.Attributes.Contains("new_taxnumber") ? preImage["new_taxnumber"].ToString() : string.Empty;
                        string taxNumber = entity["new_taxnumber"].ToString();

                        if (taxNumber != oldTaxNumber)
                        {
                            MsCrmResult taxResult = AccountHelper.CheckDuplicateTaxNumber(taxNumber, sda);
                            if (!taxResult.Success)
                                throw new Exception(taxResult.Result);   
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
                    sda.closeConnection();
            }
        }
    }
}
