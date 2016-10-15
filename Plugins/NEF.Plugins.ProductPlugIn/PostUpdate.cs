using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.Plugins.ProductPlugIn
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

                #region |DEFINE IMAGE IF EXISTS|
                Entity postImage = null;
                if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                {
                    postImage = (Entity)context.PostEntityImages["PostImage"];
                }
                #endregion

                #region | CREATE PRODUCT PRICE LEVEL |

                if (entity.Contains("price") && entity["price"] != null && postImage.Contains("transactioncurrencyid") && postImage["transactioncurrencyid"] != null && postImage.Contains("pricelevelid") && postImage["pricelevelid"] != null)
                {
                    MsCrmResult resultProductPriceList = ProductHelper.UpdateProductPriceLists(((Money)entity["price"]).Value, (EntityReference)postImage["transactioncurrencyid"], (EntityReference)postImage["pricelevelid"], entity.Id, sda, service);

                    if (!resultProductPriceList.Success)
                    {
                        throw new Exception(resultProductPriceList.Result);
                    }
                }

                #endregion

                #region | Set Per Square Meter |
                if (context.Depth < 2)
                {
                    ProductHelper.SetPerSquareMeter(postImage, service);
                }
                #endregion | Set Per Square Meter |

              

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
