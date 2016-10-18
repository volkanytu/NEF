using NEF.Library.Entities.CrmEntities;
using System;
namespace NEF.Library.Business.Interfaces
{
    public interface IContactBusiness
    {
        Contact Get(Guid id);
        Guid Insert(Contact contact);
        void Update(Contact contact);
        void SetLoyaltySegment(Guid contactId, Contact.LoyaltySegmentCode? segmentCode);
    }
}
