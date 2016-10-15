using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class DiscountProtocolHelper
    {
        public static void SetStatusDeactiveDiscountProtocol(Guid quoteId, IOrganizationService service, SqlDataAccess sda)
        {
            string getDiscountTypesQuery = @"SELECT 
	                                            new_sales AS QuoteId,
	                                            new_discounttype AS DiscountType,
	                                            new_discountprotocolsId AS DisccountID,
                                                new_referancesales AS ReferanceSalesId
                                            FROM
	                                            new_discountprotocols AS dp WITH(NOLOCK)
                                            WHERE 
	                                            dp.new_sales = @quoteId";

            DataTable discountProtocolsDt = sda.getDataTable(getDiscountTypesQuery, new SqlParameter[] { new SqlParameter("quoteId", quoteId) });

            foreach (DataRow item in discountProtocolsDt.Rows)
            {
                SetStateRequest state = new SetStateRequest();
                state.State = new OptionSetValue(0);
                state.Status = new OptionSetValue(2);
                state.EntityMoniker = new EntityReference("new_discountprotocols", new Guid(Convert.ToString(item["DisccountID"])));
                service.Execute(state);
            }
        }
    }
}
