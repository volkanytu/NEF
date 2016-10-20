using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("quote")]
    public class Quote
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("quoteid")]
        public Guid Id { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("name")]
        public string Name { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_referencecontactid")]
        public EntityReferenceWrapper ReferenceContactId { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_usagetype")]
        public OptionSetValueWrapper UsageType { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statuscode")]
        public OptionSetValueWrapper Status { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statecode")]
        public OptionSetValueWrapper State { get; set; }

        public const string LOGICAL_NAME = "quote";

        public enum StateCode
        {
            ACTIVE = 0,
            PASSIVE = 1
        }

        public enum StatusCode
        {
            ACTIVE = 1,
            PASSIVE = 2,
        }

        public enum UsageTypeCode
        {
            CARD = 1,
            CASH = 2
        }
    }
}
