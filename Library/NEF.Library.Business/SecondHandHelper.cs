using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace NEF.Library.Business
{
    public static class SecondHandHelper
    {
        public static MsCrmResult UpdateSecondHandStatus(Guid quoteId, SecondHandStatuses status, Guid userId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_resalerecord");
                ent.Id = quoteId;
                ent["statuscode"] = new OptionSetValue((int)status);
                service.Update(ent);
                returnValue.Success = true;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetSecondHandDetail(Guid rentalid, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {

                #region | SQL QUERY |

                string sqlQuery = @"SELECT
									q.new_name,
									q.new_resalerecordId,
									q.new_accountid,
									q.new_accountidName,
									q.new_productid,
									q.new_productidName,
									q.new_contactid,
									q.new_contactidName,
									q.new_resalespreamount,
                                    q.new_commission,
									q.new_salesfee,
									q.TransactionCurrencyId,
									q.TransactionCurrencyIdName,
                                    q.new_prepaymentdate,
                                    q.new_prepaymenttype,
									q.OwnerId, 
									q.OwnerIdName,
									p.new_projectid,
									p.new_projectidName,
                                    p.new_paymentofhire,
                                    p.TransactionCurrencyId as pCurrencyId,
									p.TransactionCurrencyIdName as pCurrencyName,
                                    q.new_prepaymentdate,
                                    q.new_prepaymenttype,
                                    smStateCode.AttributeValue AS StateCode,
                                    smStateCode.Value AS StateValue,

                                    smStatusCode.AttributeValue AS StatusCode,
                                    smStatusCode.Value AS StatusValue,

									prodStatusCode.Value as pStatusName,
									prodStatusCode.AttributeValue as pStatusCode

                                FROM
                                new_resalerecord AS q (NOLOCK)
	                                JOIN
		                                StringMap AS smStateCode (NOLOCK)
			                                ON
			                                smStateCode.ObjectTypeCode=10087
			                                AND
			                                smStateCode.AttributeName='statecode'
			                                AND
			                                smStateCode.AttributeValue=q.StateCode
	                                JOIN
		                                StringMap AS smStatusCode (NOLOCK)
			                                ON
			                                smStatusCode.ObjectTypeCode=10087
			                                AND
			                                smStatusCode.AttributeName='statuscode'
			                                AND
			                                smStatusCode.AttributeValue=q.StatusCode
								     JOIN 
											Product as P 
											ON q.new_productid = p.ProductId
	                               LEFT JOIN
		                                StringMap AS prodStatusCode (NOLOCK)
			                                ON
			                                prodStatusCode.ObjectTypeCode=1024
			                                AND
			                                prodStatusCode.AttributeName='new_usedrentalandsalesstatus'
			                                AND
			                                prodStatusCode.AttributeValue=P.new_usedrentalandsalesstatus
                                WHERE
	                                q.new_resalerecordId=@secondhandid ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@secondhandid", rentalid) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    #region | FILL SECOND HAND INFO |
                    SecondHand _secondHand = new SecondHand();
                    _secondHand.SecondHandId = (Guid)dt.Rows[0]["new_resalerecordId"];
                    _secondHand.Name = dt.Rows[0]["new_name"].ToString();

                    if (dt.Rows[0]["new_salesfee"] != DBNull.Value)
                    {
                        _secondHand.SecondHandAmount = (decimal)dt.Rows[0]["new_salesfee"];
                        _secondHand.SecondHandAmountStr = ((decimal)dt.Rows[0]["new_salesfee"]).ToString("N2");
                    }
                    if (dt.Rows[0]["new_paymentofhire"] != DBNull.Value)
                    {
                        _secondHand.ProductAmount = (decimal)dt.Rows[0]["new_paymentofhire"];
                        _secondHand.ProductAmountStr = ((decimal)dt.Rows[0]["new_paymentofhire"]).ToString("N2");
                    }

                    if (dt.Rows[0]["new_commission"] != DBNull.Value)
                    {
                        _secondHand.CommissionAmount = (decimal)dt.Rows[0]["new_commission"];
                        _secondHand.CommissionAmountStr = ((decimal)dt.Rows[0]["new_commission"]).ToString("N2");
                    }
                    if (dt.Rows[0]["new_resalespreamount"] != DBNull.Value)
                    {
                        _secondHand.PrePayment = (decimal)dt.Rows[0]["new_resalespreamount"];
                        _secondHand.PrePaymentStr = ((decimal)dt.Rows[0]["new_resalespreamount"]).ToString("N2");
                    }
                    if (dt.Rows[0]["OwnerId"] != DBNull.Value)
                    {
                        _secondHand.Owner = new EntityReference() { Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString(), LogicalName = "systemuser" };
                    }

                    if (dt.Rows[0]["new_contactid"] != DBNull.Value)
                    {
                        _secondHand.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["new_contactid"], Name = dt.Rows[0]["new_contactidName"].ToString(), LogicalName = "contact" };
                    }
                    if (dt.Rows[0]["new_accountid"] != DBNull.Value)
                    {
                        _secondHand.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["new_accountid"], Name = dt.Rows[0]["new_accountidName"].ToString(), LogicalName = "account" };
                    }

                    if (dt.Rows[0]["TransactionCurrencyId"] != DBNull.Value)
                    {
                        _secondHand.Currency = new EntityReference() { Id = (Guid)dt.Rows[0]["TransactionCurrencyId"], Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(), LogicalName = "transactioncurrency" };
                    }
                    if (dt.Rows[0]["pCurrencyId"] != DBNull.Value)
                    {
                        _secondHand.pCurrency = new EntityReference() { Id = (Guid)dt.Rows[0]["pCurrencyId"], Name = dt.Rows[0]["pCurrencyName"].ToString(), LogicalName = "transactioncurrency" };
                    }
                    if (dt.Rows[0]["new_productid"] != DBNull.Value)
                    {
                        _secondHand.Product = new EntityReference() { Id = (Guid)dt.Rows[0]["new_productid"], Name = dt.Rows[0]["new_productidName"].ToString(), LogicalName = "product" };
                    }
                    if (dt.Rows[0]["new_projectid"] != DBNull.Value)
                    {
                        _secondHand.Project = new EntityReference() { Id = (Guid)dt.Rows[0]["new_projectid"], Name = dt.Rows[0]["new_projectidName"].ToString(), LogicalName = "new_project" };
                    }
                    if (dt.Rows[0]["new_prepaymentdate"] != DBNull.Value)
                    {
                        _secondHand.PrePaymentDate = (DateTime)dt.Rows[0]["new_prepaymentdate"];
                        _secondHand.PrePaymentDateStr = ((DateTime)dt.Rows[0]["new_prepaymentdate"]).ToLocalTime().ToShortDateString();
                    }
                    if (dt.Rows[0]["new_prepaymenttype"] != DBNull.Value)
                    {
                        _secondHand.PrePaymentType = (int)dt.Rows[0]["new_prepaymenttype"];
                    }
                    _secondHand.StateCode = new StringMap() { Name = dt.Rows[0]["StateValue"].ToString(), Value = (int)dt.Rows[0]["StateCode"] };
                    _secondHand.StatusCode = new StringMap() { Name = dt.Rows[0]["StatusValue"].ToString(), Value = (int)dt.Rows[0]["StatusCode"] };
                    _secondHand.pStatusCode = new StringMap() { Name = dt.Rows[0]["pStatusName"].ToString(), Value = (int)dt.Rows[0]["pStatusCode"] };
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _secondHand;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetSecondHandProducts(Guid secondHandId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
	                                ,P.new_homenumber HomeNumber	
                                FROM
	                                new_resalerecord QD WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                QD.new_resalerecordId = '{0}'
	                                AND
	                                P.ProductId = QD.new_productid";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, secondHandId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET QUOTE PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);
                        returnList.Add(_product);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "2. El şatışa ait ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult SendMailSecondHandToApproval(Product secondHandProduct, Entity _secondHand, UserTypes type, SqlDataAccess sda, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {

                #region | SEND INFORMATIONS |
                string projectName = secondHandProduct.Project != null ? secondHandProduct.Project.Name : string.Empty;
                string blockName = secondHandProduct.Block != null ? secondHandProduct.Block.Name : string.Empty;
                string floorNumber = secondHandProduct.FloorNumber != null ? secondHandProduct.FloorNumber.ToString() : string.Empty;
                string generalhomeType = secondHandProduct.GeneralHomeType != null ? secondHandProduct.GeneralHomeType.Name : string.Empty;
                string homeType = secondHandProduct.HomeType != null ? secondHandProduct.HomeType.Name : string.Empty;
                string net = secondHandProduct.Net != null ? ((decimal)secondHandProduct.Net).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                string brut = secondHandProduct.Brut != null ? ((decimal)secondHandProduct.Brut).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                string productAmount = secondHandProduct.PaymentOfHire.HasValue ? secondHandProduct.PaymentOfHire.Value.ToString("N2") : string.Empty;
                string rentalAmount = _secondHand.GetAttributeValue<Money>("new_salesfee") != null ? _secondHand.GetAttributeValue<Money>("new_salesfee").Value.ToString("N2") : string.Empty;

                string currencyName = _secondHand.GetAttributeValue<EntityReference>("transactioncurrencyid") != null ? (_secondHand.GetAttributeValue<EntityReference>("transactioncurrencyid")).Name : string.Empty;
                #endregion

                #region | GET CURRENCY |
                string exchangeRate = string.Empty;
                Guid currencyId = (_secondHand.GetAttributeValue<EntityReference>("transactioncurrencyid")).Id;
                MsCrmResultObject currencyResult = CurrencyHelper.GetExchangeRateByCurrency(DateTime.Now, currencyId, sda);
                if (currencyResult.Success)
                {
                    ExchangeRate rate = (ExchangeRate)currencyResult.ReturnObject;
                    exchangeRate = ((decimal)rate.SaleRate).ToString("N0", CultureInfo.CurrentCulture);
                }
                #endregion

                string body = "<table>";
                body += "<tr><td>Proje : </td><td>" + projectName + "</td></tr>";
                body += "<tr><td>Blok : </td><td>" + blockName + "</td></tr>";
                body += "<tr><td>Kat : </td><td>" + floorNumber + "</td></tr>";
                body += "<tr><td>Daire No : </td><td>" + secondHandProduct.HomeNumber + "</td></tr>";
                body += "<tr><td>Tip : </td><td>" + generalhomeType + "</td></tr>";
                body += "<tr><td>Daire Tipi : </td><td>" + homeType + "</td></tr>";
                body += "<tr><td>Konut Satış Fiyatı Fiyatı : </td><td>" + productAmount + "</td></tr>";
                body += "<tr><td>Satılmak İstenen Fiyat : </td><td>" + rentalAmount + "</td></tr>";
                body += "<tr><td>Net m2 : </td><td>" + net + "</td></tr>";
                body += "<tr><td>Brüt m2 : </td><td>" + brut + "</td></tr>";
                body += "<tr><td>Para Birimi : </td><td>" + currencyName + "</td></tr>";
                body += "<tr><td>Güncel Kur : </td><td>" + exchangeRate + "</td></tr>";
                body += "</table>";
                body += "<br/>";
                body += "<br/>";
                body += "<a href='{0}' target='_blank'>Kiralamayı onaylamak/reddetmek için lütfen tıklayınız.</a>";

                string url = "http://fenix.centralproperty.com.tr/index.aspx?page=secondhandconfirm&name=secondhandid&pageid=" + _secondHand.Id;
                body = string.Format(body, url);

                //MsCrmResultObject managerResult = SystemUserHelper.GetSalesManager(sda);
                MsCrmResultObject managerResult = SystemUserHelper.GetUsersByUserTypes(type, sda);

                if (managerResult != null && managerResult.Success)
                {
                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = _secondHand.GetAttributeValue<EntityReference>("ownerid");
                    Entity[] fromPartyColl = new Entity[] { fromParty };

                    #region | SET TO |

                    List<SystemUser> returnList = (List<SystemUser>)managerResult.ReturnObject;
                    Entity[] toPartyColl = new Entity[returnList.Count];
                    for (int i = 0; i < returnList.Count; i++)
                    {
                        Entity toParty = new Entity("activityparty");
                        toParty["partyid"] = new EntityReference("systemuser", returnList[i].SystemUserId);
                        toPartyColl[i] = toParty;
                    }
                    #endregion
                    Annotation anno = null;
                    MsCrmResult mailResult = GeneralHelper.SendMail(_secondHand.Id, "new_resalerecord", fromPartyColl, toPartyColl, "2.El Satış Onayı", body, anno, service);
                    returnValue = mailResult;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = managerResult.Result;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetCustomerSecondHands(Guid? contactid, Guid? accountid, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                Q.new_resalerecordId Id
	                                ,SM.Value Status
                                FROM
	                                new_resalerecord Q WITH (NOLOCK)
                                INNER JOIN
	                                StringMap SM WITH (NOLOCK)
	                                ON
	                                {0}
	                                AND
	                                SM.ObjectTypeCode = 10087
	                                AND
	                                SM.AttributeName = 'statuscode'
	                                AND
	                                SM.AttributeValue = Q.StatusCode     
                                    ORDER BY
                                    Q.CreatedOn DESC";
                #endregion
                string customerId = string.Empty;
                if (accountid.HasValue)
                {
                    customerId = string.Format("Q.new_accountid = '{0}'", accountid.Value.ToString());
                }
                if (contactid.HasValue)
                {
                    customerId = string.Format("Q.new_contactid = '{0}'", contactid.Value.ToString());
                }

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET SECOND HAND |
                    List<SecondHand> returnList = new List<SecondHand>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MsCrmResultObject rentalResult = SecondHandHelper.GetSecondHandDetail((Guid)dt.Rows[i]["Id"], sda);

                        if (rentalResult.Success)
                        {
                            SecondHand _secondHand = (SecondHand)rentalResult.ReturnObject;

                            returnList.Add(_secondHand);
                        }
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteriye ait Kiralama Kaydı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult SendToApproval(Guid secondHandId, IOrganizationService service)
        {
            MsCrmResult resultVal = new MsCrmResult();
            try
            {
                Entity rental = service.Retrieve("new_resalerecord", secondHandId, new Microsoft.Xrm.Sdk.Query.ColumnSet("new_issendingapproval"));
                if (rental != null)
                {
                    bool approvalVal = rental.GetAttributeValue<bool>("new_issendingapproval");
                    if (approvalVal != true)
                    {
                        Entity ent = new Entity("new_resalerecord");
                        ent.Id = secondHandId;
                        ent["new_issendingapproval"] = true;
                        service.Update(ent);

                        resultVal.CrmId = rental.Id;
                        resultVal.Success = true;
                        resultVal.Result = "Kayıt onaya gönderildi.";

                    }
                    else
                    {
                        resultVal.CrmId = rental.Id;
                        resultVal.Success = false;
                        resultVal.Result = "Kayıt daha önceden onaya gönderilmiştir.";
                    }
                }

            }
            catch (Exception ex)
            {
                resultVal.CrmId = secondHandId;
                resultVal.Success = false;
                resultVal.Result = ex.Message;
                throw;
            }
            return resultVal;
        }

        public static MsCrmResult UpdateOrCreateSecondHand(SecondHand _secondHand, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_resalerecord");
                ent["new_name"] = _secondHand.Name;
                if (_secondHand.Contact != null)
                {
                    ent["new_contactid"] = _secondHand.Contact;
                }
                if (_secondHand.Account != null)
                {
                    ent["new_accountid"] = _secondHand.Account;
                }
                if (_secondHand.Owner != null)
                {
                    ent["ownerid"] = _secondHand.Owner;
                }
                if (_secondHand.Product != null)
                {
                    ent["new_productid"] = _secondHand.Product;
                }

                if (_secondHand.SecondHandAmount != null)
                {
                    ent["new_salesfee"] = new Money(_secondHand.SecondHandAmount.Value);
                }
                if (_secondHand.Currency != null)
                {
                    ent["transactioncurrencyid"] = _secondHand.Currency;
                }
                if (_secondHand.CommissionAmount != null)
                {
                    ent["new_commission"] = new Money(_secondHand.CommissionAmount.Value);
                }



                if (_secondHand.Currency != null)
                {
                    ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", _secondHand.Currency.Id);
                }

                if (_secondHand.Currency != null)
                {
                    ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", _secondHand.Currency.Id);
                }


                if (_secondHand.StatusCode != null)
                {
                    if (_secondHand.StatusCode.Value == (int)SecondHandStatuses.Onaylandi)
                    {
                        ent["new_prepaymentdate"] = DateTime.Now;
                        if (_secondHand.PrePayment.HasValue)
                        {
                            ent["new_resalespreamount"] = new Money(_secondHand.PrePayment.Value);
                        }
                        if (_secondHand.PrePaymentType.HasValue)
                        {
                            ent["new_prepaymenttype"] = new OptionSetValue(_secondHand.PrePaymentType.Value);
                        }
                    }
                }

                if (_secondHand.SecondHandId.HasValue)
                {
                    ent.Id = _secondHand.SecondHandId.Value;
                    service.Update(ent);
                    returnValue.CrmId = _secondHand.SecondHandId.Value;
                    returnValue.Success = true;
                    returnValue.Result = "2.El Satış kaydı başarıyla güncelleştirildi.";
                }
                else
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "2.El Satış kaydı başarıyla oluşturuldu.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult CreateOrUpdateAuthorityDocument(AuthorityDocument document, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_registrationdoc");

                if (document.Product != null)
                {
                    ent["new_productid"] = document.Product;
                }
                if (document.Contact != null)
                {
                    ent["new_authorizingpersonid"] = document.Contact;
                }
                if (document.StartDate != null)
                {
                    ent["new_startofauthority"] = document.StartDate;
                }
                if (document.EndDate != null)
                {
                    ent["new_endofauthority"] = document.EndDate;
                }
                if (document.Name != null)
                {
                    ent["new_name"] = document.Name;
                }
                ent["new_isimportauthoritydoc"] = true;

                if (document.AuthorityDocumentId.HasValue)
                {
                    ent.Id = document.AuthorityDocumentId.Value;
                    returnValue.CrmId = document.AuthorityDocumentId.Value;
                    service.Update(ent);
                    ProductHelper.UpdateProductAuthorityDoc(document, service);
                    returnValue.Success = true;
                    returnValue.Result = "Yetki Doküman kaydı başarıyla güncelleştirildi.";
                }
                else
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Yetki Doküman kaydı başarıyla oluşturuldu.";
                }

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject MakeAuthorityDocSearch(Guid? projectId, DateTime? startDate, DateTime? endDate, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                SqlParameter[] parameters = null;
                #region | SQL QUERY |
                string query = @"SELECT 
									R.new_registrationdocId
                                FROM
	                                new_registrationdoc R WITH (NOLOCK)
									JOIN Product as P WITH(NOLOCK)
									ON 
									P.ProductId = R.new_productid
                                WHERE
	                                P.StateCode = 0";

                if (projectId.HasValue)
                {
                    query += @"	AND
	                            P.new_projectid = '{0}'";
                    query = string.Format(query, projectId.Value);
                }
                else
                {
                    OrganizationServiceContext orgServiceContext = new OrganizationServiceContext(service);

                    var linqQuery = (from a in orgServiceContext.CreateQuery("new_project")
                                     where
                                     ((OptionSetValue)a["statecode"]).Value == 0
                                     select new
                                     {
                                         Id = a.Id
                                     }).ToList();
                    if (linqQuery != null && linqQuery.Count > 0)
                    {
                        query += @"	AND
	                            P.new_projectid IN(";

                        for (int i = 0; i < linqQuery.Count; i++)
                        {
                            if (i != linqQuery.Count - 1)
                            {
                                query += "'" + linqQuery[i].Id + "',";
                            }
                            else
                            {
                                query += "'" + linqQuery[i].Id + "'";
                            }
                        }
                        query += ")";
                    }

                }

                if (startDate.HasValue)
                {
                    query += @"	AND
	                                    R.new_startofauthority >= @minValue";
                    parameters = new SqlParameter[] { new SqlParameter("@minValue", startDate.Value) };
                }
                else if (endDate.HasValue)
                {
                    query += @"	AND
	                                    R.new_endofauthority <= @maxValue";
                    parameters = new SqlParameter[] { new SqlParameter("@maxValue", endDate.Value) };
                }

                #endregion

                DataTable dt = null;
                if (parameters == null)
                {
                    dt = sda.getDataTable(query);
                }
                else
                {
                    dt = sda.getDataTable(query, parameters);
                }


                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<AuthorityDocument> returnList = new List<AuthorityDocument>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        AuthorityDocument doc = (AuthorityDocument)GetAuthorityDocument((Guid)dt.Rows[i]["new_registrationdocId"], sda).ReturnObject;
                        returnList.Add(doc);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere ait konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        private static MsCrmResultObject GetAuthorityDocument(Guid authorityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {

                #region | SQL QUERY |

                string sqlQuery = @"SELECT 
                                        R.new_registrationdocId,
									    R.new_authorizingpersonid,
									    R.new_authorizingpersonidName,
									    P.ProductId,
									    P.Name,
									    R.new_startofauthority,
									    R.new_endofauthority,
									    R.new_name,
									    P.new_projectid,
									    P.new_projectidName,
									    P.new_blockidName,
									    P.new_homenumber
									FROM
									    new_registrationdoc AS R WITH(NOLOCK)
									JOIN Product AS P WITH(NOLOCK)
									    ON P.ProductId = R.new_productid
									WHERE
									    R.new_registrationdocId = @docid";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@docid", authorityId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    #region | FILL DOCUMENT INFO |
                    AuthorityDocument _document = new AuthorityDocument();
                    _document.AuthorityDocumentId = (Guid)dt.Rows[0]["new_registrationdocId"];
                    _document.Name = dt.Rows[0]["new_name"].ToString();

                    if (dt.Rows[0]["new_name"] != DBNull.Value)
                    {
                        _document.Name = dt.Rows[0]["new_name"].ToString();
                    }
                    if (dt.Rows[0]["ProductId"] != DBNull.Value)
                    {
                        _document.Product = new EntityReference() { Id = (Guid)dt.Rows[0]["ProductId"], Name = dt.Rows[0]["Name"].ToString(), LogicalName = "product" };
                    }
                    if (dt.Rows[0]["new_projectid"] != DBNull.Value)
                    {
                        _document.Project = new EntityReference() { Id = (Guid)dt.Rows[0]["new_projectid"], Name = dt.Rows[0]["new_projectidName"].ToString(), LogicalName = "new_project" };
                    }

                    if (dt.Rows[0]["new_authorizingpersonid"] != DBNull.Value)
                    {
                        _document.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["new_authorizingpersonid"], Name = dt.Rows[0]["new_authorizingpersonidName"].ToString(), LogicalName = "contact" };
                    }
                    if (dt.Rows[0]["new_startofauthority"] != DBNull.Value)
                    {
                        _document.StartDate = (DateTime)dt.Rows[0]["new_startofauthority"];
                        _document.StartDateStr = ((DateTime)dt.Rows[0]["new_startofauthority"]).ToLocalTime().ToShortDateString();
                    }

                    if (dt.Rows[0]["new_endofauthority"] != DBNull.Value)
                    {
                        _document.EndDate = (DateTime)dt.Rows[0]["new_endofauthority"];
                        _document.EndDateStr = ((DateTime)dt.Rows[0]["new_endofauthority"]).ToLocalTime().ToShortDateString();
                    }


                    if (dt.Rows[0]["new_blockidName"] != DBNull.Value)
                    {
                        _document.BlockName = dt.Rows[0]["new_blockidName"].ToString();
                    }

                    if (dt.Rows[0]["new_homenumber"] != DBNull.Value)
                    {
                        _document.HomeNumber = dt.Rows[0]["new_homenumber"].ToString();
                    }

                    #endregion

                    returnValue.Success = true;
                    returnValue.Result = "Kayıt başarıyla alındı";
                    returnValue.ReturnObject = _document;
                }
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