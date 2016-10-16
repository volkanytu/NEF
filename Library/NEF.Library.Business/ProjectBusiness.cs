using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Business.Interfaces;
using NEF.Library.Entities;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Business
{
    public class ProjectBusiness : IProjectBusiness
    {
         private IProjectDao _projectDao;

         public ProjectBusiness(IProjectDao projectDao)
        {
            _projectDao = projectDao;
        }

        public Guid Insert(Project project)
        {
            return _projectDao.Insert(project);
        }

        public void Update(Project project)
        {
            _projectDao.Update(project);
        }

        public Project Get(Guid id)
        {
            return _projectDao.Get(id);
        }
    }
}
