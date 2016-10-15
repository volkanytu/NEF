using Microsoft.Xrm.Sdk;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.UpdateProductStatus
{
    public static class ProductProcess
    {
        public static void Process(SqlDataAccess sda, IOrganizationService service)
        {
            try
            {

                #region | GET PRODUCTS |
                MsCrmResultObject productResult = ProductHelper.GetOutOfOptionProducts(sda);

                if (productResult.Success)
                {
                    List<Product> returnList = (List<Product>)productResult.ReturnObject;

                    for (int i = 0; i < returnList.Count; i++)
                    {

                        MsCrmResult productStatusResult = ProductHelper.UpdateHouseStatus(returnList[i].ProductId, ProductStatuses.Bos, service);
                        if (!productStatusResult.Success)
                        {
                            Console.SetCursorPosition(0, 6);
                            Console.WriteLine(productStatusResult.Result);
                        }
                    }
                }
                else
                {
                    Console.SetCursorPosition(0, 6);
                    Console.WriteLine(productResult.Result);
                }
                #endregion
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
        }
    }
}
