using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Configuration;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NEF.DataLibrary.SqlDataLayer.Interfaces;
using NEF.Library.Utility;
using System;
using System.Net;
using System.ServiceModel.Description;
using System.Threading;

namespace SAHIBINDEN.DataLibrary.SqlDataLayer
{
    public class MsCrmAccess : IMsCrmAccess
    {
        private readonly bool _isAdmin = true;
        private readonly string _behalfOfUserId = string.Empty;
        private IOrganizationService _crmService = null;
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public IOrganizationService CrmService
        {
            get
            {
                return _crmService;
            }
            set
            {
                _crmService = value;
            }
        }
        public MsCrmAccess(bool isAdmin)
        {
            _isAdmin = isAdmin;
        }

        public MsCrmAccess(string behalfOfUserId)
        {
            _isAdmin = true;
            _behalfOfUserId = behalfOfUserId;
        }

        public IOrganizationService GetCrmService()
        {
            //IOrganizationService crmService = null;
            cacheLock.EnterReadLock();

            try
            {
                if (_crmService == null)
                {
                    if (!string.IsNullOrEmpty(_behalfOfUserId))
                    {
                        CrmService = GetCrmServiceBehalfOfUser(_behalfOfUserId);
                    }
                    else if (_isAdmin)
                    {
                        CrmService = GetAdminCrmService();
                    }
                    else
                    {
                        CrmService = GetCurrentUserCrmService();
                    }
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            return _crmService;
        }

        private object servicelock = new object();

        private IOrganizationService GetAdminCrmService()
        {
            string connectionUrl = string.Format("Url={0}{1}; Domain={2}; Username={3}; Password={4};", Globals.CrmUrl, Globals.OrganizationName, Globals.DomainName, Globals.AdminUserName, Globals.AdminPassword);

            CrmConnection connection = CrmConnection.Parse(connectionUrl);

            connection.ServiceConfigurationInstanceMode = ServiceConfigurationInstanceMode.PerRequest;
            connection.Timeout = new TimeSpan(2, 0, 0);
            connection.UserTokenExpiryWindow = new TimeSpan(0, 1, 0);

            OrganizationService service = new OrganizationService(connection);

            return service;
        }

        private IOrganizationService GetCrmServiceBehalfOfUser(string callerId)
        {
            ClientCredentials credential = new ClientCredentials();
            credential.Windows.ClientCredential = new NetworkCredential(Globals.AdminUserName, Globals.AdminPassword, Globals.DomainName);

            OrganizationServiceProxy orgServiceProxy = new OrganizationServiceProxy(new Uri(Globals.OrganizationServiceUrl), null, credential, null);

            orgServiceProxy.CallerId = new Guid(callerId);

            return orgServiceProxy;
        }

        private IOrganizationService GetCurrentUserCrmService()
        {
            ClientCredentials credential = new ClientCredentials();
            credential.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

            OrganizationServiceProxy orgServiceProxy = new OrganizationServiceProxy(new Uri(Globals.OrganizationServiceUrl), null, credential, null);

            return orgServiceProxy;
        }

        private TProxy GetProxy<TService, TProxy>(
            IServiceManagement<TService> serviceManagement,
            AuthenticationCredentials authCredentials)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            Type classType = typeof(TProxy);

            if (serviceManagement.AuthenticationType !=
                AuthenticationProviderType.ActiveDirectory)
            {
                AuthenticationCredentials tokenCredentials =
                    serviceManagement.Authenticate(authCredentials);
                // Obtain discovery/organization service proxy for Federated, LiveId and OnlineFederated environments. 
                // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and SecurityTokenResponse.
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }

            // Obtain discovery/organization service proxy for ActiveDirectory environment.
            // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and ClientCredentials.
            return (TProxy)classType
                .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }

    }
}
