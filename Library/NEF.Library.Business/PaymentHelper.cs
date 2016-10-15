using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class PaymentHelper
    {
        public static MsCrmResult CreateOrUpdatePayment(Payment _payment, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity product = GetProductByQuoteId(service, _payment.Quote.Id);
                Guid projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
                EntityCollection list = GetProjectSalesCollaborateList(service, projectId);

                foreach (Entity collaborate in list.Entities)
                {
                    Entity ent = new Entity("new_payment");

                    if (!string.IsNullOrEmpty(_payment.Name)) { ent["new_name"] = _payment.Name; }
                    if (_payment.Contact != null) { ent["new_contactid"] = _payment.Contact; }
                    if (_payment.Account != null) { ent["new_accountid"] = _payment.Account; }
                    if (_payment.Quote != null) { ent["new_quoteid"] = _payment.Quote; }
                    if (_payment.Currency != null) { ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", _payment.Currency.Id); }
                    if (_payment.Owner != null) { ent["ownerid"] = _payment.Owner; }
                    if (_payment.PaymentType != null) { ent["new_type"] = new OptionSetValue((int)_payment.PaymentType); }
                    if (_payment.PaymentCashType != null) { ent["new_itype"] = new OptionSetValue((int)_payment.PaymentCashType); }
                    if (_payment.PaymentDate != null) { ent["new_date"] = _payment.PaymentDate; }
                    if (_payment.PaymentStatus != null) { ent["statuscode"] = new OptionSetValue((int)_payment.PaymentStatus); }
                    if (_payment.PaymentAccountingTypes != null) { ent["new_vtype"] = new OptionSetValue((int)_payment.PaymentAccountingTypes); }
                    if (_payment.FinancialAccount != null) { ent["new_financialaccountid"] = new EntityReference("new_financialaccount", _payment.FinancialAccount.Id); }


                    if (_payment.PaymentId != Guid.Empty)
                    {
                        ent["new_paymentid"] = _payment.PaymentId;
                        service.Update(ent);
                        returnValue.Result = "Ödeme kaydı başarıyla güncelleştirildi.";
                        returnValue.Success = true;
                    }
                    else
                    {
                        if (_payment.PaymentAmount != null)
                        {
                            ent["new_paymentamount"] = new Money(((decimal)_payment.PaymentAmount * (decimal)collaborate["new_salescollaboraterate"]) / 100);
                        }
                        //if (_payment.PaymentAmount != null && projectId != Globals.TopkapiProjectId)
                        //{
                        //    ent["new_paymentamount"] = new Money(((decimal)_payment.PaymentAmount * (decimal)collaborate["new_salescollaboraterate"]) / 100);
                        //}
                        //else if (_payment.PaymentAmount != null && projectId == Globals.TopkapiProjectId)
                        //{
                        //    ent["new_paymentamount"] = new Money((decimal)_payment.PaymentAmount);
                        //}
                        ent["new_collaborateaccountid"] = collaborate.Attributes["new_accountid"];

                        returnValue.CrmId = service.Create(ent);
                        returnValue.Result = "Ödeme kaydı başarıyla oluşturuldu.";
                        returnValue.Success = true;
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



        /// <summary>
        /// Teklif üzerindeki kayıtlar çiftlenmemesi için silinir.
        /// </summary>
        /// <param name="quoteId">Teklif Id</param>
        /// <param name="service">servis</param>
        /// <param name="sda">sql data access</param>
        public static void DeletePaymentsIfExist(Guid quoteId, IOrganizationService service, SqlDataAccess sda)
        {
            #region | SQL QUERY |
            string query = @"SELECT
	                                P.new_paymentId                            
                                FROM
                                    new_payment P WITH (NOLOCK)
                                WHERE
                                    P.new_quoteid = '{0}'
                                AND 
	                                P.new_type <> 4";

            #endregion

            DataTable dt = sda.getDataTable(string.Format(query, quoteId));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow currentPayment in dt.Rows)
                {
                    service.Delete("new_payment", new Guid(Convert.ToString(currentPayment[0])));
                }
            }

        }

        public static MsCrmResultObject GetPaymentDetail(Guid paymentId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_paymentId Id
	                                ,P.new_name Name
	                                ,P.new_contactid ContactId
	                                ,P.new_contactidName ContactIdName
	                                ,P.new_quoteid QuoteId 
	                                ,P.new_quoteidName QuoteIdName
	                                ,P.transactioncurrencyId CurrencyId 
	                                ,P.transactioncurrencyIdName CurrencyIdName
	                                ,P.ownerId OwnerId
	                                ,P.ownerIdName OwnerIdName
	                                ,P.new_paymentamount PaymentAmount
	                                ,P.new_type PaymentType
	                                ,P.new_itype PaymentCashType
                                    ,P.new_date PaymentDate
                                    ,P.CreatedOn
                                FROM
	                                new_payment P WITH (NOLOCK)
                                WHERE
	                                P.new_paymentId = '{0}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, paymentId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Payment _payment = new Payment();
                        _payment.PaymentId = (Guid)dt.Rows[i]["Id"];

                        if (dt.Rows[i]["ContactId"] != DBNull.Value) { _payment.Contact = new EntityReference() { Id = (Guid)dt.Rows[i]["ContactId"], Name = dt.Rows[i]["ContactIdName"].ToString(), LogicalName = "contact" }; }
                        if (dt.Rows[i]["QuoteId"] != DBNull.Value) { _payment.Quote = new EntityReference() { Id = (Guid)dt.Rows[i]["QuoteId"], Name = dt.Rows[i]["QuoteIdName"].ToString(), LogicalName = "quote" }; }
                        if (dt.Rows[i]["CurrencyId"] != DBNull.Value) { _payment.Currency = new EntityReference() { Id = (Guid)dt.Rows[i]["CurrencyId"], Name = dt.Rows[i]["CurrencyIdName"].ToString(), LogicalName = "transactioncurrency" }; }
                        if (dt.Rows[i]["OwnerId"] != DBNull.Value) { _payment.Owner = new EntityReference() { Id = (Guid)dt.Rows[i]["OwnerId"], Name = dt.Rows[i]["OwnerIdName"].ToString(), LogicalName = "systemuser" }; }
                        _payment.PaymentDateString = dt.Rows[i]["PaymentDate"] != DBNull.Value ? ((DateTime)dt.Rows[i]["PaymentDate"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                        _payment.CreatedOnString = dt.Rows[i]["CreatedOn"] != DBNull.Value ? ((DateTime)dt.Rows[i]["CreatedOn"]).ToLocalTime().ToString("dd.MM.yyyy HH:mm") : "";

                        if (dt.Rows[i]["PaymentAmount"] != DBNull.Value) { _payment.PaymentAmount = (decimal)dt.Rows[i]["PaymentAMount"]; }
                        if (dt.Rows[i]["PaymentType"] != DBNull.Value) { _payment.PaymentType = (PaymentTypes)dt.Rows[i]["PaymentType"]; }
                        if (dt.Rows[i]["PaymentCashType"] != DBNull.Value) { _payment.PaymentCashType = (PaymentCashTypes)dt.Rows[i]["PaymentCashType"]; }
                        if (dt.Rows[i]["PaymentDate"] != DBNull.Value) { _payment.PaymentDate = ((DateTime)dt.Rows[i]["PaymentDate"]).ToLocalTime(); }
                        if (dt.Rows[i]["CreatedOn"] != DBNull.Value) { _payment.CreatedOnDate = ((DateTime)dt.Rows[i]["CreatedOn"]).ToLocalTime(); }

                        returnValue.Success = true;
                        returnValue.ReturnObject = _payment;
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

        internal static void UpdateAmount(Entity entity, Entity preImage, IOrganizationService adminService)
        {
            //// new_type //1 ise Ara Ödeme , 2 ise Düzenli Taksit
            //Entity payment = adminService.Retrieve("new_payment", entity.Id, new ColumnSet("new_quoteid", "new_date"));
            //Guid quoteId = ((EntityReference)payment.Attributes["new_quoteid"]).Id;
            //DateTime date = (DateTime)payment.Attributes["new_date"];//Vade Tarihi
            //Guid collaborateAccountId = preImage.Contains("new_collaborateaccountid") ? ((EntityReference)preImage.Attributes["new_collaborateaccountid"]).Id : Guid.Empty;

            //decimal sumInterval = 0;
            //decimal instAmount = 0;
            //int type = ((OptionSetValue)entity.Attributes["new_type"]).Value;
            //decimal paymentAmount = ((Money)entity.Attributes["new_paymentamount"]).Value;
            //Entity quote = adminService.Retrieve("quote", quoteId, new ColumnSet("new_suminterval", "new_instamount"));
            //if (quote.Contains("new_suminterval"))
            //{
            //    sumInterval = GetSumInterval(quoteId, collaborateAccountId, adminService);//((Money)quote.Attributes["new_suminterval"]).Value;
            //}
            //if (quote.Contains("new_instamount"))
            //{
            //    instAmount = GetSumInstallment(quoteId, collaborateAccountId, adminService); //((Money)quote.Attributes["new_instamount"]).Value;
            //}

            //decimal totalHistoryAmount = GetTotalHistoryAmount(quoteId, date, type, collaborateAccountId, adminService);
            //if (type == 1)
            //{
            //    totalHistoryAmount = sumInterval - totalHistoryAmount - paymentAmount;
            //}
            //else if (type == 2)
            //{
            //    totalHistoryAmount = instAmount - totalHistoryAmount - paymentAmount;
            //}
            //else
            //{
            //    return;
            //}

            //ConditionExpression con1 = new ConditionExpression();
            //con1.AttributeName = "new_quoteid";
            //con1.Operator = ConditionOperator.Equal;
            //con1.Values.Add(quoteId);

            //ConditionExpression con2 = new ConditionExpression();
            //con2.AttributeName = "new_date";
            //con2.Operator = ConditionOperator.GreaterThan;
            //con2.Values.Add(date);

            //ConditionExpression con3 = new ConditionExpression();
            //con3.AttributeName = "new_type";
            //con3.Operator = ConditionOperator.Equal;
            //con3.Values.Add(type);

            //ConditionExpression con4 = new ConditionExpression();
            //con4.AttributeName = "new_date";
            //con4.Operator = ConditionOperator.LessThan;
            //con4.Values.Add(date);

            //ConditionExpression con5 = new ConditionExpression();
            //con5.AttributeName = "statecode";
            //con5.Operator = ConditionOperator.Equal;
            //con5.Values.Add(0);

            //ConditionExpression con6 = new ConditionExpression();
            //con6.AttributeName = "new_collaborateaccountid";
            //con6.Operator = ConditionOperator.Equal;
            //con6.Values.Add(collaborateAccountId);


            //FilterExpression filter = new FilterExpression();
            //filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(con1);
            //filter.Conditions.Add(con2);
            //filter.Conditions.Add(con3);
            //if (totalHistoryAmount == 0)
            //{
            //    filter.Conditions.Add(con4);
            //}
            //filter.Conditions.Add(con5);
            //filter.Conditions.Add(con6);

            //QueryExpression Query = new QueryExpression("new_payment");
            //Query.ColumnSet = new ColumnSet("new_paymentid");//Ödenmesi Gereken Tutar
            //Query.Criteria.FilterOperator = LogicalOperator.And;
            //Query.Criteria.Filters.Add(filter);

            //EntityCollection Result = adminService.RetrieveMultiple(Query);
            //if (Result.Entities.Count > 0)
            //{
            //    foreach (Entity p in Result.Entities)
            //    {
            //        Entity paymentUpdate = new Entity("new_payment");
            //        paymentUpdate.Id = p.Id;
            //        paymentUpdate.Attributes["new_paymentamount"] = new Money(totalHistoryAmount / Result.Entities.Count);
            //        adminService.Update(paymentUpdate);

            //    }

            //}
            entity["new_vnumber"] = null;
            entity["new_isupdated"] = false;
        }

        private static decimal GetSumInstallment(Guid quoteId, Guid collaborateAccountId, IOrganizationService adminService)
        {
            decimal sumInterval = 0;
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_type";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(2);//Düzenli Taksit

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "statecode";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(0);

            ConditionExpression con4 = new ConditionExpression();
            con4.AttributeName = "new_collaborateaccountid";
            con4.Operator = ConditionOperator.Equal;
            con4.Values.Add(collaborateAccountId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);
            filter.Conditions.Add(con4);



            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");//Ödenmesi Gereken Tutar
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = adminService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    sumInterval += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return sumInterval;
        }

        private static decimal GetSumInterval(Guid quoteId, Guid collaborateAccountId, IOrganizationService adminService)
        {
            decimal sumInterval = 0;
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_type";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(1);//Ara Ödeme

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "statecode";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(0);

            ConditionExpression con4 = new ConditionExpression();
            con4.AttributeName = "new_collaborateaccountid";
            con4.Operator = ConditionOperator.Equal;
            con4.Values.Add(collaborateAccountId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);
            filter.Conditions.Add(con4);



            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");//Ödenmesi Gereken Tutar
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = adminService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    sumInterval += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return sumInterval;
        }

        private static decimal GetTotalHistoryAmount(Guid quoteId, DateTime date, int type, Guid collaborateAccountId, IOrganizationService adminService)
        {
            decimal totalHistoryAmount = 0;
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_date";
            con2.Operator = ConditionOperator.LessThan;
            con2.Values.Add(date);

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_type";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(type);

            ConditionExpression con4 = new ConditionExpression();
            con4.AttributeName = "statecode";
            con4.Operator = ConditionOperator.Equal;
            con4.Values.Add(0);

            ConditionExpression con5 = new ConditionExpression();
            con5.AttributeName = "new_collaborateaccountid";
            con5.Operator = ConditionOperator.Equal;
            con5.Values.Add(collaborateAccountId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);
            filter.Conditions.Add(con4);
            filter.Conditions.Add(con5);



            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");//Ödenmesi Gereken Tutar
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = adminService.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    totalHistoryAmount += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return totalHistoryAmount;
        }

        public static MsCrmResultObject GetQuotePrePayments(Guid quoteId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_paymentId PaymentId
                                FROM
	                                new_payment AS P WITH (NOLOCK)
                                WHERE
	                                P.new_type = {0}
                                    AND 
	                                P.new_quoteid = '{1}'
                                    AND
                                    P.StateCode = 0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)PaymentTypes.KaporaOdemesi, quoteId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Payment> returnList = new List<Payment>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Payment _payment = new Payment();
                        _payment.PaymentId = (Guid)dt.Rows[i]["PaymentId"];

                        returnList.Add(_payment);
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult QuoteHasPrePayment(Guid quoteId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_paymentId PaymentId
                                FROM
	                                new_payment AS P WITH (NOLOCK)
                                WHERE
	                                P.new_type = {0}
                                    AND 
	                                P.new_quoteid = '{1}'
                                    AND
                                    P.StateCode = 0";
                #endregion
                IOrganizationService service = MSCRM.GetOrgService(true);
                DataTable dt = sda.getDataTable(string.Format(query, (int)PaymentTypes.KaporaOdemesi, quoteId));
                Entity product = GetProductByQuoteId(service, quoteId);
                if (product == null)
                {
                    returnValue.Success = false;
                    return returnValue;
                }
                Guid projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
                EntityCollection list = GetProjectSalesCollaborateList(service, projectId);
                if (dt != null && (dt.Rows.Count >= list.Entities.Count))//Ortaklık yapısı 2 olduğu için ÖDeme kaydı 2 adet oluşuyor
                {
                    returnValue.CrmId = (Guid)dt.Rows[0]["PaymentId"];
                    returnValue.Result = "Satışla ilgili kapora kaydı bulunmaktadır!";
                    returnValue.Success = true;
                }
                //else if (dt != null && dt.Rows.Count == 0)
                else
                {
                    returnValue.Result = "Satışla ilgili kapora kaydı bulunmamaktadır!";
                    returnValue.Success = false;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return returnValue;
        }

        internal static void UpdatePaymentCustomers(Guid quoteId, EntityReference customer, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentid");//Ödenmesi Gereken Tutar
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);

            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {

                    Entity payment = new Entity("new_payment");
                    payment.Id = p.Id;

                    if (customer != null)
                    {
                        if (customer.LogicalName == "contact")
                        {
                            payment["new_contactid"] = customer;
                            payment["new_accountid"] = null;
                        }
                        else if (customer.LogicalName == "account")
                        {
                            payment["new_accountid"] = customer;
                            payment["new_contactid"] = null;
                        }
                    }
                    else
                    {
                        payment["new_contactid"] = null;
                        payment["new_accountid"] = null;
                    }
                    service.Update(payment);
                }
            }
        }

        internal static void UpdatePayments(Guid quoteId, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentid");//Ödenmesi Gereken Tutar
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);

            foreach (Entity p in Result.Entities)
            {

                Entity payment = new Entity("new_payment");
                payment.Id = p.Id;
                payment["new_vstatus"] = new OptionSetValue(22);//Senet Durumu Satış İptal
                service.Update(payment);


                SetStateRequest setStateReq = new SetStateRequest();
                setStateReq.EntityMoniker = new EntityReference("new_payment", p.Id);
                setStateReq.State = new OptionSetValue(1);
                setStateReq.Status = new OptionSetValue(100000001);//Satış İptal
                SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
            }
        }

        internal static void SetVoucherNumber(Guid quoteId, string contractNumber, IOrganizationService service, SqlDataAccess sda)
        {
            if (contractNumber == string.Empty)
                return;

            #region SQL QUERY
            string sqlQuery = @"SELECT 
	                                TOP 1
	                                SUBSTRING(p.new_vnumber,(len(p.new_vnumber)-2),len(p.new_vnumber)) as lastNumber
                                 FROM 
	                                new_payment P (NOLOCK)
                                 WHERE P.new_quoteid ='{0}'
                                 and
                                 P.new_vnumber IS NOT NULL
                                 ORDER BY
	                                CONVERT(int ,SUBSTRING(p.new_vnumber,(len(p.new_vnumber)-2),len(p.new_vnumber))) DESC";
            sqlQuery = string.Format(sqlQuery, quoteId);
            DataTable dt = sda.getDataTable(sqlQuery);
            #endregion SQL QUERY

            #region Set NUMBER
            int lastNumber = 0;
            string voucherNumber = dt.Rows.Count > 0 ? dt.Rows[0]["lastNumber"] != DBNull.Value ? dt.Rows[0]["lastNumber"].ToString() : string.Empty : string.Empty;
            if (voucherNumber != string.Empty)
            {
                lastNumber = Convert.ToInt32(voucherNumber.Substring(voucherNumber.Length - 3));
            }

            #endregion Set NUMBER

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            //ConditionExpression con2 = new ConditionExpression();
            //con2.AttributeName = "new_isvoucher";
            //con2.Operator = ConditionOperator.Equal;
            //con2.Values.Add(true);

            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_vnumber";
            con3.Operator = ConditionOperator.Null;


            ConditionExpression con4 = new ConditionExpression();
            con4.AttributeName = "new_type";
            con4.Operator = ConditionOperator.NotEqual;
            con4.Values.Add(3);

            ConditionExpression con5 = new ConditionExpression();
            con5.AttributeName = "new_type";
            con5.Operator = ConditionOperator.NotEqual;
            con5.Values.Add(4);




            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            //filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);
            filter.Conditions.Add(con4);
            filter.Conditions.Add(con5);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            foreach (Entity p in Result.Entities)
            {
                Entity _p = new Entity("new_payment");
                _p.Id = p.Id;
                _p.Attributes["new_isvoucher"] = true;
                _p.Attributes["new_vnumber"] = contractNumber + "." + (lastNumber + Result.Entities.IndexOf(p) + 1).ToString().PadLeft(3, '0');
                service.Update(_p);
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

        private static EntityCollection GetProjectSalesCollaborateList(IOrganizationService service, Guid projectId)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_projectid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(projectId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_accountid";
            con2.Operator = ConditionOperator.NotEqual;
            con2.Values.Add(Globals.NEFSalesCollaboraterateId);//Proje Ortaklığı üzerindeki Ortaklık yapılan firma (Timur yapı A.Ş) NEF

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            if (projectId == Globals.TopkapiProjectId)
            {
                // filter.Conditions.Add(con2);
            }

            QueryExpression Query = new QueryExpression("new_projectsalescollaborate");
            Query.ColumnSet = new ColumnSet("new_salescollaboraterate", "new_accountid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            return service.RetrieveMultiple(Query);
        }

        internal static void UpdatePaymentsFinancialAccount(IOrganizationService service, Guid quoteId, Guid financialAccountId, EntityReference customer)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);

            foreach (Entity p in Result.Entities)
            {

                Entity payment = new Entity("new_payment");
                payment.Id = p.Id;
                payment["new_financialaccountid"] = new EntityReference("new_financialaccount", financialAccountId);
                if (customer.LogicalName == "contact")
                {
                    payment["new_contactid"] = new EntityReference("contact", customer.Id);
                }
                else if (customer.LogicalName == "account")
                {
                    payment["new_accountid"] = new EntityReference("account", customer.Id);
                }
                service.Update(payment);
            }
        }

        internal static void UpdateFinancialAccount(Guid quoteId, Guid financialAccountId, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);

            foreach (Entity p in Result.Entities)
            {

                Entity payment = new Entity("new_payment");
                payment.Id = p.Id;
                payment["new_financialaccountid"] = new EntityReference("new_financialaccount", financialAccountId);
                service.Update(payment);
            }
        }
    }
}
