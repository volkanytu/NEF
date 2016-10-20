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
	                                                    ,LP.new_paymentid AS PaymentId
	                                                    ,LP.new_paymentidName AS PaymentIdName
	                                                    ,LP.new_projectid AS ProjectId
	                                                    ,LP.new_projectidName AS ProjectIdName
	                                                    ,LP.new_pointtype AS PointType
	                                                    ,LP.new_usagetype AS UsageType
	                                                    ,LP.new_amount AS Amount
	                                                    ,LP.new_expiredate AS [ExpireDate]
	                                                    ,LP.new_description AS [Description]
                                                        ,LP.StateCode AS State
                                                        ,LP.StatusCode AS Status
                                                    FROM
	                                                    new_loyaltypoint LP WITH (NOLOCK)
                                                    WHERE
	                                                    LP.new_loyaltypointId = @id";

        #endregion

        #region | GET_WON_POINTS_OF_CONTACT |

        public const string GET_WON_POINTS_OF_CONTACT = @"DECLARE @EndDate DATETIME = GETUTCDATE()
                            SELECT
	                            LP.new_contactid AS ContactId
	                            ,SUM(LP.new_amount) AS TotalPoint
                            FROM
	                            new_loyaltypoint LP WITH (NOLOCK)
                            WHERE
	                            LP.StateCode = 0
	                            AND
	                            LP.StatusCode = 100000001 --Onaylandı
                                AND
	                            LP.new_expiredate > @EndDate
                                AND
                                LP.new_pointtype = 1 --KAZANIM
                            GROUP BY LP.new_contactid";

        #endregion

        #region | GET_ALL_POINTS_OF_CONTACT |
        public const string GET_ALL_POINTS_OF_CONTACT = @"SELECT
	                                                    LP.new_loyaltypointId AS Id
	                                                    ,LP.new_name AS Name
	                                                    ,LP.new_contactid AS ContactId
	                                                    ,LP.new_contactidName AS ContactIdName
	                                                    ,LP.new_quoteid AS QuoteId
	                                                    ,LP.new_quoteidName AS QuoteIdName
	                                                    ,LP.new_paymentid AS PaymentId
	                                                    ,LP.new_paymentidName AS PaymentIdName
	                                                    ,LP.new_projectid AS ProjectId
	                                                    ,LP.new_projectidName AS ProjectIdName
	                                                    ,LP.new_pointtype AS PointType
	                                                    ,LP.new_usagetype AS UsageType
	                                                    ,LP.new_amount AS Amount
	                                                    ,LP.new_expiredate AS [ExpireDate]
	                                                    ,LP.new_description AS [Description]
                                                        ,LP.StateCode AS State
                                                        ,LP.StatusCode AS Status
                                                    FROM
	                                                    new_loyaltypoint LP WITH (NOLOCK)
                                                    WHERE
	                                                    LP.new_contactid = @contactId";

        #endregion
    }
}
