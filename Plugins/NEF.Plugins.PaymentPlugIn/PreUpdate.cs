using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Plugins.PaymentPlugIn
{
    public class PreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;
            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | Service |
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService adminService = serviceFactory.CreateOrganizationService(Globals.AdministratorId);
            OrganizationServiceContext orgContext = new OrganizationServiceContext(adminService);

            #region |DEFINE IMAGE IF EXISTS|
            Entity preImage = null;
            if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
            {
                preImage = (Entity)context.PreEntityImages["PreImage"];
            }
            #endregion

            #endregion

            Entity entity = (Entity)context.InputParameters["Target"];

            if (entity.Contains("new_isupdated") && (bool)entity.Attributes["new_isupdated"])
            {
                PaymentHelper.UpdateAmount(entity, preImage, adminService);
            }

            if (entity.Contains("new_paymentamount"))
            {
                Guid quoteId = preImage.GetAttributeValue<EntityReference>("new_quoteid").Id;
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

                int lastNumber = 0;
                string voucherNumber = dt.Rows.Count > 0 ? dt.Rows[0]["lastNumber"] != DBNull.Value ? dt.Rows[0]["lastNumber"].ToString() : string.Empty : string.Empty;
                if (voucherNumber != string.Empty)
                {
                    lastNumber = Convert.ToInt32(voucherNumber.Substring(voucherNumber.Length - 3));
                }
                Entity quote = adminService.Retrieve("quote", quoteId, new ColumnSet(true));
                entity["new_vnumber"] = quote.GetAttributeValue<string>("new_contractnumber") + "." + (lastNumber + 1).ToString().PadLeft(3, '0');
                entity["new_isupdated"] = false;
            }

            PaymentTypes type = entity.Attributes.Contains("new_type") ? (PaymentTypes)((OptionSetValue)entity["new_type"]).Value : preImage.Attributes.Contains("new_type") ? (PaymentTypes)((OptionSetValue)preImage["new_type"]).Value : PaymentTypes.Bos;

            #region | PRE PAYMENT PROCESS |
            if (type == PaymentTypes.KaporaOdemesi)
            {
                #region | MÜŞTERİ İLGİLİ KİŞİ İSE |
                EntityReference _c = entity.Attributes.Contains("new_contactid") ? (EntityReference)entity["new_contactid"] : preImage.Attributes.Contains("new_contactid") ? (EntityReference)preImage["new_contactid"] : null;
                if (_c != null)
                {
                    MsCrmResultObject contactResult = ContactHelper.GetContactDetail(_c.Id, sda);
                    if (contactResult.Success)
                    {
                        Contact _contact = (Contact)contactResult.ReturnObject;

                        if (_contact.Nationality.Name == "TC")
                        {
                            MsCrmResult identityControl = GeneralHelper.CheckIdentityNumber(_contact.IdentityNumber);
                            if (!identityControl.Success)
                            {
                                throw new Exception(identityControl.Result);
                            }
                        }
                    }
                }
                #endregion

                #region | MÜŞTERİ FİRMA İSE |
                EntityReference acc = entity.Attributes.Contains("new_accountid") ? (EntityReference)entity["new_accountid"] : preImage.Attributes.Contains("new_accountid") ? (EntityReference)preImage["new_accountid"] : null;
                if (acc != null)
                {
                    MsCrmResultObject accountResult = AccountHelper.GetAccountDetail(acc.Id, sda);
                    if (accountResult.Success)
                    {
                        Account _account = (Account)accountResult.ReturnObject;
                        if (string.IsNullOrEmpty(_account.TaxNumber))
                        {
                            throw new Exception("Firma vergi numarası boş olamaz!");
                        }
                    }
                }
                #endregion
            }
            #endregion
        }
    }
}
