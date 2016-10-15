using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class OpportunityHelper
    {
        public static MsCrmResultObject GetCustomerOpportunities(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                OP.OpportunityId 
	                                ,OP.OwnerId
	                                ,OP.OwnerIdName
	                                ,OP.CreatedOn
                                    ,OP.new_name AS OppCode
	                                ,OP.new_relatedactivitystatusid AS ActivityStatusId
	                                ,OP.new_relatedactivitystatusidName AS ActivityStatusIdName
	                                ,SM.Value Status
									,(
										SELECT	
										 COUNT(0)
										FROM
                                        OpportunityProduct AS oppPro (NOLOCK)
                                    WHERE
                                        oppPro.OpportunityId=OP.OpportunityId
								     )ProductCount
									 ,(
										 SELECT	
											TOP 1
											 PROJ.new_iswithisgyo
											FROM
											OpportunityProduct AS oppPro WITH (NOLOCK)
										INNER JOIN	
											Product P WITH (NOLOCK)
											ON
											oppPro.OpportunityId=OP.OpportunityId
											AND
											P.ProductId = oppPro.ProductId
										INNER JOIN
											new_project PROJ WITH (NOLOCK)
											ON
											PROJ.new_projectId = P.new_projectid
										ORDER BY oppPro.CreatedOn DESC
									 )ProjectIsGyo
                                FROM
	                                Opportunity OP WITH(NOLOCK)
                                INNER JOIN
	                                StringMap SM (NoLock)
	                                ON
	                                SM.ObjectTypeCOde = 3
	                                AND
	                                SM.AttributeName = 'statuscode'
	                                AND
	                                SM.AttributeValue = OP.StatusCode
                                WHERE
	                                OP.customerId = '{0}'
                                ORDER BY
                                    OP.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | MAKE SEARCH |
                    List<Opportunity> returnList = new List<Opportunity>();
                    //returnValue.ReturnObject = dt.ToList<Contact>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Opportunity _opportunity = new Opportunity();
                        _opportunity.OpportunityId = (Guid)dt.Rows[i]["OpportunityId"];
                        _opportunity.CreatedOnString = dt.Rows[i]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[i]["CreatedOn"]).ToString("dd.MM.yyyy") : "";
                        _opportunity.Status = dt.Rows[i]["Status"] != DBNull.Value ? dt.Rows[i]["Status"].ToString() : string.Empty;
                        _opportunity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString() };
                        _opportunity.OpportunityCode = dt.Rows[i]["OppCode"] != DBNull.Value ? dt.Rows[i]["OppCode"].ToString() : string.Empty;
                        _opportunity.IsProjectGYO = dt.Rows[0]["ProjectIsGyo"] != DBNull.Value ? (bool)dt.Rows[0]["ProjectIsGyo"] : false;

                        if (dt.Rows[i]["ProductCount"] != DBNull.Value)
                        {
                            _opportunity.OppProductCount = (int)dt.Rows[i]["ProductCount"];
                        }

                        if (dt.Rows[0]["ActivityStatusId"] != DBNull.Value)
                            _opportunity.ActivityStatus = new EntityReference() { Name = dt.Rows[0]["ActivityStatusIdName"].ToString(), Id = (Guid)dt.Rows[0]["ActivityStatusId"], LogicalName = "new_activitystatusdetail" };

                        returnList.Add(_opportunity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait fırsat bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetUserOpportunities(Guid userId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                OP.OpportunityId 
	                                ,OP.OwnerId
	                                ,OP.OwnerIdName
	                                ,OP.CreatedOn
	                                ,SM.Value StatusValue
                                    ,SM2.Value StateValue    
	                                ,OP.StatusCode
	                                ,OP.CustomerId
	                                ,OP.CustomerIdName
                                    ,OP.CustomerIdType
                                    ,OP.StateCode
                                    ,OP.new_relatedactivitystatusid ActivityStatusDetailId
                                    ,OP.new_relatedactivitystatusidName ActivityStatusDetailIdName
                                    ,(
										SELECT	
										 COUNT(0)
										FROM
                                        OpportunityProduct AS oppPro (NOLOCK)
                                    WHERE
                                        oppPro.OpportunityId=OP.OpportunityId
								     )ProductCount
                                FROM
	                                Opportunity OP WITH(NOLOCK)
                                INNER JOIN
	                                StringMap SM (NoLock)
	                                ON
	                                SM.ObjectTypeCOde = 3
	                                AND
	                                SM.AttributeName = 'statuscode'
	                                AND
	                                SM.AttributeValue = OP.StatusCode
                                INNER JOIN
	                                StringMap SM2 (NoLock)
	                                ON
	                                SM2.ObjectTypeCOde = 3
	                                AND
	                                SM2.AttributeName = 'statecode'
	                                AND
	                                SM2.AttributeValue = OP.StateCode
                                WHERE
	                                OP.OwnerId=@userId
                                AND
	                                OP.CustomerId IS NOT NULL ORDER BY OP.CreatedOn DESC";
                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", userId) };

                DataTable dt = sda.getDataTable(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | MAKE SEARCH |
                    List<Opportunity> returnList = new List<Opportunity>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Opportunity _opportunity = new Opportunity();
                        _opportunity.OpportunityId = (Guid)dt.Rows[i]["OpportunityId"];
                        _opportunity.CreatedOnString = dt.Rows[i]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[i]["CreatedOn"]).ToString("dd.MM.yyyy HH:mm") : "";
                        _opportunity.Status = dt.Rows[i]["StatusValue"] != DBNull.Value ? dt.Rows[i]["StatusValue"].ToString() : string.Empty;
                        _opportunity.StatusCode = new StringMap() { Name = dt.Rows[i]["StatusValue"].ToString(), Value = (int)dt.Rows[i]["StatusCode"] };
                        _opportunity.StateCode = new StringMap() { Name = dt.Rows[i]["StateValue"].ToString(), Value = (int)dt.Rows[i]["StateCode"] };
                        _opportunity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString() };
                        _opportunity.Contact = new EntityReference() { Name = dt.Rows[i]["CustomerIdName"].ToString(), Id = (Guid)dt.Rows[i]["CustomerId"], LogicalName = "contact" };

                        if (dt.Rows[i]["ProductCount"] != DBNull.Value)
                        {
                            _opportunity.OppProductCount = (int)dt.Rows[i]["ProductCount"];
                        }

                        if (dt.Rows[i]["CustomerIdType"] != DBNull.Value)
                        {
                            _opportunity.CustomerType = (int)dt.Rows[i]["CustomerIdType"];
                        }

                        if (dt.Rows[i]["ActivityStatusDetailId"] != DBNull.Value)
                        {
                            _opportunity.ActivityStatusDetail = new EntityReference() { Name = dt.Rows[i]["ActivityStatusDetailIdName"].ToString(), Id = (Guid)dt.Rows[i]["ActivityStatusDetailId"], LogicalName = "new_activitystatusdetail" };
                        }

                        returnList.Add(_opportunity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetOpportunityProducts(Guid oppId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
                                        oppPro.OpportunityProductId AS Id
                                        ,oppPro.OpportunityId
                                        ,oppPro.ProductId
                                    FROM
                                        OpportunityProduct AS oppPro (NOLOCK)
                                    WHERE
                                        oppPro.OpportunityId=@oppId";
                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@oppId", oppId) };

                DataTable dt = sda.getDataTable(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | MAKE SEARCH |
                    List<Product> returnList = new List<Product>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product pInfo = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);

                        if (pInfo != null && pInfo.ProductId != Guid.Empty)
                        {
                            returnList.Add(pInfo);
                        }
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static Opportunity GetOpportunityDetail(Guid oppId, SqlDataAccess sda)
        {
            Opportunity returnValue = new Opportunity();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                OP.OpportunityId 
	                                ,OP.OwnerId
	                                ,OP.OwnerIdName
	                                ,OP.CreatedOn
	                                ,SM.Value StatusValue
                                    ,SM2.Value StateValue    
	                                ,OP.StatusCode
	                                ,OP.CustomerId
	                                ,OP.CustomerIdName
                                    ,OP.CustomerIdType
                                    ,OP.StateCode
	                                ,OP.new_name AS OppCode
	                                ,OP.new_relatedactivitystatusid AS ActivityStatusId
	                                ,OP.new_relatedactivitystatusidName AS ActivityStatusIdName
	                                ,c.new_customertype AS ContactType
                                FROM
	                                Opportunity OP WITH(NOLOCK)
                                INNER JOIN
	                                StringMap SM (NoLock)
	                                ON
	                                SM.ObjectTypeCOde = 3
	                                AND
	                                SM.AttributeName = 'statuscode'
	                                AND
	                                SM.AttributeValue = OP.StatusCode
                                INNER JOIN
	                                StringMap SM2 (NoLock)
	                                ON
	                                SM2.ObjectTypeCOde = 3
	                                AND
	                                SM2.AttributeName = 'statecode'
	                                AND
	                                SM2.AttributeValue = OP.StateCode
                                LEFT JOIN
	                                Contact AS c (NOLOCK)
		                                ON
		                                OP.CustomerId=c.ContactId
                                WHERE
	                                OP.OpportunityId=@oppId";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@oppId", oppId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                #region | FILL OPP. INFO |

                returnValue.OpportunityId = (Guid)dt.Rows[0]["OpportunityId"];
                returnValue.CreatedOnString = dt.Rows[0]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[0]["CreatedOn"]).ToString("dd.MM.yyyy HH:mm") : "";
                returnValue.Status = dt.Rows[0]["StatusValue"] != DBNull.Value ? dt.Rows[0]["StatusValue"].ToString() : string.Empty;
                returnValue.StatusCode = new StringMap() { Name = dt.Rows[0]["StatusValue"].ToString(), Value = (int)dt.Rows[0]["StatusCode"] };
                returnValue.StateCode = new StringMap() { Name = dt.Rows[0]["StateValue"].ToString(), Value = (int)dt.Rows[0]["StateCode"] };
                returnValue.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString() };

                if (dt.Rows[0]["CustomerId"] != DBNull.Value)
                    returnValue.Contact = new EntityReference() { Name = dt.Rows[0]["CustomerIdName"].ToString(), Id = (Guid)dt.Rows[0]["CustomerId"], LogicalName = "contact" };

                if (dt.Rows[0]["ActivityStatusId"] != DBNull.Value)
                    returnValue.ActivityStatus = new EntityReference() { Name = dt.Rows[0]["ActivityStatusIdName"].ToString(), Id = (Guid)dt.Rows[0]["ActivityStatusId"], LogicalName = "new_activitystatusdetail" };

                if (dt.Rows[0]["OppCode"] != DBNull.Value)
                    returnValue.OpportunityCode = dt.Rows[0]["OppCode"].ToString();

                if (dt.Rows[0]["ContactType"] != DBNull.Value)
                {
                    returnValue.ContactTypeCode = (int)dt.Rows[0]["ContactType"];
                }

                if (dt.Rows[0]["CustomerIdType"] != DBNull.Value)
                {
                    returnValue.CustomerType = (int)dt.Rows[0]["CustomerIdType"];
                }

                MsCrmResultObject resultProdutcs = OpportunityHelper.GetOpportunityProducts(returnValue.OpportunityId, sda);

                if (resultProdutcs.Success)
                {
                    returnValue.OppProducts = (List<Product>)resultProdutcs.ReturnObject;
                }

                #endregion

            }

            return returnValue;
        }

        public static MsCrmResult CreateOrUpdateOpportunity(Opportunity _opportunity, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("opportunity");
                if (_opportunity.Owner != null)
                {
                    ent["ownerid"] = _opportunity.Owner;
                }
                if (_opportunity.Contact != null)
                {
                    ent["customerid"] = _opportunity.Contact;
                    ent["name"] = _opportunity.Contact.Name + " - " + DateTime.Now.ToShortDateString();
                }
                if (_opportunity.ActivityStatusDetail != null)
                {
                    ent["new_relatedactivitystatusid"] = _opportunity.ActivityStatusDetail;
                }


                if (_opportunity.OpportunityId == Guid.Empty)
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Fırsat başarıyla oluşturuldu.";
                }
                else
                {
                    service.Update(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Fırsat başarıyla güncellendi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        /// <summary>
        /// Aynı müşteri ve satış danışmanına ait açık fırsat kontrolünü yapar
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="ownerId"></param>
        /// <param name="sda"></param>
        /// <returns></returns>
        public static MsCrmResult HasOpenOpportunityToSalesConsultantAndContact(Guid contactId, Guid ownerId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                            opp.OpportunityId
                            FROM
	                            opportunity AS opp WITH (NOLOCK)
                            WHERE
	                            opp.customerId = '{0}'
                            AND
	                            opp.ownerId = '{1}'
                            AND
	                            opp.statecode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, contactId, ownerId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.CrmId = (Guid)dt.Rows[0]["OpportunityId"];
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aynı müşteri ve satış danışmanına ait açık fırsat bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;

        }

        /// <summary>
        /// Fırsat Ürünü Kontrolü
        /// </summary>
        /// <param name="opportunityId"></param>
        /// <param name="productId"></param>
        /// <param name="sda"></param>
        /// <returns></returns>
        public static MsCrmResult HasOpportunityProduct(Guid opportunityId, Guid productId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                            oppPro.opportunityproductId
                            FROM
	                            opportunityproduct AS oppPro WITH (NOLOCK)
                            WHERE
	                            oppPro.OpportunityId = '{0}'
                            AND
	                            oppPro.ProductId = '{1}'";
                #endregion
                DataTable dt = sda.getDataTable(string.Format(query, opportunityId, productId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Ürün, fırsat ürün olarak bulunmaktadır.";
                }
                else
                    returnValue.Success = false;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult CreateOpportunityProduct(Guid opportuntiyId, Product _proc, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity oppProEntity = new Entity("opportunityproduct");
                oppProEntity["opportunityid"] = new EntityReference("opportunity", opportuntiyId);
                oppProEntity["productid"] = new EntityReference("product", _proc.ProductId);
                oppProEntity["uomid"] = _proc.Uom;
                oppProEntity["quantity"] = new decimal(1);

                returnValue.CrmId = service.Create(oppProEntity);
                returnValue.Success = true;
                returnValue.Result = "Fırsat ürünü oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static void CloseOppAsWin(Guid oppId, int statusCode, DateTime actualEnd, IOrganizationService service)
        {
            try
            {
                if (oppId != Guid.Empty)
                {
                    WinOpportunityRequest req = new WinOpportunityRequest();
                    Entity opportunityClose = new Entity("opportunityclose");
                    opportunityClose["opportunityid"] = new EntityReference("opportunity", oppId);
                    opportunityClose["subject"] = "Win the Opportunity!";
                    opportunityClose["actualend"] = actualEnd;

                    req.OpportunityClose = opportunityClose;
                    req.Status = new OptionSetValue(statusCode);
                    WinOpportunityResponse resp = (WinOpportunityResponse)service.Execute(req);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MsCrmResult CloseOppAsLost(Guid oppId, int statusCode, DateTime actualEnd, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                if (oppId != Guid.Empty)
                {
                    LoseOpportunityRequest req = new LoseOpportunityRequest();
                    Entity opportunityClose = new Entity("opportunityclose");
                    opportunityClose["opportunityid"] = new EntityReference("opportunity", oppId);
                    opportunityClose["subject"] = "Lost the Opportunity!";
                    opportunityClose["actualend"] = actualEnd;

                    req.OpportunityClose = opportunityClose;
                    req.Status = new OptionSetValue(statusCode); // Durum açıklaması "Kaybedildi"
                    LoseOpportunityResponse resp = (LoseOpportunityResponse)service.Execute(req);
                    returnValue.Success = true;
                    returnValue.Result = "Fırsat kaybedildi olarak kapatıldı.";

                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı. Hata = " + ex.Message;
            }

            return returnValue;
        }

        internal static void OpportunityClosed(Guid QuoteId, IOrganizationService service)
        {
            Entity q = service.Retrieve("quote", QuoteId, new ColumnSet("customerid", "ownerid"));
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "customerid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(((EntityReference)q.Attributes["customerid"]).Id);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "ownerid";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(((EntityReference)q.Attributes["ownerid"]).Id);

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "statecode";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);


            QueryExpression Query = new QueryExpression("opportunity");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                Guid oppId = Result.Entities[0].Id;

                var OpportunityClose = new Entity("opportunityclose");
                OpportunityClose.Attributes.Add("opportunityid", Result.Entities[0].ToEntityReference());
                WinOpportunityRequest req = new WinOpportunityRequest()
                {
                    OpportunityClose = OpportunityClose,
                    Status = new OptionSetValue(3)
                };
                service.Execute(req);
            }

        }

        public static MsCrmResultObject GetUserAllIntrestedHouses(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |
                string query = @"
                                    SELECT
	                                    DISTINCT
	                                    A.ProductId
                                    FROM
                                    (
	                                    SELECT
		                                    opro.ProductId
	                                    FROM
	                                    Opportunity AS opp (NOLOCK)
		                                    JOIN
			                                    OpportunityProduct AS opro (NOLOCK)
				                                    ON
				                                    opp.OpportunityId=opro.OpportunityId
	                                    WHERE
		                                    opp.CustomerId=@customerId

	                                    UNION

	                                    SELECT
		                                    ip.new_productid AS ProductId
	                                    FROM
	                                    new_interestedproducts AS ip (NOLOCK)
		                                    JOIN
			                                    ActivityParty AS ap (NOLOCK)
				                                    ON
				                                    ap.ActivityId=ip.new_appointmentid
				                                    AND
				                                    ap.ParticipationTypeMask=5 --Required
	                                    WHERE
		                                    ap.PartyId=@customerId

	                                    UNION


	                                    SELECT
		                                    ip.new_productid AS ProductId
	                                    FROM
	                                    new_interestedproducts AS ip (NOLOCK)
		                                    JOIN
			                                    ActivityParty AS ap (NOLOCK)
				                                    ON
				                                    ap.ActivityId=ip.new_phonecallid
				                                    AND
				                                    ap.ParticipationTypeMask IN (1,2) --TO or FROM
	                                    WHERE
		                                    ap.PartyId=@customerId
                                    ) AS A";
                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@customerId", customerId) };

                DataTable dt = sda.getDataTable(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | FILL ENTITY INFO |

                    List<Product> returnList = new List<Product>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product pInfo = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);

                        if (pInfo != null && pInfo.ProductId != Guid.Empty)
                        {
                            returnList.Add(pInfo);
                        }
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static void ChecksRule(Entity entity, SqlDataAccess sda, IOrganizationService adminService)
        {
            if (!entity.Attributes.Contains("opportunityid"))
                return;
            Guid opportunityId = ((EntityReference)entity.Attributes["opportunityid"]).Id;
            Entity opportunity = adminService.Retrieve("opportunity", opportunityId, new ColumnSet(true));
            Guid customerId = opportunity.Contains("customerid") ? ((EntityReference)opportunity.Attributes["customerid"]).Id : Guid.Empty;
            Guid ownerId = opportunity.Contains("ownerid") ? ((EntityReference)opportunity.Attributes["ownerid"]).Id : Guid.Empty;
            if (customerId == Guid.Empty)
                throw new InvalidPluginExecutionException("Kapatmak istediğiniz fırsatın müşterisi bulunmamaktadır.");

            #region | Fırsat Kapatırken Müşterinin Acık Aktivitesi olmamalı |

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
                            CASE WHEN APP.new_activitytopicidName IS NOT NULL
                                THEN APP.new_activitytopicidName
                            	WHEN PC.new_activitytopicidName IS NOT NULL
                                THEN PC.new_activitytopicidName
                                ELSE PC.new_activitytopicidName
                            	END AS  [Subject]
                            FROM
                            ActivityPointer APO WITH (NOLOCK)
                            JOIN
                            ActivityParty AP WITH (NOLOCK)
                            ON
                            APO.ActivityId=AP.ActivityId
                            LEFT JOIN
                            PhoneCall PC WITH (NOLOCK)
                            ON
                            PC.ActivityId=AP.ActivityId
                            LEFT JOIN
                            Appointment APP WITH (NOLOCK)
                            ON
                            APP.ActivityId=AP.ActivityId
                            WHERE
                            APO.OwnerId=@OwnerId
                            AND
                            AP.PartyId=@CustomerId
                            AND
                            (
                            AP.ParticipationTypeMask=2--ToRecipient- PhoneCall
                            OR
                            AP.ParticipationTypeMask=5--RequiredAttendee- Appointment
                            )
                            AND
                            (
                            APO.StateCode=0--PhoneCall için Açık
                            OR
                            APO.StateCode=3--Appointment için Zamanlanmış açık
                            )";

            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@OwnerId", ownerId), new SqlParameter("@CustomerId", customerId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);
            bool control = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Subject"].ToString() == "Tahsilat Araması")
                    continue;
                else
                    control = true;

            }
            if (control)
                throw new InvalidPluginExecutionException("Kapatmak istediğiniz fırsatın açık aktivitesi bulunmaktadır.");

            //ConditionExpression con1 = new ConditionExpression();
            //con1.AttributeName = "regardingobjectid";
            //con1.Operator = ConditionOperator.Equal;
            //con1.Values.Add(opportunityId);

            //ConditionExpression con2 = new ConditionExpression();
            //con2.AttributeName = "statecode";
            //con2.Operator = ConditionOperator.Equal;
            //con2.Values.Add(0);

            //FilterExpression filter = new FilterExpression();
            //filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(con1);
            //filter.Conditions.Add(con2);

            //QueryExpression Query = new QueryExpression("activitypointer");
            //Query.ColumnSet = new ColumnSet(true);
            //Query.Criteria.FilterOperator = LogicalOperator.And;
            //Query.Criteria.Filters.Add(filter);
            //EntityCollection Result = adminService.RetrieveMultiple(Query);
            //if (Result.Entities.Count > 0)
            //{
            //    throw new InvalidPluginExecutionException("Kapatmak istediğiniz fırsatın açık aktivitesi bulunmaktadır.");
            //}
            #endregion

            #region | Son 3 gün içerisinde Fırsata ait tamamlanmış aktivite olmalı |

            #region | SQL QUERY |
            sqlQuery = @"SELECT
                            CASE 
                            WHEN APP.new_activitytopicidName IS NOT NULL
                               THEN APP.new_activitytopicidName
                            WHEN PC.new_activitytopicidName IS NOT NULL
                               THEN PC.new_activitytopicidName
                               ELSE PC.new_activitytopicidName
                            END AS  [Subject]
                            ,APO.ActualEnd
                            FROM
                            ActivityPointer APO WITH (NOLOCK)
                            JOIN
                            ActivityParty AP WITH (NOLOCK)
                            ON
                            APO.ActivityId=AP.ActivityId
                            LEFT JOIN
                            PhoneCall PC WITH (NOLOCK)
                            ON
                            PC.ActivityId=AP.ActivityId
                            LEFT JOIN
                            Appointment APP WITH (NOLOCK)
                            ON
                            APP.ActivityId=AP.ActivityId
                            WHERE
                            APO.OwnerId=@OwnerId
                            AND
                            AP.PartyId=@CustomerId
                            AND
                            (
                            AP.ParticipationTypeMask=2--ToRecipient- PhoneCall
                            OR
                            AP.ParticipationTypeMask=5--RequiredAttendee- Appointment
                            )
                            AND
                            (
                            APO.StateCode=1--PhoneCall ve Appointment için tamamlandı                           
                            )";

            #endregion

            parameters = new SqlParameter[] { new SqlParameter("@OwnerId", ownerId), new SqlParameter("@CustomerId", customerId) };

            dt = sda.getDataTable(sqlQuery, parameters);
            control = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["actualend"] != DBNull.Value && ((DateTime)dr["actualend"]).ToLocalTime().Date >= DateTime.Now.AddDays(-3).Date && dr["Subject"].ToString() != "Tahsilat Araması")
                    control = true;
            }
            if (!control)
                throw new InvalidPluginExecutionException("Son 3 gün içerisinde Fırsata ait tamamlanmış aktivite bulunmamaktadır.");


            //con1 = new ConditionExpression();
            //con1.AttributeName = "regardingobjectid";
            //con1.Operator = ConditionOperator.Equal;
            //con1.Values.Add(opportunityId);

            //con2 = new ConditionExpression();
            //con2.AttributeName = "statecode";
            //con2.Operator = ConditionOperator.Equal;
            //con2.Values.Add(1);

            //filter = new FilterExpression();
            //filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(con1);
            //filter.Conditions.Add(con2);

            //Query = new QueryExpression("activitypointer");
            //Query.ColumnSet = new ColumnSet("actualend");
            //Query.Criteria.FilterOperator = LogicalOperator.And;
            //Query.Criteria.Filters.Add(filter);
            //Result = adminService.RetrieveMultiple(Query);
            //if (Result.Entities.Count > 0)
            //{
            //    bool control = false;
            //    foreach (Entity item in Result.Entities)
            //    {
            //        if (item.Contains("actualend"))
            //        {
            //            if (((DateTime)item.Attributes["actualend"]).ToLocalTime().Date >= DateTime.Now.AddDays(-3).Date)
            //            {
            //                control = true;
            //            }
            //        }
            //    }
            //    if (!control)
            //    {
            //        throw new InvalidPluginExecutionException("Son 3 gün içerisinde Fırsata ait tamamlanmış aktivite bulunmamaktadır.");
            //    }
            //}

            #endregion

            #region | Fırsata ait kaporalı satış olmamalı |
            //ConditionExpression con1 = new ConditionExpression();
            //con1.AttributeName = "opportunityid";
            //con1.Operator = ConditionOperator.Equal;
            //con1.Values.Add(opportunityId);

            //FilterExpression filter = new FilterExpression();
            //filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(con1);

            //QueryExpression Query = new QueryExpression("opportunityproduct");
            //Query.ColumnSet = new ColumnSet("productid");
            //Query.Criteria.FilterOperator = LogicalOperator.And;
            //Query.Criteria.Filters.Add(filter);
            //EntityCollection Result = adminService.RetrieveMultiple(Query);
            //if (Result.Entities.Count > 0)
            //{
            //    control = false;
            //    foreach (Entity item in Result.Entities)
            //    {
            //        if (item.Contains("productid"))
            //        {
            //            Entity p = adminService.Retrieve("product", ((EntityReference)item.Attributes["productid"]).Id, new ColumnSet("statuscode"));
            //            if (p.Contains("statuscode"))
            //            {
            //                if ((ProductStatuses)((OptionSetValue)p.Attributes["statuscode"]).Value == ProductStatuses.OnSatisYapildi)
            //                {
            //                    control = true;
            //                }
            //            }
            //        }
            //    }
            //    if (control)
            //    {
            //        throw new InvalidPluginExecutionException("Fırsata ait kaporalı satış bulunmaktadır, bu nedenle kapatılamaz.");
            //    }
            //}

            #endregion

            #region | Müşteriye ait kaporalı satış olmamalı |
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "customerid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(customerId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quote");
            Query.ColumnSet = new ColumnSet("statuscode");//100.000.007
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = adminService.RetrieveMultiple(Query);

            if (Result.Entities.Count > 0)
            {
                control = false;
                foreach (Entity item in Result.Entities)
                {
                    if (item.Contains("statuscode"))
                    {
                        if ((QuoteStatus)((OptionSetValue)item.Attributes["statuscode"]).Value == QuoteStatus.KaporaAlındı)
                        {
                            control = true;
                        }
                    }
                }
                if (control)
                {
                    throw new InvalidPluginExecutionException("Müşteriye ait kaporalı satış bulunmaktadır, bu nedenle kapatılamaz.");
                }
            }

            #endregion
        }
    }
}
