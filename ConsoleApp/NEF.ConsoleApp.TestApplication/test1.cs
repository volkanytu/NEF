using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.TestApplication
{
    public class test1
    {
        private ILoyaltyPointBusiness _loyaltyBusiness;

        public test1(ILoyaltyPointBusiness loyaltyBusiness)
        {
            _loyaltyBusiness = loyaltyBusiness;
        }

        public void DoWork()
        {
            Console.WriteLine("DoWork started...");

            LoyaltyPoint loyaltyPoint = _loyaltyBusiness.Get(new Guid(""));

        }
    }
}
