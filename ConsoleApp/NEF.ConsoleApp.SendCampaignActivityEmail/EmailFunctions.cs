using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
//using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.auth;
//using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.campaign;
//using NEF.ConsoleApp.SendCampaignActivityEmail.sendlist;
using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.auth.live;
using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.campaign.live;
using NEF.ConsoleApp.SendCampaignActivityEmail.sendlist.live;
using NEF.Library.Utility;
using Chilkat;
using Microsoft.Crm.Sdk.Messages;
using NEF.ConsoleApp.SendCampaignActivityEmail.com.euromsg.report.live;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class EmailFunctions
    {
        SqlConnection conn;
        SqlDataAccess sda;
        SqlDataAccess sdaCustom;
        Campaign campaign;
        IOrganizationService orgService;
        TEMPEventLog logMe;
        Report report;

        public static string fileEncoding = "ISO-8859-9";

        public EmailFunctions()
        {
            sda = new SqlDataAccess();
            sdaCustom = new SqlDataAccess();
            campaign = new Campaign();
            orgService = MSCRM.AdminOrgService;
            logMe = new TEMPEventLog(orgService, "Nef.ConsoleApp.SendCampaignActivityEmail", sdaCustom);
            report = new Report();
        }

        public void UpdateEmailActivity(EmailDetail emaildetail, SqlDataAccess sda)
        {
            try
            {
                sda.openConnection(Globals.ConnectionString);

                #region |   Update Query    |
                string query = @"
                                UPDATE 
	                                NEFCUSTOM_MSCRM..EuroMessageMailTempTable
                                SET
	                                EmailAddress = @EmailAddress,
                                    Subject = @Subject,
                                    Description = @Description,
                                    StatusCode = 100000001,
                                    StateCode = 0
                                WHERE
	                                RecordId = @RecordId
                                ";
                SqlParameter[] parameters = new SqlParameter[]{
                                                new SqlParameter("@EmailAddress", emaildetail.CustomerMailAddres),
                                                new SqlParameter("@RecordId",emaildetail.RecordId),
                                                new SqlParameter("@Description", emaildetail.Description),
                                                new SqlParameter("@Subject", emaildetail.Subject)
                                             };

                #endregion |   Update Query    |

                sda.ExecuteNonQuery(query, parameters);

                sda.closeConnection();
            }
            catch (Exception ex)
            {
                throw new InvalidProgramException(ex.ToString());
            }
        }

        public string BuildXml(DataRow drActivity)
        {
            string file = string.Empty;

            try
            {
                StringBuilder campaignXml = new StringBuilder();

                campaignXml.AppendLine("<euro.message>");
                // if something goes wrong with data upload and campaign creation, this email address will be notified. 
                campaignXml.Append("<NOTIFICATION_EMAIL>").Append("emrah.eroglu@innthebox.com").AppendLine("</NOTIFICATION_EMAIL>");
                campaignXml.AppendLine("<CAMPAIGN type=\"EMAIL\">");
                campaignXml.Append("<CAMP_ID>").Append(drActivity["CAID"].ToString().ToUpper().Replace("-", "")).AppendLine("</CAMP_ID>");
                campaignXml.Append("<DELIVERY_DATE>").Append(Convert.ToDateTime(drActivity["CAS"]).ToString("yyyy-MM-dd HH:mm:ss")).Append("</DELIVERY_DATE>");
                campaignXml.Append("<DEMOGRAFIC_INFO>").Append("</DEMOGRAFIC_INFO>");
                campaignXml.Append("<EXPIRE_DATE>").Append("</EXPIRE_DATE>");
                campaignXml.Append("<REPORT_ADMINS>").Append("</REPORT_ADMINS>");
                campaignXml.AppendLine("<PDF_REPORT enabled=\"false\">");
                campaignXml.AppendLine("<FIRST_REPORT enabled=\"false\">");
                campaignXml.AppendLine("<EMAILS/>");
                campaignXml.AppendLine("<DAY_AFTER/>");
                campaignXml.AppendLine("<LANG_ID/>");
                campaignXml.AppendLine("</FIRST_REPORT>");
                campaignXml.AppendLine("<SECOND_REPORT enabled=\"false\">");
                campaignXml.AppendLine("<EMAILS/>");
                campaignXml.AppendLine("<DAY_AFTER/>");
                campaignXml.AppendLine("<LANG_ID/>");
                campaignXml.AppendLine("</SECOND_REPORT>");
                campaignXml.AppendLine("</PDF_REPORT>");
                campaignXml.AppendLine("</CAMPAIGN>");
                campaignXml.AppendLine("</euro.message>");

                // Build file from DataTable
                string workingDirectory = ConfigurationManager.AppSettings["Working.Directory"].ToString() + "\\";
                Directory.CreateDirectory(workingDirectory);
                file = workingDirectory + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".xml";
                StreamWriter xmlFileSW = new StreamWriter(file, false, System.Text.Encoding.GetEncoding(fileEncoding));
                xmlFileSW.AutoFlush = true;
                xmlFileSW.Write(campaignXml.ToString());
                xmlFileSW.Close();
                xmlFileSW = null;

                return file;
            }
            catch (Exception ex)
            {
                logMe.Log("SendMailIntegration - Execute", ex, TEMPEventLog.EventType.Exception, "new_campaignactivity", drActivity["CAID"].ToString());
            }

            return file;
        }

        public DataTable GetCustomerList(SqlDataAccess sda, Guid campaignActivityId, Guid ListId)
        {
            DataTable dt = null;

            try
            {
                #region |   Query Contact   |
                string queryList = @"
                                  SELECT
                                      E.RecordId RID,  
                                      E.ActivityId AID,
                                      E.ListId LID,
                                      C.ContactId CID,                                        
                                      C.FirstName CFN,
	                                  C.LastName CLN,
									  CA.Subject SJ,
									  CA.Description DSC,
                                      C.EmailAddress1 EM
                                  FROM
                                      NEFCUSTOM_MSCRM..EuroMessageMailTempTable E (NOLOCK)
                                          JOIN
                                          Contact C (NOLOCK)
                                          ON C.ContactId = E.CustomerId                                                        
                                          JOIN
                                          CampaignActivity CA (NOLOCK)
                                          ON E.ActivityId = CA.ActivityId
                                  WHERE
                                  		E.Error=0
                                  	AND
                                  		E.ActivityId =@campaignActivityId
                                    AND
                                        E.ListId = @listId 
                                    AND
                                         C.EmailAddress1 is not null
                                  ";
                #endregion |   Query Contact   |

                SqlParameter[] parameters = {
                                            new SqlParameter("@campaignActivityId", campaignActivityId),
                                            new SqlParameter("@listId", ListId)
                                        };
                sda.openConnection(Globals.ConnectionString);
                dt = sda.getDataTable(queryList, parameters);
                sda.closeConnection();

                return dt;
            }
            catch (Exception ex)
            {
                logMe.Log("EmailFunctions - GetCustomerList", ex, TEMPEventLog.EventType.Exception, "new_campaignactivity", campaignActivityId.ToString());
            }

            return dt;
        }

        public string BuildMailList(DataTable customerList)
        {
            string mailList = string.Empty;
            try
            {
                List<EmailDetail> emailDetails = new List<EmailDetail>();

                foreach (DataRow dr in customerList.Rows)
                {
                    mailList = mailList + dr["EM"].ToString().Replace("|", "").Replace(Environment.NewLine, "").Replace(Convert.ToChar(0x0).ToString(), "") + "|" +
                        dr["CFN"].ToString().Replace("|", "").Replace(Environment.NewLine, "").Replace(Convert.ToChar(0x0).ToString(), "") + "|" +
                        dr["CLN"].ToString().Replace("|", "").Replace(Environment.NewLine, "").Replace(Convert.ToChar(0x0).ToString(), "") + "\r\n";

                    emailDetails.Add(new EmailDetail
                    {
                        ActivityId = dr["AID"].ToString(),
                        CustomerMailAddres = dr["EM"] != DBNull.Value ? dr["EM"].ToString() : "",
                        FirstName = dr["CFN"] != DBNull.Value ? dr["CFN"].ToString() : "",
                        LastName = dr["CLN"] != DBNull.Value ? dr["CLN"].ToString() : "",
                        RecordId = dr["RID"].ToString(),
                        Subject = dr["SJ"].ToString(),
                        Description = dr["DSC"].ToString()
                    });
                }
                mailList = mailList.Trim();
                foreach (EmailDetail emailDetail in emailDetails)
                {
                    if (emailDetail != null)
                        UpdateEmailActivity(emailDetail, sda);
                }

                return mailList;
            }
            catch (Exception ex)
            {
                logMe.Log("EmailFunctions - BuildMailList", ex, TEMPEventLog.EventType.Exception, "marketinglist", customerList.Rows[0]["LID"].ToString());
            }

            return mailList;
        }

        public string BuildDataFile(string customerList)
        {
            string file = string.Empty;
            string line = string.Empty;

            string workingDirectory = ConfigurationManager.AppSettings["Working.Directory"].ToString() + "\\";
            Directory.CreateDirectory(workingDirectory);
            file = workingDirectory + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".txt";
            StreamWriter dataFileSW = new StreamWriter(file, false, System.Text.Encoding.GetEncoding(fileEncoding));
            dataFileSW.AutoFlush = true;

            // build header
            line = "EMAIL|NAME|SURNAME";
            // write header
            dataFileSW.WriteLine(line);

            // write file
            line = customerList;
            dataFileSW.WriteLine(line);

            dataFileSW.Close();
            dataFileSW = null;

            return file;
        }

        public string WriteZip(string dataFile, string campaignXmlFile)
        {
            Zip zip = new Zip();

            // Anything begins the 30-day trial
            bool unlocked = zip.UnlockComponent("HURRIYZIP_vxCeqCGp3Not");
            if (!unlocked)
                throw new ApplicationException(zip.LastErrorText);

            // Build file from DataTable
            string workingDirectory = ConfigurationManager.AppSettings["Working.Directory"].ToString() + "\\";
            Directory.CreateDirectory(workingDirectory);
            string file = workingDirectory + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".zip";

            bool success = zip.NewZip(file);
            if (!success)
                throw new ApplicationException(zip.LastErrorText);

            // Append data file
            bool saveExtraPath;
            saveExtraPath = false;
            success = zip.AppendOneFileOrDir(dataFile, saveExtraPath);
            if (success != true)
                throw new ApplicationException(zip.LastErrorText);

            success = zip.AppendOneFileOrDir(campaignXmlFile, saveExtraPath);
            if (success != true)
                throw new ApplicationException(zip.LastErrorText);

            // Write test.zip
            success = zip.WriteZipAndClose();
            if (!success)
                throw new ApplicationException(zip.LastErrorText);

            return file;
        }

        public void UploadViaSftp(string zipFileName, string sftpUrl, string userName, string password)
        {
            SFtp sftp = new SFtp();

            //  Any string automatically begins a fully-functional 30-day trial.
            bool success;
            success = sftp.UnlockComponent("HURRIYSSH_XAN0ZUeq9En4");
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Set some timeouts, in milliseconds:
            sftp.ConnectTimeoutMs = 60000;
            sftp.IdleTimeoutMs = 300000;

            //  Connect to the SSH server.
            //  The standard SSH port = 22
            //  The hostname may be a hostname or IP address.
            int port;
            string hostname;
            hostname = sftpUrl;
            port = 22;
            success = sftp.Connect(hostname, port);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Authenticate with the SSH server.  Chilkat SFTP supports
            //  both password-based authenication as well as public-key
            //  authentication.  This example uses password authenication.
            success = sftp.AuthenticatePw(userName, password);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  After authenticating, the SFTP subsystem must be initialized:
            success = sftp.InitializeSftp();
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Open a file on the server for writing.
            //  "createTruncate" means that a new file is created; if the file already exists, it is opened and truncated.
            string handle;
            handle = sftp.OpenFile(Path.GetFileName(zipFileName), "writeOnly", "createTruncate");
            if (handle == null)
                throw new ApplicationException(sftp.LastErrorText);

            //  Upload from the local file to the SSH server.
            success = sftp.UploadFile(handle, zipFileName);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Close the file.
            success = sftp.CloseHandle(handle);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);
        }

        public string AuthenticationEM()
        {


            try
            {
                string AuthenticationServiceKey = string.Empty;

                Auth auth = new Auth();
                EmAuthResult authResult = auth.Login(Globals.EuroMessageUserNameLive, Globals.EuroMessagePasswordLive);
                if (authResult.Code == "00")
                {
                    AuthenticationServiceKey = authResult.ServiceTicket;
                    return AuthenticationServiceKey;
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void LogoutEM(string AuthenticationServiceKey)
        {


            if (AuthenticationServiceKey != "")
            {
                Auth auth = new Auth();
                auth.Logout(AuthenticationServiceKey);
            }
        }

        /// <summary>
        /// Periyodik kampanya aktivitelerini teste gönderir.
        /// </summary>
        public void TestCampaignActivity()
        {
            Guid campaignActivityId = Guid.Empty;
            string campId = string.Empty;

            try
            {
                string serviceTicket = AuthenticationEM();

                if (!string.IsNullOrEmpty(serviceTicket))
                {
                    string query = @"DECLARE @Now DATETIME
                                    SET @Now = CONVERT(DATETIME, CONVERT(NVARCHAR(10), GETUTCDATE(), 104), 104)

                                    SELECT
	                                    CA.ActivityId CAID
                                    FROM
	                                    CampaignActivity CA (NOLOCK)
                                    WHERE
		                                    CA.new_sendingtype = 2
	                                    AND
		                                    CA.StateCode = 0
	                                    AND
		                                    CA.StatusCode = 1
	                                    AND
		                                    CA.ChannelTypeCode = 7
                                        AND
                                            CA.CreatedOn >= @Now";
                    sda.openConnection(Globals.ConnectionString);
                    DataTable dt = sda.getDataTable(query);
                    sda.closeConnection();

                    foreach (DataRow dr in dt.Rows)
                    {
                        campaignActivityId = new Guid(dr["CAID"].ToString());
                        try
                        {
                            campId = campaignActivityId.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();

                            EmCampaignFuncRes emResult = campaign.TestEmailCampaign(serviceTicket, campId, "CRM", "test crm");

                            // Kampanya aktivitesi durumu Sent to Test olarak güncellenir.
                            if (emResult.Code == "00")
                            {
                                SetStateRequest stateRequest = new SetStateRequest()
                                {
                                    EntityMoniker = new EntityReference("campaignactivity", campaignActivityId),
                                    State = new OptionSetValue(0),
                                    Status = new OptionSetValue(100000005)
                                };
                                SetStateResponse stateResponse = (SetStateResponse)orgService.Execute(stateRequest);
                            }
                            else
                                logMe.Log("EmailFunctions - TestCampaignActivity", "Kampanya aktivitesi teste gönderilemedi. Code: " + emResult.Code + " Message: " + emResult.Message, TEMPEventLog.EventType.Info, "campaignactivity", campaignActivityId.ToString());
                        }
                        catch (Exception ex)
                        {
                            logMe.Log("EmailFunctions - TestCampaignActivity", ex, TEMPEventLog.EventType.Exception, "campaignactivity", campaignActivityId.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logMe.Log("EmailFunctions - TestCampaignActivity", ex, TEMPEventLog.EventType.Exception);
            }
        }

        /// <summary>
        /// Periyodik gönderim olarak set edilmiş kampanya aktivitelerini dağıtıldı durumuna çeker.
        /// </summary>
        public void SetCampaignActivityDistributed()
        {
            Guid campaignActivityId = Guid.Empty;
            string campId = string.Empty;

            try
            {
                string serviceTicket = AuthenticationEM();

                if (!string.IsNullOrEmpty(serviceTicket))
                {
                    // Email, Teste gönderildi, Periyodik
                    string query = @"DECLARE @Now DATETIME
                                     SET @Now = CONVERT(DATETIME, CONVERT(NVARCHAR(10), GETUTCDATE(), 104), 104)
                                     
                                     SELECT
                                         CA.ActivityId CAID
                                     FROM
                                         CampaignActivity CA (NOLOCK)
                                     WHERE
                                             CA.new_sendingtype = 2
                                         AND
                                             CA.StateCode = 0
                                         AND
                                             CA.StatusCode = 100000005
                                         AND
                                             CA.ChannelTypeCode = 7
	                                     AND
		                                     CA.CreatedOn >= @Now";
                    sda.openConnection(Globals.ConnectionString);
                    DataTable dt = sda.getDataTable(query);
                    sda.closeConnection();

                    foreach (DataRow dr in dt.Rows)
                    {
                        campaignActivityId = new Guid(dr["CAID"].ToString());

                        try
                        {
                            campId = campaignActivityId.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();

                            EmCampaignReportResult eCampaign;
                            EmReportResult rResult = report.GetCampaignReportWithCampID(serviceTicket, campId, out eCampaign);

                            if (rResult.Code == "00")
                            {
                                if (eCampaign.Status == "TESTED")
                                {
                                    SetStateRequest closeLead = new SetStateRequest()
                                    {
                                        EntityMoniker = new EntityReference("campaignactivity", campaignActivityId),
                                        State = new OptionSetValue(0),
                                        Status = new OptionSetValue(6)
                                    };
                                    orgService.Execute(closeLead);

                                    #region |   Set Euromessage Response Period   |
                                    Entity campaignActivity = new Entity("campaignactivity");

                                    campaignActivity.Attributes.Add("activityid", campaignActivityId);
                                    campaignActivity.Attributes.Add("new_euromessagereportlimit", DateTime.Today.AddDays(7));

                                    orgService.Update(campaignActivity);
                                    #endregion |   Set Euromessage Response Period   |
                                }
                            }
                            else
                                logMe.Log("EmailFunctions - SetCampaignActivityDistributed", "Test sonuçları alınırken hata alındı. Code: " + rResult.Code + " Message: " + rResult.Message, TEMPEventLog.EventType.Info, "campaignactivity", campaignActivityId.ToString());
                        }
                        catch (Exception ex)
                        {
                            logMe.Log("EmailFunctions - SetCampaignActivityDistributed", ex, TEMPEventLog.EventType.Exception, "campaignactivity", campaignActivityId.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logMe.Log("EmailFunctions - SetCampaignActivityDistributed", ex, TEMPEventLog.EventType.Exception);
            }
        }
    }
}
