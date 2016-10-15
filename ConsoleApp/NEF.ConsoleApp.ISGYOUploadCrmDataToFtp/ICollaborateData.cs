using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.ISGYOUploadCrmDataToFtp
{
    public interface ICollaborateData
    {
        MsCrmResult Process(SqlDataAccess sda);
    }
}
