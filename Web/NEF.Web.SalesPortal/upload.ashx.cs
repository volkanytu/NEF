using Microsoft.Office.Interop.Excel;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace NEF.Web.SalesPortal
{
    /// <summary>
    /// Summary description for upload
    /// </summary>
    public class upload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string message = string.Empty;

            var op = context.Request.QueryString["operation"];


            if (op != null && op == "1")
            {
                var base64Data = context.Request.Form["data"];
                var quoteId = context.Request.Form["quoteid"];

                IOrganizationService service = MSCRM.GetOrgService(true);

                Entity attach = new Entity("annotation");

                attach["filename"] = context.Request.Form["name"];
                attach["mimetype"] = context.Request.Form["type"];
                attach["filesize"] = context.Request.Form["size"];
                attach["subject"] = context.Request.Form["name"];
                attach["documentbody"] = base64Data;
                attach["objecttypecode"] = 1084;
                attach["isdocument"] = true;
                attach["objectid"] = new EntityReference("quote", new Guid(quoteId));

                service.Create(attach);

                var data = serializer.Serialize(true);
                context.Response.Write(data);
            }
            else if (op == "2")
            {
                var data = context.Request.Form["data"];

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<Activity> phoneCallList = js.Deserialize<List<Activity>>(data);
                MsCrmResult result = ExportPhoneCall(phoneCallList);

                var dataRes = serializer.Serialize(result);
                context.Response.Write(dataRes);
            }


        }

        public MsCrmResult ExportPhoneCall(List<Activity> phoneCallList)
        {
            MsCrmResult result = new MsCrmResult();
            System.Data.DataTable dt = new System.Data.DataTable("PhoneCall");
            dt.Columns.Add("Aktivite ID");
            dt.Columns.Add("Müşteri");
            dt.Columns.Add("Kullanıcı");
            dt.Columns.Add("Öncelik");
            dt.Columns.Add("Oluşturulma Tarihi");
            dt.Columns.Add("Telefon Numarası");
            dt.Columns.Add("İlgilendiği Proje");
            dt.Columns.Add("Mesaj");

            foreach (Activity ac in phoneCallList)
            {
                DataRow dr = dt.NewRow();
                dr[0] = ac.ActivityId.ToString();
                dr[1] = ac.Contact.Name.ToString();
                dr[2] = ac.Owner.Name.ToString();
                dr[3] = ac.PriorityString.ToString();
                dr[4] = ac.CreatedOnString.ToString();
                dr[5] = string.IsNullOrEmpty(ac.PhoneNumber) ? string.Empty : ac.PhoneNumber.Replace("+90", "").Replace("-", "");
                dr[6] = string.IsNullOrEmpty(ac.ProjectName) ? string.Empty : ac.ProjectName;
                dr[7] = string.IsNullOrEmpty(ac.ContactMessage) ? string.Empty : ac.ContactMessage;
                dt.Rows.Add(dr);
            }

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            try
            {

                Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Worksheet ws = (Worksheet)wb.Worksheets[1];
                excelApp.Visible = false;
                ws.Name = dt.TableName;
                for (int i = 1; i < dt.Columns.Count + 1; i++)
                {
                    ws.Cells[1, i] = dt.Columns[i - 1].ColumnName;
                }

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        ws.Cells[j + 2, k + 1] = dt.Rows[j].ItemArray[k].ToString();
                    }
                }
                if (File.Exists(@Globals.PortalAttachmentFolder + "data.xlsx"))
                {
                    File.Delete(@Globals.PortalAttachmentFolder + "data.xlsx");
                }


                wb.SaveAs(@Globals.PortalAttachmentFolder + "data.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                wb.Close();
                result.Success = true;
                result.Result = @Globals.PortalAttachmentFolder + "data.xlsx";

            }
            catch (Exception)
            {

                result.Success = false;
                result.Result = string.Empty;
            }
            finally
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
            }


            return result;

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}