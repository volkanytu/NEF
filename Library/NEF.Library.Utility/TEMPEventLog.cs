using System;
using System.Data.SqlClient;
using Microsoft.Xrm.Sdk;
//using System.Web.Services.Protocols;

namespace NEF.Library.Utility
{ 
    public class TEMPEventLog
    {
        public TEMPEventLog(IOrganizationService orgService, SqlDataAccess sda)
        {
            this.OrgService = orgService;
            this.sda = sda;

        }
        public TEMPEventLog(IOrganizationService orgService, string applicationName, SqlDataAccess sda)
        {
            this.OrgService = orgService;
            this.ApplicationName = applicationName;
            this.sda = sda;

        }

        public enum EventType
        {
            Info = 100000000,
            Warning,
            Exception
        }
        public string ApplicationName { get; set; }
        public IOrganizationService OrgService { get; set; }
        public SqlDataAccess sda { get; set; }

        public static void GetExceptionString(Exception ex, out string detailedText)
        {
            detailedText = null;
            if (ex != null)
            {
                //if (ex is SoapException)
                //{
                //    SoapException soap = ex as SoapException;
                //    detailedText = string.Format("Message:\n {0} \n\n StackTrace:\n {1} \n\n Detail:\n {2}", soap.Message, soap.ToString(), soap.Detail.InnerText);
                //}
                //else 
                if (ex is SqlException)
                {
                    SqlException sql = ex as SqlException;
                    detailedText = string.Format("Message:\n {0} \n\n StackTrace:\n {1} \n\n SqlErrors:\n {2}", sql.Message, sql.ToString(), sql.Errors.Count);
                }
                else
                {
                    detailedText = string.Format("Message:\n {0} \n\n StackTrace:\n {1}", ex.Message, ex.ToString());
                }
            }
        }

        public void Log(string functionName, string detail, EventType eventType)
        {
            try
            {
               
                sda.openConnection(Globals.EuroMessageCustomDBString);
                string query = "Insert Into EventLog (ApplicationName, FunctionName, Description, EventType, CreatedOn) Values(@Applicationname,@functionname,@description,@EventType, @CreatedOn)";
                sda.ExecuteNonQuery(query, new SqlParameter("@Applicationname", this.ApplicationName), new SqlParameter("@functionname", functionName), new SqlParameter("@description", detail), new SqlParameter("@EventType", eventType.ToString()), new SqlParameter("@CreatedOn", DateTime.Now));
                sda.closeConnection();
            }
            catch (Exception ex) { throw ex; }
        }
        public void Log(string functionName, string detail, EventType eventType, string relatedObjectType, string relatedObjectId)
        {
            try
            {
                sda.openConnection(Globals.EuroMessageCustomDBString);
                string query = "Insert Into EventLog Values(@Applicationname,@functionname,@description,@EventType, @CreatedOn, @RelatedObjectType, @RelatedObjectId)";
                sda.ExecuteNonQuery(query, new SqlParameter("@Applicationname", this.ApplicationName), new SqlParameter("@functionname", functionName), new SqlParameter("@description", detail), new SqlParameter("@EventType", eventType.ToString()), new SqlParameter("@CreatedOn", DateTime.Now), new SqlParameter("@RelatedObjectType", relatedObjectType), new SqlParameter("@RelatedObjectId", relatedObjectId));
                sda.closeConnection();
            }
            catch { }
        }
        public void Log(string functionName, Exception ex, EventType eventType)
        {
            try
            {
                sda.openConnection(Globals.EuroMessageCustomDBString);

                string exDetail = null;
                GetExceptionString(ex, out exDetail);

                string query = "Insert Into EventLog (ApplicationName, FunctionName, Description, EventType, CreatedOn) Values(@Applicationname,@functionname,@description,@EventType, @CreatedOn)";
                sda.ExecuteNonQuery(query, new SqlParameter("@Applicationname", this.ApplicationName), new SqlParameter("@functionname", functionName), new SqlParameter("@description", exDetail), new SqlParameter("@EventType", eventType.ToString()), new SqlParameter("@CreatedOn", DateTime.Now));
                sda.closeConnection();
            }
            catch { }
        }
        public void Log(string functionName, Exception ex, EventType eventType, string relatedObjectType, string relatedObjectId)
        {
            try
            {
                sda.openConnection(Globals.EuroMessageCustomDBString);

                string exDetail = null;
                GetExceptionString(ex, out exDetail);

                string query = "Insert Into EventLog Values(@Applicationname,@functionname,@description,@EventType, @CreatedOn, @RelatedObjectType, @RelatedObjectId)";
                sda.ExecuteNonQuery(query, new SqlParameter("@Applicationname", this.ApplicationName), new SqlParameter("@functionname", functionName), new SqlParameter("@description", exDetail), new SqlParameter("@EventType", eventType.ToString()), new SqlParameter("@CreatedOn", DateTime.Now), new SqlParameter("@RelatedObjectType", relatedObjectType), new SqlParameter("@RelatedObjectId", relatedObjectId));
                sda.closeConnection();
            }
            catch { }
        }

        public static void Log(IOrganizationService orgService, string applicationName, string functionName, string detail, EventType eventType)
        {
            try
            {
                Entity e = new Entity("new_eventlog");
                e["new_name"] = string.Format("{0} - {1} - {2}", applicationName, functionName, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                e["new_applicationname"] = applicationName;
                e["new_functionname"] = functionName;
                e["new_description"] = detail;
                e["new_eventtype"] = new OptionSetValue((int)eventType);
                orgService.Create(e);
            }
            catch { }
        }
        public static void Log(IOrganizationService orgService, string applicationName, string functionName, string detail, EventType eventType, string relatedObjectType, string relatedObjectId)
        {
            try
            {
                Entity e = new Entity("new_eventlog");
                e["new_name"] = string.Format("{0} - {1} - {2}", applicationName, functionName, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                e["new_applicationname"] = applicationName;
                e["new_functionname"] = functionName;
                e["new_description"] = detail;
                e["new_eventtype"] = new OptionSetValue((int)eventType);
                e["new_objecttype"] = relatedObjectType;
                e["new_objectid"] = relatedObjectId;

                orgService.Create(e);
            }
            catch { }
        }
        public static void Log(IOrganizationService orgService, string applicationName, string functionName, Exception ex, EventType eventType)
        {
            try
            {
                Entity e = new Entity("new_eventlog");
                e["new_name"] = string.Format("{0} - {1} - {2}", applicationName, functionName, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                e["new_applicationname"] = applicationName;
                e["new_functionname"] = functionName;
                string exDetail = null;
                GetExceptionString(ex, out exDetail);
                e["new_description"] = exDetail;
                e["new_eventtype"] = new OptionSetValue((int)eventType);
                orgService.Create(e);
            }
            catch { }
        }
        public static void Log(IOrganizationService orgService, string applicationName, string functionName, Exception ex, EventType eventType, string relatedObjectType, string relatedObjectId)
        {
            try
            {
                Entity e = new Entity("new_eventlog");
                e["new_name"] = string.Format("{0} - {1} - {2}", applicationName, functionName, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                e["new_applicationname"] = applicationName;
                e["new_functionname"] = functionName;
                string exDetail = null;
                GetExceptionString(ex, out exDetail);
                e["new_description"] = exDetail;
                e["new_eventtype"] = new OptionSetValue((int)eventType);
                e["new_objecttype"] = relatedObjectType;
                e["new_objectid"] = relatedObjectId;

                orgService.Create(e);
            }
            catch { }
        }
    }
}