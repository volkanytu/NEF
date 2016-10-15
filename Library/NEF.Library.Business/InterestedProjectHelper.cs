
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
    public static class InterestedProjectHelper
    {
        public static MsCrmResultObject GetActivityInterestedProjects(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedprojectId Id
                                    ,IP.new_projectid ProjectId
                                    ,IP.new_projectidName ProjectIdName
                                FROM
	                                new_interestedproject IP WITH (NOLOCK)
                                INNER JOIN
	                                new_project P WITH (NOLOCK)
	                                ON
                                    (
                                        IP.new_phonecallid = '{0}'
                                        OR
	                                    IP.new_appointmentid = '{0}'
                                    )
	                                AND
	                                P.new_projectId = IP.new_projectid
                                WHERE
	                                IP.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY INTEREST PRODUCTS |
                    List<InterestProject> returnList = new List<InterestProject>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InterestProject _interest = new InterestProject();
                        _interest.InterestProjectId = (Guid)dt.Rows[i]["Id"];

                        if (dt.Rows[i]["ProjectId"] != DBNull.Value)
                        {
                            EntityReference er = new EntityReference();
                            er.Id = (Guid)dt.Rows[i]["ProjectId"];
                            if (dt.Rows[i]["ProjectIdName"] != DBNull.Value) { er.Name = dt.Rows[i]["ProjectIdName"].ToString(); }
                            er.LogicalName = "new_project";

                            _interest.InterestedProject = er;
                        }

                        returnList.Add(_interest);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aktiviye ait ilgilendiği bir proje bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProjectsForActivity(Guid phonecallId, Guid appointmentId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                SqlParameter[] parameters = null;
                #region | PROJECTS |
                string query = @"SELECT
	                                *
                                INTO
	                                #Projects
                                FROM
                                (
	                                SELECT	
		                                P.new_projectId ProjectId
	                                FROM
		                                new_project P WITH (NOLOCK)
	                                WHERE
		                                P.statecode = 0
                                )A";
                query += System.Environment.NewLine;
                #endregion

                #region | INTERESTED PROJECT |
                query += @"SELECT
                            *
                            INTO
	                            #InterestedProjects
                            FROM
                            (
	                            SELECT
		                            IP.new_projectid ProjectId
	                            FROM
		                            new_interestedproject IP WITH (NOLOCK)
		                            WHERE";
                if (phonecallId != Guid.Empty)
                {
                    query += System.Environment.NewLine;
                    query += "      IP.new_phonecallid = '{0}'";
                    query = string.Format(query, phonecallId);
                }
                else
                {
                    query += System.Environment.NewLine;
                    query += "      IP.new_appointmentid = '{0}'";
                    query = string.Format(query, appointmentId);
                }

                query += System.Environment.NewLine;
                query += @"           AND
		                            IP.statecode = 0
                            )B";

                #endregion

                query += System.Environment.NewLine;
                query += @" SELECT
	                            *
                            FROM
	                            #Projects T
                            WHERE
	                            T.ProjectId NOT IN (SELECT * FROM #InterestedProjects)

                            DROP TABLE #Projects
                            DROP TABLE #InterestedProjects";
                #endregion

                DataTable dt = null;
                if (phonecallId != Guid.Empty)
                {
                    dt = sda.getDataTable(string.Format(query, phonecallId));
                }
                else
                {
                    dt = sda.getDataTable(string.Format(query, appointmentId));
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PROJECTS |
                    List<Project> returnList = new List<Project>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Project _project = GetProjectDetail((Guid)dt.Rows[i]["ProjectId"], sda);
                        returnList.Add(_project);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin proje bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreateInterestedProject(Guid projectId, Guid phoneCallId, Guid appointmentId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_interestedproject");
                ent["new_projectid"] = new EntityReference("new_project", projectId); ;

                if (phoneCallId != Guid.Empty)
                {
                    ent["new_phonecallid"] = new EntityReference("phonecall", phoneCallId);
                }

                if (appointmentId != Guid.Empty)
                {
                    ent["new_appointmentid"] = new EntityReference("appointment", appointmentId);
                }

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "İlgili proje başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static Project GetProjectDetail(Guid projectId, SqlDataAccess sda)
        {
            Project _p = new Project();
            string query = @"SELECT
						P.new_projectId Id
						,P.new_name Name
					FROM
						new_project P WITH (NOLOCK)
					WHERE
						P.new_projectId = '{0}'";
            DataTable dt = sda.getDataTable(string.Format(query, projectId));

            if (dt != null)
            {
                _p.ProjectId = (Guid)dt.Rows[0]["Id"];
                _p.Name = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;
            }
            return _p;
        }

        public static MsCrmResult RemoveInterestedProject(Guid interestedProjectId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                SetStateRequest stateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference("new_interestedproject", interestedProjectId),
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(2)
                };

                SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult ContactHasThisProject(Guid contactId, Guid projectId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_projectid ProjectId
                                FROM
	                                new_contact_new_project IP WITH (NOLOCK)
                                WHERE
	                                IP.contactid = '{0}'
	                                AND
	                                IP.new_projectid = '{1}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, contactId, projectId));

                if (dt != null && dt.Rows.Count > 0)
                    returnValue.Success = true;
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
    }
}
