using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Constants.SqlQueries;
using NEF.Library.Entities;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer
{
    public class ProjectDao : IProjectDao
    {
        private IMsCrmAccess _msCrmAccess;
        private ISqlAccess _sqlAccess;

        public ProjectDao(IMsCrmAccess msCrmAccess, ISqlAccess sqlAccess)
        {
            _msCrmAccess = msCrmAccess;
            _sqlAccess = sqlAccess;
        }

        public Guid Insert(Project project)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = project.ToCrmEntity();

            Guid id = service.Create(entity);

            return id;
        }

        public void Update(Project project)
        {
            IOrganizationService service = _msCrmAccess.GetCrmService();

            Entity entity = project.ToCrmEntity();

            service.Update(entity);
        }

        public Project Get(Guid id)
        {
            SqlParameter[] parameters = { new SqlParameter("@id", id) };

            DataTable dt = _sqlAccess.GetDataTable(ProjectQueries.ANY_QUERY, parameters);

            return dt.ToList<Project>().FirstOrDefault();
        }
    }
}
