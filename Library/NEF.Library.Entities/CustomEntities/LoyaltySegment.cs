using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Entities.CustomEntities
{
    public class LoyaltySegment
    {
        public Guid ContactId { get; set; }
        public decimal TotalPoint { get; set; }
        public Contact.LoyaltySegmentCode? Segment { get; set; }
    }
}
