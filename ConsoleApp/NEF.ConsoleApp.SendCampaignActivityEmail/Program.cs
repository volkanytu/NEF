using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var parameter = args[0].Replace("-", "").Replace("/", "").Replace(" ", "").Trim();

                if (parameter == "1")  // Mail gönderimi ise
                {
                    CreateMail crt = new CreateMail();
                    crt.Execute();

                    SendMailIntegration email = new SendMailIntegration();
                    email.Execute();

                }
                else if (parameter == "2") // Response alınacak ise
                {
                    if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
                    {
                        GetEmailResponse response = new GetEmailResponse();
                        response.Execute();

                        GetFilteredMembers members = new GetFilteredMembers();
                        members.Execute();

                        UpdateCampaignActivity ca = new UpdateCampaignActivity();
                        ca.Execute();
                    }
                }
            }
        }
    }
}
