using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;
using NEF.Library.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities;

namespace NEF.Library.Business
{
    public class LoyaltyPointBusiness : ILoyaltyPointBusiness
    {
        private ILoyaltyPointDao _loyaltyPointDao;

        public LoyaltyPointBusiness(ILoyaltyPointDao loyaltyPointDao)
        {
            _loyaltyPointDao = loyaltyPointDao;
        }

        public Guid Insert(LoyaltyPoint loyaltyPoint)
        {
            return _loyaltyPointDao.Insert(loyaltyPoint);
        }

        public void Update(LoyaltyPoint loyaltyPoint)
        {
            _loyaltyPointDao.Update(loyaltyPoint);
        }

        public LoyaltyPoint Get(Guid id)
        {
            return _loyaltyPointDao.Get(id);
        }

        public List<LoyaltySegment> GetWonPointsOfContacts()
        {
            return _loyaltyPointDao.GetWonPointsOfContacts();
        }

        public void SetContactLoyaltySegment(List<LoyaltySegment> loyaltySegmentList, List<LoyaltySegmentConfig> loyaltySegmentConfigList)
        {
            foreach (LoyaltySegment loyaltySegment in loyaltySegmentList)
            {
                loyaltySegment.Segment =
                    loyaltySegmentConfigList
                        .FirstOrDefault(p => p.MinValue < loyaltySegment.TotalPoint && p.MaxValue > loyaltySegment.TotalPoint)
                        .LoyaltySegment.ToEnum<Contact.LoyaltySegmentCode>();
            }
        }

        public List<LoyaltyPoint> GetAllPointsOfContact(Guid contactId)
        {
            return _loyaltyPointDao.GetAllPointsOfContact(contactId);
        }

        public decimal GetBalanceOfContact(Guid contactId)
        {
            var pointList = this.GetAllPointsOfContact(contactId);

            return pointList.Where(p => p.Amount != null
                             && p.State.ToEnum<LoyaltyPoint.StateCode>() == LoyaltyPoint.StateCode.ACTIVE
                             && p.Status.ToEnum<LoyaltyPoint.StatusCode>() == LoyaltyPoint.StatusCode.CONFIRMED)
                             .Sum(p => p.ReelValue);
        }

        public void TransferPoints(Guid sourceContactId, Guid targetContactId, EntityReferenceWrapper erPointTransfer = null)
        {
            decimal balanceOfSource = this.GetBalanceOfContact(sourceContactId);

            if (balanceOfSource > 0)
            {
                LoyaltyPoint lp = new LoyaltyPoint
                {
                    ContactId = sourceContactId.ToEntityReferenceWrapper<Contact>(),
                    Amount = balanceOfSource,
                    Description = "PUAN TRANSFER",
                    ExpireDate = DateTime.Now.AddMonths(3),
                    Name = "PUAN TRANSFER(AZALTIM)",
                    PointType = LoyaltyPoint.PointTypeCode.LESSENING.ToOptionSetValueWrapper(),
                    UsageType = LoyaltyPoint.UsageTypeCode.CARD.ToOptionSetValueWrapper(),
                    PointTransferId = erPointTransfer
                };


                this.Insert(lp);

                lp.Name = "PUAN TRANSFER(ARTTIRIM)";
                lp.ContactId = targetContactId.ToEntityReferenceWrapper<Contact>();
                lp.PointType = LoyaltyPoint.PointTypeCode.ADDING.ToOptionSetValueWrapper();

                this.Insert(lp);
            }
        }

        public void ConfirmPointUsage(Guid loyaltyPointId)
        {
            LoyaltyPoint lp = new LoyaltyPoint
            {
                Id = loyaltyPointId,
                Status = LoyaltyPoint.StatusCode.CONFIRMED.ToOptionSetValueWrapper()
            };

            this.Update(lp);
        }

        public void RefusePointUsage(Guid loyaltyPointId)
        {
            LoyaltyPoint lp = new LoyaltyPoint
            {
                Id = loyaltyPointId,
                Status = LoyaltyPoint.StatusCode.REFUSED.ToOptionSetValueWrapper()
            };

            this.Update(lp);
        }

        public LoyaltyPointSummary GetContactPointSummary(Guid contactId)
        {
            var pointList = this.GetAllPointsOfContact(contactId);

            if (pointList == null || pointList.Count == 0)
            {
                return null;
            }

            LoyaltyPointSummary summary = new LoyaltyPointSummary();
            summary.ContactId = contactId.ToEntityReferenceWrapper<Contact>();

            summary.Balance = pointList.Where(p => p.Amount != null
                                         && p.State.ToEnum<LoyaltyPoint.StateCode>() == LoyaltyPoint.StateCode.ACTIVE
                                         && p.Status.ToEnum<LoyaltyPoint.StatusCode>() == LoyaltyPoint.StatusCode.CONFIRMED)
                                         .Sum(p => p.ReelValue);

            summary.TotalWonPoint = pointList.Where(p => p.Amount != null
                             && p.State.ToEnum<LoyaltyPoint.StateCode>() == LoyaltyPoint.StateCode.ACTIVE
                             && p.Status.ToEnum<LoyaltyPoint.StatusCode>() == LoyaltyPoint.StatusCode.CONFIRMED
                             && p.PointType.ToEnum<LoyaltyPoint.PointTypeCode>() == LoyaltyPoint.PointTypeCode.EARNING)
                             .Sum(p => p.ReelValue);

            summary.CardAmount = pointList.Where(p => p.Amount != null
                             && p.State.ToEnum<LoyaltyPoint.StateCode>() == LoyaltyPoint.StateCode.ACTIVE
                             && p.Status.ToEnum<LoyaltyPoint.StatusCode>() == LoyaltyPoint.StatusCode.CONFIRMED
                             && p.PointType.ToEnum<LoyaltyPoint.UsageTypeCode>() == LoyaltyPoint.UsageTypeCode.CARD)
                             .Sum(p => p.ReelValue);

            summary.CashAmount = pointList.Where(p => p.Amount != null
                             && p.State.ToEnum<LoyaltyPoint.StateCode>() == LoyaltyPoint.StateCode.ACTIVE
                             && p.Status.ToEnum<LoyaltyPoint.StatusCode>() == LoyaltyPoint.StatusCode.CONFIRMED
                             && p.PointType.ToEnum<LoyaltyPoint.UsageTypeCode>() == LoyaltyPoint.UsageTypeCode.CASH)
                             .Sum(p => p.ReelValue);

            return summary;
        }
    }
}
