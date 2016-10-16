using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Business
{
    public class QuoteBusiness : IQuoteBusiness
    {
        private IQuoteDao _quoteDao;

        public QuoteBusiness(IQuoteDao quoteDao)
        {
            _quoteDao = quoteDao;
        }

        public Guid Insert(Quote quote)
        {
            return _quoteDao.Insert(quote);
        }

        public void Update(Quote quote)
        {
            _quoteDao.Update(quote);
        }

        public Quote Get(Guid id)
        {
            return _quoteDao.Get(id);
        }
    }
}
