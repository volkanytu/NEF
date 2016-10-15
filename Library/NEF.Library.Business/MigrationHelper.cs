using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using System.Data;

namespace NEF.Library.Business
{
    public static class MigrationHelper
    {
        public static void executeMultipleInsert(EntityCollection entities, IOrganizationService service)
        {
            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (Entity e in entities.Entities)
            {
                if (e.Attributes.Contains("statecode") && ((OptionSetValue)e["statecode"]).Value != 0)
                {
                    e.Attributes.Remove("statecode");

                    if (e.Attributes.Contains("statuscode") && ((OptionSetValue)e["statuscode"]).Value != 0)
                    {
                        e.Attributes.Remove("statuscode");
                    }
                }

                //In this instance, we use a CreateRequest, although there are also options for UpdateRequest and DeleteRequest		
                CreateRequest createRequest = new CreateRequest();
                //Point it at the entity to insert
                createRequest.Target = e;
                //Add the entity to the ExecuteMultipleRequest
                multipleRequest.Requests.Add(createRequest);
            }
            //This is how to simply execute the command without any response. See example below of how to use get responses from the request. 
            //_crmServ.Execute(multipleRequest);

            //This is how to get responses
            ExecuteMultipleResponse executeResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            if (executeResponse.Results != null)
            {
                //Loop through responses
                foreach (ExecuteMultipleResponseItem responseItem in executeResponse.Responses)
                {
                    if (responseItem.Response != null)
                    {
                        OrganizationRequest req = multipleRequest.Requests[responseItem.RequestIndex];

                        if (req.Parameters.Contains("Target"))
                        {
                            Entity entity = (Entity)req.Parameters["Target"];

                            if (entity.LogicalName == "systemuser")
                            {
                                service.Associate(
                                                    "systemuser",
                                                    entity.Id,
                                                    new Relationship("systemuserroles_association"),
                                                    new EntityReferenceCollection() { new EntityReference("role", new Guid("7661CBFA-5997-E411-80C0-005056A60603")) } //CEO Rolü
                                                    );
                            }
                        }
                        //Awesome, command completed as expected
                        //success++;
                    }
                    else if (responseItem.Fault != null)
                    {
                        OrganizationRequest req = multipleRequest.Requests[responseItem.RequestIndex];

                        if (req.Parameters.Contains("Target"))
                        {
                            Entity entity = (Entity)req.Parameters["Target"];

                            GeneralHelper.WriteToText(entity.Id + ";INSERT-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + entity.LogicalName + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        else
                        {
                            GeneralHelper.WriteToText(Guid.Empty + ";INSERT-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + "noentity" + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        //Uh oh, error 
                        //error++;
                    }
                    else
                    {
                        //Error reporting error;
                    }
                }
            }
        }

        public static void executeMultipleSetSate(EntityCollection entities, IOrganizationService service)
        {
            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (Entity e in entities.Entities)
            {
                SetStateRequest setStateReq = new SetStateRequest();
                setStateReq.EntityMoniker = new EntityReference(e.LogicalName, e.Id);

                setStateReq.State = (OptionSetValue)e["statecode"];
                setStateReq.Status = (OptionSetValue)e["statuscode"];

                multipleRequest.Requests.Add(setStateReq);
            }

            //This is how to get responses
            ExecuteMultipleResponse executeResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            if (executeResponse.Results != null)
            {
                //Loop through responses
                foreach (ExecuteMultipleResponseItem responseItem in executeResponse.Responses)
                {
                    if (responseItem.Response != null)
                    {
                        //Awesome, command completed as expected
                        //success++;
                    }
                    else if (responseItem.Fault != null)
                    {
                        OrganizationRequest req = multipleRequest.Requests[responseItem.RequestIndex];

                        if (req.Parameters.Contains("Target"))
                        {
                            Entity entity = (Entity)req.Parameters["Target"];

                            GeneralHelper.WriteToText(entity.Id + ";SETSATE-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + entity.LogicalName + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        else
                        {
                            GeneralHelper.WriteToText(Guid.Empty + ";SETSATE-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + "noentity" + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        //Uh oh, error 
                        //error++;
                    }
                    else
                    {
                        //Error reporting error;
                    }
                }
            }
        }

        public static void executeMultipleCloseOpportunity(EntityCollection entities, IOrganizationService service)
        {
            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (Entity e in entities.Entities)
            {
                if (((OptionSetValue)e["statecode"]).Value == 1)
                {
                    WinOpportunityRequest req = new WinOpportunityRequest();
                    Entity opportunityClose = new Entity("opportunityclose");
                    opportunityClose["opportunityid"] = new EntityReference("opportunity", e.Id);
                    opportunityClose["subject"] = "Win the Opportunity!";
                    opportunityClose["actualend"] = (DateTime)e["actualend"];

                    req.OpportunityClose = opportunityClose;
                    req.Status = (OptionSetValue)e["statuscode"];

                    multipleRequest.Requests.Add(req);
                }
                else if (((OptionSetValue)e["statecode"]).Value == 2)
                {
                    LoseOpportunityRequest req = new LoseOpportunityRequest();
                    Entity opportunityClose = new Entity("opportunityclose");
                    opportunityClose["opportunityid"] = new EntityReference("opportunity", e.Id);
                    opportunityClose["subject"] = "Lost the Opportunity!";
                    opportunityClose["actualend"] = (DateTime)e["actualend"];

                    req.OpportunityClose = opportunityClose;
                    req.Status = (OptionSetValue)e["statuscode"];

                    multipleRequest.Requests.Add(req);
                }


            }

            //This is how to get responses
            ExecuteMultipleResponse executeResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            if (executeResponse.Results != null)
            {
                //Loop through responses
                foreach (ExecuteMultipleResponseItem responseItem in executeResponse.Responses)
                {
                    if (responseItem.Response != null)
                    {
                        //Awesome, command completed as expected
                        //success++;
                    }
                    else if (responseItem.Fault != null)
                    {
                        OrganizationRequest req = multipleRequest.Requests[responseItem.RequestIndex];

                        if (req.Parameters.Contains("Target"))
                        {
                            Entity entity = (Entity)req.Parameters["Target"];

                            GeneralHelper.WriteToText(entity.Id + ";OPPCLOSE-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + entity.LogicalName + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        else
                        {
                            GeneralHelper.WriteToText(Guid.Empty + ";OPPCLOSE-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + "noentity" + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        //Uh oh, error 
                        //error++;
                    }
                    else
                    {
                        //Error reporting error;
                    }
                }
            }
        }

        public static void executeMultipleCloseQuote(EntityCollection entities, IOrganizationService service)
        {
            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (Entity e in entities.Entities)
            {
                if (((OptionSetValue)e["statecode"]).Value == 3)
                {
                    Entity quoteClose = new Entity("quoteclose");
                    quoteClose["quoteid"] = new EntityReference("quote", e.Id);
                    quoteClose["subject"] = "Quote Close" + DateTime.Now.ToString();

                    WinQuoteRequest req = new WinQuoteRequest()
                    {
                        QuoteClose = quoteClose,
                        Status = (OptionSetValue)e["statuscode"]
                    };

                    multipleRequest.Requests.Add(req);
                }
                else if (((OptionSetValue)e["statecode"]).Value == 4)
                {
                    Entity quoteClose = new Entity("quoteclose");
                    quoteClose["quoteid"] = new EntityReference("quote", e.Id);
                    quoteClose["subject"] = "Quote Close" + DateTime.Now.ToString();

                    CloseQuoteRequest req = new CloseQuoteRequest()
                    {
                        QuoteClose = quoteClose,
                        Status = (OptionSetValue)e["statuscode"]
                    };

                    multipleRequest.Requests.Add(req);
                }
            }

            //This is how to get responses
            ExecuteMultipleResponse executeResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            if (executeResponse.Results != null)
            {
                //Loop through responses
                foreach (ExecuteMultipleResponseItem responseItem in executeResponse.Responses)
                {
                    if (responseItem.Response != null)
                    {
                        //Awesome, command completed as expected
                        //success++;
                    }
                    else if (responseItem.Fault != null)
                    {
                        OrganizationRequest req = multipleRequest.Requests[responseItem.RequestIndex];

                        if (req.Parameters.Contains("Target"))
                        {
                            Entity entity = (Entity)req.Parameters["Target"];

                            GeneralHelper.WriteToText(entity.Id + ";QUOTECLOSE-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + entity.LogicalName + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        else
                        {
                            GeneralHelper.WriteToText(Guid.Empty + ";QUOTECLOSE-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + "noentity" + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        //Uh oh, error 
                        //error++;
                    }
                    else
                    {
                        //Error reporting error;
                    }
                }
            }
        }

        public static void executeMultipleWithRequests(OrganizationRequestCollection requests, string logFileName, IOrganizationService service)
        {
            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            multipleRequest.Requests = requests;

            //This is how to get responses
            ExecuteMultipleResponse executeResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            if (executeResponse.Results != null)
            {
                //Loop through responses
                foreach (ExecuteMultipleResponseItem responseItem in executeResponse.Responses)
                {
                    if (responseItem.Response != null)
                    {
                        //Awesome, command completed as expected
                        //success++;
                    }
                    else if (responseItem.Fault != null)
                    {
                        OrganizationRequest req = multipleRequest.Requests[responseItem.RequestIndex];
                        if (req.Parameters.Contains("Target"))
                        {
                            Entity entity = (Entity)req.Parameters["Target"];

                            GeneralHelper.WriteToText(entity.Id + ";" + req.RequestName + "-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + logFileName + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        else
                        {
                            GeneralHelper.WriteToText(((AssociateEntitiesRequest)req).Moniker1.Id + ";" + req.RequestName + "-" + responseItem.Fault.Message, @"C:\sahibinden\migration_" + logFileName + "_" + DateTime.Now.ToShortDateString() + ".txt");
                        }
                        //Uh oh, error 
                        //error++;
                    }
                    else
                    {
                        //Error reporting error;
                    }
                }
            }
        }

        public static string GetContactGroupFinancialCode(Guid contactId, SqlDataAccess sda)
        {
            string returnValue = string.Empty;

            try
            {
                #region | SQLQ QUERY |

                string sqlQuery = @"SELECT
                                    DISTINCT
	                                    gs.new_grupsirketId AS Id
	                                    ,gs.new_carikodu AS Code
                                    FROM
                                    Opportunity AS opp (NOLOCK)
	                                    JOIN
		                                    new_grupsirket AS gs (NOLOCK)
			                                    ON
			                                    opp.new_grupsirketid=gs.new_grupsirketId
                                    WHERE
	                                    opp.CustomerId='{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, contactId));

                if (dt.Rows.Count > 0)
                {

                    returnValue = dt.Rows[0]["Code"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static void SetStateEntity(Entity entity, IOrganizationService service)
        {
            try
            {
                SetStateRequest setStateReq = new SetStateRequest();
                setStateReq.EntityMoniker = new EntityReference(entity.LogicalName, entity.Id);

                setStateReq.State = (OptionSetValue)entity["statecode"];
                setStateReq.Status = (OptionSetValue)entity["statuscode"];

                SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
            }
            catch (Exception ex)
            {

            }

        }

        public static Guid GetContactIdByGroupFinancialCode(string groupFinancialCode, SqlDataAccess sda)
        {
            Guid returnValue = Guid.Empty;

            try
            {
                #region | SQLQ QUERY |

                string sqlQuery = @"SELECT
                                    c.ContactId AS Id
                                    FROM
                                    Contact AS c (NOLOCK)
                                    WHERE
                                    c.new_groupfinancialcode='{0}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, groupFinancialCode));

                if (dt.Rows.Count > 0)
                {
                    returnValue = (Guid)dt.Rows[0]["Id"];
                }

            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
    }
}
