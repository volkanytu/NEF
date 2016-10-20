using Microsoft.Xrm.Sdk;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Constants.SqlQueries;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.DataLibrary.SqlDataLayer
{
    public class LoyaltyPointDao : ILoyaltyPointDao
    {
        private IMsCrmAccess _msCrmAccess;
        private ISqlAccess _sqlAccess;

        public LoyaltyPointDao(IMsCrmAccess msCrmAccess, ISqlAccess sqlAccess)
        {
            _msCrmAccess = msCrmAccess;
            _sqlAccess = sqlAccess;
        }

        public Guid Insert(LoyaltyPoint loyaltyPoint)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = loyaltyPoint.ToCrmEntity();

            Guid id = service.Create(entity);

            return id;
        }

        public void Update(LoyaltyPoint loyaltyPoint)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = loyaltyPoint.ToCrmEntity();

            service.Update(entity);
        }

        public LoyaltyPoint Get(Guid id)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@id", id) };

            DataTable dt = _sqlAccess.GetDataTable(LoyaltyPointQueries.GET_LOYTALTY_POINT, parameters);

            return dt.ToList<LoyaltyPoint>().FirstOrDefault();
        }

        public List<LoyaltySegment> GetWonPointsOfContacts()
        {
            DataTable dt = _sqlAccess.GetDataTable(LoyaltyPointQueries.GET_WON_POINTS_OF_CONTACT);

            return dt.ToList<LoyaltySegment>();
        }

        public List<LoyaltyPoint> GetAllPointsOfContact(Guid contactId)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@contactId", contactId) };

            DataTable dt = _sqlAccess.GetDataTable(LoyaltyPointQueries.GET_ALL_POINTS_OF_CONTACT, parameters);

            return dt.ToList<LoyaltyPoint>();
        }
    }
}
