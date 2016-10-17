using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Utility;
using System.Data;

namespace NEF.Library.Business
{
    public static class ProjectHelper
    {
        public static MsCrmResultObject GetProjectDetail(Guid projectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                string query = @"SELECT
	                                P.new_projectId AS Id
	                                ,P.new_loyaltypointratio AS Ratio
	                                ,P.new_loyaltypointexpiredate AS ExpireDate 
                                FROM
	                                new_project P WITH (NOLOCK)
                                WHERE 
	                                P.new_projectId = '{0}'";

                DataTable dt = sda.getDataTable(string.Format(query, projectId));
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    Project project = new Project();
                    project.ProjectId = (Guid) dt.Rows[0]["Id"];
                    if (dt.Rows[0]["Ratio"] != DBNull.Value)
                    {
                        project.Ratio = (decimal) dt.Rows[0]["Ratio"];
                    }

                    if (dt.Rows[0]["ExpireDate"] != DBNull.Value)
                    {
                        project.ExpireDate = (DateTime)dt.Rows[0]["ExpireDate"];
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = project;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetTotalSalesAmount(Guid quoteId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                string query = @"SELECT
	                                Q.new_totalsalesamountbytax AS TotalAmount
                                FROM
	                                Quote Q WITH (NOLOCK)
                                WHERE
	                                Q.QuoteId = '{0}'";

                DataTable dt = sda.getDataTable(string.Format(query, quoteId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.ReturnObject = (decimal) dt.Rows[0]["TotalAmount"];
                }
            }
            catch (Exception ex)
            {
               
            }
            return returnValue;
        }
    }
}
