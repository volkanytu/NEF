using System;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface ILoyaltyPointDao
    {
        LoyaltyPoint Get(Guid id);
        Guid Insert(LoyaltyPoint loyaltyPoint);
        void Update(LoyaltyPoint loyaltyPoint);
    }
}
