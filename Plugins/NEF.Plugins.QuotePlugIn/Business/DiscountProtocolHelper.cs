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
    public static class DiscountProtocolHelper
    {
        public static void SetStatusDeactiveDiscountProtocol(Guid quoteId, IOrganizationService service, SqlDataAccess sda)
        {
            string getDiscountTypesQuery = @"SELECT 
	                                            new_sales AS QuoteId,
	                                            new_discounttype AS DiscountType,
	                                            new_discountprotocolsId AS DisccountID,
                                                new_referancesales AS ReferanceSalesId
                                            FROM
	                                            new_discountprotocols AS dp WITH(NOLOCK)
                                            WHERE 
	                                            dp.new_referancesales = @quoteId";

            DataTable discountProtocolsDt = sda.getDataTable(getDiscountTypesQuery, new SqlParameter[] { new SqlParameter("quoteId", quoteId) });

            foreach (DataRow item in discountProtocolsDt.Rows)
            {
                SetStateRequest state = new SetStateRequest();
                state.State = new OptionSetValue(1);
                state.Status = new OptionSetValue(2);
                state.EntityMoniker = new EntityReference("new_discountprotocols", new Guid(Convert.ToString(item["DisccountID"])));
                service.Execute(state);
            }
        }


        public static void SetStatusDiscountProtocol(Guid discountProtocolId, IOrganizationService service, int stateCode, int statusCode)
        {
                SetStateRequest state = new SetStateRequest();
                state.State = new OptionSetValue(stateCode);
                state.Status = new OptionSetValue(statusCode);
                state.EntityMoniker = new EntityReference("new_discountprotocols", discountProtocolId);
                service.Execute(state);
            
        }



        public static void SetDiscountProtocolsForNewQuote(Entity quote, Guid quoteId, IOrganizationService adminService)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_sales";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("new_discountprotocols");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = adminService.RetrieveMultiple(Query);
            foreach (Entity p in Result.Entities)
            {
                Entity dProtocol = new Entity("new_discountprotocols");
                dProtocol.Attributes["new_sales"] = new EntityReference("quote", quote.Id);
                if (p.Contains("new_referancesales"))
                    dProtocol["new_referancesales"] = new EntityReference("quote", p.GetAttributeValue<EntityReference>("new_referancesales").Id);
                if (p.Contains("new_accountid"))
                    dProtocol["new_accountid"] = new EntityReference("account", p.GetAttributeValue<EntityReference>("new_accountid").Id);
                else if (p.Contains("new_customer"))
                    dProtocol["new_customer"] = new EntityReference("contact", p.GetAttributeValue<EntityReference>("new_customer").Id);
                if (p.Contains("new_discounttype"))
                    dProtocol["new_discounttype"] = p.GetAttributeValue<OptionSetValue>("new_discounttype");
                if (p.Contains("new_name"))
                    dProtocol["new_name"] = p.GetAttributeValue<string>("new_name");
                if (p.Contains("new_description"))
                    dProtocol["new_description"] = p.GetAttributeValue<string>("new_description");
                if (p.Contains("new_referancecontact"))
                    dProtocol["new_referancecontact"] = new EntityReference("contact", p.GetAttributeValue<EntityReference>("new_referancecontact").Id);

                if (p.Contains("transactioncurrencyid"))
                    dProtocol["transactioncurrencyid"] = p["transactioncurrencyid"];

                if (p.Contains("new_istransfered"))
                {
                    if (p.GetAttributeValue<bool>("new_istransfered"))
                        dProtocol["new_istransfered"] = true;
                    else
                        dProtocol["new_istransfered"] = false;
                }
               
                if (p.Contains("new_discountamount"))
                {
                    dProtocol["new_discountamount"] = p.GetAttributeValue<Decimal>("new_discountamount");
                }
                Guid discountProtocolId = adminService.Create(dProtocol);
                SetStatusDiscountProtocol(discountProtocolId, adminService, p.GetAttributeValue<OptionSetValue>("statecode").Value, p.GetAttributeValue<OptionSetValue>("statuscode").Value);
                adminService.Delete("new_discountprotocols", p.Id);
            }
        }
    }
}
