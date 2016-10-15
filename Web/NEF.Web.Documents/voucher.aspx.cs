using MessagingToolkit.Barcode.QRCode;
using Microsoft.Office.Interop.Word;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NEF.Web.Documents
{
    public partial class voucher : System.Web.UI.Page
    {

       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Request.QueryString["Id"] != null)
                {                   
                    Guid quoteId = Guid.Parse(this.Request.QueryString["Id"]);
                    string fileName = new VoucherHelper().ExecuteVoucher(quoteId, this.Server.MapPath("/"), "Timur Gayrimenkul");
                    if (!(fileName != string.Empty))
                        return;
                    FileInfo fileInfo = new FileInfo(fileName);                   
                    if (fileInfo.Exists)
                    {
                        if (File.Exists(fileName.Replace("docx", "pdf")))
                        {
                            File.Delete(fileName.Replace("docx", "pdf"));
                        }                       
                        string wordFilename = @"C:/NEF.Web.Documents/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                        string pdfPath = @"C:/NEF.Web.Documents/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/";

                        Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                        Microsoft.Office.Interop.Word.Document wordDocument = new Microsoft.Office.Interop.Word.Document();

                        wordDocument = appWord.Documents.Open(wordFilename);
                        appWord.Documents.Open(wordFilename);
                        wordDocument.ExportAsFixedFormat(pdfPath + fileInfo.Name.Replace("docx", "pdf"), WdExportFormat.wdExportFormatPDF);
                        fileInfo = new FileInfo(fileName.Replace("docx", "pdf"));
                        if (fileInfo.Exists)
                        {

                            appWord.Documents.Close(Type.Missing, Type.Missing, Type.Missing);
                            appWord.Quit(Type.Missing, Type.Missing, Type.Missing);

                            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(appWord);
                            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordDocument);

                            Response.ClearHeaders();
                            Response.ContentType = "application/pdf";
                            Response.Clear();
                            Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Senetler - " + VoucherHelper.quoteNumber + ".pdf"));
                            Response.TransmitFile(fileName.Replace("docx", "pdf"));
                            Response.End();
                        }
                        //this.Response.ClearContent();
                        //this.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                        //this.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                        //this.Response.ContentType = "application/octet-stream";
                        //string filename = "/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                        //this.Response.TransmitFile(filename);
                        //this.Response.End();
                    }
                }
                else
                    this.lblUyari.Text = "Satış Id Eksik.";
            }
            catch (Exception ex)
            {
                this.lblUyari.Text = ex.Message;
            }

        }
             

    }
}