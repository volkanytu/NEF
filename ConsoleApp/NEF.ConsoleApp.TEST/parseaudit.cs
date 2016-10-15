using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.ConsoleApp.TEST.test2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using System.Data;
using System.Globalization;
using Microsoft.Xrm.Sdk;
//using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
namespace NEF.ConsoleApp.TEST
{
    public static class parseaudit
    {
        public static void Process()
        {

        }

        public static void ExtractFromAudit()
        {
            IOrganizationService service = MSCRM.GetOrgService(true);

            // The GUID of the object you want to retirve in this case i am passing contactid  
            var entityId = new Guid("B53045DC-8BF1-E411-80D0-005056A60603");

            Console.WriteLine("Retrieving the change history.\n");

            //
            //RetrieveAttributeChangeHistoryRequest req = new RetrieveAttributeChangeHistoryRequest();
            //req.Target = new Microsoft.Xrm.Sdk.EntityReference("quote", new Guid("592AF7E6-ADC1-E011-8D7C-1CC1DE79838E"));
            //req.AttributeLogicalName = "totallineitemamount";
            //RetrieveAttributeChangeHistoryResponse resp = (RetrieveAttributeChangeHistoryResponse)service.Execute(req);

            






            // Retrieve the audit history for the account and display it.  
            RetrieveRecordChangeHistoryRequest changeRequest = new RetrieveRecordChangeHistoryRequest();
            changeRequest.Target = new Microsoft.Xrm.Sdk.EntityReference("quote", entityId);




            
            RetrieveRecordChangeHistoryResponse changeResponse =
                (RetrieveRecordChangeHistoryResponse)service.Execute(changeRequest);

            AuditDetailCollection details = changeResponse.AuditDetailCollection;

            foreach (AttributeAuditDetail detail in details.AuditDetails)
            {
                DisplayAuditDetails(detail);
                // Display some of the detail information in each audit record.   
                
            }
        }

        private static void DisplayAuditDetails(AuditDetail detail)
        {
            // Write out some of the change history information in the audit record.   
            Entity record = detail.AuditRecord;

            

            // Show additional details for certain AuditDetail sub-types.  
            var detailType = detail.GetType();
            if (detailType == typeof(AttributeAuditDetail))
            {
                var attributeDetail = (AttributeAuditDetail)detail;
                
                // Display the old and new attribute values.  
                foreach (KeyValuePair<string, object> attribute in attributeDetail.NewValue.Attributes)
                {
                    if (attribute.Key != "totallineitemamount")
                    {
                        continue;
                    }
                    Console.WriteLine("\nAudit record created on: {0}", record["createdon"]);
                    Console.WriteLine("Entity: {0}, Action: {1}, Operation: {2}",
                        record.LogicalName, record.FormattedValues["action"],
                        record.FormattedValues["operation"]);

                    String oldValue = "(no value)", newValue = "(no value)";

                    //TODO Display the lookup values of those attributes that do not contain strings.  
                    if (attributeDetail.OldValue.Contains(attribute.Key))
                        oldValue = ((Money)attributeDetail.OldValue[attribute.Key]).Value.ToString();

                    newValue = ((Money)attributeDetail.NewValue[attribute.Key]).Value.ToString();

                    Console.WriteLine("Attribute: {0}, old value: {1}, new value: {2}",
                        attribute.Key, oldValue, newValue);
                }

                foreach (KeyValuePair<string, object> attribute in attributeDetail.OldValue.Attributes)
                {
                    if (attribute.Key != "totallineitemamount")
                    {
                        continue;
                    }
                    Console.WriteLine("\nAudit record created on: {0}", record["createdon"]);
                    Console.WriteLine("Entity: {0}, Action: {1}, Operation: {2}",
                        record.LogicalName, record.FormattedValues["action"],
                        record.FormattedValues["operation"]);

                    if (!attributeDetail.NewValue.Contains(attribute.Key))
                    {
                        String newValue = "(no value)";

                        //TODO Display the lookup values of those attributes that do not contain strings.  
                        String oldValue = attributeDetail.OldValue[attribute.Key].ToString();

                        Console.WriteLine("Attribute: {0}, old value: {1}, new value: {2}",
                            attribute.Key, oldValue, newValue);
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
