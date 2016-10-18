using NEF.Library.Entities.CrmEntities;
using NEF.Library.Entities.CustomEntities;
using System;
using System.Collections.Generic;
namespace NEF.Library.Business.Interfaces
{
    public interface ILoyaltyPointBusiness
    {
        LoyaltyPoint Get(Guid id);
        Guid Insert(LoyaltyPoint loyaltyPoint);
        void Update(LoyaltyPoint loyaltyPoint);
        List<LoyaltySegment> GetPointsWithContacts();
        void SetContactLoyaltySegment(List<LoyaltySegment> loyaltySegmentList, List<LoyaltySegmentConfig> loyaltySegmentConfigList);
    }
}
