using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using System.Web.Script.Serialization;

using CsvParser;
using System.Data;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.Web.ProductImport
{
    /// <summary>
    /// Summary description for process
    /// </summary>
    public class process : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                var base64Data = context.Request.Form["data"];

                if (base64Data != null)
                {
                    #region |ADD CSV TO DATATABLE TO CHECK|

                    CsvParse pars = new CsvParse(base64Data, CsvParser.CsvParse.delimiter.NoktalıVirgül);

                    DataTable dt = new DataTable();

                    dt = pars.GetDataTableFromCSV();

                    #endregion

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        returnValue.Result = "Yanlış içerik veya kayıt yok. <br /> Lütfen kontrol ediniz.";
                    }
                    else
                    {
                        if (isColumnNamesOK(dt))
                        {
                            IOrganizationService service;
                            service = MSCRM.GetOrgService(true);

                            Entity ent = new Entity("new_houseimport");
                            ent["new_name"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                            ent["statuscode"] = new OptionSetValue(1); //Beklemede

                            Guid importId = service.Create(ent);

                            Entity attach = new Entity("annotation");

                            attach["filename"] = context.Request.Form["name"];
                            attach["mimetype"] = context.Request.Form["type"];
                            attach["filesize"] = context.Request.Form["size"];
                            attach["subject"] = "Konut Import Dosyası";
                            attach["documentbody"] = base64Data;
                            attach["objecttypecode"] = Globals.HouseImportObjectTypeCode;
                            attach["isdocument"] = true;
                            attach["objectid"] = new EntityReference("new_houseimport", importId);

                            service.Create(attach);

                            returnValue.Success = true;
                            returnValue.Result = "Dosya başarılı bir şekilde import edildi.<br/> İşlem tamamlandığında mail ile bilgilendirme yapılacaktır.";

                        }
                        else
                        {
                            returnValue.Result = "Dosya içerisindeki kolon isimleri uyuşmuyor.<br/> Lütfen kontrol ediniz.";
                        }

                    }
                }
                else
                {
                    returnValue.Result = "Dosya içeriği gönderilmedi.<br/> Lütfen kontrol ediniz.";
                }

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            var data = serializer.Serialize(returnValue);
            context.Response.Write(data);
        }

        bool isColumnNamesOK(DataTable dt)
        {
            bool returnValue = true;

            string[] columnNames = new string[] { 
            "Proje","Blok","Kat","Daire No","Daire Kimlik No","Ünite Tipi","Genel Daire Tipi","Daire Tipi","Aks","Konum","Ruhsat No","Açıklama","Yön","Net M2","Balkon M2","Teras M2",
            "Depo M2","Brüt M2","Liste Fiyatı","Para Birimi","Kdv","Damga Vergisi","Paylaşım",
            "İl","İlçe","Mahalle","Pafta","Ada","Parsel"};

            for (int i = 0; i < columnNames.Length; i++)
            {
                var query = (from DataColumn a in dt.Columns
                             where a.ColumnName == columnNames[i]
                             select a.ColumnName).ToList();
                if (query.Count == 0)
                    returnValue = false;
            }

            return returnValue;
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