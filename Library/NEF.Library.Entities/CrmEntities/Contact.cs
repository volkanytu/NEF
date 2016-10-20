using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("contact")]
    public class Contact
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("contactid")]
        public Guid Id { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("fullname")]
        public string Name { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_loyaltysegment")]
        public OptionSetValueWrapper LoyaltySegment { get; set; }

        public const string LOGICAL_NAME = "contact";

        public enum LoyaltySegmentCode
        {
            RED = 1,
            BLACK,
            WHITE
        }
    }
}
