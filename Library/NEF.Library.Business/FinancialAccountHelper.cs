using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public class FinancialAccountHelper
    {
        public static MsCrmResult CreateOrUpdateFinancialAccount(FinancialAccount financial, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("new_financialaccount");

                if (!string.IsNullOrEmpty(financial.Name))
                    ent["new_name"] = financial.Name;

                if (financial.Contact != null && financial.Contact.Id != Guid.Empty)
                    ent["new_contactid"] = new EntityReference("contact", financial.Contact.Id);
                else
                    ent["new_contactid"] = null;

                if (financial.Account != null && financial.Account.Id != Guid.Empty)
                    ent["new_accountid"] = new EntityReference("account", financial.Account.Id);
                else
                    ent["new_accountid"] = null;

                if (financial.FinancialAccountId == Guid.Empty)
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Kayıt başarıyla eklendi";
                }
                else
                {
                    ent["new_financialaccountid"] = financial.FinancialAccountId;
                    service.Update(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Bilgiler başarıyla güncellendi";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetFinancialAccountNumberByCharacter(char character, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    C.new_financialaccountnumber Number
                                    FROM
	                                    new_financialaccount C WITH (NOLOCK)
                                    WHERE
	                                    C.new_financialaccountcharacter = '{0}'
                                    ORDER BY
	                                    C.new_financialaccountnumber DESC";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, character));

                returnValue.Success = true;
                returnValue.ReturnObject = dt.Rows.Count > 0 ? dt.Rows[0]["Number"] != DBNull.Value ? (int)dt.Rows[0]["Number"] : 0 : 0;
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
