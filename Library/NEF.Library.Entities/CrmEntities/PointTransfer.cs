using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("new_pointtransfer")]
    public class PointTransfer
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("new_pointtransferid")]
        public Guid Id { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("new_name")]
        public string Name { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_sourcecontactid")]
        public EntityReferenceWrapper SourceContactId { get; set; }

        [CrmFieldDataType(CrmDataType.ENTITYREFERENCE)]
        [CrmFieldName("new_targetcontactid")]
        public EntityReferenceWrapper TargetContactId { get; set; }

        public const string LOGICAL_NAME = "new_pointtransfer";
        public const string SOURCE_CONTACT_ID = "new_sourcecontactid";
        public const string TARGET_CONTACT_ID = "new_targetcontactid";

        public enum StateCode
        {
            ACTIVE = 0,
            PASSIVE = 1
        }

        public enum StatusCode
        {
            ACTIVE = 1,
            PASSIVE = 2
        }
    }
}
