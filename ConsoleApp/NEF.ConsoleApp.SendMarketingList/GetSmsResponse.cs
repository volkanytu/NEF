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
//using Tefal.ConsoleApp.SendCampaignActivitySms.com.euromsg.Report;
using NEF.ConsoleApp.SendMarketingList.com.euromsg.Report;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.SendMarketingList
{
    public class GetSmsResponse
    {
        string AuthorizationServiceKey { get; set; }
        string ftpAdres = Globals.FTPAddressSms;
        string serverIp = Globals.ServerIp;
        string ftpUser = Globals.FTPUser;
        string ftpPassword = Globals.FTPPassword;
        string zipPath = @"C:\crmAkademi\EuroMessageSmsIntegration\txtZip";

        SmsFunctions eFunc { get; set; }

        IOrganizationService orgService;
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        TEMPEventLog logMe;

        public GetSmsResponse()
        {
            orgService = MSCRM.AdminOrgService;
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivitySms", sdaCustom);
        }

        public void ExecuteResponse()
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
                logMe.Log("GetSmsResponse - ExecuteResponse", "GetSmsResponse Uygulaması Başladı.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

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
                        filesOnFtp.Add(strArray[strArray.Length - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logMe.Log("GetSmsResponse - Execute", "Exception.Message : FTP üzerindeki dosyalara ulaşılamadı.\n\n" +
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
                    logMe.Log("GetSmsResponse - Execute", "Exception.Message : FTP üzerinden dosya silme işlemi yapılamadı.\n\n" +
                                        "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception);
                }
            }

            #endregion |   Delete Files on FTP Older Than 1 Week   |

            #region |   Get Required Files To Local And Send Results To CRM   |

            //Daha önceden yapılan son periyot süresine dahil olan bütün istekler veritabanından çekilir.
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

                    #region |   Delete Received File From TempTable   |

                    // (18.03.2014) Artık mail sonuçları sürekli kontrol edileceği için bu tablodaki datalar silinmeyecek. Okunurken de periyot süresi içerisinde bulunan datalar okunacak.

                    //string deleteQuery = "DELETE NEFCUSTOM_MSCRM..crmTempMailResponseTable WHERE ConversationID=@ConversationID";

                    //SqlDataAccess sdaDelete = new SqlDataAccess();
                    //sda.openConnection(crmService.SQLConnection);
                    //SqlParameter[] parametersInsert = {
                    //    new SqlParameter("@ConversationID", item)
                    //};

                    //sda.ExecuteNonQuery(deleteQuery, parametersInsert);


                    #endregion |    Delete Received File From TempTable   |

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
                            string gsm = values[0].Replace(".", "");
                            string status = values[2];
                            string lastChange = values[3];
                            string deliveryDesc = values[4];

                            CreateSmsRecord(campaignActivityID, gsm, status, sda, orgService);
                        }
                        else
                        {
                            isHeader = false;
                        }
                    }

                    sr.Close();
                    fs.Close();
                    //res.Close();
                    //res2.Close();

                    #endregion |   Extract Zip File    |
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    logMe.Log("GetSmsResponse - Execute", "Exception.Message : Dosya bulunamadı, Zip dosya açılamadı veya sonuç CRM'e işlenemedi. \n\n" +
                                       "Exception.DetailedMessage : " + ex.Message, TEMPEventLog.EventType.Exception, "CampaignActivity", item);
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

            logMe.Log("GetSmsResponse - ExecuteResponse", "GetSmsResponse Uygulaması Bitti.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

          
        }

        public void ExecuteRequest()
        {
            DateTime deliveryDate;

            logMe.Log("GetSmsResponse - ExecuteRequest", "GetSmsResponse Uygulaması Başladı.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);

            #region |   Result Requesting   |

            Login();

            DataTable campaignActivities = GetCampaignIdsToBeRequested(sda);

            if (campaignActivities != null && campaignActivities.Rows.Count > 0)
            {
                foreach (DataRow dr in campaignActivities.Rows)
                {
                    string campaignActivityID = dr["CAID"].ToString();

                    deliveryDate = Convert.ToDateTime(dr["CAS"].ToString());

                    CreateRequest(campaignActivityID, deliveryDate.ToLocalTime(), orgService);
                }
            }

            Logout();

            #endregion

            logMe.Log("GetSmsResponse - ExecuteRequest", "GetSmsResponse Uygulaması Bitti.Tarih : " + DateTime.Now.ToString(), TEMPEventLog.EventType.Info);
        }

        #region |   EUROMSG PROCESSES   |

        public string Login()
        {
            eFunc = new SmsFunctions();
            AuthorizationServiceKey = eFunc.AuthenticationEM();
            return AuthorizationServiceKey;
        }

        public void CreateRequest(string campaignId, DateTime deliveryDate, IOrganizationService service)
        {
            Report emReport = new Report();
            string columnMap = "GSM_NO=GSM_NO";
            string newcampaignId = campaignId.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();
            EmReportResult result = emReport.GetSmsCampaignDeliveryStatusReportBetweenTwoDatesViaFtp(AuthorizationServiceKey, newcampaignId,
                columnMap, "pass", "", string.Format("{0:yyyyMMdd HH:mm:ss}", deliveryDate.AddDays(-1)), string.Format("{0:yyyyMMdd HH:mm:ss}", DateTime.Now.AddDays(7)),
                new FtpInfo()
                {
                    ChangeDir = "EuroMessageSms",
                    Password = ftpPassword,
                    ServerIP = serverIp,
                    Username = ftpUser,
                    Port = 21
                });
            if (result.Code == "00")
                InsertTempTable(new Guid(campaignId), result.ConversationID, service);
            else
                logMe.Log("GetSmsResponse - CreateRequest", result.DetailedMessage + " " + result.Message, TEMPEventLog.EventType.Exception);
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

        void UpdateSmsTempTableRecord(Guid campaignActivityID, int statusCode, Guid customerId, Guid smsId, string gsm, SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);

                #region |   Update Query    |

                string query = @"
                                    UPDATE 
	                                    NEFCUSTOM_MSCRM..EuroMessageSmsTempTable 
                                    SET 
	                                    StateCode = 2,
	                                    StatusCode = @Status,
                                        SmsId = @SmsId,
                                        MobilePhone = @gsm
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
                                                                      new SqlParameter("@SmsId",smsId),
                                                                      new SqlParameter("@gsm", gsm)
                                                                  };

                sda.ExecuteNonQuery(query, activityParameter);

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetSmsResponse - UpdateSmsTempTableRecord", "Sms aktivitesi update edilirken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
        }

        void CreateSmsRecord(Guid campaignActivityID, string gsm, string status, SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                int statusCode = 100000001;
                if (status == "R") // Read
                {
                    statusCode = 100000000;
                }
                else if (status == "W" || status == "") // Waiting
                {
                    statusCode = 100000001;
                }
                else if (status == "F") // Failed
                {
                    statusCode = 100000002;
                }
                else if (status == "T") // Timeout
                {
                    statusCode = 100000003;
                }

                sda.openConnection(Globals.ConnectionString);

                string query = @"
                                   SELECT 
	                                    *
                                    FROM
	                                    NEFCUSTOM_MSCRM..EuroMessageSmsTempTable EM
                                    WHERE
	                                    EM.ActivityId = @CAID
	                                    AND
	                                    EM.MobilePhone = @MP
	                                    --AND
	                                    --EM.StateCode = 0
	                                    --AND
	                                    --EM.StatusCode = 100000001
	                                    AND
	                                    EM.Error = 0 
                                ";

                SqlParameter[] activityParameter = {
                                                                      new SqlParameter("@CAID",campaignActivityID),
                                                                      new SqlParameter("@MP",gsm)
                                                                  };
                
                DataTable dt = sda.getDataTable(query, activityParameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string sms_Id = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        #region |   Check Sms   |

                        Entity sms = new Entity("letter");
                        sms_Id = dt.Rows[i]["SmsId"] != DBNull.Value ? Convert.ToString(dt.Rows[i]["SmsId"]) : string.Empty;

                        if (sms_Id == string.Empty)
                        {
                            #region |   Create Sms   |

                            Entity fromParty = new Entity("activityparty");
                            fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);
                            sms["from"] = new Entity[] { fromParty };

                            Entity toParty = new Entity("activityparty");
                            toParty["partyid"] = new EntityReference("contact", new Guid(dt.Rows[i]["CustomerId"].ToString()));
                            sms["to"] = new Entity[] { toParty };

                            sms["regardingobjectid"] = new EntityReference("campaignactivity", new Guid(dt.Rows[i]["ActivityId"].ToString()));
                            sms["subject"] = dt.Rows[i]["Subject"].ToString();
                            sms["description"] = dt.Rows[i]["Description"].ToString();
                            sms["scheduledstart"] = (DateTime)dt.Rows[0]["ScheduledStart"];
                            sms["scheduledend"] = (DateTime)dt.Rows[0]["ScheduledEnd"];
                            sms["statecode"] = new OptionSetValue(0);
                            sms["statuscode"] = new OptionSetValue(1);
                            sms["new_euromessagestatecode"] = new OptionSetValue(statusCode);
                            sms["new_phonenumber"] = gsm;

                            Guid smsId = service.Create(sms);

                            int campaignActivityState = 1;
                            int campaignActivityStatus = 3;

                            if (statusCode == 100000003)
                            {
                                campaignActivityState = 2;
                                campaignActivityStatus = 5;
                            }

                            SetStateRequest stateRequest = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference("letter", smsId),
                                State = new OptionSetValue(campaignActivityState),
                                Status = new OptionSetValue(campaignActivityStatus)
                            };
                            SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);

                            UpdateSmsTempTableRecord(new Guid(dt.Rows[i]["ActivityId"].ToString()), statusCode, new Guid(dt.Rows[i]["CustomerId"].ToString()), smsId, gsm, sda, service);

                            #endregion |   Create Sms   |
                        }
                        else
                        {
                            #region |   Update Sms   |

                            sms.Attributes.Add("activityid", new Guid(sms_Id));
                            sms.Attributes.Add("new_euromessagestatecode", new OptionSetValue(statusCode));

                            service.Update(sms);

                            UpdateSmsTempTableRecord(new Guid(dt.Rows[i]["ActivityId"].ToString()), statusCode, new Guid(dt.Rows[i]["CustomerId"].ToString()), new Guid(sms_Id), gsm, sda, service);

                            #endregion |   Update Sms   |
                        }

                        #endregion |   Check Sms   |
                    }
                }

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetEmailResponse - CreateSmsRecord", "Sms aktivitesi update edilirken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
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
		                                CA.ActivityId CAID,
                                        CA.ScheduledStart CAS
	                                FROM 
		                                CampaignActivity CA (NOLOCK)
	                                WHERE
		                                CA.StatusCode = 100000004
                                    AND
                                        CA.ChannelTypeCode=3 and
                                        @CurrentUtcDate <= CA.new_euromessagereportlimit";
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

                string deleteQuery = "DELETE FROM NEFCUSTOM_MSCRM..crmTempSmsResponseTable WHERE CampaignActivityID = @CampaignActivityID";

                SqlParameter[] parametersDelete = { new SqlParameter("@CampaignActivityID", campaignActivityID) };
                sda.ExecuteNonQuery(deleteQuery, parametersDelete);

                #endregion |   Eski Talepler Siliniyor   |

                #region |   Yeni Talep Insert Ediliyor   |

                string query = "INSERT INTO NEFCUSTOM_MSCRM..crmTempSmsResponseTable(CampaignActivityID,ConversationID) VALUES(@CampaignActivityID,@ConversationID)";

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
                logMe.Log("GetSmsResponse - InsertTempTable", "Temp tabloya veri eklerken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
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
                                    NEFCUSTOM_MSCRM..crmTempSmsResponseTable res ,
                                    CampaignActivity ca
                                WHERE 
                                    ca.ActivityId = res.CampaignActivityID and
                                    GETDATE() <= ca.new_euromessagereportlimit
                                ORDER BY res.Tarih DESC";

                dt = sda.getDataTable(query);
                #endregion |   Get Requested Campaign activity    |

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                logMe.Log("GetSmsResponse - GetRequestedCampaignActivityFromTempTable", "Temp tablodan veri alırken hata ile karşılaşıldı. Hata mesajı = " + ex.Message, TEMPEventLog.EventType.Exception);
            }
            return dt;
        }

        #endregion
    }
}
