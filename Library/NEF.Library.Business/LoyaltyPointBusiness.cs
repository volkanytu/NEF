using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
