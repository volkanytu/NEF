using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Crm.Sdk.Messages;
using MessagingToolkit.Barcode.QRCode;
using System.Drawing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.Xrm.Sdk.Client;

namespace NEF.WindowsForm.CreateSalesQRCode
{
    public class MSCRM
    {
        public static IOrganizationService GetOrgService(bool admin = false, string callerId = null, string organization = null)
        {
            ClientCredentials credential = new ClientCredentials();

            //if (Globals.OrganizationServiceUrl.Contains("https"))
            //{

            //}
            //else
            //{
            //    credential.Windows.ClientCredential = admin ? new NetworkCredential("indibox", "Nef1234", "NEF") : CredentialCache.DefaultNetworkCredentials;
            //}

            credential.Windows.ClientCredential = admin ? new NetworkCredential("indibox", "Nef1234", "NEF") : CredentialCache.DefaultNetworkCredentials;
            credential.UserName.UserName = "NEF" + @"\" + "indibox";
            credential.UserName.Password = "Nef1234";


            OrganizationServiceProxy orgServiceProxy = new OrganizationServiceProxy(new Uri("http://fenixcrm.nef.com.tr/FENiX/XRMServices/2011/Organization.svc"), null, credential, null);
            orgServiceProxy.Timeout = new TimeSpan(0, 10, 0);

            if (!string.IsNullOrEmpty(callerId))
            {
                orgServiceProxy.CallerId = new Guid(callerId);
            }
            return orgServiceProxy;
        }

        private static IOrganizationService adminService = null;
        private static readonly object lockthread = new object();

        public static IOrganizationService AdminOrgService
        {
            get
            {
                lock (lockthread)
                {
                    if (adminService == null)
                    {
                        adminService = GetOrgService(true);
                    }
                    return adminService;
                }
            }
        }
    }
    public static class Process
    {
        private static Entity GetCurrencyDetail(Guid id, string[] Columns)
        {
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = "transactioncurrencyid";
            conditionExpression.Operator = ConditionOperator.Equal;
            conditionExpression.Values.Add((object)id);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            columnSet.AddColumns(Columns);



            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)MSCRM.AdminOrgService.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = new QueryExpression()
                {
                    ColumnSet = columnSet,
                    Criteria = filterExpression,
                    EntityName = "transactioncurrency"
                }
            });
            if (multipleResponse.EntityCollection.Entities != null && multipleResponse.EntityCollection.Entities.Count > 0)
                return Enumerable.First<Entity>((IEnumerable<Entity>)multipleResponse.EntityCollection.Entities);
            else
                return (Entity)null;
        }

        public static string CreateQrCode(string quoteNumber)
        {
            string retVal = string.Empty;
            try
            {
                string financialAccount = string.Empty;
                Entity quote = null;
                ConditionExpression quoteCon = new ConditionExpression();
                quoteCon.AttributeName = "quotenumber";
                quoteCon.Operator = ConditionOperator.Equal;
                quoteCon.Values.Add(quoteNumber);

                FilterExpression quoteFilter = new FilterExpression();
                quoteFilter.FilterOperator = LogicalOperator.And;
                quoteFilter.Conditions.Add(quoteCon);


                QueryExpression quoteQuery = new QueryExpression("quote");
                quoteQuery.ColumnSet = new ColumnSet(true);
                quoteQuery.Criteria.FilterOperator = LogicalOperator.And;
                quoteQuery.Criteria.Filters.Add(quoteFilter);
                EntityCollection quoteResult = MSCRM.AdminOrgService.RetrieveMultiple(quoteQuery);

                if (quoteResult.Entities.Count > 0)
                {
                    quote = quoteResult.Entities[0];
                    if (quote.Contains("new_financialaccountid"))
                    {
                        financialAccount = quote.GetAttributeValue<EntityReference>("new_financialaccountid").Name;
                    }
                }
                else
                {
                    retVal = quoteNumber + " satış bulunamadı. Kontrol ediniz";
                    return retVal;
                }



                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "new_quoteid";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(quote.Id);

                ConditionExpression con2 = new ConditionExpression();
                con2.AttributeName = "new_vstatus";
                con2.Values.AddRange((object)2, (object)7);
                con2.Operator = ConditionOperator.NotIn;

                ConditionExpression con3 = new ConditionExpression();
                con3.AttributeName = "new_isvoucher";
                con3.Operator = ConditionOperator.Equal;
                con3.Values.Add(true);

                FilterExpression filter = new FilterExpression();
                filter.FilterOperator = LogicalOperator.And;
                filter.Conditions.Add(con1);
                filter.Conditions.Add(con2);
                filter.Conditions.Add(con3);

                QueryExpression Query = new QueryExpression("new_payment");
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.FilterOperator = LogicalOperator.And;
                Query.Criteria.Filters.Add(filter);
                EntityCollection Result = MSCRM.AdminOrgService.RetrieveMultiple(Query);

                string path = @"C:\\QR_CODES\\";

                string fileName = "QR" + "_" + quoteNumber;
                path = path + fileName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    DeleteDirectory(path);
                    Directory.CreateDirectory(path);
                }

                if (Result.Entities.Count() > 0)
                {
                    foreach (Entity p in Result.Entities)
                    {
                        Entity currencyDetail = GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
                        currencyDetail["currencysymbol"].ToString();

                        CreateQRCodes(path, (string)p["new_vnumber"], financialAccount, ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"),
                                 ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencyDetail["currencysymbol"].ToString());
                    }
                }
                else
                {
                    retVal = quoteNumber + " numaralı satışa ait senet bulunmamaktadır.";
                    return retVal;
                }

                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory != null)
                {
                    FileInfo[] files = directory.GetFiles();
                    CombineImages(files, path, quoteNumber);
                    return "İşlem tamamlandı.";
                }
            }
            catch (Exception ex)
            {
                retVal = "HATA: " + ex.Message;
            }

            return retVal;
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        private static void CreateQRCodes(string path, string voucherNumber, string financialAccount, string paymentDate, string paymentAmount)
        {
            string content = voucherNumber + Environment.NewLine + financialAccount + Environment.NewLine + paymentDate + Environment.NewLine + paymentAmount;
            QRCodeEncoder encoder = new QRCodeEncoder();
            MessagingToolkit.Barcode.Common.BitMatrix a = encoder.Encode(content, MessagingToolkit.Barcode.BarcodeFormat.QRCode, 160, 160);
            Bitmap mBitmap = new Bitmap(a.GetWidth(), a.GetHeight());
            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    mBitmap.SetPixel(i, j, a.Get(i, j) ? Color.Black : Color.White);
                }
            }
            Graphics g = Graphics.FromImage(mBitmap);
            g.DrawString(voucherNumber, new Font("Arial", 7), Brushes.Red, 0, 150);
            mBitmap.Save(path + "\\" + voucherNumber + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);


        }

        private static void CombineImages(FileInfo[] files, string filePath, string fileName)
        {
            //change the location to store the final image.

            List<FileInfo> temp = new List<FileInfo>();
            if (files.Count() >= 50)
            {
                string finalImage = filePath + "\\" + fileName + "_{0}.jpg";
                int a = 1;
                for (int i = 0; i < files.Count(); i++)
                {
                    if (!temp.Contains(files[i]))
                    {
                        temp.Add(files[i]);
                    }
                    if (temp.Count() == 50)
                    {
                        string path = string.Format(finalImage, a.ToString());
                        MakeImage(temp.ToArray(), path);
                        a++;
                        temp.Clear();
                        continue;
                    }
                    if (i == files.Count() - 1)
                    {
                        string path = string.Format(finalImage, a.ToString());
                        MakeImage(temp.ToArray(), path);
                        a++;
                        temp.Clear();
                        continue;
                    }
                }
            }
            else
            {
                string finalImage = filePath + "\\" + fileName + ".jpg";
                MakeImage(files, finalImage);
            }

        }

        private static void MakeImage(FileInfo[] files, string finalImage)
        {
            List<int> imageHeights = new List<int>();
            List<FileInfo> currentList = new List<FileInfo>();

            decimal filesCount = files.Count();
            int heightCount = (int)Math.Ceiling(filesCount / 10);
            int heightSize = heightCount * 160;
            int widthSize = 10 * 160;
            int width = 0;
            int height = 0;

            Bitmap img3 = new Bitmap(widthSize, heightSize);
            Graphics g = Graphics.FromImage(img3);
            g.Clear(SystemColors.AppWorkspace);

            int co = 0;
            for (int i = 0; i < heightCount; i++)
            {

                Image img = null;
                for (int h = 0; h < 10; h++)
                {
                    if (co == filesCount)
                        break;

                    img = Image.FromFile(files[co].FullName);

                    if (i == 0 && h == 0)
                    {
                        g.DrawImage(img, new Point(0, 0));
                        width += img.Width;
                        co++;
                    }
                    else
                    {
                        g.DrawImage(img, new Point(width, height));
                        width += img.Width;
                        co++;
                    }
                }
                height += img.Height;
                width = 0;

            }
            g.Dispose();
            img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            img3.Dispose();
        }


    }
}
