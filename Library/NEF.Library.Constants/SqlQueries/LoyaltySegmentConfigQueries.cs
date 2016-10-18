using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Constants.SqlQueries
{
    public class LoyaltySegmentConfigQueries
    {
        #region | GET_CONFIG |
        public const string GET_CONFIG = @""; 
        #endregion

        #region | GET_LIST |
        public const string GET_LIST = @"SELECT
	                                            LSC.new_loyaltysegmentconfigId AS Id
	                                            ,LSC.new_name AS Name
	                                            ,LSC.new_loyaltysegment AS LoyaltySegment
	                                            ,LSC.new_minvalue AS MinValue
	                                            ,LSC.new_maxvalue AS MaxValue
                                            FROM
	                                            new_loyaltysegmentconfig LSC WITH (NOLOCK)
                                            WHERE 
	                                            LSC.StateCode = 0"; 
        #endregion
    }
}
