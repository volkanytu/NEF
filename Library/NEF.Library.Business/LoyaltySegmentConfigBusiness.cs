using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Business
{
    public class LoyaltySegmentConfigBusiness : ILoyaltySegmentConfigBusiness
    {
        private ILoyaltySegmentConfigDao _loyaltySegmentConfigDao;

        public LoyaltySegmentConfigBusiness(ILoyaltySegmentConfigDao loyaltySegmentConfigDao)
        {
            _loyaltySegmentConfigDao = loyaltySegmentConfigDao;
        }

        public Guid Insert(LoyaltySegmentConfig loyaltySegmentConfig)
        {
            return _loyaltySegmentConfigDao.Insert(loyaltySegmentConfig);
        }

        public void Update(LoyaltySegmentConfig loyaltySegmentConfig)
        {
            _loyaltySegmentConfigDao.Update(loyaltySegmentConfig);
        }

        public LoyaltySegmentConfig Get(Guid id)
        {
            return _loyaltySegmentConfigDao.Get(id);
        }

        public List<LoyaltySegmentConfig> GetList()
        {
            return _loyaltySegmentConfigDao.GetList();
        }
    }
}
