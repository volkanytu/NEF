using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;

namespace NEF.WebServices.Common
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Common : ICommon
    {

        public string CloseQuoteRequest(string quoteId)
        {
            string result = "false";
            try
            {
                #region | Satış iptal |
                IOrganizationService service = MSCRM.AdminOrgService;

                //SetStateRequest setStateReq = new SetStateRequest();
                //setStateReq.EntityMoniker = new EntityReference("quote", new Guid(quoteId));

                //setStateReq.State = new OptionSetValue(2);
                //setStateReq.Status = new OptionSetValue(4);

                //SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);



                var quoteclose = new Entity("quoteclose");
                quoteclose.Attributes.Add("quoteid", new EntityReference("quote", new Guid(quoteId)));
                CloseQuoteRequest closeQuoteRequest = new CloseQuoteRequest()
                {
                    QuoteClose = quoteclose,
                    Status = new OptionSetValue(6)
                };
                service.Execute(closeQuoteRequest);
                #endregion | Satış iptal |

                #region | Satış iptal detayı kapama |
                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "new_quoteid";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(new Guid(quoteId));

                ConditionExpression con2 = new ConditionExpression();
                con2.AttributeName = "statecode";
                con2.Operator = ConditionOperator.Equal;
                con2.Values.Add(0);

                FilterExpression filter = new FilterExpression();
                filter.FilterOperator = LogicalOperator.And;
                filter.Conditions.Add(con1);
                filter.Conditions.Add(con2);

                QueryExpression Query = new QueryExpression("new_salescanceldetail");
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.FilterOperator = LogicalOperator.And;
                Query.Criteria.Filters.Add(filter);
                EntityCollection Result = service.RetrieveMultiple(Query);
                if (Result.Entities.Count > 0)
                {
                    SetStateRequest setStateReq2 = new SetStateRequest();
                    setStateReq2.EntityMoniker = new EntityReference("new_salescanceldetail", Result.Entities[0].Id);

                    setStateReq2.State = new OptionSetValue(1);
                    setStateReq2.Status = new OptionSetValue(2);

                    service.Execute(setStateReq2);
                }
                #endregion | Satış iptal detayı kapama |


                result = "true";
            }
            catch (Exception)
            {
                result = "false";

            }
            return result;

        }
        public string Muhasebelestir(string quoteId)
        {
            string result = "false";
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                Entity quote = service.Retrieve("quote", new Guid(quoteId), new ColumnSet("customerid", "new_financialaccountid", "transactioncurrencyid"));
                EntityReference customer = quote.Attributes.Contains("customerid") ? (EntityReference)quote["customerid"] : null;
                EntityReference financialAccount = quote.Attributes.Contains("new_financialaccountid") ? (EntityReference)quote["new_financialaccountid"] : null;
                EntityReference currency = quote.Attributes.Contains("transactioncurrencyid") ? (EntityReference)quote["transactioncurrencyid"] : null;
                string customerName = customer.Name;

                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                #region | UPDATE FINANCIAL ACCOUNT CODE -VOLKAN 08.03.2015 |

                if (!string.IsNullOrEmpty(customerName) && financialAccount != null && !financialAccount.Name.Contains("329") && currency != null)
                {
                    char character = customerName[0];
                    MsCrmResultObject fAccountResult = FinancialAccountHelper.GetFinancialAccountNumberByCharacter(character, sda);
                    if (fAccountResult.Success)
                    {
                        int fAccountCode = (int)fAccountResult.ReturnObject;
                        fAccountCode++;

                        string fAccountCodeLeft = fAccountCode.ToString().PadLeft(5, '0');

                        Entity fAccount = new Entity("new_financialaccount");
                        fAccount["new_financialaccountid"] = financialAccount.Id;
                        fAccount["new_financialaccountnumber"] = fAccountCode;
                        fAccount["new_financialaccountcharacter"] = character + "";
                        fAccount["new_name"] = "329CA20" + (currency.Name == "TL" ? "TRL" : (currency.Name == "Euro" ? "EUR" : currency.Name)) + character + fAccountCodeLeft;
                        service.Update(fAccount);
                    }
                    else
                    {
                        throw new Exception(fAccountResult.Result);
                    }
                }
               

                #endregion

                #region | SET GROUP CODE |
                if (customer.LogicalName == "contact")
                {
                    if (!string.IsNullOrEmpty(customerName) && !ContactHelper.CheckContactHasGroupCode(customer.Id, sda).Success)
                    {
                        char character = customerName[0];
                        MsCrmResultObject groupCodeResult = ContactHelper.GetContactGroupCodeByCharacter(character, sda);
                        if (groupCodeResult.Success)
                        {
                            int groupCode = (int)groupCodeResult.ReturnObject;
                            groupCode++;
                            string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                            Entity entContact = new Entity("contact");
                            entContact["contactid"] = customer.Id;
                            entContact["new_groupcodenumber"] = groupCode;
                            entContact["new_groupcodecharacter"] = character + "";
                            entContact["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                            service.Update(entContact);
                        }
                        else
                        {
                            throw new Exception(groupCodeResult.Result);
                        }
                    }                   
                }

                if (customer.LogicalName == "account")
                {
                    if (!string.IsNullOrEmpty(customerName) && !AccountHelper.CheckAccountHasGroupCode(customer.Id, sda).Success)
                    {
                        char character = customerName[0];
                        MsCrmResultObject groupCodeResult = AccountHelper.GetAccountGroupCodeByCharacter(character, sda);
                        if (groupCodeResult.Success)
                        {
                            int groupCode = (int)groupCodeResult.ReturnObject;
                            groupCode++;
                            string groupCodeLeft = groupCode.ToString().PadLeft(4, '0');

                            Entity entAccount = new Entity("account");
                            entAccount["accountid"] = customer.Id;
                            entAccount["new_groupcodenumber"] = groupCode;
                            entAccount["new_groupcodecharacter"] = character + "";
                            entAccount["new_groupcode"] = "CRM" + "-" + character + groupCodeLeft;
                            service.Update(entAccount);
                        }
                        else
                        {
                            throw new Exception(groupCodeResult.Result);
                        }
                    }                    
                }
                #endregion

                #region | QUERY UPDATE STATUS|
                string sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   StatusCode=@StatusCode,
                                       ModifiedOn=GETUTCDATE()
                                    WHERE
	                                    QuoteId='{0}'";
                #endregion

                sda.ExecuteNonQuery(string.Format(sqlQuery, quoteId), new SqlParameter[] { new SqlParameter("StatusCode", (int)QuoteStatus.MuhasebeyeAktarıldı) });               

                #region | QUERY UPDATE LOGO TRANSFER |
                sqlQuery = string.Empty;
                sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   new_islogotransferred=@Islogotransferred
                                    WHERE
	                                    QuoteId='{0}'";
                #endregion

                sda.ExecuteNonQuery(string.Format(sqlQuery, quoteId), new SqlParameter[] { new SqlParameter("Islogotransferred", 1) });
                result = "true";
            }
            catch (Exception)
            {

                result = "false";
            }
            return result;
        }

        //Sözleşme İmzalandı
        public string ContractSigned(string quoteId)
        {
            string result = "false";
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                #region | QUERY UPDATE STATUS|
                string sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   StatusCode=@StatusCode,
                                       ModifiedOn=GETUTCDATE(),
                                       new_contractprocessdate=GETUTCDATE()
                                    WHERE
	                                    QuoteId='{0}'";
                #endregion

                sda.ExecuteNonQuery(string.Format(sqlQuery, quoteId), new SqlParameter[] { new SqlParameter("StatusCode", (int)QuoteStatus.Sözleşmeİmzalandı) });

                result = "true";
            }
            catch (Exception)
            {

                result = "false";
            }
            return result;

        }
        public string GetVoucher(string quoteId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            List<PrePayment> lstPayments = new List<PrePayment>();
            IOrganizationService service = MSCRM.AdminOrgService;
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(new Guid(quoteId));

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_isvoucher";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(true);

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "statecode";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(0);

            ConditionExpression con4 = new ConditionExpression();
            con4.AttributeName = "new_sign";
            con4.Operator = ConditionOperator.Equal;
            con4.Values.Add(false);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);
            filter.Conditions.Add(con4);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            foreach (Entity p in Result.Entities)
            {
                Entity currencyDetail = GetCurrencyDetail(((EntityReference)p["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" }, service);


                PrePayment _p = new PrePayment();
                _p.PaymentId = p.Id;
                _p.Amount = (((Money)p.Attributes["new_paymentamount"]).Value).ToString("N2") + " " + currencyDetail["currencysymbol"].ToString();
                _p.QuoteName = p.Contains("new_quoteid") ? ((EntityReference)p.Attributes["new_quoteid"]).Name : string.Empty;
                if (p.Contains("new_contactid"))
                {
                    _p.CustomerName = ((EntityReference)p.Attributes["new_contactid"]).Name;
                }
                else if (p.Contains("new_accountid"))
                {
                    _p.CustomerName = ((EntityReference)p.Attributes["new_accountid"]).Name;
                }
                _p.AmountDate = p.Contains("new_date") ? ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                if (p.Contains("new_itype"))
                {
                    _p.AmountType = new StringMap() { Name = GetOptionsSetTextFromValue(service, "new_payment", "new_itype", ((OptionSetValue)p.Attributes["new_itype"]).Value), Value = ((OptionSetValue)p.Attributes["new_itype"]).Value };//Ödeme Tipi
                }
                else
                {
                    _p.AmountType = new StringMap() { Name = string.Empty, Value = -1 };//Ödeme Tipi
                }
                if (p.Contains("new_type"))
                {
                    _p.VoucherType = new StringMap() { Name = GetOptionsSetTextFromValue(service, "new_payment", "new_type", ((OptionSetValue)p.Attributes["new_type"]).Value), Value = ((OptionSetValue)p.Attributes["new_type"]).Value };//Ödeme Türü
                }
                else
                {
                    _p.VoucherType = new StringMap() { Name = string.Empty, Value = -1 };//Ödeme Türü
                }



                lstPayments.Add(_p);
            }
            returnValue.Success = true;
            returnValue.ReturnObject = lstPayments;
            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);

            return json;
        }

        public string SingnedVoucher(string new_paymentid, string new_type, string new_itype)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                Entity p = new Entity("new_payment");
                p.Id = new Guid(new_paymentid);
                if (!new_type.Contains("? number:-1 ?"))
                {
                    p.Attributes["new_type"] = new OptionSetValue(Convert.ToInt32(new_type));
                }
                if (!new_itype.Contains("? number:-1 ?"))
                {
                    p.Attributes["new_itype"] = new OptionSetValue(Convert.ToInt32(new_itype));
                }
                p.Attributes["new_sign"] = true;
                service.Update(p);
                returnValue.Success = true;

            }
            catch (Exception ex)
            {

                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string json = jSer.Serialize(returnValue);
            return json;
        }

        private Entity GetCurrencyDetail(Guid id, string[] Columns, IOrganizationService service)
        {
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = "transactioncurrencyid";
            conditionExpression.Operator = ConditionOperator.Equal;
            conditionExpression.Values.Add((object)id);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            columnSet.AddColumns(Columns);
            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)service.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = (QueryBase)new QueryExpression()
                {
                    ColumnSet = columnSet,
                    Criteria = filterExpression,
                    EntityName = "transactioncurrency"
                }
            });
            if (multipleResponse.EntityCollection.Entities != null && multipleResponse.EntityCollection.Entities.Count > 0)
                return Enumerable.First<Entity>((IEnumerable<Entity>)multipleResponse.EntityCollection.Entities);
            else
                return (Entity)null;
        }
        public static OptionMetadata[] GetOptionsSetText(IOrganizationService service, string entityName, string attributeName)
        {
            RetrieveAttributeRequest request = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };

            RetrieveAttributeResponse response = (RetrieveAttributeResponse)service.Execute(request);

            PicklistAttributeMetadata metaData = (PicklistAttributeMetadata)response.AttributeMetadata;
            OptionMetadata[] optionList = metaData.OptionSet.Options.ToArray();

            return optionList;


        }

        public static string GetOptionsSetTextFromValue(IOrganizationService service, string entityName, string attributeName, int attributeValue)
        {
            return GetOptionsSetText(service, entityName, attributeName).Where(x => x.Value == attributeValue).First().Label.UserLocalizedLabel.Label;
        }
    }


}
