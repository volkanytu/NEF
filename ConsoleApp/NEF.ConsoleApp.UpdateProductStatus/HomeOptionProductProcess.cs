using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.UpdateProductStatus
{
    public static class HomeOptionProductProcess
    {
        //Developed by KBY
        //Opsiyon bitiş tarihi geçen opsiyonların durumu Tamamlandı olarak set edilir.
        public static void SetOptionState(SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                List<Guid> returnList = ProductHelper.GetOutOfDateOptions(sda); //Bitiş tarihi geçmiş opsiyonlar alınır.
                if (returnList.Count > 0)
                {
                    for (int i = 0; i < returnList.Count; i++)
                    {
                        SetStateRequest setStateReq = new SetStateRequest();
                        setStateReq.EntityMoniker = new EntityReference("new_optionofhome", returnList[i]);

                        setStateReq.State = new OptionSetValue(1);
                        setStateReq.Status = new OptionSetValue(100000002);

                        SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
                    }
                }
            }
            catch (Exception ex)
            {
   
            }
        }

        //Developed by KBY
        //Etkin opsiyonu olmayan, durum açıklamaları Yönetici Opsiyonlu konutların durumları boş olarak set edilir.
        public static void SetProductStatus(SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                MsCrmResultObject productResult = ProductHelper.GetProductSByStatus(ProductStatuses.YoneticiOpsiyonlu, sda);
                if (productResult.Success)
                {
                    List<Product> returnList = (List<Product>)productResult.ReturnObject;

                    for (int i = 0; i < returnList.Count; i++)
                    {
                        #region | SET PRODUCT STATUS |
                        MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(returnList[i].ProductId, ProductStatuses.Bos, service);
                        if (!productStatusResult.Success)
                            throw new Exception(productStatusResult.Result);
                        #endregion
                        
                    }
                }
            }
            catch (Exception ex)
            {
     
            }
        }
    }
}
