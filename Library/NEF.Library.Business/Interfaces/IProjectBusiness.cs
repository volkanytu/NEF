using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities;
using NEF.Library.Entities.CrmEntities;

namespace NEF.Library.Business.Interfaces
{
    public interface IProjectBusiness
    {
        Project Get(Guid id);
        Guid Insert(Project project);
        void Update(Project project);
    }
}
