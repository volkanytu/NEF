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
    public static class InterestProductHelper
    {
        public static MsCrmResultObject GetPhoneCallInterestedProjects(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedproductsId Id
                                    ,P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
	                                ,P.new_blockid BlockId
	                                ,P.new_blockidName BlockIdName
	                                ,P.new_generaltypeofhomeid GeneralHomeTypeId
	                                ,P.new_generaltypeofhomeidName GeneralHomeTypeIdName
	                                ,P.new_typeofhomeid HomeTypeId
	                                ,P.new_typeofhomeidName HomeTypeIdName
	                                ,P.new_floornumber FloorNumber
	                                ,P.new_homenumber HomeNumber
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                IP.new_phonecallid = '{0}'
	                                AND
	                                P.ProductId = IP.new_productid
                                WHERE
	                                IP.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PHONECALL INTEREST PRODUCTS |
                    List<InterestProduct> returnList = new List<InterestProduct>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InterestProduct _interest = new InterestProduct();
                        _interest.InterestProductId = (Guid)dt.Rows[i]["Id"];

                        if (dt.Rows[i]["ProductId"] != DBNull.Value)
                        {
                            Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);
                            _interest.InterestedProduct = _product;
                        }

                        returnList.Add(_interest);
                    }
                    #endregion


                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aktiviye ait ilgilendiği bir konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetAppointmentInterestedProjects(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedproductsId Id
                                    ,P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
	                                ,P.new_blockid BlockId
	                                ,P.new_blockidName BlockIdName
	                                ,P.new_generaltypeofhomeid GeneralHomeTypeId
	                                ,P.new_generaltypeofhomeidName GeneralHomeTypeIdName
	                                ,P.new_typeofhomeid HomeTypeId
	                                ,P.new_typeofhomeidName HomeTypeIdName
	                                ,P.new_floornumber FloorNumber
	                                ,P.new_homenumber HomeNumber
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                IP.new_appointmentid = '{0}'
	                                AND
	                                P.ProductId = IP.new_productid
                                WHERE
	                                IP.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET APPOINTMENT INTEREST PRODUCTS |
                    List<InterestProduct> returnList = new List<InterestProduct>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InterestProduct _interest = new InterestProduct();
                        _interest.InterestProductId = (Guid)dt.Rows[i]["Id"];

                        if (dt.Rows[i]["ProductId"] != DBNull.Value)
                        {
                            Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);
                            _interest.InterestedProduct = _product;
                        }

                        returnList.Add(_interest);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aktiviye ait ilgilendiği bir konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivityInterestedProjects(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedproductsId Id
                                    ,P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
	                                ,P.new_blockid BlockId
	                                ,P.new_blockidName BlockIdName
	                                ,P.new_generaltypeofhomeid GeneralHomeTypeId
	                                ,P.new_generaltypeofhomeidName GeneralHomeTypeIdName
	                                ,P.new_typeofhomeid HomeTypeId
	                                ,P.new_typeofhomeidName HomeTypeIdName
	                                ,P.new_floornumber FloorNumber
	                                ,P.new_homenumber HomeNumber
                                    ,P.new_locationid LocationId
	                                ,P.new_locationidName LocationIdName
	                                ,P.Price Price
									,P.TransactionCurrencyId
									,P.TransactionCurrencyIdName
                                    ,P.StatusCode
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
                                    (
                                        IP.new_phonecallid = '{0}'
                                        OR
	                                    IP.new_appointmentid = '{0}'
                                    )
	                                AND
	                                P.ProductId = IP.new_productid
                                WHERE
	                                IP.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY INTEREST PRODUCTS |
                    List<InterestProduct> returnList = new List<InterestProduct>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InterestProduct _interest = new InterestProduct();
                        _interest.InterestProductId = (Guid)dt.Rows[i]["Id"];

                        if (dt.Rows[i]["ProductId"] != DBNull.Value)
                        {
                            Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);
                            _interest.InterestedProduct = _product;
                        }

                        returnList.Add(_interest);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aktiviye ait ilgilendiği bir konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivityInterestedProjectsForSR(Guid activityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedproductsId Id
                                    ,P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
	                                ,P.new_blockid BlockId
	                                ,P.new_blockidName BlockIdName
	                                ,P.new_generaltypeofhomeid GeneralHomeTypeId
	                                ,P.new_generaltypeofhomeidName GeneralHomeTypeIdName
	                                ,P.new_typeofhomeid HomeTypeId
	                                ,P.new_typeofhomeidName HomeTypeIdName
	                                ,P.new_floornumber FloorNumber
	                                ,P.new_homenumber HomeNumber
                                    ,P.new_locationid LocationId
	                                ,P.new_locationidName LocationIdName
	                                ,P.Price Price
									,P.TransactionCurrencyId
									,P.TransactionCurrencyIdName
                                    ,P.StatusCode
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
                                    (
                                        IP.new_phonecallid = '{0}'
                                        OR
	                                    IP.new_appointmentid = '{0}'
                                    )
	                                AND
	                                P.ProductId = IP.new_productid
                               
                                WHERE
	                                IP.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, activityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET ACTIVITY INTEREST PRODUCTS |
                    List<InterestProduct> returnList = new List<InterestProduct>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InterestProduct _interest = new InterestProduct();
                        _interest.InterestProductId = (Guid)dt.Rows[i]["Id"];

                        if (dt.Rows[i]["ProductId"] != DBNull.Value)
                        {
                            Product _product = ProductHelper.GetProductDetailForSR((Guid)dt.Rows[i]["ProductId"], sda);
                            _interest.InterestedProduct = _product;
                        }

                        returnList.Add(_interest);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aktiviye ait ilgilendiği bir konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult RemoveInterestedHouse(Guid interestedHouseId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                SetStateRequest stateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference("new_interestedproducts", interestedHouseId),
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(2)
                };

                SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        /// <summary>
        /// Konut önceden aktiviteye ilgilendiği konut olarak eklenmiş mi kontrol eder.
        /// Telefon görüşmesine ve randevuyu ayrı ayrı kontrol etmek için ikiside parametre olarak alınmıştır.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="phonecallId"></param>
        /// <param name="appointmentId"></param>
        /// <param name="sda"></param>
        /// <returns></returns>
        public static MsCrmResult HasAddedInterestedHouse(Guid productId, Guid phonecallId, Guid appointmentId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                COUNT(0) Sonuc
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                WHERE
	                                IP.new_productid = '{0}'
	                                AND
	                                IP.StateCode = 0";
                if (phonecallId != Guid.Empty)
                {
                    query += " AND IP.new_phonecallid = '{1}'";
                    query = string.Format(query, productId, phonecallId);
                }
                else
                {
                    query += " AND IP.new_appointmentid = '{1}'";
                    query = string.Format(query, productId, appointmentId);
                }
                #endregion

                int sonuc = (int)sda.ExecuteScalar(query);
                if (sonuc > 0)
                {
                    returnValue.Success = false;
                    returnValue.Result = "Seçilen konut aktivitede ilgilendiği konut olarak bulunmaktadır!";
                }
                else
                {
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult CreateInterestHouse(InterestProduct interestedHouse, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_interestedproducts");
                ent["new_name"] = DateTime.Now.ToShortDateString();
                ent["new_productid"] = new EntityReference("product", interestedHouse.InterestedProduct.ProductId);

                if (interestedHouse.PhoneCall != null)
                {
                    ent["new_phonecallid"] = interestedHouse.PhoneCall;
                }
                else
                {
                    ent["new_appointmentid"] = interestedHouse.Appointment;
                }

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Konut başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult InterestHouseHasSameProduct(Guid productId, Guid activityId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedproductsId Id
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                WHERE
	                                IP.new_productid = '{0}'
	                                AND
	                                IP.statecode = 0
	                                AND
	                                (
		                                IP.new_phonecallid = '{1}'
		                                OR
		                                IP.new_appointmentid = '{1}'
	                                )";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, productId, activityId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = false;
                    returnValue.Result = "Konut önceden eklenmiş.";
                }
                else
                {
                    returnValue.Success = true;
                    returnValue.Result = "Konut eklenebilir.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetInterestedHouseDetail(Guid interestedHouseId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                IP.new_interestedproductsId Id
                                    ,IP.new_productid ProductId
	                                ,IP.new_productidName ProductIdName
	                                ,IP.new_phonecallid PhoneCallId
	                                ,IP.new_phonecallidName PhoneCallIdName
	                                ,IP.new_appointmentid AppointmentId
	                                ,IP.new_appointmentidName AppointmentIdName
                                FROM
	                                new_interestedproducts IP WITH (NOLOCK)
                                WHERE
	                                IP.StateCode = 0
	                                AND
	                                IP.new_interestedproductsId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, interestedHouseId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET INTEREST PRODUCT DETAIL |
                    InterestProduct _interest = new InterestProduct();
                    _interest.InterestProductId = (Guid)dt.Rows[0]["Id"];

                    if (dt.Rows[0]["ProductId"] != DBNull.Value)
                    {
                        Product _product = new Product();
                        _product.ProductId = (Guid)dt.Rows[0]["ProductId"];
                        _product.Name = dt.Rows[0]["ProductIdName"] != DBNull.Value ? dt.Rows[0]["ProductIdName"].ToString() : string.Empty;

                        _interest.InterestedProduct = _product;
                    }

                    if (dt.Rows[0]["PhoneCallId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["PhoneCallId"];
                        if (dt.Rows[0]["PhoneCallIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["PhoneCallIdName"].ToString(); }
                        er.LogicalName = "phonecall";

                        _interest.PhoneCall = er;
                    }

                    if (dt.Rows[0]["AppointmentId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["AppointmentId"];
                        if (dt.Rows[0]["AppointmentIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["AppointmentIdName"].ToString(); }
                        er.LogicalName = "appointment";

                        _interest.Appointment = er;
                    }
                    #endregion


                    returnValue.Success = true;
                    returnValue.ReturnObject = _interest;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata ile karşılaşıldı!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static void UpdateContact(Entity entity, IOrganizationService service)
        {
            Guid productId = entity.Contains("new_productid") ? ((EntityReference)entity.Attributes["new_productid"]).Id : Guid.Empty;
            Guid phoneCallId = entity.Contains("new_phonecallid") ? ((EntityReference)entity.Attributes["new_phonecallid"]).Id : Guid.Empty;
            Guid appointmentId = entity.Contains("new_appointmentid") ? ((EntityReference)entity.Attributes["new_appointmentid"]).Id : Guid.Empty;
            if (productId == Guid.Empty)
                return;
            if (phoneCallId == Guid.Empty && appointmentId == Guid.Empty)
                return;
            Guid contactId = Guid.Empty;
            Entity product = service.Retrieve("product", productId, new ColumnSet("new_unittypeid", "new_generaltypeofhomeid"));
            string unitType = product.Contains("new_unittypeid") ? ((EntityReference)product.Attributes["new_unittypeid"]).Name : string.Empty;
            string generalTypeOfHome = product.Contains("new_generaltypeofhomeid") ? ((EntityReference)product.Attributes["new_generaltypeofhomeid"]).Name : string.Empty;

            #region GET CONTACT

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "activityid";
            con1.Operator = ConditionOperator.Equal;
            if (appointmentId != Guid.Empty)
            {
                con1.Values.Add(appointmentId);
            }
            else if (phoneCallId != Guid.Empty)
            {
                con1.Values.Add(phoneCallId);
            }

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "partyobjecttypecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(2);//Contact

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression("activityparty");
            Query.ColumnSet = new ColumnSet("partyid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                contactId = ((EntityReference)Result.Entities[0].Attributes["partyid"]).Id;
            }
            else
            {
                return;
            }

            #endregion GET CONTACT

            Entity c = new Entity("contact");
            c.Id = contactId;
            switch (unitType)
            {
                case "Ofis":
                    c.Attributes["new_preferenceoffice"] = true;
                    break;
                case "Konut":
                    c.Attributes["new_preferencehome"] = true;
                    break;
                case "Mağaza":
                    c.Attributes["new_preferencestore"] = true;
                    break;
            }
            if (generalTypeOfHome.Contains("1+1"))
            {
                c.Attributes["new_1plus1"] = true;
            }
            else if (generalTypeOfHome.Contains("2+1"))
            {
                c.Attributes["new_2plus1"] = true;

            }
            else if (generalTypeOfHome.Contains("3+1"))
            {
                c.Attributes["new_3plus1"] = true;

            }
            else if (generalTypeOfHome.Contains("4+1"))
            {

                c.Attributes["new_4plus1"] = true;
            }
            service.Update(c);
        }
    }
}
