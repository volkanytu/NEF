using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.ISGYOUploadCrmDataToFtp
{
    public class GetDataFactory
    {
        private static object syncRoot = new Object();
        private Dictionary<CollaborateDataType, ICollaborateData> dictionary = new Dictionary<CollaborateDataType, ICollaborateData>();

        private GetDataFactory()
        {
            dictionary.Add(CollaborateDataType.HouseData, new GetHousesData(CollaborateDataType.HouseData));
            dictionary.Add(CollaborateDataType.OpportunityData, new GetOpportunityData(CollaborateDataType.OpportunityData));
            dictionary.Add(CollaborateDataType.ContactData, new GetContactData(CollaborateDataType.ContactData));
            dictionary.Add(CollaborateDataType.AppointmentData, new GetAppointmentData(CollaborateDataType.AppointmentData));
            dictionary.Add(CollaborateDataType.PhoneCallData, new GetPhoneCallData(CollaborateDataType.PhoneCallData));
            dictionary.Add(CollaborateDataType.SalesData, new GetSalesData(CollaborateDataType.SalesData));
            dictionary.Add(CollaborateDataType.PaymentData, new GetPaymentData(CollaborateDataType.PaymentData));
            dictionary.Add(CollaborateDataType.WebFormData, new GetWebFormsData(CollaborateDataType.WebFormData));
            dictionary.Add(CollaborateDataType.AccountData, new GetAccountData(CollaborateDataType.AccountData));
        }

        public static GetDataFactory Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly GetDataFactory instance = new GetDataFactory();
        }

        public ICollaborateData GetCollaborationData(CollaborateDataType dataType)
        {
            if (!dictionary.ContainsKey(dataType))
            {
                throw new Exception(string.Format("A GetCollaborate method of type '{0}' cannot be found", dataType.ToString()));
            }

            return dictionary[dataType];
        }
    }
}
