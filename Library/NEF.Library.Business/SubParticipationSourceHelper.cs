using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public class SubParticipationSourceHelper
    {
        internal static void SetCode(Entity entity, SqlDataAccess sda, IOrganizationService adminService)
        {
            string query = @"SELECT 
	                                TOP 1 
	                                C.new_code
                                 FROM 
	                                new_subsourceofparticipation C (NOLOCK)
                                 WHERE
                                    C.new_code IS NOT NULL
                                 ORDER BY
	                               		 TRY_PARSE(C.new_code AS INT ) DESC";
            object value = sda.ExecuteScalar(query);
            if (value != null && value != DBNull.Value)
            {
                entity["new_code"] = (Convert.ToInt32(value) + 1).ToString();
            }
            else
            {
                entity["new_code"] = "1";
            }
        }
    }
}
