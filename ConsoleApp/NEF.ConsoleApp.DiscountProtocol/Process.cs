using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.ConsoleApp.DiscountProtocol
{
    class Process
    {
        internal static void Execute(SqlDataAccess sda, IOrganizationService service)
        {
            try
            {
                Guid quoteId = Guid.Empty;
                Guid discountProtocolId = Guid.Empty;
                Guid referanceSalesId = Guid.Empty;
                int discountType = 0;
                DataTable discountProtocolsDt = GetDiscountProtocols(sda, (int)DiscountProtocolStatus.Waiting);
                if (discountProtocolsDt.Rows.Count > 0)
                {
                    Console.WriteLine("Bulunan kayıt sayıs: " + discountProtocolsDt.Rows.Count.ToString());
                    for (int i = 0; i < discountProtocolsDt.Rows.Count; i++)
                    {
                        if (discountProtocolsDt.Rows[i]["QuoteId"] != DBNull.Value)
                            quoteId = new Guid(Convert.ToString(discountProtocolsDt.Rows[i]["QuoteId"]));
                        if (discountProtocolsDt.Rows[i]["DisccountID"] != DBNull.Value)
                            discountProtocolId = new Guid(Convert.ToString(discountProtocolsDt.Rows[i]["DisccountID"]));
                        if (discountProtocolsDt.Rows[i]["DiscountType"] != DBNull.Value)
                            discountType = Convert.ToInt32(discountProtocolsDt.Rows[i]["DiscountType"]);
                        if (discountProtocolsDt.Rows[i]["ReferanceSalesId"] != DBNull.Value)
                            referanceSalesId = new Guid(Convert.ToString(discountProtocolsDt.Rows[i]["ReferanceSalesId"]));
                        if (discountType != ((int)DiscountProtocolTypes.NefAile))
                        {
                            SetStatusDiscountProtocol(discountProtocolId, (int)DiscountProtocolStatus.Active, service);
                        }
                        else
                        {
                            if (PaymentCheckByQuoteId(sda, referanceSalesId))
                            {
                                decimal discount = GetReferanceSalesDiscount(referanceSalesId, service);
                                Entity discountProtocol = new Entity("new_discountprotocols");
                                discountProtocol.Id = discountProtocolId;
                                discountProtocol["new_discountamount"] = discount;
                                service.Update(discountProtocol);
                                SetStatusDiscountProtocol(discountProtocolId, (int)DiscountProtocolStatus.Active, service);
                            }
                        }
                        Console.Write(discountProtocolId.ToString() + " crm idli imdirim protokolü güncellenmiştir.");
                    }
                }
                else
                {
                    Console.Write("Kayıt bulunamdı.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// İndirim protokollerinin durumlarını günceller
        /// </summary>
        /// <param name="discountProtocolId">İndirim Protokolü Id</param>
        /// <param name="status">Durum</param>
        /// <param name="service">Crm Servis</param>
        private static void SetStatusDiscountProtocol(Guid discountProtocolId, int status, IOrganizationService service)
        {
            SetStateRequest state = new SetStateRequest();
            state.State = new OptionSetValue(0);
            state.Status = new OptionSetValue(status);
            state.EntityMoniker = new EntityReference("new_discountprotocols", discountProtocolId);
            service.Execute(state);
        }

        /// <summary>
        /// Bekleyen indirim protokollerini geri döndürür.
        /// </summary>
        /// <param name="sda">sda</param>
        /// <returns>Protokoller</returns>
        private static DataTable GetDiscountProtocols(SqlDataAccess sda, int status)
        {
            string discountProtocolQuery = @"SELECT 
	                                            new_sales AS QuoteId,
	                                            new_discounttype AS DiscountType,
	                                            new_discountprotocolsId AS DisccountID,
                                                new_referancesales AS ReferanceSalesId
                                            FROM
	                                            new_discountprotocols AS dp WITH(NOLOCK)
                                            WHERE 
	                                            dp.statuscode = @statu";

            return sda.getDataTable(discountProtocolQuery, new SqlParameter[] { new SqlParameter("statu", status) });
        }

        /// <summary>
        /// Satışa ait ödemelerin %25'nin ödenip ödenmediği kontrolü yapılır.
        /// </summary>
        /// <param name="sda">sql data access</param>
        /// <param name="quoteId">satış id</param>
        /// <returns></returns>
        private static bool PaymentCheckByQuoteId(SqlDataAccess sda, Guid referanceSalesId)
        {
            bool retVal = false;

            string paymentSumQuery = @"SELECT 
	                                        ISNULL(SUM(new_amount), 0) AS Amount,
	                                        ISNULL(SUM(new_paymentamount), 0) AS PaymentAmount
                                        FROM 
	                                        new_payment as py WITH(NOLOCK)
                                        WHERE 
	                                        py.statecode = 0
                                        AND
	                                        py.new_quoteid = @quoteId";

            DataTable resultDt = sda.getDataTable(paymentSumQuery, new SqlParameter[] { new SqlParameter("quoteId", referanceSalesId) });

            if (resultDt.Rows.Count > 0)
            {
                decimal amount = 0; //ödenen miktar
                decimal paymentAmount = 0; //ödenmesi gereken miktar
                Decimal.TryParse(Convert.ToString(resultDt.Rows[0]["Amount"]), out amount);
                Decimal.TryParse(Convert.ToString(resultDt.Rows[0]["PaymentAmount"]), out paymentAmount);
                if (amount != 0)
                {
                    if (((paymentAmount * 24) / 100) <= amount)
                        retVal = true;
                }
            }
            return retVal;
        }

        private static decimal GetReferanceSalesDiscount(Guid quoteId, IOrganizationService service)
        {
            decimal discount = 0;
            decimal amount = 0;
            Entity quote = service.Retrieve("quote", quoteId, new ColumnSet("totalamountlessfreight"));
            Decimal.TryParse(Convert.ToString(quote.GetAttributeValue<Money>("totalamountlessfreight").Value), out amount);
            if (amount != 0)
            {
                discount = (amount * 1) / 100;
            }
            return discount;
        }
    }
}
