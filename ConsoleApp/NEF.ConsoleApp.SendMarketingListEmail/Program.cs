using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.ConsoleApp.SendMarketingListEmail
{
    class Program
    {
        static void Main(string[] args)
        {

            CreateMail crt = new CreateMail();
            crt.Execute();

            SendMailIntegration email = new SendMailIntegration();
            email.Execute();


            //GetEmailResponse response = new GetEmailResponse();
            //response.Execute();

            //GetFilteredMembers members = new GetFilteredMembers();
            //members.Execute();

            //UpdateCampaignActivity ca = new UpdateCampaignActivity();
            //ca.Execute();
        }
    }
}
