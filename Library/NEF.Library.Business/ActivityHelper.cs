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
    public static class ActivityHelper
    {
        public static MsCrmResultObject GetCustomerActivities(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                *
                                FROM
                                (
	                                SELECT
	                                    PC.ActivityId
	                                    ,PC.PhoneNumber
	                                    ,PC.OwnerId
	                                    ,PC.OwnerIdName
	                                    ,PC.CreatedOn AS 'ScheduledStart'
										,PC.ModifiedOn AS 'ScheduledEnd'
	                                    ,PC.StateCode
	                                    ,'Telefon' ActivityType
                                        ,PC.new_retailerid RetailerId,
									     PC.new_retaileridName RetailerName
                                    FROM
	                                    PhoneCall PC WITH(NOLOCK)
                                    INNER JOIN
                                        ActivityParty AS AP WITH(NOLOCK)
                                        ON
                                        AP.ActivityId = PC.ActivityId
                                        AND
                                        AP.ParticipationTypeMask IN (1,2) -- FROM,TO
	                                    AND
	                                    AP.PartyId = '{0}'

                                    UNION

                                    SELECT
	                                    A.ActivityId
	                                    ,NULL PhoneNumber
	                                    ,A.OwnerId
	                                    ,A.OwnerIdName
	                                    ,A.CreatedOn AS 'ScheduledStart'
										,A.ModifiedOn AS 'ScheduledEnd'
	                                    ,A.StateCode
	                                    ,'Randevu' ActivityType
                                        ,A.new_retailerid RetailerId,
									     A.new_retaileridName RetailerName
                                    FROM
	                                    Appointment A WITH(NOLOCK)
                                    INNER JOIN
                                        ActivityParty AS AP WITH(NOLOCK)
                                        ON
                                        AP.ActivityId = A.ActivityId
                                        AND
                                        AP.ParticipationTypeMask = 5 -- TO
	                                    AND
	                                    AP.PartyId = '{0}'
                                )A
                                ORDER BY
                                A.ScheduledStart DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | MAKE SEARCH |
                    List<Activity> returnList = new List<Activity>();
                    //returnValue.ReturnObject = dt.ToList<Contact>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Activity _activity = new Activity();
                        _activity.ActivityId = (Guid)dt.Rows[i]["ActivityId"];
                        _activity.ActivityType = dt.Rows[i]["ActivityType"] != DBNull.Value ? dt.Rows[i]["ActivityType"].ToString() : string.Empty;
                        _activity.PhoneNumber = dt.Rows[i]["PhoneNumber"] != DBNull.Value ? dt.Rows[i]["PhoneNumber"].ToString() : string.Empty;
                        _activity.ScheduledStartString = dt.Rows[i]["ScheduledStart"] != DBNull.Value ? ((DateTime)dt.Rows[i]["ScheduledStart"]).ToString("dd.MM.yyyy") : "";
                        _activity.ScheduledEndString = dt.Rows[i]["ScheduledEnd"] != DBNull.Value ? ((DateTime)dt.Rows[i]["ScheduledEnd"]).ToString("dd.MM.yyyy") : "";
                        _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString() };
                        if (dt.Rows[i]["RetailerId"] != DBNull.Value)
                            _activity.Retailer = new EntityReference() { LogicalName = "new_retailer", Id = (Guid)dt.Rows[i]["RetailerId"], Name = dt.Rows[i]["RetailerName"].ToString() };
                        #region | FILL ENUMS |
                        if (dt.Rows[i]["StateCode"] != DBNull.Value)
                        {
                            _activity.StateCode = (ActivityStateCodes)dt.Rows[i]["StateCode"];
                        }
                        #endregion

                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait aktivite bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetCustomerLastActivity(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TOP 1
	                                *
                                FROM
	                                (
		                                SELECT
			                                PC.ActualEnd    
                                            ,PC.OwnerId   
	                                        ,PC.OwnerIdName
			                                ,PC.ActivityId
                                        FROM
	                                        PhoneCall PC WITH(NOLOCK)
                                        INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                            ON
                                            AP.ActivityId = PC.ActivityId
                                            AND
                                            AP.ParticipationTypeMask IN(1, 2) -- TO
	                                        AND
	                                        AP.PartyId = '{0}'
		                                WHERE
			                                PC.StateCode = 1

                                        UNION

                                        SELECT
	                                        A.ActualEnd 
                                            ,A.OwnerId   
	                                        ,A.OwnerIdName
			                                ,A.ActivityId
                                        FROM
	                                        Appointment A WITH(NOLOCK)
                                        INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                            ON
                                            AP.ActivityId = A.ActivityId
                                            AND
                                            AP.ParticipationTypeMask = 5 -- TO
	                                        AND
	                                        AP.PartyId = '{0}'
		                                WHERE
			                                A.StateCode = 2
	                                )A
                                ORDER BY
	                                A.ActualEnd DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY |

                    Activity _activity = new Activity();
                    _activity.ActivityId = (Guid)dt.Rows[0]["ActivityId"];
                    _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString() };

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _activity;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait aktivite bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetCustomerLastAppointment(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
                                        TOP 1
	                                        A.ActualEnd 
                                            ,A.OwnerId   
	                                        ,A.OwnerIdName
			                                ,A.ActivityId
                                            ,A.new_salesofficeid SalesOfficeId
                                            ,A.new_salesofficeidName SalesOfficeIdName
                                        FROM
	                                        Appointment A WITH(NOLOCK)
                                        INNER JOIN
                                            ActivityParty AS AP WITH(NOLOCK)
                                            ON
                                            AP.ActivityId = A.ActivityId
                                            AND
                                            AP.ParticipationTypeMask = 5 -- TO
	                                        AND
	                                        AP.PartyId = '{0}'
		                                WHERE
			                                A.StateCode = 1
                                        AND
                                            A.new_salesofficeid IS NOT NULL
                                        ORDER BY
                                            A.ActualEnd DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY |

                    Activity _activity = new Activity();
                    _activity.ActivityId = (Guid)dt.Rows[0]["ActivityId"];
                    _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString() };
                    _activity.SalesOffice = new EntityReference() { LogicalName = "new_salesoffice", Id = (Guid)dt.Rows[0]["SalesOfficeId"], Name = dt.Rows[0]["SalesOfficeIdName"].ToString() };


                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _activity;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait aktivite bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreatePhoneCall(Activity _activity, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("phonecall");
                ent["subject"] = DateTime.Now.ToShortDateString();
                ent["ownerid"] = _activity.Owner;
                ent["scheduledstart"] = DateTime.Now;
                ent["scheduledend"] = DateTime.Now.AddDays(1);
                ent["directioncode"] = _activity.Direction == Directions.Giden ? true : false;

                if (_activity.Retailer != null && _activity.Retailer.Id != Guid.Empty)
                {
                    ent["new_retailerid"] = new EntityReference("new_retailer", _activity.Retailer.Id);
                }

                Entity toParty = new Entity("activityparty");
                toParty["partyid"] = _activity.Direction == Directions.Giden ? _activity.ActivityParty : _activity.Owner;
                ent["to"] = new Entity[] { toParty };

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = _activity.Direction == Directions.Giden ? _activity.Owner : _activity.ActivityParty;
                ent["from"] = new Entity[] { fromParty };

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Telefon görüşmesi başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult UpdatePhoneCall(Activity _activity, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("phonecall");
                ent["activityid"] = _activity.ActivityId;
                if (_activity.Subject != null && _activity.Subject != -1)
                {
                    ent["new_subject"] = new OptionSetValue((int)_activity.Subject);
                }

                if (_activity.ActivityStatus != null && _activity.ActivityStatus.Id != Guid.Empty)
                {
                    ent["new_activitystatus"] = new EntityReference("new_activitystatus", _activity.ActivityStatus.Id);
                }

                if (_activity.ActivityStatusDetail != null && _activity.ActivityStatusDetail.Id != Guid.Empty)
                {
                    ent["new_activitystatusdetail"] = new EntityReference("new_activitystatusdetail", _activity.ActivityStatusDetail.Id);
                }

                if (_activity.ActivityTopic != null && _activity.ActivityTopic.Id != Guid.Empty)
                {
                    ent["new_activitytopicid"] = new EntityReference("new_activitytopic", _activity.ActivityTopic.Id);
                }

                if (_activity.NextCallDate != null)
                {
                    ent["new_nextcalldate"] = _activity.NextCallDate;
                }

                if (_activity.NextAppointmentDate != null)
                {
                    ent["new_nextappointmentdate"] = _activity.NextAppointmentDate;
                }

                if (_activity.NextPaymentDate != null)
                {
                    ent["new_nextpaymentdate"] = _activity.NextPaymentDate;
                }

                if (_activity.CallCenterAgent != null && _activity.CallCenterAgent != -1)
                {
                    ent["new_callcenteragent"] = new OptionSetValue((int)_activity.CallCenterAgent);
                }

                if (_activity.Retailer != null && _activity.Retailer.Id != Guid.Empty)
                {
                    ent["new_retailerid"] = new EntityReference("new_retailer", _activity.Retailer.Id);
                }

                if (_activity.Direction != null)
                {
                    ent["directioncode"] = _activity.Direction == Directions.Giden ? true : false;

                    Entity toParty = new Entity("activityparty");
                    toParty["partyid"] = _activity.Direction == Directions.Giden ? _activity.ActivityParty : _activity.Owner;
                    ent["to"] = new Entity[] { toParty };

                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = _activity.Direction == Directions.Giden ? _activity.Owner : _activity.ActivityParty;
                    ent["from"] = new Entity[] { fromParty };
                }

                if (!string.IsNullOrEmpty(_activity.Note))
                {
                    ent["description"] = _activity.Note;
                }
                service.Update(ent);

                CloseActivity(ObjectTypeCodes.Telefon, 1, 2, _activity.ActivityId, service);

                returnValue.Success = true;
                returnValue.Result = "Telefon görüşmesi başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult UpdateAppointment(Activity _activity, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("appointment");
                ent["activityid"] = _activity.ActivityId;

                if (_activity.PresentationType != null && _activity.PresentationType != -1)
                {
                    ent["new_presentationtype"] = new OptionSetValue((int)_activity.PresentationType);
                }

                if (_activity.ActivityStatus != null && _activity.ActivityStatus.Id != Guid.Empty)
                {
                    ent["new_activitystatus"] = new EntityReference("new_activitystatus", _activity.ActivityStatus.Id);
                }

                if (_activity.ActivityStatusDetail != null && _activity.ActivityStatusDetail.Id != Guid.Empty)
                {
                    ent["new_activitystatusdetail"] = new EntityReference("new_activitystatusdetail", _activity.ActivityStatusDetail.Id);
                }

                if (_activity.ActivityTopic != null && _activity.ActivityTopic.Id != Guid.Empty)
                {
                    ent["new_activitytopicid"] = new EntityReference("new_activitytopic", _activity.ActivityTopic.Id);
                }

                if (_activity.SalesOffice != null && _activity.SalesOffice.Id != Guid.Empty)
                {
                    ent["new_salesofficeid"] = new EntityReference("new_salesoffice", _activity.SalesOffice.Id);
                }

                if (_activity.Subject != null && _activity.Subject != -1)
                {
                    ent["new_subject"] = new OptionSetValue((int)_activity.Subject);
                }
                else if (_activity.Subject == -1)
                {
                    ent["new_subject"] = null;
                }

                if (!string.IsNullOrEmpty(_activity.Note))
                {
                    ent["description"] = _activity.Note;
                }
                if (_activity.Retailer != null && _activity.Retailer.Id != Guid.Empty)
                {
                    ent["new_retailerid"] = new EntityReference("new_retailer", _activity.Retailer.Id);
                }
                service.Update(ent);

                CloseActivity(ObjectTypeCodes.Randevu, 1, 3, _activity.ActivityId, service);

                returnValue.Success = true;
                returnValue.Result = "Randevu başarıyla güncelledi.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult CreateAppointment(Activity _activity, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("appointment");
                ent["subject"] = DateTime.Now.ToShortDateString();
                ent["ownerid"] = _activity.Owner;
                ent["scheduledstart"] = DateTime.Now;
                ent["scheduledend"] = DateTime.Now.AddDays(1);
                if (_activity.Retailer != null && _activity.Retailer.Id != Guid.Empty)
                {
                    ent["new_retailer"] = new EntityReference("new_retailer", _activity.Retailer.Id);
                }

                Entity toParty = new Entity("activityparty");
                toParty["partyid"] = _activity.ActivityParty;
                ent["requiredattendees"] = new Entity[] { toParty };

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Randevu başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetActivityInfo(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                PC.ActivityId
	                                ,PC.OwnerId
	                                ,PC.OwnerIdName
	                                ,PC.new_activitystatus ActivityStatusId
	                                ,PC.new_activitystatusName ActivityStatusIdName
	                                ,PC.new_activitystatusdetail ActivityStatusDetailId
	                                ,PC.new_activitystatusdetailName ActivityStatusDetailIdName
									,PC.ActivityId SalesOfficeId
                                    ,PC.new_activitytopicid TopicId
                                    --,PC.new_activitytopicidName TopicIdName
	                                ,'' SalesOfficeIdName
									,PC.Description
                                    ,PC.new_retailerid RetailerId,
									PC.new_retaileridName RetailerName
                                   -- ,PC.new_callcenteragent CallCenterAgent
	                                ,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4210
											AND
											SM.AttributeName = 'new_subject'
											AND
											SM.AttributeValue = PC.new_subject
									) Subject
									,'' PresentationType
									,'Telefon' ActivityType
                                FROM
	                                PhoneCall PC WITH (NOLOCK)
								WHERE
									PC.ActivityId = '{0}'

								UNION

								SELECT
	                                AP.ActivityId
	                                ,AP.OwnerId
	                                ,AP.OwnerIdName
	                                ,AP.new_activitystatus ActivityStatusId
	                                ,AP.new_activitystatusName ActivityStatusIdName
	                                ,AP.new_activitystatusdetail ActivityStatusDetailId
	                                ,AP.new_activitystatusdetailName ActivityStatusDetailIdName
                                    ,AP.new_activitytopicid TopicId
                                    --,AP.new_activitytopicidName TopicIdName
									,AP.new_salesofficeid SalesOfficeId
									,AP.new_salesofficeidName SalesOfficeIdName
									,AP.Description
                                    ,AP.new_retailerid RetailerId,
									AP.new_retaileridName RetailerName
                                    --,'' CallCenterAgent
	                                ,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4201
											AND
											SM.AttributeName = 'new_subject'
											AND
											SM.AttributeValue = AP.new_subject
									) Subject
									,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4201
											AND
											SM.AttributeName = 'new_presentationtype'
											AND
											SM.AttributeValue = AP.new_presentationtype
									) PresentationType
									,'Randevu' ActivityType
                                FROM
	                                Appointment AP WITH (NOLOCK)
								WHERE
									AP.ActivityId = '{0}'

";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY |

                    Activity _activity = new Activity();
                    _activity.ActivityId = (Guid)dt.Rows[0]["ActivityId"];
                    _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString() };

                    if (dt.Rows[0]["ActivityStatusId"] != DBNull.Value)
                        _activity.ActivityStatus = new EntityReference() { LogicalName = "new_activitystatus", Id = (Guid)dt.Rows[0]["ActivityStatusId"], Name = dt.Rows[0]["ActivityStatusIdName"].ToString() };

                    if (dt.Rows[0]["ActivityStatusDetailId"] != DBNull.Value)
                        _activity.ActivityStatusDetail = new EntityReference() { LogicalName = "new_activitystatusdetail", Id = (Guid)dt.Rows[0]["ActivityStatusDetailId"], Name = dt.Rows[0]["ActivityStatusDetailIdName"].ToString() };

                    if (dt.Rows[0]["SalesOfficeId"] != DBNull.Value)
                        _activity.SalesOffice = new EntityReference() { LogicalName = "new_salesoffice", Id = (Guid)dt.Rows[0]["SalesOfficeId"], Name = dt.Rows[0]["SalesOfficeIdName"].ToString() };

                    if (dt.Rows[0]["TopicId"] != DBNull.Value)
                        _activity.ActivityTopic = new EntityReference() { LogicalName = "new_activitytopic", Id = (Guid)dt.Rows[0]["TopicId"], Name = "" };

                    if (dt.Rows[0]["RetailerId"] != DBNull.Value)
                        _activity.Retailer = new EntityReference() { LogicalName = "new_retailer", Id = (Guid)dt.Rows[0]["RetailerId"], Name = dt.Rows[0]["RetailerName"].ToString() };

                    _activity.Note = dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : "";
                    _activity.SubjectString = dt.Rows[0]["Subject"] != DBNull.Value ? dt.Rows[0]["Subject"].ToString() : "";
                    _activity.PresentationTypeString = dt.Rows[0]["PresentationType"] != DBNull.Value ? dt.Rows[0]["PresentationType"].ToString() : "";
                    _activity.ActivityType = dt.Rows[0]["ActivityType"] != DBNull.Value ? dt.Rows[0]["ActivityType"].ToString() : "";


                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _activity;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait aktivite bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetPhoneCallActivityInfo(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                PC.ActivityId
	                                ,PC.OwnerId
	                                ,PC.OwnerIdName
	                                ,PC.new_activitystatus ActivityStatusId
	                                ,PC.new_activitystatusName ActivityStatusIdName
	                                ,PC.new_activitystatusdetail ActivityStatusDetailId
	                                ,PC.new_activitystatusdetailName ActivityStatusDetailIdName
									,PC.ActivityId SalesOfficeId
                                    ,PC.new_activitytopicid TopicId
                                    ,PC.new_activitytopicidName TopicIdName
	                                ,'' SalesOfficeIdName
									,PC.Description
                                    ,PC.new_retailerid RetailerId,
									PC.new_retaileridName RetailerName
                                   -- ,PC.new_callcenteragent CallCenterAgent
	                                ,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4210
											AND
											SM.AttributeName = 'new_subject'
											AND
											SM.AttributeValue = PC.new_subject
									) Subject
									,'' PresentationType
									,'Telefon' ActivityType
                                FROM
	                                PhoneCall PC WITH (NOLOCK)
								WHERE
									PC.ActivityId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY |

                    Activity _activity = new Activity();
                    _activity.ActivityId = (Guid)dt.Rows[0]["ActivityId"];
                    _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString() };

                    if (dt.Rows[0]["ActivityStatusId"] != DBNull.Value)
                        _activity.ActivityStatus = new EntityReference() { LogicalName = "new_activitystatus", Id = (Guid)dt.Rows[0]["ActivityStatusId"], Name = dt.Rows[0]["ActivityStatusIdName"].ToString() };

                    if (dt.Rows[0]["ActivityStatusDetailId"] != DBNull.Value)
                        _activity.ActivityStatusDetail = new EntityReference() { LogicalName = "new_activitystatusdetail", Id = (Guid)dt.Rows[0]["ActivityStatusDetailId"], Name = dt.Rows[0]["ActivityStatusDetailIdName"].ToString() };

                    if (dt.Rows[0]["SalesOfficeId"] != DBNull.Value)
                        _activity.SalesOffice = new EntityReference() { LogicalName = "new_salesoffice", Id = (Guid)dt.Rows[0]["SalesOfficeId"], Name = dt.Rows[0]["SalesOfficeIdName"].ToString() };

                    if (dt.Rows[0]["TopicId"] != DBNull.Value)
                        _activity.ActivityTopic = new EntityReference() { LogicalName = "new_activitytopic", Id = (Guid)dt.Rows[0]["TopicId"], Name = dt.Rows[0]["TopicIdName"].ToString() };


                    if (dt.Rows[0]["RetailerId"] != DBNull.Value)
                        _activity.Retailer = new EntityReference() { LogicalName = "new_retailer", Id = (Guid)dt.Rows[0]["RetailerId"], Name = dt.Rows[0]["RetailerName"].ToString() };

                    _activity.Note = dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : "";
                    _activity.SubjectString = dt.Rows[0]["Subject"] != DBNull.Value ? dt.Rows[0]["Subject"].ToString() : "";
                    _activity.PresentationTypeString = dt.Rows[0]["PresentationType"] != DBNull.Value ? dt.Rows[0]["PresentationType"].ToString() : "";
                    _activity.ActivityType = dt.Rows[0]["ActivityType"] != DBNull.Value ? dt.Rows[0]["ActivityType"].ToString() : "";


                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _activity;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait aktivite bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetAppointmentActivityInfo(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                AP.ActivityId
	                                ,AP.OwnerId
	                                ,AP.OwnerIdName
	                                ,AP.new_activitystatus ActivityStatusId
	                                ,AP.new_activitystatusName ActivityStatusIdName
	                                ,AP.new_activitystatusdetail ActivityStatusDetailId
	                                ,AP.new_activitystatusdetailName ActivityStatusDetailIdName
                                    ,AP.new_activitytopicid TopicId
                                    ,AP.new_activitytopicidName TopicIdName
									,AP.new_salesofficeid SalesOfficeId
									,AP.new_salesofficeidName SalesOfficeIdName
									,AP.Description
                                    ,AP.new_retailerid RetailerId,
									AP.new_retaileridName RetailerName
                                    --,'' CallCenterAgent
	                                ,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4201
											AND
											SM.AttributeName = 'new_subject'
											AND
											SM.AttributeValue = AP.new_subject
									) Subject
									,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4201
											AND
											SM.AttributeName = 'new_presentationtype'
											AND
											SM.AttributeValue = AP.new_presentationtype
									) PresentationType
									,'Randevu' ActivityType
                                FROM
	                                Appointment AP WITH (NOLOCK)
								WHERE
									AP.ActivityId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY |

                    Activity _activity = new Activity();
                    _activity.ActivityId = (Guid)dt.Rows[0]["ActivityId"];
                    _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString() };

                    if (dt.Rows[0]["ActivityStatusId"] != DBNull.Value)
                        _activity.ActivityStatus = new EntityReference() { LogicalName = "new_activitystatus", Id = (Guid)dt.Rows[0]["ActivityStatusId"], Name = dt.Rows[0]["ActivityStatusIdName"].ToString() };

                    if (dt.Rows[0]["ActivityStatusDetailId"] != DBNull.Value)
                        _activity.ActivityStatusDetail = new EntityReference() { LogicalName = "new_activitystatusdetail", Id = (Guid)dt.Rows[0]["ActivityStatusDetailId"], Name = dt.Rows[0]["ActivityStatusDetailIdName"].ToString() };

                    if (dt.Rows[0]["SalesOfficeId"] != DBNull.Value)
                        _activity.SalesOffice = new EntityReference() { LogicalName = "new_salesoffice", Id = (Guid)dt.Rows[0]["SalesOfficeId"], Name = dt.Rows[0]["SalesOfficeIdName"].ToString() };

                    if (dt.Rows[0]["TopicId"] != DBNull.Value)
                        _activity.ActivityTopic = new EntityReference() { LogicalName = "new_activitytopic", Id = (Guid)dt.Rows[0]["TopicId"], Name = dt.Rows[0]["TopicIdName"].ToString() };

                    _activity.Note = dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : "";
                    _activity.SubjectString = dt.Rows[0]["Subject"] != DBNull.Value ? dt.Rows[0]["Subject"].ToString() : "";
                    _activity.PresentationTypeString = dt.Rows[0]["PresentationType"] != DBNull.Value ? dt.Rows[0]["PresentationType"].ToString() : "";
                    _activity.ActivityType = dt.Rows[0]["ActivityType"] != DBNull.Value ? dt.Rows[0]["ActivityType"].ToString() : "";

                    if (dt.Rows[0]["RetailerId"] != DBNull.Value)
                        _activity.Retailer = new EntityReference() { LogicalName = "new_retailer", Id = (Guid)dt.Rows[0]["RetailerId"], Name = dt.Rows[0]["RetailerName"].ToString() };

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _activity;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait aktivite bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivityStatuses(Guid subjectId, string userTypeCode, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            UserTypes userType = UserTypes.Bos;

            string addSqlQuery = string.Empty;

            if (!string.IsNullOrEmpty(userTypeCode))
            {
                userType = (UserTypes)Convert.ToInt32(userTypeCode);

                if (userType == UserTypes.CallCenter)
                {
                    addSqlQuery = " AND A.new_isreceptionist=1 ";
                }
                else if (userType == UserTypes.MusteriIliskileri)
                {
                    addSqlQuery = " AND A.new_iscustomerrelationship=1 ";
                }
                else
                {
                    addSqlQuery = " AND A.new_issalesperson=1 ";
                }
            }

            try
            {
                #region | SQL QUERY |
                string query = @"

                                SELECT
	                                A.new_activitystatusId Id
	                                ,A.new_name Name
	                                ,A.new_id Code
                                    ,A.new_requiredforhome Required
                                FROM
	                                new_new_activitystatus_new_activitytopic AT WITH (NOLOCK)
                                INNER JOIN
	                                new_activitystatus A WITh (NOLOCK)
	                                ON
	                                AT.new_activitytopicid = '{0}'
	                                AND
	                                A.new_activitystatusId = AT.new_activitystatusid	
                                    AND
	                                A.StateCode = 0" + addSqlQuery;
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, subjectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY STATUSES |
                    List<ActivityStatus> returnList = new List<ActivityStatus>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ActivityStatus _activity = new ActivityStatus();
                        _activity.ActivityStatusId = (Guid)dt.Rows[i]["Id"];
                        _activity.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;
                        _activity.ActivityStatusCode = dt.Rows[i]["Code"] != DBNull.Value ? (ActivityStatusCodes)Convert.ToInt32(dt.Rows[i]["Code"]) : ActivityStatusCodes.Bos;
                        _activity.Required = dt.Rows[i]["Required"] != DBNull.Value ? dt.Rows[i]["Required"].ToString() : string.Empty;
                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Sistemde etkin görüşme sonucu bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivityStatusDetail(Guid activityStatusId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                A.new_activitystatusId Id
	                                ,A.new_id Code
	                                ,A.new_name Name
                                FROM
	                                new_activitystatus A WITH (NOLOCK)
                                WHERE
	                                A.new_activitystatusId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityStatusId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY STATUS DETAIL |
                    ActivityStatus status = new ActivityStatus();
                    status.ActivityStatusId = (Guid)dt.Rows[0]["Id"];
                    status.Name = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;
                    status.ActivityStatusCode = dt.Rows[0]["Code"] != DBNull.Value ? (ActivityStatusCodes)Convert.ToInt32(dt.Rows[0]["Code"]) : ActivityStatusCodes.Bos;
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = status;
                }
                else
                {
                    returnValue.Result = "Beklenmedik hata ile karşılaşıldı!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivityStatusDetails(Guid activityStatusId, string userTypeCode, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            UserTypes userType = UserTypes.Bos;

            string addSqlQuery = string.Empty;

            if (!string.IsNullOrEmpty(userTypeCode))
            {
                userType = (UserTypes)Convert.ToInt32(userTypeCode);

                if (userType == UserTypes.CallCenter)
                {
                    addSqlQuery = " AND A.new_isreceptionist=1 ";
                }
                else if (userType == UserTypes.MusteriIliskileri)
                {
                    addSqlQuery = " AND A.new_iscustomerrelationship=1 ";
                }
                else
                {
                    addSqlQuery = " AND A.new_issalesperson=1 ";
                }
            }

            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                A.new_activitystatusdetailId Id
	                                ,A.new_name Name
                                    ,A.new_id AS Code
                                FROM
	                                new_activitystatusdetail A WITh (NOLOCK)
                                WHERE
	                                A.StateCode = 0
	                                AND
	                                A.new_activitystatusid = '{0}'" + addSqlQuery;
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityStatusId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY STATUS DETAILS |
                    List<ActivityStatusDetail> returnList = new List<ActivityStatusDetail>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ActivityStatusDetail _activity = new ActivityStatusDetail();
                        _activity.ActivityStatusDetailId = (Guid)dt.Rows[i]["Id"];
                        _activity.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        if (dt.Rows[i]["Code"] != DBNull.Value)
                        {
                            _activity.Code = dt.Rows[i]["Code"].ToString();
                        }

                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Görüşme sonucuna ait ayrıntı bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetSalesOffices(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                SO.new_salesofficeId Id
	                                ,SO.new_name Name
                                    ,SO.new_ip Ip
                                FROM
	                                new_salesoffice SO WITH (NOLOCK)
                                WHERE
	                                SO.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET SALES OFFICES |
                    List<SalesOffice> returnList = new List<SalesOffice>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SalesOffice _office = new SalesOffice();
                        _office.SalesOfficeId = (Guid)dt.Rows[i]["Id"];
                        _office.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;
                        _office.IpAddress = dt.Rows[i]["Ip"] != DBNull.Value ? dt.Rows[i]["Ip"].ToString() : string.Empty;

                        returnList.Add(_office);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Sistemde etkin satış ofisi bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetUserActivities(Guid systemUserId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
                                *
                                FROM
                                (
                                SELECT
	                                PC.ActivityId
	                                ,PC.PhoneNumber
	                                ,PC.OwnerId
	                                ,PC.OwnerIdName
	                                ,PC.ScheduledStart
	                                ,PC.StateCode
	                                ,'Telefon' ActivityType
	                                ,AP.PartyId AS ContactId
	                                ,AP.PartyIdName AS ContactIdName
                                    ,AP.PartyObjectTypeCode EntityType
                                    ,PC.CreatedBy
                                    ,PC.CreatedByName
                                    ,PC.CreatedOn
                                FROM
	                                PhoneCall PC WITH(NOLOCK)
                                INNER JOIN
                                    ActivityParty AS AP WITH(NOLOCK)
                                    ON
                                    AP.ActivityId = PC.ActivityId
                                    AND
                                    AP.ParticipationTypeMask = 2 -- TO
	                                AND
	                                AP.PartyObjectTypeCode IN(1,2) -- Account,Contact
	                                AND
	                                PC.OwnerId=@userId

                                UNION

                                SELECT
	                                A.ActivityId
	                                ,NULL PhoneNumber
	                                ,A.OwnerId
	                                ,A.OwnerIdName
	                                ,A.ScheduledStart
	                                ,A.StateCode
	                                ,'Randevu' ActivityType
	                                ,AP.PartyId AS ContactId
	                                ,AP.PartyIdName AS ContactIdName
                                    ,AP.PartyObjectTypeCode EntityType
                                    ,A.CreatedBy
                                    ,A.CreatedByName
                                     ,A.CreatedOn
                                FROM
	                                Appointment A WITH(NOLOCK)
                                INNER JOIN
                                    ActivityParty AS AP WITH(NOLOCK)
                                    ON
                                    AP.ActivityId = A.ActivityId
                                    AND
                                    AP.ParticipationTypeMask = 5 -- TO
	                                AND
	                                AP.PartyObjectTypeCode IN(1,2) -- Account,Contact
	                                AND
	                                A.OwnerId=@userId
                                ) AS A ORDER BY A.CreatedOn DESC";
                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@userId", systemUserId) };

                DataTable dt = sda.getDataTable(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | FILL ACTIVITY LIST |
                    List<Activity> returnList = new List<Activity>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Activity _activity = new Activity();
                        _activity.ActivityId = (Guid)dt.Rows[i]["ActivityId"];
                        _activity.ActivityType = dt.Rows[i]["ActivityType"] != DBNull.Value ? dt.Rows[i]["ActivityType"].ToString() : string.Empty;
                        _activity.PhoneNumber = dt.Rows[i]["PhoneNumber"] != DBNull.Value ? dt.Rows[i]["PhoneNumber"].ToString() : string.Empty;
                        _activity.ScheduledStartString = dt.Rows[i]["ScheduledStart"] != DBNull.Value ? ((DateTime)dt.Rows[i]["ScheduledStart"]).ToString("dd.MM.yyyy HH:mm") : "";
                        _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString() };
                        _activity.EntityType = dt.Rows[i]["EntityType"] != DBNull.Value ? dt.Rows[i]["EntityType"].ToString() : string.Empty;
                        _activity.CreatedBy = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["CreatedBy"], Name = dt.Rows[i]["CreatedByName"].ToString() };
                        _activity.CreatedOnString = dt.Rows[i]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[i]["CreatedOn"]).ToString("dd.MM.yyyy HH:mm") : "";

                        if (dt.Rows[i]["ContactId"] != DBNull.Value)
                        {
                            _activity.Contact = new EntityReference()
                            {
                                Id = (Guid)dt.Rows[i]["ContactId"],
                                Name = dt.Rows[i]["ContactIdName"].ToString(),
                                LogicalName = "contact"
                            };
                        }

                        #region | FILL ENUMS |
                        if (dt.Rows[i]["StateCode"] != DBNull.Value)
                        {
                            _activity.StateCode = (ActivityStateCodes)dt.Rows[i]["StateCode"];
                        }
                        #endregion

                        returnList.Add(_activity);
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

        public static MsCrmResult CloseActivity(ObjectTypeCodes code, int stateCode, int statusCode, Guid activityId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                SetStateRequest setStateReq = new SetStateRequest();
                if (code == ObjectTypeCodes.Randevu)
                {
                    setStateReq.EntityMoniker = new EntityReference("appointment", activityId);
                }
                else
                {
                    setStateReq.EntityMoniker = new EntityReference("phonecall", activityId);
                }

                setStateReq.State = new OptionSetValue(stateCode);
                setStateReq.Status = new OptionSetValue(statusCode);

                SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);

                returnValue.Success = true;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult AddPhoneCallToQueue(Guid phoneCallId, Guid queueId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                AddToQueueRequest routeRequest = new AddToQueueRequest
                {
                    Target = new EntityReference("phonecall", phoneCallId),
                    DestinationQueueId = queueId
                };
                service.Execute(routeRequest);

                returnValue.Success = true;
                returnValue.Result = "Telefon görüşmesi başarıyla kuyruğa eklendi.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetPhonecallsByQueue(QueueTypes queueType, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT 
	                                PC.ActivityId
	                                ,PC.DirectionCode
	                                ,PC.PhoneNumber
                                    ,PC.OwnerId
                                    ,PC.OwnerIdName
	                                ,AP.PartyId ToId
	                                ,AP.PartyIdName ToIdName
	                                ,AP2.PartyId FromId
	                                ,AP2.PartyIdName FromIdName
	                                ,SM.AttributeValue PriorityValue
	                                ,SM.Value PriorityName
                                    ,PC.CreatedOn
                                    ,(SELECT
											new_projectidName
										FROM
											new_webform with(nolock)
										WHERE 
											new_webformId = PC.RegardingObjectId) AS ProjectName
                                    ,(SELECT
											new_message
										FROM
											new_webform with(nolock)
										WHERE 
											new_webformId = PC.RegardingObjectId) AS ContactMessage
                                    ,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4210
											AND
											SM.AttributeName = 'new_subject'
											AND
											SM.AttributeValue = PC.new_subject
									) Subject
                                FROM
	                                Queue Q WITH (NOLOCK)
                                INNER JOIN
	                                QueueItem QI WITH (NOLOCK)
	                                ON
	                                Q.new_queuetype = {0}
	                                AND
	                                QI.QueueId = Q.QueueId
                                INNER JOIN
	                                PhoneCall PC WITH (NOLOCK)
	                                ON
	                                PC.ActivityId = QI.ObjectId
	                                AND
	                                PC.StateCode = 0
                                INNER JOIN
	                                ActivityParty AS AP
	                                ON
	                                AP.ActivityId = PC.ActivityId 
	                                AND
	                                AP.ParticipationTypeMask=2 -- To
                                INNER JOIN
	                                ActivityParty AS AP2
	                                ON
	                                AP2.ActivityId = PC.ActivityId 
	                                AND
	                                AP2.ParticipationTypeMask=1 -- From
                                INNER JOIN
	                                StringMap SM WITH (NOLOCK)
	                                ON
	                                SM.ObjectTypeCode = 4210
	                                AND
	                                SM.AttributeName = 'prioritycode'
	                                AND
	                                SM.AttributeValue = PC.PriorityCode
                                ORDER BY
                                    PC.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)queueType));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET QUEUE'S PHONE CALL |
                    List<Activity> returnList = new List<Activity>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Activity _activity = new Activity();
                        _activity.ActivityId = (Guid)dt.Rows[i]["ActivityId"];
                        _activity.Direction = dt.Rows[i]["DirectionCode"] != DBNull.Value ? (bool)dt.Rows[i]["DirectionCode"] ? Directions.Giden : Directions.Gelen : Directions.Giden;
                        _activity.PhoneNumber = dt.Rows[i]["PhoneNumber"] != DBNull.Value ? dt.Rows[i]["PhoneNumber"].ToString() : string.Empty;
                        _activity.CreatedOnString = dt.Rows[i]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[i]["CreatedOn"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                        _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString() };
                        _activity.Priority = (PriorityValues)(int)dt.Rows[i]["PriorityValue"];
                        _activity.PriorityString = dt.Rows[i]["PriorityName"].ToString();
                        _activity.ActivityStatusCode = ActivityStatusCodes.Bos;
                        _activity.ProjectName = dt.Rows[i]["ProjectName"] != DBNull.Value ? dt.Rows[i]["ProjectName"].ToString() : string.Empty;
                        _activity.ContactMessage = dt.Rows[i]["ContactMessage"] != DBNull.Value ? dt.Rows[i]["ContactMessage"].ToString() : string.Empty;

                        if (_activity.Direction == Directions.Giden)
                            _activity.Contact = new EntityReference() { LogicalName = "contact", Id = (Guid)dt.Rows[i]["ToId"], Name = dt.Rows[i]["ToIdName"].ToString() };
                        else
                            _activity.Contact = new EntityReference() { LogicalName = "contact", Id = (Guid)dt.Rows[i]["FromId"], Name = dt.Rows[i]["FromIdName"].ToString() };

                        if (dt.Rows[i]["FromId"] != DBNull.Value)
                        {
                            if (_activity.Direction == Directions.Giden)
                                _activity.ActivityParty = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["FromId"], Name = dt.Rows[i]["FromIdName"].ToString() };
                            else
                                _activity.ActivityParty = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["ToId"], Name = dt.Rows[i]["ToIdName"].ToString() };
                        }

                        _activity.SubjectString = dt.Rows[i]["Subject"] != DBNull.Value ? dt.Rows[i]["Subject"].ToString() : "";

                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Sistemde etkin telefon görüşmesi bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetPhonecallsByQueueId(Guid queueId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                PC.ActivityId
	                                ,PC.DirectionCode
	                                ,PC.PhoneNumber
                                    ,PC.OwnerId
                                    ,PC.OwnerIdName
	                                ,AP.PartyId ToId
	                                ,AP.PartyIdName ToIdName
	                                ,AP2.PartyId FromId
	                                ,AP2.PartyIdName FromIdName
	                                ,SM.AttributeValue PriorityValue
	                                ,SM.Value PriorityName
                                    ,PC.CreatedOn
                                    ,(SELECT
											new_projectidName
										FROM
											new_webform with(nolock)
										WHERE 
											new_webformId = PC.RegardingObjectId) AS ProjectName
                                    ,(SELECT
											new_message
										FROM
											new_webform with(nolock)
										WHERE 
											new_webformId = PC.RegardingObjectId) AS ContactMessage
                                    ,(
										SELECT
											TOP 1
											SM.Value
										FROM
											StringMap SM WITH (NOLOCK)
										WHERE
											SM.ObjectTypeCode = 4210
											AND
											SM.AttributeName = 'new_subject'
											AND
											SM.AttributeValue = PC.new_subject
									) Subject
                                FROM
	                                Queue Q WITH (NOLOCK)
                                INNER JOIN
	                                QueueItem QI WITH (NOLOCK)
	                                ON
	                                Q.QueueId = '{0}'
	                                AND
	                                QI.QueueId = Q.QueueId
                                INNER JOIN
	                                PhoneCall PC WITH (NOLOCK)
	                                ON
	                                PC.ActivityId = QI.ObjectId
	                                AND
	                                PC.StateCode = 0
                                INNER JOIN
	                                ActivityParty AS AP
	                                ON
	                                AP.ActivityId = PC.ActivityId 
	                                AND
	                                AP.ParticipationTypeMask=2 -- To
                                INNER JOIN
	                                ActivityParty AS AP2
	                                ON
	                                AP2.ActivityId = PC.ActivityId 
	                                AND
	                                AP2.ParticipationTypeMask=1 -- From
                                INNER JOIN
	                                StringMap SM WITH (NOLOCK)
	                                ON
	                                SM.ObjectTypeCode = 4210
	                                AND
	                                SM.AttributeName = 'prioritycode'
	                                AND
	                                SM.AttributeValue = PC.PriorityCode
                                ORDER BY
                                    PC.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, queueId.ToString()));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET QUEUE'S PHONE CALL |
                    List<Activity> returnList = new List<Activity>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Activity _activity = new Activity();
                        _activity.ActivityId = (Guid)dt.Rows[i]["ActivityId"];
                        _activity.Direction = dt.Rows[i]["DirectionCode"] != DBNull.Value ? (bool)dt.Rows[i]["DirectionCode"] ? Directions.Giden : Directions.Gelen : Directions.Giden;
                        _activity.PhoneNumber = dt.Rows[i]["PhoneNumber"] != DBNull.Value ? dt.Rows[i]["PhoneNumber"].ToString() : string.Empty;
                        _activity.CreatedOnString = dt.Rows[i]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[i]["CreatedOn"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                        _activity.Owner = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString() };
                        _activity.Priority = (PriorityValues)(int)dt.Rows[i]["PriorityValue"];
                        _activity.PriorityString = dt.Rows[i]["PriorityName"].ToString();
                        _activity.ActivityStatusCode = ActivityStatusCodes.Bos;
                        _activity.ProjectName = dt.Rows[i]["ProjectName"] != DBNull.Value ? dt.Rows[i]["ProjectName"].ToString() : string.Empty;
                        _activity.ContactMessage = dt.Rows[i]["ContactMessage"] != DBNull.Value ? dt.Rows[i]["ContactMessage"].ToString() : string.Empty;
                        if (_activity.Direction == Directions.Giden)
                            _activity.Contact = new EntityReference() { LogicalName = "contact", Id = (Guid)dt.Rows[i]["ToId"], Name = dt.Rows[i]["ToIdName"].ToString() };
                        else
                            _activity.Contact = new EntityReference() { LogicalName = "contact", Id = (Guid)dt.Rows[i]["FromId"], Name = dt.Rows[i]["FromIdName"].ToString() };

                        if (dt.Rows[i]["FromId"] != DBNull.Value)
                        {
                            if (_activity.Direction == Directions.Giden)
                                _activity.ActivityParty = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["FromId"], Name = dt.Rows[i]["FromIdName"].ToString() };
                            else
                                _activity.ActivityParty = new EntityReference() { LogicalName = "systemuser", Id = (Guid)dt.Rows[i]["ToId"], Name = dt.Rows[i]["ToIdName"].ToString() };
                        }

                        _activity.SubjectString = dt.Rows[i]["Subject"] != DBNull.Value ? dt.Rows[i]["Subject"].ToString() : "";

                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Sistemde etkin telefon görüşmesi bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetCallCenterPhoneCallsByCustomer(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                PC.ActivityId
	                                ,PC.DirectionCode
	                                ,PC.PhoneNumber
	                                ,AP.PartyId ContactId
	                                ,AP.PartyIdName ContactIdName
                                FROM
	                                PhoneCall PC WITH (NOLOCK)
                                INNER JOIN
	                                ActivityParty AS AP
	                                ON
									PC.ActivityId = AP.ActivityId
									AND
	                                AP.PartyId = '{0}' 
									AND
									AP.ParticipationTypeMask IN(1,2)
									AND
									PC.StateCode = 0
                                INNER JOIN
	                                QueueItem QI WITH (NOLOCK)
									ON
									QI.ObjectId = PC.ActivityId
								INNER JOIN
									Queue Q WITH (NOLOCK)
									ON
									QI.QueueId = Q.QueueId
									AND
									Q.new_queuetype = 1
								ORDER BY
									PC.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET QUEUE'S PHONE CALL |
                    List<Activity> returnList = new List<Activity>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Activity _activity = new Activity();
                        _activity.ActivityId = (Guid)dt.Rows[i]["ActivityId"];
                        _activity.Direction = dt.Rows[i]["DirectionCode"] != DBNull.Value ? (bool)dt.Rows[i]["DirectionCode"] ? Directions.Giden : Directions.Gelen : Directions.Giden;
                        _activity.PhoneNumber = dt.Rows[i]["PhoneNumber"] != DBNull.Value ? dt.Rows[i]["PhoneNumber"].ToString() : string.Empty;

                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Müşteriye ait etkin çağrı merkezi görüşmesi bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivityTopics(string userTypeCode, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            UserTypes userType = UserTypes.Bos;

            string addSqlQuery = string.Empty;

            if (!string.IsNullOrEmpty(userTypeCode))
            {
                userType = (UserTypes)Convert.ToInt32(userTypeCode);

                if (userType == UserTypes.CallCenter)
                {
                    addSqlQuery = " AND A.new_isreceptionist=1 ";
                }
                else if (userType == UserTypes.MusteriIliskileri)
                {
                    addSqlQuery = " AND A.new_iscustomerrelationship=1 ";
                }
                else
                {
                    addSqlQuery = " AND A.new_issalesperson=1 ";
                }
            }

            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                A.new_activitytopicId Id
	                                ,A.new_name Name
	                                ,A.new_id Code
                                FROM
	                                new_activitytopic A WITh (NOLOCK)
                                WHERE
	                                A.StateCode = 0" + addSqlQuery;
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY TPICS |
                    List<ActivityTopic> returnList = new List<ActivityTopic>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ActivityTopic _activity = new ActivityTopic();
                        _activity.Id = (Guid)dt.Rows[i]["Id"];
                        _activity.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;
                        _activity.Code = dt.Rows[i]["Code"] != DBNull.Value ? dt.Rows[i]["Code"].ToString() : string.Empty;

                        returnList.Add(_activity);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Result = "Sistemde etkin görüşme konusu bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CheckActivityOwnership(Guid userId, Guid activityId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    COUNT(0) RecCount
                                    FROM
	                                    ActivityPointer AS apnt (NOLOCK)
                                    WHERE
	                                    apnt.ActivityId='{1}'
                                    AND
	                                    apnt.OwnerId='{0}'";

                #endregion

                int recCount = (int)sda.ExecuteScalar(string.Format(sqlQuery, userId, activityId));

                if (recCount > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Yetkiniz var.";
                }
                else
                {
                    returnValue.Result = "Yetkiniz yok.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static void AddPhoneNumber(Entity entity, IOrganizationService service)
        {
            Entity account = null;
            Entity contact = null;
            Guid Id = Guid.Empty;
            string phoneNumber = string.Empty;
            EntityReference to = ((EntityReference)((EntityCollection)(entity["to"])).Entities[0]["partyid"]);
            if (to.LogicalName.ToLower() == "account")
            {
                account = service.Retrieve("account", to.Id, new ColumnSet("telephone1"));
                phoneNumber = account.Contains("telephone1") ? (string)account.Attributes["telephone1"] : string.Empty;
            }
            else if (to.LogicalName.ToLower() == "contact")
            {
                contact = service.Retrieve("contact", to.Id, new ColumnSet("mobilephone"));
                phoneNumber = contact.Contains("mobilephone") ? (string)contact.Attributes["mobilephone"] : string.Empty;
            }
            if (phoneNumber != string.Empty)
            {

                entity["phonenumber"] = phoneNumber;

            }

        }

        public static MsCrmResultObject GetRetailUserFromActivity(string activityId, SqlDataAccess sda)
        {
            MsCrmResultObject result = new MsCrmResultObject();
            string sql = @"  SELECT 
	                            P.new_retailerid AS 'ID'
                            FROM 
	                            PhoneCall P WITH (NOLOCK) 
                            WHERE 
	                            p.ActivityId = '{0}' 
	                            UNION
                            SELECT 
	                            A.new_retailerid AS ID
                            FROM 
	                            Appointment A WITH(NOLOCK) 
                            WHERE 
	                            A.ActivityId = '{0}' ";

            DataTable dt = sda.getDataTable(string.Format(sql, activityId));
            if (dt.Rows.Count > 0)
            {
                result.Success = true;
                result.CrmId = (Guid)dt.Rows[0]["ID"];
            }
            result.Success = false;
            return result;
        }
    }
}
