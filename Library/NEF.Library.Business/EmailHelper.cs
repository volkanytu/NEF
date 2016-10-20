using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class EmailHelper
    {
        public static MsCrmResult SendMail(Guid ObjectId, string ObjectType, Entity[] fromPartyArray, Entity[] toPartyArray
            , string subject, string mailBody, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region Create Email

                Entity email = new Entity("email");
                email["to"] = toPartyArray;
                email["from"] = fromPartyArray;
                email["subject"] = subject;
                email["description"] = mailBody;
                email["directioncode"] = true;

                if (ObjectId != Guid.Empty && !string.IsNullOrEmpty(ObjectType))
                {
                    EntityReference regardingObject = new EntityReference(ObjectType, ObjectId);
                    email.Attributes.Add("regardingobjectid", regardingObject);
                }

                returnValue.CrmId = service.Create(email);
                #endregion

                #region Send Email
                if (Globals.IsSendMailActive == "1")
                {
                    var req = new SendEmailRequest
                    {
                        EmailId = returnValue.CrmId,
                        TrackingToken = string.Empty,
                        IssueSend = true
                    };

                    var res = (SendEmailResponse)service.Execute(req);
                }
                #endregion

                returnValue.Success = true;
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult SendMail(Guid ObjectId, string ObjectType, Entity fromParty, Entity toParty
            , string subject, string mailBody, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region Create Email

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["subject"] = subject;
                email["description"] = mailBody;
                email["directioncode"] = true;

                if (ObjectId != Guid.Empty && !string.IsNullOrEmpty(ObjectType))
                {
                    EntityReference regardingObject = new EntityReference(ObjectType, ObjectId);
                    email.Attributes.Add("regardingobjectid", regardingObject);
                }

                returnValue.CrmId = service.Create(email);
                #endregion

                #region Send Email

                if (Globals.IsSendMailActive == "1")
                {
                    var req = new SendEmailRequest
                    {
                        EmailId = returnValue.CrmId,
                        TrackingToken = string.Empty,
                        IssueSend = true
                    };

                    var res = (SendEmailResponse)service.Execute(req);
                }
                #endregion

                returnValue.Success = true;
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }
    }
}
