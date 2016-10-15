using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class SalesControlHelper
    {
        internal static void ChangeFieldsMail(Entity preImage, Entity postImage, IOrganizationService adminService)
        {
            ArrayList updateFields = new ArrayList();
            ArrayList oldValue = new ArrayList();
            ArrayList newValue = new ArrayList();
            string modifiedOn = ((DateTime)postImage["modifiedon"]).ToString("dd.MM.yyyy");
            string modifiedBy = ((EntityReference)postImage["modifiedby"]).Name;
            string projectName = postImage.Contains("new_project")?((EntityReference)postImage["new_project"]).Name:string.Empty;

            #region Satış Dan. İnd. Oranı
            if (!preImage.Contains("new_salesconsultantdiscountrate") && postImage.Contains("new_salesconsultantdiscountrate"))
            {
                updateFields.Add("Satış Dan. İnd. Oranı");
                oldValue.Add(" - ");
                newValue.Add(((decimal)postImage["new_salesconsultantdiscountrate"]).ToString("N2"));
            }
            else if (preImage.Contains("new_salesconsultantdiscountrate") && !postImage.Contains("new_salesconsultantdiscountrate"))
            {
                updateFields.Add("Satış Dan. İnd. Oranı");
                oldValue.Add(((decimal)preImage["new_salesconsultantdiscountrate"]).ToString("N2"));
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesconsultantdiscountrate") && postImage.Contains("new_salesconsultantdiscountrate") &&
                     (decimal)preImage["new_salesconsultantdiscountrate"] != (decimal)postImage["new_salesconsultantdiscountrate"])
            {
                updateFields.Add("Satış Dan. İnd. Oranı");
                oldValue.Add(((decimal)preImage["new_salesconsultantdiscountrate"]).ToString("N2"));
                newValue.Add(((decimal)postImage["new_salesconsultantdiscountrate"]).ToString("N2"));
            }
            #endregion

            #region Satış Dan. Ops. Süresi (gün)

            if (!preImage.Contains("new_salesconsultantoptionday") && postImage.Contains("new_salesconsultantoptionday"))
            {
                updateFields.Add("Satış Dan. Ops. Süresi (gün)");
                oldValue.Add(" - ");
                newValue.Add(postImage["new_salesconsultantoptionday"].ToString());
            }
            else if (preImage.Contains("new_salesconsultantoptionday") && !postImage.Contains("new_salesconsultantoptionday"))
            {
                updateFields.Add("Satış Dan. Ops. Süresi (gün)");
                oldValue.Add(preImage["new_salesconsultantoptionday"].ToString());
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesconsultantoptionday") && postImage.Contains("new_salesconsultantoptionday") &&
                     (int)preImage["new_salesconsultantoptionday"] != (int)postImage["new_salesconsultantoptionday"])
            {
                updateFields.Add("Satış Dan. Ops. Süresi (gün)");
                oldValue.Add(preImage["new_salesconsultantoptionday"].ToString());
                newValue.Add(postImage["new_salesconsultantoptionday"].ToString());
            }
            #endregion

            #region Satış Dan. Birim m2 Fiyatı (sınır)

            if (!preImage.Contains("new_salesconsultantpersquaremeter") && postImage.Contains("new_salesconsultantpersquaremeter"))
            {
                updateFields.Add("Satış Dan. Birim m2 Fiyatı (sınır)");
                oldValue.Add(" - ");
                newValue.Add(((Money)postImage["new_salesconsultantpersquaremeter"]).Value.ToString("N2"));
            }
            else if (preImage.Contains("new_salesconsultantpersquaremeter") && !postImage.Contains("new_salesconsultantpersquaremeter"))
            {
                updateFields.Add("Satış Dan. Birim m2 Fiyatı (sınır)");
                oldValue.Add(((Money)preImage["new_salesconsultantpersquaremeter"]).Value.ToString("N2"));
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesconsultantpersquaremeter") && postImage.Contains("new_salesconsultantpersquaremeter") &&
                     ((Money)preImage["new_salesconsultantpersquaremeter"]).Value != ((Money)postImage["new_salesconsultantpersquaremeter"]).Value)
            {
                updateFields.Add("Satış Dan. Birim m2 Fiyatı (sınır)");
                oldValue.Add(((Money)preImage["new_salesconsultantpersquaremeter"]).Value.ToString("N2"));
                newValue.Add(((Money)postImage["new_salesconsultantpersquaremeter"]).Value.ToString("N2"));
            }
            #endregion

            #region Satış Müd. İnd. Oranı
            if (!preImage.Contains("new_salesmanagerdiscountrate") && postImage.Contains("new_salesmanagerdiscountrate"))
            {
                updateFields.Add("Satış Müd. İnd. Oranı");
                oldValue.Add(" - ");
                newValue.Add(((decimal)postImage["new_salesmanagerdiscountrate"]).ToString("N2"));
            }
            else if (preImage.Contains("new_salesmanagerdiscountrate") && !postImage.Contains("new_salesmanagerdiscountrate"))
            {
                updateFields.Add("Satış Müd. İnd. Oranı");
                oldValue.Add(((decimal)preImage["new_salesmanagerdiscountrate"]).ToString("N2"));
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesmanagerdiscountrate") && postImage.Contains("new_salesmanagerdiscountrate") &&
                     (decimal)preImage["new_salesmanagerdiscountrate"] != (decimal)postImage["new_salesmanagerdiscountrate"])
            {
                updateFields.Add("Satış Müd. İnd. Oranı");
                oldValue.Add(((decimal)preImage["new_salesmanagerdiscountrate"]).ToString("N2"));
                newValue.Add(((decimal)postImage["new_salesmanagerdiscountrate"]).ToString("N2"));
            }
            #endregion

            #region Satış Müd. Ops. Süresi (gün)

            if (!preImage.Contains("new_salesmanageroptionday") && postImage.Contains("new_salesmanageroptionday"))
            {
                updateFields.Add("Satış Müd. Ops. Süresi (gün)");
                oldValue.Add(" - ");
                newValue.Add(postImage["new_salesmanageroptionday"].ToString());
            }
            else if (preImage.Contains("new_salesmanageroptionday") && !postImage.Contains("new_salesmanageroptionday"))
            {
                updateFields.Add("Satış Müd. Ops. Süresi (gün)");
                oldValue.Add(preImage["new_salesmanageroptionday"].ToString());
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesmanageroptionday") && postImage.Contains("new_salesmanageroptionday") &&
                     (int)preImage["new_salesmanageroptionday"] != (int)postImage["new_salesmanageroptionday"])
            {
                updateFields.Add("Satış Müd. Ops. Süresi (gün)");
                oldValue.Add(preImage["new_salesmanageroptionday"].ToString());
                newValue.Add(postImage["new_salesmanageroptionday"].ToString());
            }
            #endregion

            #region Satış Müd. Birim m2 Fiyatı (sınır)

            if (!preImage.Contains("new_salesmanagerpersquaremeter") && postImage.Contains("new_salesmanagerpersquaremeter"))
            {
                updateFields.Add("Satış Müd. Birim m2 Fiyatı (sınır)");
                oldValue.Add(" - ");
                newValue.Add(((Money)postImage["new_salesmanagerpersquaremeter"]).Value.ToString("N2"));
            }
            else if (preImage.Contains("new_salesmanagerpersquaremeter") && !postImage.Contains("new_salesmanagerpersquaremeter"))
            {
                updateFields.Add("Satış Müd. Birim m2 Fiyatı (sınır)");
                oldValue.Add(((Money)preImage["new_salesmanagerpersquaremeter"]).Value.ToString("N2"));
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesmanagerpersquaremeter") && postImage.Contains("new_salesmanagerpersquaremeter") &&
                     ((Money)preImage["new_salesmanagerpersquaremeter"]).Value != ((Money)postImage["new_salesmanagerpersquaremeter"]).Value)
            {
                updateFields.Add("Satış Müd. Birim m2 Fiyatı (sınır)");
                oldValue.Add(((Money)preImage["new_salesmanagerpersquaremeter"]).Value.ToString("N2"));
                newValue.Add(((Money)postImage["new_salesmanagerpersquaremeter"]).Value.ToString("N2"));
            }
            #endregion

            #region Satış Dir. İnd. Oranı
            if (!preImage.Contains("new_salesdirectordiscountrate") && postImage.Contains("new_salesdirectordiscountrate"))
            {
                updateFields.Add("Satış Dir. İnd. Oranı");
                oldValue.Add(" - ");
                newValue.Add(((decimal)postImage["new_salesdirectordiscountrate"]).ToString("N2"));
            }
            else if (preImage.Contains("new_salesdirectordiscountrate") && !postImage.Contains("new_salesdirectordiscountrate"))
            {
                updateFields.Add("Satış Dir. İnd. Oranı");
                oldValue.Add(((decimal)preImage["new_salesdirectordiscountrate"]).ToString("N2"));
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesdirectordiscountrate") && postImage.Contains("new_salesdirectordiscountrate") &&
                     (decimal)preImage["new_salesdirectordiscountrate"] != (decimal)postImage["new_salesdirectordiscountrate"])
            {
                updateFields.Add("Satış Dir. İnd. Oranı");
                oldValue.Add(((decimal)preImage["new_salesdirectordiscountrate"]).ToString("N2"));
                newValue.Add(((decimal)postImage["new_salesdirectordiscountrate"]).ToString("N2"));
            }
            #endregion

            #region Satış Dir. Ops. Süresi (gün)

            if (!preImage.Contains("new_salesdirectoroptionday") && postImage.Contains("new_salesdirectoroptionday"))
            {
                updateFields.Add("Satış Dir. Ops. Süresi (gün)");
                oldValue.Add(" - ");
                newValue.Add(postImage["new_salesdirectoroptionday"].ToString());
            }
            else if (preImage.Contains("new_salesdirectoroptionday") && !postImage.Contains("new_salesdirectoroptionday"))
            {
                updateFields.Add("Satış Dir. Ops. Süresi (gün)");
                oldValue.Add(preImage["new_salesdirectoroptionday"].ToString());
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesdirectoroptionday") && postImage.Contains("new_salesdirectoroptionday") &&
                     (int)preImage["new_salesdirectoroptionday"] != (int)postImage["new_salesdirectoroptionday"])
            {
                updateFields.Add("Satış Dir. Ops. Süresi (gün)");
                oldValue.Add(preImage["new_salesdirectoroptionday"].ToString());
                newValue.Add(postImage["new_salesdirectoroptionday"].ToString());
            }
            #endregion

            #region Satış Dir. Birim m2 Fiyatı (sınır)

            if (!preImage.Contains("new_salesdirectorpersquaremeter") && postImage.Contains("new_salesdirectorpersquaremeter"))
            {
                updateFields.Add("Satış Dir. Birim m2 Fiyatı (sınır)");
                oldValue.Add(" - ");
                newValue.Add(((Money)postImage["new_salesdirectorpersquaremeter"]).Value.ToString("N2"));
            }
            else if (preImage.Contains("new_salesdirectorpersquaremeter") && !postImage.Contains("new_salesdirectorpersquaremeter"))
            {
                updateFields.Add("Satış Dir. Birim m2 Fiyatı (sınır)");
                oldValue.Add(((Money)preImage["new_salesdirectorpersquaremeter"]).Value.ToString("N2"));
                newValue.Add(" - ");
            }
            else if (preImage.Contains("new_salesdirectorpersquaremeter") && postImage.Contains("new_salesdirectorpersquaremeter") &&
                     ((Money)preImage["new_salesdirectorpersquaremeter"]).Value != ((Money)postImage["new_salesdirectorpersquaremeter"]).Value)
            {
                updateFields.Add("Satış Dir. Birim m2 Fiyatı (sınır)");
                oldValue.Add(((Money)preImage["new_salesdirectorpersquaremeter"]).Value.ToString("N2"));
                newValue.Add(((Money)postImage["new_salesdirectorpersquaremeter"]).Value.ToString("N2"));
            }
            #endregion

            #region Create and send Mail
            if (updateFields.Count > 0)
            {

                Entity fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);

                Entity toParty = new Entity("activityparty");
                toParty["addressused"] = "olcay.sen@innthebox.com";

                EntityCollection bccCollection = new EntityCollection();
                Entity bccParty1 = new Entity("activityparty");
                Entity bccParty2 = new Entity("activityparty");
                //Entity bccParty3 = new Entity("activityparty");
                bccParty1["addressused"] = "erkan.ozvar@nef.com.tr";
                bccParty2["addressused"] = "erhan.serter@nef.com.tr";
                //bccParty3["addressused"] = "aleksi.komorosano@nef.com.tr";
                bccCollection.Entities.Add(bccParty1);
                bccCollection.Entities.Add(bccParty2);
                //bccCollection.Entities.Add(bccParty3);

                Entity email = new Entity("email");
                email["to"] = new Entity[] { toParty };
                email["from"] = new Entity[] { fromParty };
                email["bcc"] = bccCollection;
                email["subject"] = "Satış Kontrol Kuralı Güncelleme";
                email["description"] = MailInfoUpdate(projectName, modifiedOn, modifiedBy, updateFields, oldValue, newValue);
                email["directioncode"] = true;
                Guid id = adminService.Create(email);
               
                #region Send Email
                var req = new SendEmailRequest
                {
                    EmailId = id,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };

                try
                {
                    var res = (SendEmailResponse)adminService.Execute(req);

                }
                catch (Exception ex)
                {

                }
                #endregion

            }
            #endregion Create and send Mail
        }
        private static object MailInfoUpdate(string projectName, string modifiedOn, string modifiedBy, ArrayList updateFields, ArrayList oldValue, ArrayList newValue)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table border='1' cellpadding='5' cellspacing='0' ");
            sb.Append("style='border: solid 1px Silver; font-size: x-small;'>");

            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("Proje Adı : " + projectName);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("İşlem Tarihi : " + modifiedOn);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("İşlemi Yapan Kullanıcı : " + modifiedBy);
            sb.Append("</td>");
            sb.Append("</tr>");

            for (int i = 0; i < updateFields.Count; i++)
            {
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("Güncelleme Yapılan Alan : " + updateFields[i].ToString());
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("Eski Değeri : " + oldValue[i].ToString());
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("Yeni Değeri : " + newValue[i].ToString());
                sb.Append("</td>");

                sb.Append("</tr>");
            }


            sb.Append("</table>");
            return sb.ToString();
        }
    }
}
