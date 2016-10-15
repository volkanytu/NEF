using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.UpdateChannelOfAwareness
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = MSCRM.GetOrgService(true);
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string query= @"SELECT new_channelofawarenessId FROM new_channelofawareness";
            DataTable dt = sda.getDataTable(query);
            sda.closeConnection();

            foreach (DataRow dr in dt.Rows)
            {
                Entity c = new Entity("new_channelofawareness");
                c.Id = (Guid)dr["new_channelofawarenessId"];
                c["new_code"] = (dt.Rows.IndexOf(dr) + 1).ToString();
                service.Update(c);

            }

        }
    }
}
