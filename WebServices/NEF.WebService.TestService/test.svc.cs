using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.WebService.TestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class test : Itest
    {
        public Contact GetContactDetail()
        {
            Contact returnValue = new Contact();

            SqlDataAccess sda=new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            MsCrmResultObject result = ContactHelper.GetContactDetail(new Guid("77ACE19D-8827-E311-9322-842B2B64B22F"), sda);

            if(result.Success)
            {
                returnValue = (Contact)result.ReturnObject;
            }

            return returnValue;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
