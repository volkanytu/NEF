using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace NEF.WebService.CompanyInternalService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CompanyInternal : ICompanyInternal
    {
        private string SuccessMessage = "Başarılıdır";
        public EventLogHelper eventLog;

        public string GetQuote(string productNumber)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();

            QuoteInfoResult quoteInfoResult = new QuoteInfoResult();
            QuoteInfo quoteInfo = new QuoteInfo();


            MsCrmResult result = new MsCrmResult();

            IOrganizationService service;

            if(string.IsNullOrWhiteSpace(productNumber))
            {
                result.Message = "productNumber Değeri Boş Olmaz";
                result.Success = false;

                returnValue = ser.Serialize(quoteInfoResult);
                return returnValue;
            }

            try
            {
                service = MSCRM.GetOrgService(true);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "CRM Erişimi Yapılamamaktadır "+ex.Message;

                quoteInfoResult.Result = result;
                returnValue = ser.Serialize(quoteInfoResult);
                return returnValue;
            }

            try
            {

                LinkEntity productLink = new LinkEntity();
                productLink.EntityAlias = "productLink";
                productLink.LinkFromEntityName = "quote";
                productLink.LinkFromAttributeName = "new_productid";
                productLink.LinkToEntityName = "product";
                productLink.LinkToAttributeName = "productid";
                productLink.Columns = new ColumnSet("productid", "new_licencenumber");
                productLink.LinkCriteria = new FilterExpression(LogicalOperator.And);
                productLink.LinkCriteria.AddCondition("productnumber", ConditionOperator.Equal, productNumber);


                FilterExpression filterExpression = new FilterExpression();
                filterExpression.FilterOperator = LogicalOperator.Or;
                filterExpression.AddCondition("statuscode", ConditionOperator.Equal, (int)QuoteStatus.Kazanıldı);
                filterExpression.AddCondition("statuscode", ConditionOperator.Equal, (int)QuoteStatus.SozlesmeHazirlandi);
                filterExpression.AddCondition("statuscode", ConditionOperator.Equal, (int)QuoteStatus.Sözleşmeİmzalandı);
                filterExpression.AddCondition("statuscode", ConditionOperator.Equal, (int)QuoteStatus.MuhasebeyeAktarıldı);
                filterExpression.AddCondition("statuscode", ConditionOperator.Equal, (int)QuoteStatus.TeslimEdildi);

                QueryExpression quoteQuery = new QueryExpression();
                quoteQuery.EntityName = "quote";
                quoteQuery.Criteria = new FilterExpression(LogicalOperator.And);
                quoteQuery.ColumnSet = new ColumnSet("quotenumber", "new_projectid", "new_productid", "customerid");
                quoteQuery.Criteria.AddFilter(filterExpression);
                quoteQuery.LinkEntities.Add(productLink);
                quoteQuery.NoLock = true;
                quoteQuery.TopCount = 1;
                EntityCollection quoteList = service.RetrieveMultiple(quoteQuery);
                if (quoteList.Entities.Count > 0)
                {
                    string name = string.Empty;
                    string phone = string.Empty;
                    string emailaddress = string.Empty;

                    Entity quote = quoteList.Entities[0];
                    AliasedValue licenceNumber = quote.GetAttributeValue<AliasedValue>("productLink.new_licencenumber");
                    string licenceNumberValue = string.Empty;
                    if(licenceNumber!=null)
                    {
                        licenceNumberValue = Convert.ToString(licenceNumber.Value);
                    }

                    EntityReference productRef = quote.GetAttributeValue<EntityReference>("new_productid");
                    EntityReference projectRef = quote.GetAttributeValue<EntityReference>("new_projectid");
                    string quoteNumber = quote.GetAttributeValue<string>("quotenumber");
                    EntityReference customerRef = quote.GetAttributeValue<EntityReference>("customerid");

                    if (customerRef.LogicalName == "account")
                    {
                        Entity account = service.Retrieve(customerRef.LogicalName, customerRef.Id, new ColumnSet("name", "telephone1", "emailaddress1"));
                        name = account.GetAttributeValue<string>("name");
                        phone = account.GetAttributeValue<string>("telephone1");
                        emailaddress = account.GetAttributeValue<string>("emailaddress1");
                    }
                    else if (customerRef.LogicalName == "contact")
                    {
                        Entity contact = service.Retrieve(customerRef.LogicalName, customerRef.Id, new ColumnSet("fullname", "mobilephone", "emailaddress1"));
                        name = contact.GetAttributeValue<string>("fullname");
                        phone = contact.GetAttributeValue<string>("mobilephone");
                        emailaddress = contact.GetAttributeValue<string>("emailaddress1");
                    }

                    quoteInfo.Name = name;
                    quoteInfo.Email = emailaddress;
                    quoteInfo.Phone = phone;
                    quoteInfo.ProjectName = projectRef.Name;
                    quoteInfo.QuoteNumber = quoteNumber;
                    quoteInfo.LicenceNumber = licenceNumberValue;

                    result.Success = true;
                    result.Message = SuccessMessage;

                    quoteInfoResult.QuoteInfo = quoteInfo;
                }
                else
                {
                    result.Success = false;
                    result.Message = productNumber+ " Nolu Konut Bulunmamakta yada Uygun Satış Bilgisi Bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                eventLog = new EventLogHelper(service, "CompanyInternal");
                eventLog.Log("GetQuote", ex.Message, EventLogHelper.EventType.Exception);
                result.Message = ex.Message;
                result.Success = false;
            }

            quoteInfoResult.Result = result;
            returnValue = ser.Serialize(quoteInfoResult);
            return returnValue;
        }
    }
}
