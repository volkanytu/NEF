using NEF.Library.Entities.CrmEntities;
using System;
using System.Collections.Generic;
namespace NEF.Library.Business.Interfaces
{
    public interface ILoyaltyPointBusiness
    {
        LoyaltyPoint Get(Guid id);
        Guid Insert(LoyaltyPoint loyaltyPoint);
        void Update(LoyaltyPoint loyaltyPoint);
        List<LoyaltyPoint> GetPointsWithContacts();
    }
}
