using Autofac;
using NEF.DataLibrary.SqlDataLayer;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.IocManager;
using NEF.Library.Utility;
using SAHIBINDEN.DataLibrary.SqlDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEF.Library.Entities;

namespace NEF.ConsoleApp.TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = IocContainerBuilder.GetTestIocContainer();

            ILoyaltyPointBusiness loyaltyBusiness = container.Resolve<ILoyaltyPointBusiness>();

            LoyaltyPoint lp = new LoyaltyPoint()
            {
                Name = "123",
                ContactId = new EntityReferenceWrapper()
                {
                    Id = new Guid(""),
                    LogicalName = "contact"
                },
                PointType = LoyaltyPoint.PointTypeCode.CARD.ToOptionSetValueWrapper()
            };


            Guid id = loyaltyBusiness.Insert(lp);


        }
    }
}
