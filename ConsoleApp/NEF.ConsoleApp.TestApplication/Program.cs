using Autofac;
using NEF.DataLibrary.SqlDataLayer;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.IocManager;
using NEF.Library.Utility;
using NEF.DataLibrary.SqlDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEF.Library.Entities;
using Microsoft.Xrm.Sdk;

namespace NEF.ConsoleApp.TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //IContainer container = IocContainerBuilder.GetIocContainer();

            //ILoyaltyPointBusiness loyaltyBusiness = container.Resolve<ILoyaltyPointBusiness>();

            //LoyaltyPoint p = loyaltyBusiness.Get(new Guid("117436E3-8393-E611-8103-005056A60603"));

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | GET PROJECT DETAIL |
            Library.Utility.Project project = null;
            EntityReference projectId = new EntityReference("new_project", new Guid("CB9CC4EF-1117-E011-817F-00123F4DA0F7"));

            MsCrmResultObject projectResultObject = ProjectHelper.GetProjectDetail(projectId.Id, sda);
            if (projectResultObject.Success)
            {
                project = (Library.Utility.Project)projectResultObject.ReturnObject;
            }
            else
            {
                throw new Exception(projectResultObject.Result);
            }
            #endregion

            #region | GET SALES AMOUNT |
            decimal? salesAmount = null;
            EntityReference quoteId = new EntityReference("quote", new Guid("475A1A3F-5F95-E611-8103-005056A60603"));

            //entity["new_name"] = quoteId.Name;

            MsCrmResultObject salesAmountResultObject = ProjectHelper.GetTotalSalesAmount(quoteId.Id, sda);
            if (salesAmountResultObject.Success)
            {
                salesAmount = (decimal)salesAmountResultObject.ReturnObject;
            }
            else
            {
                throw new Exception(salesAmountResultObject.Result);
            }
            #endregion

            if (project != null)
            {
                if (project.Ratio != null && salesAmount != null)
                {
                    decimal pointAmount = (decimal)((salesAmount * project.Ratio));
                    var a = pointAmount;
                }

                if (project.ExpireDate != null)
                {
                    var b = (DateTime)project.ExpireDate;
                }
            }

            //LoyaltyPoint lp = new LoyaltyPoint()
            //{
            //    Name = "123",
            //    ContactId = new EntityReferenceWrapper()
            //    {
            //        Id = new Guid(""),
            //        LogicalName = "contact"
            //    },
            //    PointType = LoyaltyPoint.PointTypeCode.CARD.ToOptionSetValueWrapper()
            //};


            //Guid id = loyaltyBusiness.Insert(lp);


        }
    }
}
