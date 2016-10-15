using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class FeedsHelper
    {

        public static List<UserFeed> GetUserOldFeeds(Guid userId, SqlDataAccess sda)
        {
            List<UserFeed> returnValue = new List<UserFeed>();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
                                    n.new_notificationId AS Id
                                FROM
                                new_notification AS n
                                WHERE
                                n.new_systemuserid='{0}' 
                                AND 
                                n.StatusCode!=100000002 --Okundu değil
                                ORDER BY n.CreatedOn DESC";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, userId));

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        UserFeed uf = FeedsHelper.GetFeedInfo((Guid)dt.Rows[i]["Id"], sda);

                        if (uf != null && uf.Id != Guid.Empty)
                            returnValue.Add(uf);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static UserFeed GetFeedInfo(Guid feedId, SqlDataAccess sda)
        {
            UserFeed returnValue = new UserFeed();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
                                    n.new_notificationId AS Id
                                    ,n.new_name AS Name
                                    ,n.new_url AS Url
                                    ,n.new_systemuserid AS UserId
                                    ,n.new_systemuseridName AS UserIdName
                                    ,n.new_notificationdescription AS Description
                                    ,n.new_notificationtype FeedType
                                    ,sm.Value AS FeedTypeName
                                FROM
                                new_notification AS n
	                                JOIN
		                                Entity AS e (NOLOCK)
			                                ON
			                                e.Name='new_notification'
	                                JOIN
		                                StringMap AS sm (NOLOCK)
			                                ON
			                                sm.ObjectTypeCode=e.ObjectTypeCode
			                                AND
			                                sm.AttributeName='new_notificationtype'
			                                AND
			                                sm.AttributeValue=n.new_notificationtype
                                WHERE
                                n.new_notificationId='{0}' ";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, feedId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Id = (Guid)dt.Rows[0]["Id"];
                    returnValue.Name = dt.Rows[0]["Name"].ToString();

                    if (dt.Rows[0]["FeedType"] != DBNull.Value)
                    {
                        StringMap sm = new StringMap()
                        {
                            Name = dt.Rows[0]["FeedTypeName"].ToString(),
                            Value = (int)dt.Rows[0]["FeedType"]
                        };

                        returnValue.FeedType = sm;
                    }

                    if (dt.Rows[0]["UserId"] != DBNull.Value)
                    {
                        EntityReference sm = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["UserId"],
                            Name = dt.Rows[0]["UserIdName"].ToString(),
                            LogicalName = "systemuser"
                        };

                        returnValue.User = sm;
                    }

                    if (dt.Rows[0]["Url"] != DBNull.Value)
                    {
                        returnValue.Url = dt.Rows[0]["Url"].ToString();
                    }


                    if (dt.Rows[0]["Description"] != DBNull.Value)
                    {
                        returnValue.Description = dt.Rows[0]["Description"].ToString();
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static List<UserFeed> GetNotPostedFeeds(SqlDataAccess sda)
        {
            List<UserFeed> returnValue = new List<UserFeed>();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
                                    n.new_notificationId AS Id
                                FROM
                                new_notification AS n
                                WHERE
                                n.StatusCode=1 --Beklemede
                                ORDER BY n.CreatedOn DESC";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery));

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        UserFeed uf = FeedsHelper.GetFeedInfo((Guid)dt.Rows[i]["Id"], sda);

                        if (uf != null && uf.Id != Guid.Empty)
                            returnValue.Add(uf);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static MsCrmResult UpdateFeedAsPosted(Guid feedId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {

                Entity ent = new Entity("new_notification");
                ent.Id = feedId;
                ent["statuscode"] = new OptionSetValue(100000000);

                service.Update(ent);

                returnValue.Success = true;
                returnValue.Result = "Bildirim kaydı güncellendi.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult UpdateFeedAsRead(Guid feedId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {

                Entity ent = new Entity("new_notification");
                ent.Id = feedId;
                ent["statuscode"] = new OptionSetValue(100000002); //Okundu

                service.Update(ent);

                returnValue.Success = true;
                returnValue.Result = "Bildirim kaydı güncellendi.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreateFeedEtity(UserFeed userFeed, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("new_notification");

                #region | FILL ENTITY INFO |
                if (!string.IsNullOrEmpty(userFeed.Name))
                {
                    ent["new_name"] = userFeed.Name;
                }
                else
                {
                    ent["new_name"] = "Bildirim";
                }

                if (userFeed.FeedType != null && userFeed.FeedType.Value != null)
                {
                    ent["new_notificationtype"] = new OptionSetValue((int)userFeed.FeedType.Value);
                }

                if (!string.IsNullOrEmpty(userFeed.Description))
                {
                    ent["new_notificationdescription"] = userFeed.Description;
                }

                if (userFeed.User != null && userFeed.User.Id != Guid.Empty)
                {
                    ent["new_systemuserid"] = userFeed.User;
                }

                if (!string.IsNullOrEmpty(userFeed.Url))
                {
                    ent["new_url"] = userFeed.Url;
                }

                if (userFeed.Status != null && userFeed.Status.Value != null)
                {
                    ent["statuscode"] = new OptionSetValue((int)userFeed.Status.Value);
                }

                #endregion

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Bildirim oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }
    }
}
