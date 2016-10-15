using Microsoft.Xrm.Sdk;
using System;
namespace NEF.DataLibrary.SqlDataLayer.Interfaces
{
    public interface IMsCrmAccess
    {
        IOrganizationService GetCrmService();
        IOrganizationService CrmService { get; set; }
    }
}
