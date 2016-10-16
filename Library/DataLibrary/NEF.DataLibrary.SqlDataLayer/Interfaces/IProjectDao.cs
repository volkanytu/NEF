using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEF.Library.Entities;
using NEF.Library.Entities.CrmEntities;

namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface IProjectDao
    {
        Project Get(Guid id);
        Guid Insert(Project project);
        void Update(Project project);
    }
}
