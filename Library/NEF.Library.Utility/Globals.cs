using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEF.Library.Utility
{
    public class Globals
    {
        public static string BirthdayCampaignId
        {
            get { return RegistryHelper.Get.Value("BirthdayCampaignId"); }
        }

        public static string EuromsgFromAddressLive
        {
            get { return RegistryHelper.Get.Value("EuromsgFromAddressLive"); }
        }

        public static string EuromsgFromNameLive
        {
            get { return RegistryHelper.Get.Value("EuromsgFromNameLive"); }
        }
        public static string EuromsgReplyAddressLive
        {
            get { return RegistryHelper.Get.Value("EuromsgReplyAddressLive"); }
        }

        public static string FTPAddressEmail
        {
            get { return RegistryHelper.Get.Value("FTPAddressEmail"); }
        }

        public static string FTPAddressFilterMemberEmail
        {
            get { return RegistryHelper.Get.Value("FTPAddressFilterMemberEmail"); }
        }

        public static string FTPAddressSms
        {
            get { return RegistryHelper.Get.Value("FTPAddressSms"); }
        }

        public static string ServerIp
        {
            get { return RegistryHelper.Get.Value("ServerIp"); }
        }

        public static string FTPUser
        {
            get { return RegistryHelper.Get.Value("FTPUser"); }
        }

        public static string FTPPassword
        {
            get { return RegistryHelper.Get.Value("FTPPassword"); }
        }

        public static string EuroMessageUserNameLive
        {
            get { return RegistryHelper.Get.Value("EuroMessageUserNameLive"); }
        }

        public static string EuroMessagePasswordLive
        {
            get { return RegistryHelper.Get.Value("EuroMessagePasswordLive"); }
        }

        public static string SftpUsernameLive
        {
            get { return RegistryHelper.Get.Value("SftpUsernameLive"); }
        }

        public static string SftpPasswordLive
        {
            get { return RegistryHelper.Get.Value("SftpPasswordLive"); }
        }

        public static string sftpUrl
        {
            get { return RegistryHelper.Get.Value(""); }
        }

        public static string EuroMessageCustomDBString
        {
            get { return RegistryHelper.Get.Value("EuroMessageCustomDBString"); }
        }

        public static string DomainName
        {
            get { return RegistryHelper.Get.Value("DomainName"); }
        }

        public static string AdminName
        {
            get { return RegistryHelper.Get.Value("AdminName"); }
        }

        public static string AdminUserName
        {
            get { return RegistryHelper.Get.Value("AdminUserName"); }
        }

        public static string AdminPassword
        {
            get { return RegistryHelper.Get.Value("AdminPassword"); }
        }

        public static Guid AdministratorId
        {
            get { return new Guid(RegistryHelper.Get.Value("AdminId")); }
        }

        public static string CrmUrl
        {
            get { return RegistryHelper.Get.Value("CrmUrl"); }
        }

        public static string OrganizationName
        {
            get { return RegistryHelper.Get.Value("OrganizationName"); }
        }

        public static string OrganizationServiceUrl
        {
            get { return RegistryHelper.Get.Value("OrganizationServiceUrl"); }
        }

        public static string DatabaseName
        {
            get { return RegistryHelper.Get.Value("DatabaseName"); }
        }

        public static string ConnectionString
        {
            get { return RegistryHelper.Get.Value("ConnectionString"); }
        }

        public static string PortalUrl
        {
            get { return RegistryHelper.Get.Value("PortalUrl"); }
        }

        public static Guid DefaultUoMId
        {
            get { return new Guid(RegistryHelper.Get.Value("DefaultUoMId")); }
        }

        public static Guid DefaultUoMScheduleId
        {
            get { return new Guid(RegistryHelper.Get.Value("DefaultUoMScheduleId")); }
        }

        public static Guid CurrencyIdTL
        {
            get { return new Guid(RegistryHelper.Get.Value("CurrencyIdTL")); }
        }

        public static Guid CurrencyIdUSD
        {
            get { return new Guid(RegistryHelper.Get.Value("CurrencyIdUSD")); }
        }

        public static Guid CurrencyIdEUR
        {
            get { return new Guid(RegistryHelper.Get.Value("CurrencyIdEUR")); }
        }

        public static int HouseImportObjectTypeCode
        {
            get { return Convert.ToInt32(RegistryHelper.Get.Value("HouseImportObjectTypeCode")); }
        }

        public static string ConnectionString11
        {
            get { return RegistryHelper.Get.Value("ConnectionString11"); }
        }
        public static string WebFormParticipationId
        {
            get { return RegistryHelper.Get.Value("WebFormParticipationId"); }
        }

        public static string MobilFormParticipationId
        {
            get { return RegistryHelper.Get.Value("MobilFormParticipationId"); }
        }
        public static string ConnectionStringLogo
        {
            get { return RegistryHelper.Get.Value("ConnectionStringLogo"); }
        }
        public static string WebFormSytemUserId
        {
            get { return RegistryHelper.Get.Value("WebFormSytemUserId"); }
        }

        public static string PortalAttachmentFolder
        {
            get { return RegistryHelper.Get.Value("PortalAttachmentFolder"); }
        }

        public static string ConnectionStringLogo2
        {
            get { return RegistryHelper.Get.Value("ConnectionStringLogo2"); }
        }
        public static string HouseCrmLink
        {
            get { return RegistryHelper.Get.Value("HouseCrmLink"); }
        }
        public static Guid TopkapiProjectId
        {
            get { return new Guid(RegistryHelper.Get.Value("TopkapiProjectId")); }
        }
        public static Guid NEFSalesCollaboraterateId
        {
            get { return new Guid(RegistryHelper.Get.Value("NEFSalesCollaboraterateId")); }
        }

        public static string YellowExcelProcessTempFolder
        {
            get { return RegistryHelper.Get.Value("YellowExcelProcessTempFolder"); }
        }

        public static Guid AlternatifBusinessUnitId
        {
            get { return new Guid(RegistryHelper.Get.Value("AlternatifBusinessUnitId")); }
        }
        public static Guid AlternatifDirectorSystemUserId
        {
            get { return new Guid(RegistryHelper.Get.Value("AlternatifDirectorSystemUserId")); }
        }

        public static Guid IsGayrimenkulYatirimOrtakligiId
        {
            get { return new Guid(RegistryHelper.Get.Value("IsGayrimenkulYatirimOrtakligi")); }
        }

        public static string ConnectionStringRequests
        {
            get { return RegistryHelper.Get.Value("ConnectionStringRequest"); }
        }

        public static string IsSendMailActive
        {
            get { return RegistryHelper.Get.Value("IsSendMailActive"); }
        }
    }
}
