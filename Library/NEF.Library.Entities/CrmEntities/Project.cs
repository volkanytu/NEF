using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("new_project")]
    public class Project
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("new_projectid")]
        public Guid Id { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("new_name")]
        public string Name { get; set; }

        [CrmFieldDataType(CrmDataType.DECIMAL)]
        [CrmFieldName("new_loyaltypointratio")]
        public decimal? Ratio { get; set; }

        [CrmFieldDataType(CrmDataType.DATETIME)]
        [CrmFieldName("new_loyaltypointexpiredate")]
        public DateTime? ExpireDate { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statuscode")]
        public OptionSetValueWrapper Status { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statecode")]
        public OptionSetValueWrapper State { get; set; }

        public const string LOGICAL_NAME = "new_project";

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
    }
}
