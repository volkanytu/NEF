using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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
    public static class RentalHelper
    {
        public static MsCrmResult UpdateRentalStatus(Guid quoteId, RentalStatuses status, Guid userId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_rentalrecord");
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

        public static MsCrmResultObject GetRentalDetail(Guid rentalid, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {

                #region | SQL QUERY |

                string sqlQuery = @"SELECT
									q.new_name,
									q.new_rentalrecordId,
									q.new_accountid,
									q.new_accountidName,
									q.new_productid,
									q.new_productidName,
									q.new_contactid,
									q.new_contactidName,
									q.new_guarantorid,
									q.new_guarantoridName,
                                    q.new_commission,
									q.new_rentalfee,
                                    q.new_deposit,
									q.TransactionCurrencyId,
									q.TransactionCurrencyIdName,
									q.new_contractenddate,
									q.new_contractstartdate,
									q.OwnerId, 
									q.OwnerIdName,
									p.new_projectid,
									p.new_projectidName,
                                    p.new_paymentofhire,
                                    p.TransactionCurrencyId as pCurrencyId,
									p.TransactionCurrencyIdName as pCurrencyName,
                                    
                                    smStateCode.AttributeValue AS StateCode,
                                    smStateCode.Value AS StateValue,

                                    smStatusCode.AttributeValue AS StatusCode,
                                    smStatusCode.Value AS StatusValue,

									prodStatusCode.Value as pStatusName,
									prodStatusCode.AttributeValue as pStatusCode,

                                    q.new_guarantor,
                                    q.new_guarantorphone

                                FROM
                                new_rentalrecord AS q (NOLOCK)
	                                JOIN
		                                StringMap AS smStateCode (NOLOCK)
			                                ON
			                                smStateCode.ObjectTypeCode=10086
			                                AND
			                                smStateCode.AttributeName='statecode'
			                                AND
			                                smStateCode.AttributeValue=q.StateCode
	                                JOIN
		                                StringMap AS smStatusCode (NOLOCK)
			                                ON
			                                smStatusCode.ObjectTypeCode=10086
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
	                                q.new_rentalrecordId=@rentalid ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@rentalid", rentalid) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    #region | FILL QUOTE INFO |
                    Rental _rental = new Rental();
                    _rental.RentalId = (Guid)dt.Rows[0]["new_rentalrecordId"];
                    _rental.Name = dt.Rows[0]["new_name"].ToString();

                    if (dt.Rows[0]["new_contractstartdate"] != DBNull.Value)
                    {
                        _rental.ContractStartDate = ((DateTime)dt.Rows[0]["new_contractstartdate"]).ToLocalTime();
                        _rental.ContractStartDateStr = _rental.ContractStartDate.Value.ToString("dd.MM.yyyy");
                    }
                    if (dt.Rows[0]["new_contractenddate"] != DBNull.Value)
                    {
                        _rental.ContractEndDate = ((DateTime)dt.Rows[0]["new_contractenddate"]).ToLocalTime();
                        _rental.ContractEndDateStr = _rental.ContractEndDate.Value.ToString("dd.MM.yyyy");
                    }
                    if (dt.Rows[0]["new_rentalfee"] != DBNull.Value)
                    {
                        _rental.RentalAmount = (decimal)dt.Rows[0]["new_rentalfee"];
                        _rental.RentalAmountStr = ((decimal)dt.Rows[0]["new_rentalfee"]).ToString("N2");
                    }
                    if (dt.Rows[0]["new_paymentofhire"] != DBNull.Value)
                    {
                        _rental.ProductAmount = (decimal)dt.Rows[0]["new_paymentofhire"];
                        _rental.ProductAmountStr = ((decimal)dt.Rows[0]["new_paymentofhire"]).ToString("N2");
                    }

                    if (dt.Rows[0]["new_commission"] != DBNull.Value)
                    {
                        _rental.CommissionAmount = (decimal)dt.Rows[0]["new_commission"];
                        _rental.CommissionAmountStr = ((decimal)dt.Rows[0]["new_commission"]).ToString("N2");
                    }
                    if (dt.Rows[0]["new_deposit"] != DBNull.Value)
                    {
                        _rental.DepositAmount = (decimal)dt.Rows[0]["new_deposit"];
                        _rental.DepositAmountStr = ((decimal)dt.Rows[0]["new_deposit"]).ToString("N2");
                    }
                    if (dt.Rows[0]["OwnerId"] != DBNull.Value)
                    {
                        _rental.Owner = new EntityReference() { Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString(), LogicalName = "systemuser" };
                    }
                    if (dt.Rows[0]["new_contactid"] != DBNull.Value)
                    {
                        _rental.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["new_contactid"], Name = dt.Rows[0]["new_contactidName"].ToString(), LogicalName = "contact" };
                    }
                    if (dt.Rows[0]["new_accountid"] != DBNull.Value)
                    {
                        _rental.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["new_accountid"], Name = dt.Rows[0]["new_accountidName"].ToString(), LogicalName = "account" };
                    }

                    if (dt.Rows[0]["TransactionCurrencyId"] != DBNull.Value)
                    {
                        _rental.Currency = new EntityReference() { Id = (Guid)dt.Rows[0]["TransactionCurrencyId"], Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(), LogicalName = "transactioncurrency" };
                    }
                    if (dt.Rows[0]["pCurrencyId"] != DBNull.Value)
                    {
                        _rental.pCurrency = new EntityReference() { Id = (Guid)dt.Rows[0]["pCurrencyId"], Name = dt.Rows[0]["pCurrencyName"].ToString(), LogicalName = "transactioncurrency" };
                    }
                    if (dt.Rows[0]["new_productid"] != DBNull.Value)
                    {
                        _rental.Product = new EntityReference() { Id = (Guid)dt.Rows[0]["new_productid"], Name = dt.Rows[0]["new_productidName"].ToString(), LogicalName = "product" };
                    }

                    if (dt.Rows[0]["new_projectid"] != DBNull.Value)
                    {
                        _rental.Project = new EntityReference() { Id = (Guid)dt.Rows[0]["new_projectid"], Name = dt.Rows[0]["new_projectidName"].ToString(), LogicalName = "new_project" };
                    }
                    if (dt.Rows[0]["new_guarantor"] != DBNull.Value)
                    {
                        _rental.GuarantorName = dt.Rows[0]["new_guarantor"].ToString();
                    }
                    if (dt.Rows[0]["new_guarantorphone"] != DBNull.Value)
                    {
                        _rental.GuarantorPhone = dt.Rows[0]["new_guarantorphone"].ToString();
                    }
                    _rental.StateCode = new StringMap() { Name = dt.Rows[0]["StateValue"].ToString(), Value = (int)dt.Rows[0]["StateCode"] };
                    _rental.StatusCode = new StringMap() { Name = dt.Rows[0]["StatusValue"].ToString(), Value = (int)dt.Rows[0]["StatusCode"] };
                    _rental.pStatusCode = new StringMap() { Name = dt.Rows[0]["pStatusName"].ToString(), Value = (int)dt.Rows[0]["pStatusCode"] };
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _rental;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }


            return returnValue;
        }

        public static MsCrmResultObject GetRentalProducts(Guid rentalId, SqlDataAccess sda)
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
	                                new_rentalrecord QD WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                QD.new_rentalrecordId = '{0}'
	                                AND
	                                P.ProductId = QD.new_productid";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, rentalId));

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
                    returnValue.Result = "Kiralamaya ait ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult SendMailRentalToApproval(Product rentalProduct, Entity _rental, UserTypes type, SqlDataAccess sda, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {

                #region | SEND INFORMATIONS |
                string projectName = rentalProduct.Project != null ? rentalProduct.Project.Name : string.Empty;
                string blockName = rentalProduct.Block != null ? rentalProduct.Block.Name : string.Empty;
                string floorNumber = rentalProduct.FloorNumber != null ? rentalProduct.FloorNumber.ToString() : string.Empty;
                string generalhomeType = rentalProduct.GeneralHomeType != null ? rentalProduct.GeneralHomeType.Name : string.Empty;
                string homeType = rentalProduct.HomeType != null ? rentalProduct.HomeType.Name : string.Empty;
                string net = rentalProduct.Net != null ? ((decimal)rentalProduct.Net).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                string brut = rentalProduct.Brut != null ? ((decimal)rentalProduct.Brut).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                string productAmount = rentalProduct.PaymentOfHire.HasValue ? rentalProduct.PaymentOfHire.Value.ToString("N2") : string.Empty;
                string rentalAmount = _rental.GetAttributeValue<Money>("new_rentalfee") != null ? _rental.GetAttributeValue<Money>("new_rentalfee").Value.ToString("N2") : string.Empty;

                string currencyName = _rental.GetAttributeValue<EntityReference>("transactioncurrencyid") != null ? (_rental.GetAttributeValue<EntityReference>("transactioncurrencyid")).Name : string.Empty;
                #endregion

                #region | GET CURRENCY |
                string exchangeRate = string.Empty;
                Guid currencyId = (_rental.GetAttributeValue<EntityReference>("transactioncurrencyid")).Id;
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
                body += "<tr><td>Daire No : </td><td>" + rentalProduct.HomeNumber + "</td></tr>";
                body += "<tr><td>Tip : </td><td>" + generalhomeType + "</td></tr>";
                body += "<tr><td>Daire Tipi : </td><td>" + homeType + "</td></tr>";
                body += "<tr><td>Konut Kiralama Fiyatı : </td><td>" + productAmount + "</td></tr>";
                body += "<tr><td>Kiralamak İstenen Fiyat : </td><td>" + rentalAmount + "</td></tr>";
                body += "<tr><td>Net m2 : </td><td>" + net + "</td></tr>";
                body += "<tr><td>Brüt m2 : </td><td>" + brut + "</td></tr>";
                body += "<tr><td>Para Birimi : </td><td>" + currencyName + "</td></tr>";
                body += "<tr><td>Güncel Kur : </td><td>" + exchangeRate + "</td></tr>";
                body += "</table>";
                body += "<br/>";
                body += "<br/>";
                body += "<a href='{0}' target='_blank'>Kiralamayı onaylamak/reddetmek için lütfen tıklayınız.</a>";

                string url = "http://fenix.centralproperty.com.tr/index.aspx?page=rentalconfirm&name=rentalid&pageid=" + _rental.Id;
                body = string.Format(body, url);

                //MsCrmResultObject managerResult = SystemUserHelper.GetSalesManager(sda);
                MsCrmResultObject managerResult = SystemUserHelper.GetUsersByUserTypes(type, sda);

                if (managerResult != null && managerResult.Success)
                {
                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = _rental.GetAttributeValue<EntityReference>("ownerid");
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
                    MsCrmResult mailResult = GeneralHelper.SendMail(_rental.Id, "new_rentalrecord", fromPartyColl, toPartyColl, "Kiralama Onayı", body, anno, service);
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

        public static MsCrmResultObject GetCustomerRentals(Guid? contactid, Guid? accountid, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                Q.new_rentalrecordId Id
	                                ,Q.new_contractenddate EndDate
									,Q.new_contractstartdate StartDate
	                                ,SM.Value Status
                                FROM
	                                new_rentalrecord Q WITH (NOLOCK)
                                INNER JOIN
	                                StringMap SM WITH (NOLOCK)
	                                ON
	                                {0}
	                                AND
	                                SM.ObjectTypeCode = 10086
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
                    #region | GET QUOTES |
                    List<Rental> returnList = new List<Rental>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MsCrmResultObject rentalResult = RentalHelper.GetRentalDetail((Guid)dt.Rows[i]["Id"], sda);

                        if (rentalResult.Success)
                        {
                            Rental _quote = (Rental)rentalResult.ReturnObject;

                            returnList.Add(_quote);
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

        public static MsCrmResult SendToApproval(Guid rentalId, IOrganizationService service)
        {
            MsCrmResult resultVal = new MsCrmResult();
            try
            {
                Entity rental = service.Retrieve("new_rentalrecord", rentalId, new Microsoft.Xrm.Sdk.Query.ColumnSet("new_issendingapproval"));
                if (rental != null)
                {
                    bool approvalVal = rental.GetAttributeValue<bool>("new_issendingapproval");
                    if (approvalVal != true)
                    {
                        Entity ent = new Entity("new_rentalrecord");
                        ent.Id = rentalId;
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
                resultVal.CrmId = rentalId;
                resultVal.Success = false;
                resultVal.Result = ex.Message;
                throw;
            }
            return resultVal;
        }

        public static MsCrmResult UpdateOrCreateRental(Rental _rental, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_rentalrecord");

                ent["new_name"] = _rental.Name;
                if (_rental.Contact != null)
                {
                    ent["new_contactid"] = _rental.Contact;
                }
                if (_rental.Account != null)
                {
                    ent["new_accountid"] = _rental.Account;
                }
                if (_rental.Owner != null)
                {
                    ent["ownerid"] = _rental.Owner;
                }
                if (_rental.Product != null)
                {
                    ent["new_productid"] = _rental.Product;
                }
                if (_rental.Quantor != null)
                {
                    ent["new_guarantorid"] = _rental.Quantor;
                }
                if (_rental.RentalAmount != null)
                {
                    ent["new_rentalfee"] = new Money(_rental.RentalAmount.Value);
                }
                if (_rental.Currency != null)
                {
                    ent["transactioncurrencyid"] = _rental.Currency;
                }
                if (_rental.ContractStartDate != null)
                {
                    ent["new_contractstartdate"] = _rental.ContractStartDate.Value;
                }
                if (_rental.ContractEndDate != null)
                {
                    ent["new_contractenddate"] = _rental.ContractEndDate.Value;
                }
                if (_rental.Currency != null)
                {
                    ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", _rental.Currency.Id);
                }
                if (_rental.CommissionAmount != null)
                {
                    ent["new_commission"] = new Money(_rental.CommissionAmount.Value);
                }
                if (_rental.DepositAmount != null)
                {
                    ent["new_deposit"] = new Money(_rental.DepositAmount.Value);
                }

                if (_rental.GuarantorName != null)
                {
                    ent["new_guarantor"] = _rental.GuarantorName;
                }
                if (_rental.GuarantorPhone != null)
                {
                    ent["new_guarantorphone"] = _rental.GuarantorPhone;
                }
                

                if (_rental.RentalId.HasValue)
                {
                    ent.Id = _rental.RentalId.Value;
                    service.Update(ent);
                    returnValue.CrmId = _rental.RentalId.Value;
                    returnValue.Success = true;
                    returnValue.Result = "Kiralama kaydı başarıyla güncelleştirildi.";
                }
                else
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Kiralama kaydı başarıyla oluşturuldu.";
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
