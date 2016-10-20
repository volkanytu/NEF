using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    [CrmSchemaName("new_loyaltysegmentconfig")]
    public class LoyaltySegmentConfig
    {
        [CrmFieldDataType(CrmDataType.UNIQUEIDENTIFIER)]
        [CrmFieldName("new_loyaltysegmentconfigid")]
        public Guid Id { get; set; }

        [CrmFieldDataType(CrmDataType.STRING)]
        [CrmFieldName("new_name")]
        public string Name { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("new_loyaltysegment")]
        public OptionSetValueWrapper LoyaltySegment { get; set; }

        [CrmFieldDataType(CrmDataType.DECIMAL)]
        [CrmFieldName("new_minvalue")]
        public decimal? MinValue { get; set; }

        [CrmFieldDataType(CrmDataType.DECIMAL)]
        [CrmFieldName("new_maxvalue")]
        public decimal? MaxValue { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statuscode")]
        public OptionSetValueWrapper Status { get; set; }

        [CrmFieldDataType(CrmDataType.OPTIONSETVALUE)]
        [CrmFieldName("statecode")]
        public OptionSetValueWrapper State { get; set; }

        public const string LOGICAL_NAME = "new_loyaltysegmentconfig";

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

        public enum LoyaltySegmentCode
        {
            RED = 1,
            BLACK = 2,
            WHITE = 3
        }
    }
}
