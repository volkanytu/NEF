using NEF.Library.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.LoyaltyPointManager
{
    public class LoyaltySegmentCalculate : ILoyaltySegmentCalculate
    {
        private ILoyaltyPointBusiness _loyaltyBusiness;
        private ILoyaltySegmentConfigBusiness _loyaltyConfigBusiness;
        private IContactBusiness _contactBusiness;

        public LoyaltySegmentCalculate(ILoyaltyPointBusiness loyaltyBusiness, ILoyaltySegmentConfigBusiness loyaltyConfigBusiness
            , IContactBusiness contactBusiness)
        {
            _loyaltyBusiness = loyaltyBusiness;
            _loyaltyConfigBusiness = loyaltyConfigBusiness;
            _contactBusiness = contactBusiness;
        }

        public void DoWork()
        {
            var wonPointsOfContact = _loyaltyBusiness.GetWonPointsOfContacts();
            var loyaltySegmentConfigs = _loyaltyConfigBusiness.GetList();


            if (wonPointsOfContact == null || loyaltySegmentConfigs == null)
            {
                return;
            }

            _loyaltyBusiness.SetContactLoyaltySegment(wonPointsOfContact, loyaltySegmentConfigs);

            foreach (var segment in wonPointsOfContact)
            {
                _contactBusiness.SetLoyaltySegment(segment.ContactId, segment.Segment);
            }
        }
    }
}
