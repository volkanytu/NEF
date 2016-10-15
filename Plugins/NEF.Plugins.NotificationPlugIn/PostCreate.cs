using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using System.Net.Sockets;
namespace NEF.Plugins.NotificationPlugIn
{
    public class PostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //SqlDataAccess sda = null;

            //sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);

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

                #region | NODE TRIGGER |

                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect("fenix.nef.com.tr", 8000);
                NetworkStream clientStream = tcpClient.GetStream();

                byte[] message = new byte[4096];
                int bytesRead;
                bytesRead = 0;

                try
                {
                    // Read up to 4096 bytes
                    //bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    /*a socket error has occured*/
                }

                //We have read the message.
                ASCIIEncoding encoder = new ASCIIEncoding();
                Console.WriteLine(encoder.GetString(message, 0, bytesRead));

                tcpClient.Close();

                #endregion

            }
            catch (Exception ex)
            {
                //throw new InvalidPluginExecutionException(ex.Message);
            }
            finally
            {
                //if (sda != null)
                //sda.closeConnection();
            }
        }
    }
}
