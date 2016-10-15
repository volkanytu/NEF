using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.ISGYOUploadCrmDataToFtp
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFolder = @Environment.CurrentDirectory + @"\log\";
            string dataFolder = @Environment.CurrentDirectory + @"\files\";

            //string logFolder = @"C:\isgyodata\log\";
            //string dataFolder = @"C:\isgyodata\files\";

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            ICollaborateData process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.HouseData);
            MsCrmResult resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result,logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.OpportunityData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.ContactData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.AppointmentData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.PhoneCallData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.PaymentData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.SalesData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.WebFormData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);

            process = GetDataFactory.Instance.GetCollaborationData(CollaborateDataType.AccountData);
            resultProcess = process.Process(sda);
            FileLogHelper.LogEvent(resultProcess.Success.ToString() + "|" + resultProcess.Result, logFolder);



            SendFilesToFtp sendFile = new SendFilesToFtp(dataFolder, "ftp://ftp.nef.com.tr", "indibox", "sGw1WMtj");
            sendFile.Process();
        }
    }
}
