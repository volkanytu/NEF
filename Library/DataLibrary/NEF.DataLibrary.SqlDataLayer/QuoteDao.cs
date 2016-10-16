using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Constants.SqlQueries;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer
{
    public class QuoteDao : IQuoteDao
    {
        private IMsCrmAccess _msCrmAccess;
        private ISqlAccess _sqlAccess;

        public QuoteDao(IMsCrmAccess msCrmAccess, ISqlAccess sqlAccess)
        {
            _msCrmAccess = msCrmAccess;
            _sqlAccess = sqlAccess;
        }

        public Guid Insert(Quote quote)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = quote.ToCrmEntity();

            Guid id = service.Create(entity);

            return id;
        }

        public void Update(Quote quote)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = quote.ToCrmEntity();

            service.Update(entity);
        }

        public Quote Get(Guid id)
        {
            SqlParameter[] parameters = { new SqlParameter("@id", id) };

            DataTable dt = _sqlAccess.GetDataTable(QuoteQueries.ANY_QUERY, parameters);

            return dt.ToList<Quote>().FirstOrDefault();
        }
    }
}
