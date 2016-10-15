using Ionic.Zip;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.report;
using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.report.live;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class GetEmailResponse
    {
        string AuthorizationServiceKey { get; set; }
        string ftpAdres = Globals.FTPAddressEmail;
        string serverIp = Globals.ServerIp;
        string ftpUser = Globals.FTPUser;
        string ftpPassword = Globals.FTPPassword;
        string zipPath = @"C:\InnTheBox\EuroMessageEmailIntegration\txtZip";

        EmailFunctions eFunc { get; set; }

        IOrganizationService orgService;
        SqlDataAccess sda = new SqlDataAccess();
        SqlDataAccess sdaCustom = new SqlDataAccess();
        TEMPEventLog logMe;

        public GetEmailResponse()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "NEF.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
        }

        public void Execute()
        {
            List<string> filesOnFtp = new List<string>();
            FileStream fs;
            StreamReader sr;
            FtpWebRequest req;
            FtpWebResponse res;

            #region |   Result Processing   |

            #region |   Get Files on FTP   |

            try
            {
                logMe.Log("GetEmailResponse - Execute", "GetEmailResponse Uygulaması Başladı.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

                req = FtpWebRequest.Create(ftpAdres) as FtpWebRequest;
                req.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                req.UsePassive = true;
                req.UseBinary = true;
                req.KeepAlive = false;
                res = req.GetResponse() as FtpWebResponse;

                sr = new StreamReader(res.GetResponseStream());


                while (sr.BaseStream.CanRead)
                {
                    System.Console.WriteLine("Okuma işlemi başladı");
                    string fileName = sr.ReadLine();

                    if (fileName != null)
                    {
                        int index = fileName.IndexOf("PM");
                        fileName = fileName.Substring(index + 2);

                        string[] strArray = fileName.Split(' ');

                        string filesOr = strArray[strArray.Length - 1];
                        if (Path.GetExtension(filesOr).Equals(".zip"))
                        {
                            filesOnFtp.Add(filesOr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logMe.Log("GetEmailResponse - Execute", "Exception.Message : FTP üzerindeki dosyalara ulaşılamadı.\n\n" +
                                    "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception);
            }

            #endregion

            #region |   Delete Files on FTP Older Than 1 Week   |

            foreach (string fileName in filesOnFtp)
            {
                try
                {
                    req = FtpWebRequest.Create(ftpAdres + "/" + fileName) as FtpWebRequest;
                    req.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                    req.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    req.Proxy = new WebProxy();

                    res = req.GetResponse() as FtpWebResponse;

                    if (res.LastModified < DateTime.Now.AddDays(-7))
                    {
                        req = FtpWebRequest.Create(ftpAdres + "/" + fileName) as FtpWebRequest;
                        req.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                        req.Method = WebRequestMethods.Ftp.DeleteFile;
                        req.Proxy = new WebProxy();

                        res = req.GetResponse() as FtpWebResponse;
                    }
                }
                catch (Exception ex)
                {
                    logMe.Log("GetEmailResponse - Execute", "Exception.Message : FTP üzerinden dosya silme işlemi yapılamadı.\n\n" +
                                        "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception);
                }
            }

            #endregion |   Delete Files on FTP Older Than 1 Week   |

            #region |   Get Required Files To Local And Send Results To CRM   |

            //  Daha önceden yapılan son periyot süresine dahil olan bütün istekler veritabanından çekilir.
            DataTable requestCampaignFromTempTable = GetRequestedCampaignActivityFromTempTable(orgService);

            for (int i = 0; i < requestCampaignFromTempTable.Rows.Count; i++)
            {
                string item = requestCampaignFromTempTable.Rows[i]["ConversationID"].ToString();
                Guid campaignActivityID = new Guid(requestCampaignFromTempTable.Rows[i]["CampaignActivityID"].ToString());

                try
                {
                    #region |   Get Files From Nef Ftp    |

                    string completePath = zipPath + "\\data.csv.tmp";

                    if (System.IO.File.Exists(completePath))
                    {
                        System.IO.File.Delete(completePath);
                    }

                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpAdres + "/" + item + ".zip");
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    request.Credentials = new NetworkCredential(ftpUser, ftpPassword);

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();

                    FileStream file = File.Create(zipPath + "\\" + item + ".zip");
                    byte[] buffer = new byte[32 * 1024];
                    int read;

                    while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, read);
                    }

                    file.Close();
                    responseStream.Close();
                    response.Close();

                    #endregion |   Get Files From Nef Ftp    |

                    #region |   Extract Zip File    |

                    ZipFile zipFile = ZipFile.Read(zipPath + "\\" + item + ".zip");
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
                            string status = values[1];
                            string lastChangeTime = values[2];
                            string isMarkedSpam = values[3];
                            string isClicked = values[4];

                            CreateEmailRecord(campaignActivityID, email, status, sda, orgService);
                        }
                        else
                        {
                            isHeader = false;
                        }
                    }

                    sr.Close();
                    fs.Close();

                    #endregion |   Extract Zip File    |
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    logMe.Log("GetEmailResponse Execute", "Exception.Message : Dosya bulunamadı, Zip dosya açılamadı veya sonuç CRM'e işlenemedi. \n\n" +
                                       "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception, "CampaignActvity", item);
                    continue;
                }
                finally
                {
                    string completePath = zipPath + "\\data.csv.tmp";

                    if (System.IO.File.Exists(completePath))
                    {
                        System.IO.File.Delete(completePath);
                    }
                }
            }

            #endregion

            #endregion

            #region |   Result Requesting   |

            Login();

            DataTable campaignActivities = GetCampaignIdsToBeRequested(sda);

            if (campaignActivities != null && campaignActivities.Rows.Count > 0)
            {
                foreach (DataRow dr in campaignActivities.Rows)
                {
                    string campaignActivityID = dr["CAID"].ToString();

                    CreateRequest(campaignActivityID, orgService);
                }
            }

            Logout();

            #endregion

            logMe.Log("GetEmailResponse - Execute", "GetEmailResponse Uygulaması Bitti.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

            sda.closeConnection();
        }

        #region |   EUROMSG PROCESSES   |

        public string Login()
        {

            eFunc = new EmailFunctions();
            AuthorizationServiceKey = eFunc.AuthenticationEM();
            return AuthorizationServiceKey;
        }

        public void CreateRequest(string campaignId, IOrganizationService service)
        {

            Report emReport = new Report();
            string columnMap = "EMAIL=Email";
            string newcampaignId = campaignId.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();
            EmReportResult result = emReport.GetEmailCampaignDeliveryStatusReportViaFtp(AuthorizationServiceKey, newcampaignId,
                columnMap, "pass", "",
                new FtpInfo()
                {
                    ChangeDir = "EuroMessageEmail",
                    Password = ftpPassword,
                    ServerIP = serverIp,
                    Username = ftpUser,
                    Port = 21
                });
            if (result.Code == "00")
                InsertTempTable(new Guid(campaignId), result.ConversationID, service);
            else
            {
                logMe.Log("GetEmailResponse - CreateRequest", result.DetailedMessage + " " + result.Message, TEMPEventLog.EventType.Exception);
            }
        }

        void Logout()
        {
            if (AuthorizationServiceKey != "")
            {
                AuthorizationServiceKey = "";
                eFunc.LogoutEM(AuthorizationServiceKey);
            }
        }

        #endregion

        #region |   CRM PROCESSES   |

        void UpdateEmailTempTableRecord(Guid campaignActivityID, int statusCode, Guid customerId, Guid emailId, SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);

                #region |   Update Query    |

                string query = @"
                                    UPDATE 
	                                    NEFCUSTOM_MSCRM..EuroMessageMailTempTable 
                                    SET 
	                                    StateCode = 2,
	                                    StatusCode = @Status ,
                                        EmailId = @EmailId
                                    WHERE 
	                                    ActivityId = @CAID 
	                                    AND 
	                                    CustomerId = @CU
	                                    AND
	                                    StatusCode = 100000001
	                                    AND
	                                    Error = 0
                                ";
                #endregion |   Update Query    |

                SqlParameter[] activityParameter = {
                                                                      new SqlParameter("@CAID",campaignActivityID),
                                                                      new SqlParameter("@CU",customerId),
                                                                      new SqlParameter("@Status",statusCode),
                                                                      new SqlParameter("@EmailId",emailId)
                                                                  };

                sda.ExecuteNonQuery(query, activityParameter);

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetEmailResponse - UpdateEmailTempTableRecord", "Email aktivitesi update edilirken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        void CreateEmailRecord(Guid campaignActivityID, string emailAddress, string status, SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                int statusCode = 100000001;
                if (status == "RD") // Eposta Gönderildi ve Okundu
                {
                    statusCode = 100000002;
                }
                else if (status == "RE" || status == "") // Gönderildi ama Okunmadı
                {
                    statusCode = 100000005;
                }
                else if (status == "HU") // Geçersiz Adres
                {
                    statusCode = 100000003;
                }
                else if (status == "SU") // Geçici Olarak Ulaşılamayan Adres
                {
                    statusCode = 100000004;
                }

                sda.openConnection(Globals.ConnectionString);

                string query = @"
                                   SELECT 
	                                    *
                                    FROM
	                                    NEFCUSTOM_MSCRM..EuroMessageMailTempTable EM
                                    WHERE
	                                    EM.ActivityId = @CAID
	                                    AND
	                                    EM.EmailAddress = @EM
	                                    --AND
	                                    --EM.StateCode = 0
	                                    --AND
	                                    --EM.StatusCode = 100000001
	                                    AND
	                                    EM.Error = 0 
                                ";



                SqlParameter[] activityParameter = {
                                                                      new SqlParameter("@CAID",campaignActivityID),
                                                                      new SqlParameter("@EM",emailAddress)
                                                                  };
                sda.openConnection(Globals.ConnectionString);
                DataTable dt = sda.getDataTable(query, activityParameter);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string emailId = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        #region |   Check Email   |

                        Entity email = new Entity("email");
                        emailId = dt.Rows[i]["EmailId"] != DBNull.Value ? Convert.ToString(dt.Rows[i]["EmailId"]) : string.Empty;

                        if (emailId == string.Empty)
                        {
                            #region |   Create Email   |

                            Entity fromParty = new Entity("activityparty");
                            fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);
                            email["from"] = new Entity[] { fromParty };

                            Entity toParty = new Entity("activityparty");
                            toParty["partyid"] = new EntityReference("contact", new Guid(dt.Rows[i]["CustomerId"].ToString()));
                            email["to"] = new Entity[] { toParty };

                            email["regardingobjectid"] = new EntityReference("campaignactivity", new Guid(dt.Rows[i]["ActivityId"].ToString()));
                            email["subject"] = dt.Rows[i]["subject"].ToString();
                            email["description"] = dt.Rows[i]["description"].ToString();
                            email["scheduledstart"] = (DateTime)dt.Rows[0]["ScheduledStart"];
                            email["scheduledend"] = (DateTime)dt.Rows[0]["ScheduledEnd"];
                            email["statecode"] = new OptionSetValue(0);
                            email["statuscode"] = new OptionSetValue(1);
                            email["new_euromessagestatus"] = new OptionSetValue(statusCode);
                            email["new_mailaddress"] = dt.Rows[i]["emailaddress"].ToString();

                            Guid mailId = service.Create(email);

                            int campaignActivityState = 1;
                            int campaignActivityStatus = 3;

                            if (statusCode == 100000003)
                            {
                                campaignActivityState = 2;
                                campaignActivityStatus = 5;
                            }

                            SetStateRequest stateRequest = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference("email", mailId),
                                State = new OptionSetValue(campaignActivityState),
                                Status = new OptionSetValue(campaignActivityStatus)
                            };
                            SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);

                            UpdateEmailTempTableRecord(new Guid(dt.Rows[i]["ActivityId"].ToString()), statusCode, new Guid(dt.Rows[i]["CustomerId"].ToString()), mailId, sda, service);

                            #endregion |   Create Email   |
                        }
                        else
                        {
                            #region |   Update Email   |

                            email.Attributes.Add("activityid", new Guid(emailId));
                            email.Attributes.Add("new_euromessagestatus", new OptionSetValue(statusCode));

                            service.Update(email);

                            UpdateEmailTempTableRecord(new Guid(dt.Rows[i]["ActivityId"].ToString()), statusCode, new Guid(dt.Rows[i]["CustomerId"].ToString()), new Guid(emailId), sda, service);

                            #endregion |   Update Email   |
                        }

                        #endregion |   Check Email   |
                    }
                }

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetEmailResponse - CreateEmailRecord", "Email aktivitesi update edilirken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        DataTable GetCampaignIdsToBeRequested(SqlDataAccess sda)
        {
            DataTable dt = null;

            sda.openConnection(Globals.ConnectionString);

            try
            {
                #region |   Get Query   |
                //Status Code değeri 'Servise Gönderimi Yapıldı' olan kampanya aktiviteleri çekiliyor.
                string queryCA = @"
                                    DECLARE @CurrentUtcDate DATETIME
                                    SET @CurrentUtcDate = GETUTCDATE()


                                    SELECT 
		                                CA.ActivityId CAID
	                                FROM 
		                                CampaignActivity CA (NOLOCK)
	                                WHERE
		                                CA.StatusCode = 100000001
                                    AND
                                        CA.ChannelTypeCode=7 
                                    --and
                                       -- @CurrentUtcDate <= CA.new_euromessagereportlimit";
                #endregion |   Get Query   |

                dt = sda.getDataTable(queryCA);

                sda.closeConnection();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        #endregion

        #region |   DATABASE PROCESSES  |

        private void InsertTempTable(Guid campaignActivityID, string conversationID, IOrganizationService service)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);

                #region |   Eski Talepler Siliniyor   |

                string deleteQuery = "DELETE FROM NEFCUSTOM_MSCRM..crmTempMailResponseTable WHERE CampaignActivityID = @CampaignActivityID";

                SqlParameter[] parametersDelete = { new SqlParameter("@CampaignActivityID", campaignActivityID) };
                sda.ExecuteNonQuery(deleteQuery, parametersDelete);

                #endregion |   Eski Talepler Siliniyor   |

                #region |   Yeni Talep Insert Ediliyor   |

                string query = "INSERT INTO NEFCUSTOM_MSCRM..crmTempMailResponseTable(CampaignActivityID,ConversationID) VALUES(@CampaignActivityID,@ConversationID)";

                SqlParameter[] parametersInsert = {
                                                        new SqlParameter("@CampaignActivityID",campaignActivityID),
                                                        new SqlParameter("@ConversationID", conversationID)
                                                    };

                sda.ExecuteNonQuery(query, parametersInsert);

                #endregion |   Yeni Talep Insert Ediliyor   |

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetEmailResponse - InsertTempTable", "Temp tabloya veri eklerken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        private DataTable GetRequestedCampaignActivityFromTempTable(IOrganizationService service)
        {
            DataTable dt = null;
            try
            {
                #region |   Get Requested Campaign activity    |
                sda.openConnection(Globals.ConnectionString);

                string query = @"SELECT res.* FROM 
                                    NEFCUSTOM_MSCRM..crmTempMailResponseTable res ,
                                    CampaignActivity ca
                                WHERE 
                                    ca.ActivityId = res.CampaignActivityID 
                                --and
                                    --GETDATE() <= ca.new_euromessagereportlimit
                                ORDER BY res.Tarih DESC";

                dt = sda.getDataTable(query);
                #endregion |   Get Requested Campaign activity    |

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetEmailResponse - getRequestedCampaignActivityFromTempTable", "Temp tablodan veri alırken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
            return dt;
        }

        #endregion
    }
}
