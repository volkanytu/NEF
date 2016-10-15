using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NEF.Library.Business;
using NEF.Library.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data.SqlClient;

namespace NEF.Web.LogoTransfer
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //txtSalesNo.Value = slctType.Items[slctType.SelectedIndex].Text;
                lblError.Visible = false;
            }
        }

        protected void btnSave_ServerClick(object sender, EventArgs e)
        {
            lblError.Visible = false;

            int status = Convert.ToInt32(slctType.Items[slctType.SelectedIndex].Value);

            if (status == 0)
            {
                lblError.Visible = true;
                lblError.InnerHtml = "İşlem Tipi seçiniz...";
                return;
            }

            QuoteStatus quoteStatus = (QuoteStatus)status;
            string quoteNo = txtSalesNo.Value;

            Dictionary<Guid, QuoteStatus> quoteInfo = GetQuoteId(quoteNo);

            if (quoteInfo == null)
            {
                lblError.Visible = true;
                lblError.InnerHtml = "Satış bulunamadı...";
                return;
            }
            else
            {
                if (quoteStatus == QuoteStatus.MuhasebeyeAktarıldı)
                {
                    MsCrmResult result = QuoteHelper.Muhasebelestir(quoteInfo.Keys.FirstOrDefault().ToString());

                    lblError.Visible = true;
                    lblError.InnerHtml = result.Result;
                    return;
                }
                else //if(quoteStatus==QuoteStatus.İptalEdildi)
                {
                    //if(quoteInfo.Values.FirstOrDefault()==QuoteStatus.İptalEdildi || quoteInfo.Values.FirstOrDefault()==QuoteStatus.İptalAktarıldı)
                    //{
                    MsCrmResult resultIptal = IptalEt(quoteInfo.Keys.FirstOrDefault().ToString());

                    lblError.Visible = true;
                    lblError.InnerHtml = resultIptal.Result;
                    return;
                    //}
                }
            }

        }

        Dictionary<Guid, QuoteStatus> GetQuoteId(string quoteNumber)
        {
            Dictionary<Guid, QuoteStatus> returnValue = new Dictionary<Guid, QuoteStatus>();

            IOrganizationService service = MSCRM.GetOrgService(true);
            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quotenumber";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(quoteNumber);

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

                returnValue.Add(q.Id, (QuoteStatus)(int)((OptionSetValue)q["statuscode"]).Value);
            }

            return returnValue;
        }

        public MsCrmResult IptalEt(string quoteId)
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

                sda.ExecuteNonQuery(string.Format(sqlQuery, quoteId), new SqlParameter[] { new SqlParameter("StatusCode", (int)QuoteStatus.İptalEdildi) });

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

    }
}