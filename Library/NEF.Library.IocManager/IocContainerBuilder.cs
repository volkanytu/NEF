using Autofac;
using NEF.DataLibrary.SqlDataLayer;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business;
using NEF.Library.Business.Interfaces;
using NEF.Library.Utility;
using SAHIBINDEN.DataLibrary.SqlDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.IocManager
{
    public static class IocContainerBuilder
    {
        private const string SQL_ACCESS_CRM = "SQL_ACCESS_CRM";
        private const string SQL_ACCESS_LOGO = "SQL_ACCESS_LOGO";

        public static IContainer GetTestIocContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.Register<IMsCrmAccess>(c => new MsCrmAccess(true)).InstancePerDependency();
            builder.Register<ISqlAccess>(c => new SqlAccess(Globals.ConnectionString))
                .Named<ISqlAccess>(SQL_ACCESS_CRM)
                .InstancePerDependency();

            builder.Register<ISqlAccess>(c => new SqlAccess(Globals.ConnectionStringLogo))
                .Named<ISqlAccess>(SQL_ACCESS_LOGO)
                .InstancePerDependency();

            builder.Register<ILoyaltyPointDao>(c => new LoyaltyPointDao(c.Resolve<IMsCrmAccess>()
                ,c.ResolveNamed<ISqlAccess>(SQL_ACCESS_CRM)))
                .InstancePerDependency();

            builder.Register<ILoyaltyPointBusiness>(c => new LoyaltyPointBusiness(c.Resolve<ILoyaltyPointDao>())).InstancePerDependency();

            return builder.Build();
        }
    }
}
