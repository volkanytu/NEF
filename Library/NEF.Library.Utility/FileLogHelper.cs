using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NEF.Library.Utility
{
    public class FileLogHelper
    {
        public static void LogEvent(string message, string logPath)
        {
            try
            {
                string logPathFileToday = logPath + DateTime.Now.ToString("yyyy.MM.dd") + ".txt";

                string logMessage = String.Format("Log Date: {0}Log Message: {1} *-----------*-----------*-----------*", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine, Environment.NewLine + message + Environment.NewLine);

                if (!string.IsNullOrEmpty(logPath))
                {
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    if (!File.Exists(logPathFileToday))
                    {
                        FileStream _fs = new FileStream(logPathFileToday, FileMode.OpenOrCreate);
                        _fs.Close();
                        File.AppendAllText(logPathFileToday, logMessage + Environment.NewLine);
                    }
                    else
                    {
                        FileStream _fs = new FileStream(logPathFileToday, FileMode.Open);
                        _fs.Close();
                        File.AppendAllText(logPathFileToday, logMessage + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}