using Microsoft.Office.Interop.Word;
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
    public partial class BankCreditPlan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.Request.QueryString["Id"] != null)
                {
                    Guid bankCreditPlanId = Guid.Parse(this.Request.QueryString["Id"]);

                    //Guid bankCreditPlanId = new Guid("B5CB0593-F308-E511-80D6-005056A60603");
                    string fileName = new BankCreditHelper().ExecuteContractCover(bankCreditPlanId, this.Server.MapPath("/"));
                    
                    if (BankCreditHelper.projectNameGlobal.Equals("853 NEF 03 Kağıthane") ||
                                         BankCreditHelper.projectNameGlobal.Equals("857 NEF 12 Merter") ||
                                         BankCreditHelper.projectNameGlobal.Equals("855 NEF 13 Merter"))
                    {
                        // akbank işbank garanti
                        if (BankCreditHelper.bankNameGlobal.Equals("Akbank T.A.Ş."))
                        {
                            if (!(fileName != string.Empty))
                                return;
                            CreateFile(bankCreditPlanId, fileName);
                        }
                        else if (BankCreditHelper.bankNameGlobal.Equals("Türkiye İş Bankası A.Ş."))
                        {
                            if (!(fileName != string.Empty))
                                return;
                            CreateFile(bankCreditPlanId, fileName);
                        }
                        else if (BankCreditHelper.bankNameGlobal.Equals("Türkiye Garanti Bankası A.Ş."))
                        {
                            if (!(fileName != string.Empty))
                                return;
                            CreateFile(bankCreditPlanId, fileName);
                        }
                        else
                        {
                            if (!(fileName != string.Empty))
                                return;
                            this.lblUyari.Text = "İlgili banka kredi kaydına ait doküman bulunmamaktadır.";
                            return;
                        }
                    }
                    else if (BankCreditHelper.projectNameGlobal.Equals("827 Inistanbul Topkapı"))
                    {
                        if (BankCreditHelper.bankNameGlobal.Equals("Türkiye İş Bankası A.Ş.") || 
                            BankCreditHelper.bankNameGlobal.Equals("Yapı ve Kredi Bankası A.Ş.") ||
                            BankCreditHelper.bankNameGlobal.Equals("Türkiye Garanti Bankası A.Ş.") ||
                            BankCreditHelper.bankNameGlobal.Equals("Türkiye Vakıflar Bankası T.A.O."))
                        {
                            if (!(fileName != string.Empty))
                                return;
                            CreateFile(bankCreditPlanId, fileName);
                        }
                        else
                        {
                            this.lblUyari.Text = "İlgili banka kredi kaydına ait doküman bulunmamaktadır.";
                            return;
                        }
                    }
                    else if (BankCreditHelper.projectNameGlobal.Equals("847 NEF 08 Kağıthane") ||
                                                      BankCreditHelper.projectNameGlobal.Equals("837 NEF 06 Points") ||
                                                      BankCreditHelper.projectNameGlobal.Equals("843 NEF 04 Points") ||
                                                      BankCreditHelper.projectNameGlobal.Equals("841 NEF 25 Şişli") ||
                                                      BankCreditHelper.projectNameGlobal.Equals("831 NEF 14 Kağıthane"))
                    {
                        if (BankCreditHelper.bankNameGlobal.Equals("Denizbank A.Ş."))
                        {
                            if (!(fileName != string.Empty))
                                return;
                            CreateFile(bankCreditPlanId, fileName);
                        }
                        else
                        {
                            this.lblUyari.Text = "İlgili banka kredi kaydına ait doküman bulunmamaktadır.";
                            return;
                        }
                    }
                    else if (BankCreditHelper.projectNameGlobal.Equals("833 NEF 22 Ataköy"))
                    {
                        if (BankCreditHelper.bankNameGlobal.Equals("Türkiye İş Bankası A.Ş.") || BankCreditHelper.bankNameGlobal.Equals("Odea Bank A.Ş."))
                        {
                            if (!(fileName != string.Empty))
                                return;
                            CreateFile(bankCreditPlanId, fileName);
                        }
                        else
                        {
                            this.lblUyari.Text = "İlgili banka kredi kaydına ait doküman bulunmamaktadır.";
                            return;
                        }
                    }
                    else
                    {
                        this.lblUyari.Text = "İlgili projeye ait doküman bulunmamaktadır.";
                        return;

                    }
                }
                else
                    this.lblUyari.Text = "İlgili banka kredi kaydına ulaşılamadı.";
            }
            catch (Exception ex)
            {
                this.lblUyari.Text = ex.Message;
            }
        }

        private void CreateFile(Guid bankCreditPlanId, string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            if (fileInfo.Exists)
            {
                this.Response.ClearContent();
                this.Response.AppendHeader("Content-Disposition", "attachment;filename*=UTF-8''" + Uri.EscapeDataString("Banka Kredi Eki.docx"));
                this.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                this.Response.ContentType = "application/octet-stream";
                string filename = "/DocumentMerge/Document/" + bankCreditPlanId.ToString().Replace("{", "").Replace("}", "") + "/" + fileInfo.Name;
                this.Response.TransmitFile(filename);
                this.Response.End();
            }
        }
    }
}