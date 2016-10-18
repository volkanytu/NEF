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
            ContainerBuilder builder = IocContainerBuilder.GetIocContainerBuilder();

            builder.Register<ILoyaltySegmentCalculate>(p => new LoyaltySegmentCalculate(p.Resolve<ILoyaltyPointBusiness>()
                , p.Resolve<ILoyaltySegmentConfigBusiness>()
                , p.Resolve<IContactBusiness>())).InstancePerDependency();

            IContainer container = builder.Build();

            ILoyaltySegmentCalculate calculate = container.Resolve<ILoyaltySegmentCalculate>();
            calculate.DoWork();
        }
    }
}
