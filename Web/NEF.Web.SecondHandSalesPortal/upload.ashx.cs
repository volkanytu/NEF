
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;


namespace NEF.Web.SecondHandSalesPortal
{
    /// <summary>
    /// Summary description for upload
    /// </summary>
    public class upload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var op = context.Request.QueryString["operation"];

            if (op != null && op == "1")
            {
                #region İlgili Kişi - Nüfus cüzdanı yükleme
                context.Response.ContentType = "application/json";

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string message = string.Empty;
                var base64Data = context.Request.Form["data"];
                var contactId = context.Request.Form["contactid"];

                IOrganizationService service = MSCRM.GetOrgService(true);

                Entity attach = new Entity("annotation");

                attach["filename"] = context.Request.Form["name"];
                attach["mimetype"] = context.Request.Form["type"];
                attach["filesize"] = context.Request.Form["size"];
                attach["subject"] = context.Request.Form["name"];
                attach["documentbody"] = base64Data;
                attach["objecttypecode"] = 2;
                attach["isdocument"] = true;
                attach["objectid"] = new EntityReference("contact", new Guid(contactId));

                service.Create(attach);

                var data = serializer.Serialize(true);
                context.Response.Write(data);
                #endregion
            }
            else if (op != null && op == "2")
            {
                #region İlgili Kişi - Nüfus cüzdanı indirme
                var id = context.Request.QueryString["id"];
                if (id != null)
                {
                    HttpResponse response = context.Response;
                    Entity ann = GetAnnotionByContactId(new Guid(Convert.ToString(id)));
                    if (ann == null)
                    {
                        response.Write("<center><h3>Dosya Bulunamadı.</h3></center>");
                        return;
                    }

                    byte[] toEncryptArray = Convert.FromBase64String(ann.GetAttributeValue<string>("documentbody"));
                    string fileName = ann.GetAttributeValue<string>("filename");
                    response.ClearContent();
                    response.ContentType = ann.GetAttributeValue<string>("mimetype");
                    response.CacheControl = "no-cache";
                    response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    response.AddHeader("Content-Length", toEncryptArray.Length.ToString());
                    response.BinaryWrite(toEncryptArray);
                    response.Buffer = true;
                    response.End();
                }
                #endregion
            }
            else if (op != null && op == "6")
            {
                #region Aktivite - Yer gösterme belgesi örneği indirme
                string serverPath = HttpContext.Current.Server.MapPath("~");
                string fileLocation = serverPath + "\\RentalDocs\\YerGostermeBelgesi.docx";

                FileInfo fileInfo = new FileInfo(fileLocation);
                if (fileInfo.Exists)
                {
                    context.Response.ClearContent();
                    context.Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Yer Gösterme Belgesi.docx"));
                    context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.TransmitFile(fileLocation);
                    context.Response.End();
                }
                #endregion
            }
            else if (op != null && op == "7")
            {
                #region Aktivite - Yer göstermesi belgesi ekleme
                context.Response.ContentType = "application/json";

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string message = string.Empty;
                var base64Data = context.Request.Form["data"];
                var activityId = context.Request.Form["activityid"];
                var activityType = context.Request.Form["activityType"];

                IOrganizationService service = MSCRM.GetOrgService(true);

                Entity attach = new Entity("annotation");

                attach["filename"] = "YGB_" + context.Request.Form["name"];
                attach["mimetype"] = context.Request.Form["type"];
                attach["filesize"] = context.Request.Form["size"];
                attach["subject"] = context.Request.Form["name"];
                attach["documentbody"] = base64Data;
                if (activityType.Equals("appointment"))
                {
                    attach["objecttypecode"] = 4201;
                }
                else if (activityType.Equals("phonecall"))
                {
                    attach["objecttypecode"] = 4210;
                }

                attach["isdocument"] = true;

                attach["objectid"] = new EntityReference(activityType, new Guid(activityId));

                service.Create(attach);

                var data = serializer.Serialize(true);
                context.Response.Write(data);
                #endregion
            }
            else if (op != null && op == "8")
            {
                #region Aktivite - Yer göstermesi belgesi indirme
                var id = context.Request.QueryString["id"];
                if (id != null)
                {
                    HttpResponse response = context.Response;
                    Entity ann = GetDisplayLocationAnnByActivityId(new Guid(Convert.ToString(id)));
                    if (ann == null)
                    {
                        response.Write("<center><h3>Dosya Bulunamadı.</h3></center>");
                        return;
                    }

                    byte[] toEncryptArray = Convert.FromBase64String(ann.GetAttributeValue<string>("documentbody"));
                    string fileName = ann.GetAttributeValue<string>("filename");
                    response.ClearContent();
                    response.ContentType = ann.GetAttributeValue<string>("mimetype");
                    response.CacheControl = "no-cache";
                    response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    response.AddHeader("Content-Length", toEncryptArray.Length.ToString());
                    response.BinaryWrite(toEncryptArray);
                    response.Buffer = true;
                    response.End();
                }
                #endregion
            }

            if (op != null && op == "9")
            {
                #region Yetki Belgesi Dokümanı Oluşturma
                context.Response.ContentType = "application/json";

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string message = string.Empty;
                var base64Data = context.Request.Form["data"];
                var contactId = context.Request.Form["documentid"];
                IOrganizationService service = MSCRM.GetOrgService(true);

                Entity attach = new Entity("annotation");

                attach["filename"] = context.Request.Form["name"];
                attach["mimetype"] = context.Request.Form["type"];
                attach["filesize"] = context.Request.Form["size"];
                attach["subject"] = context.Request.Form["name"];
                attach["documentbody"] = base64Data;
                attach["objecttypecode"] = 10085;
                attach["isdocument"] = true;
                attach["objectid"] = new EntityReference("new_registrationdoc", new Guid(contactId));

                service.Create(attach);

                var data = serializer.Serialize(true);
                context.Response.Write(data);
                #endregion
            }
            else if (op != null && op == "10")
            {
                #region Aktivite - Yer göstermesi belgesi indirme
                var id = context.Request.QueryString["id"];
                if (id != null)
                {
                    HttpResponse response = context.Response;
                    Entity ann = GetAuthorityDocById(new Guid(Convert.ToString(id)));
                    if (ann == null)
                    {
                        response.Write("<center><h3>Dosya Bulunamadı.</h3></center>");
                        return;
                    }

                    byte[] toEncryptArray = Convert.FromBase64String(ann.GetAttributeValue<string>("documentbody"));
                    string fileName = ann.GetAttributeValue<string>("filename");
                    response.ClearContent();
                    response.ContentType = ann.GetAttributeValue<string>("mimetype");
                    response.CacheControl = "no-cache";
                    response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    response.AddHeader("Content-Length", toEncryptArray.Length.ToString());
                    response.BinaryWrite(toEncryptArray);
                    response.Buffer = true;
                    response.End();
                }
                #endregion
            }
        }

        /// <summary>
        /// Nüfus Cüzdanı Dökümanları alınıyor
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        private Entity GetAnnotionByContactId(Guid contactId)
        {
            Entity retVal = new Entity();

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "objectid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(contactId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "filename";
            con2.Operator = ConditionOperator.Like;
            con2.Values.Add("%CRM_NFZ%");


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("annotation");
            Query.ColumnSet = new ColumnSet("objectid", "filename", "documentbody", "mimetype", "annotationid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            Query.AddOrder("createdon", OrderType.Descending);
            EntityCollection Result = MSCRM.AdminOrgService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                retVal = Result.Entities[0];
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }

        /// <summary>
        /// Kiralama kayıtları alınıyor
        /// </summary>
        /// <param name="rentalid"></param>
        /// <returns></returns>
        private Entity GetAnnotionByRentalRecordId(Guid rentalid)
        {
            Entity retVal = new Entity();

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "objectid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(rentalid);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "filename";
            con2.Operator = ConditionOperator.Like;
            con2.Values.Add("%TT_%");


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("annotation");
            Query.ColumnSet = new ColumnSet("objectid", "filename", "documentbody", "mimetype", "annotationid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            Query.AddOrder("createdon", OrderType.Descending);
            EntityCollection Result = MSCRM.AdminOrgService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                retVal = Result.Entities[0];
            }
            else
            {
                retVal = null;
            }
            return retVal;

        }
        /// <summary>
        /// Yer Gösterme belgeleri
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private Entity GetDisplayLocationAnnByActivityId(Guid activityId)
        {
            Entity retVal = new Entity();

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "objectid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(activityId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "filename";
            con2.Operator = ConditionOperator.Like;
            con2.Values.Add("%YGB_%");


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("annotation");
            Query.ColumnSet = new ColumnSet("objectid", "filename", "documentbody", "mimetype", "annotationid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            Query.AddOrder("createdon", OrderType.Descending);
            EntityCollection Result = MSCRM.AdminOrgService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                retVal = Result.Entities[0];
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }

        private Entity GetAuthorityDocById(Guid documentId)
        {
            Entity retVal = new Entity();

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "objectid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(documentId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("annotation");
            Query.ColumnSet = new ColumnSet("objectid", "filename", "documentbody", "mimetype", "annotationid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            Query.AddOrder("createdon", OrderType.Descending);
            EntityCollection Result = MSCRM.AdminOrgService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                retVal = Result.Entities[0];
            }
            else
            {
                retVal = null;
            }
            return retVal;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}