using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NEF.Plugins.ContactPlugIn
{
    public class PostUpdate : IPlugin
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

                Guid contactId = Guid.Empty;
                EntityReference userGrantUpdateUser = null;

                OptionSetValue grantBeforeValue = null;
                OptionSetValue grantNewValue = null;

                contactId = entity.Id;

                #region | CREAE USER GRANT UPDATE HISTORY |

                if (entity.Contains("new_marketinggrant"))
                {

                    try
                    {
                        #region | SET USER GRANT UPDATE USER |

                        if (entity.Contains("new_grantupdatesystemuserid") && entity["new_grantupdatesystemuserid"] != null)
                        {
                            userGrantUpdateUser = (EntityReference)entity["new_grantupdatesystemuserid"];
                        }
                        else
                        {
                            userGrantUpdateUser = new EntityReference("systemuser", context.UserId);
                        }

                        #endregion


                        #region | SET USER GRANT BEFORE & NEW VALUE |

                        if (preImage.Contains("new_marketinggrant") && preImage["new_marketinggrant"] != null)
                        {
                            grantBeforeValue = (OptionSetValue)preImage["new_marketinggrant"];
                        }

                        if (entity.Contains("new_marketinggrant") && entity["new_marketinggrant"] != null)
                        {
                            grantNewValue = (OptionSetValue)entity["new_marketinggrant"];
                        }

                        #endregion

                        #region | CREATE HISTORY |

                        Entity entGrantHistory = new Entity("new_usergranthistory");
                        entGrantHistory["new_name"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                        entGrantHistory["new_systemuserid"] = userGrantUpdateUser;
                        entGrantHistory["new_contactid"] = entity.ToEntityReference();

                        if (grantBeforeValue != null)
                        {
                            entGrantHistory["new_previousvalue"] = grantBeforeValue;
                        }

                        if (grantNewValue != null)
                        {
                            entGrantHistory["new_newvalue"] = grantNewValue;
                        }

                        service.Create(entGrantHistory);

                        #endregion
                    }
                    catch (Exception ex)
                    {

                    }

                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
        }
    }
}
