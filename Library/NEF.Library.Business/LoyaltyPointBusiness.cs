using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities;

namespace NEF.Library.Business
{
    public class LoyaltyPointBusiness : ILoyaltyPointBusiness
    {
        private ILoyaltyPointDao _loyaltyPointDao;

        public LoyaltyPointBusiness(ILoyaltyPointDao loyaltyPointDao)
        {
            _loyaltyPointDao = loyaltyPointDao;
        }

        public Guid Insert(LoyaltyPoint loyaltyPoint)
        {
            return _loyaltyPointDao.Insert(loyaltyPoint);
        }

        public void Update(LoyaltyPoint loyaltyPoint)
        {
            _loyaltyPointDao.Update(loyaltyPoint);
        }

        public LoyaltyPoint Get(Guid id)
        {
            return _loyaltyPointDao.Get(id);
        }

        public List<LoyaltySegment> GetPointsWithContacts()
        {
            return _loyaltyPointDao.GetPointsWithContacts();
        }

        public void SetContactLoyaltySegment(List<LoyaltySegment> loyaltySegmentList, List<LoyaltySegmentConfig> loyaltySegmentConfigList)
        {
            loyaltySegmentList.Select(sl => sl.Segment = loyaltySegmentConfigList
                .Where(conf => conf.MinValue < sl.TotalPoint && conf.MaxValue > sl.TotalPoint)
                .FirstOrDefault().LoyaltySegment.ToEnum<Contact.LoyaltySegmentCode>());
        }
    }
}
