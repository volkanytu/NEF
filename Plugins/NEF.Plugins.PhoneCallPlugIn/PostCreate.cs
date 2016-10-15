using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.Plugins.PhoneCallPlugIn
{
    public class PostCreate : IPlugin
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

                #region | ADD PHONECALL QUEUE |

                Guid ownerId = entity.Attributes.Contains("ownerid") ? ((EntityReference)entity["ownerid"]).Id : Guid.Empty;

              
                if (ownerId != Guid.Empty)
                {
                    SystemUser user = SystemUserHelper.GetSystemUserInfo(ownerId, sda);
                    if (user.SystemUserId != Guid.Empty)
                    {
                        if (user.UserType != null && user.UserType == UserTypes.CallCenter) //Telefon görüşmesinin sahibi Call Center kullanıcısı ise
                        {
                            MsCrmResultObject callCenterQueueResult = GeneralHelper.GetQueues(QueueTypes.CallCenter, sda);
                            if (callCenterQueueResult.Success)
                            {
                                Queue callCenterQueue = ((List<Queue>)callCenterQueueResult.ReturnObject)[0];

                                MsCrmResult phoneQueueResult = ActivityHelper.AddPhoneCallToQueue(entity.Id, callCenterQueue.QueueId, service);

                                if (!phoneQueueResult.Success)
                                {
                                    throw new Exception(phoneQueueResult.Result);
                                }

                            }
                        }
                    }
                }
                if (entity.Attributes.Contains("regardingobjecttypecode"))
                {

                    
                }

                #endregion

                #region | ADD PHONECALL QUEUE | Kuyruk lookup'ında hangi kuyruk seçilirse create anında kaydı otomatik olarak o kuyruğa atanır.

                Guid queueId = entity.Attributes.Contains("new_queueid") ? ((EntityReference)entity["new_queueid"]).Id : Guid.Empty;
                if (queueId != Guid.Empty)
                {
                    MsCrmResult phoneQueueResult = ActivityHelper.AddPhoneCallToQueue(entity.Id, queueId, service);

                    if (!phoneQueueResult.Success)
                    {
                        throw new Exception(phoneQueueResult.Result);
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
