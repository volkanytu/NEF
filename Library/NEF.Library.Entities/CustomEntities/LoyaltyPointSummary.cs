using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Entities.CustomEntities
{
    public class LoyaltyPointSummary
    {
        public EntityReferenceWrapper ContactId { get; set; }
        public decimal? TotalWonPoint { get; set; }
        public decimal? Balance { get; set; }
        public decimal? CardAmount { get; set; }
        public decimal? CashAmount { get; set; }
    }
}
