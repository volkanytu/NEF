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
    public partial class contratCoverEng : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Request.QueryString["Id"] != null)
                {
                    Guid quoteId = Guid.Parse(this.Request.QueryString["Id"]);
                    string fileName = new ContractCoverEngHelper().ExecuteContractCover(quoteId, this.Server.MapPath("/"));
                    if (!(fileName != string.Empty))
                        return;
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.Exists)
                    {
                        if (ContractCoverEngHelper.projectNameGlobal == "857 NEF 12 Merter" || ContractCoverEngHelper.projectNameGlobal == "855 NEF 13 Merter" ||
                            ContractCoverEngHelper.projectNameGlobal == "845 NEF 98 Points" || ContractCoverEngHelper.projectNameGlobal == "877 NEF 11 Kağıthane")
                        {
                            this.Response.ClearContent();
                            //this.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                            this.Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Sözleşme Kapağı Ing.docx"));
                            this.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                            this.Response.ContentType = "application/octet-stream";
                            string filename = "/DocumentMerge/Document/" + quoteId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                            this.Response.TransmitFile(filename);
                            this.Response.End();
                        }
                        else
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
                                Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Sözleşme Kapağı Ing.pdf"));
                                Response.TransmitFile(fileName.Replace("docx", "pdf"));
                                Response.End();
                            }
                        }


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