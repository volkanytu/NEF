using Ionic.Zip;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using Tefal.ConsoleApp.SendCampaignActivityEmail.com.euromsg.report;
using NEF.ConsoleApp.SendMarketingListEmail.com.euromsg.report;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendMarketingListEmail
{
    public class GetFilteredMembers
    {
        EmailFunctions eFunc = new EmailFunctions();
        string ftpAdres = Globals.FTPAddressFilterMemberEmail;
        string serverIp = Globals.ServerIp;
        string ftpUser = Globals.FTPUser;
        string ftpPassword = Globals.FTPPassword;
        string columnMap = "EMAIL=Email";

        IOrganizationService orgService;
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public GetFilteredMembers()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
        }

        public void Execute()
        {
            try
            {
                logMe.Log("GetFilteredMembers - Execute", "GetFilteredMembers Uygulaması Başladı.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

                Console.WriteLine("Üye filtreleme işlemleri başladı.");
                string authentication = eFunc.AuthenticationEM();

                //Daha önce yapılan istekler veritabanından alınır.
                DataTable requested = GetRequestedMembersFromTempTable();

                int requestCount = 0;
                if (requested != null)
                    requestCount = requested.Rows.Count;
                else
                    requestCount = 0;

                logMe.Log("GetFilteredMembers - Execute", "Daha önce yapılan istekler veritabanından alındı. Adet : " + requestCount + ". Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

                if (requested != null && requested.Rows.Count > 0)
                {
                    //Her bir istek için veritabanına kayıt edilen ID ile FTP'den dosya alınır.
                    foreach (DataRow dr in requested.Rows)
                    {
                        try
                        {
                            string conversationID = dr["ConversationID"].ToString();
                            int type = Convert.ToInt32(dr["Type"]);

                            //Type = 1 ise; üyelikten ayrılmak isteyen kişilerin izinleri değiştirilir.
                            //Type = 2 ise; mail adresi sistemden kaldırılır.
                            logMe.Log("GetFilteredMembers - Execute", "GetMember methodu çağırılıyor. Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);
                            GetMember(authentication, conversationID, type);

                            //İşlem yapılan istek bilgisi veritabanından silinir.
                            logMe.Log("GetFilteredMembers - Execute", "DeleteFromTempTable methodu çağırılıyor. Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);
                            DeleteFromTempTable(conversationID);
                        }
                        catch (Exception ex)
                        {
                            logMe.Log("GetFilteredMembers - Execute", ex, TEMPEventLog.EventType.Exception);
                            continue;
                        }
                    }
                }
                else
                {
                    //Üyelikten kendi isteği ile ayrılmış kişileri getirir.
                    string passiveConversationID = PassiveMembers(authentication, "X", "Y");
                    InsertTempTable(passiveConversationID, 1);

                    //Geçersiz mail adreslerini getirir.
                    string wrongMailConversationID = PassiveMembers(authentication, "A", "N");
                    InsertTempTable(wrongMailConversationID, 2);
                }

                Console.WriteLine("Üye filtreleme işlemleri bitti.");
                eFunc.LogoutEM(authentication);

                logMe.Log("GetFilteredMembers - Execute", "GetFilteredMembers Uygulaması Bitti.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);
            }
            catch (Exception ex)
            {
                logMe.Log("GetFilteredMembers - Execute", "Execute işleminde hata alındı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        #region |   EUROMSG PROCESSES   |

        private string PassiveMembers(string authentication, string status, string permit)
        {
            string conversationID = string.Empty;
            try
            {
                Report rp = new Report();
                EmKeyOperatorValue a = new EmKeyOperatorValue();
                a.Key = "STATUS";
                a.Operator = "=";
                a.Value = status;

                EmKeyOperatorValue b = new EmKeyOperatorValue();
                b.Key = "EMAIL_PERMIT";
                b.Operator = "=";
                b.Value = permit;

                EmKeyOperatorValue[] dizi = new EmKeyOperatorValue[2];
                dizi[0] = a;
                dizi[1] = b;

                EmReportResult result = rp.GetFilteredMembers(authentication, dizi, columnMap, "pass", "",
                    new FtpInfo()
                    {
                        ChangeDir = "/FilterMemberEmail",
                        Password = ftpPassword,
                        ServerIP = serverIp,
                        Username = ftpUser,
                        Port = 21
                    });

                if (result.Code == "00")
                    conversationID = result.ConversationID;
            }
            catch (Exception ex)
            {
                logMe.Log("GetFilteredMembers - PassiveMembers", "Euromsg işlemleri sırasında hata alındı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }

            return conversationID;
        }

        #endregion

        #region |   FTP PROCESSES   |

        /// <summary>
        /// İstekte bulunulan dosya FTP'den alınır ve içerisindeki kişilere bakılır.
        /// Eğer Type = 1 ise kişinin izinleri değiştirilir.
        /// Eğer Type = 2 ise mail adresi geçersizdir, sistemden silinir.
        /// </summary>
        /// <param name="authentication"></param>
        /// <param name="conversationID"></param>
        /// <param name="type">Type = 1 => Kişi kendi isteği ile üyelikten ayrılmış,
        ///                    Type = 2 => Mail adresi geçersiz
        /// </param>
        private void GetMember(string authentication, string conversationID, int type)
        {
            try
            {
                List<string> filesOnFtp = new List<string>();
                FileStream fs;
                StreamReader sr;
                FtpWebRequest request;
                FtpWebResponse response;
                string zipPath = @"C:\crmAkademi\FilterMemberEmail";

                #region |   Get Required Files To Local And Send Results To CRM   |

                try
                {
                    string completePath = zipPath + "\\data.csv.tmp";

                    if (System.IO.File.Exists(completePath))
                    {
                        System.IO.File.Delete(completePath);
                    }

                    request = (FtpWebRequest)WebRequest.Create(ftpAdres + "/" + conversationID + ".zip");
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                    response = (FtpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();

                    FileStream file = File.Create(zipPath + "\\" + conversationID + ".zip");
                    byte[] buffer = new byte[32 * 1024];
                    int read;

                    while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, read);
                    }

                    file.Close();
                    responseStream.Close();
                    response.Close();

                    // Bu dosya artık alındı
                    ZipFile zipFile = ZipFile.Read(zipPath + "\\" + conversationID + ".zip");
                    zipFile.Password = "pass";
                    zipFile.ExtractAll(zipPath, ExtractExistingFileAction.OverwriteSilently);

                    fs = new FileStream(zipPath + "\\data.csv", FileMode.Open);
                    sr = new StreamReader(fs);

                    bool isHeader = true;
                    while (!sr.EndOfStream)
                    {
                        string[] values = sr.ReadLine().Split(';');
                        if (!isHeader)
                        {
                            string email = values[0];
                            if (type == 1)
                                ChangeMemberPermission(email);
                            else if (type == 2)
                                SetAsInvalidMail(email);
                        }
                        else
                            isHeader = false;
                    }

                    sr.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    logMe.Log("GetFilteredMembers - GetMember", "Exception.Message : Dosya bulunamadı, Zip dosya açılamadı veya sonuç CRM'e işlenemedi. Kampanya ID: " + conversationID + "\n\n" +
                                        "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception);
                }
                finally
                {
                    string completePath = zipPath + "\\data.csv.tmp";

                    if (System.IO.File.Exists(completePath))
                    {
                        System.IO.File.Delete(completePath);
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                logMe.Log("GetFilteredMembers - GetMember", "Exception.Message : Dosya bulunamadı, Zip dosya açılamadı veya sonuç CRM'e işlenemedi. Kampanya ID: " + conversationID + "\n\n" +
                                        "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        #endregion

        #region |   CRM PROCESSES   |

        //Üyelikten çıkan kişinin mail izinleri değiştirilir.
        private void ChangeMemberPermission(string email)
        {
            try
            {
                OrganizationServiceContext orgContext = new OrganizationServiceContext(orgService);

                var query = (
                                from b in orgContext.CreateQuery("contact")
                                where
                                (string)b["emailaddress1"] == email
                                &&
                                (bool)b["donotemail"] == false
                                select new
                                {
                                    ContactID = (Guid)b["contactid"]
                                }

                            ).ToList();

                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        Entity contact = new Entity("contact");
                        contact["contactid"] = query[i].ContactID;
                        contact["donotemail"] = true;
                        orgService.Update(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                logMe.Log("GelFilteredMembers - ChangeMemberPermission", "Üyelikten çıkan kişilerin crm işlemleri sırasında hata alındı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        //  Yanlış mail adreslerinin Invalid Mail alanı güncelleştirilir.
        private void SetAsInvalidMail(string email)
        {
            try
            {
                OrganizationServiceContext orgContext = new OrganizationServiceContext(orgService);

                var query = (
                                from b in orgContext.CreateQuery("contact")
                                where
                                (string)b["emailaddress1"] == email
                                &&
                                ((bool?)b["new_email"] == false
                                || (bool?)b["new_email"] == null)
                                select new
                                {
                                    ContactID = (Guid)b["contactid"]
                                }

                            ).ToList();

                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        Entity contact = new Entity("contact");
                        contact["contactid"] = query[i].ContactID;
                        contact["new_email"] = true;
                        orgService.Update(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                logMe.Log("GelFilteredMembers - ChangeMemberPermission", "Üyelikten çıkan kişilerin crm işlemleri sırasında hata alındı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        #endregion

        #region |   DATABASE PROCESSES  |

        private void InsertTempTable(string conversationID, int type)
        {
            try
            {
                string query = "INSERT INTO NEFCUSTOM_MSCRM..crmTempMailMemberResponseTable(ConversationID,Type) VALUES(@ConversationID,@Type)";
                sda.openConnection(Globals.ConnectionString);
                SqlParameter[] parametersInsert = {   
                                                                        new SqlParameter("@ConversationID", conversationID),
                                                                        new SqlParameter("@Type", type)
                                                                    };

                sda.ExecuteNonQuery(query, parametersInsert);
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable GetRequestedMembersFromTempTable()
        {
            DataTable dt = null;
            try
            {
                sda.openConnection(Globals.ConnectionString);
                string query = "SELECT * FROM NEFCUSTOM_MSCRM..crmTempMailMemberResponseTable ORDER BY Tarih DESC";
                dt = sda.getDataTable(query);
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private void DeleteFromTempTable(string conversationID)
        {
            try
            {
                string query = "DELETE FROM NEFCUSTOM_MSCRM..crmTempMailMemberResponseTable WHERE ConversationID=@CID";
                sda.openConnection(Globals.ConnectionString);
                SqlParameter[] parametersInsert = {   
                                                                        new SqlParameter("@CID", conversationID)
                                                                    };

                sda.ExecuteNonQuery(query, parametersInsert);
                sda.closeConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
