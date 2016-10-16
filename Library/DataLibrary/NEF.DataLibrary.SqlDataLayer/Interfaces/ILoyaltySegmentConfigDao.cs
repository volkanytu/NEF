using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface ILoyaltySegmentConfigDao
    {
        LoyaltySegmentConfig Get(Guid id);
        Guid Insert(LoyaltySegmentConfig loyaltySegmentConfig);
        void Update(LoyaltySegmentConfig loyaltySegmentConfig);
    }
}
