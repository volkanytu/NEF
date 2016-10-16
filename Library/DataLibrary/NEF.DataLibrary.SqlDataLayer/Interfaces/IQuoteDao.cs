using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface IQuoteDao
    {
        Quote Get(Guid id);
        Guid Insert(Quote quote);
        void Update(Quote quote);
    }
}
