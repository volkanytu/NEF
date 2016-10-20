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
        List<LoyaltySegment> GetWonPointsOfContacts();
        void SetContactLoyaltySegment(List<LoyaltySegment> loyaltySegmentList, List<LoyaltySegmentConfig> loyaltySegmentConfigList);
        List<LoyaltyPoint> GetAllPointsOfContact(Guid contactId);
        decimal GetBalanceOfContact(Guid contactId);
        void TransferPoints(Guid sourceContactId, Guid targetContactId, EntityReferenceWrapper erPointTransfer = null);
        void ConfirmPointUsage(Guid loyaltyPointId);
        void RefusePointUsage(Guid loyaltyPointId);
        LoyaltyPointSummary GetContactPointSummary(Guid contactId);
    }
}
