using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Constants.SqlQueries
{
    public class LoyaltyPointQueries
    {
        #region | GET_LOYALTY_POINT |
        public const string GET_LOYTALTY_POINT = @"SELECT
	                                                    LP.new_loyaltypointId AS Id
	                                                    ,LP.new_name AS Name
	                                                    ,LP.new_contactid AS ContactId
	                                                    ,LP.new_contactidName AS ContactIdName
	                                                    ,LP.new_quoteid AS QuoteId
	                                                    ,LP.new_quoteidName AS QuoteIdName
	                                                    ,LP.new_contractid AS ContractId
	                                                    ,LP.new_contractidName AS ContractIdName
	                                                    ,LP.new_projectid AS ProjectId
	                                                    ,LP.new_projectidName AS ProjectIdName
	                                                    ,LP.new_pointtype AS PointType
	                                                    ,LP.new_usagetype AS UsageType
	                                                    ,LP.new_amount AS Amount
	                                                    ,LP.new_expiredate AS [ExpireDate]
	                                                    ,LP.new_description AS [Description]
                                                    FROM
	                                                    new_loyaltypoint LP WITH (NOLOCK)
                                                    WHERE
	                                                    LP.new_loyaltypointId = @Id";

        #endregion
    }
}
