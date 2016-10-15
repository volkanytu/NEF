using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class WebFormHelper
    {
        public static MsCrmResultObject GetContactWebForms(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                W.new_webformId Id
								    ,W.new_contactid ContactId
                                    ,W.new_contactidName ContactIdName
                                FROM
	                                new_webform AS W WITH (NOLOCK)
                                WHERE
	                                W.new_contactid = '{0}'
                                ORDER BY
                                    W.CreatedOn";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, contactId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<WebForm> returnList = new List<WebForm>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        WebForm form = new WebForm();
                        form.WebFormId = (Guid)dt.Rows[i]["Id"];
                        form.Contact = new EntityReference() { LogicalName = "contact", Id = (Guid)dt.Rows[i]["ContactId"], Name = dt.Rows[i]["ContactIdName"].ToString() };
                        returnList.Add(form);
                    }

                    returnValue.ReturnObject = returnList;
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

        public static MsCrmResult WebFormClose(Guid webFormId, int state, int status, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                SetStateRequest setState = new SetStateRequest
                {
                    EntityMoniker = new EntityReference("new_webform", webFormId),
                    State = new OptionSetValue(state),
                    Status = new OptionSetValue(status)
                };
                SetStateResponse myres = (SetStateResponse)service.Execute(setState);

                returnValue.Success = true;
                returnValue.Result = "Web form başarıyla kapatıldı.";
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
