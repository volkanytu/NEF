using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.PaymentMailWeekly
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

                IOrganizationService service =  MSCRM.GetOrgService(true);
                //Musterının sahıbı olan Satıs Temsılcısıne 1 haftalık vadesı olan senetlerı Gönderir
                Process.ExecuteFilterUser(sda, service);
                //Filtresiz Mail Grubuna Göndeir
                Process.ExecuteMailGroup(sda, service);


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
