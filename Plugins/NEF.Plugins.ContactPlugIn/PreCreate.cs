using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NEF.Plugins.ContactPlugIn
{
    public class PreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {

                #region | SERVICE |
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                #region | Validate Request |
                //Target yoksa veya Entity tipinde değilse, devam etme.
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                {
                    return;
                }
                #endregion

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                #endregion

                Entity entity = (Entity)context.InputParameters["Target"];

                #region | SET TITLECASE |
                if (entity.Attributes.Contains("firstname") && !string.IsNullOrEmpty(entity["firstname"].ToString()))
                    entity["firstname"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entity["firstname"].ToString().ToLower());
                if (entity.Attributes.Contains("lastname") && !string.IsNullOrEmpty(entity["lastname"].ToString()))
                    entity["lastname"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entity["lastname"].ToString().ToLower());
                #endregion

                #region | CHECK DUPLICATE |
                if (entity.Attributes.Contains("new_tcidentitynumber"))
                {
                    if (entity["new_tcidentitynumber"] != null)
                    {
                        string identityNumber = entity["new_tcidentitynumber"].ToString();

                        MsCrmResult identityResult = ContactHelper.CheckDuplicateIdentity(identityNumber, sda);
                        if (!identityResult.Success)
                            throw new Exception(identityResult.Result);
                    }
                }

                if (entity.Attributes.Contains("emailaddress1"))
                {
                    string email = entity["emailaddress1"].ToString();

                    MsCrmResult emailResult = ContactHelper.CheckDuplicateEmail(email, sda);
                    if (!emailResult.Success)
                        throw new Exception(emailResult.Result);
                }
                else
                {
                    throw new Exception("Email adresi boş bırakılamaz!");
                }

                if (entity.Attributes.Contains("mobilephone"))
                {
                    string mobilePhone = entity["mobilephone"].ToString();

                    MsCrmResult phoneResult = ContactHelper.CheckDuplicatePhone(mobilePhone, sda);
                    if (!phoneResult.Success)
                        throw new Exception(phoneResult.Result);

                    TelephoneNumber number = GeneralHelper.CheckTelephoneNumber(mobilePhone);
                    if (number.isFormatOK)
                    {
                        entity["new_countrycodemobilephone"] = number.countryCode;
                        entity["new_cellphonenumber"] = number.phoneNo;
                    }
                    else
                    {
                        throw new Exception("Lütfen doğru bir telefon giriniz!");
                    }
                }
                else
                {
                    throw new Exception("Cep Telefonu boş bırakılamaz!");
                }
                #endregion

                #region | SET CUSTOMER NUMBER|
                ContactHelper.SetCustomerNumber(entity, sda);
                #endregion

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
        }
    }
}
