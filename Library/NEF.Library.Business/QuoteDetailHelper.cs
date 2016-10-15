using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class QuoteDetailHelper
    {
        internal static void SetProductAndProjectOnQuote(Entity entity, IOrganizationService adminService)
        {
            Guid quoteId = ((EntityReference)entity.Attributes["quoteid"]).Id;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet("quotedetailid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = adminService.RetrieveMultiple(Query);
            Entity q = adminService.Retrieve("quote", quoteId, new ColumnSet("new_taxrate", "new_taxofstamp", "totalamount", "discountamount", "revisionnumber", "quotenumber"));
            Entity oldQuote = GetPreviousQuoteByRevisionAndQuoteNumber(Convert.ToInt32(q["revisionnumber"]), Convert.ToString(q["quotenumber"]), adminService);

            if (Convert.ToInt32(q["revisionnumber"]) > 0)
            {
                Entity qd = new Entity("quotedetail");
                qd.Id = Result.Entities[0].Id;
                qd["ispriceoverridden"] = true;
                qd["priceperunit"] = oldQuote["totallineitemamount"];
                adminService.Update(qd);
            }
            else if (Result.Entities.Count > 0)
            {
                Entity qd = new Entity("quotedetail");
                qd.Id = Result.Entities[0].Id;
                qd["ispriceoverridden"] = false;
                adminService.Update(qd);
            }

            Guid productId = entity.Contains("productid") ? ((EntityReference)entity.Attributes["productid"]).Id : Guid.Empty;
            Guid projectId = Guid.Empty;
            if (productId != Guid.Empty)
            {
                Entity product = adminService.Retrieve("product", productId, new ColumnSet("new_projectid", "new_grossm2"));
                projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
                Entity quote = new Entity("quote");
                quote.Id = quoteId;
                quote.Attributes.Add("new_productid", new EntityReference("product", productId));
                if (projectId != Guid.Empty)
                {
                    quote.Attributes.Add("new_projectid", new EntityReference("new_project", projectId));

                }

                if (!(Convert.ToInt32(q["revisionnumber"]) > 0))
                {
                    decimal totalAmount = q.Contains("totalamount") ? ((Money)q.Attributes["totalamount"]).Value : 0;

                    decimal taxRate = q.Contains("new_taxrate") ? (decimal)q.Attributes["new_taxrate"] : 0;
                    decimal taxOfStamp = q.Contains("new_taxofstamp") ? (decimal)q.Attributes["new_taxofstamp"] : 0;
                    decimal taxAmount = (totalAmount * taxRate) / 100;
                    decimal amountWithTax = totalAmount + taxAmount;
                    decimal taxOfStampAmount = (totalAmount * taxOfStamp) / 100;
                    quote.Attributes["new_taxamount"] = new Money(taxAmount);
                    quote.Attributes["new_amountwithtax"] = new Money(amountWithTax);
                    quote.Attributes["new_taxofstampamount"] = new Money(taxOfStampAmount);
                    quote.Attributes["new_totalsalesamountbytax"] = new Money(amountWithTax + taxOfStampAmount);

                    //Birim metrekare fiyatı
                    Entity detail = GetProductFromQuoteDetail(quoteId, adminService);
                    if (detail != null)
                    {
                        if (product.Contains("new_grossm2"))
                        {
                            decimal grossM2 = (decimal)product.Attributes["new_grossm2"];
                            decimal perSquareMeter = totalAmount / grossM2;
                            quote["new_persquaremeter"] = new Money(perSquareMeter);
                        }
                    }
                }
                adminService.Update(quote);
            }
        }
        private static Entity GetProductFromQuoteDetail(Guid quoteId, IOrganizationService service)
        {


            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet("productid", "uomid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0];
            }
            else
            {
                return null;
            }



        }

        public static void SetPreviousQuoteAmount(Entity entity, IOrganizationService svc)
        {
            //Teklif detayının ait olduğu teklif bul
            Guid quoteId = ((EntityReference)entity.Attributes["quoteid"]).Id;
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);
            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            QueryExpression Query = new QueryExpression("quote");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = svc.RetrieveMultiple(Query);

            if (Result.Entities.Count > 0)
            {
                //teklif numarasını al ve bir önceki düzeltilmiş kaydın tutarını al 
                Entity currentQuote = Result.Entities[0];
                int revisionNumber = currentQuote.GetAttributeValue<int>("revisionnumber");
                string quoteNumber = currentQuote.GetAttributeValue<string>("QUO-10717-H5R4C6");

                if (revisionNumber > 0)
                {
                    Entity oldQuote = GetPreviousQuoteByRevisionAndQuoteNumber(revisionNumber, quoteNumber, svc);
                    entity["baseamount"] = oldQuote.GetAttributeValue<Money>("TotalLineItemAmount");
                    entity["ispriceoverridden"] = true;
                    svc.Update(entity);
                }
            }
        }

        private static Entity GetPreviousQuoteByRevisionAndQuoteNumber(int revisionNumber, string quoteNumber, IOrganizationService svc)
        {
            Entity retVal = new Entity();
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "revisionnumber";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(revisionNumber - 1);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "quotenumber";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(quoteNumber);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("quote");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = svc.RetrieveMultiple(Query);

            if (Result.Entities.Count > 0)
            {
                retVal = svc.Retrieve("quote", Result.Entities[0].GetAttributeValue<Guid>("quoteid"), new ColumnSet("totalamount", "new_taxofstamp", "totalamount", "discountamount", "revisionnumber", "quotenumber", "totallineitemamount"));

            }
            return retVal;
        }
    }
}
