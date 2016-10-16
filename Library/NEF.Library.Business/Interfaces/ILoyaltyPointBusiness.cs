using NEF.Library.Entities.CrmEntities;
using System;
namespace NEF.Library.Business.Interfaces
{
    public interface ILoyaltyPointBusiness
    {
        LoyaltyPoint Get(Guid id);
        Guid Insert(LoyaltyPoint loyaltyPoint);
        void Update(LoyaltyPoint loyaltyPoint);
    }
}
