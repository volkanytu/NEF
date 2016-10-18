using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities;

namespace NEF.Library.Business
{
    public class ContactBusiness : IContactBusiness
    {
        private IContactDao _contactDao;

        public ContactBusiness(IContactDao contactDao)
        {
            _contactDao = contactDao;
        }

        public Guid Insert(Contact contact)
        {
            return _contactDao.Insert(contact);
        }

        public void Update(Contact contact)
        {
            _contactDao.Update(contact);
        }

        public Contact Get(Guid id)
        {
            return _contactDao.Get(id);
        }

        public void SetLoyaltySegment(Guid contactId, Contact.LoyaltySegmentCode? segmentCode)
        {
            if(segmentCode==null)
            {
                return;
            }

            Contact cnt = new Contact();
            cnt.Id = contactId;
            cnt.LoyaltySegment = segmentCode.ToOptionSetValueWrapper();

            this.Update(cnt);
        }
    }
}
