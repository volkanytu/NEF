using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using NEF.Library.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Data.SqlClient;

namespace NEF.WebServices.Integration
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.


    public class Integration : IIntegration
    {
        public MsCrmResult GetQuote(string QuoteNumber)
        {
            MsCrmResult result = new MsCrmResult();
            QuoteDetail qd = new QuoteDetail();
            try
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "quotenumber";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(QuoteNumber);

                ConditionExpression con2 = new ConditionExpression();
                con2.AttributeName = "statuscode";
                con2.Operator = ConditionOperator.NotEqual;
                con2.Values.Add(7);//Düzeltilmiş olmayacak

                FilterExpression filter = new FilterExpression();
                filter.FilterOperator = LogicalOperator.And;
                filter.Conditions.Add(con1);
                filter.Conditions.Add(con2);

                QueryExpression Query = new QueryExpression("quote");
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.FilterOperator = LogicalOperator.And;
                Query.Criteria.Filters.Add(filter);
                EntityCollection Result = service.RetrieveMultiple(Query);
                if (Result.Entities.Count > 0)
                {
                    Entity q = Result.Entities[0];
                    Entity product = GetProductByQuoteId(service, q.Id);
                    GetCustomerByQuote(service, q, qd);
                    qd.SalesId = Result.Entities[0].Id.ToString();
                    qd.ApartmentIdentificationNumber = product.Contains("productnumber") ? product.Attributes["productnumber"].ToString() : string.Empty;
                    qd.Block = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
                    qd.ContractDate = q.Contains("new_contractdate") ? ((DateTime)q.Attributes["new_contractdate"]).ToLocalTime() : (DateTime?)null;
                    qd.CustomerName = q.Contains("customerid") ? ((EntityReference)q.Attributes["customerid"]).Name : string.Empty;
                    qd.HomeNumber = product.Contains("new_homenumber") ? product.Attributes["new_homenumber"].ToString() : string.Empty;
                    qd.PerQquareMeterAmount = product.Contains("new_persquaremeter") ? ((Money)product.Attributes["new_persquaremeter"]).Value.ToString("N2") : string.Empty;
                    qd.PrePaymentAmount = q.Contains("new_prepaymentamount") ? ((Money)q.Attributes["new_prepaymentamount"]).Value.ToString("N2") : string.Empty;
                    qd.ProjectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
                    qd.SalesConsultant = q.Contains("ownerid") ? ((EntityReference)q.Attributes["ownerid"]).Name : string.Empty;
                    qd.SalesConsultantEmail = GetSalesConsultantEmail(((EntityReference)q.Attributes["ownerid"]).Id, service);
                    qd.SalesProcessDate = q.Contains("new_salesprocessdate") ? ((DateTime)q.Attributes["new_salesprocessdate"]).ToLocalTime() : (DateTime?)null;
                    qd.TotalAmountLessFreight = q.Contains("totalamountlessfreight") ? ((Money)q.Attributes["totalamountlessfreight"]).Value.ToString("N2") : string.Empty;
                    qd.TransactionCurrency = q.Contains("transactioncurrencyid") ? ((EntityReference)q.Attributes["transactioncurrencyid"]).Name : string.Empty;
                    qd.SalesStatus = q.Contains("statuscode") ? GetOptionSetValue(1084, "statuscode", ((OptionSetValue)q.Attributes["statuscode"]).Value, service) : string.Empty;
                    qd.KonutStatus = product.Contains("statuscode") ? GetOptionSetValue(1024, "statuscode", ((OptionSetValue)product.Attributes["statuscode"]).Value, service) : string.Empty;
                    qd.HouseCrmLink = q.Contains("new_productid") ? (Globals.HouseCrmLink + ((EntityReference)q.Attributes["new_productid"]).Id.ToString()) : string.Empty;


                    ConditionExpression con3 = new ConditionExpression();
                    con3.AttributeName = "new_quoteid";
                    con3.Operator = ConditionOperator.Equal;
                    con3.Values.Add(q.Id);

                    ConditionExpression con4 = new ConditionExpression();
                    con4.AttributeName = "statecode";
                    con4.Operator = ConditionOperator.Equal;
                    con4.Values.Add(0);

                    FilterExpression filter2 = new FilterExpression();
                    filter2.FilterOperator = LogicalOperator.And;
                    filter2.Conditions.Add(con3);
                    filter2.Conditions.Add(con4);

                    QueryExpression Query2 = new QueryExpression("new_salescanceldetail");
                    Query2.ColumnSet = new ColumnSet(true);
                    Query2.Criteria.FilterOperator = LogicalOperator.And;
                    Query2.Criteria.Filters.Add(filter2);
                    EntityCollection Result2 = service.RetrieveMultiple(Query2);
                    if (Result2.Entities.Count > 0)
                    {
                        Entity r = Result2.Entities[0];
                        qd.CalcelReasonId = Result2.Entities[0].Id.ToString();
                        qd.SubCanceledReason = r.Contains("new_subcanceledreasonid") ? ((EntityReference)r.Attributes["new_subcanceledreasonid"]).Name : string.Empty;
                        qd.CanceledReason = r.Contains("new_canceledreasonid") ? ((EntityReference)r.Attributes["new_canceledreasonid"]).Name : string.Empty;
                        qd.CanceledDate = r.Contains("createdon") ? ((DateTime)r.Attributes["createdon"]).ToLocalTime() : (DateTime?)null;
                        qd.CanceledDescription = r.Contains("new_canceldescription") ? r.Attributes["new_canceldescription"].ToString() : string.Empty;
                        qd.ContractAccessOffice = r.Contains("new_contractaccessoffice") ? r.Attributes["new_contractaccessoffice"].ToString() : string.Empty;
                        qd.contractAccessDate = r.Contains("new_contractaccessdate") ? ((DateTime)r.Attributes["new_contractaccessdate"]).ToLocalTime() : (DateTime?)null;
                        qd.WageNumber = r.Contains("new_wagenumber") ? r.Attributes["new_wagenumber"].ToString() : string.Empty;
                        qd.ProtestDate = r.Contains("new_protestdate") ? ((DateTime)r.Attributes["new_protestdate"]).ToLocalTime() : (DateTime?)null;
                        qd.ContractCanceledOffice = r.Contains("new_contractcanceledoffice") ? r.Attributes["new_contractcanceledoffice"].ToString() : string.Empty;
                        qd.NotaryStatus = r.Contains("new_notarystatus") ? GetOptionsSetTextFromValue(service, "new_salescanceldetail", "new_notarystatus", ((OptionSetValue)r.Attributes["new_notarystatus"]).Value) : string.Empty;

                    }

                    result.Result = qd;
                    result.Success = true;
                    result.Message = "Satış Detayları Başarıyla Çekildi.";
                }
                else
                {
                    result.Success = false;
                    result.Message = QuoteNumber + " Numaralı Satış Bulunamadı";
                }

            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        private string GetSalesConsultantEmail(Guid systemUserId, IOrganizationService service)
        {
            Entity systemUser = service.Retrieve("systemuser", systemUserId, new ColumnSet("internalemailaddress"));
            if (systemUser.Contains("internalemailaddress"))
            {
                return systemUser.Attributes["internalemailaddress"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public MsCrmResult CancellationConfirmed(string QuoteNumber)
        {
            MsCrmResult result = new MsCrmResult();
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "quotenumber";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(QuoteNumber);

                ConditionExpression con2 = new ConditionExpression();
                con2.AttributeName = "statuscode";
                con2.Operator = ConditionOperator.NotEqual;
                con2.Values.Add(7);//Düzeltilmiş olmayacak

                ConditionExpression con3 = new ConditionExpression();
                con3.AttributeName = "statuscode";
                con3.Operator = ConditionOperator.Equal;
                con3.Values.Add(100000000);//iptal aktarıldı

                FilterExpression filter = new FilterExpression();
                filter.FilterOperator = LogicalOperator.And;
                filter.Conditions.Add(con1);
                filter.Conditions.Add(con2);
                filter.Conditions.Add(con3);

                QueryExpression Query = new QueryExpression("quote");
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.FilterOperator = LogicalOperator.And;
                Query.Criteria.Filters.Add(filter);
                EntityCollection Result = service.RetrieveMultiple(Query);
                if (Result.Entities.Count > 0)
                {
                    var quoteclose = new Entity("quoteclose");
                    quoteclose.Attributes.Add("quoteid", Result.Entities[0].ToEntityReference());
                    CloseQuoteRequest closeQuoteRequest = new CloseQuoteRequest()
                    {
                        QuoteClose = quoteclose,
                        Status = new OptionSetValue(6)
                    };
                    service.Execute(closeQuoteRequest);
                    /////////////////////////////////////
                    ConditionExpression con5 = new ConditionExpression();
                    con5.AttributeName = "new_quoteid";
                    con5.Operator = ConditionOperator.Equal;
                    con5.Values.Add(Result.Entities[0].Id);

                    ConditionExpression con4 = new ConditionExpression();
                    con4.AttributeName = "statecode";
                    con4.Operator = ConditionOperator.Equal;
                    con4.Values.Add(0);

                    FilterExpression filter2 = new FilterExpression();
                    filter2.FilterOperator = LogicalOperator.And;
                    filter2.Conditions.Add(con5);
                    filter2.Conditions.Add(con4);

                    QueryExpression Query2 = new QueryExpression("new_salescanceldetail");
                    Query2.ColumnSet = new ColumnSet(true);
                    Query2.Criteria.FilterOperator = LogicalOperator.And;
                    Query2.Criteria.Filters.Add(filter2);
                    EntityCollection Result2 = service.RetrieveMultiple(Query2);
                    if (Result2.Entities.Count > 0)
                    {

                        Entity r = Result2.Entities[0];
                        SetStateRequest setStateReq = new SetStateRequest();
                        setStateReq.EntityMoniker = new EntityReference("new_salescanceldetail", r.Id);

                        setStateReq.State = new OptionSetValue(1);
                        setStateReq.Status = new OptionSetValue(2);

                        SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
                    }

                    result.Success = true;
                    result.Message = "Satış Başarıyla İptal Edildi.";

                }
                else
                {
                    result.Success = false;
                    result.Message = QuoteNumber + " Numaralı İptale Aktarılan Satış Bulunamadı";
                }
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public MsCrmResult CancellationDenied(string QuoteNumber)
        {

            MsCrmResult result = new MsCrmResult();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "quotenumber";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(QuoteNumber);

                ConditionExpression con2 = new ConditionExpression();
                con2.AttributeName = "statuscode";
                con2.Operator = ConditionOperator.NotEqual;
                con2.Values.Add(7);//Düzeltilmiş olmayacak

                ConditionExpression con3 = new ConditionExpression();
                con3.AttributeName = "statuscode";
                con3.Operator = ConditionOperator.Equal;
                con3.Values.Add(100000000);//iptal aktarıldı

                FilterExpression filter = new FilterExpression();
                filter.FilterOperator = LogicalOperator.And;
                filter.Conditions.Add(con1);
                filter.Conditions.Add(con2);
                filter.Conditions.Add(con3);

                QueryExpression Query = new QueryExpression("quote");
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.FilterOperator = LogicalOperator.And;
                Query.Criteria.Filters.Add(filter);
                EntityCollection Result = service.RetrieveMultiple(Query);
                if (Result.Entities.Count > 0)
                {
                    if (Result.Entities[0].Attributes.Contains("new_prestatus"))
                    {
                        #region | QUERY UPDATE STATUS|
                        string sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   StatusCode=@StatusCode,
                                       ModifiedOn=GETUTCDATE()
                                    WHERE
	                                    QuoteId='{0}'";
                        #endregion

                        sda.ExecuteNonQuery(string.Format(sqlQuery, Result.Entities[0].Id.ToString()), new SqlParameter[] { new SqlParameter("StatusCode", (int)Result.Entities[0].Attributes["new_prestatus"]) });

                        /////////////////////////////////////
                        ConditionExpression con5 = new ConditionExpression();
                        con5.AttributeName = "new_quoteid";
                        con5.Operator = ConditionOperator.Equal;
                        con5.Values.Add(Result.Entities[0].Id);

                        ConditionExpression con4 = new ConditionExpression();
                        con4.AttributeName = "statecode";
                        con4.Operator = ConditionOperator.Equal;
                        con4.Values.Add(0);

                        FilterExpression filter2 = new FilterExpression();
                        filter2.FilterOperator = LogicalOperator.And;
                        filter2.Conditions.Add(con5);
                        filter2.Conditions.Add(con4);

                        QueryExpression Query2 = new QueryExpression("new_salescanceldetail");
                        Query2.ColumnSet = new ColumnSet(true);
                        Query2.Criteria.FilterOperator = LogicalOperator.And;
                        Query2.Criteria.Filters.Add(filter2);
                        EntityCollection Result2 = service.RetrieveMultiple(Query2);
                        if (Result2.Entities.Count > 0)
                        {

                            Entity r = Result2.Entities[0];
                            SetStateRequest setStateReq = new SetStateRequest();
                            setStateReq.EntityMoniker = new EntityReference("new_salescanceldetail", r.Id);

                            setStateReq.State = new OptionSetValue(1);
                            setStateReq.Status = new OptionSetValue(2);

                            SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
                        }

                        result.Success = true;
                        result.Message = "Satış Başarıyla İptal Edildi.";

                    }
                    else
                    {
                        result.Success = false;
                        result.Message = QuoteNumber + " Numaralı Satışın Eski Durumu Bulunamadı";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = QuoteNumber + " Numaralı İptale Aktarılan Satış Bulunamadı";
                }
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public MsCrmResult CancelledAndTransfered(string QuoteNumber)
        {

            MsCrmResult result = new MsCrmResult();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            try
            {
                IOrganizationService service = MSCRM.AdminOrgService;
                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "quotenumber";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(QuoteNumber);

                ConditionExpression con2 = new ConditionExpression();
                con2.AttributeName = "statuscode";
                con2.Operator = ConditionOperator.NotEqual;
                con2.Values.Add(7);//Düzeltilmiş olmayacak               

                FilterExpression filter = new FilterExpression();
                filter.FilterOperator = LogicalOperator.And;
                filter.Conditions.Add(con1);
                filter.Conditions.Add(con2);               

                QueryExpression Query = new QueryExpression("quote");
                Query.ColumnSet = new ColumnSet(true);
                Query.Criteria.FilterOperator = LogicalOperator.And;
                Query.Criteria.Filters.Add(filter);
                EntityCollection Result = service.RetrieveMultiple(Query);
                if (Result.Entities.Count > 0)
                {
                    #region | QUERY UPDATE STATUS|
                    string sqlQuery = @"UPDATE
	                                   Quote
                                    SET
	                                   StatusCode=@StatusCode,
                                       StateCode = 1,
                                       ModifiedOn=GETUTCDATE()
                                    WHERE
	                                    QuoteId='{0}'";
                    #endregion

                    sda.ExecuteNonQuery(string.Format(sqlQuery, Result.Entities[0].Id.ToString()), new SqlParameter[] { new SqlParameter("StatusCode", 100000000)});

                  
                }
                else
                {
                    result.Success = false;
                    result.Message = QuoteNumber + " Numaralı Satış Bulunamadı";
                }
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
       


        public Contact GetContactDetail(Guid contactId)
        {
            Contact returnValue = new Contact();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            MsCrmResultObject resultContact = ContactHelper.GetContactDetail(contactId, sda);

            if (resultContact.Success)
            {
                returnValue = (Contact)resultContact.ReturnObject;
            }
            return returnValue;

        }


        private void GetCustomerByQuote(IOrganizationService service, Entity quote, QuoteDetail qd)
        {
            if (quote.Contains("customerid") && ((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "contact")
            {
                Entity contact = service.Retrieve("contact", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                qd.Bank = contact.Contains("new_bankid") ? ((EntityReference)contact.Attributes["new_bankid"]).Name : string.Empty;
                qd.BankOffice = contact.Contains("new_bankoffice") ? contact.Attributes["new_bankoffice"].ToString() : string.Empty;
                qd.IBAN = contact.Contains("new_ibannumber") ? contact.Attributes["new_ibannumber"].ToString() : string.Empty;
                qd.CustomerAddress = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() + "/" : string.Empty;
                qd.CustomerAddress += contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                qd.CustomerAddress += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                qd.CustomerAddress += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;
                qd.EMail = contact.Contains("emailaddress1") ? contact.Attributes["emailaddress1"].ToString() : string.Empty;
                qd.MobilePhone = contact.Contains("mobilephone") ? contact.Attributes["mobilephone"].ToString() : string.Empty;
                qd.Phone = contact.Contains("telephone2") ? contact.Attributes["telephone2"].ToString() : string.Empty;
                qd.TcTaxNo = contact.Contains("new_tcidentitynumber") ? contact.Attributes["new_tcidentitynumber"].ToString() : string.Empty;

            }
            else if (quote.Contains("customerid") && ((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "account")
            {
                Entity account = service.Retrieve("account", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                qd.Bank = account.Contains("new_bankid") ? ((EntityReference)account.Attributes["new_bankid"]).Name : string.Empty;
                qd.BankOffice = account.Contains("new_bankoffice") ? account.Attributes["new_bankoffice"].ToString() : string.Empty;
                qd.IBAN = account.Contains("new_ibannumber") ? account.Attributes["new_ibannumber"].ToString() : string.Empty;
                qd.CustomerAddress = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() + "/" : string.Empty;
                qd.CustomerAddress += account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                qd.CustomerAddress += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                qd.CustomerAddress += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;
                qd.EMail = account.Contains("emailaddress1") ? account.Attributes["emailaddress1"].ToString() : string.Empty;
                qd.MobilePhone = account.Contains("telephone1") ? account.Attributes["telephone1"].ToString() : string.Empty;
                qd.Phone = string.Empty;
                qd.TcTaxNo = account.Contains("new_taxnumber") ? account.Attributes["new_taxnumber"].ToString() : string.Empty;


            }


        }

        private Entity GetProductByQuoteId(IOrganizationService service, Guid QuoteId)
        {
            Entity product = null;
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet("productid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                product = service.Retrieve("product", ((EntityReference)Result.Entities[0].Attributes["productid"]).Id, new ColumnSet(true));
            }
            return product;
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

        public List<Country> GetCountries()
        {
            List<Country> returnList = new List<Country>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT 
	                                C.new_countryId Id
	                                ,C.new_name Name
                                FROM
	                                new_country C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
								ORDER BY
									C.new_isdefault DESC,C.new_name ASC";
            #endregion

            DataTable dt = sda.getDataTable(query);

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET COUNTRIES |


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Country _country = new Country();
                    _country.CountryId = (Guid)dt.Rows[i]["Id"];
                    _country.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_country);
                }
                #endregion
            }


            sda.closeConnection();
            return returnList;
        }

        public List<City> GetCities(Guid countryId)
        {

            List<City> returnList = new List<City>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);


            #region | SQL QUERY |
            string query = @"SELECT 
	                                C.new_cityId Id
	                                ,C.new_name Name
                                FROM
	                                new_city C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.new_countryid = '{0}'
                                    ORDER BY
									C.new_isdefault DESC,C.new_name ASC";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(query, countryId));

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET CITIES |

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    City _city = new City();
                    _city.CityId = (Guid)dt.Rows[i]["Id"];
                    _city.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_city);
                }
                #endregion

            }


            sda.closeConnection();
            return returnList;
        }

        public List<Town> GetTowns(Guid cityId)
        {
            List<Town> returnList = new List<Town>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT 
	                                T.new_townId Id
	                                ,T.new_name Name
                                FROM
	                                new_town T WITH (NOLOCK)
                                WHERE
	                                T.StateCode = 0
	                                AND
	                                T.new_cityid = '{0}'";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(query, cityId));

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET TOWNS |


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Town _town = new Town();
                    _town.TownId = (Guid)dt.Rows[i]["Id"];
                    _town.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_town);
                }
                #endregion

            }


            return returnList;
        }

        public List<District> GetDistricts(Guid townId)
        {
            List<District> returnList = new List<District>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT 
	                                D.new_districtId Id
	                                ,D.new_name Name
                                FROM
	                                new_district D WITH (NOLOCK)
                                WHERE
	                                D.StateCode = 0
	                                AND
	                                D.new_townid = '{0}'";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(query, townId));

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET DISTRICTS |


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    District _district = new District();
                    _district.DistrictId = (Guid)dt.Rows[i]["Id"];
                    _district.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_district);
                }
                #endregion
            }

            return returnList;
        }

        public List<Nationality> GetNationalities()
        {
            List<Nationality> returnList = new List<Nationality>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT
	                                N.new_nationalityId Id
	                                ,N.new_name Name
                                FROM
	                                new_nationality N WITH (NOLOCK)
                                WHERE
	                                N.StateCode = 0
                                ORDER BY
	                                N.new_isdefault DESC, N.new_name ASC";
            #endregion

            DataTable dt = sda.getDataTable(query);

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET NATIONALITIES |


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Nationality _nationality = new Nationality();
                    _nationality.NationalityId = (Guid)dt.Rows[i]["Id"];
                    _nationality.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_nationality);
                }
                #endregion
            }
            return returnList;
        }
        public List<Participation> GetParticipations()
        {
            List<Participation> returnList = new List<Participation>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT
	                                P.new_sourceofparticipationId ParticipationId
	                                ,P.new_name Name
                                FROM
	                                new_sourceofparticipation P WITH (NOLOCK)
                                WHERE
	                                P.StateCode = 0";
            #endregion
            DataTable dt = sda.getDataTable(query);

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET PARTICIPATIONS |
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Participation _participation = new Participation();
                    _participation.ParticipationId = (Guid)dt.Rows[i]["ParticipationId"];
                    _participation.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_participation);
                }
                #endregion
            }

            return returnList;
        }
        public List<SubParticipation> GetSubParticipations(Guid participationId)
        {
            List<SubParticipation> returnList = new List<SubParticipation>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT
	                                P.new_subsourceofparticipationId SubParticipationId
	                                ,P.new_name Name
                                FROM
	                                new_subsourceofparticipation P WITH (NOLOCK)
                                WHERE
	                                P.new_participationsourceid = '{0}'
	                                AND
	                                P.StateCode = 0";
            #endregion
            DataTable dt = sda.getDataTable(string.Format(query, participationId));

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET SUB PARTICIPATIONS |
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SubParticipation _subParticipation = new SubParticipation();
                    _subParticipation.SubParticipationId = (Guid)dt.Rows[i]["SubParticipationId"];
                    _subParticipation.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_subParticipation);
                }
                #endregion
            }

            return returnList;
        }

        public List<StringMap> GetJobs()
        {
            return GetOptionSetValues(2, "new_jobtitle");
        }
        public List<StringMap> GetMaritalStatus()
        {
            return GetOptionSetValues(2, "familystatuscode");
        }
        private List<StringMap> GetOptionSetValues(int objectTypeCode, string attributeName)
        {

            List<StringMap> returnList = new List<StringMap>();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |
            string query = @"SELECT
	                                SM.Value Name
	                                ,SM.AttributeValue Value
                                FROM
	                                StringMap SM WITH (NOLOCK)
                                WHERE
	                                SM.ObjectTypeCode = {0}
	                                AND
	                                SM.AttributeName = '{1}'
                                ORDER BY
                                     SM.Value ASC";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(query, objectTypeCode, attributeName));

            if (dt != null && dt.Rows.Count > 0)
            {
                #region | GET VALUES |


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringMap _value = new StringMap();
                    _value.Value = (int)dt.Rows[i]["Value"];
                    _value.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                    returnList.Add(_value);
                }
                #endregion
            }

            return returnList;
        }

        private string GetOptionSetValue(int objectTypeCode, string attributeName, int attributeValue, IOrganizationService service)
        {

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "objecttypecode";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(objectTypeCode);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "attributename";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(attributeName);

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "attributevalue";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(attributeValue);


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("stringmap");
            Query.ColumnSet = new ColumnSet("value");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return (string)Result.Entities[0].Attributes["value"];
            }
            else
            {
                return string.Empty;
            }
        }
    }

}
