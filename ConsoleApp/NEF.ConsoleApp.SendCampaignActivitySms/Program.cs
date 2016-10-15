using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.ConsoleApp.SendCampaignActivitySms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var parameter = args[0].Replace("-", "").Replace("/", "").Replace(" ", "").Trim();

                if (parameter == "1")  // SMS gönderimi ise
                {
                    CreateSms createSms = new CreateSms();
                    createSms.Execute();

                    SendSmsIntegration SendSms = new SendSmsIntegration();
                    SendSms.Execute();
                }
                else if (parameter == "2") // Request yapılacak ise
                {
                    if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 1)
                    {
                        GetSmsResponse request = new GetSmsResponse();
                    request.ExecuteRequest();
                    }
                }
                else if (parameter == "3") // Response alınacak ise
                {
                    if (DateTime.Now.Hour >= 2 && DateTime.Now.Hour < 7)
                    {
                        GetSmsResponse response = new GetSmsResponse();
                        response.ExecuteResponse();

                        UpdateCampaignActivity ca = new UpdateCampaignActivity();
                        ca.Execute();
                    }
                }
            }
        }
    }
}
