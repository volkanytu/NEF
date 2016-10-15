using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.UpdateProductStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlDataAccess sda = null;
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                IOrganizationService service = MSCRM.GetOrgService(true);

                ProductProcess.Process(sda, service);

                HomeOptionProductProcess.SetOptionState(sda, service);

                //HomeOptionProductProcess.SetProductStatus(sda, service);
            }
            catch (Exception)
            {
                
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
