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
    class DeliveryCertificateTimurHelper
    {
        IOrganizationService service;

        internal string ExecuteDocumentWritenRecordForm(Guid productId, string Path)
        {
            string folder = DeliveryCertificateTimurHelper.CreateFolder(productId, Path);
            string projectName = string.Empty;
            string referans = string.Empty;
            string blok = string.Empty;
            string floor = string.Empty;
            string apartmentNo = string.Empty;
            string banka = string.Empty;



            Guid projectId = Guid.Empty;
            string city = string.Empty;
            string address = string.Empty;
            string passportNumber = string.Empty;
            string freeSectionIdNumber = string.Empty;

            string salesAccountName = string.Empty;
            string salesAccountAddress = string.Empty;
            string salesAccountShortName = string.Empty;
            string name = string.Empty;
            string deliveryDate = string.Empty;
            string ada = string.Empty;
            string productCity = string.Empty;

            string productDistrict = string.Empty;
            string quarter = string.Empty;
            string threader = string.Empty;
            string licenceNumber = string.Empty;
            string parcel = string.Empty;
            string contractDate = string.Empty;
            Entity quote = null;
            Entity contact = null;
            Entity account = null;
            Entity SalesAccount = null;


            service = MSCRM.AdminOrgService;



            Entity product = service.Retrieve("product", productId, new ColumnSet(true));
            projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
            projectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
            blok = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
            ada = product.Contains("new_blockofbuildingid") ? ((EntityReference)product.Attributes["new_blockofbuildingid"]).Name : string.Empty;
            floor = product.Contains("new_floornumber") ? product.Attributes["new_floornumber"].ToString() : string.Empty;
            apartmentNo = product.Contains("new_homenumber") ? (string)product.Attributes["new_homenumber"] : string.Empty;
            quarter = product.Contains("new_quarter") ? (string)product.Attributes["new_quarter"] : string.Empty;
            threader = product.Contains("new_threaderid") ? ((EntityReference)product.Attributes["new_threaderid"]).Name : string.Empty;
            parcel = product.Contains("new_parcelid") ? ((EntityReference)product.Attributes["new_parcelid"]).Name : string.Empty;
            productDistrict = product.Contains("new_district") ? (string)product.Attributes["new_district"] : string.Empty;
            productCity = product.Contains("new_city") ? (string)product.Attributes["new_city"] : string.Empty;
            deliveryDate = product.Contains("new_deliverydate") ? ((DateTime)product.Attributes["new_deliverydate"]).ToShortDateString() : string.Empty;
            
            licenceNumber = product.Contains("new_licencenumber") ? (string)product.Attributes["new_licencenumber"] : string.Empty;
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
                contractDate = quote.Contains("new_contractdate") ? ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToShortDateString() : string.Empty;
                banka = quote.Contains("new_bankid") ? ((EntityReference)quote.Attributes["new_bankid"]).Name : string.Empty;
                if (quote.Contains("new_salesshareaccountid"))
                {
                    SalesAccount = service.Retrieve("new_share", ((EntityReference)quote.Attributes["new_salesshareaccountid"]).Id, new ColumnSet(true));
                    salesAccountName = SalesAccount.Contains("new_name") ? SalesAccount.Attributes["new_name"].ToString() : string.Empty;
                    salesAccountAddress = SalesAccount.Contains("new_adressdetail") ? SalesAccount.Attributes["new_adressdetail"].ToString() : string.Empty;
                    salesAccountShortName = SalesAccount.Contains("new_shortname") ? SalesAccount.Attributes["new_shortname"].ToString() : string.Empty;
                }
                if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "contact")
                {
                    contact = service.Retrieve("contact", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                    name = contact.Contains("fullname") ? (string)contact.Attributes["fullname"] : string.Empty;
                    city = contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                    city += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                    city += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;
                    address = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() : string.Empty;
                    address += " " + city;
                    passportNumber = contact.Contains("new_passportnumber") ? (string)contact.Attributes["new_passportnumber"] : string.Empty;
                }
                else if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "account")
                {
                    account = service.Retrieve("account", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                    city = account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                    city += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                    city += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;
                    address = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() : string.Empty;
                    address += " " + city;
                }
            }

            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            dictionary1.Add("İlçe", productDistrict);
            dictionary1.Add("personal", name);
            dictionary1.Add("Mahalle", quarter);
            dictionary1.Add("Ada", ada);
            dictionary1.Add("Pafta", threader);
            dictionary1.Add("Parsel", parcel);
            dictionary1.Add("Blok", blok);
            dictionary1.Add("Kat", floor);
            dictionary1.Add("BagimsizNo", freeSectionIdNumber);
            dictionary1.Add("TeslimTarihi", deliveryDate);
            dictionary1.Add("ruhsatno", licenceNumber);
            dictionary1.Add("sozlesmetarihi", contractDate);
            if (projectName == "853 NEF 03 Kağıthane")
            {
                dictionary1.Add("Proje", "NEF KAĞITHANE 03");
            }
            else
            {
                dictionary1.Add("Proje", projectName.Substring(7, projectName.Length - 7).Trim());
            }


            byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\DeliveryCertificateTimur.docx", (DataSet)null, dictionary1);
            string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\DeliveryCertificateTimur.docx";
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
