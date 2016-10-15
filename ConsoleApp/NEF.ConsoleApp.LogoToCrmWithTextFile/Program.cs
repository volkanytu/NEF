using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.LogoToCrmWithTextFile
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

                Process.ExecutePayment(sda, service);


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
