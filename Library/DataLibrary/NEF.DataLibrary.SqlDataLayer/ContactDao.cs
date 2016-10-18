using Microsoft.Xrm.Sdk;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Constants.SqlQueries;
using NEF.Library.Entities.CrmEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.DataLibrary.SqlDataLayer
{
    public class ContactDao : IContactDao
    {
        private IMsCrmAccess _msCrmAccess;
        private ISqlAccess _sqlAccess;

        public ContactDao(IMsCrmAccess msCrmAccess, ISqlAccess sqlAccess)
        {
            _msCrmAccess = msCrmAccess;
            _sqlAccess = sqlAccess;
        }

        public Guid Insert(Contact contact)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = contact.ToCrmEntity();

            Guid id = service.Create(entity);

            return id;
        }

        public void Update(Contact contact)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = contact.ToCrmEntity();

            service.Update(entity);
        }

        public Contact Get(Guid id)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@id", id) };

            DataTable dt = _sqlAccess.GetDataTable(ContactQueries.GET_CONTACT, parameters);

            return dt.ToList<Contact>().FirstOrDefault();
        }
    }
}
