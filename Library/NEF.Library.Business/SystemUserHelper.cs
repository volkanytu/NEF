using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class SystemUserHelper
    {
        public static MsCrmResultObject GetSalesConsultants(UserTypes type, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                SU.SystemUserId
	                                ,SU.FullName
                                FROM
	                                SystemUser SU WITH (NOLOCK)
                                WHERE
                                    SU.IsDisabled = 0
                                    AND
	                                SU.new_jobstatus = {0}
                                ORDER BY
                                    SU.FullName ASC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)type));

                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.ReturnObject = dt.ToList<SystemUser>();
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }


        public static MsCrmResultObject GetSalesConsultants(List<UserTypes> types, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                List<int> idList = new List<int>();
                foreach (UserTypes currentType in types)
                {
                    idList.Add((int)currentType);
                }

                string ids = string.Join(",", idList);

                #region | SQL QUERY |
                string query = @"SELECT
	                                SU.SystemUserId
	                                ,SU.FullName
                                FROM
	                                SystemUser SU WITH (NOLOCK)
                                WHERE
                                    SU.IsDisabled = 0
                                    AND
	                                SU.new_jobstatus IN ({0})
                                ORDER BY
                                    SU.FullName ASC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, ids));

                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.ReturnObject = dt.ToList<SystemUser>();
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static SystemUser GetSystemUserInfo(Guid systemUserId, SqlDataAccess sda)
        {
            SystemUser returnValue = new SystemUser();

            #region | SQL DATA ACCCESS |

            string sqlQuery = @"SELECT
                                                su.SystemUserId
                                                ,su.FullName
                                                ,su.EntityImage
                                                ,su.EntityImage_URL
                                                ,su.EntityImageId
                                                ,su.new_jobstatus JobStatus
                                                ,su.BusinessUnitId As 'BusinessUnitId'
                                            FROM
                                                SystemUser AS su (NOLOCK)
                                            WHERE
                                                su.SystemUserId=@systemuserId";

            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@systemuserId", systemUserId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0]["BusinessUnitId"]).Equals(Globals.AlternatifBusinessUnitId.ToString()))
                {
                    returnValue = GetSystemUserInfoRetailer(systemUserId, sda);
                }
                else
                {
                    returnValue.SystemUserId = (Guid)dt.Rows[0]["SystemUserId"];
                    returnValue.FullName = dt.Rows[0]["FullName"].ToString();

                    if (dt.Rows[0]["EntityImage"] != DBNull.Value)
                    {
                        returnValue.Image = (byte[])dt.Rows[0]["EntityImage"];
                    }

                    if (dt.Rows[0]["JobStatus"] != DBNull.Value)
                    {
                        returnValue.UserType = (UserTypes)((int)dt.Rows[0]["JobStatus"]);
                    }
                }
            }
            return returnValue;
        }

        public static SystemUser GetSystemUserInfoRetailer(Guid systemUserId, SqlDataAccess sda)
        {
            SystemUser returnValue = new SystemUser();

            #region | SQL DATA ACCCESS |


            string sqlQuery = @"select 
	                                    S.DomainName
                                    ,S.SystemUserId
                                    ,S.FullName
                                    ,S.EntityImage
                                    ,S.EntityImage_URL
                                    ,S.EntityImageId
                                    ,S.new_jobstatus JobStatus
                                    ,S.BusinessUnitId  
                                    ,T.TeamId 
                                from 
	                                SystemUser as S (NOLOCK)
                                INNER JOIN	
	                                TeamMembership AS TM (NOLOCK)
                                ON
	                                S.SystemUserId = TM.SystemUserId
                                INNER JOIN 
	                                Team AS T (NOLOCK)
                                ON
                                    T.IsDefault = 0 AND
	                                T.TeamId = TM.TeamId WHERE S.SystemUserId=@systemuserId
                                ORDER BY 
	                                S.FullName ";

            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@systemuserId", systemUserId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue.SystemUserId = (Guid)dt.Rows[0]["SystemUserId"];
                returnValue.FullName = dt.Rows[0]["FullName"].ToString();

                if (dt.Rows[0]["EntityImage"] != DBNull.Value)
                {
                    returnValue.Image = (byte[])dt.Rows[0]["EntityImage"];
                }

                if (dt.Rows[0]["JobStatus"] != DBNull.Value)
                {
                    returnValue.UserType = (UserTypes)((int)dt.Rows[0]["JobStatus"]);
                }
                if (dt.Rows[0]["BusinessUnitId"] != DBNull.Value)
                {
                    returnValue.BusinessUnitId = (Guid)dt.Rows[0]["BusinessUnitId"];
                }
                if (dt.Rows[0]["TeamId"] != DBNull.Value)
                {
                    returnValue.TeamId = (Guid)dt.Rows[0]["TeamId"];
                }
            }
            return returnValue;
        }

        public static MsCrmResultObject GetSalesManager(SqlDataAccess sda)
        {

            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    SU.SystemUserId
                                        ,SU.FullName
                                    FROM
	                                    SystemUser SU WITH (NOLOCK)
                                    WHERE
                                    SU.IsDisabled = 0
                                    AND
	                                SU.new_jobstatus = {0}";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, (int)UserTypes.SatisMuduru));

                if (dt.Rows.Count > 0)
                {
                    SystemUser user = new SystemUser();

                    user.SystemUserId = (Guid)dt.Rows[0]["SystemUserId"];
                    user.FullName = dt.Rows[0]["FullName"].ToString();

                    returnValue.ReturnObject = user;
                    returnValue.Success = true;

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde satış müdürü rolüne ait kullanıcı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetUsersByUserTypes(UserTypes type, SqlDataAccess sda)
        {

            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    SU.SystemUserId
                                        ,SU.FullName
                                    FROM
	                                    SystemUser SU WITH (NOLOCK)
                                    WHERE
                                    SU.IsDisabled = 0
                                    AND
	                                SU.new_jobstatus = {0}";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, (int)type));

                if (dt.Rows.Count > 0)
                {
                    List<SystemUser> returnList = new List<SystemUser>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SystemUser user = new SystemUser();

                        user.SystemUserId = (Guid)dt.Rows[i]["SystemUserId"];
                        user.FullName = dt.Rows[i]["FullName"].ToString();

                        returnList.Add(user);
                    }


                    returnValue.ReturnObject = returnList;
                    returnValue.Success = true;

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde istediğiniz role ait kullanıcı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetUsersByUserTypesWithIsGyo(UserTypes type, SqlDataAccess sda)
        {

            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    SU.SystemUserId
                                        ,SU.FullName
                                    FROM
	                                    SystemUser SU WITH (NOLOCK)
                                    WHERE
                                    SU.IsDisabled = 0
                                    AND
	                                SU.new_jobstatus IN ({0},{1})";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, (int)type, (int)UserTypes.IsGyoSatisMuduru));

                if (dt.Rows.Count > 0)
                {
                    List<SystemUser> returnList = new List<SystemUser>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SystemUser user = new SystemUser();

                        user.SystemUserId = (Guid)dt.Rows[i]["SystemUserId"];
                        user.FullName = dt.Rows[i]["FullName"].ToString();

                        returnList.Add(user);
                    }


                    returnValue.ReturnObject = returnList;
                    returnValue.Success = true;

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde istediğiniz role ait kullanıcı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetCallCenterUser(SqlDataAccess sda)
        {

            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    SU.SystemUserId
                                        ,SU.FullName
                                    FROM
	                                    SystemUser SU WITH (NOLOCK)
                                    WHERE
                                    SU.IsDisabled = 0
                                    AND
	                                SU.new_jobstatus = {0}";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, (int)UserTypes.CallCenter));

                if (dt.Rows.Count > 0)
                {
                    SystemUser user = new SystemUser();

                    user.SystemUserId = (Guid)dt.Rows[0]["SystemUserId"];
                    user.FullName = dt.Rows[0]["FullName"].ToString();

                    returnValue.ReturnObject = user;
                    returnValue.Success = true;

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde satış müdürü rolüne ait kullanıcı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static SystemUser GetSystemUserByDomainName(string domainName, SqlDataAccess sda)
        {
            SystemUser userInfo = new SystemUser();

            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                SU.SystemUserId
                                    ,SU.FullName
                                FROM
                                    SystemUser SU (NoLock)
								WHERE
	                                SU.IsDisabled = 0
	                            AND
	                                SU.DomainName LIKE '%{0}%'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, domainName));
                if (dt != null && dt.Rows.Count > 0)
                {
                    userInfo = SystemUserHelper.GetSystemUserInfo((Guid)dt.Rows[0]["SystemUserId"], sda);
                }
            }
            catch (Exception)
            {

            }

            return userInfo;
        }

        public static UserHeaderInfo GetUserHeaderInfo(Guid userId, SqlDataAccess sda)
        {
            UserHeaderInfo returnValue = new UserHeaderInfo();

            try
            {
                #region | SQL QUERY |
                DateTime baseDate = DateTime.Today;
                DateTime thisMonthStart = baseDate.AddDays(1 - baseDate.Day).ToUniversalTime();
                DateTime thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1).ToUniversalTime();

                string sqlQuery = @"SELECT

                                (
	                                SELECT
		                                COUNT(0) AS RecCount
	                                FROM
	                                (
		                                SELECT
			                                pc.ActivityId
		                                FROM
			                                PhoneCall AS pc (NOLOCK)
                                        INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                            ON
                                            AP.ActivityId = pc.ActivityId
                                            AND
                                            AP.ParticipationTypeMask = 2 -- TO
	                                        AND
	                                        AP.PartyObjectTypeCode IN(1,2) -- Account,Contact
	                                        AND
	                                        pc.OwnerId='{0}'
                                            AND
                                            pc.StateCode = 0

		                                UNION

		                                SELECT
			                                app.ActivityId
		                                FROM
			                                Appointment AS app (NOLOCK)
		                                INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                            ON
                                            AP.ActivityId = app.ActivityId
                                            AND
                                            AP.ParticipationTypeMask = 5 -- TO
	                                        AND
	                                        AP.PartyObjectTypeCode IN(1,2) -- Account,Contact
	                                        AND
	                                        app.OwnerId='{0}'
                                            AND
                                            app.StateCode IN(0,3)
	                                ) AS A
                                ) AS OpenActivityCount
                                ,
                                (
	                                SELECT
		                                COUNT(0) AS OpenOppCount
	                                FROM
		                                Opportunity AS opp (NOLOCK)
	                                WHERE
		                                opp.OwnerId='{0}' AND opp.StateCode=0
                                ) AS OpenOppCount
                                ,
                                (
	                                SELECT
		                                COUNT(0)
	                                FROM
		                                Quote AS q (NOLOCK)
	                                WHERE
		                                q.OwnerId='{0}'
		                                AND
		                                q.new_salesprocessdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                        AND
                                        q.StatusCode IN (2,100000001,100000007,100000009)
                                ) AS SalesCount	
                                ,
                                (
	                                SELECT
		                               SUM(ISNULL( q.TotalAmountLessFreight * e.new_salesrate,q.TotalAmountLessFreight))
	                                FROM
		                                Quote AS q (NOLOCK)
LEFT JOIN 
										new_exchangerate AS e (NOLOCK)
											ON
											Convert(DateTime, Convert(VarChar, q.new_salesprocessdate, 12)) = Convert(DateTime, Convert(VarChar, e.new_currencydate, 12))
											AND
											q.TransactionCurrencyId = e.new_currencyid
	                                WHERE
		                                q.OwnerId='{0}'
		                                AND
		                                q.new_salesprocessdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                        AND
                                        q.StatusCode IN (2,100000001,100000007,100000009)
                                ) AS SalesAmount
                                ,
                                (
	                                SELECT
		                                TOP 1
		                                ct.new_amounttarget
	                                FROM
		                                new_consultanttarget AS ct (NOLOCK)
	                                WHERE
		                                ct.new_userid='{0}'
	                                AND
		                                ct.new_targetdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                ) AS AmountTarget
                                ,
                                (
	                                SELECT
		                                TOP 1
		                                ct.new_quantitytarget
	                                FROM
		                                new_consultanttarget AS ct (NOLOCK)
	                                WHERE
		                                ct.new_userid='{0}'
	                                AND
		                                ct.new_targetdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                ) AS QuantityTarget";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, userId), new SqlParameter[] { new SqlParameter("beginOfThisMonth", thisMonthStart), new SqlParameter("endOfThisMonth", thisMonthEnd) });

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["OpenActivityCount"] != DBNull.Value)
                    {
                        returnValue.OpenActivityCount = (int)dt.Rows[0]["OpenActivityCount"];
                    }

                    if (dt.Rows[0]["OpenOppCount"] != DBNull.Value)
                    {
                        returnValue.OpenOppCount = (int)dt.Rows[0]["OpenOppCount"];
                    }

                    if (dt.Rows[0]["SalesCount"] != DBNull.Value)
                    {
                        returnValue.SalesCount = (int)dt.Rows[0]["SalesCount"];
                    }

                    if (dt.Rows[0]["SalesAmount"] != DBNull.Value)
                    {
                        returnValue.SalesAmount = (decimal)dt.Rows[0]["SalesAmount"];
                    }

                    if (dt.Rows[0]["AmountTarget"] != DBNull.Value)
                    {
                        returnValue.PlannedSalesAmount = (decimal)dt.Rows[0]["AmountTarget"];
                    }

                    if (dt.Rows[0]["QuantityTarget"] != DBNull.Value)
                    {
                        returnValue.PlannedSalesCount = (int)dt.Rows[0]["QuantityTarget"];
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return returnValue;
        }

        public static UserHeaderInfo GetAllHeaderInfo(SqlDataAccess sda)
        {
            UserHeaderInfo returnValue = new UserHeaderInfo();

            try
            {
                #region | SQL QUERY |
                DateTime baseDate = DateTime.Today;
                DateTime thisMonthStart = baseDate.AddDays(1 - baseDate.Day).ToUniversalTime();
                DateTime thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1).ToUniversalTime();

                string sqlQuery = @"SELECT

                                (
	                                SELECT
		                                COUNT(0) AS RecCount
	                                FROM
	                                (
		                                SELECT
			                                pc.ActivityId
		                                FROM
			                                PhoneCall AS pc (NOLOCK)
			                                INNER JOIN 
											SystemUser AS SY WITH(NOLOCK)
										ON
											pc.OwnerId = SY.SystemUserId
                                        INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                            ON
                                            AP.ActivityId = pc.ActivityId
                                            AND
                                            AP.ParticipationTypeMask = 2 -- TO
	                                        AND
	                                        AP.PartyObjectTypeCode IN(1,2) -- Account,Contact
	                                        AND
                                            pc.StateCode = 0
                                            AND
                                            SY.new_jobstatus = 2

		                                UNION

		                                SELECT
			                                app.ActivityId
		                                FROM
			                                Appointment AS app (NOLOCK)
			                            INNER JOIN 
											SystemUser AS SY WITH(NOLOCK)
										ON
											app.OwnerId = SY.SystemUserId
		                                INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                        ON
                                            AP.ActivityId = app.ActivityId
                                            AND
                                            AP.ParticipationTypeMask = 5 -- TO
	                                        AND
	                                        AP.PartyObjectTypeCode IN(1,2) -- Account,Contact
	                                        AND
                                            app.StateCode IN(0,3)
                                            AND
                                            SY.new_jobstatus = 2
	                                ) AS A
                                ) AS OpenActivityCount
                                ,
                                (
	                                SELECT
		                                COUNT(0) AS OpenOppCount
	                                FROM
		                                Opportunity AS opp (NOLOCK)
		                            INNER JOIN 
											SystemUser AS SY WITH(NOLOCK)
										ON
											opp.OwnerId = SY.SystemUserId
	                                WHERE
		                                opp.StateCode=0
		                           AND
                                        SY.new_jobstatus = 2
                                ) AS OpenOppCount
                                ,
                                (
	                                SELECT
		                                COUNT(0)
	                                FROM
		                                Quote AS q (NOLOCK)
	                                WHERE
		                                q.new_salesprocessdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                        AND
                                        q.StatusCode IN (2,100000001,100000007,100000009)
                                ) AS SalesCount	
                                ,
                                (
	                                SELECT
		                                SUM(q.TotalAmount)
	                                FROM
		                                Quote AS q (NOLOCK)
	                                WHERE
		                                q.new_salesprocessdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                        AND
                                        q.StatusCode IN (2,100000001,100000007,100000009)
                                ) AS SalesAmount
                                ,
                                (
	                                SELECT		                                
		                                SUM(ct.new_amounttarget)
	                                FROM
		                                new_consultanttarget AS ct (NOLOCK)
	                                WHERE
		                                ct.new_targetdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                ) AS AmountTarget
                                ,
                                (
	                                SELECT
		                                SUM(ct.new_quantitytarget)
	                                FROM
		                                new_consultanttarget AS ct (NOLOCK)
	                                WHERE
		                                ct.new_targetdate BETWEEN @beginOfThisMonth AND @endOfThisMonth
                                ) AS QuantityTarget";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery), new SqlParameter[] { new SqlParameter("beginOfThisMonth", thisMonthStart), new SqlParameter("endOfThisMonth", thisMonthEnd) });

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["OpenActivityCount"] != DBNull.Value)
                    {
                        returnValue.OpenActivityCount = (int)dt.Rows[0]["OpenActivityCount"];
                    }

                    if (dt.Rows[0]["OpenOppCount"] != DBNull.Value)
                    {
                        returnValue.OpenOppCount = (int)dt.Rows[0]["OpenOppCount"];
                    }

                    if (dt.Rows[0]["SalesCount"] != DBNull.Value)
                    {
                        returnValue.SalesCount = (int)dt.Rows[0]["SalesCount"];
                    }

                    if (dt.Rows[0]["SalesAmount"] != DBNull.Value)
                    {
                        returnValue.SalesAmount = (decimal)dt.Rows[0]["SalesAmount"];
                    }

                    if (dt.Rows[0]["AmountTarget"] != DBNull.Value)
                    {
                        returnValue.PlannedSalesAmount = (decimal)dt.Rows[0]["AmountTarget"];
                    }

                    if (dt.Rows[0]["QuantityTarget"] != DBNull.Value)
                    {
                        returnValue.PlannedSalesCount = (int)dt.Rows[0]["QuantityTarget"];
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return returnValue;
        }
    }
}
