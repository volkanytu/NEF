
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Business;


namespace NEF.Plugins.InterestedProjectPlugIn
{
    public class PreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;


            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                #region | Service |
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(Globals.AdministratorId);
            OrganizationServiceContext orgContext = new OrganizationServiceContext(service);

            #endregion

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                Guid projectId = entity.Contains("new_projectid") ? ((EntityReference)entity.Attributes["new_projectid"]).Id : Guid.Empty;
                Guid phoneCallId = entity.Contains("new_phonecallid") ? ((EntityReference)entity.Attributes["new_phonecallid"]).Id : Guid.Empty;
                Guid appointmentId = entity.Contains("new_appointmentid") ? ((EntityReference)entity.Attributes["new_appointmentid"]).Id : Guid.Empty;
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

                #region | ASSOCIATE |
                if (Result.Entities.Count > 0)
                {
                    EntityReference contact = (EntityReference)Result.Entities[0].Attributes["partyid"];

                    MsCrmResult contactHasThisProject = InterestedProjectHelper.ContactHasThisProject(contact.Id, projectId, sda);

                    if (!contactHasThisProject.Success)
                    {
                        EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
                        relatedEntities.Add(contact);

                        // Add the Account Contact relationship schema name
                        Relationship relationship = new Relationship("new_contact_new_project");

                        // Associate the contact record to Account
                        service.Associate("new_project", projectId, relationship, relatedEntities);
                    }
                }
                else
                {
                    return;
                }
                #endregion

            }
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
