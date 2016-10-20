using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("new_loyaltypoint")]
    public class LoyaltyPoint
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("new_loyaltypointid")]
        public Guid Id { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("new_name")]
        public string Name { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_contactid")]
        public EntityReferenceWrapper ContactId { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_quoteid")]
        public EntityReferenceWrapper QuoteId { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_paymentid")]
        public EntityReferenceWrapper PaymentId { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_projectid")]
        public EntityReferenceWrapper ProjectId { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_pointtype")]
        public OptionSetValueWrapper PointType { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_usagetype")]
        public OptionSetValueWrapper UsageType { get; set; }

        [CrmFieldDataType(CrmDataType.DECIMAL)]
        [CrmFieldName("new_amount")]
        public decimal? Amount { get; set; }

        [CrmFieldDataType(CrmDataType.DATETIME)]
        [CrmFieldName("new_expiredate")]
        public DateTime? ExpireDate { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("new_description")]
        public string Description { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statuscode")]
        public OptionSetValueWrapper Status { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statecode")]
        public OptionSetValueWrapper State { get; set; }

        public decimal ReelValue
        {
            get
            {
                if (PointType != null && Amount != null)
                {
                    if (PointType.ToEnum<PointTypeCode>() == PointTypeCode.LESSENING
                        || PointType.ToEnum<PointTypeCode>() == PointTypeCode.SPENDING)
                    {
                        return Amount.Value * -1;
                    }
                    else
                    {
                        return Amount.Value;
                    }
                }

                return 0;
            }
        }
        public const string LOGICAL_NAME = "new_loyaltypoint";

        public enum StateCode
        {
            ACTIVE = 0,
            PASSIVE = 1
        }

        public enum StatusCode
        {
            ACTIVE = 1,
            PASSIVE = 2,
            WAITING_CONFIRMED,
            CONFIRMED,
        }

        public enum UsageTypeCode
        {
            CARD = 1,
            CASH = 2
        }

        public enum PointTypeCode
        {
            EARNING = 1,
            SPENDING = 2,
            ADDING = 3,
            LESSENING = 4
        }
    }
}
