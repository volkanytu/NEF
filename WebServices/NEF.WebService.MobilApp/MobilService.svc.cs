using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
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

namespace NEF.WebService.MobilApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MobilService : IMobilService
    {
        private string SuccessMessage = "Başarılıdır";
        public EventLogHelper eventLog;

        public string Login(string emailAddress)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            CustomerInfo customerInfo = new CustomerInfo();

            CustomerDetailResult customerDetailResult = new CustomerDetailResult();
            MsCrmResult result = new MsCrmResult();

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                result.Success = false;
                result.Message = "E-Posta Adresi Boş Olamaz";
                customerDetailResult.Result = result;

                returnValue = ser.Serialize(customerDetailResult);
                return returnValue;
            }

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string loginContactQuery = @"SELECT
                                          C.ContactId
                                         FROM
                                         Contact C WITH(NOLOCK)
                                         WHERE
                                         C.EMailAddress1='{0}'
                                         AND
                                         C.StateCode=0";

            string loginAccountQuery = @"SELECT
                                          A.AccountId
                                         FROM
                                         Account A WITH(NOLOCK)
                                         WHERE
                                         A.EMailAddress1='{0}'
                                         AND
                                         A.StateCode=0";

            string dataContactQuery = @"SELECT
                                        C.ContactId
                                       ,C.new_Number
                                       ,C.FirstName
                                       ,C.LastName
                                       ,C.EMailAddress1
                                       ,C.MobilePhone
                                       ,C.new_tcidentitynumber
                                       ,(SELECT
                                          SM.Value 
                                         FROM StringMap SM WITH(NOLOCK) 
                                         WHERE
                                         SM.AttributeName='new_customertype'
                                         AND
                                         SM.AttributeValue=C.new_customertype
                                         AND
                                         SM.LangId=1055
                                         AND
                                         SM.ObjectTypeCode=2) CType
                                       ,P.new_projectcode
                                       ,Q.new_projectidName
                                       ,Q.new_productid
                                       ,Q.new_productidName
                                       FROM
                                       Contact C WITH(NOLOCK)
                                       LEFT JOIN Quote Q WITH(NOLOCK)
                                       ON C.ContactId=Q.CustomerId
									   LEFT JOIN new_project P WITH(NOLOCK)
									   ON Q.new_projectid=P.new_projectId
                                       WHERE
                                       C.ContactId='{0}'
                                       ORDER BY Q.new_projectidName";

            string dataAccountQuery = @"SELECT
                                         A.AccountId
                                        ,A.Name
                                        ,A.EMailAddress1
                                        ,A.Telephone1
                                        ,A.new_taxnumber
                                        ,P.new_projectcode
                                        ,Q.new_projectidName
                                        ,Q.new_productid
                                        ,Q.new_productidName
                                        FROM
                                        Account A WITH(NOLOCK)
                                        LEFT JOIN Quote Q WITH(NOLOCK)
                                        ON A.AccountId=Q.CustomerId
									    LEFT JOIN new_project P WITH(NOLOCK)
									    ON Q.new_projectid=P.new_projectId
                                        WHERE
                                        A.AccountId='{0}'
                                        ORDER BY Q.new_projectidName";

            try
            {
                DataTable loginTable = sda.getDataTable(string.Format(loginContactQuery, emailAddress));
                string contactId = string.Empty;
                string accountId = string.Empty;
                if (loginTable.Rows.Count > 0)
                {
                    contactId = loginTable.Rows[0]["ContactId"] != DBNull.Value ? Convert.ToString(loginTable.Rows[0]["ContactId"]) : string.Empty;
                }
                else
                {
                    loginTable = sda.getDataTable(string.Format(loginAccountQuery, emailAddress));
                    accountId = loginTable.Rows[0]["AccountId"] != DBNull.Value ? Convert.ToString(loginTable.Rows[0]["AccountId"]) : string.Empty;
                }

                if (loginTable.Rows.Count == 0)
                {
                    result.Success = false;
                    result.Message = "E-Posta Adresi Geçersiz";
                }

                DataTable customerInfoTable = new DataTable();
                if (!string.IsNullOrWhiteSpace(contactId))
                {
                    customerInfoTable = sda.getDataTable(string.Format(dataContactQuery, contactId));
                    sda.closeConnection();
                    if (customerInfoTable.Rows.Count > 0)
                    {
                        DataRow masterCustomerInfo = customerInfoTable.Rows[0];
                        string Id = masterCustomerInfo["ContactId"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["ContactId"]) : string.Empty;
                        string number = masterCustomerInfo["new_Number"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["new_Number"]) : string.Empty;
                        string firstName = masterCustomerInfo["FirstName"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["FirstName"]) : string.Empty;
                        string lastName = masterCustomerInfo["LastName"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["LastName"]) : string.Empty;
                        string emailaddress = masterCustomerInfo["EMailAddress1"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["EMailAddress1"]) : string.Empty;
                        string mobilePhone = masterCustomerInfo["MobilePhone"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["MobilePhone"]) : string.Empty;
                        string identityNumber = masterCustomerInfo["new_tcidentitynumber"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["new_tcidentitynumber"]) : string.Empty;
                        string customerType = masterCustomerInfo["CType"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["CType"]) : string.Empty;

                        customerInfo.CustomerId = Id;
                        customerInfo.Number = number;
                        customerInfo.Name = firstName;
                        customerInfo.Surname = lastName;
                        customerInfo.Phone = mobilePhone;
                        customerInfo.Email = emailaddress;
                        customerInfo.IdentityNumber = identityNumber;
                        customerInfo.Type = customerType;
                        customerInfo.CustomerType = CustomerType.Contact;
                        List<Quote> quoteList = new List<Quote>();
                        foreach (DataRow contactRow in customerInfoTable.Rows)
                        {
                            string projectId = contactRow["new_projectcode"] != DBNull.Value ? Convert.ToString(contactRow["new_projectcode"]) : string.Empty;
                            string projectName = contactRow["new_projectidName"] != DBNull.Value ? Convert.ToString(contactRow["new_projectidName"]) : string.Empty;
                            string productCode = contactRow["new_productid"] != DBNull.Value ? Convert.ToString(contactRow["new_productid"]) : string.Empty;
                            string productName = contactRow["new_productidName"] != DBNull.Value ? Convert.ToString(contactRow["new_productidName"]) : string.Empty;

                            Quote quote = new Quote();
                            quote.ProjectCode = projectId;
                            quote.ProjectName = projectName;
                            quote.ProductId = productCode;
                            quote.ProductName = productName;

                            quoteList.Add(quote);
                        }

                        customerDetailResult.Customer = customerInfo;
                        customerDetailResult.QuoteList = quoteList;
                    }
                }
                else
                {
                    customerInfoTable = sda.getDataTable(string.Format(dataAccountQuery, accountId));
                    sda.closeConnection();
                    if (customerInfoTable.Rows.Count > 0)
                    {
                        DataRow masterCustomerInfo = customerInfoTable.Rows[0];
                        string Id = masterCustomerInfo["AccountId"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["AccountId"]) : string.Empty;
                        string number = masterCustomerInfo["new_Number"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["new_Number"]) : string.Empty;
                        string firstName = masterCustomerInfo["Name"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["Name"]) : string.Empty;
                        string emailaddress = masterCustomerInfo["EMailAddress1"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["EMailAddress1"]) : string.Empty;
                        string phone = masterCustomerInfo["Telephone1"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["Telephone1"]) : string.Empty;
                        string identityNumber = masterCustomerInfo["new_taxnumber"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["new_taxnumber"]) : string.Empty;
                        string customerType = "Gerçek Müşteri"; //Firma Üzerinde Müşteri Tipi Yok. Bu Nedenle Statik Basılması İstendi.


                        customerInfo.CustomerId = Id;
                        customerInfo.Number = number;
                        customerInfo.Name = firstName;
                        customerInfo.Phone = phone;
                        customerInfo.Email = emailaddress;
                        customerInfo.IdentityNumber = identityNumber;
                        customerInfo.Type = customerType;
                        List<Quote> quoteList = new List<Quote>();
                        foreach (DataRow accountRow in customerInfoTable.Rows)
                        {
                            string projectCode = accountRow["new_projectcode"] != DBNull.Value ? Convert.ToString(accountRow["new_projectcode"]) : string.Empty;
                            string projectName = accountRow["new_projectidName"] != DBNull.Value ? Convert.ToString(accountRow["new_projectidName"]) : string.Empty;
                            string productId = accountRow["new_productid"] != DBNull.Value ? Convert.ToString(accountRow["new_productid"]) : string.Empty;
                            string productName = accountRow["new_productidName"] != DBNull.Value ? Convert.ToString(accountRow["new_productidName"]) : string.Empty;

                            Quote quote = new Quote();
                            quote.ProjectCode = projectCode;
                            quote.ProjectName = projectName;
                            quote.ProductId = productId;
                            quote.ProductName = productName;

                            quoteList.Add(quote);
                        }
                        customerDetailResult.Customer = customerInfo;
                        customerDetailResult.QuoteList = quoteList;
                    }
                }
                result.Success = true;
                result.Message = SuccessMessage;

            }
            catch (Exception ex)
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("Login", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            customerDetailResult.Result = result;
            returnValue = ser.Serialize(customerDetailResult);
            return returnValue;
        }

        public string GetPaymentList(string customerId, CustomerType customerType)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            List<PaymentDetail> paymentDetailList = new List<PaymentDetail>();
            PaymentResult paymentResult = new PaymentResult();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                result.Success = false;
                result.Message = "Müşteri Id'si Boş Olamaz";
                paymentResult.Result = result;

                returnValue = ser.Serialize(paymentResult);
                return returnValue;
            }

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            string dataContactQuery = @"SELECT
                                         P.new_paymentId
                                        ,P.new_vnumber
                                        ,P.new_name
                                        ,P.new_amount
                                        ,P.new_paymentamount
                                        ,P.new_balanceamount
                                        ,(SELECT
                                          SM.Value 
                                          FROM StringMap SM WITH(NOLOCK) 
                                          WHERE
                                          SM.AttributeName='new_vstatus'
                                          AND
                                          SM.AttributeValue=P.new_vstatus
                                          AND
                                          SM.LangId=1055
                                          AND
                                          SM.ObjectTypeCode=10040) CStatus
                                        ,(SELECT
                                          SM.Value 
                                          FROM StringMap SM WITH(NOLOCK) 
                                          WHERE
                                          SM.AttributeName='new_type'
                                          AND
                                          SM.AttributeValue=P.new_type
                                          AND
                                          SM.LangId=1055
                                          AND
                                          SM.ObjectTypeCode=10040) CType
                                        ,P.new_paymentdate
                                        ,P.new_date
                                        ,P.TransactionCurrencyIdName
                                        ,C.ContactId CustomerId
                                        ,PT.new_projectcode
                                        ,Q.new_projectidName
                                        ,Q.new_productid
                                        ,Q.new_productidName
                                        FROM
                                        Contact C WITH(NOLOCK)
                                        LEFT JOIN new_payment P WITH(NOLOCK)
                                        ON C.ContactId=P.new_contactid
                                        LEFT JOIN Quote Q WITH(NOLOCK)
                                        ON P.new_quoteid=Q.QuoteId
									    LEFT JOIN new_project PT WITH(NOLOCK)
									    ON Q.new_projectid=PT.new_projectId
                                        WHERE
                                        C.ContactId='{0}'
                                        ORDER BY Q.new_projectidName";

            string dataAccountQuery = @"SELECT
                                         P.new_paymentId
                                        ,P.new_vnumber
                                        ,P.new_name
                                        ,P.new_amount
                                        ,P.new_paymentamount
                                        ,P.new_balanceamount
                                        ,(SELECT
                                          SM.Value 
                                          FROM StringMap SM WITH(NOLOCK) 
                                          WHERE
                                          SM.AttributeName='new_vstatus'
                                          AND
                                          SM.AttributeValue=P.new_vstatus
                                          AND
                                          SM.LangId=1055
                                          AND
                                          SM.ObjectTypeCode=10040) CStatus
                                        ,(SELECT
                                          SM.Value 
                                          FROM StringMap SM WITH(NOLOCK) 
                                          WHERE
                                          SM.AttributeName='new_type'
                                          AND
                                          SM.AttributeValue=P.new_type
                                          AND
                                          SM.LangId=1055
                                          AND
                                          SM.ObjectTypeCode=10040) CType
                                        ,P.new_paymentdate
                                        ,P.new_date
                                        ,P.TransactionCurrencyIdName
                                        ,A.AccountId CustomerId
                                        ,PT.new_projectcode
                                        ,Q.new_projectidName
                                        ,Q.new_productid
                                        ,Q.new_productidName
                                        FROM
                                        Account C WITH(NOLOCK)
                                        LEFT JOIN new_payment P WITH(NOLOCK)
                                        ON A.AccountId=P.new_accountid
                                        LEFT JOIN Quote Q WITH(NOLOCK)
                                        ON P.new_quoteid=Q.QuoteId
									    LEFT JOIN new_project PT WITH(NOLOCK)
									    ON Q.new_projectid=PT.new_projectId
                                        WHERE
                                        A.AccountId='{0}'
                                        ORDER BY Q.new_projectidName";

            DataTable paymentInfoTable = new DataTable();
            try
            {
                if (customerType == CustomerType.Contact)//Contact CRM ObjectTypeCode
                {
                    paymentInfoTable = sda.getDataTable(string.Format(dataContactQuery, customerId));
                }
                else
                {
                    paymentInfoTable = sda.getDataTable(string.Format(dataAccountQuery, customerId));
                }
                sda.closeConnection();

                foreach (DataRow paymentRow in paymentInfoTable.Rows)
                {
                    string Id = paymentRow["new_paymentId"] != DBNull.Value ? Convert.ToString(paymentRow["new_paymentId"]) : string.Empty;
                    string number = paymentRow["new_vnumber"] != DBNull.Value ? Convert.ToString(paymentRow["new_vnumber"]) : string.Empty;
                    string name = paymentRow["new_name"] != DBNull.Value ? Convert.ToString(paymentRow["new_name"]) : string.Empty;
                    decimal amount = paymentRow["new_amount"] != DBNull.Value ? Convert.ToDecimal(paymentRow["new_amount"]) : 0;
                    decimal paymentAmount = paymentRow["new_paymentamount"] != DBNull.Value ? Convert.ToDecimal(paymentRow["new_paymentamount"]) : 0;
                    decimal balanceAmount = paymentRow["new_balanceamount"] != DBNull.Value ? Convert.ToDecimal(paymentRow["new_balanceamount"]) : 0;
                    string transactionName = paymentRow["TransactionCurrencyIdName"] != DBNull.Value ? Convert.ToString(paymentRow["TransactionCurrencyIdName"]) : string.Empty;
                    string status = paymentRow["CStatus"] != DBNull.Value ? Convert.ToString(paymentRow["CStatus"]) : string.Empty;
                    string type = paymentRow["CType"] != DBNull.Value ? Convert.ToString(paymentRow["CType"]) : string.Empty;
                    DateTime paymentDate = paymentRow["new_paymentdate"] != DBNull.Value ? Convert.ToDateTime(paymentRow["new_paymentdate"]) : DateTime.MinValue;
                    DateTime date = paymentRow["new_date"] != DBNull.Value ? Convert.ToDateTime(paymentRow["new_date"]) : DateTime.MinValue;

                    string projectCode = paymentRow["new_projectcode"] != DBNull.Value ? Convert.ToString(paymentRow["new_projectcode"]) : string.Empty;
                    string projectName = paymentRow["new_projectidName"] != DBNull.Value ? Convert.ToString(paymentRow["new_projectidName"]) : string.Empty;
                    string productId = paymentRow["new_productid"] != DBNull.Value ? Convert.ToString(paymentRow["new_productid"]) : string.Empty;
                    string productName = paymentRow["new_productidName"] != DBNull.Value ? Convert.ToString(paymentRow["new_productidName"]) : string.Empty;

                    PaymentDetail paymentDetail = new PaymentDetail();
                    paymentDetail.CustomerId = customerId;
                    paymentDetail.PaymentId = Id;
                    paymentDetail.VNumber = number;
                    paymentDetail.PaymentName = name;
                    paymentDetail.Amount = amount;
                    paymentDetail.BalanceAmount = balanceAmount;
                    paymentDetail.PaymentAmount = paymentAmount;
                    paymentDetail.TransactionName = transactionName;
                    paymentDetail.VStatus = status;
                    paymentDetail.PaymentType = type;
                    paymentDetail.PaymentDate = paymentDate != DateTime.MinValue ? UTCConvertToLocalTime(paymentDate) : paymentDate;
                    paymentDetail.VDate = date != DateTime.MinValue ? UTCConvertToLocalTime(date).ToLocalTime() : date;
                    paymentDetail.ProjectCode = projectCode;
                    paymentDetail.ProjectName = projectName;
                    paymentDetail.ProductId = productId;
                    paymentDetail.ProductName = productName;

                    paymentDetailList.Add(paymentDetail);
                }
                result.Success = true;
                result.Message = SuccessMessage;
                paymentResult.Result = result;
                if (paymentDetailList.Count > 0)
                {
                    paymentResult.CustomerPayment = paymentDetailList;
                }
            }
            catch (Exception ex)
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("GetPaymentList", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            returnValue = ser.Serialize(paymentResult);
            return returnValue;
        }

        public string GetProject()
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            List<Project> projectList = new List<Project>();
            ProjectResult projectResult = new ProjectResult();
            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                string projectQuery = @"SELECT
                                         PT.new_projectcode
                                        ,PT.new_name
                                        FROM
                                        new_project PT WITH(NOLOCK)";
                DataTable projectTable = new DataTable();
                projectTable = sda.getDataTable(projectQuery);
                foreach (DataRow project in projectTable.Rows)
                {
                    string projectCode = project["new_projectcode"] != DBNull.Value ? Convert.ToString(project["new_projectcode"]) : string.Empty;
                    string projectName = project["new_name"] != DBNull.Value ? Convert.ToString(project["new_name"]) : string.Empty;

                    Project tempProject = new Project();
                    tempProject.ProjectCode = projectCode;
                    tempProject.ProjectName = projectName;
                    projectList.Add(tempProject);
                }
                projectResult.ProjectList = projectList;

                result.Success = true;
                result.Message = SuccessMessage;
            }
            catch (Exception ex)
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("GetProject", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }
            projectResult.Result = result;
            returnValue = ser.Serialize(projectResult);
            return returnValue;
        }

        public string GetCustomer(int page, int rowCount, CustomerType customerType)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();

            CustomerProjectListResult customerDetailListResult = new CustomerProjectListResult();
            List<CustomerProject> customerProjectList = new List<CustomerProject>();


            MsCrmResult result = new MsCrmResult();

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            string contactListQuery = @"SELECT
                                        C.ContactId
                                       ,C.new_Number
                                       ,C.FirstName
                                       ,C.LastName
                                       ,C.EMailAddress1
                                       ,C.MobilePhone
                                       ,(SELECT
                                          SM.Value 
                                         FROM StringMap SM WITH(NOLOCK) 
                                         WHERE
                                         SM.AttributeName='new_customertype'
                                         AND
                                         SM.AttributeValue=C.new_customertype
                                         AND
                                         SM.LangId=1055
                                         AND
                                         SM.ObjectTypeCode=2) CType
                                       ,P.new_projectcode
                                       ,Q.new_projectidName
                                       ,Q.new_productid
                                       ,Q.new_productidName
                                       FROM
                                       Contact C WITH(NOLOCK)
                                       LEFT JOIN Quote Q WITH(NOLOCK)
                                       ON C.ContactId=Q.CustomerId
									   LEFT JOIN new_project P WITH(NOLOCK)
									   ON Q.new_projectid=P.new_projectId
                                       ORDER BY C.new_Number
                                       OFFSET ({0})*{1} ROWS
                                       FETCH NEXT {1} ROWS ONLY";

            string accountListQuery = @"SELECT
                                         A.AccountId
                                        ,A.Name
                                        ,A.EMailAddress1
                                        ,A.Telephone1
                                        ,P.new_projectcode
                                        ,Q.new_projectidName
                                        ,Q.new_productid
                                        ,Q.new_productidName
                                        FROM
                                        Account A WITH(NOLOCK)
                                        LEFT JOIN Quote Q WITH(NOLOCK)
                                        ON A.AccountId=Q.CustomerId
									    LEFT JOIN new_project P WITH(NOLOCK)
									    ON Q.new_projectid=P.new_projectId
                                        ORDER BY A.Name
                                        OFFSET ({0})*{1} ROWS
                                        FETCH NEXT {1} ROWS ONLY";

            try
            {
                DataTable customerInfoTable = new DataTable();
                if (customerType == CustomerType.Contact)
                {
                    customerInfoTable = sda.getDataTable(string.Format(contactListQuery, (page - 1), rowCount));
                    sda.closeConnection();
                    if (customerInfoTable.Rows.Count > 0)
                    {
                        foreach (DataRow contactRow in customerInfoTable.Rows)
                        {
                            CustomerProject customerProject = new CustomerProject();
                            string Id = contactRow["ContactId"] != DBNull.Value ? Convert.ToString(contactRow["ContactId"]) : string.Empty;
                            string number = contactRow["new_Number"] != DBNull.Value ? Convert.ToString(contactRow["new_Number"]) : string.Empty;
                            string firstName = contactRow["FirstName"] != DBNull.Value ? Convert.ToString(contactRow["FirstName"]) : string.Empty;
                            string lastName = contactRow["LastName"] != DBNull.Value ? Convert.ToString(contactRow["LastName"]) : string.Empty;
                            string emailaddress = contactRow["EMailAddress1"] != DBNull.Value ? Convert.ToString(contactRow["EMailAddress1"]) : string.Empty;
                            string mobilePhone = contactRow["MobilePhone"] != DBNull.Value ? Convert.ToString(contactRow["MobilePhone"]) : string.Empty;
                            string customerStatus = contactRow["CType"] != DBNull.Value ? Convert.ToString(contactRow["CType"]) : string.Empty;



                            CustomerInfo customerInfo = new CustomerInfo();

                            customerInfo.CustomerId = Id;
                            customerInfo.Number = number;
                            customerInfo.Name = firstName;
                            customerInfo.Surname = lastName;
                            customerInfo.Phone = mobilePhone;
                            customerInfo.Email = emailaddress;
                            customerInfo.Type = customerStatus;
                            customerInfo.CustomerType = CustomerType.Contact;

                            string projectId = contactRow["new_projectcode"] != DBNull.Value ? Convert.ToString(contactRow["new_projectcode"]) : string.Empty;
                            string projectName = contactRow["new_projectidName"] != DBNull.Value ? Convert.ToString(contactRow["new_projectidName"]) : string.Empty;
                            string productCode = contactRow["new_productid"] != DBNull.Value ? Convert.ToString(contactRow["new_productid"]) : string.Empty;
                            string productName = contactRow["new_productidName"] != DBNull.Value ? Convert.ToString(contactRow["new_productidName"]) : string.Empty;

                            Quote quote = new Quote();
                            quote.ProjectCode = projectId;
                            quote.ProjectName = projectName;
                            quote.ProductId = productCode;
                            quote.ProductName = productName;

                            List<Quote> quoteList = new List<Quote>();

                            quoteList.Add(quote);
                            customerProject.Customer = customerInfo;
                            customerProject.QuoteList = quoteList;
                            customerProjectList.Add(customerProject);
                        }
                    }
                }
                else
                {
                    customerInfoTable = sda.getDataTable(string.Format(accountListQuery, (page - 1), rowCount));
                    sda.closeConnection();
                    if (customerInfoTable.Rows.Count > 0)
                    {
                        foreach (DataRow accountRow in customerInfoTable.Rows)
                        {
                            CustomerProject customerProject = new CustomerProject();
                            string Id = accountRow["AccountId"] != DBNull.Value ? Convert.ToString(accountRow["AccountId"]) : string.Empty;
                            string firstName = accountRow["Name"] != DBNull.Value ? Convert.ToString(accountRow["Name"]) : string.Empty;
                            string emailaddress = accountRow["EMailAddress1"] != DBNull.Value ? Convert.ToString(accountRow["EMailAddress1"]) : string.Empty;
                            string phone = accountRow["Telephone1"] != DBNull.Value ? Convert.ToString(accountRow["Telephone1"]) : string.Empty;
                            string customerStatus = "Gerçek Müşteri"; //Firma Üzerinde Müşteri Tipi Yok. Bu Nedenle Statik Basılması İstendi.


                            CustomerInfo customerInfo = new CustomerInfo();

                            customerInfo.CustomerId = Id;
                            customerInfo.Name = firstName;
                            customerInfo.Phone = phone;
                            customerInfo.Email = emailaddress;
                            customerInfo.Type = customerStatus;

                            string projectCode = accountRow["new_projectcode"] != DBNull.Value ? Convert.ToString(accountRow["new_projectcode"]) : string.Empty;
                            string projectName = accountRow["new_projectidName"] != DBNull.Value ? Convert.ToString(accountRow["new_projectidName"]) : string.Empty;
                            string productId = accountRow["new_productid"] != DBNull.Value ? Convert.ToString(accountRow["new_productid"]) : string.Empty;
                            string productName = accountRow["new_productidName"] != DBNull.Value ? Convert.ToString(accountRow["new_productidName"]) : string.Empty;

                            Quote quote = new Quote();
                            quote.ProjectCode = projectCode;
                            quote.ProjectName = projectName;
                            quote.ProductId = productId;
                            quote.ProductName = productName;


                            List<Quote> quoteList = new List<Quote>();

                            quoteList.Add(quote);

                            customerProject.Customer = customerInfo;
                            customerProject.QuoteList = quoteList;
                            customerProjectList.Add(customerProject);

                        }
                    }
                }
                customerDetailListResult.CustomerProjectList = customerProjectList;
                result.Success = true;
                result.Message = SuccessMessage;

            }
            catch (Exception ex)
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("Login", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            customerDetailListResult.Result = result;
            returnValue = ser.Serialize(customerDetailListResult);
            return returnValue;
        }

        public string CreateFoldhome(Foldhome foldhome)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            IOrganizationService service = MSCRM.GetOrgService(true);
            if (string.IsNullOrWhiteSpace(foldhome.CustomerId))
            {
                result.Success = false;
                result.Message = "Müşteri Id'si Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }
            if (string.IsNullOrWhiteSpace(foldhome.ProjectCode))
            {
                result.Success = false;
                result.Message = "Proje Kodu Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }

            try
            {
                Guid projectId = GetRecord("new_projectcode", foldhome.ProjectCode, "new_project", service);
                if (projectId == Guid.Empty)
                {
                    result.Success = false;
                    result.Message = "Proje Kodu Yanlış";
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }

                Entity foldhomeActivity = new Entity();
                foldhomeActivity.LogicalName = "new_foldhomeactivity";
                foldhomeActivity.Attributes["new_amount"] = new Money(foldhome.Amount.Value);
                foldhomeActivity.Attributes["scheduledstart"] = foldhome.StartDate.Value;
                foldhomeActivity.Attributes["scheduledend"] = foldhome.EndDate.Value;

                foldhomeActivity.Attributes["actualstart"] = foldhome.StartDate.Value;
                foldhomeActivity.Attributes["actualend"] = foldhome.EndDate.Value;

                foldhomeActivity.Attributes["new_projectid"] = new EntityReference("new_project", projectId);
                if (CustomerType.Contact == foldhome.CustomerType)
                {
                    foldhomeActivity.Attributes["regardingobjectid"] = new EntityReference("contact", new Guid(foldhome.CustomerId));
                }
                else
                {
                    foldhomeActivity.Attributes["regardingobjectid"] = new EntityReference("account", new Guid(foldhome.CustomerId));
                }
                foldhomeActivity.Attributes["new_roomtype"] = new OptionSetValue((int)foldhome.RoomType);
                Guid foldhomeActivityId = service.Create(foldhomeActivity);
                result.Success = true;
                result.Message = SuccessMessage;
                result.CRMId = Convert.ToString(foldhomeActivityId);
            }
            catch (Exception ex)
            {
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("CreateFoldhome", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            returnValue = ser.Serialize(result);
            return returnValue;
        }

        public string UpdateFoldhome(Foldhome foldhome)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            IOrganizationService service = MSCRM.GetOrgService(true);
            if (string.IsNullOrWhiteSpace(foldhome.FoldhomeId))
            {
                result.Success = false;
                result.Message = "Foldhome Id'si Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }


            try
            {
                Entity foldhomeActivity = new Entity();
                foldhomeActivity.LogicalName = "new_foldhomeactivity";
                foldhomeActivity.Id = new Guid(foldhome.FoldhomeId);
                if (foldhome.Amount.HasValue)
                {
                    foldhomeActivity.Attributes["new_amount"] = new Money(foldhome.Amount.Value);
                }
                if (foldhome.StartDate.HasValue)
                {
                    foldhomeActivity.Attributes["actualstart"] = foldhome.StartDate.Value;
                }
                if (foldhome.EndDate.HasValue)
                {
                    foldhomeActivity.Attributes["actualend"] = foldhome.EndDate.Value;
                }
                if (foldhome.StatusCode.HasValue && (int)foldhome.StatusCode.Value != 0)
                {
                    foldhomeActivity.Attributes["new_status"] = new OptionSetValue((int)foldhome.StatusCode.Value);
                }
                if (foldhome.PaymentStatusCode.HasValue && (int)foldhome.PaymentStatusCode.Value != 0)
                {
                    foldhomeActivity.Attributes["new_paymentstatus"] = new OptionSetValue((int)foldhome.PaymentStatusCode.Value);
                }
                if (foldhome.SurveyResult.HasValue)
                {
                    foldhomeActivity.Attributes["new_surveyresult"] = foldhome.SurveyResult.Value;
                }
                service.Update(foldhomeActivity);

                result.Success = true;
                result.Message = SuccessMessage;
            }
            catch (Exception ex)
            {
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("UpdateFoldhome", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            returnValue = ser.Serialize(result);
            return returnValue;
        }

        public string UpdateStatusFoldhome(Foldhome foldhome)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            IOrganizationService service = MSCRM.GetOrgService(true);
            if (string.IsNullOrWhiteSpace(foldhome.FoldhomeId))
            {
                result.Success = false;
                result.Message = "Foldhome Id'si Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }
            if (!foldhome.StatusCode.HasValue)
            {
                result.Success = false;
                result.Message = "StatusCode Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }

            try
            {
                if (foldhome.StatusCode == FoldhomeStatus.Iptal)
                {
                    SetStateRequest setStateRequset = new SetStateRequest()
                       {
                           EntityMoniker = new EntityReference("new_foldhomeactivity", new Guid(foldhome.FoldhomeId)),
                           State = new OptionSetValue((int)FoldhomeState.Iptal),
                           Status = new OptionSetValue((int)FoldhomeStatus.Iptal)
                       };
                    service.Execute(setStateRequset);
                }
                else if (foldhome.StatusCode == FoldhomeStatus.Tamamlandi)
                {
                    SetStateRequest setStateRequset = new SetStateRequest()
                    {
                        EntityMoniker = new EntityReference("new_foldhomeactivity", new Guid(foldhome.FoldhomeId)),
                        State = new OptionSetValue((int)FoldhomeState.Tamamlandi),
                        Status = new OptionSetValue((int)FoldhomeStatus.Tamamlandi)
                    };
                    service.Execute(setStateRequset);
                }
                else
                {
                    result.Success = false;
                    result.Message = "İptal veya Tamamlandı Dışında Bir Statüye Çekilemez.";
                }

                result.Success = true;
                result.Message = SuccessMessage;
            }
            catch (Exception ex)
            {
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("UpdateStatusFoldhome", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            returnValue = ser.Serialize(result);
            return returnValue;

        }

        public string GetFoldhome(string customerId)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            List<Foldhome> foldhomeList = new List<Foldhome>();
            FoldhomeResult foldhomeResult = new FoldhomeResult();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                result.Success = false;
                result.Message = "Müşteri Id'si Boş Olamaz";
                foldhomeResult.Result = result;

                returnValue = ser.Serialize(foldhomeResult);
                return returnValue;
            }

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            string foldhomeQuery = @"SELECT
                                    FA.ActivityId
                                   ,FA.ActualStart
                                   ,FA.ActualEnd
                                   ,FA.new_amount
                                   ,FA.new_roomtype
                                   ,FA.new_surveyresult
                                   ,FA.new_status
                                   ,FA.new_paymentstatus
                                   ,PT.new_projectcode
                                   FROM
                                   new_foldhomeactivity FA WITH(NOLOCK)
                                   LEFT JOIN new_project PT WITH(NOLOCK)
								   ON FA.new_projectid=PT.new_projectId
                                   WHERE
                                   FA.RegardingObjectId='{0}'";

            DataTable foldhomeTable = new DataTable();
            foldhomeTable = sda.getDataTable(string.Format(foldhomeQuery, customerId));
            try
            {
                foreach (DataRow foldhome in foldhomeTable.Rows)
                {
                    string foldhomeId = foldhome["ActivityId"] != DBNull.Value ? Convert.ToString(foldhome["ActivityId"]) : string.Empty;
                    DateTime startDate = foldhome["ActualStart"] != DBNull.Value ? Convert.ToDateTime(foldhome["ActualStart"]) : DateTime.MinValue;
                    DateTime endDate = foldhome["ActualEnd"] != DBNull.Value ? Convert.ToDateTime(foldhome["ActualEnd"]) : DateTime.MinValue;
                    decimal amount = foldhome["new_amount"] != DBNull.Value ? Convert.ToDecimal(foldhome["new_amount"]) : 0;
                    int roomType = foldhome["new_roomtype"] != DBNull.Value ? Convert.ToInt32(foldhome["new_roomtype"]) : 0;
                    int statusCode = foldhome["new_status"] != DBNull.Value ? Convert.ToInt32(foldhome["new_status"]) : 0;
                    int paymentStatusCode = foldhome["new_paymentstatus"] != DBNull.Value ? Convert.ToInt32(foldhome["new_paymentstatus"]) : 0;
                    int surveyResult = foldhome["new_surveyresult"] != DBNull.Value ? Convert.ToInt32(foldhome["new_surveyresult"]) : 0;
                    string projectCode = foldhome["new_projectcode"] != DBNull.Value ? Convert.ToString(foldhome["new_projectcode"]) : string.Empty;

                    Foldhome tempFoldhome = new Foldhome();
                    tempFoldhome.FoldhomeId = foldhomeId;
                    tempFoldhome.Amount = amount;
                    tempFoldhome.StartDate = startDate != DateTime.MinValue ? UTCConvertToLocalTime(startDate) : startDate;
                    tempFoldhome.EndDate = endDate != DateTime.MinValue ? UTCConvertToLocalTime(endDate) : endDate;
                    tempFoldhome.ProjectCode = projectCode;
                    if (roomType != 0)
                    {
                        tempFoldhome.RoomType = (RoomType)roomType;
                    }
                    if (paymentStatusCode != 0)
                    {
                        tempFoldhome.PaymentStatusCode = (PaymentStatus)paymentStatusCode;
                    }
                    if (statusCode != 0)
                    {
                        tempFoldhome.StatusCode = (FoldhomeStatus)statusCode;
                    }
                    tempFoldhome.SurveyResult = surveyResult;
                    foldhomeList.Add(tempFoldhome);
                }

                result.Success = true;
                result.Message = SuccessMessage;
                foldhomeResult.FoldhomeList = foldhomeList;
            }
            catch (Exception ex)
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("GetFoldhome", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            foldhomeResult.Result = result;
            returnValue = ser.Serialize(foldhomeResult);
            return returnValue;

        }

        public string CreateWebForm(WebCustomerInfo webCustomerInfo)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            IOrganizationService service = MSCRM.GetOrgService(true);
            Guid customerId = Guid.Empty;

            CustomerType customerType = new CustomerType();
            CustomerInfo customerInfo = new CustomerInfo();

            if (!string.IsNullOrEmpty(webCustomerInfo.Customer.Email))
            {
                customerId = GetRecord("emailaddress1", webCustomerInfo.Customer.Email, "contact", service);
                if (customerId == Guid.Empty)
                {
                    customerId = GetRecord("emailaddress1", webCustomerInfo.Customer.Email, "account", service);
                    customerType = CustomerType.Account;
                }
                else
                {
                    customerType = CustomerType.Contact;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "E-Posta Adresi Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }

            if (customerId == Guid.Empty)
            {
                Entity customer = new Entity();
                if (string.IsNullOrEmpty(webCustomerInfo.Customer.Name))
                {
                    result.Success = false;
                    result.Message = "Lütfen adınızı yazınız";
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }
                customer.Attributes["firstname"] = webCustomerInfo.Customer.Name;
                if (!string.IsNullOrWhiteSpace(webCustomerInfo.Customer.Surname))
                {
                    customer.Attributes["lastname"] = webCustomerInfo.Customer.Surname;
                }
                if (!string.IsNullOrWhiteSpace(webCustomerInfo.Customer.Phone))
                {
                    customer.Attributes["mobilephone"] = webCustomerInfo.Customer.Phone;
                }
                customer.Attributes["emailaddress1"] = webCustomerInfo.Customer.Email;
                customer.Attributes["new_customertype"] = new OptionSetValue(100000002);//Potansiyel Müşteri
                customer.Attributes["new_sourceofparticipationid"] = new EntityReference("new_sourceofparticipation", new Guid(Globals.MobilFormParticipationId));//Katılım kaynağı

                try
                {
                    customer.Attributes["ownerid"] = new EntityReference("systemuser", new Guid(Globals.WebFormSytemUserId));//Web Form Sahibi
                    customerId = service.Create(customer);
                }
                catch (Exception ex)
                {
                    eventLog = new EventLogHelper(service, "MobilWebServis");
                    eventLog.Log("CreateWebForm-CreateCustomer", ex.Message, EventLogHelper.EventType.Exception);
                    result.Success = false;
                    result.Message = ex.Message;
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }
            }

            #region | CREATE WEB FORM |
            if (customerId != Guid.Empty)
            {
                Guid projectId = GetRecord("new_projectcode", webCustomerInfo.Quote.ProjectCode, "new_project", service);
                if (projectId == Guid.Empty)
                {
                    result.Success = false;
                    result.Message = "Proje Kodu Yanlış";
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }

                Entity webForm = new Entity("new_webform");
                if (string.IsNullOrWhiteSpace(webCustomerInfo.Customer.Surname))
                {
                    webForm.Attributes["new_name"] = webCustomerInfo.Customer.Name + " - " + DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    webForm.Attributes["new_name"] = webCustomerInfo.Customer.Name + " " + webCustomerInfo.Customer.Surname + " - " + DateTime.Now.ToString("dd/MM/yyyy");
                    webForm.Attributes["new_lastname"] = webCustomerInfo.Customer.Surname;
                }
                webForm.Attributes["new_firstname"] = webCustomerInfo.Customer.Name;
                webForm.Attributes["new_mobilephone"] = webCustomerInfo.Customer.Phone;
                webForm.Attributes["new_emailadress"] = webCustomerInfo.Customer.Email;
                webForm.Attributes["new_sourceofparticipationid"] = new EntityReference("new_sourceofparticipation", new Guid(Globals.WebFormParticipationId));
                webForm.Attributes["new_projectid"] = new EntityReference("new_project", projectId);


                if (webCustomerInfo.CustomerInvestmentRange.HasValue)
                {
                    if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionOne)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionOne);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionTwo)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionTwo);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionThree)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionThree);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionFour)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFour);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionFive)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionFive);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionSix)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionSix);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionSeven)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionSeven);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionEight)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionEight);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionNine)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionNine);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionTen)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionTen);
                    }
                    else if (webCustomerInfo.CustomerInvestmentRange.Value == InvestmentRange.optionEleven)
                    {
                        webForm.Attributes["new_investmentrange"] = new OptionSetValue((int)InvestmentRange.optionEleven);
                    }
                }
                if (customerType == CustomerType.Contact)
                {
                    webForm.Attributes["new_contactid"] = new EntityReference("contact", customerId);
                }
                //else if (customerType == CustomerType.Account)
                //{
                //    webForm.Attributes["new_contactid"] = new EntityReference("account", customerId);
                //}

                try
                {
                    webForm.Attributes["ownerid"] = new EntityReference("systemuser", new Guid(Globals.WebFormSytemUserId));//Web Form Sahibi
                    service.Create(webForm);

                    result.Success = true;
                    result.Message = SuccessMessage;
                    result.CRMId = Convert.ToString(customerId);

                }
                catch (Exception ex)
                {
                    eventLog = new EventLogHelper(service, "MobilWebServis");
                    eventLog.Log("CreateWebForm", ex.Message, EventLogHelper.EventType.Exception);
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }

            #endregion
            returnValue = ser.Serialize(result);
            return returnValue;
        }

        public string CreateLead(LeadInfo leadInfo)
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MsCrmResult result = new MsCrmResult();
            IOrganizationService service = MSCRM.GetOrgService(true);
            Guid customerId = Guid.Empty;

            if (!string.IsNullOrEmpty(leadInfo.Customer.Email))
            {
                customerId = GetRecord("emailaddress1", leadInfo.Customer.Email, "contact", service);
                if (customerId == Guid.Empty)
                {
                    customerId = GetRecord("emailaddress1", leadInfo.Customer.Email, "account", service);
                }
                if (customerId != Guid.Empty)
                {
                    result.Success = false;
                    result.Message = "Kayıtlı E-Posta Adresi";
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "E-Posta Adresi Boş Olamaz";
                returnValue = ser.Serialize(result);
                return returnValue;
            }

            if (customerId == Guid.Empty)
            {
                Entity customer = new Entity();
                customer.LogicalName = "contact";
                if (string.IsNullOrEmpty(leadInfo.Customer.Name))
                {
                    result.Success = false;
                    result.Message = "Lütfen adınızı yazınız";
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }
                if (string.IsNullOrEmpty(leadInfo.Customer.Phone))
                {
                    result.Success = false;
                    result.Message = "Telefon Numarası Bış Bırakılamaz";
                    returnValue = ser.Serialize(result);
                    return returnValue;
                }
                customer.Attributes["firstname"] = leadInfo.Customer.Name;
                if (!string.IsNullOrWhiteSpace(leadInfo.Customer.Surname))
                {
                    customer.Attributes["lastname"] = leadInfo.Customer.Surname;
                }

                customer.Attributes["mobilephone"] = leadInfo.Customer.Phone;
                customer.Attributes["emailaddress1"] = leadInfo.Customer.Email;
                customer.Attributes["new_customertype"] = new OptionSetValue(100000002);//Potansiyel Müşteri
                customer.Attributes["new_sourceofparticipationid"] = new EntityReference("new_sourceofparticipation", new Guid(Globals.MobilFormParticipationId));//Katılım kaynağı

                try
                {
                    customer.Attributes["ownerid"] = new EntityReference("systemuser", new Guid(Globals.WebFormSytemUserId));//Web Form Sahibi
                    customerId = service.Create(customer);
                    result.Success = true;
                    result.Message = SuccessMessage;
                    result.CRMId = Convert.ToString(customerId);
                }
                catch (Exception ex)
                {
                    eventLog = new EventLogHelper(service, "MobilWebServis");
                    eventLog.Log("CreateLead", ex.Message, EventLogHelper.EventType.Exception);
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }

            returnValue = ser.Serialize(result);
            return returnValue;
        }

        public string GetMobilAppCustomer()
        {
            string returnValue = string.Empty;
            JavaScriptSerializer ser = new JavaScriptSerializer();

            CustomerProjectListResult customerDetailListResult = new CustomerProjectListResult();
            List<CustomerProject> customerProjectList = new List<CustomerProject>();


            MsCrmResult result = new MsCrmResult();

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            string contactListQuery = @"SELECT
                                        C.ContactId
                                       ,C.new_Number
                                       ,C.FirstName
                                       ,C.LastName
                                       ,C.EMailAddress1
                                       ,C.MobilePhone
                                       ,(SELECT
                                          SM.Value 
                                         FROM StringMap SM WITH(NOLOCK) 
                                         WHERE
                                         SM.AttributeName='new_customertype'
                                         AND
                                         SM.AttributeValue=C.new_customertype
                                         AND
                                         SM.LangId=1055
                                         AND
                                         SM.ObjectTypeCode=2) CType
                                       ,P.new_projectcode
                                       ,Q.new_projectidName
                                       ,Q.new_productid
                                       ,Q.new_productidName
                                       FROM
                                       Contact C WITH(NOLOCK)
                                       LEFT JOIN Quote Q WITH(NOLOCK)
                                       ON C.ContactId=Q.CustomerId
									   LEFT JOIN new_project P WITH(NOLOCK)
									   ON Q.new_projectid=P.new_projectId
                                       WHERE
                                       C.new_sourceofparticipationid='8786D52F-4453-E611-80FF-005056A60603'
                                       ORDER BY C.new_Number";

            
            try
            {
                DataTable customerInfoTable = new DataTable();
               
                customerInfoTable = sda.getDataTable(contactListQuery);
                    sda.closeConnection();
                    if (customerInfoTable.Rows.Count > 0)
                    {
                        foreach (DataRow contactRow in customerInfoTable.Rows)
                        {
                            CustomerProject customerProject = new CustomerProject();
                            string Id = contactRow["ContactId"] != DBNull.Value ? Convert.ToString(contactRow["ContactId"]) : string.Empty;
                            string number = contactRow["new_Number"] != DBNull.Value ? Convert.ToString(contactRow["new_Number"]) : string.Empty;
                            string firstName = contactRow["FirstName"] != DBNull.Value ? Convert.ToString(contactRow["FirstName"]) : string.Empty;
                            string lastName = contactRow["LastName"] != DBNull.Value ? Convert.ToString(contactRow["LastName"]) : string.Empty;
                            string emailaddress = contactRow["EMailAddress1"] != DBNull.Value ? Convert.ToString(contactRow["EMailAddress1"]) : string.Empty;
                            string mobilePhone = contactRow["MobilePhone"] != DBNull.Value ? Convert.ToString(contactRow["MobilePhone"]) : string.Empty;
                            string customerStatus = contactRow["CType"] != DBNull.Value ? Convert.ToString(contactRow["CType"]) : string.Empty;



                            CustomerInfo customerInfo = new CustomerInfo();

                            customerInfo.CustomerId = Id;
                            customerInfo.Number = number;
                            customerInfo.Name = firstName;
                            customerInfo.Surname = lastName;
                            customerInfo.Phone = mobilePhone;
                            customerInfo.Email = emailaddress;
                            customerInfo.Type = customerStatus;
                            customerInfo.CustomerType = CustomerType.Contact;

                            string projectId = contactRow["new_projectcode"] != DBNull.Value ? Convert.ToString(contactRow["new_projectcode"]) : string.Empty;
                            string projectName = contactRow["new_projectidName"] != DBNull.Value ? Convert.ToString(contactRow["new_projectidName"]) : string.Empty;
                            string productCode = contactRow["new_productid"] != DBNull.Value ? Convert.ToString(contactRow["new_productid"]) : string.Empty;
                            string productName = contactRow["new_productidName"] != DBNull.Value ? Convert.ToString(contactRow["new_productidName"]) : string.Empty;

                            Quote quote = new Quote();
                            quote.ProjectCode = projectId;
                            quote.ProjectName = projectName;
                            quote.ProductId = productCode;
                            quote.ProductName = productName;

                            List<Quote> quoteList = new List<Quote>();

                            quoteList.Add(quote);
                            customerProject.Customer = customerInfo;
                            customerProject.QuoteList = quoteList;
                            customerProjectList.Add(customerProject);
                        }
                    }
                
                customerDetailListResult.CustomerProjectList = customerProjectList;
                result.Success = true;
                result.Message = SuccessMessage;

            }
            catch (Exception ex)
            {
                IOrganizationService service = MSCRM.GetOrgService(true);
                eventLog = new EventLogHelper(service, "MobilWebServis");
                eventLog.Log("Login", ex.Message, EventLogHelper.EventType.Exception);
                result.Success = false;
                result.Message = ex.Message;
            }

            customerDetailListResult.Result = result;
            returnValue = ser.Serialize(customerDetailListResult);
            return returnValue;
        }

        private Guid GetRecord(string attributeName, string attributeValue, string entityName, IOrganizationService service)
        {
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = attributeName;
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(attributeValue);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);

            QueryExpression Query = new QueryExpression(entityName);
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

        private CustomerInfo GetCustomer(string customerId, CustomerType customerType)
        {
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            CustomerInfo customerInfo = new CustomerInfo();

            string dataContactQuery = @"SELECT
                                        C.ContactId
                                       ,C.FirstName
                                       ,C.LastName
                                       ,C.EMailAddress1
                                       ,C.MobilePhone
                                       FROM
                                       Contact C WITH(NOLOCK)
                                       WHERE
                                       C.ContactId='{0}'";

            string dataAccountQuery = @"SELECT
                                         A.AccountId
                                        ,A.Name
                                        ,A.EMailAddress1
                                        ,A.Telephone1
                                        ,A.new_taxnumber
                                        FROM
                                        Account A WITH(NOLOCK)
                                        WHERE
                                        A.AccountId='{0}'";
            DataTable customerInfoTable = new DataTable();
            if (customerType == CustomerType.Contact)
            {
                customerInfoTable = sda.getDataTable(string.Format(dataContactQuery, customerId));
                sda.closeConnection();
                if (customerInfoTable.Rows.Count > 0)
                {
                    DataRow masterCustomerInfo = customerInfoTable.Rows[0];
                    string Id = masterCustomerInfo["ContactId"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["ContactId"]) : string.Empty;
                    string number = masterCustomerInfo["new_Number"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["new_Number"]) : string.Empty;
                    string firstName = masterCustomerInfo["FirstName"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["FirstName"]) : string.Empty;
                    string lastName = masterCustomerInfo["LastName"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["LastName"]) : string.Empty;
                    string emailaddress = masterCustomerInfo["EMailAddress1"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["EMailAddress1"]) : string.Empty;
                    string mobilePhone = masterCustomerInfo["MobilePhone"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["MobilePhone"]) : string.Empty;
                    string identityNumber = masterCustomerInfo["new_tcidentitynumber"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["new_tcidentitynumber"]) : string.Empty;

                    customerInfo.CustomerId = Id;
                    customerInfo.Name = firstName;
                    customerInfo.Surname = lastName;
                    customerInfo.Phone = mobilePhone;
                    customerInfo.Email = emailaddress;
                }
            }
            else
            {
                customerInfoTable = sda.getDataTable(string.Format(dataAccountQuery, customerId));
                sda.closeConnection();
                if (customerInfoTable.Rows.Count > 0)
                {
                    DataRow masterCustomerInfo = customerInfoTable.Rows[0];
                    string Id = masterCustomerInfo["AccountId"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["AccountId"]) : string.Empty;
                    string firstName = masterCustomerInfo["Name"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["Name"]) : string.Empty;
                    string emailaddress = masterCustomerInfo["EMailAddress1"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["EMailAddress1"]) : string.Empty;
                    string phone = masterCustomerInfo["Telephone1"] != DBNull.Value ? Convert.ToString(masterCustomerInfo["Telephone1"]) : string.Empty;


                    customerInfo.CustomerId = Id;
                    customerInfo.Name = firstName;
                    customerInfo.Phone = phone;
                    customerInfo.Email = emailaddress;

                }
            }
            return customerInfo;
        }

        private DateTime UTCConvertToLocalTime(DateTime UTCDate)
        {
            DateTime tempDate = new DateTime(UTCDate.Ticks, DateTimeKind.Utc);
            DateTime localDate = tempDate.ToLocalTime();
            return localDate;
        }
    }
}
