﻿using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.ConsoleApp.DiscountProtocol
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
                Process.Execute(sda, service);
            }
            catch (Exception ex)
            {
                throw ex;
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
