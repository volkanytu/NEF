using Microsoft.Office.Interop.Word;
using NEF.Library.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NEF.Web.Documents
{
    /// <summary>
    /// ÖN SATIŞ FORMU
    /// </summary>
    public partial class prePaymentForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid quoteId = Guid.Parse(this.Request.QueryString["Id"]);

            string fileName = new prePaymentFormHelper().ExecutePrePaymentForm(quoteId, this.Server.MapPath("/"));

            if (fileName != string.Empty)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {

                    //if (File.Exists(fileName.Replace("docx", "pdf")))
                    //{
                    //    File.Delete(fileName.Replace("docx", "pdf"));
                    //}
                    //////PİLOT
                    ////string wordFilename = @"C:/Documents/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                    ////string pdfPath = @"C:/Documents/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/";
                    //////CANLI
                    //string wordFilename = @"C:/NEF.Web.Documents/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                    //string pdfPath = @"C:/NEF.Web.Documents/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/";

                    //Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                    //Microsoft.Office.Interop.Word.Document wordDocument = new Microsoft.Office.Interop.Word.Document();

                    //wordDocument = appWord.Documents.Open(wordFilename);
                    //appWord.Documents.Open(wordFilename);
                    //wordDocument.ExportAsFixedFormat(pdfPath + fileInfo.Name.Replace("docx", "pdf"), WdExportFormat.wdExportFormatPDF);
                    //fileInfo = new FileInfo(fileName.Replace("docx", "pdf"));
                    //if (fileInfo.Exists)
                    //{

                    //    appWord.Documents.Close(Type.Missing, Type.Missing, Type.Missing);
                    //    appWord.Quit(Type.Missing, Type.Missing, Type.Missing);

                    //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(appWord);
                    //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordDocument);

                    //    Response.ClearHeaders();
                    //    Response.ContentType = "application/pdf";
                    //    Response.Clear();
                    //    Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Ön Satış Formu.pdf"));
                    //    Response.TransmitFile(fileName.Replace("docx", "pdf"));
                    //    Response.End();
                    //}

                    this.Response.ClearContent();
                   // this.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                    this.Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Ön Satış Formu.docx"));
                    this.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    this.Response.ContentType = "application/octet-stream";
                    string filename = "/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                    this.Response.TransmitFile(filename);
                    this.Response.End();



                }
            }
        }
    }
}