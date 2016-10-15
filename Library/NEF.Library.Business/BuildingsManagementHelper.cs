using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace NEF.Library.Business
{
    public static class BuildingsManagementHelper
    {
        internal static void CreatePortal(Entity entity, SqlDataAccess sda, IOrganizationService adminService)
        {
            try
            {
                string address = "http://siteyonetim.nef.com.tr/servlets/RequesterServlet";
                NameValueCollection data = new NameValueCollection();

                data.Add("operation", "AddRequester");
                if (entity.Contains("new_contactid"))
                {
                    Entity contact = adminService.Retrieve("contact", ((EntityReference)entity.Attributes["new_contactid"]).Id, new ColumnSet("fullname"));
                    string name = (string)contact.Attributes["fullname"];
                    data.Add("name", name);
                }
                if (entity.Contains("new_project") && ((EntityReference)entity.Attributes["new_project"]) != null)
                {
                    Entity project = adminService.Retrieve("new_project", ((EntityReference)entity.Attributes["new_project"]).Id, new ColumnSet("new_name"));
                    string projectName = (string)project.Attributes["new_name"];
                    data.Add("site", projectName.Substring(4, projectName.Length - 4));
                }
                if (entity.Contains("new_block") && ((EntityReference)entity.Attributes["new_block"]) != null)
                {
                    Entity blok = adminService.Retrieve("new_block", ((EntityReference)entity.Attributes["new_block"]).Id, new ColumnSet("new_name"));
                    string blokName = (string)blok.Attributes["new_name"];
                    data.Add("phone", blokName);
                }
                if (entity.Contains("new_productid") && ((EntityReference)entity.Attributes["new_productid"]) != null)
                {
                    Entity product = adminService.Retrieve("product", ((EntityReference)entity.Attributes["new_productid"]).Id, new ColumnSet("name"));
                    string productName = (string)product.Attributes["name"];
                    data.Add("jobTitle", productName);
                }
                if (entity.Contains("new_mobilephone") && ((string)entity.Attributes["new_mobilephone"]) != null)
                {
                    data.Add("mobile", (string)entity.Attributes["new_mobilephone"]);
                }

                if (entity.Contains("new_ruhsatno") && ((string)entity.Attributes["new_ruhsatno"]) != null)
                {
                    data.Add("departmentName", (string)entity.Attributes["new_ruhsatno"]);
                }
                else
                {
                    data.Add("departmentName", "Toplum");
                }
                if (entity.Contains("new_outdate") && ((DateTime)entity.Attributes["new_outdate"]) != null)
                {
                    data.Add("Çıkış Tarihi", ((DateTime)entity.Attributes["new_outdate"]).ToLocalTime().ToString("dd.MM.yyyy"));
                }

                if (entity.Contains("new_entrydate") && ((DateTime)entity.Attributes["new_entrydate"]) != null)
                {
                    data.Add("Giriş Tarihi", ((DateTime)entity.Attributes["new_entrydate"]).ToLocalTime().ToString("dd.MM.yyyy"));
                }

                if (entity.Contains("new_m2") && ((string)entity.Attributes["new_m2"]) != null)
                {
                    data.Add("m2", (string)entity.Attributes["new_m2"]);
                }

                if (entity.Contains("new_generaltypeofhome") && ((EntityReference)entity.Attributes["new_generaltypeofhome"]) != null)
                {
                    Entity generalTypeofHome = adminService.Retrieve("new_generaltypeofhome", ((EntityReference)entity.Attributes["new_generaltypeofhome"]).Id, new ColumnSet("new_name"));
                    string name = (string)generalTypeofHome.Attributes["new_name"];
                    data.Add("Daire Tipi", name);
                }
                if (entity.Contains("new_kat") && entity.Attributes["new_kat"] != null)
                {
                    data.Add("Kat", entity.Attributes["new_kat"].ToString());
                }

                if (entity.Contains("new_residenttype"))
                {
                    if (((OptionSetValue)entity.Attributes["new_residenttype"]).Value == 1)
                    {
                        data.Add("Sakin Tipi", "Kiracı");
                    }
                    if (((OptionSetValue)entity.Attributes["new_residenttype"]).Value == 2)
                    {
                        data.Add("Sakin Tipi", "Malik");
                    }
                    if (((OptionSetValue)entity.Attributes["new_residenttype"]).Value == 3)
                    {
                        data.Add("Sakin Tipi", "Toplum");
                    }
                }

                if (entity.Contains("statuscode") && ((OptionSetValue)entity.Attributes["statuscode"]) != null)
                {
                    data.Add("Durum", ((OptionSetValue)entity.Attributes["statuscode"]).Value == 1 ? "Aktif" : "Pasif");
                }

                if (entity.Contains("new_emailaddress") && entity.Attributes["new_emailaddress"] != null)
                {
                    data.Add("email", (string)entity.Attributes["new_emailaddress"]);

                }

                data.Add("username", "crmadmin");
                data.Add("password", "ZXas123456");
                data.Add("logonDomainName", "AD_AUTH");
                data.Add("DOMAIN_NAME", "NEF");

                WebClient wC = new WebClient();
                wC.Headers.Add("Cache-Control", "no-cache");

                byte[] bytes = wC.UploadValues(address, data);
                if (bytes == null)
                    return;
                string @string = Encoding.GetEncoding("windows-1254").GetString(bytes);
                if (!string.IsNullOrEmpty(@string) && @string.Contains("userid"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(@string);
                    if (xmlDocument.GetElementsByTagName("operationstatus").Item(0).InnerText == "Success")
                    {
                        string innerText = xmlDocument.GetElementsByTagName("userid").Item(0).InnerText;
                        entity.Attributes.Add("new_recordid", innerText);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        internal static void UpdatePortal(Entity entity, IOrganizationService adminService)
        {
            try
            {
                string address = "http://siteyonetim.nef.com.tr/servlets/RequesterServlet";
                NameValueCollection data = new NameValueCollection();

                if (!entity.Contains("new_recordid") || entity.Attributes["new_recordid"] == null)
                {
                    data.Add("operation", "AddRequester");
                }
                else
                {
                    data.Add("operation", "UpdateRequester");
                    data.Add("reqUserName", "crmadmin");
                    data.Add("reqLoginName", "ZXas123456");
                    data.Add("reqDomainName", "NEF");
                    data.Add("userid", (string)entity.Attributes["new_recordid"]);
                }

                if (entity.Contains("new_contactid"))
                    data.Add("name", ((EntityReference)entity.Attributes["new_contactid"]).Name);
                if (entity.Contains("new_project"))
                    data.Add("site", ((EntityReference)entity.Attributes["new_project"]).Name.Substring(4, ((EntityReference)entity.Attributes["new_project"]).Name.Length - 4));
                if (entity.Contains("new_block"))
                    data.Add("phone", ((EntityReference)entity.Attributes["new_block"]).Name);
                if (entity.Contains("new_productid"))
                    data.Add("jobTitle", ((EntityReference)entity.Attributes["new_productid"]).Name);
                if (entity.Contains("new_mobilephone"))
                    data.Add("mobile", (string)entity.Attributes["new_mobilephone"]);
                if (entity.Contains("new_ruhsatno"))
                    data.Add("departmentName", (string)entity.Attributes["new_ruhsatno"]);
                if (entity.Contains("new_outdate"))
                    data.Add("Çıkış Tarihi", ((DateTime)entity.Attributes["new_outdate"]).ToLocalTime().ToString("dd.MM.yyyy"));
                if (entity.Contains("new_entrydate"))
                    data.Add("Giriş Tarihi", ((DateTime)entity.Attributes["new_entrydate"]).ToLocalTime().ToString("dd.MM.yyyy"));
                if (entity.Contains("new_m2"))
                    data.Add("m2", (string)entity.Attributes["new_m2"]);
                if (entity.Contains("new_generaltypeofhome"))
                    data.Add("Daire Tipi", ((EntityReference)entity.Attributes["new_generaltypeofhome"]).Name);
                if (entity.Contains("new_kat"))
                    data.Add("Kat", entity.Attributes["new_kat"].ToString());
                if (entity.Contains("new_residenttype"))
                {
                    if (((OptionSetValue)entity.Attributes["new_residenttype"]).Value == 1)
                    {
                        data.Add("Sakin Tipi", "Kiracı");
                    }
                    if (((OptionSetValue)entity.Attributes["new_residenttype"]).Value == 2)
                    {
                        data.Add("Sakin Tipi", "Malik");
                    }
                    if (((OptionSetValue)entity.Attributes["new_residenttype"]).Value == 3)
                    {
                        data.Add("Sakin Tipi", "Toplum");
                    }
                }
                if (entity.Contains("statuscode"))
                    data.Add("Durum", ((OptionSetValue)entity.Attributes["statuscode"]).Value == 1 ? "Aktif" : "Pasif");
                //if (entity.Contains("new_name"))
                //    data.Add("loginName", (string)entity.Attributes["new_name"]);
                if (entity.Contains("new_emailaddress"))
                    data.Add("email", (string)entity.Attributes["new_emailaddress"]);
                data.Add("username", "crmadmin");
                data.Add("password", "ZXas123456");
                data.Add("logonDomainName", "AD_AUTH");
                data.Add("DOMAIN_NAME", "NEF");
                byte[] bytes = new WebClient().UploadValues(address, data);
                if (bytes == null)
                    return;
                string @string = Encoding.GetEncoding("windows-1254").GetString(bytes);
                if (!string.IsNullOrEmpty(@string) && @string.Contains("userid"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(@string);
                    if (xmlDocument.GetElementsByTagName("operationstatus").Item(0).InnerText == "Success")
                    {
                        string innerText = xmlDocument.GetElementsByTagName("userid").Item(0).InnerText;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static void SetNumber(Entity entity, SqlDataAccess sda, IOrganizationService adminService)
        {
            if (!entity.Contains("new_project"))
                return;
            Entity project = adminService.Retrieve("new_project", ((EntityReference)entity.Attributes["new_project"]).Id, new ColumnSet("new_name"));
            string projectNumber = ((string)project.Attributes["new_name"]).Substring(0, 3);
            #region SQL QUERY
            string sqlQuery = @"SELECT TOP 1
								 CONVERT(int ,SUBSTRING(BM.new_name,4,5)) lastNumber							 
                                 FROM 
	                             new_buildingsmanagement BM (NOLOCK)
                                 WHERE 
								 BM.new_buildingsmanagementId!='{0}'
                                 and
                                 BM.new_name IS NOT NULL
								 and
								 SUBSTRING(BM.new_productidName,0,4)='{1}'
								 and
								 BM.new_productidName NOT LIKE '%Omerd%'								 
                                 ORDER BY
	                             CONVERT(int ,SUBSTRING(BM.new_name,4,5)) DESC";
            sqlQuery = string.Format(sqlQuery, entity.Id, projectNumber);
            DataTable dt = sda.getDataTable(sqlQuery);
            #endregion SQL QUERY

            #region Set NUMBER
            string lastNumber = dt.Rows.Count > 0 ? dt.Rows[0]["lastNumber"] != DBNull.Value ? dt.Rows[0]["lastNumber"].ToString() : string.Empty : string.Empty;
            if (lastNumber != string.Empty)
            {
                entity["new_name"] = projectNumber + (Convert.ToInt32(lastNumber) + 1).ToString();
            }
            else
            {
                entity["new_name"] = projectNumber + "10000";
            }
            #endregion Set NUMBER

        }

        internal static void CheckRecordDublicate(Entity entity, SqlDataAccess sda, IOrganizationService adminService)
        {
            if (!entity.Contains("new_contactid"))
                return;
            if (!entity.Contains("new_productid"))
                return;
            Guid contactId = ((EntityReference)entity.Attributes["new_contactid"]).Id;
            Guid productId = ((EntityReference)entity.Attributes["new_productid"]).Id;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_productid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(productId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_contactid";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(contactId);

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_buildingsmanagementid";
            con3.Operator = ConditionOperator.NotEqual;
            con3.Values.Add(entity.Id);


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("new_buildingsmanagement");
            Query.ColumnSet = new ColumnSet("new_buildingsmanagementid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = adminService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                throw new InvalidPluginExecutionException("Bu Kişi ve Konutun daha önceden site yönetim kaydı vardır.");
            }
        }
    }
}
