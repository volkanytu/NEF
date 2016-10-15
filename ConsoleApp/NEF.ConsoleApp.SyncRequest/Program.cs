using Microsoft.Crm.Sdk.Messages;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.ConsoleApp.SyncRequest
{

    class Program
    {
        enum crmStatusCode
        {
            Complete = 8,
            Continue = 1
        }
        enum smStatusCode
        {
            Complete = 3,
            Continue = 1
        }
        static void Main(string[] args)
        {
            // crm connection sql
            SqlDataAccess crmSda = new SqlDataAccess();
            // site yönetimi sql
            SqlDataAccess smSda = new SqlDataAccess();
            crmSda.openConnection(Globals.ConnectionString);
            smSda.openConnection(Globals.ConnectionStringRequests);
            try
            {
                DataTable requests = GetRequest(smSda);
                if (requests.Rows.Count > 0)
                {
                    int counter = 1;
                    string requestId = string.Empty;
                    foreach (DataRow currentRequest in requests.Rows)
                    {
                        Entity serviceAppointment = new Entity("serviceappointment");
                        DataRow crmRecord = null;
                        int smStatus = -1;
                        int crmStatus = -1;

                        if (currentRequest["request_id"] != DBNull.Value)
                        {
                            requestId = currentRequest["request_id"].ToString();
                            smStatus = Convert.ToInt32(currentRequest["request_status_id"]);

                            crmRecord = CheckRequestExistOnCrm(crmSda, requestId);
                            if (crmRecord != null)
                            {
                                crmStatus = Convert.ToInt32(crmRecord["StatusCode"]);
                                if ((int)smStatusCode.Complete == smStatus && (int)crmStatusCode.Complete == crmStatus)
                                {
                                    continue;
                                }
                                else if ((int)smStatusCode.Continue == smStatus && (int)crmStatusCode.Continue == crmStatus)
                                {
                                    continue;
                                }
                            }
                            serviceAppointment["new_requestid"] = Convert.ToInt32(requestId);
                        }

                        if (currentRequest["request_source"] != DBNull.Value)
                        {
                            serviceAppointment["new_geribildirimkaynagi"] = new OptionSetValue(Convert.ToInt32(currentRequest["request_source"]));
                        }
                        if (currentRequest["request_group"] != DBNull.Value)
                        {
                            serviceAppointment["new_group"] = new OptionSetValue(SetRequestGroup(Convert.ToString(currentRequest["request_group"])));
                        }
                        if (currentRequest["request_customer"] != DBNull.Value)
                        {
                            string customerName = Convert.ToString(currentRequest["request_customer"]);
                            serviceAppointment["new_customername"] = customerName;
                            Guid customerId = GetCustomerIdByFullName(crmSda, customerName);

                            if (customerId != Guid.Empty)
                            {
                                List<Entity> customers = new List<Entity>();
                                Entity toParty = new Entity("activityparty");
                                toParty["partyid"] = new EntityReference("contact", customerId);
                                customers.Add(toParty);
                                serviceAppointment["customers"] = customers.ToArray();
                            }
                            else
                            {
                                serviceAppointment["customers"] = null;
                            }
                        }
                        if (currentRequest["request_flat"] != DBNull.Value && !Convert.ToString(currentRequest["request_flat"]).Equals("N/A"))
                        {
                            string productNumber = Convert.ToString(currentRequest["request_flat"]);
                            Guid productId = GetProductIdByNumber(crmSda, productNumber);
                            if (productId != Guid.Empty)
                            {
                                serviceAppointment["new_productid"] = new EntityReference("product", productId);
                            }
                            else
                            {
                                serviceAppointment["new_productid"] = null;
                            }
                        }
                        if (currentRequest["request_proof"] != DBNull.Value)
                        {
                            serviceAppointment["new_proof"] = currentRequest["request_proof"].ToString();
                        }
                        if (currentRequest["request_object"] != DBNull.Value)
                        {
                            serviceAppointment["new_object"] = currentRequest["request_object"].ToString();
                        }
                        if (currentRequest["request_licence_number"] != DBNull.Value && !Convert.ToString(currentRequest["request_licence_number"]).Equals("N/A"))
                        {
                            serviceAppointment["new_licancenumber"] = Convert.ToInt32(currentRequest["request_licence_number"]);
                        }
                        if (currentRequest["request_category"] != DBNull.Value)
                        {
                            serviceAppointment["new_category"] = new OptionSetValue(Convert.ToInt32(currentRequest["request_category"]));
                        }
                        if (currentRequest["request_subcategory"] != DBNull.Value)
                        {
                            serviceAppointment["new_subcategory"] = new OptionSetValue(Convert.ToInt32(currentRequest["request_subcategory"]));
                        }
                        if (currentRequest["request_subject"] != DBNull.Value)
                        {
                            serviceAppointment["subject"] = Convert.ToString(currentRequest["request_subject"]);
                        }
                        if (currentRequest["request_createdby"] != DBNull.Value)
                        {
                            serviceAppointment["new_createdby"] = currentRequest["request_createdby"].ToString();
                        }
                        if (currentRequest["request_technician"] != DBNull.Value)
                        {
                            serviceAppointment["new_technician"] = currentRequest["request_technician"].ToString();
                        }

                        if (currentRequest["request_desc"] != DBNull.Value)
                        {
                            serviceAppointment["description"] = HtmlRemoval.StripTagsRegexCompiled(Convert.ToString(currentRequest["request_desc"]));
                        }
                        if (currentRequest["request_project"] != DBNull.Value)
                        {
                            string projectName = Convert.ToString(currentRequest["request_project"]);
                            Guid projectId = GetProjectIdByName(crmSda, projectName);
                            if (projectId != Guid.Empty)
                            {
                                serviceAppointment["new_project"] = new EntityReference("new_project", projectId);
                            }
                            else
                            {
                                serviceAppointment["new_project"] = null;
                            }
                        }
                        if (currentRequest["request_service_category"] != DBNull.Value)
                        {
                            serviceAppointment["new_service"] = new OptionSetValue(Convert.ToInt32(currentRequest["request_service_category"]));
                        }

                        if (currentRequest["request_creadtedon"] != DBNull.Value)
                        {
                            serviceAppointment["actualstart"] = (DateTime)currentRequest["request_creadtedon"];
                            serviceAppointment["scheduledstart"] = (DateTime)currentRequest["request_creadtedon"];

                            if (currentRequest["request_completed_time"] != DBNull.Value &&
                                (DateTime)currentRequest["request_creadtedon"] > (DateTime)currentRequest["request_completed_time"])
                            {
                                serviceAppointment["actualend"] = ((DateTime)currentRequest["request_creadtedon"]).AddHours(1);
                                serviceAppointment["scheduledend"] = ((DateTime)currentRequest["request_creadtedon"]).AddHours(1);
                            }
                            else
                            {
                                serviceAppointment["actualend"] = (DateTime)currentRequest["request_completed_time"];
                                serviceAppointment["scheduledend"] = (DateTime)currentRequest["request_completed_time"];
                            }
                        }

                        try
                        {
                            Guid crmId = Guid.Empty;
                            if ((int)smStatusCode.Complete == smStatus && (int)crmStatusCode.Continue == crmStatus)
                            {
                                crmId = new Guid(Convert.ToString(crmRecord["ActivityId"]));
                                serviceAppointment.Id = crmId;
                                MSCRM.AdminOrgService.Update(serviceAppointment);
                                SetStateRequestToCompleted(crmId);
                            }
                            else if ((int)crmStatusCode.Continue == smStatus)
                            {
                                crmId = MSCRM.AdminOrgService.Create(serviceAppointment);
                            }
                            else
                            {
                                crmId = MSCRM.AdminOrgService.Create(serviceAppointment);
                                SetStateRequestToCompleted(crmId);
                            }

                        }
                        catch (Exception ex)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(requestId + "Talep aktarımında hata oluştu.");
                            sb.AppendLine("Hata Detayı:");
                            sb.AppendLine("Message : " + ex.Message);
                            SendMail(sb);
                        }

                        Console.WriteLine(counter);
                        counter++;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Talep aktarımında hata oluştu.");
                sb.AppendLine("Hata Detayı:");
                sb.AppendLine("Message : " + ex.Message);
                SendMail(sb);
            }
            finally
            {
                crmSda.closeConnection();
                smSda.closeConnection();
            }
        }

        /// <summary>
        /// Bilgilendirme maili gönder //
        /// </summary>
        /// <param name="sb">Mesaj içeriği</param>
        private static void SendMail(StringBuilder sb)
        {
            Entity fromParty = new Entity("activityparty");
            fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

            Entity toParty = new Entity("activityparty");
            toParty["addressused"] = "erkan.ozvar@nef.com.tr";

            Entity email = new Entity("email");
            email["to"] = new Entity[] { toParty };
            email["from"] = new Entity[] { fromParty };
            email["subject"] = DateTime.Now.AddDays(-1).Date.ToString("dd/MM/yyyy") + " Talep Aktarımları";
            email["description"] = sb.ToString();
            email["directioncode"] = true;
            Guid id = MSCRM.AdminOrgService.Create(email);


            var req = new SendEmailRequest
            {
                EmailId = id,
                TrackingToken = string.Empty,
                IssueSend = true
            };

            try
            {
                var res = (SendEmailResponse)MSCRM.AdminOrgService.Execute(req);
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Talepleri tamamlandı durumuna çeker.
        /// </summary>
        /// <param name="serviceAppointmentId">AppointmentId</param>
        private static void SetStateRequestToCompleted(Guid serviceAppointmentId)
        {
            SetStateRequest state = new SetStateRequest();

            state.State = new OptionSetValue(1);
            state.Status = new OptionSetValue(8);

            state.EntityMoniker = new EntityReference("serviceappointment", serviceAppointmentId);

            SetStateResponse stateSet = (SetStateResponse)MSCRM.AdminOrgService.Execute(state);
        }

        private static void SetStateRequestToContinue(Guid serviceAppointmentId)
        {
            SetStateRequest state = new SetStateRequest();

            state.State = new OptionSetValue(0);
            state.Status = new OptionSetValue(1);

            state.EntityMoniker = new EntityReference("serviceappointment", serviceAppointmentId);

            SetStateResponse stateSet = (SetStateResponse)MSCRM.AdminOrgService.Execute(state);
        }

        /// <summary>
        /// Crm'den Proje adı ile idsi  alınıyor.
        /// </summary>
        /// <param name="crmSda">Crm Sql Data Access</param>
        /// <param name="projectName">Proje Adı</param>
        /// <returns></returns>
        private static Guid GetProjectIdByName(SqlDataAccess crmSda, string projectName)
        {
            Guid retVal = Guid.Empty;
            string query = @"SELECT 
	                            new_projectId
                            FROM 
	                            new_project AS PRO WITH(NOLOCK) 
                            WHERE 
	                            new_name LIKE '%{0}%'";
            DataTable dt = crmSda.getDataTable(string.Format(query, projectName));
            if (dt.Rows.Count > 0)
            {
                retVal = new Guid(Convert.ToString(dt.Rows[0]["new_projectId"]));
            }
            return retVal;
        }

        /// <summary>
        /// Crm'den Daire Kimlik numarası alınıyor.
        /// </summary>
        /// <param name="crmSda">Crm Sql Data Access</param>
        /// <param name="productNumber">Daire Kimlik Numarası</param>
        /// <returns></returns>
        private static Guid GetProductIdByNumber(SqlDataAccess crmSda, string productNumber)
        {
            Guid retVal = Guid.Empty;
            string query = @"SELECT 
	                            ProductId 
                            FROM 
	                            Product AS PRO WITH(NOLOCK) 
                            WHERE 
	                            PRO.ProductNumber = '{0}'";
            DataTable dt = crmSda.getDataTable(string.Format(query, productNumber));
            if (dt.Rows.Count > 0)
            {
                retVal = new Guid(Convert.ToString(dt.Rows[0]["ProductId"]));
            }
            return retVal;
        }

        /// <summary>
        /// Müşeteriler isimden CRM'de varsa getirir - Site Yönetiminde id ile tutulmamış
        /// </summary>
        /// <param name="crmSda">Crm Sql Data Access</param>
        /// <param name="customerName">Müşteri - Talepte bulunan</param>
        /// <returns></returns>
        private static Guid GetCustomerIdByFullName(SqlDataAccess crmSda, string customerName)
        {
            Guid retVal = Guid.Empty;
            string query = @"SELECT
                                ContactId
                            FROM 
                                CONTACT AS CON WITH(NOLOCK) 
                             WHERE 
                                CON.FullName = '{0}'
                             AND 
                                CON.new_customertype = 100000001";
            DataTable dt = crmSda.getDataTable(string.Format(query, customerName));
            if (dt.Rows.Count > 0)
            {
                retVal = new Guid(Convert.ToString(dt.Rows[0]["ContactId"]));
            }
            return retVal;
        }

        /// <summary>
        /// Site yönetimi içerisinde çoğul idye sahip kayıtları adından yakalayarak tekilleştirir.
        /// </summary>
        /// <param name="group">Grup Adı</param>
        /// <returns>Option Set Value</returns>
        private static int SetRequestGroup(string group)
        {
            int retVal = 0;
            switch (group)
            {
                case "Handyman": retVal = 1; break;
                case "Handyman - NEF 02": retVal = 2; break;
                case "Handyman - NEF 04": retVal = 3; break;
                case "Handyman - NEF 09": retVal = 4; break;
                case "Handyman - NEF 10": retVal = 5; break;
                case "Handyman - NEF 11": retVal = 6; break;
                case "Handyman - NEF 163": retVal = 7; break;
                case "Handyman - NEF 47": retVal = 8; break;
                case "Hukuk": retVal = 9; break;
                case "Muhasebe": retVal = 10; break;
                case "Müşteri Finans Takip": retVal = 11; break;
                case "NEF - Müşteri İlişkileri": retVal = 12; break;
                case "Satış": retVal = 13; break;
                case "Site Yönetimi - NEF 02": retVal = 14; break;
                case "Site Yönetimi - NEF 04": retVal = 15; break;
                case "Site Yönetimi - NEF 09": retVal = 16; break;
                case "Site Yönetimi - NEF 10": retVal = 17; break;
                case "Site Yönetimi - NEF 11": retVal = 18; break;
                case "Site Yönetimi - NEF 163": retVal = 19; break;
                case "Site Yönetimi - NEF 47": retVal = 20; break;
                case "Tapu": retVal = 21; break;
                case "Teslim Sonrası Hizmetler": retVal = 22; break;
                case "Teslim Sonrası Hizmetler - NEF 02": retVal = 23; break;
                case "Teslim Sonrası Hizmetler - NEF 04": retVal = 24; break;
                case "Teslim Sonrası Hizmetler - NEF 09": retVal = 25; break;
                case "Teslim Sonrası Hizmetler - NEF 10": retVal = 26; break;
                case "Teslim Sonrası Hizmetler - NEF 11": retVal = 27; break;
                case "Teslim Sonrası Hizmetler - NEF 163": retVal = 28; break;
                case "Teslim Sonrası Hizmetler - NEF 47": retVal = 29; break;
                case "Yapım": retVal = 30; break;
            }
            return retVal;
        }

        /// <summary>
        /// Talepleri site yönetiminden alır
        /// </summary>
        /// <param name="smSda">Site Yönetimi Sql Data Access</param>
        /// <returns></returns>
        private static DataTable GetRequest(SqlDataAccess smSda)
        {
            string allRequestQuery = @"SELECT
                                        wo.WORKORDERID AS 'request_id',
                                        mdd.MODEID 'request_source',
                                        qd.QUEUENAME AS 'request_group',
                                        aau.FIRST_NAME AS 'request_customer',
                                        (SELECT
                                        CASE WHEN ISNUMERIC(SUBSTRING(SD.JOBTITLE,1,3)) = 1 AND LEN(SD.JOBTITLE) >1 THEN SD.JOBTITLE
                                        ELSE 'N/A'
                                        END
                                        FROM SDUser as SD  where SD.USERID = aau.[USER_ID] ) AS 'request_flat',
                                        (CASE WHEN ISNUMERIC(dpt.DEPTNAME) = 1 THEN  dpt.DEPTNAME ELSE 'N/A' END) AS 'request_licence_number',
                                        cd.CATEGORYID AS 'request_category',
                                        scd.SUBCATEGORYID AS 'request_subcategory',
                                        wo.TITLE AS  'request_subject',
                                        cri.FIRST_NAME AS 'request_createdby',
                                        ti.FIRST_NAME  AS 'request_technician',
                                        wotodesc.FULLDESCRIPTION AS 'request_desc',
                                        sdo.NAME AS 'request_project',
                                        serdef.SERVICEID AS 'request_service_category',std.STATUSNAME AS 'request_status',
                                        std.STATUSID AS 'request_status_id',
                                        wof.UDF_CHAR3 as 'request_proof' ,
                                        icd.Name as 'request_object',
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.CREATEDTIME / 1000), '1970-01-01 00:00:00') 'request_creadtedon' ,
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.COMPLETEDTIME / 1000), '1970-01-01 00:00:00') AS 'request_completed_time'
                                        FROM WorkOrder wo
                                        LEFT JOIN ModeDefinition mdd ON wo.MODEID = mdd.MODEID
                                        LEFT JOIN SDUser sdu ON wo.REQUESTERID = sdu.USERID
                                        LEFT JOIN AaaUser aau ON sdu.USERID = aau.USER_ID
                                        LEFT JOIN DepartmentDefinition dpt ON wo.DEPTID = dpt.DEPTID
                                        LEFT JOIN SDUser crd ON wo.CREATEDBYID = crd.USERID
                                        LEFT JOIN AaaUser cri ON crd.USERID = cri.USER_ID
                                        LEFT JOIN WorkOrderToDescription wotodesc ON wo.WORKORDERID = wotodesc.WORKORDERID
                                        LEFT JOIN SiteDefinition siteDef ON wo.SITEID = siteDef.SITEID
                                        LEFT JOIN SDOrganization sdo ON siteDef.SITEID = sdo.ORG_ID
                                        LEFT JOIN RegionDefinition regionDef ON siteDef.REGIONID = regionDef.REGIONID
                                        LEFT JOIN ServiceDefinition serdef ON wo.SERVICEID = serdef.SERVICEID
                                        LEFT JOIN Resources res ON wo.WORKORDERID = res.RESOURCEID
                                        LEFT JOIN WorkOrderStates wos ON wo.WORKORDERID = wos.WORKORDERID
                                        LEFT JOIN CategoryDefinition cd ON wos.CATEGORYID = cd.CATEGORYID
                                        LEFT JOIN SubCategoryDefinition scd ON wos.SUBCATEGORYID = scd.SUBCATEGORYID
                                        LEFT JOIN ItemDefinition icd ON wos.ITEMID = icd.ITEMID
                                        LEFT JOIN SDUser td ON wos.OWNERID = td.USERID
                                        LEFT JOIN AaaUser ti ON td.USERID = ti.USER_ID
                                        LEFT JOIN ApprovalStatusDefinition appStDef ON wos.APPR_STATUSID = appStDef.STATUSID
                                        LEFT JOIN RequestClosureCode rcode ON wos.CLOSURECODEID = rcode.CLOSURECODEID
                                        LEFT JOIN PriorityDefinition pd ON wos.PRIORITYID = pd.PRIORITYID
                                        LEFT JOIN LevelDefinition lvd ON wos.LEVELID = lvd.LEVELID
                                        LEFT JOIN StatusDefinition std ON wos.STATUSID = std.STATUSID
                                        LEFT JOIN WorkOrder_Queue woq ON wo.WORKORDERID = woq.WORKORDERID
                                        LEFT JOIN QueueDefinition qd ON woq.QUEUEID = qd.QUEUEID
                                        LEFT JOIN RequestResolver rrr ON wo.WORKORDERID = rrr.REQUESTID
                                        LEFT JOIN RequestResolution rrs ON rrr.REQUESTID = rrs.REQUESTID
                                        LEFT JOIN WorkOrder_Fields wof ON wo.WORKORDERID = wof.WORKORDERID
                                        LEFT JOIN ServiceCatalog_Fields scf ON wo.WORKORDERID = scf.WORKORDERID
                                        LEFT JOIN SDUser obosdu ON wo.OBOID = obosdu.USERID
                                        LEFT JOIN AaaUser oboau ON obosdu.USERID = oboau.USER_ID          
                                        WHERE(wo.ISPARENT = '1') and std.STATUSID = 1 OR std.STATUSID = 3";

            string todayRequestsQuery = @"SELECT
                                        wo.WORKORDERID AS 'request_id',
                                        mdd.MODEID 'request_source',
                                        qd.QUEUENAME AS 'request_group',
                                        aau.FIRST_NAME AS 'request_customer',
                                        (SELECT
                                        CASE WHEN ISNUMERIC(SUBSTRING(SD.JOBTITLE,1,3)) = 1 AND LEN(SD.JOBTITLE) >1 THEN SD.JOBTITLE
                                        ELSE 'N/A'
                                        END
                                        FROM SDUser as SD  where SD.USERID = aau.[USER_ID] ) AS 'request_flat',
                                        (CASE WHEN ISNUMERIC(dpt.DEPTNAME) = 1 THEN  dpt.DEPTNAME ELSE 'N/A' END) AS 'request_licence_number',
                                        cd.CATEGORYID AS 'request_category',
                                        scd.SUBCATEGORYID AS 'request_subcategory',
                                        wo.TITLE AS  'request_subject',
                                        cri.FIRST_NAME AS 'request_createdby',
                                        ti.FIRST_NAME  AS 'request_technician',
                                        wotodesc.FULLDESCRIPTION AS 'request_desc',
                                        sdo.NAME AS 'request_project',
                                        serdef.SERVICEID AS 'request_service_category',std.STATUSNAME AS 'request_status',
                                        std.STATUSID AS 'request_status_id',
                                        wof.UDF_CHAR3 as 'request_proof' ,
                                        icd.Name as 'request_object',
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.CREATEDTIME / 1000), '1970-01-01 00:00:00') 'request_creadtedon' ,
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.COMPLETEDTIME / 1000), '1970-01-01 00:00:00') AS 'request_completed_time'
                                        FROM WorkOrder wo
                                        LEFT JOIN ModeDefinition mdd ON wo.MODEID = mdd.MODEID
                                        LEFT JOIN SDUser sdu ON wo.REQUESTERID = sdu.USERID
                                        LEFT JOIN AaaUser aau ON sdu.USERID = aau.USER_ID
                                        LEFT JOIN DepartmentDefinition dpt ON wo.DEPTID = dpt.DEPTID
                                        LEFT JOIN SDUser crd ON wo.CREATEDBYID = crd.USERID
                                        LEFT JOIN AaaUser cri ON crd.USERID = cri.USER_ID
                                        LEFT JOIN WorkOrderToDescription wotodesc ON wo.WORKORDERID = wotodesc.WORKORDERID
                                        LEFT JOIN SiteDefinition siteDef ON wo.SITEID = siteDef.SITEID
                                        LEFT JOIN SDOrganization sdo ON siteDef.SITEID = sdo.ORG_ID
                                        LEFT JOIN RegionDefinition regionDef ON siteDef.REGIONID = regionDef.REGIONID
                                        LEFT JOIN ServiceDefinition serdef ON wo.SERVICEID = serdef.SERVICEID
                                        LEFT JOIN Resources res ON wo.WORKORDERID = res.RESOURCEID
                                        LEFT JOIN WorkOrderStates wos ON wo.WORKORDERID = wos.WORKORDERID
                                        LEFT JOIN CategoryDefinition cd ON wos.CATEGORYID = cd.CATEGORYID
                                        LEFT JOIN SubCategoryDefinition scd ON wos.SUBCATEGORYID = scd.SUBCATEGORYID
                                        LEFT JOIN ItemDefinition icd ON wos.ITEMID = icd.ITEMID
                                        LEFT JOIN SDUser td ON wos.OWNERID = td.USERID
                                        LEFT JOIN AaaUser ti ON td.USERID = ti.USER_ID
                                        LEFT JOIN ApprovalStatusDefinition appStDef ON wos.APPR_STATUSID = appStDef.STATUSID
                                        LEFT JOIN RequestClosureCode rcode ON wos.CLOSURECODEID = rcode.CLOSURECODEID
                                        LEFT JOIN PriorityDefinition pd ON wos.PRIORITYID = pd.PRIORITYID
                                        LEFT JOIN LevelDefinition lvd ON wos.LEVELID = lvd.LEVELID
                                        LEFT JOIN StatusDefinition std ON wos.STATUSID = std.STATUSID
                                        LEFT JOIN WorkOrder_Queue woq ON wo.WORKORDERID = woq.WORKORDERID
                                        LEFT JOIN QueueDefinition qd ON woq.QUEUEID = qd.QUEUEID
                                        LEFT JOIN RequestResolver rrr ON wo.WORKORDERID = rrr.REQUESTID
                                        LEFT JOIN RequestResolution rrs ON rrr.REQUESTID = rrs.REQUESTID
                                        LEFT JOIN WorkOrder_Fields wof ON wo.WORKORDERID = wof.WORKORDERID
                                        LEFT JOIN ServiceCatalog_Fields scf ON wo.WORKORDERID = scf.WORKORDERID
                                        LEFT JOIN SDUser obosdu ON wo.OBOID = obosdu.USERID
                                        LEFT JOIN AaaUser oboau ON obosdu.USERID = oboau.USER_ID                
                                        WHERE
											(wo.ISPARENT = '1') AND
										((std.STATUSID = 1 AND 	CONVERT(nvarchar, (CAST(dateadd(second, (wo.CREATEDTIME / 1000) + 8*60*60, '19700101') AS date)),104)   = '{0}')
											OR
											(std.STATUSID = 3 AND 	CONVERT(nvarchar, (CAST(dateadd(second, (wo.COMPLETEDTIME / 1000) + 8*60*60, '19700101') AS date)),104)   =  '{0}'))";
            //return smSda.getDataTable(allRequestQuery);
            //return smSda.getDataTable(string.Format(todayRequestsQuery, DateTime.Now.ToShortDateString()));
            string date = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);//.ToShortDateString();
            return smSda.getDataTable(string.Format(todayRequestsQuery, date));
        }

        /// <summary>
        /// Talep id ile crmde var mı kontrol eder?
        /// </summary>
        /// <param name="crmSda">Crm Data Access</param>
        /// <param name="requestId">Talep Id</param>
        /// <returns></returns>
        private static DataRow CheckRequestExistOnCrm(SqlDataAccess crmSda, string requestId)
        {
            string querry = @"SELECT 
	                            sa.ActivityId,
                                sa.StatusCode
                            FROM 
	                            ServiceAppointment as sa WITH(NOLOCK)
                            WHERE 
	                            sa.new_requestid = '{0}'";

            DataTable resultDt = crmSda.getDataTable(string.Format(querry, requestId));
            if (resultDt.Rows.Count > 0)
            {
                return resultDt.Rows[0];
            }
            return null;
        }

        private static DataTable GetRequestByDate(SqlDataAccess smSda, string date)
        {
            string allRequestQuery = @"SELECT
                                        wo.WORKORDERID AS 'request_id',
                                        mdd.MODEID 'request_source',
                                        qd.QUEUENAME AS 'request_group',
                                        aau.FIRST_NAME AS 'request_customer',
                                        (SELECT
                                        CASE WHEN ISNUMERIC(SUBSTRING(SD.JOBTITLE,1,3)) = 1 AND LEN(SD.JOBTITLE) >1 THEN SD.JOBTITLE
                                        ELSE 'N/A'
                                        END
                                        FROM SDUser as SD  where SD.USERID = aau.[USER_ID] ) AS 'request_flat',
                                        (CASE WHEN ISNUMERIC(dpt.DEPTNAME) = 1 THEN  dpt.DEPTNAME ELSE 'N/A' END) AS 'request_licence_number',
                                        cd.CATEGORYID AS 'request_category',
                                        scd.SUBCATEGORYID AS 'request_subcategory',
                                        wo.TITLE AS  'request_subject',
                                        cri.FIRST_NAME AS 'request_createdby',
                                        ti.FIRST_NAME  AS 'request_technician',
                                        wotodesc.FULLDESCRIPTION AS 'request_desc',
                                        sdo.NAME AS 'request_project',
                                        serdef.SERVICEID AS 'request_service_category',std.STATUSNAME AS 'request_status',
                                        std.STATUSID AS 'request_status_id',
                                        wof.UDF_CHAR3 as 'request_proof' ,
                                        icd.Name as 'request_object',
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.CREATEDTIME / 1000), '1970-01-01 00:00:00') 'request_creadtedon' ,
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.COMPLETEDTIME / 1000), '1970-01-01 00:00:00') AS 'request_completed_time'
                                        FROM WorkOrder wo
                                        LEFT JOIN ModeDefinition mdd ON wo.MODEID = mdd.MODEID
                                        LEFT JOIN SDUser sdu ON wo.REQUESTERID = sdu.USERID
                                        LEFT JOIN AaaUser aau ON sdu.USERID = aau.USER_ID
                                        LEFT JOIN DepartmentDefinition dpt ON wo.DEPTID = dpt.DEPTID
                                        LEFT JOIN SDUser crd ON wo.CREATEDBYID = crd.USERID
                                        LEFT JOIN AaaUser cri ON crd.USERID = cri.USER_ID
                                        LEFT JOIN WorkOrderToDescription wotodesc ON wo.WORKORDERID = wotodesc.WORKORDERID
                                        LEFT JOIN SiteDefinition siteDef ON wo.SITEID = siteDef.SITEID
                                        LEFT JOIN SDOrganization sdo ON siteDef.SITEID = sdo.ORG_ID
                                        LEFT JOIN RegionDefinition regionDef ON siteDef.REGIONID = regionDef.REGIONID
                                        LEFT JOIN ServiceDefinition serdef ON wo.SERVICEID = serdef.SERVICEID
                                        LEFT JOIN Resources res ON wo.WORKORDERID = res.RESOURCEID
                                        LEFT JOIN WorkOrderStates wos ON wo.WORKORDERID = wos.WORKORDERID
                                        LEFT JOIN CategoryDefinition cd ON wos.CATEGORYID = cd.CATEGORYID
                                        LEFT JOIN SubCategoryDefinition scd ON wos.SUBCATEGORYID = scd.SUBCATEGORYID
                                        LEFT JOIN ItemDefinition icd ON wos.ITEMID = icd.ITEMID
                                        LEFT JOIN SDUser td ON wos.OWNERID = td.USERID
                                        LEFT JOIN AaaUser ti ON td.USERID = ti.USER_ID
                                        LEFT JOIN ApprovalStatusDefinition appStDef ON wos.APPR_STATUSID = appStDef.STATUSID
                                        LEFT JOIN RequestClosureCode rcode ON wos.CLOSURECODEID = rcode.CLOSURECODEID
                                        LEFT JOIN PriorityDefinition pd ON wos.PRIORITYID = pd.PRIORITYID
                                        LEFT JOIN LevelDefinition lvd ON wos.LEVELID = lvd.LEVELID
                                        LEFT JOIN StatusDefinition std ON wos.STATUSID = std.STATUSID
                                        LEFT JOIN WorkOrder_Queue woq ON wo.WORKORDERID = woq.WORKORDERID
                                        LEFT JOIN QueueDefinition qd ON woq.QUEUEID = qd.QUEUEID
                                        LEFT JOIN RequestResolver rrr ON wo.WORKORDERID = rrr.REQUESTID
                                        LEFT JOIN RequestResolution rrs ON rrr.REQUESTID = rrs.REQUESTID
                                        LEFT JOIN WorkOrder_Fields wof ON wo.WORKORDERID = wof.WORKORDERID
                                        LEFT JOIN ServiceCatalog_Fields scf ON wo.WORKORDERID = scf.WORKORDERID
                                        LEFT JOIN SDUser obosdu ON wo.OBOID = obosdu.USERID
                                        LEFT JOIN AaaUser oboau ON obosdu.USERID = oboau.USER_ID          
                                        WHERE(wo.ISPARENT = '1') and std.STATUSID = 1 OR std.STATUSID = 3";

            string todayRequestsQuery = @"SELECT
                                        wo.WORKORDERID AS 'request_id',
                                        mdd.MODEID 'request_source',
                                        qd.QUEUENAME AS 'request_group',
                                        aau.FIRST_NAME AS 'request_customer',
                                        (SELECT
                                        CASE WHEN ISNUMERIC(SUBSTRING(SD.JOBTITLE,1,3)) = 1 AND LEN(SD.JOBTITLE) >1 THEN SD.JOBTITLE
                                        ELSE 'N/A'
                                        END
                                        FROM SDUser as SD  where SD.USERID = aau.[USER_ID] ) AS 'request_flat',
                                        (CASE WHEN ISNUMERIC(dpt.DEPTNAME) = 1 THEN  dpt.DEPTNAME ELSE 'N/A' END) AS 'request_licence_number',
                                        cd.CATEGORYID AS 'request_category',
                                        scd.SUBCATEGORYID AS 'request_subcategory',
                                        wo.TITLE AS  'request_subject',
                                        cri.FIRST_NAME AS 'request_createdby',
                                        ti.FIRST_NAME  AS 'request_technician',
                                        wotodesc.FULLDESCRIPTION AS 'request_desc',
                                        sdo.NAME AS 'request_project',
                                        serdef.SERVICEID AS 'request_service_category',std.STATUSNAME AS 'request_status',
                                        std.STATUSID AS 'request_status_id',
                                        wof.UDF_CHAR3 as 'request_proof' ,
                                        icd.Name as 'request_object',
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.CREATEDTIME / 1000), '1970-01-01 00:00:00') 'request_creadtedon' ,
                                        dateadd(s, datediff(s, GETUTCDATE(), getdate()) + (wo.COMPLETEDTIME / 1000), '1970-01-01 00:00:00') AS 'request_completed_time'
                                        FROM WorkOrder wo
                                        LEFT JOIN ModeDefinition mdd ON wo.MODEID = mdd.MODEID
                                        LEFT JOIN SDUser sdu ON wo.REQUESTERID = sdu.USERID
                                        LEFT JOIN AaaUser aau ON sdu.USERID = aau.USER_ID
                                        LEFT JOIN DepartmentDefinition dpt ON wo.DEPTID = dpt.DEPTID
                                        LEFT JOIN SDUser crd ON wo.CREATEDBYID = crd.USERID
                                        LEFT JOIN AaaUser cri ON crd.USERID = cri.USER_ID
                                        LEFT JOIN WorkOrderToDescription wotodesc ON wo.WORKORDERID = wotodesc.WORKORDERID
                                        LEFT JOIN SiteDefinition siteDef ON wo.SITEID = siteDef.SITEID
                                        LEFT JOIN SDOrganization sdo ON siteDef.SITEID = sdo.ORG_ID
                                        LEFT JOIN RegionDefinition regionDef ON siteDef.REGIONID = regionDef.REGIONID
                                        LEFT JOIN ServiceDefinition serdef ON wo.SERVICEID = serdef.SERVICEID
                                        LEFT JOIN Resources res ON wo.WORKORDERID = res.RESOURCEID
                                        LEFT JOIN WorkOrderStates wos ON wo.WORKORDERID = wos.WORKORDERID
                                        LEFT JOIN CategoryDefinition cd ON wos.CATEGORYID = cd.CATEGORYID
                                        LEFT JOIN SubCategoryDefinition scd ON wos.SUBCATEGORYID = scd.SUBCATEGORYID
                                        LEFT JOIN ItemDefinition icd ON wos.ITEMID = icd.ITEMID
                                        LEFT JOIN SDUser td ON wos.OWNERID = td.USERID
                                        LEFT JOIN AaaUser ti ON td.USERID = ti.USER_ID
                                        LEFT JOIN ApprovalStatusDefinition appStDef ON wos.APPR_STATUSID = appStDef.STATUSID
                                        LEFT JOIN RequestClosureCode rcode ON wos.CLOSURECODEID = rcode.CLOSURECODEID
                                        LEFT JOIN PriorityDefinition pd ON wos.PRIORITYID = pd.PRIORITYID
                                        LEFT JOIN LevelDefinition lvd ON wos.LEVELID = lvd.LEVELID
                                        LEFT JOIN StatusDefinition std ON wos.STATUSID = std.STATUSID
                                        LEFT JOIN WorkOrder_Queue woq ON wo.WORKORDERID = woq.WORKORDERID
                                        LEFT JOIN QueueDefinition qd ON woq.QUEUEID = qd.QUEUEID
                                        LEFT JOIN RequestResolver rrr ON wo.WORKORDERID = rrr.REQUESTID
                                        LEFT JOIN RequestResolution rrs ON rrr.REQUESTID = rrs.REQUESTID
                                        LEFT JOIN WorkOrder_Fields wof ON wo.WORKORDERID = wof.WORKORDERID
                                        LEFT JOIN ServiceCatalog_Fields scf ON wo.WORKORDERID = scf.WORKORDERID
                                        LEFT JOIN SDUser obosdu ON wo.OBOID = obosdu.USERID
                                        LEFT JOIN AaaUser oboau ON obosdu.USERID = oboau.USER_ID                
                                        WHERE
											(wo.ISPARENT = '1') AND
										((std.STATUSID = 1 AND 	CONVERT(nvarchar, (CAST(dateadd(second, (wo.COMPLETEDTIME / 1000) + 8*60*60, '19700101') AS date)),104)   = '{0}')
											OR
											(std.STATUSID = 3 AND 	CONVERT(nvarchar, (CAST(dateadd(second, (wo.COMPLETEDTIME / 1000) + 8*60*60, '19700101') AS date)),104)   =  '{0}'))";
            //return smSda.getDataTable(allRequestQuery);
            //return smSda.getDataTable(string.Format(todayRequestsQuery, DateTime.Now.ToShortDateString()));

            return smSda.getDataTable(string.Format(todayRequestsQuery, date));
        }
    }
}
