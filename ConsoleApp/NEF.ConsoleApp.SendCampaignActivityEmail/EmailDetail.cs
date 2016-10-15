using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.SendCampaignActivityEmail
{
    public class EmailDetail
    {
        public string Key { get; set; }
        public string RecordId { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerMailAddres { get; set; }
        public string ActivityId { get; set; }
        public string Subject { get; set; }
    }
}
