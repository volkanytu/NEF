using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using System.Net;
using System.IO;
using Chilkat;

namespace NEF.ConsoleApp.ISGYOUploadCrmDataToFtp
{
    public class SendFilesToFtp
    {
        string _dataFolder = string.Empty;
        string _ftpUrl = string.Empty;
        string _userName = string.Empty;
        string _password = string.Empty;

        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 24;

        public MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            string[] filePaths = Directory.GetFiles(@_dataFolder, "*.xlsx");

            if (filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    //MsCrmResult resultUpload = SendFile(filePaths[i], Path.GetFileName(filePaths[i]));
                    //upload(Path.GetFileName(filePaths[i]), filePaths[i]);
                    UploadViaSftp(filePaths[i]);

                }
            }
            return returnValue;
        }

        public MsCrmResult SendFile(string filePath, string fileName)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpUrl + "/isgyodata/" + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(_userName, _password);

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(filePath);

                byte[] fileContents = Encoding.Default.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                returnValue.Result = string.Format("Upload File Complete, status {0}", response.StatusDescription);
                returnValue.Success = true;

                response.Close();
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public void upload(string remoteFile, string localFile)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(_ftpUrl + "/isgyodata/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(_userName, _password);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream = File.OpenRead(localFile);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = 0;

                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    do
                    {
                        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                    }

                    while (bytesSent != 0);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        public void UploadViaSftp(string filePath)
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
            hostname = _ftpUrl;
            port = 22;
            success = sftp.Connect(hostname, port);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Authenticate with the SSH server.  Chilkat SFTP supports
            //  both password-based authenication as well as public-key
            //  authentication.  This example uses password authenication.
            success = sftp.AuthenticatePw(_userName, _password);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  After authenticating, the SFTP subsystem must be initialized:
            success = sftp.InitializeSftp();
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Open a file on the server for writing.
            //  "createTruncate" means that a new file is created; if the file already exists, it is opened and truncated.

            string handle;
            handle = sftp.OpenFile(Path.GetFileName(filePath), "writeOnly", "createTruncate");
            if (handle == null)
                throw new ApplicationException(sftp.LastErrorText);

            //  Upload from the local file to the SSH server.
            success = sftp.UploadFile(handle, filePath);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);

            //  Close the file.
            success = sftp.CloseHandle(handle);
            if (success != true)
                throw new ApplicationException(sftp.LastErrorText);
        }


        public SendFilesToFtp(string dataFolder, string ftpUrl, string userName, string password)
        {
            _dataFolder = dataFolder;
            _ftpUrl = ftpUrl;
            _userName = userName;
            _password = password;
        }
    }
}
