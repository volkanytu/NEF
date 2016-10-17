﻿using Autofac;
using NEF.DataLibrary.SqlDataLayer;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business;
using NEF.Library.Business.Interfaces;
using SAHIBINDEN.DataLibrary.SqlDataLayer;
using NEF.Library.Utility;

namespace NEF.Library.IocManager
{
    public static class IocContainerBuilder
    {
        private const string SQL_ACCESS_CRM = "SQL_ACCESS_CRM";
        private const string SQL_ACCESS_LOGO = "SQL_ACCESS_LOGO";

        public static IContainer GetIocContainer()
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

            builder.Register<IQuoteDao>(p => new QuoteDao(p.Resolve<IMsCrmAccess>(), p.Resolve<ISqlAccess>()))
                .InstancePerDependency();
            builder.Register<IQuoteBusiness>(p => new QuoteBusiness(p.Resolve<IQuoteDao>())).InstancePerDependency();

            builder.Register<IProjectDao>(p => new ProjectDao(p.Resolve<IMsCrmAccess>(), p.Resolve<ISqlAccess>()))
                .InstancePerDependency();
            builder.Register<IProjectBusiness>(p => new ProjectBusiness(p.Resolve<IProjectDao>())).InstancePerDependency();

            builder.Register<ILoyaltySegmentConfigDao>(p => new LoyaltySegmentConfigDao(p.Resolve<IMsCrmAccess>(), p.Resolve<ISqlAccess>()))
                .InstancePerDependency();
            builder.Register<ILoyaltySegmentConfigBusiness>(p => new LoyaltySegmentConfigBusiness(p.Resolve<ILoyaltySegmentConfigDao>())).InstancePerDependency();

            return builder.Build();
        }
    }
}
