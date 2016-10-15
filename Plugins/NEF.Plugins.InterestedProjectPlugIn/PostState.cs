using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.InterestedProjectPlugIn
{
    public class PostState : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {
                #region | SERVICE |
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                #region | Validate Request |
                //Target yoksa veya Entity tipinde değilse, devam etme.
                if (!context.InputParameters.Contains("EntityMoniker") || !(context.InputParameters["EntityMoniker"] is EntityReference))
                {
                    return;
                }
                #endregion


                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


                Guid interestedProjectId = ((EntityReference)context.InputParameters["EntityMoniker"]).Id;
                Entity entity = service.Retrieve("new_interestedproject", interestedProjectId, new ColumnSet(true));
                int stateCode = ((OptionSetValue)entity["statecode"]).Value;
                int statusCode = ((OptionSetValue)entity["statuscode"]).Value;

                if (stateCode == 1)
                {
                    Guid phoneCallId = entity.Contains("new_phonecallid") ? ((EntityReference)entity.Attributes["new_phonecallid"]).Id : Guid.Empty;
                    Guid appointmentId = entity.Contains("new_appointmentid") ? ((EntityReference)entity.Attributes["new_appointmentid"]).Id : Guid.Empty;
                    Guid projectId = entity.Contains("new_projectid") ? ((EntityReference)entity.Attributes["new_projectid"]).Id : Guid.Empty;
                    
                    if (projectId == Guid.Empty)
                        return;
                    if (phoneCallId == Guid.Empty && appointmentId == Guid.Empty)
                        return;

                    #region | GET CONTACT |
                    ConditionExpression con1 = new ConditionExpression();
                    con1.AttributeName = "activityid";
                    con1.Operator = ConditionOperator.Equal;
                    if (appointmentId != Guid.Empty)
                    {
                        con1.Values.Add(appointmentId);
                    }
                    else if (phoneCallId != Guid.Empty)
                    {
                        con1.Values.Add(phoneCallId);
                    }

                    ConditionExpression con2 = new ConditionExpression();
                    con2.AttributeName = "partyobjecttypecode";
                    con2.Operator = ConditionOperator.Equal;
                    con2.Values.Add(2);//Contact

                    FilterExpression filter = new FilterExpression();
                    filter.FilterOperator = LogicalOperator.And;
                    filter.Conditions.Add(con1);
                    filter.Conditions.Add(con2);

                    QueryExpression Query = new QueryExpression("activityparty");
                    Query.ColumnSet = new ColumnSet("partyid");
                    Query.Criteria.FilterOperator = LogicalOperator.And;
                    Query.Criteria.Filters.Add(filter);
                    EntityCollection Result = service.RetrieveMultiple(Query);
                    #endregion

                    #region | DISASSOCIATE |
                    if (Result.Entities.Count > 0)
                    {
                        EntityReference contact = (EntityReference)Result.Entities[0].Attributes["partyid"];

                        EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
                        relatedEntities.Add(contact);

                        // Add the Account Contact relationship schema name
                        Relationship relationship = new Relationship("new_contact_new_project");

                        // DisAssociate the contact record to Account
                        service.Disassociate("new_project", projectId, relationship, relatedEntities);
                    }
                    else
                    {
                        return;
                    }
                    #endregion
                }

                #endregion


            }
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException(ex.Message);
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

        }
    }
}
