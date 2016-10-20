using Microsoft.Xrm.Sdk;
using NEF.DataLibrary.SqlDataLayer;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business;
using NEF.Library.Business.Interfaces;
using NEF.Library.Utility;
using NEF.DataLibrary.SqlDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Plugins.PointTransferPlugin
{
    public static class Initializer
    {
        public static ILoyaltyPointBusiness LoyatyPointBusiness;

        public static void Init(IOrganizationService service)
        {
            ISqlAccess sqlAccess = new SqlAccess(Globals.ConnectionString);
            IMsCrmAccess msCrmAccess = new MsCrmAccess(true);
            msCrmAccess.CrmService = service;

            ILoyaltyPointDao loyaltyPointDao = new LoyaltyPointDao(msCrmAccess, sqlAccess);

            LoyatyPointBusiness = new LoyaltyPointBusiness(loyaltyPointDao);
        }
    }
}
