using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using Microsoft.Xrm.Sdk;

namespace NEF.ConsoleApp.LogoIntegration
{
    public static class SalesProcess
    {

        public static MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                IOrganizationService service = MSCRM.GetOrgService(true);

                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                List<Satislar> satisList = LogoHelper.GetSatisList(sda);

                if (satisList.Count > 0)
                {
                    foreach (Satislar item in satisList)
                    {
                        if (item.SatisDurumu != null)
                        {
                            if (item.SatisDurumu.Equals("İPTAL"))
                            {
                                if (!LogoHelper.ControlQuotePayment(item.SatisID))
                                {
                                    LogoHelper.UpdateLogoAktarimStatus(item.SatisID, "0");
                                    continue;
                                }
                            }
                        }


                        List<LogoAccount> logofirma = LogoHelper.GetLogoFirmalar(item.ProjeID, sda);

                        if (logofirma.Count > 0)
                        {
                            foreach (LogoAccount firma in logofirma)
                            {
                                if (LogoHelper.ExecuteCari(item, firma, sda))
                                {
                                    if (!LogoHelper.SatisKontrol(item))
                                    {
                                        LogoHelper.CreateSatis(item, firma);
                                    }
                                    else
                                    {
                                        LogoHelper.CreateSatis(item, firma);
                                    }

                                    LogoHelper.ExecuteSatisOdemePlani(item.SatisID, item.SatisRef, sda);
                                    LogoHelper.UpdateLogoAktarimStatus(item.SatisID, "0");
                                }
                            }
                        }
                        else if (LogoHelper.ExecuteCari(item, null, sda))
                        {
                            if (!LogoHelper.SatisKontrol(item))
                            {
                                LogoHelper.CreateSatis(item, null);
                            }
                            else
                            {
                                LogoHelper.CreateSatis(item, null);
                            }

                            LogoHelper.ExecuteSatisOdemePlani(item.SatisID, item.SatisRef, sda);
                            LogoHelper.UpdateLogoAktarimStatus(item.SatisID, "0");
                        }
                    }
                }

                returnValue.Success = true;
                returnValue.Result = "Satış entegrasyonu tamamlandı.";

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

    }
}
