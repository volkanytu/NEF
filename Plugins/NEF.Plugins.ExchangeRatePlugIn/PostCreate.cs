using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.Plugins.ExchangeRatePlugIn
{
    public class PostCreate : IPlugin
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

                #region | UPDATE PRICES |

                EntityReference currencyId = null;
                decimal exRate = 1;

                if (entity.Contains("new_currencyid") && entity["new_currencyid"] != null && entity.Contains("new_salesrate") && entity["new_salesrate"] != null)
                {
                    currencyId = (EntityReference)entity["new_currencyid"];
                    exRate = (decimal)entity["new_salesrate"];

                    MsCrmResult resultUpdatePrice = CurrencyHelper.UpdateProductPriceLevelPricesByExchange(currencyId.Id, Globals.CurrencyIdTL, exRate, true, sda);

                    if (resultUpdatePrice.Success)
                    {
                        resultUpdatePrice = CurrencyHelper.UpdateProductPriceLevelPricesByExchange(Globals.CurrencyIdTL, currencyId.Id, exRate, false, sda);

                        if (!resultUpdatePrice.Success)
                        {
                            throw new Exception(resultUpdatePrice.Result);
                        }
                    }
                    else
                    {
                        throw new Exception(resultUpdatePrice.Result);
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
