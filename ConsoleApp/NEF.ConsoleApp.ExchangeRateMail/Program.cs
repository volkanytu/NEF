using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.ExchangeRateMail
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlDataAccess sda = null;
            try
            {
               
                Process.Execute();


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
