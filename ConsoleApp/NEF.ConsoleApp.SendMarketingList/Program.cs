using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.ConsoleApp.SendMarketingList
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateSms CreateSms = new SendMarketingList.CreateSms();
            CreateSms.Execute();
            SendSmsIntegration SendSms = new SendSmsIntegration();
            SendSms.Execute();
        }
    }
}
