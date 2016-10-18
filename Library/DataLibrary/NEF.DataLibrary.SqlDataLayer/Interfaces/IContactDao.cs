using NEF.Library.Entities.CrmEntities;
using System;
namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface IContactDao
    {
        Contact Get(Guid id);
        Guid Insert(Contact contact);
        void Update(Contact contact);
    }
}
