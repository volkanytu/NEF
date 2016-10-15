using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.WebServices.Common
{
    class PrePayment
    {
        public Guid PaymentId { get; set; }
        public string CustomerName { get; set; }
        public string QuoteName { get; set; }
        public string Amount { get; set; }
        public string AmountDate { get; set; }
        public StringMap VoucherType { get; set; }
        public StringMap AmountType { get; set; }
    }
}
