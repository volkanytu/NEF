using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("new_pointransfer")]
    public class PointTransfer
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("new_pointransferid")]
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

        public const string LOGICAL_NAME = "new_pointransfer";
        public const string SOURCE_CONTACT_ID = "new_sourcecontactid";
        public const string TARGET_CONTACT_ID = "new_targetcontactid";
    }
}
