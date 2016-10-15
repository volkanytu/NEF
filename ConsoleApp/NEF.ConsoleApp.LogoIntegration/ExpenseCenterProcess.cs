using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using Microsoft.Xrm.Sdk;

namespace NEF.ConsoleApp.LogoIntegration
{
    public static class ExpenseCenterProcess
    {
        static IOrganizationService service = null;

        public static MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                service=MSCRM.GetOrgService(true);

                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                List<ExpenseCenter> expenseCenterList = LogoHelper.GetExpenseCenterList(sda);

                if (expenseCenterList.Count > 0)
                {
                    for (int i = 0; i < expenseCenterList.Count; i++)
                    {
                        List<LogoAccount> logoAccountList = LogoHelper.GetLogoFirmalar(expenseCenterList[i].ProjeID, sda);

                        if (logoAccountList.Count > 0)
                        {
                            for (int j = 0; j < logoAccountList.Count; j++)
                            {
                                LogoHelper.CreateExpenseCenter(expenseCenterList[i], logoAccountList[j]);
                            }
                        }

                        LogoHelper.LogoAktarimFalse(expenseCenterList[i].MasrafMerkezId, "product", service);
                    }
                }

                returnValue.Success = true;
                returnValue.Result = "Masraf merkezi entegrasyonu tamamlandı.";

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }
    }
}
