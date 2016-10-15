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

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_pointtype")]
        public OptionSetValueWrapper PointType { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_contactid")]
        public EntityReferenceWrapper ContactId { get; set; }

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

        public enum PointTypeCode
        {
            CARD=100000000
        }
    }
}
