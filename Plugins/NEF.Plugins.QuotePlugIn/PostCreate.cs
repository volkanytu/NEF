using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.QuotePlugIn
{
    public class PostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
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

            Entity entity = (Entity)context.InputParameters["Target"];

            #region | Satış Düzelt Denildiğinde ilgili Ödeme kayıtlarınıda kopyalar |
            if (context.ParentContext.InputParameters.Contains("QuoteId"))
            {
                Guid quoteId = (Guid)context.ParentContext.InputParameters["QuoteId"];
                QuoteHelper.SetPaymentForNewQuote(entity, quoteId, adminService);
                DiscountProtocolHelper.SetDiscountProtocolsForNewQuote(entity, quoteId, adminService);
                //QuoteHelper.SetAmountForNewQuote(entity, quoteId, adminService);
            }
            #endregion | Satış Düzelt Denildiğinde ilgili Ödeme kayıtlarınıda kopyalar |

            #region | Satış Üzerine Fırsatı Set Eder FIRSAT KAPATILIRKEN HATA VERİYOR. İLGİLİ TEKLİF HALA AÇIK OLDUĞU İÇİN. **KAPALI** |
            // QuoteHelper.SetOpportunity(entity, adminService);
            #endregion | Satış Üzerine Fırsatı Set Eder FIRSAT KAPATILIRKEN HATA VERİYOR. İLGİLİ TEKLİF HALA AÇIK OLDUĞU İÇİN. **KAPALI** |

            #region | Birim metrekare |
            QuoteHelper.CalculatePerSquareMeter(entity, adminService);
            #endregion | Birim metrekare |

            #region | Birim metrekare |
            if (entity.Contains("new_salesprocessdate") && entity.Contains("transactioncurrencyid") && !context.ParentContext.InputParameters.Contains("QuoteId"))
            {
                QuoteHelper.SetExchangeRateOnQuotePostCreate(entity, adminService);
            }
            #endregion
        }
    }
}
