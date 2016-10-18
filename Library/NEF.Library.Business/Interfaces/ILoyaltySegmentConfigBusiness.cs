using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Business.Interfaces
{
    public interface ILoyaltySegmentConfigBusiness
    {
        LoyaltySegmentConfig Get(Guid id);
        Guid Insert(LoyaltySegmentConfig loyaltySegmentConfig);
        void Update(LoyaltySegmentConfig loyaltySegmentConfig);
        List<LoyaltySegmentConfig> GetList();
    }
}
