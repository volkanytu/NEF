using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NEF.Web.Documents.Business
{
    public class ProductAdmittingProxyHelper
    {
        IOrganizationService service;
        internal string ExecuteDocumentWritenRecordForm(Guid productId, string Path)
        {
            string folder = ProductAdmittingProxyHelper.CreateFolder(productId, Path);
            string projectName = string.Empty;
            string referans = string.Empty;
            string blok = string.Empty;
            Guid projectId = Guid.Empty;
            string freeSectionIdNumber = string.Empty;
            string fullName = string.Empty;
            string ada = string.Empty;
            string productCity = string.Empty;
            string parcel = string.Empty;
            string productDistrict = string.Empty;
            string quarter = string.Empty;
            Entity quote = null;
            Entity contact = null;
            Entity account = null;
            string licenceNumber = string.Empty;

            service = MSCRM.AdminOrgService;
            Entity product = service.Retrieve("product", productId, new ColumnSet(true));
            licenceNumber = product.Contains("new_licencenumber") ? (string)product.Attributes["new_licencenumber"] : string.Empty;
            projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
            projectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
            blok = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
            parcel = product.Contains("new_parcelid") ? ((EntityReference)product.Attributes["new_parcelid"]).Name : string.Empty;
            ada = product.Contains("new_blockofbuildingid") ? ((EntityReference)product.Attributes["new_blockofbuildingid"]).Name : string.Empty;
            quarter = product.Contains("new_quarter") ? (string)product.Attributes["new_quarter"] : string.Empty;
            productDistrict = product.Contains("new_district") ? (string)product.Attributes["new_district"] : string.Empty;
            productCity = product.Contains("new_city") ? (string)product.Attributes["new_city"] : string.Empty;
            freeSectionIdNumber = product.Contains("new_freesectionidnumber") ? (string)product.Attributes["new_freesectionidnumber"] : string.Empty;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "productid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(productId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "quotestatecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(1);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet("quoteid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                quote = service.Retrieve("quote", ((EntityReference)Result.Entities[0].Attributes["quoteid"]).Id, new ColumnSet(true));
                if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "contact")
                {
                    contact = service.Retrieve("contact", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                    fullName = contact.Contains("fullname") ? (string)contact.Attributes["fullname"] : string.Empty;
                }
                else if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "account")
                {
                    account = service.Retrieve("account", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));

                }
            }
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            dictionary1.Add("BagimsizNo", freeSectionIdNumber);
            dictionary1.Add("Blok", blok);
            dictionary1.Add("Ada", ada);
            dictionary1.Add("Mahalle", quarter);
            dictionary1.Add("Parsel", parcel);
            dictionary1.Add("İl", productCity);
            dictionary1.Add("İlçe", productDistrict);
            dictionary1.Add("day", DateTime.Now.ToString("dd"));
            dictionary1.Add("month", DateTime.Now.ToString("MM"));
            dictionary1.Add("year", DateTime.Now.ToString("yyyy"));
            if (projectName == "853 NEF 03 Kağıthane")
            {
                dictionary1.Add("Proje", "NEF KAĞITHANE 03");
            }
            else
            {
                dictionary1.Add("Proje", projectName.Substring(7, projectName.Length - 7).Trim());
            }
            dictionary1.Add("İlgiliKişi", fullName);
            dictionary1.Add("ruhsatno", licenceNumber);
            byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\ProductAdmittingProxy.docx", (DataSet)null, dictionary1);
            string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\ProductAdmittingProxy.docx";
            if (path1 != string.Empty)
                System.IO.File.WriteAllBytes(path1, bytes);
            return path1;
        }

        private static string CreateFolder(Guid QuoteId, string Path)
        {
            string str1 = QuoteId.ToString();
            if (!Directory.Exists(Path + "\\DocumentMerge"))
                Directory.CreateDirectory(Path + "\\DocumentMerge");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document"))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document\\" + str1))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document\\" + str1);
            return str1;
        }

    }
}
