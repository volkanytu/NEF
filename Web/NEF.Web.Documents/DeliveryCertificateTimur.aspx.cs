using NEF.Web.Documents.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NEF.Web.Documents
{
    public partial class DeliveryCertificateTimur : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid productId = Guid.Parse(this.Request.QueryString["Id"]);

            string fileName = new DeliveryCertificateTimurHelper().ExecuteDocumentWritenRecordForm(productId, this.Server.MapPath("/"));

            if (fileName != string.Empty)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    this.Response.ClearContent();
                    this.Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Teslim Tutanağı BTE - Timur.docx"));
                    this.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    this.Response.ContentType = "application/octet-stream";
                    string filename = "/DocumentMerge/Document/" + productId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                    this.Response.TransmitFile(filename);
                    this.Response.End();
                }
            }
        }
    }
}