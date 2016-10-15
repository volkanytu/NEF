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
    public partial class LossAssessment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid productId = Guid.Parse(this.Request.QueryString["Id"]);

            string fileName = this.Server.MapPath("/") + "\\DocumentMerge\\Templates\\LossAssessment.docx";

            if (fileName != string.Empty)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    this.Response.ClearContent();
                    this.Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("HASAR TESPİT TUTANAĞI.docx"));
                    this.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    this.Response.ContentType = "application/octet-stream";
                    this.Response.TransmitFile(fileName);
                    this.Response.End();
                }
            }
        }
    }
}