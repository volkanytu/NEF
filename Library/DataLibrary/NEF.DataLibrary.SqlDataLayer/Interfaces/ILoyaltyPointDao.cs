using System;
using System.Collections.Generic;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.Entities.CustomEntities;

namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface ILoyaltyPointDao
    {
        LoyaltyPoint Get(Guid id);
        Guid Insert(LoyaltyPoint loyaltyPoint);
        void Update(LoyaltyPoint loyaltyPoint);
        List<LoyaltySegment> GetWonPointsOfContacts();
        List<LoyaltyPoint> GetAllPointsOfContact(Guid contactId);
    }
}
