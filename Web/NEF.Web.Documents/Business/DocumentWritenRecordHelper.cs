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
    public class DocumentWritenRecordHelper
    {
        IOrganizationService service;

        internal string ExecuteDocumentWritenRecordForm(Guid productId, string Path)
        {
            string folder = DocumentWritenRecordHelper.CreateFolder(productId, Path);
            string projectName = string.Empty;
            string apartmentNo = string.Empty;
            string licenceNumber = string.Empty;
            service = MSCRM.AdminOrgService;
            Entity product = service.Retrieve("product", productId, new ColumnSet(true));
            projectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
            apartmentNo = product.Contains("new_homenumber") ? (string)product.Attributes["new_homenumber"] : string.Empty;
            licenceNumber = product.Contains("new_licencenumber") ? (string)product.Attributes["new_licencenumber"] : string.Empty;

            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          
            dictionary1.Add("daireno", apartmentNo);
            dictionary1.Add("day", DateTime.Now.ToString("dd"));
            dictionary1.Add("month", DateTime.Now.ToString("MM"));
            dictionary1.Add("year", DateTime.Now.ToString("yyyy"));
            dictionary1.Add("ruhsatno", licenceNumber);

            if (projectName == "853 NEF 03 Kağıthane")
            {
                dictionary1.Add("Proje", "NEF KAĞITHANE 03");
            }
            else
            {
                dictionary1.Add("Proje", projectName.Substring(7, projectName.Length - 7).Trim());
            }

            byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\DocumentWritenRecord.docx", (DataSet)null, dictionary1);
            string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\DocumentWritenRecord.docx";
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
