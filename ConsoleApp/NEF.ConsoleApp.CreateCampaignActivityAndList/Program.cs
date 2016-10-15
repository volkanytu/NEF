using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.CreateCampaignActivityAndList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateCampaignAndList.Execute(MSCRM.AdminOrgService);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
