using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using System.Globalization;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using System.Web;
using System.IO;
using System.Data.OleDb;

namespace NEF.Library.Business
{
    public static class QuoteHelper
    {
        public static MsCrmResultObject GetCustomerQuotes(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                Q.QuoteId
	                                ,Q.new_salesprocessdate SalesDate
	                                ,SM.Value Status
                                FROM
	                                Quote Q WITH (NOLOCK)
                                INNER JOIN
	                                StringMap SM WITH (NOLOCK)
	                                ON
	                                Q.CustomerId = '{0}'
	                                AND
	                                SM.ObjectTypeCode = 1084
	                                AND
	                                SM.AttributeName = 'statuscode'
	                                AND
	                                SM.AttributeValue = Q.StatusCode
                                    AND
                                    Q.StatusCode!=7 --Düzeltilmiş değil 
                                ORDER BY
                                    Q.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET QUOTES |
                    List<Quote> returnList = new List<Quote>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MsCrmResultObject quoteResult = QuoteHelper.GetQuoteDetail((Guid)dt.Rows[i]["QuoteId"], sda);

                        if (quoteResult.Success)
                        {
                            Quote _quote = (Quote)quoteResult.ReturnObject;

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
                    returnValue.Result = "Müşteriye ait satış bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }



        public static MsCrmResult CustomerCheckApartmentOwner(Guid customerId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                Q.QuoteId
	                                ,Q.new_salesprocessdate SalesDate
	                                ,SM.Value Status
                                FROM
	                                Quote Q WITH (NOLOCK)
                                INNER JOIN
	                                StringMap SM WITH (NOLOCK)
	                                ON
	                                Q.CustomerId = '{0}'
	                                AND
	                                SM.ObjectTypeCode = 1084
	                                AND
	                                SM.AttributeName = 'statuscode'
	                                AND
	                                SM.AttributeValue = Q.StatusCode
                                    AND
                                    Q.StatusCode IN({1},{2})
                                ORDER BY
                                    Q.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, customerId, (int)QuoteStatus.MuhasebeyeAktarıldı, (int)QuoteStatus.Sözleşmeİmzalandı));

                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşteri bir daire sahibi değildir.!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetSystemUserQuotes(Guid systemUserId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                q.QuoteId
                                FROM
	                                Quote AS q (NOLOCK)
                                WHERE
	                                q.OwnerId=@ownerId AND q.StatusCode!=7 ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ownerId", systemUserId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    List<Quote> lst = new List<Quote>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MsCrmResultObject quoteResult = QuoteHelper.GetQuoteDetail((Guid)dt.Rows[i]["QuoteId"], sda);
                        if (quoteResult.Success)
                        {
                            Quote _quote = (Quote)quoteResult.ReturnObject;
                            lst.Add(_quote);
                        }
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lst;
                    returnValue.Result = "Kullanıcı satışları çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message + "-GetSystemUserQuotes";

            }

            return returnValue;
        }

        public static MsCrmResultObject GetQuoteDetail(Guid quoteId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {

                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                q.QuoteId
	                                ,q.QuoteNumber
	                                ,q.RevisionNumber
	                                ,q.Name
	                                ,q.new_salesprocessdate AS SalesDate
	                                ,q.TotalLineItemAmount
	                                ,q.TotalAmount
	                                ,q.StateCode
	                                ,q.StatusCode
	                                ,smStateCode.Value AS StateValue
	                                ,smStatusCode.Value AS StatusValue
	                                ,q.OpportunityId
                                    ,q.OpportunityIdName
	                                ,q.CustomerId
                                    ,q.CustomerIdName
                                    ,q.CustomerIdType
	                                ,q.new_paymentplan AS PaymentPlan
                                    ,q.TransactionCurrencyId AS CurrencyId
	                                ,q.TransactionCurrencyIdName AS CurrencyIdName
                                    ,q.CreatedOn
									,q.OwnerId
									,q.OwnerIdName
									,q.new_salestype SalesType
									,q.new_contractdate ContractDate
									,q.new_isrevisiononhome IsRevision
									,q.new_revisiondescription RevisionDescription
									,q.discountpercentage DiscountPercentage
									,q.discountamount DiscountPrice
									,q.TotalAmountLessFreight HouseDiscountPrice
									,q.new_prepaymentamount PrePaymentPrice
                                    ,q.new_prepaymentdate PrePaymentDate
									,q.new_suminterval SumInterval
									,q.new_instnumber InstNumber
									,q.new_instamount SumInstPrice
                                    ,q.new_paymentplan PaymentPlan
                                    ,q.new_paymentterm PaymentTerm
                                    ,q.new_paymentplandiscountrate PaymentPlanDiscountRate
                                    ,q.new_persquaremeter PerSquareMeterPrice
                                    ,q.new_approvaltype ApprovalType
                                    ,q.new_fallingapprovaltype FallingApprovalType
                                    ,q.new_usercomment UserComment
                                    ,q.new_hassecondcustomer AS HasSecondCustomer
                                    ,q.new_secondcontactid AS SecondContactId
                                    ,q.new_secondcontactidName AS SecondContactIdName
                                    ,q.new_referencecontactid AS ReferenceContactId
                                    ,q.new_referencecontactidName AS ReferenceContactIdName
                                    ,q.new_usagetype AS UsageType
									,(
										SELECT
											P.new_iswithisgyo 
										FROM
											new_project P WITH (NOLOCK)
										WHERE
											P.new_projectId = q.new_projectid
									)ProjectIsGyo
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                StringMap AS smStateCode (NOLOCK)
			                                ON
			                                smStateCode.ObjectTypeCode=1084
			                                AND
			                                smStateCode.AttributeName='statecode'
			                                AND
			                                smStateCode.AttributeValue=q.StateCode
	                                JOIN
		                                StringMap AS smStatusCode (NOLOCK)
			                                ON
			                                smStatusCode.ObjectTypeCode=1084
			                                AND
			                                smStatusCode.AttributeName='statuscode'
			                                AND
			                                smStatusCode.AttributeValue=q.StatusCode
                                WHERE
	                                q.QuoteId=@quoteId ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@quoteId", quoteId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    #region | FILL QUOTE INFO |
                    Quote _quote = new Quote();
                    _quote.QuoteId = (Guid)dt.Rows[0]["QuoteId"];
                    _quote.Name = dt.Rows[0]["Name"].ToString();
                    _quote.UserComment = dt.Rows[0]["UserComment"] != DBNull.Value ? dt.Rows[0]["UserComment"].ToString() : "";

                    _quote.CreatedOn = ((DateTime)dt.Rows[0]["CreatedOn"]).ToLocalTime();
                    _quote.CreatedOnString = dt.Rows[0]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[0]["CreatedOn"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    _quote.ContratDateString = dt.Rows[0]["ContractDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["ContractDate"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    _quote.SalesDateString = dt.Rows[0]["SalesDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["SalesDate"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    _quote.SalesDateStringHour = dt.Rows[0]["SalesDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["SalesDate"]).ToLocalTime().ToString("dd.MM.yyyy HH:mm") : "";


                    _quote.HouseListPriceString = dt.Rows[0]["TotalLineItemAmount"] != DBNull.Value ? ((decimal)dt.Rows[0]["TotalLineItemAmount"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.HouseSalePriceString = dt.Rows[0]["TotalAmount"] != DBNull.Value ? ((decimal)dt.Rows[0]["TotalAmount"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.DiscountPriceString = dt.Rows[0]["DiscountPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["DiscountPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.HouseDiscountPriceString = dt.Rows[0]["HouseDiscountPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["HouseDiscountPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.PrePaymentPriceString = dt.Rows[0]["PrePaymentPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["PrePaymentPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.PerSquareMeterPriceString = dt.Rows[0]["PerSquareMeterPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["PerSquareMeterPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.SumIntervalPriceString = dt.Rows[0]["SumInterval"] != DBNull.Value ? ((decimal)dt.Rows[0]["SumInterval"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    _quote.PaymentPlan = dt.Rows[0]["PaymentPlan"] != DBNull.Value ? (bool)dt.Rows[0]["PaymentPlan"] : false;
                    _quote.IsProjectGyo = dt.Rows[0]["ProjectIsGyo"] != DBNull.Value ? (bool)dt.Rows[0]["ProjectIsGyo"] : false;

                    _quote.SumInstPriceString = dt.Rows[0]["SumInstPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["SumInstPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";

                    _quote.RevisionDescription = dt.Rows[0]["RevisionDescription"] != DBNull.Value ? dt.Rows[0]["RevisionDescription"].ToString() : "";

                    _quote.RevisionNumber = (int)dt.Rows[0]["RevisionNumber"];
                    _quote.QuoteNumber = dt.Rows[0]["QuoteNumber"].ToString();

                    _quote.StateCode = new StringMap() { Name = dt.Rows[0]["StateValue"].ToString(), Value = (int)dt.Rows[0]["StateCode"] };
                    _quote.StatusCode = new StringMap() { Name = dt.Rows[0]["StatusValue"].ToString(), Value = (int)dt.Rows[0]["StatusCode"] };

                    if (dt.Rows[0]["PrePaymentDate"] != DBNull.Value)
                    {
                        _quote.PrePaymentDate = (DateTime)dt.Rows[0]["PrePaymentDate"];
                    }

                    if (dt.Rows[0]["SalesDate"] != DBNull.Value)
                    {
                        _quote.SalesDate = ((DateTime)dt.Rows[0]["SalesDate"]).ToLocalTime();

                    }

                    if (dt.Rows[0]["PerSquareMeterPrice"] != DBNull.Value)
                    {
                        _quote.PerSquareMeterPrice = (decimal)dt.Rows[0]["PerSquareMeterPrice"];
                    }

                    if (dt.Rows[0]["TotalLineItemAmount"] != DBNull.Value)
                    {
                        _quote.HouseListPrice = (decimal)dt.Rows[0]["TotalLineItemAmount"];
                    }

                    if (dt.Rows[0]["TotalAmount"] != DBNull.Value)
                    {
                        _quote.HouseSalePrice = (decimal)dt.Rows[0]["TotalAmount"];
                    }

                    if (dt.Rows[0]["OpportunityId"] != DBNull.Value)
                    {
                        _quote.Opportunity = new EntityReference() { Id = (Guid)dt.Rows[0]["OpportunityId"], Name = dt.Rows[0]["OpportunityIdName"].ToString(), LogicalName = "opportunity" };
                    }

                    if (dt.Rows[0]["CustomerId"] != DBNull.Value)
                    {
                        _quote.EntityType = dt.Rows[0]["CustomerIdType"].ToString();
                        if (_quote.EntityType == "2")
                        {
                            _quote.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["CustomerId"], Name = dt.Rows[0]["CustomerIdName"].ToString(), LogicalName = "contact" };
                        }
                        else
                        {
                            _quote.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["CustomerId"], Name = dt.Rows[0]["CustomerIdName"].ToString(), LogicalName = "account" };
                        }

                    }

                    if (dt.Rows[0]["PaymentPlan"] != DBNull.Value)
                    {
                        _quote.PaymentPlan = (bool)dt.Rows[0]["PaymentPlan"];
                    }

                    if (dt.Rows[0]["CurrencyId"] != DBNull.Value)
                    {
                        _quote.Currency = new EntityReference() { Id = (Guid)dt.Rows[0]["CurrencyId"], Name = dt.Rows[0]["CurrencyIdName"].ToString(), LogicalName = "transactioncurrency" };
                    }

                    if (dt.Rows[0]["OwnerId"] != DBNull.Value)
                    {
                        _quote.Owner = new EntityReference() { Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString(), LogicalName = "systemuser" };
                    }

                    if (dt.Rows[0]["SalesType"] != DBNull.Value)
                    {
                        _quote.SalesType = (SalesTypes)dt.Rows[0]["SalesType"];
                    }

                    if (dt.Rows[0]["ContractDate"] != DBNull.Value)
                    {
                        _quote.ContratDate = (DateTime)dt.Rows[0]["ContractDate"];
                    }

                    if (dt.Rows[0]["IsRevision"] != DBNull.Value)
                    {
                        _quote.IsRevision = (bool)dt.Rows[0]["IsRevision"];
                    }

                    if (dt.Rows[0]["DiscountPercentage"] != DBNull.Value)
                    {
                        _quote.DiscountPercentage = (decimal)dt.Rows[0]["DiscountPercentage"];
                    }

                    if (dt.Rows[0]["DiscountPrice"] != DBNull.Value)
                    {
                        _quote.DiscountPrice = (decimal)dt.Rows[0]["DiscountPrice"];
                    }

                    if (dt.Rows[0]["HouseDiscountPrice"] != DBNull.Value)
                    {
                        _quote.HouseDiscountPrice = (decimal)dt.Rows[0]["HouseDiscountPrice"];
                    }

                    if (dt.Rows[0]["PrePaymentPrice"] != DBNull.Value)
                    {
                        _quote.PrePaymentPrice = (decimal)dt.Rows[0]["PrePaymentPrice"];
                    }

                    if (dt.Rows[0]["SumInterval"] != DBNull.Value)
                    {
                        _quote.SumIntervalPrice = (decimal)dt.Rows[0]["SumInterval"];
                    }

                    if (dt.Rows[0]["InstNumber"] != DBNull.Value)
                    {
                        _quote.InstNumber = (int)dt.Rows[0]["InstNumber"];
                    }

                    if (dt.Rows[0]["SumInstPrice"] != DBNull.Value)
                    {
                        _quote.SumInstPrice = (decimal)dt.Rows[0]["SumInstPrice"];
                    }

                    if (dt.Rows[0]["PaymentTerm"] != DBNull.Value)
                    {
                        _quote.PaymentTerm = (int)dt.Rows[0]["PaymentTerm"];
                    }

                    if (dt.Rows[0]["PaymentPlanDiscountRate"] != DBNull.Value)
                    {
                        _quote.PaymentPlanDiscountRate = (decimal)dt.Rows[0]["PaymentPlanDiscountRate"];
                    }

                    if (dt.Rows[0]["ApprovalType"] != DBNull.Value)
                    {
                        _quote.ApprovalType = (FallingApprovalTypes)(int)dt.Rows[0]["ApprovalType"];
                    }

                    if (dt.Rows[0]["FallingApprovalType"] != DBNull.Value)
                    {
                        _quote.FallingApprovalType = (FallingApprovalTypes)(int)dt.Rows[0]["FallingApprovalType"];
                    }

                    if (dt.Rows[0]["HasSecondCustomer"] != DBNull.Value)
                    {
                        _quote.HasSecondCustomer = (bool)dt.Rows[0]["HasSecondCustomer"];
                    }

                    if (dt.Rows[0]["SecondContactId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference
                        {
                            Id = (Guid)dt.Rows[0]["SecondContactId"],
                            Name = dt.Rows[0]["SecondContactId"].ToString()
                        };

                        _quote.SecondCustomer = er;
                    }

                    if (dt.Rows[0]["ReferenceContactId"] != DBNull.Value)
                    {
                        _quote.ReferenceContact = new EntityReference() { Id = (Guid)dt.Rows[0]["ReferenceContactId"], Name = dt.Rows[0]["ReferenceContactIdName"].ToString(), LogicalName = "contact" };
                    }

                    if (dt.Rows[0]["UsageType"] != DBNull.Value)
                    {
                        _quote.UsageType = (int)dt.Rows[0]["UsageType"];
                    }

                    _quote.Annotation = QuoteHelper.GetQuoteAttachment(_quote.QuoteId, sda);

                    MsCrmResultObject resultProduct = QuoteHelper.GetQuoteProducts(_quote.QuoteId, sda);

                    if (resultProduct.Success)
                    {
                        _quote.Products = (List<Product>)resultProduct.ReturnObject;
                    }

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _quote;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }


            return returnValue;
        }

        public static MsCrmResultObject GetQuoteProducts(Guid quoteId, SqlDataAccess sda)
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
	                                QuoteDetail QD WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                QD.QuoteId = '{0}'
	                                AND
	                                P.ProductId = QD.ProductId";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, quoteId));

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
                    returnValue.Result = "Satışa ait ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetSalesQuoteProducts(Guid quoteId, SqlDataAccess sda)
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
	                                QuoteDetail QD WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                QD.QuoteId = '{0}'
	                                AND
	                                P.ProductId = QD.ProductId
                                    AND
                                    P.new_salesofprepaymentid = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, quoteId));

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
                    returnValue.Result = "Satışa ait ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetSalesRentalProducts(Guid rentalId, SqlDataAccess sda)
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

        public static MsCrmResult UpdateOrCreateQuote(Quote _quote, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("quote");

                if (_quote.Contact != null)
                {
                    ent["customerid"] = _quote.Contact;
                    ent["name"] = _quote.Contact.Name;
                }

                if (_quote.Owner != null)
                    ent["ownerid"] = _quote.Owner;

                if (_quote.Opportunity != null)
                    ent["opportunityid"] = _quote.Opportunity;

                if (_quote.ReferenceContact != null)
                    ent["new_referencecontactid"] = _quote.ReferenceContact;

                if (_quote.Products != null)
                    ent["name"] += " - " + _quote.Products[0].Name;

                if (_quote.ContratDate != null)
                    ent["new_contractdate"] = _quote.ContratDate;
                else
                    ent["new_contractdate"] = DateTime.Now;

                if (_quote.SalesDate != null)
                    ent["new_salesprocessdate"] = _quote.SalesDate;
                else
                    ent["new_salesprocessdate"] = DateTime.Now;

                if (_quote.SalesType != null && _quote.SalesType != SalesTypes.Bos)
                    ent["new_salestype"] = new OptionSetValue((int)_quote.SalesType);
                else
                    ent["new_salestype"] = new OptionSetValue((int)SalesTypes.Yeni);

                if (_quote.UsageType != null && _quote.UsageType != -1)
                    ent["new_usagetype"] = new OptionSetValue((int)_quote.UsageType);

                ent["new_paymentplan"] = _quote.PaymentPlan;

                ent["discountpercentage"] = _quote.DiscountPercentage;

                if (_quote.DiscountPrice != null)
                    ent["discountamount"] = new Money((decimal)_quote.DiscountPrice);
                else
                    ent["discountamount"] = null;

                if (_quote.Currency != null && _quote.Currency.Id != Guid.Empty)
                {
                    MsCrmResultObject resultPl = ProductHelper.GetPriceLevelByCurrencyId(_quote.Currency.Id, sda);

                    if (resultPl.Success)
                    {
                        PriceLevel pl = (PriceLevel)resultPl.ReturnObject;
                        ent["pricelevelid"] = new EntityReference("pricelevel", pl.Id);
                        ent["transactioncurrencyid"] = _quote.Currency;
                    }
                }

                if (_quote.PaymentTerm != null)
                {
                    ent["new_paymentterm"] = (int)_quote.PaymentTerm;
                }

                if (_quote.HasSecondCustomer != null)
                {
                    ent["new_hassecondcustomer"] = _quote.HasSecondCustomer;
                }

                if (_quote.PaymentPlanDiscountRate != null)
                    ent["new_paymentplandiscountrate"] = (decimal)_quote.PaymentPlanDiscountRate;
                else
                    ent["new_paymentplandiscountrate"] = null;


                if (_quote.Products != null && _quote.Products.Count > 0)
                {
                    decimal amountWithTax = 0;
                    if (_quote.Products[0].KdvRatio != null) // kdv Oranı
                    {
                        ent["new_taxrate"] = _quote.Products[0].KdvRatio;

                        if (_quote.HouseSalePrice != null && _quote.HouseSalePrice != 0)
                        {
                            amountWithTax = ((decimal)_quote.HouseSalePrice * (decimal)_quote.Products[0].KdvRatio) / 100;
                            ent["new_taxamount"] = new Money(amountWithTax);
                        }
                        else
                        {
                            if (_quote.Products[0].Price != null)
                            {
                                amountWithTax = ((decimal)_quote.Products[0].Price * (decimal)_quote.Products[0].KdvRatio) / 100;
                                ent["new_taxamount"] = new Money(amountWithTax);
                            }
                        }
                    }

                    if (amountWithTax != 0 && _quote.HouseSalePrice != null) // kdv dahil tutar
                    {
                        ent["new_amountwithtax"] = new Money(amountWithTax + (decimal)_quote.HouseSalePrice);
                    }

                    if (_quote.Products[0].TaxofStampRatio != null) // damga vergisi
                    {
                        ent["new_taxofstamp"] = _quote.Products[0].TaxofStampRatio;
                    }

                    //if (_quote.Products[0].PriceList != null) // fiyat listesi
                    //{
                    //    ent["pricelevelid"] = _quote.Products[0].PriceList;
                    //}
                }

                SystemUser systemUser = SystemUserHelper.GetSystemUserInfo(_quote.Owner.Id, sda);
                if (systemUser.BusinessUnitId != null)
                {
                    if (systemUser.BusinessUnitId.ToString().ToUpper() == Globals.AlternatifBusinessUnitId.ToString().ToUpper())
                    {
                        ent["new_cutomersalestype"] = new OptionSetValue((int)CustomerSalesType.Alternative);
                    }
                    else
                    {
                        ent["new_cutomersalestype"] = new OptionSetValue((int)CustomerSalesType.personal);
                    }
                }
                else
                {
                    ent["new_cutomersalestype"] = new OptionSetValue((int)CustomerSalesType.personal);
                }

                if (_quote.Retailer != null)
                {
                    ent["new_retailerid"] = new EntityReference("new_retailer", _quote.Retailer.Id);
                }

                if (_quote.QuoteId != Guid.Empty)
                {
                    #region | CURRRENCY UPDATE PROCESS |

                    if (_quote.Currency != null && _quote.Currency.Id != Guid.Empty)
                    {
                        MsCrmResultObject quoteResult = QuoteHelper.GetQuoteDetail(_quote.QuoteId, sda);

                        if (quoteResult.Success)
                        {
                            Quote qDet = (Quote)quoteResult.ReturnObject;

                            Entity quoteEntity = new Entity("quote");
                            quoteEntity.Id = _quote.QuoteId;
                            quoteEntity["transactioncurrencyid"] = _quote.Currency;


                            Entity quotePreEntity = new Entity("quote");
                            quotePreEntity.Id = _quote.QuoteId;
                            quotePreEntity["transactioncurrencyid"] = qDet.Currency;

                            if (qDet.PrePaymentDate != null)
                                quotePreEntity["new_prepaymentdate"] = (DateTime)qDet.PrePaymentDate;

                            if (qDet.PrePaymentPrice != null)
                                quotePreEntity["new_prepaymentamount"] = (decimal)qDet.PrePaymentPrice;

                            Entity resultEntity = QuoteHelper.ChangeTransactionCurrency(quoteEntity, quotePreEntity, service);

                            if (resultEntity != null)
                            {
                                ent["pricelevelid"] = resultEntity["pricelevelid"];

                                if (resultEntity.Contains("new_prepaymentamount") && resultEntity["new_prepaymentamount"] != null)
                                {
                                    ent["new_prepaymentamount"] = resultEntity["new_prepaymentamount"];
                                }
                            }
                            else
                            {
                                returnValue.Result = "Para birimi değişim işlemleri sırasında hata ile karılaşıldı.";

                                return returnValue;
                            }
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = quoteResult.Result;

                            return returnValue;
                        }
                    }

                    #endregion

                    ent["quoteid"] = _quote.QuoteId;
                    service.Update(ent);
                    returnValue.Result = "Satış başarıyla güncellendi.";
                }
                else
                {
                    if (_quote.Products[0].PriceList != null) // fiyat listesi
                    {
                        ent["pricelevelid"] = _quote.Products[0].PriceList;
                    }

                    returnValue.CrmId = service.Create(ent);
                    returnValue.Result = "Satış başarıyla oluşturuldu.";
                }


                returnValue.Success = true;

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        

        public static MsCrmResult CreateQuoteDetail(Guid quoteId, Product _proc, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("quotedetail");
                ent["productid"] = new EntityReference("product", _proc.ProductId);
                ent["quoteid"] = new EntityReference("quote", quoteId);
                ent["uomid"] = _proc.Uom;
                ent["quantity"] = new decimal(1);

                returnValue.CrmId = service.Create(ent);

                returnValue.Success = true;
                returnValue.Result = "Satış başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetQuotePayments(Guid quoteId, PaymentTypes paymentType, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string query = @"SELECT
	                                    P.new_paymentId Id
                                    FROM
	                                    new_payment P WITH (NOLOCK)
                                    WHERE
	                                    P.new_quoteid = '{0}'
                                        AND
                                        P.StateCode = 0";

                query = string.Format(query, quoteId);

                if (paymentType != PaymentTypes.Bos)
                {
                    query += " AND P.new_type = {0}";
                    query = string.Format(query, (int)paymentType);
                }
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt.Rows.Count > 0)
                {
                    List<Payment> lstPayments = new List<Payment>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Guid paymentId = (Guid)dt.Rows[i]["Id"];
                        MsCrmResultObject paymentResult = PaymentHelper.GetPaymentDetail((Guid)dt.Rows[i]["Id"], sda);

                        if (paymentResult.Success)
                        {
                            Payment _payment = (Payment)paymentResult.ReturnObject;
                            lstPayments.Add(_payment);
                        }
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lstPayments;
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message + "-GetSystemUserQuotes";

            }

            return returnValue;
        }

        public static MsCrmResult UpdateQuoteStatus(Guid quoteId, QuoteStatus status, Guid userId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("quote");
                ent["quoteid"] = quoteId;
                ent["statuscode"] = new OptionSetValue((int)status);

                switch (status)
                {
                    case QuoteStatus.DevamEdiyor:
                        break;
                    case QuoteStatus.Sözleşmeİmzalandı:
                        break;
                    case QuoteStatus.KaporaAlındı:
                        break;
                    case QuoteStatus.Kazanıldı:
                        break;
                    case QuoteStatus.Kaybedildi:
                        break;
                    case QuoteStatus.İptalEdildi:
                        break;
                    case QuoteStatus.Düzeltilmiş:
                        break;
                    case QuoteStatus.İptalAktarıldı:
                        break;
                    case QuoteStatus.MuhasebeyeAktarıldı:
                        break;
                    case QuoteStatus.TeslimEdildi:
                        break;
                    case QuoteStatus.BittiSatıldı:
                        break;
                    case QuoteStatus.OnayBekleniyor:
                        returnValue.Result = "Satış onaya gönderildi.";
                        break;
                    case QuoteStatus.Onaylandı:
                        ent["new_confirmuserid"] = new EntityReference("systemuser", userId);
                        ent["new_salesprocessdate"] = DateTime.Now;
                        returnValue.Result = "Satış onaylandı.";
                        break;
                    case QuoteStatus.Reddedildi:
                        returnValue.Result = "Satış reddedildi.";
                        break;
                    default:
                        break;
                }

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

        public static MsCrmResult SendMailQuoteToApproval(Guid quoteId, SqlDataAccess sda, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                MsCrmResultObject quoteResult = QuoteHelper.GetQuoteDetail(quoteId, sda);
                if (quoteResult.Success)
                {
                    Quote _quote = (Quote)quoteResult.ReturnObject;
                    if (_quote.Products != null && _quote.Products.Count > 0)
                    {
                        Product quoteProduct = _quote.Products[0];

                        #region | SEND INFORMATIONS |
                        string projectName = quoteProduct.Project != null ? quoteProduct.Project.Name : string.Empty;
                        string blockName = quoteProduct.Block != null ? quoteProduct.Block.Name : string.Empty;
                        string floorNumber = quoteProduct.FloorNumber != null ? quoteProduct.FloorNumber.ToString() : string.Empty;
                        string generalhomeType = quoteProduct.GeneralHomeType != null ? quoteProduct.GeneralHomeType.Name : string.Empty;
                        string homeType = quoteProduct.HomeType != null ? quoteProduct.HomeType.Name : string.Empty;
                        string net = quoteProduct.Net != null ? ((decimal)quoteProduct.Net).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                        string brut = quoteProduct.Brut != null ? ((decimal)quoteProduct.Brut).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                        string discountPercentage = _quote.DiscountPercentage != null ? ((decimal)_quote.DiscountPercentage).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                        string currencyName = _quote.Currency != null ? _quote.Currency.Name : string.Empty;
                        #endregion

                        #region | GET CURRENCY |
                        string exchangeRate = string.Empty;
                        MsCrmResultObject currencyResult = CurrencyHelper.GetExchangeRateByCurrency(DateTime.Now, _quote.Currency.Id, sda);
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
                        body += "<tr><td>Daire No : </td><td>" + quoteProduct.HomeNumber + "</td></tr>";
                        body += "<tr><td>Tip : </td><td>" + generalhomeType + "</td></tr>";
                        body += "<tr><td>Daire Tipi : </td><td>" + homeType + "</td></tr>";
                        body += "<tr><td>Liste Fiyatı : </td><td>" + _quote.HouseListPriceString + "</td></tr>";
                        body += "<tr><td>Net m2 : </td><td>" + net + "</td></tr>";
                        body += "<tr><td>Brüt m2 : </td><td>" + brut + "</td></tr>";
                        body += "<tr><td>Satış Fiyatı : </td><td>" + _quote.HouseSalePriceString + "</td></tr>";
                        body += "<tr><td>İndirim Oranı : </td><td>" + discountPercentage + "</td></tr>";
                        body += "<tr><td>M2 Birim Fiyatı : </td><td>" + _quote.PerSquareMeterPriceString + "</td></tr>";
                        body += "<tr><td>Para Birimi : </td><td>" + currencyName + "</td></tr>";
                        body += "<tr><td>Güncel Kur : </td><td>" + exchangeRate + "</td></tr>";
                        body += "<tr><td>Kullanıcı Notu : </td><td>" + _quote.UserComment + "</td></tr>";
                        body += "</table>";
                        body += "<br/>";
                        body += "<br/>";
                        body += "<a href='{0}' target='_blank'>Satışı onaylamak/reddetmek için lütfen tıklayınız.</a>";

                        string url = Globals.PortalUrl + "index.aspx?page=confirm&name=quoteid&pageid=" + quoteId;
                        body = string.Format(body, url);

                        Entity fromParty = new Entity("activityparty");
                        fromParty["partyid"] = _quote.Owner;
                        Entity[] fromPartyColl = new Entity[] { fromParty };

                        #region | SET TO |


                        List<Entity> toPartyColl = new List<Entity>();

                        Entity toParty = new Entity("activityparty");
                        toParty["partyid"] = new EntityReference("systemuser", Globals.AlternatifDirectorSystemUserId);
                        toPartyColl.Add(toParty);

                        #endregion

                        MsCrmResultObject attachmentResult = GeneralHelper.GetAttachmentByObjectId(quoteId, sda);
                        Annotation anno = null;
                        if (attachmentResult.Success)
                        {
                            anno = (Annotation)attachmentResult.ReturnObject;
                        }

                        MsCrmResult mailResult = GeneralHelper.SendMail(quoteId, "quote", fromPartyColl, toPartyColl.ToArray(), "Satış Onayı", body, anno, service);
                        returnValue = mailResult;


                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult SendMailQuoteToApproval(Guid quoteId, UserTypes type, SqlDataAccess sda, IOrganizationService service, bool isIsGyo = false)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                MsCrmResultObject quoteResult = QuoteHelper.GetQuoteDetail(quoteId, sda);
                if (quoteResult.Success)
                {
                    Quote _quote = (Quote)quoteResult.ReturnObject;
                    if (_quote.Products != null && _quote.Products.Count > 0)
                    {
                        Product quoteProduct = _quote.Products[0];

                        #region | SEND INFORMATIONS |
                        string projectName = quoteProduct.Project != null ? quoteProduct.Project.Name : string.Empty;
                        string blockName = quoteProduct.Block != null ? quoteProduct.Block.Name : string.Empty;
                        string floorNumber = quoteProduct.FloorNumber != null ? quoteProduct.FloorNumber.ToString() : string.Empty;
                        string generalhomeType = quoteProduct.GeneralHomeType != null ? quoteProduct.GeneralHomeType.Name : string.Empty;
                        string homeType = quoteProduct.HomeType != null ? quoteProduct.HomeType.Name : string.Empty;
                        string net = quoteProduct.Net != null ? ((decimal)quoteProduct.Net).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                        string brut = quoteProduct.Brut != null ? ((decimal)quoteProduct.Brut).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                        string discountPercentage = _quote.DiscountPercentage != null ? ((decimal)_quote.DiscountPercentage).ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
                        string currencyName = _quote.Currency != null ? _quote.Currency.Name : string.Empty;
                        #endregion

                        #region | GET CURRENCY |
                        string exchangeRate = string.Empty;
                        MsCrmResultObject currencyResult = CurrencyHelper.GetExchangeRateByCurrency(DateTime.Now, _quote.Currency.Id, sda);
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
                        body += "<tr><td>Daire No : </td><td>" + quoteProduct.HomeNumber + "</td></tr>";
                        body += "<tr><td>Tip : </td><td>" + generalhomeType + "</td></tr>";
                        body += "<tr><td>Daire Tipi : </td><td>" + homeType + "</td></tr>";
                        body += "<tr><td>Liste Fiyatı : </td><td>" + _quote.HouseListPriceString + "</td></tr>";
                        body += "<tr><td>Net m2 : </td><td>" + net + "</td></tr>";
                        body += "<tr><td>Brüt m2 : </td><td>" + brut + "</td></tr>";
                        body += "<tr><td>Satış Fiyatı : </td><td>" + _quote.HouseSalePriceString + "</td></tr>";
                        body += "<tr><td>İndirim Oranı : </td><td>" + discountPercentage + "</td></tr>";
                        body += "<tr><td>M2 Birim Fiyatı : </td><td>" + _quote.PerSquareMeterPriceString + "</td></tr>";
                        body += "<tr><td>Para Birimi : </td><td>" + currencyName + "</td></tr>";
                        body += "<tr><td>Güncel Kur : </td><td>" + exchangeRate + "</td></tr>";
                        body += "<tr><td>Kullanıcı Notu : </td><td>" + _quote.UserComment + "</td></tr>";
                        body += "</table>";
                        body += "<br/>";
                        body += "<br/>";
                        body += "<a href='{0}' target='_blank'>Satışı onaylamak/reddetmek için lütfen tıklayınız.</a>";

                        string url = Globals.PortalUrl + "index.aspx?page=confirm&name=quoteid&pageid=" + quoteId;
                        body = string.Format(body, url);

                        //MsCrmResultObject managerResult = SystemUserHelper.GetSalesManager(sda);
                        MsCrmResultObject managerResult = null;
                        if (isIsGyo) //Proje İş Gyo Projesi ise 
                        {
                            managerResult = SystemUserHelper.GetUsersByUserTypesWithIsGyo(type, sda);
                        }
                        else
                        {
                            managerResult = SystemUserHelper.GetUsersByUserTypes(type, sda);
                        }

                        if (managerResult != null && managerResult.Success)
                        {
                            Entity fromParty = new Entity("activityparty");
                            fromParty["partyid"] = _quote.Owner;
                            Entity[] fromPartyColl = new Entity[] { fromParty };

                            #region | SET TO |

                            List<SystemUser> returnList = (List<SystemUser>)managerResult.ReturnObject;
                            Entity[] toPartyColl = new Entity[returnList.Count];
                            for (int i = 0; i < returnList.Count; i++)
                            {
                                Entity toParty = new Entity("activityparty");
                                toParty["partyid"] = new EntityReference("systemuser", returnList[i].SystemUserId);
                                toPartyColl[i] = toParty;
                            }//ToDo: EmrahE mail ayarları
                            #endregion

                            MsCrmResultObject attachmentResult = GeneralHelper.GetAttachmentByObjectId(quoteId, sda);
                            Annotation anno = null;
                            if (attachmentResult.Success)
                            {
                                anno = (Annotation)attachmentResult.ReturnObject;
                            }

                            MsCrmResult mailResult = GeneralHelper.SendMail(quoteId, "quote", fromPartyColl, toPartyColl, "Satış Onayı", body, anno, service);
                            returnValue = mailResult;

                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Result = managerResult.Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult SendMailUserApprovalResult(Guid quoteId, Guid quoteOwnerId, string quoteOwnerName, DateTime createdOn, string customerName, string confirmUserName, DateTime salesDate, QuoteStatus status, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                string body = "";

                MsCrmResultObject productResult = QuoteHelper.GetQuoteProducts(quoteId, sda);
                if (productResult.Success)
                {
                    List<Product> products = (List<Product>)productResult.ReturnObject;

                    Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);

                    if (status == QuoteStatus.Onaylandı)
                    {
                        body += "<p>Sayın {0}</p>";
                        body += "<p>{1} tarihinde {2} müşterisinin {3} projesi {4} dairesi için oluşturduğunuz teklif {5} tarafından {6} tarihinde onaylanmıştır.</p>";
                        body += "<p>Satış ekranınızdaki Kapora Aldım butonunu tıklamayı unutmayın.</p>";


                        body = string.Format(body, quoteOwnerName, createdOn.ToLocalTime().ToString("dd.MM.yyyy HH:mm"), customerName, _proc.Project != null ? _proc.Project.Name : string.Empty, !string.IsNullOrEmpty(_proc.HomeNumber) ? _proc.HomeNumber : string.Empty, confirmUserName, salesDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"));

                    }
                    else
                    {
                        body += "<p>Sayın {0}</p>";
                        body += "<p>{1} tarihinde {2} müşterisinin {3} projesi {4} dairesi için oluşturduğunuz teklif reddedilmiştir.</p>";

                        body = string.Format(body, quoteOwnerName, createdOn.ToLocalTime().ToString("dd.MM.yyyy HH:mm"), customerName, _proc.Project != null ? _proc.Project.Name : string.Empty, !string.IsNullOrEmpty(_proc.HomeNumber) ? _proc.HomeNumber : string.Empty);

                    }
                    body += "<a href='{0}' target='_blank'>Satışı görmek için tıklayınız.</a>";
                    body += "<p>Teşekkürler</p>";
                    body += "<p>NEF CRM</p>";
                    string url = Globals.PortalUrl + "index.aspx?page=editsale&name=quoteid&pageid=" + quoteId;
                    body = string.Format(body, url);

                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);
                    Entity[] fromPartyColl = new Entity[] { fromParty };

                    Entity toParty = new Entity("activityparty");
                    toParty["partyid"] = new EntityReference("systemuser", quoteOwnerId);
                    Entity[] toPartyColl = new Entity[] { toParty };

                    MsCrmResult mailResult = GeneralHelper.SendMail(quoteId, "quote", fromPartyColl, toPartyColl, "Satış Onayı", body, null, service);
                    returnValue = mailResult;

                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = productResult.Result;
                }

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static void UpdateTaxAmount(Entity entity, IOrganizationService adminService)
        {
            Entity q = adminService.Retrieve("quote", entity.Id, new ColumnSet("new_taxrate", "new_taxofstamp", "new_isnotarizedsales", "new_containstax"));
            decimal totalAmount = ((Money)entity.Attributes["totalamount"]).Value;
            decimal taxRate = q.Contains("new_taxrate") ? (decimal)q.Attributes["new_taxrate"] : 0;
            if (q.Contains("new_containstax"))
            {
                if ((bool)q["new_containstax"])
                {
                    taxRate = 0;
                }
            }
            decimal taxOfStamp = q.Contains("new_taxofstamp") ? (decimal)q.Attributes["new_taxofstamp"] : 0;
            decimal taxAmount = (totalAmount * taxRate) / 100;
            decimal amountWithTax = totalAmount + taxAmount;
            decimal taxOfStampAmount = (totalAmount * taxOfStamp) / 100;
            entity.Attributes.Add("new_taxamount", new Money(taxAmount));
            entity.Attributes.Add("new_amountwithtax", new Money(amountWithTax));
            entity.Attributes.Add("new_taxofstampamount", new Money(taxOfStampAmount));
            entity.Attributes.Add("new_totalsalesamountbytax", new Money(amountWithTax + taxOfStampAmount));
            if (q.Contains("new_isnotarizedsales") && !(bool)q["new_isnotarizedsales"])
            {

                entity["new_taxamount"] = new Money(taxAmount);
                entity["new_amountwithtax"] = new Money(amountWithTax);
                entity["new_taxofstampamount"] = new Money(taxOfStampAmount);
                entity["new_totalsalesamountbytax"] = new Money(amountWithTax + taxOfStampAmount);
            }
            else if (q.Contains("new_isnotarizedsales") && (bool)q["new_isnotarizedsales"])
            {
                entity["new_taxofstampamount"] = new Money(0);
                entity["new_totalsalesamountbytax"] = new Money(amountWithTax);
            }
            //Birim metrekare fiyatı
            Entity detail = GetProductFromQuoteDetail(entity.Id, adminService);
            if (detail != null)
            {
                Guid productId = detail.Contains("productid") ? ((EntityReference)detail.Attributes["productid"]).Id : Guid.Empty;
                Entity product = adminService.Retrieve("product", productId, new ColumnSet("new_grossm2"));
                if (product.Contains("new_grossm2"))
                {
                    decimal grossM2 = (decimal)product.Attributes["new_grossm2"];
                    decimal perSquareMeter = totalAmount / grossM2;
                    entity.Attributes.Add("new_persquaremeter", new Money(perSquareMeter));
                }
            }

            //Birim metrekare fiyatı
        }

        public static void SetAmountForNewQuote(Entity quote, Guid quoteId, IOrganizationService adminService)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quote.Id);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = adminService.RetrieveMultiple(Query);

            if (Result.Entities.Count > 0)
            {
                Entity uEntity = Result.Entities[0];
                if (Convert.ToInt32(uEntity["RevisionNumber"]) > 0)
                {
                    uEntity["ispriceoverridden"] = true;
                    adminService.Update(uEntity);
                }
            }
        }

        public static void SetPaymentForNewQuote(Entity quote, Guid quoteId, IOrganizationService adminService)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = adminService.RetrieveMultiple(Query);
            foreach (Entity p in Result.Entities)
            {
                if (((OptionSetValue)p.Attributes["new_type"]).Value == 6 || ((OptionSetValue)p.Attributes["new_type"]).Value == 7)//KDV veya Damga vergisi değil ise
                {
                    continue;
                }
                Entity payment = new Entity("new_payment");
                payment = p;
                // payment.Id = p.Id;
                payment.Id = Guid.Empty;
                payment.Attributes["new_quoteid"] = new EntityReference("quote", quote.Id);
                payment.Attributes.Remove("new_paymentid");
                payment.Attributes.Remove("new_sign");
                payment.Attributes.Remove("new_isvoucher");
                adminService.Create(payment);
            }
        }

        public static void SetOpportunity(Entity entity, IOrganizationService adminService)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "customerid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(((EntityReference)entity.Attributes["customerid"]).Id);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "ownerid";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(((EntityReference)entity.Attributes["ownerid"]).Id);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("opportunity");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = adminService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                Guid oppId = Result.Entities[0].Id;
                Entity quote = new Entity("quote");
                quote.Id = entity.Id;
                quote["opportunityid"] = new EntityReference("opportunity", oppId);
                adminService.Update(quote);
            }
        }

        public static Entity ChangeTransactionCurrency(Entity entity, Entity preImage, IOrganizationService service)
        {
            try
            {
                Guid newTransactionCurrencyId = ((EntityReference)entity.Attributes["transactioncurrencyid"]).Id;
                string newTransactionCurrencyName = ((EntityReference)entity.Attributes["transactioncurrencyid"]).Name;

                Guid oldTransactionCurrencyId = ((EntityReference)preImage.Attributes["transactioncurrencyid"]).Id;
                string oldTransactionCurrencyName = ((EntityReference)preImage.Attributes["transactioncurrencyid"]).Name;

                DateTime? prePaymentDate = preImage.Contains("new_prepaymentdate") ? (DateTime)preImage.Attributes["new_prepaymentdate"] : (DateTime?)null;
                decimal prePaymentAmount = preImage.Contains("new_prepaymentamount") ? ((Money)preImage.Attributes["new_prepaymentdate"]).Value : 0;

                Guid newPriceLevelId = GetNewPriceLevel(newTransactionCurrencyId, service);

                if (newPriceLevelId == Guid.Empty)
                    return entity;

                Entity detail = GetProductFromQuoteDetail(entity.Id, service);

                if (detail == null)
                    return entity;

                Guid productId = detail.Contains("productid") ? ((EntityReference)detail.Attributes["productid"]).Id : Guid.Empty;
                Guid UomId = detail.Contains("uomid") ? ((EntityReference)detail.Attributes["uomid"]).Id : Guid.Empty;

                service.Delete("quotedetail", detail.Id);

                entity["pricelevelid"] = new EntityReference("pricelevel", newPriceLevelId);

                service.Update(entity);

                Entity quoteDetail = new Entity("quotedetail");
                quoteDetail.Attributes["quoteid"] = new EntityReference("quote", entity.Id);
                quoteDetail.Attributes["productid"] = new EntityReference("product", productId);
                quoteDetail.Attributes["uomid"] = new EntityReference("uom", UomId);
                quoteDetail.Attributes["quantity"] = new decimal(1);
                service.Create(quoteDetail);


                if (prePaymentDate != null && prePaymentAmount != 0)
                {
                    if (oldTransactionCurrencyId == Globals.CurrencyIdTL)
                    {
                        decimal exchangeRate = GetExchangeRate(newTransactionCurrencyId, prePaymentDate, service);

                        if (exchangeRate == 0)
                            throw new Exception("Kur Bilgisi Bulunamadı!!!");

                        entity["new_prepaymentamount"] = new Money(prePaymentAmount / exchangeRate);

                    }
                    else if (newTransactionCurrencyId == Globals.CurrencyIdTL && oldTransactionCurrencyId != Globals.CurrencyIdTL)
                    {
                        decimal exchangeRate = GetExchangeRate(oldTransactionCurrencyId, prePaymentDate, service);

                        if (exchangeRate == 0)
                            throw new Exception("Kur Bilgisi Bulunamadı!!!");

                        entity["new_prepaymentamount"] = new Money(prePaymentAmount * exchangeRate);
                    }
                    else if (newTransactionCurrencyId != Globals.CurrencyIdTL && oldTransactionCurrencyId != Globals.CurrencyIdTL)
                    {
                        decimal exchangeRateOld = GetExchangeRate(oldTransactionCurrencyId, prePaymentDate, service);

                        if (exchangeRateOld == 0)
                            throw new Exception("Kur Bilgisi Bulunamadı!!!");

                        decimal exchangeRateNew = GetExchangeRate(newTransactionCurrencyId, prePaymentDate, service);

                        if (exchangeRateNew == 0)
                            throw new Exception("Kur Bilgisi Bulunamadı!!!");
                        entity["new_prepaymentamount"] = new Money((prePaymentAmount * exchangeRateOld) / exchangeRateNew);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return entity;
        }

        private static decimal GetExchangeRate(Guid TransactionCurrencyId, DateTime? prePaymentDate, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_currencydate";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(prePaymentDate);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_currencyid";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(TransactionCurrencyId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_exchangerate");
            Query.ColumnSet = new ColumnSet("new_salesrate");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return (decimal)Result.Entities[0].Attributes["new_salesrate"];
            }
            else
            {
                return 0;
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

        private static Guid GetNewPriceLevel(Guid transactionCurrencyId, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "transactioncurrencyid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(transactionCurrencyId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("pricelevel");
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                return Result.Entities[0].Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        internal static void CalculatePerSquareMeter(Entity entity, IOrganizationService adminService)
        {
            decimal totalAmount = ((Money)entity.Attributes["totalamount"]).Value;
            Entity detail = GetProductFromQuoteDetail(entity.Id, adminService);
            if (detail != null)
            {
                Guid productId = detail.Contains("productid") ? ((EntityReference)detail.Attributes["productid"]).Id : Guid.Empty;
                Entity product = adminService.Retrieve("product", productId, new ColumnSet("new_grossm2"));
                if (product.Contains("new_grossm2"))
                {
                    decimal grossM2 = (decimal)product.Attributes["new_grossm2"];
                    decimal perSquareMeter = totalAmount / grossM2;
                    Entity q = new Entity("quote");
                    q.Id = entity.Id;
                    q.Attributes["new_persquaremeter"] = new Money(perSquareMeter);
                    adminService.Update(q);
                }
            }
        }

        public static MsCrmResult UpdateQuoteUserComment(Guid quoteId, string comment, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("quote");
                ent["quoteid"] = quoteId;
                ent["new_usercomment"] = comment;

                service.Update(ent);
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static string SetContractNumber(Entity quote, IOrganizationService service, SqlDataAccess sda)
        {
            Entity q = service.Retrieve("quote", quote.Id, new ColumnSet("new_contractnumber"));
            if (!q.Contains("new_contractnumber"))
            {
                #region SQL QUERY
                string sqlQuery = @"SELECT 
	                                TOP 1
	                                Q.new_contractnumber
                                 FROM 
	                                Quote Q (NOLOCK)
                                 WHERE 
								 Q.QuoteId !='{0}'
                                 and
                                 Q.new_contractnumber IS NOT NULL
								 AND
								 Q.new_contractnumber NOT LIKE '%TT.CRM.%'
                                 AND
								 CONVERT(int ,SUBSTRING(Q.new_contractnumber,8,4))= YEAR(Getdate())
                                 ORDER BY
	                             CONVERT(int ,SUBSTRING(Q.new_contractnumber,13,7))  DESC";
                sqlQuery = string.Format(sqlQuery, quote.Id);
                DataTable dt = sda.getDataTable(sqlQuery);
                #endregion SQL QUERY

                #region Set NUMBER
                //2016.8002804
                string contractNumber = dt.Rows.Count > 0 ? dt.Rows[0]["new_contractnumber"] != DBNull.Value ? dt.Rows[0]["new_contractnumber"].ToString() : string.Empty : string.Empty;
                if (contractNumber != string.Empty)
                {
                    int lastNumber = Convert.ToInt32(contractNumber.Substring(contractNumber.Length - 7));
                    quote["new_contractnumber"] = "FX.CRM." + DateTime.Now.Year.ToString() + "." + (lastNumber + 1).ToString();
                }
                else
                {
                    quote["new_contractnumber"] = "FX.CRM." + DateTime.Now.Year.ToString() + ".1000000";
                }
                #endregion Set NUMBER

                return quote["new_contractnumber"].ToString();
            }
            else
            {
                return q["new_contractnumber"].ToString();
            }

        }

        public static void SetLogoTransmission(Entity entity, IOrganizationService service)
        {
            if (entity.Contains("new_islogotransferred") && entity["new_islogotransferred"] != null)
            {
                entity.Attributes["new_islogotransferred"] = (bool)entity["new_islogotransferred"];
            }
            else
            {
                entity.Attributes["new_islogotransferred"] = true;
            }

        }

        internal static void SetExchangeRateOnQuotePreUpdate(Entity entity, Entity preImage, IOrganizationService service)
        {

            Guid transactioncurrencyid = Guid.Empty;
            DateTime ContractDate;
            if (entity.Contains("new_salesprocessdate"))
            {
                ContractDate = (DateTime)entity.Attributes["new_salesprocessdate"];
            }
            else
            {
                ContractDate = (DateTime)preImage.Attributes["new_salesprocessdate"];
            }

            if (entity.Contains("transactioncurrencyid"))
            {
                transactioncurrencyid = ((EntityReference)entity.Attributes["transactioncurrencyid"]).Id;
            }
            else
            {
                transactioncurrencyid = ((EntityReference)preImage.Attributes["transactioncurrencyid"]).Id;
            }
            decimal exchangeRate = GetExchangeRate(transactioncurrencyid, ContractDate, service);
            if (exchangeRate == 0)
            {
                entity.Attributes["new_exchangerate"] = 1;
            }
            else
            {
                entity.Attributes["new_exchangerate"] = exchangeRate;
            }


        }

        public static MsCrmResultObject GetProjectDetail(Guid projectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_minpayment Payment
                                FROM
	                                new_project P WITH (NOLOCK)
                                WHERE
	                                P.new_projectId = '{0}'
	                            AND
	                            P.new_minpayment IS NOT NULL";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.ReturnObject = (decimal)dt.Rows[0]["Payment"];

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Projenin min. kapora tutarı bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static void SetExchangeRateOnQuotePostCreate(Entity entity, IOrganizationService adminService)
        {
            Guid transactioncurrencyid = ((EntityReference)entity.Attributes["transactioncurrencyid"]).Id;
            DateTime ContractDate = (DateTime)entity.Attributes["new_salesprocessdate"];
            decimal exchangeRate = GetExchangeRate(transactioncurrencyid, ContractDate, adminService);
            Entity quote = new Entity("quote");
            quote.Id = entity.Id;
            if (exchangeRate == 0)
            {
                quote.Attributes["new_exchangerate"] = Convert.ToDecimal(1);
            }
            else
            {
                quote.Attributes["new_exchangerate"] = Convert.ToDecimal(exchangeRate);
            }
            adminService.Update(quote);
        }

        internal static void UpdateProduct(Guid quoteId, IOrganizationService service, QuoteStatus _quoteStatus)
        {
            Entity q = service.Retrieve("quote", quoteId, new ColumnSet("transactioncurrencyid", "new_salesprocessdate", "totalamount", "totalamountlessfreight"));
            Entity product = GetProductByQuoteId(service, quoteId);
            if (product != null)
            {
                if (_quoteStatus == QuoteStatus.KaporaAlındı)
                {
                    Entity p = new Entity("product");
                    p.Id = product.Id;
                    p.Attributes["new_realsalesprice"] = ((Money)q.Attributes["totalamountlessfreight"]).Value;
                    p.Attributes["new_realsalespricecurrencyid"] = (EntityReference)q.Attributes["transactioncurrencyid"];
                    p.Attributes["new_realsalesdate"] = (DateTime)q.Attributes["new_salesprocessdate"];
                    p.Attributes["new_salesofprepaymentid"] = new EntityReference("quote", quoteId);
                    service.Update(p);
                }
                else if (_quoteStatus == QuoteStatus.İptalEdildi)
                {
                    EntityReference saleQuoteId = product.Attributes.Contains("new_salesofprepaymentid") ? (EntityReference)product["new_salesofprepaymentid"] : null;
                    if (saleQuoteId != null && saleQuoteId.Id == quoteId)
                    {
                        Entity p = new Entity("product");
                        p.Id = product.Id;
                        p.Attributes["new_realsalesprice"] = null;
                        p.Attributes["new_realsalespricecurrencyid"] = null;
                        p.Attributes["new_realsalesdate"] = null;
                        p.Attributes["new_salesofprepaymentid"] = null;

                        service.Update(p);
                    }
                }

            }


        }

        private static Entity GetProductByQuoteId(IOrganizationService service, Guid QuoteId)
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

        internal static void SetPreStatus(Entity entity, IOrganizationService service, QuoteStatus _oldQuoteStatus)
        {
            entity["new_prestatus"] = (int)_oldQuoteStatus;
        }

        public static MsCrmResultObject GetSystemUserQuotesBySalesDateRange(Guid systemUserId, DateTime startDate, DateTime endDate, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                q.QuoteId
                                    ,q.Name
	                                ,q.new_salesprocessdate AS SalesDate
	                                ,q.TotalAmountLessFreight
                                    ,q.StateCode
	                                ,q.StatusCode
	                                ,smStateCode.Value AS StateValue
	                                ,smStatusCode.Value AS StatusValue
                                FROM
	                                Quote AS q (NOLOCK)
                                        JOIN
		                                    StringMap AS smStateCode (NOLOCK)
			                                    ON
			                                    smStateCode.ObjectTypeCode=1084
			                                    AND
			                                    smStateCode.AttributeName='statecode'
			                                    AND
			                                    smStateCode.AttributeValue=q.StateCode
	                                    JOIN
		                                    StringMap AS smStatusCode (NOLOCK)
			                                    ON
			                                    smStatusCode.ObjectTypeCode=1084
			                                    AND
			                                    smStatusCode.AttributeName='statuscode'
			                                    AND
			                                    smStatusCode.AttributeValue=q.StatusCode
                                WHERE
	                                q.OwnerId=@ownerId AND q.StatusCode!=7 AND new_salesprocessdate BETWEEN @start AND @end  ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ownerId", systemUserId), new SqlParameter("@start", startDate), new SqlParameter("@end", endDate) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    List<Quote> lst = new List<Quote>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Quote _quote = new Quote();
                        _quote.QuoteId = (Guid)dt.Rows[i]["QuoteId"];
                        _quote.Name = dt.Rows[i]["Name"].ToString();
                        _quote.StateCode = new StringMap() { Name = dt.Rows[i]["StateValue"].ToString(), Value = (int)dt.Rows[i]["StateCode"] };
                        _quote.StatusCode = new StringMap() { Name = dt.Rows[i]["StatusValue"].ToString(), Value = (int)dt.Rows[i]["StatusCode"] };

                        if (dt.Rows[i]["SalesDate"] != DBNull.Value)
                        {
                            _quote.SalesDate = ((DateTime)dt.Rows[i]["SalesDate"]).ToLocalTime();

                        }

                        if (dt.Rows[i]["TotalAmountLessFreight"] != DBNull.Value)
                        {
                            _quote.HouseListPrice = (decimal)dt.Rows[i]["TotalAmountLessFreight"];
                        }

                        lst.Add(_quote);

                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lst;
                    returnValue.Result = "Kullanıcı satışları çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message + "-GetSystemUserQuotes";

            }

            return returnValue;
        }

        public static MsCrmResultObject GetQuoteDetailFOrChart(Guid quoteId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {

                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                q.QuoteId
	                                --,q.QuoteNumber
	                                --,q.RevisionNumber
	                                ,q.Name
	                                ,q.new_salesprocessdate AS SalesDate
	                                ,q.TotalAmountLessFreight
	                                --,q.TotalAmount
	                                ,q.StateCode
	                                ,q.StatusCode
	                                ,smStateCode.Value AS StateValue
	                                ,smStatusCode.Value AS StatusValue
	                                --,q.OpportunityId
                                    --,q.OpportunityIdName
	                                --,q.CustomerId
                                    --,q.CustomerIdName
                                    --,q.CustomerIdType
	                                --,q.new_paymentplan AS PaymentPlan
                                    --,q.TransactionCurrencyId AS CurrencyId
	                                --,q.TransactionCurrencyIdName AS CurrencyIdName
                                    --,q.CreatedOn
									--,q.OwnerId
									--,q.OwnerIdName
									--,q.new_salestype SalesType
									--,q.new_contractdate ContractDate
									--,q.new_isrevisiononhome IsRevision
									--,q.new_revisiondescription RevisionDescription
									--,q.discountpercentage DiscountPercentage
									--,q.discountamount DiscountPrice
									--,q.TotalAmountLessFreight HouseDiscountPrice
									--,q.new_prepaymentamount PrePaymentPrice
                                    --,q.new_prepaymentdate PrePaymentDate
									--,q.new_suminterval SumInterval
									--,q.new_instnumber InstNumber
									--,q.new_instamount SumInstPrice
                                    --,q.new_paymentplan PaymentPlan
                                    --,q.new_paymentterm PaymentTerm
                                    --,q.new_paymentplandiscountrate PaymentPlanDiscountRate
                                    --,q.new_persquaremeter PerSquareMeterPrice
                                    --,q.new_approvaltype ApprovalType
                                    --,q.new_fallingapprovaltype FallingApprovalType
                                    --,q.new_usercomment UserComment
                                    --,q.new_hassecondcustomer AS HasSecondCustomer
                                    --,q.new_secondcontactid AS SecondContactId
                                    --,q.new_secondcontactidName AS SecondContactIdName
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                StringMap AS smStateCode (NOLOCK)
			                                ON
			                                smStateCode.ObjectTypeCode=1084
			                                AND
			                                smStateCode.AttributeName='statecode'
			                                AND
			                                smStateCode.AttributeValue=q.StateCode
	                                JOIN
		                                StringMap AS smStatusCode (NOLOCK)
			                                ON
			                                smStatusCode.ObjectTypeCode=1084
			                                AND
			                                smStatusCode.AttributeName='statuscode'
			                                AND
			                                smStatusCode.AttributeValue=q.StatusCode
                                WHERE
	                                q.QuoteId=@quoteId ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@quoteId", quoteId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    #region | FILL QUOTE INFO |
                    Quote _quote = new Quote();
                    _quote.QuoteId = (Guid)dt.Rows[0]["QuoteId"];
                    _quote.Name = dt.Rows[0]["Name"].ToString();
                    //_quote.UserComment = dt.Rows[0]["UserComment"] != DBNull.Value ? dt.Rows[0]["UserComment"].ToString() : "";

                    //_quote.CreatedOn = ((DateTime)dt.Rows[0]["CreatedOn"]).ToLocalTime();
                    //_quote.CreatedOnString = dt.Rows[0]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[0]["CreatedOn"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    //_quote.ContratDateString = dt.Rows[0]["ContractDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["ContractDate"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    //_quote.SalesDateString = dt.Rows[0]["SalesDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["SalesDate"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    //_quote.SalesDateStringHour = dt.Rows[0]["SalesDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["SalesDate"]).ToLocalTime().ToString("dd.MM.yyyy HH:mm") : "";


                    //_quote.HouseListPriceString = dt.Rows[0]["TotalLineItemAmount"] != DBNull.Value ? ((decimal)dt.Rows[0]["TotalLineItemAmount"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.HouseSalePriceString = dt.Rows[0]["TotalAmount"] != DBNull.Value ? ((decimal)dt.Rows[0]["TotalAmount"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.DiscountPriceString = dt.Rows[0]["DiscountPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["DiscountPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.HouseDiscountPriceString = dt.Rows[0]["HouseDiscountPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["HouseDiscountPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.PrePaymentPriceString = dt.Rows[0]["PrePaymentPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["PrePaymentPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.PerSquareMeterPriceString = dt.Rows[0]["PerSquareMeterPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["PerSquareMeterPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.SumIntervalPriceString = dt.Rows[0]["SumInterval"] != DBNull.Value ? ((decimal)dt.Rows[0]["SumInterval"]).ToString("N0", CultureInfo.CurrentCulture) : "";
                    //_quote.PaymentPlan = dt.Rows[0]["PaymentPlan"] != DBNull.Value ? (bool)dt.Rows[0]["PaymentPlan"] : false;

                    //_quote.SumInstPriceString = dt.Rows[0]["SumInstPrice"] != DBNull.Value ? ((decimal)dt.Rows[0]["SumInstPrice"]).ToString("N0", CultureInfo.CurrentCulture) : "";

                    //_quote.RevisionDescription = dt.Rows[0]["RevisionDescription"] != DBNull.Value ? dt.Rows[0]["RevisionDescription"].ToString() : "";

                    //_quote.RevisionNumber = (int)dt.Rows[0]["RevisionNumber"];
                    //_quote.QuoteNumber = dt.Rows[0]["QuoteNumber"].ToString();

                    _quote.StateCode = new StringMap() { Name = dt.Rows[0]["StateValue"].ToString(), Value = (int)dt.Rows[0]["StateCode"] };
                    _quote.StatusCode = new StringMap() { Name = dt.Rows[0]["StatusValue"].ToString(), Value = (int)dt.Rows[0]["StatusCode"] };

                    //if (dt.Rows[0]["PrePaymentDate"] != DBNull.Value)
                    //{
                    //    _quote.PrePaymentDate = (DateTime)dt.Rows[0]["PrePaymentDate"];
                    //}

                    if (dt.Rows[0]["SalesDate"] != DBNull.Value)
                    {
                        _quote.SalesDate = ((DateTime)dt.Rows[0]["SalesDate"]).ToLocalTime();

                    }

                    //if (dt.Rows[0]["PerSquareMeterPrice"] != DBNull.Value)
                    //{
                    //    _quote.PerSquareMeterPrice = (decimal)dt.Rows[0]["PerSquareMeterPrice"];
                    //}

                    if (dt.Rows[0]["TotalAmountLessFreight"] != DBNull.Value)
                    {
                        _quote.HouseListPrice = (decimal)dt.Rows[0]["TotalAmountLessFreight"];
                    }

                    //if (dt.Rows[0]["TotalAmount"] != DBNull.Value)
                    //{
                    //    _quote.HouseSalePrice = (decimal)dt.Rows[0]["TotalAmount"];
                    //}

                    //if (dt.Rows[0]["OpportunityId"] != DBNull.Value)
                    //{
                    //    _quote.Opportunity = new EntityReference() { Id = (Guid)dt.Rows[0]["OpportunityId"], Name = dt.Rows[0]["OpportunityIdName"].ToString(), LogicalName = "opportunity" };
                    //}

                    //if (dt.Rows[0]["CustomerId"] != DBNull.Value)
                    //{
                    //    _quote.EntityType = dt.Rows[0]["CustomerIdType"].ToString();
                    //    if (_quote.EntityType == "2")
                    //    {
                    //        _quote.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["CustomerId"], Name = dt.Rows[0]["CustomerIdName"].ToString(), LogicalName = "contact" };
                    //    }
                    //    else
                    //    {
                    //        _quote.Contact = new EntityReference() { Id = (Guid)dt.Rows[0]["CustomerId"], Name = dt.Rows[0]["CustomerIdName"].ToString(), LogicalName = "account" };
                    //    }

                    //}

                    //if (dt.Rows[0]["PaymentPlan"] != DBNull.Value)
                    //{
                    //    _quote.PaymentPlan = (bool)dt.Rows[0]["PaymentPlan"];
                    //}

                    //if (dt.Rows[0]["CurrencyId"] != DBNull.Value)
                    //{
                    //    _quote.Currency = new EntityReference() { Id = (Guid)dt.Rows[0]["CurrencyId"], Name = dt.Rows[0]["CurrencyIdName"].ToString(), LogicalName = "transactioncurrency" };
                    //}

                    //if (dt.Rows[0]["OwnerId"] != DBNull.Value)
                    //{
                    //    _quote.Owner = new EntityReference() { Id = (Guid)dt.Rows[0]["OwnerId"], Name = dt.Rows[0]["OwnerIdName"].ToString(), LogicalName = "systemuser" };
                    //}

                    //if (dt.Rows[0]["SalesType"] != DBNull.Value)
                    //{
                    //    _quote.SalesType = (SalesTypes)dt.Rows[0]["SalesType"];
                    //}

                    //if (dt.Rows[0]["ContractDate"] != DBNull.Value)
                    //{
                    //    _quote.ContratDate = (DateTime)dt.Rows[0]["ContractDate"];
                    //}

                    //if (dt.Rows[0]["IsRevision"] != DBNull.Value)
                    //{
                    //    _quote.IsRevision = (bool)dt.Rows[0]["IsRevision"];
                    //}

                    //if (dt.Rows[0]["DiscountPercentage"] != DBNull.Value)
                    //{
                    //    _quote.DiscountPercentage = (decimal)dt.Rows[0]["DiscountPercentage"];
                    //}

                    //if (dt.Rows[0]["DiscountPrice"] != DBNull.Value)
                    //{
                    //    _quote.DiscountPrice = (decimal)dt.Rows[0]["DiscountPrice"];
                    //}

                    //if (dt.Rows[0]["HouseDiscountPrice"] != DBNull.Value)
                    //{
                    //    _quote.HouseDiscountPrice = (decimal)dt.Rows[0]["HouseDiscountPrice"];
                    //}

                    //if (dt.Rows[0]["PrePaymentPrice"] != DBNull.Value)
                    //{
                    //    _quote.PrePaymentPrice = (decimal)dt.Rows[0]["PrePaymentPrice"];
                    //}

                    //if (dt.Rows[0]["SumInterval"] != DBNull.Value)
                    //{
                    //    _quote.SumIntervalPrice = (decimal)dt.Rows[0]["SumInterval"];
                    //}

                    //if (dt.Rows[0]["InstNumber"] != DBNull.Value)
                    //{
                    //    _quote.InstNumber = (int)dt.Rows[0]["InstNumber"];
                    //}

                    //if (dt.Rows[0]["SumInstPrice"] != DBNull.Value)
                    //{
                    //    _quote.SumInstPrice = (decimal)dt.Rows[0]["SumInstPrice"];
                    //}

                    //if (dt.Rows[0]["PaymentTerm"] != DBNull.Value)
                    //{
                    //    _quote.PaymentTerm = (int)dt.Rows[0]["PaymentTerm"];
                    //}

                    //if (dt.Rows[0]["PaymentPlanDiscountRate"] != DBNull.Value)
                    //{
                    //    _quote.PaymentPlanDiscountRate = (decimal)dt.Rows[0]["PaymentPlanDiscountRate"];
                    //}

                    //if (dt.Rows[0]["ApprovalType"] != DBNull.Value)
                    //{
                    //    _quote.ApprovalType = (FallingApprovalTypes)(int)dt.Rows[0]["ApprovalType"];
                    //}

                    //if (dt.Rows[0]["FallingApprovalType"] != DBNull.Value)
                    //{
                    //    _quote.FallingApprovalType = (FallingApprovalTypes)(int)dt.Rows[0]["FallingApprovalType"];
                    //}

                    //if (dt.Rows[0]["HasSecondCustomer"] != DBNull.Value)
                    //{
                    //    _quote.HasSecondCustomer = (bool)dt.Rows[0]["HasSecondCustomer"];
                    //}

                    //if (dt.Rows[0]["SecondContactId"] != DBNull.Value)
                    //{
                    //    EntityReference er = new EntityReference
                    //    {
                    //        Id = (Guid)dt.Rows[0]["SecondContactId"],
                    //        Name = dt.Rows[0]["SecondContactId"].ToString()
                    //    };

                    //    _quote.SecondCustomer = er;
                    //}

                    //MsCrmResultObject resultProduct = QuoteHelper.GetQuoteProducts(_quote.QuoteId, sda);

                    //if (resultProduct.Success)
                    //{
                    //    _quote.Products = (List<Product>)resultProduct.ReturnObject;
                    //}

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _quote;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }


            return returnValue;
        }

        public static MsCrmResult Muhasebelestir(string quoteId)
        {
            MsCrmResult returnValue = new MsCrmResult();
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
                    if (!string.IsNullOrEmpty(customerName))
                    {
                        if (!ContactHelper.CheckContactHasGroupCode(customer.Id, sda).Success)
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

                returnValue.Success = true;
                returnValue.Result = "Satış başarılı bir şekilde güncellendi...";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        internal static void UpdateTaxOfStamp(Entity entity, IOrganizationService service)
        {
            Entity q = service.Retrieve("quote", entity.Id, new ColumnSet("new_taxrate", "new_taxofstamp", "totalamount"));

            decimal totalAmount = q.Contains("totalamount") ? ((Money)q.Attributes["totalamount"]).Value : 0;
            decimal taxRate = q.Contains("new_taxrate") ? (decimal)q.Attributes["new_taxrate"] : 0;
            decimal taxOfStamp = q.Contains("new_taxofstamp") ? (decimal)q.Attributes["new_taxofstamp"] : 0;
            decimal taxAmount = (totalAmount * taxRate) / 100;
            decimal amountWithTax = totalAmount + taxAmount;
            decimal taxOfStampAmount = (totalAmount * taxOfStamp) / 100;
            if (!(bool)entity["new_isnotarizedsales"])
            {

                entity["new_taxamount"] = new Money(taxAmount);
                entity["new_amountwithtax"] = new Money(amountWithTax);
                entity["new_taxofstampamount"] = new Money(taxOfStampAmount);
                entity["new_totalsalesamountbytax"] = new Money(amountWithTax + taxOfStampAmount);
            }
            else
            {
                entity["new_taxofstampamount"] = new Money(0);
                entity["new_totalsalesamountbytax"] = new Money(amountWithTax);
            }

        }

        public static DataTable GetNotProcessedYellowExcelQuotes(SqlDataAccess sda)
        {
            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                q.QuoteId,
                                    q.CustomerIdType,
                                    q.CustomerId,
                                    q.new_financialaccountid,
                                    q.totalamountlessfreight,
                                    q.TotalLineItemAmount,
									q.new_projectid
                                FROM
                                Quote AS q (NOLOCK)
                                WHERE
                                q.StateCode=0
                                --AND
									--q.new_projectid = 'E37FAB80-9BCC-E411-80CF-005056A60603'
                                AND
                                  q.new_yellowexcelstate=1";
            //emrahe : Canlı

            //            string sqlQuery = @"SELECT
            //	                                q.QuoteId,
            //                                    q.CustomerIdType,
            //                                    q.CustomerId,
            //                                    q.new_financialaccountid,
            //                                    q.totalamountlessfreight,
            //                                    q.TotalLineItemAmount
            //                                FROM
            //                                Quote AS q (NOLOCK)
            //                                WHERE
            //                                q.StateCode=0
            //                                AND
            //                                q.new_yellowexcelstate=1 --Yüklendi";
            #endregion
            return sda.getDataTable(sqlQuery);
        }

        public static MsCrmResult UpdateYellowExcelState(Guid quoteId, int state, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("quote");
                ent.Id = quoteId;
                ent["new_yellowexcelstate"] = new OptionSetValue(state);

                service.Update(ent);

                returnValue.Success = true;
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static void UpdateYellowExcelState(Guid quoteId, int state, IOrganizationService service, string message)
        {
            if ((message.Length > 2000))
            {
                message = message.Substring(0, 2000);
            }
            Entity ent = new Entity("quote");
            ent.Id = quoteId;
            ent["new_yellowexcelstatedesc"] = message;
            ent["new_yellowexcelstate"] = new OptionSetValue(state);
            service.Update(ent);
        }

        /// <summary>
        /// Hata mesajları loglanır.
        /// </summary>
        /// <param name="quoteId">Teklif Id</param>
        /// <param name="message">Hata Mesajı</param>
        /// <param name="service">Servis</param>
        public static void LogYellowExcelError(Guid quoteId, string message, IOrganizationService service)
        {
            try
            {
                if ((message.Length > 2000))
                {
                    message = message.Substring(0, 2000);
                }
                Entity ent = new Entity("quote");
                ent.Id = quoteId;
                ent["new_yellowexcelstatedesc"] = message;
                ent["new_yellowexcelstate"] = new OptionSetValue(4);
                service.Update(ent);
            }
            catch (Exception ex)
            {

            }
        }

        public static DataTable GetNotProcessedYellowExcelData(Guid quoteId, string sheetName, SqlDataAccess sda)
        {
            DataTable returnValue = new DataTable();

            string fileName = @Globals.YellowExcelProcessTempFolder + quoteId.ToString() + ".xlsx";

            #region |SQL QUERY|
            string sqlQuery = @"SELECT TOP 1
	                                si.QuoteId
	                                ,an.DocumentBody
                                FROM
                                Quote AS si (NOLOCK)
	                                JOIN
		                                Annotation AS an (NOLOCK)
			                                ON
			                                si.QuoteId=an.ObjectId
			                                AND
			                                an.FileName LIKE '%xlsx%'
			                                AND
			                                an.DocumentBody IS NOT NULL
                                WHERE
	                                si.QuoteId='{0}' ORDER BY an.CreatedOn DESC";
            #endregion

            try
            {
                DataTable dt = sda.getDataTable(string.Format(sqlQuery, quoteId.ToString()));

                if (dt.Rows.Count > 0)
                {
                    if (File.Exists(@fileName))
                    {
                        File.Delete(@fileName);
                    }

                    var bytes = Convert.FromBase64String(dt.Rows[0]["DocumentBody"].ToString());
                    using (var imageFile = new FileStream(@fileName, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + "; Extended Properties=Excel 12.0");
                    baglanti.Open();
                    string sorgu = "select * from [" + sheetName + "$]";
                    OleDbDataAdapter data_adaptor = new OleDbDataAdapter(sorgu, baglanti);
                    baglanti.Close();

                    data_adaptor.Fill(returnValue);

                    File.Delete(@fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Console.ReadLine();
            }

            return returnValue;
        }

        public static MsCrmResultObject GetQuotesBySalesDateRange(DateTime startDate, DateTime endDate, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                q.QuoteId
                                    ,q.Name
	                                ,q.new_salesprocessdate AS SalesDate
	                                ,q.TotalLineItemAmount
                                    ,q.StateCode
	                                ,q.StatusCode
	                                ,smStateCode.Value AS StateValue
	                                ,smStatusCode.Value AS StatusValue
                                    ,q.TotalAmountLessFreight
                                FROM
	                                Quote AS q (NOLOCK)
                                        JOIN
		                                    StringMap AS smStateCode (NOLOCK)
			                                    ON
			                                    smStateCode.ObjectTypeCode=1084
			                                    AND
			                                    smStateCode.AttributeName='statecode'
			                                    AND
			                                    smStateCode.AttributeValue=q.StateCode
	                                    JOIN
		                                    StringMap AS smStatusCode (NOLOCK)
			                                    ON
			                                    smStatusCode.ObjectTypeCode=1084
			                                    AND
			                                    smStatusCode.AttributeName='statuscode'
			                                    AND
			                                    smStatusCode.AttributeValue=q.StatusCode
                                WHERE
	                                q.StatusCode!=7 AND new_salesprocessdate BETWEEN @start AND @end  ORDER BY q.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    List<Quote> lst = new List<Quote>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Quote _quote = new Quote();
                        _quote.QuoteId = (Guid)dt.Rows[i]["QuoteId"];
                        _quote.Name = dt.Rows[i]["Name"].ToString();
                        _quote.StateCode = new StringMap() { Name = dt.Rows[i]["StateValue"].ToString(), Value = (int)dt.Rows[i]["StateCode"] };
                        _quote.StatusCode = new StringMap() { Name = dt.Rows[i]["StatusValue"].ToString(), Value = (int)dt.Rows[i]["StatusCode"] };

                        if (dt.Rows[i]["SalesDate"] != DBNull.Value)
                        {
                            _quote.SalesDate = ((DateTime)dt.Rows[i]["SalesDate"]).ToLocalTime();

                        }

                        if (dt.Rows[i]["TotalAmountLessFreight"] != DBNull.Value)
                        {
                            _quote.HouseListPrice = (decimal)dt.Rows[i]["TotalAmountLessFreight"];
                        }

                        lst.Add(_quote);

                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lst;
                    returnValue.Result = "Kullanıcı satışları çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message + "-GetSystemUserQuotes";

            }

            return returnValue;
        }

        public static MsCrmResultObject GetAllQuotes(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                q.QuoteId
                                FROM
	                                Quote AS q (NOLOCK)
                                WHERE
	                                q.StatusCode!=7 ORDER BY q.CreatedOn DESC";

                #endregion

                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    List<Quote> lst = new List<Quote>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MsCrmResultObject quoteResult = QuoteHelper.GetQuoteDetail((Guid)dt.Rows[i]["QuoteId"], sda);
                        if (quoteResult.Success)
                        {
                            Quote _quote = (Quote)quoteResult.ReturnObject;
                            lst.Add(_quote);
                        }
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lst;
                    returnValue.Result = "Kullanıcı satışları çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message + "-GetSystemUserQuotes";

            }

            return returnValue;
        }

        public static EntityReference GetQuoteAttachment(Guid quoteId, SqlDataAccess sda)
        {
            EntityReference returnValue = new EntityReference();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT TOP 1
                                a.AnnotationId
                                ,a.FileName
                                FROM
                                Annotation AS a (NOLOCK)
                                WHERE
                                a.ObjectId='{0}' ORDER BY  a.CreatedOn DESC";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(sqlQuery, quoteId.ToString()));

            if (dt.Rows.Count > 0)
            {
                returnValue.Id = (Guid)dt.Rows[0]["AnnotationId"];
                returnValue.Name = dt.Rows[0]["FileName"].ToString();
            }
            else
            {
                returnValue = null;
            }

            return returnValue;
        }

        public static List<ChartKeyValues> GetSalesAmountByProjectData(DateTime startDate, DateTime endDate, SqlDataAccess sda)
        {
            List<ChartKeyValues> returnValue = new List<ChartKeyValues>();

            #region | SQL QUERY |
            string sqlQuery = @"SELECT
	                                p.new_projectidName AS Name,
	                                CAST(SUM(ISNULL( q.TotalAmountLessFreight * e.new_salesrate,q.TotalAmountLessFreight)) AS INT) Value
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                Product AS p (NOLOCK)
			                                ON
			                                p.ProductId=q.new_productid
                                LEFT JOIN 
										new_exchangerate AS e (NOLOCK)
											ON
											Convert(DateTime, Convert(VarChar, q.new_salesprocessdate, 12)) = Convert(DateTime, Convert(VarChar, e.new_currencydate, 12))
											AND
											q.TransactionCurrencyId = e.new_currencyid
                                WHERE
                                q.StatusCode IN (2,100000001,100000009,100000007)
                                AND
                                q.new_salesprocessdate IS NOT NULL
                                AND
                                q.new_productid IS NOT NULL
                                AND
                                q.new_salesprocessdate BETWEEN @start AND @end
                                GROUP BY
	                                p.new_projectidName";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate) };
            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue = dt.ToList<ChartKeyValues>();
            }

            return returnValue;
        }

        public static List<ChartKeyValues> GetSalesGeneralAmountByProjectData(DateTime startDate, DateTime endDate, SqlDataAccess sda)
        {
            List<ChartKeyValues> returnValue = new List<ChartKeyValues>();

            #region | SQL QUERY |
            string sqlQuery = @"SELECT
	                                p.new_projectidName AS Name,
	                                CAST(SUM(ISNULL( q.TotalAmountLessFreight * e.new_salesrate,q.TotalAmountLessFreight)) AS INT) Value
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                Product AS p (NOLOCK)
			                                ON
			                                p.ProductId=q.new_productid
                                LEFT JOIN 
										new_exchangerate AS e (NOLOCK)
											ON
											Convert(DateTime, Convert(VarChar, q.new_salesprocessdate, 12)) = Convert(DateTime, Convert(VarChar, e.new_currencydate, 12))
											AND
											q.TransactionCurrencyId = e.new_currencyid
                                WHERE
                                q.StatusCode IN (2,100000001,100000007,100000009)
                                AND
                                q.new_salesprocessdate IS NOT NULL
                                AND
                                q.new_productid IS NOT NULL
                                AND
                                q.new_salesprocessdate BETWEEN @start AND @end
                                GROUP BY
	                                p.new_projectidName";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate) };
            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue = dt.ToList<ChartKeyValues>();
            }

            return returnValue;
        }

        public static List<ChartKeyValues> GetSalesQuantityByProjectData(DateTime startDate, DateTime endDate, SqlDataAccess sda)
        {
            List<ChartKeyValues> returnValue = new List<ChartKeyValues>();

            #region | SQL QUERY |
            string sqlQuery = @"SELECT
	                                p.new_projectidName AS Name
	                                ,CAST(COUNT(0) AS INT) Value
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                Product AS p (NOLOCK)
			                                ON
			                                p.ProductId=q.new_productid
                                WHERE
                                q.StatusCode IN (2,100000001,100000009,100000007)
                                AND
                                q.new_salesprocessdate IS NOT NULL
                                AND
                                q.new_productid IS NOT NULL
                                AND
                                q.new_salesprocessdate BETWEEN @start AND @end
                                GROUP BY
	                                p.new_projectidName";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate) };
            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue = dt.ToList<ChartKeyValues>();
            }

            return returnValue;
        }

        public static List<ChartKeyValues> GetSalesGeneralQuantityByProjectData(DateTime startDate, DateTime endDate, SqlDataAccess sda)
        {
            List<ChartKeyValues> returnValue = new List<ChartKeyValues>();

            #region | SQL QUERY |
            string sqlQuery = @"SELECT
	                                p.new_projectidName AS Name
	                                ,CAST(COUNT(0) AS INT) Value
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                Product AS p (NOLOCK)
			                                ON
			                                p.ProductId=q.new_productid
                                WHERE
                                q.StatusCode IN (2,100000001,100000007,100000009)
                                AND
                                q.new_salesprocessdate IS NOT NULL
                                AND
                                q.new_productid IS NOT NULL
                                AND
                                q.new_salesprocessdate BETWEEN @start AND @end
                                GROUP BY
	                                p.new_projectidName";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate) };
            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue = dt.ToList<ChartKeyValues>();
            }

            return returnValue;
        }

        public static List<ChartKeyValues> GetUserSalesAmountByProjectData(DateTime startDate, DateTime endDate, Guid systemUserId, SqlDataAccess sda)
        {
            List<ChartKeyValues> returnValue = new List<ChartKeyValues>();

            #region | SQL QUERY |
            // Farklı kur satışlarını türk lirası gibi aldığı için güncellendi.
            //            string sqlQuery = @"SELECT
            //	                                p.new_projectidName AS Name
            //	                                ,CAST(SUM(q.TotalLineItemAmount) AS INT) Value
            //                                FROM
            //                                Quote AS q (NOLOCK)
            //	                                JOIN
            //		                                Product AS p (NOLOCK)
            //			                                ON
            //			                                p.ProductId=q.new_productid
            //                                WHERE
            //                                q.OwnerId=@userId
            //                                AND
            //                                q.StatusCode IN (2,100000001)
            //                                AND
            //                                q.new_salesprocessdate IS NOT NULL
            //                                AND
            //                                q.new_productid IS NOT NULL
            //                                AND
            //                                q.new_salesprocessdate BETWEEN @start AND @end
            //                                GROUP BY
            //	                                p.new_projectidName";
            //Satış işlem tarihine göre o güne ait döviz kuru çekilirek tl ye çevrilir.
            string sqlQuery = @"SELECT
	                                p.new_projectidName AS Name,
	                                CAST(SUM(ISNULL( q.TotalAmountLessFreight * e.new_salesrate,q.TotalAmountLessFreight)) AS INT) Value
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                Product AS p (NOLOCK)
			                                ON
			                                p.ProductId=q.new_productid
                                LEFT JOIN 
										new_exchangerate AS e (NOLOCK)
											ON
											Convert(DateTime, Convert(VarChar, q.new_salesprocessdate, 12)) = Convert(DateTime, Convert(VarChar, e.new_currencydate, 12))
											AND
											q.TransactionCurrencyId = e.new_currencyid
                                WHERE
                                q.OwnerId=@userId
                                AND
                                q.StatusCode IN (2,100000001,100000007,100000009)
                                AND
                                q.new_salesprocessdate IS NOT NULL
                                AND
                                q.new_productid IS NOT NULL
                                AND
                                q.new_salesprocessdate BETWEEN @start AND @end
                                GROUP BY
	                                p.new_projectidName";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate), new SqlParameter("@userId", systemUserId) };
            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue = dt.ToList<ChartKeyValues>();
            }

            return returnValue;
        }

        public static List<ChartKeyValues> GetUserSalesQuantityByProjectData(DateTime startDate, DateTime endDate, Guid systemUserId, SqlDataAccess sda)
        {
            List<ChartKeyValues> returnValue = new List<ChartKeyValues>();

            #region | SQL QUERY |
            string sqlQuery = @"SELECT
	                                p.new_projectidName AS Name
	                                ,CAST(COUNT(0) AS INT) Value
                                FROM
                                Quote AS q (NOLOCK)
	                                JOIN
		                                Product AS p (NOLOCK)
			                                ON
			                                p.ProductId=q.new_productid
                                WHERE
                                q.OwnerId=@userId
                                AND
                                q.StatusCode IN (2,100000001,100000007,100000009)
                                AND
                                q.new_salesprocessdate IS NOT NULL
                                AND
                                q.new_productid IS NOT NULL
                                AND
                                q.new_salesprocessdate BETWEEN @start AND @end
                                GROUP BY
	                                p.new_projectidName";
            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@start", startDate), new SqlParameter("@end", endDate), new SqlParameter("@userId", systemUserId) };
            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                returnValue = dt.ToList<ChartKeyValues>();
            }

            return returnValue;
        }

        internal static void UpdateQuoteProduct(Guid quoteId, IOrganizationService service, bool overridePrice)
        {
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
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                Entity qd = new Entity("quotedetail");
                qd.Id = Result.Entities[0].Id;
                qd["ispriceoverridden"] = overridePrice;
                service.Update(qd);
            }
        }

        public static EntityReference GetRentralAttachment(Guid rentalId, SqlDataAccess sda)
        {
            EntityReference returnValue = new EntityReference();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT TOP 1
                                a.AnnotationId
                                ,a.FileName
                                FROM
                                Annotation AS a (NOLOCK)
                                WHERE
                                a.ObjectId='{0}' AND filename like '%TT_%' ORDER BY  a.CreatedOn DESC";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(sqlQuery, rentalId.ToString()));

            if (dt.Rows.Count > 0)
            {
                returnValue.Id = (Guid)dt.Rows[0]["AnnotationId"];
                returnValue.Name = dt.Rows[0]["FileName"].ToString();
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static EntityReference GetDisplayLocationAttachment(Guid activityId, SqlDataAccess sda)
        {
            EntityReference returnValue = new EntityReference();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT TOP 1
                                a.AnnotationId
                                ,a.FileName
                                FROM
                                Annotation AS a (NOLOCK)
                                WHERE
                                a.ObjectId='{0}' AND filename like '%YGB_%' ORDER BY  a.CreatedOn DESC";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(sqlQuery, activityId.ToString()));

            if (dt.Rows.Count > 0)
            {
                returnValue.Id = (Guid)dt.Rows[0]["AnnotationId"];
                returnValue.Name = dt.Rows[0]["FileName"].ToString();
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }



    }

}
