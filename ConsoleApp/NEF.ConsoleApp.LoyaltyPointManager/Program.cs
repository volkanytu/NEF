using Autofac;
using NEF.Library.Business.Interfaces;
using NEF.Library.IocManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.ConsoleApp.LoyaltyPointManager
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = IocContainerBuilder.GetIocContainer();

            ILoyaltyPointBusiness loyaltyBusiness = container.Resolve<ILoyaltyPointBusiness>();
            List<LoyaltyPoint> loyaltyPoints = loyaltyBusiness.GetPointsWithContacts();

            //Burada config ayarları çekilecek
            //müşteriler üzerinde hangi aralığa girdiklerine karar verilip set edilecek

        }
    }
}
