﻿using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Business;

namespace NEF.Plugins.LoyaltyPointPlugIn
{
    public class PreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SqlDataAccess sda = null;

            sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            try
            {

                #region | SERVICE |
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                #region | Validate Request |
                //Target yoksa veya Entity tipinde değilse, devam etme.
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                {
                    return;
                }
                #endregion

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                #endregion

                Entity entity = (Entity)context.InputParameters["Target"];

                #region | GET PROJECT DETAIL |
                Project project = null;
                if (entity.Attributes.Contains("new_projectid") && entity["new_projectid"] != null)
                {
                    EntityReference projectId = (EntityReference)entity["new_projectid"];

                    MsCrmResultObject projectResultObject = ProjectHelper.GetProjectDetail(projectId.Id, sda);
                    if (projectResultObject.Success)
                    {
                        project = (Project)projectResultObject.ReturnObject;
                    }
                    else
                    {
                        throw new Exception(projectResultObject.Result);
                    }
                }
                #endregion

                #region | GET SALES AMOUNT |
                decimal? salesAmount = null;
                if (entity.Attributes.Contains("new_quoteid") && entity["new_quoteid"] != null)
                {
                    EntityReference quoteId = (EntityReference)entity["new_quoteid"];

                    entity["new_name"] = quoteId.Name;

                    MsCrmResultObject salesAmountResultObject = ProjectHelper.GetTotalSalesAmount(quoteId.Id, sda);
                    if (salesAmountResultObject.Success)
                    {
                        salesAmount = (decimal)salesAmountResultObject.ReturnObject;
                    }
                    else
                    {
                        throw new Exception(salesAmountResultObject.Result);
                    }
                }
                #endregion


                if (project != null)
                {
                    if (project.Ratio != null && salesAmount != null)
                    {
                        decimal pointAmount = (decimal)((salesAmount * project.Ratio));
                        entity["new_amount"] = pointAmount;
                    }

                    if (project.ExpireDate != null)
                    {
                        entity["new_expiredate"] = (DateTime)project.ExpireDate;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            finally
            {
                if (sda != null)
                    sda.closeConnection();
            }
        }
    }
}