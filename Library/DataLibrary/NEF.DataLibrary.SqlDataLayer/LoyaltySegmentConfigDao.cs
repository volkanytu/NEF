using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Constants.SqlQueries;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer
{
    public class LoyaltySegmentConfigDao : ILoyaltySegmentConfigDao
    {
        private IMsCrmAccess _msCrmAccess;
        private ISqlAccess _sqlAccess;

        public LoyaltySegmentConfigDao(IMsCrmAccess msCrmAccess, ISqlAccess sqlAccess)
        {
            _msCrmAccess = msCrmAccess;
            _sqlAccess = sqlAccess;
        }

        public Guid Insert(LoyaltySegmentConfig loyaltySegmentConfig)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = loyaltySegmentConfig.ToCrmEntity();

            Guid id = service.Create(entity);

            return id;
        }

        public void Update(LoyaltySegmentConfig loyaltySegmentConfig)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = loyaltySegmentConfig.ToCrmEntity();

            service.Update(entity);
        }

        public LoyaltySegmentConfig Get(Guid id)
        {
            SqlParameter[] parameters = { new SqlParameter("@id", id) };

            DataTable dt = _sqlAccess.GetDataTable(LoyaltySegmentConfigQueries.GET_CONFIG, parameters);

            return dt.ToList<LoyaltySegmentConfig>().FirstOrDefault();
        }

        public List<LoyaltySegmentConfig> GetList()
        {
            DataTable dt = _sqlAccess.GetDataTable(LoyaltySegmentConfigQueries.GET_LIST);

            return dt.ToList<LoyaltySegmentConfig>();
        }
    }
}
