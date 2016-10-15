using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class ContactHelper
    {
        public static MsCrmResultObject GetTeamMembers(Guid teamId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT 
	                                RT.new_name AS 'Name', 
	                                RT.new_retailerId AS 'Id', 
	                                RT.OwningTeam  AS 'TeamId'
                                FROM 
	                                new_retailer RT WITH(NOLOCK)
                                WHERE
	                                RT.statecode = 0
                                AND
	                                RT.OwningTeam = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, teamId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET TOWNS |
                    List<Retailer> returnList = new List<Retailer>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Retailer retailer = new Retailer();
                        retailer.RetailerId = (Guid)dt.Rows[i]["Id"];
                        retailer.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(retailer);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bayiye ait kayıt bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject ContactSearch(string search, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.new_tcidentitynumber = '{0}'

                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.FullName LIKE '%{0}%'

                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.FullName LIKE '%{1}%'

                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.EmailAddress1 = '{0}'

                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                REPLACE(REPLACE(C.MobilePhone,'+90',''),'-','') = '{0}'

                                UNION

                                SELECT
	                                 C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Product P WITH (NOLOCK)
                                INNER JOIN
	                                Contact C WITH (NOLOCK)
	                                ON
	                                P.new_projectidName = '{0}'
	                                AND
	                                P.StateCode = 0
									AND
	                                C.ContactId = P.new_contactownerofhomeid
	                                and
	                                C.StateCode = 0

								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.Name LIKE '%{0}%' 

								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.Name LIKE '%{1}%' 

								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.new_taxnumber = '{0}'

								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.EmailAddress1 = '{0}'

								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.Telephone1 = '{0}'

                                ORDER BY 
                                    C.FullName ASC";
                #endregion

                string englishSearch = search.ToTurkishCharacter();
                DataTable dt = sda.getDataTable(string.Format(query, search, englishSearch));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | MAKE SEARCH |
                    List<Contact> returnList = new List<Contact>();
                    //returnValue.ReturnObject = dt.ToList<Contact>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Contact _contact = new Contact();
                        _contact.ContactId = (Guid)dt.Rows[i]["ContactId"];
                        _contact.FullName = dt.Rows[i]["FullName"] != DBNull.Value ? dt.Rows[i]["FullName"].ToString() : string.Empty;
                        _contact.EmailAddress1 = dt.Rows[i]["EmailAddress1"] != DBNull.Value ? dt.Rows[i]["EmailAddress1"].ToString() : string.Empty;
                        _contact.MobilePhone = dt.Rows[i]["MobilePhone"] != DBNull.Value ? dt.Rows[i]["MobilePhone"].ToString() : string.Empty;
                        _contact.IdentityNumber = dt.Rows[i]["IdentityNumber"] != DBNull.Value ? dt.Rows[i]["IdentityNumber"].ToString() : string.Empty;
                        _contact.EntityType = dt.Rows[i]["EntityType"] != DBNull.Value ? dt.Rows[i]["EntityType"].ToString() : string.Empty;

                        if (dt.Rows[i]["CustomerType"] != DBNull.Value)
                        {
                            _contact.ContactType = (ContactTypes)dt.Rows[i]["CustomerType"];
                        }

                        returnList.Add(_contact);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere uygun müşteri bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject ContactSearch(string search, string ownerId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.new_tcidentitynumber = '{0}'
                                    AND
									C.OwningUser = '{2}'
                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.FullName LIKE '%{0}%'
								AND
									C.OwningUser = '{2}'
                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.FullName LIKE '%{1}%'
								AND
									C.OwningUser = '{2}'
                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.EmailAddress1 = '{0}'
								AND
									C.OwningUser = '{2}'
                                UNION

                                SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                REPLACE(REPLACE(C.MobilePhone,'+90',''),'-','') = '{0}'
								AND
									C.OwningUser = '{2}'
                                UNION

                                SELECT
	                                 C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                    ,C.new_tcidentitynumber IdentityNumber
									,'2' EntityType
                                FROM
	                                Product P WITH (NOLOCK)
                                INNER JOIN
	                                Contact C WITH (NOLOCK)
	                                ON
	                                P.new_projectidName = '{0}'
	                                AND
	                                P.StateCode = 0
									AND
	                                C.ContactId = P.new_contactownerofhomeid
	                                and
	                                C.StateCode = 0
								AND
									C.OwningUser = '{2}'
								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.Name LIKE '%{0}%' 
								AND
									AC.OwningUser = '{2}'
								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.Name LIKE '%{1}%' 
								AND
									AC.OwningUser = '{2}'
								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.new_taxnumber = '{0}'
								AND
									AC.OwningUser = '{2}'
								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.EmailAddress1 = '{0}'
								AND
									AC.OwningUser = '{2}'
								UNION

								SELECT
	                                AC.AccountId ContactId
	                                ,AC.Name FullName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1 MobilePhone
									,'1' CustomerType
                                    ,AC.new_taxnumber IdentityNumber
									,'1' EntityType
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.Telephone1 = '{0}'
								AND
									AC.OwningUser = '{2}'
                                ORDER BY 
                                    C.FullName ASC";
                #endregion

                string englishSearch = search.ToTurkishCharacter();
                DataTable dt = sda.getDataTable(string.Format(query, search, englishSearch, ownerId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | MAKE SEARCH |
                    List<Contact> returnList = new List<Contact>();
                    //returnValue.ReturnObject = dt.ToList<Contact>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Contact _contact = new Contact();
                        _contact.ContactId = (Guid)dt.Rows[i]["ContactId"];
                        _contact.FullName = dt.Rows[i]["FullName"] != DBNull.Value ? dt.Rows[i]["FullName"].ToString() : string.Empty;
                        _contact.EmailAddress1 = dt.Rows[i]["EmailAddress1"] != DBNull.Value ? dt.Rows[i]["EmailAddress1"].ToString() : string.Empty;
                        _contact.MobilePhone = dt.Rows[i]["MobilePhone"] != DBNull.Value ? dt.Rows[i]["MobilePhone"].ToString() : string.Empty;
                        _contact.IdentityNumber = dt.Rows[i]["IdentityNumber"] != DBNull.Value ? dt.Rows[i]["IdentityNumber"].ToString() : string.Empty;
                        _contact.EntityType = dt.Rows[i]["EntityType"] != DBNull.Value ? dt.Rows[i]["EntityType"].ToString() : string.Empty;

                        if (dt.Rows[i]["CustomerType"] != DBNull.Value)
                        {
                            _contact.ContactType = (ContactTypes)dt.Rows[i]["CustomerType"];
                        }

                        returnList.Add(_contact);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere uygun müşteri bulunmamaktadır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetContactDetail(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                 C.ContactId
                                    ,C.FullName
	                                ,C.FirstName
	                                ,C.LastName
                                    ,C.new_countryid CountryId
									,C.new_countryidName CountryIdName
									,C.new_addresscityid CityId
									,C.new_addresscityidName CityIdName
									,C.new_addresstownid TownId
									,C.new_addresstownidName TownIdName
                                    ,C.new_addressdistrictid DistrictId
									,C.new_addressdistrictidName DistrictIdName
                                    ,C.new_tcidentitynumber IdentityNumber
                                    ,C.new_addressdetail AddressDetail
                                    ,C.new_address3countryid OverCountryId
									,C.new_address3countryidName OverCountryIdName
                                    ,C.new_address3cityid OverCityId
									,C.new_address3cityidName OverCityIdName
                                    ,C.new_nontcidentityaddress OverAddressDetail
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
	                                ,C.new_customertype CustomerType
	                                ,C.GenderCode
	                                ,C.FamilyStatusCode
	                                ,C.new_sourceofparticipationid ParticipationId
	                                ,C.new_sourceofparticipationidName ParticipationIdName
	                                ,C.new_subsourceofparticipationid SubParticipationId
	                                ,C.new_subsourceofparticipationidName SubParticipationIdName
                                    ,C.new_jobtitle JobTitle
                                    ,C.OwnerId
                                    ,C.OwnerIdName
                                    ,C.new_nationalityid NationalityId
                                    ,C.new_nationalityidName NationalityIdName
                                    ,C.Description
                                    ,C.new_isvipcustomer AS IsVip
                                    ,C.new_iscreditpaymentproblem AS HasCreditProblem
                                    ,C.new_isplanningpaymentproblem AS HasPaymentProblem
                                    ,C.new_isblacklist AS IsBlackList
                                    ,C.new_referencecontactid AS RefContactId
                                    ,C.new_referencecontactidName AS RefContactIdName
                                    ,C.new_customerrelationspecialistid AS CrOwnerId
                                    ,C.new_customerrelationspecialistidName AS CrOwnerIdName
                                    ,C.new_channelofawarenessid ChannelId
                                    ,C.new_channelofawarenessidName ChannelIdName
                                    ,C.new_customerrelationspecialistid SpecialistId
                                    ,C.new_customerrelationspecialistidName SpecialistIdName
                                    ,C.new_marketinggrant AS PermissionMarketing
                                    ,C.new_secondrypersonname
                                    ,C.new_secondrypersonlastname
                                    ,C.new_secondrypersonphone
                                    ,C.new_passportnumber
                                    ,C.telephone3
                                    ,C.donotemail
                                    ,C.donotpostalmail
                                    ,C.donotsendmm
                                    ,C.donotphone
                                    ,C.donotfax
                                    ,C.new_investmenttype
                                    ,C.new_guarantorid AS GuarantorId
                                    ,C.new_guarantoridName AS GuarantorIdName
                                    ,C.new_guarantorname
                                    ,C.new_guarantorphone
                                FROM
	                                Contact C WITH(NOLOCK)
								WHERE
									C.ContactId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, contactId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET DETAIL |
                    Contact _contact = new Contact();
                    _contact.ContactId = (Guid)dt.Rows[0]["ContactId"];
                    _contact.FullName = dt.Rows[0]["FullName"] != DBNull.Value ? dt.Rows[0]["FullName"].ToString() : string.Empty;

                    _contact.GuarantorName = dt.Rows[0]["new_guarantorname"] != DBNull.Value ? dt.Rows[0]["new_guarantorname"].ToString() : string.Empty;
                    _contact.GuarantorPhone = dt.Rows[0]["new_guarantorphone"] != DBNull.Value ? dt.Rows[0]["new_guarantorphone"].ToString() : string.Empty;


                    _contact.FirstName = dt.Rows[0]["FirstName"] != DBNull.Value ? dt.Rows[0]["FirstName"].ToString() : string.Empty;
                    _contact.LastName = dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString() : string.Empty;
                    _contact.IdentityNumber = dt.Rows[0]["IdentityNumber"] != DBNull.Value ? dt.Rows[0]["IdentityNumber"].ToString() : string.Empty;
                    _contact.PassportNumber = dt.Rows[0]["new_passportnumber"] != DBNull.Value ? dt.Rows[0]["new_passportnumber"].ToString() : string.Empty;
                    _contact.EmailAddress1 = dt.Rows[0]["EmailAddress1"] != DBNull.Value ? dt.Rows[0]["EmailAddress1"].ToString() : string.Empty;
                    _contact.Telephone = dt.Rows[0]["telephone3"] != DBNull.Value ? dt.Rows[0]["telephone3"].ToString() : string.Empty;
                    _contact.MobilePhone = dt.Rows[0]["MobilePhone"] != DBNull.Value ? dt.Rows[0]["MobilePhone"].ToString() : string.Empty;
                    _contact.AddressDetail = dt.Rows[0]["AddressDetail"] != DBNull.Value ? dt.Rows[0]["AddressDetail"].ToString() : string.Empty;
                    _contact.OverAddressDetail = dt.Rows[0]["OverAddressDetail"] != DBNull.Value ? dt.Rows[0]["OverAddressDetail"].ToString() : string.Empty;
                    _contact.Description = dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : string.Empty;

                    _contact.SecondryPersonName = dt.Rows[0]["new_secondrypersonname"] != DBNull.Value ? dt.Rows[0]["new_secondrypersonname"].ToString() : string.Empty;
                    _contact.SecondryPersonLastName = dt.Rows[0]["new_secondrypersonlastname"] != DBNull.Value ? dt.Rows[0]["new_secondrypersonlastname"].ToString() : string.Empty;
                    _contact.SecondryPersonPhone = dt.Rows[0]["new_secondrypersonphone"] != DBNull.Value ? dt.Rows[0]["new_secondrypersonphone"].ToString() : string.Empty;

                    _contact.IsVip = dt.Rows[0]["IsVip"] != DBNull.Value ? (bool)dt.Rows[0]["IsVip"] : false;
                    _contact.HasCreditProblem = dt.Rows[0]["HasCreditProblem"] != DBNull.Value ? (bool)dt.Rows[0]["HasCreditProblem"] : false;
                    _contact.HasPaymentProblem = dt.Rows[0]["HasPaymentProblem"] != DBNull.Value ? (bool)dt.Rows[0]["HasPaymentProblem"] : false;
                    _contact.IsBlackList = dt.Rows[0]["IsBlackList"] != DBNull.Value ? (bool)dt.Rows[0]["IsBlackList"] : false;
                    _contact.MarketingGrantValue = dt.Rows[0]["PermissionMarketing"] != DBNull.Value ? (int)dt.Rows[0]["PermissionMarketing"] : 0;
                    _contact.sendEmail = dt.Rows[0]["donotemail"] != DBNull.Value ? !((bool)dt.Rows[0]["donotemail"]) : true;
                    _contact.sendFax = dt.Rows[0]["donotfax"] != DBNull.Value ? !((bool)dt.Rows[0]["donotfax"]) : true;
                    _contact.sendSMS = dt.Rows[0]["donotsendmm"] != DBNull.Value ? !((bool)dt.Rows[0]["donotsendmm"]) : true;
                    _contact.sendMail = dt.Rows[0]["donotpostalmail"] != DBNull.Value ? !((bool)dt.Rows[0]["donotpostalmail"]) : true;
                    _contact.contactTelephone = dt.Rows[0]["donotphone"] != DBNull.Value ? !((bool)dt.Rows[0]["donotphone"]) : true;
                    _contact.Annotation = ContactHelper.GetContactAttachment(contactId, sda);

                    #region | FILL ENUMS |
                    if (dt.Rows[0]["CustomerType"] != DBNull.Value)
                    {
                        _contact.ContactType = (ContactTypes)dt.Rows[0]["CustomerType"];
                    }

                    if (dt.Rows[0]["GenderCode"] != DBNull.Value)
                    {
                        _contact.GenderCode = (GenderCodes)dt.Rows[0]["GenderCode"];
                    }

                    if (dt.Rows[0]["FamilyStatusCode"] != DBNull.Value)
                    {
                        _contact.FamilyStatusCode = (FamilyStatusCodes)dt.Rows[0]["FamilyStatusCode"];
                    }

                    if (dt.Rows[0]["JobTitle"] != DBNull.Value)
                    {
                        _contact.JobTitle = (JobTitles)dt.Rows[0]["JobTitle"];
                    }
                    if (dt.Rows[0]["new_investmenttype"] != DBNull.Value)
                    {
                        _contact.InvestmentType = (InvestmentType)dt.Rows[0]["new_investmenttype"];
                    }
                    #endregion

                    #region | FILL ENTITY REFERENCES |
                    if (dt.Rows[0]["ParticipationId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["ParticipationId"];
                        if (dt.Rows[0]["ParticipationIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["ParticipationIdName"].ToString(); }
                        er.LogicalName = "new_sourceofparticipation";

                        _contact.Participation = er;
                    }

                    if (dt.Rows[0]["SubParticipationId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["SubParticipationId"];
                        if (dt.Rows[0]["SubParticipationIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["SubParticipationIdName"].ToString(); }
                        er.LogicalName = "new_subsourceofparticipation";

                        _contact.SubParticipation = er;
                    }

                    if (dt.Rows[0]["ChannelId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["ChannelId"];
                        if (dt.Rows[0]["ChannelIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["ChannelIdName"].ToString(); }
                        er.LogicalName = "new_channelofawareness";

                        _contact.Channel = er;
                    }

                    if (dt.Rows[0]["CountryId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CountryId"];
                        if (dt.Rows[0]["CountryIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CountryIdName"].ToString(); }
                        er.LogicalName = "new_country";

                        _contact.Country = er;
                    }

                    if (dt.Rows[0]["CityId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CityId"];
                        if (dt.Rows[0]["CityIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CityIdName"].ToString(); }
                        er.LogicalName = "new_city";

                        _contact.City = er;
                    }

                    if (dt.Rows[0]["TownId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["TownId"];
                        if (dt.Rows[0]["TownIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["TownIdName"].ToString(); }
                        er.LogicalName = "new_town";

                        _contact.Town = er;
                    }

                    if (dt.Rows[0]["DistrictId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["DistrictId"];
                        if (dt.Rows[0]["DistrictIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["DistrictIdName"].ToString(); }
                        er.LogicalName = "new_district";

                        _contact.District = er;
                    }

                    if (dt.Rows[0]["OwnerId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["OwnerId"];
                        if (dt.Rows[0]["OwnerIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["OwnerIdName"].ToString(); }
                        er.LogicalName = "systemuser";

                        _contact.Owner = er;
                    }

                    if (dt.Rows[0]["NationalityId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["NationalityId"];
                        if (dt.Rows[0]["NationalityIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["NationalityIdName"].ToString(); }
                        er.LogicalName = "new_nationality";

                        _contact.Nationality = er;
                    }

                    if (dt.Rows[0]["OverCountryId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["OverCountryId"];
                        if (dt.Rows[0]["OverCountryIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["OverCountryIdName"].ToString(); }
                        er.LogicalName = "new_country";

                        _contact.OverCountry = er;
                    }

                    if (dt.Rows[0]["OverCityId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["OverCityId"];
                        if (dt.Rows[0]["OverCityIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["OverCityIdName"].ToString(); }
                        er.LogicalName = "new_city";

                        _contact.OverCity = er;
                    }

                    if (dt.Rows[0]["RefContactId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["RefContactId"];
                        if (dt.Rows[0]["RefContactIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["RefContactIdName"].ToString(); }
                        er.LogicalName = "contact";

                        _contact.RefContact = er;
                    }

                    if (dt.Rows[0]["CrOwnerId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CrOwnerId"];
                        if (dt.Rows[0]["CrOwnerIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CrOwnerIdName"].ToString(); }
                        er.LogicalName = "systemuser";

                        _contact.CrOwner = er;
                    }

                    if (dt.Rows[0]["SpecialistId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["SpecialistId"];
                        if (dt.Rows[0]["SpecialistIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["SpecialistIdName"].ToString(); }
                        er.LogicalName = "systemuser";

                        _contact.CustomerSpecialist = er;
                    }

                    if (dt.Rows[0]["GuarantorId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["GuarantorId"];
                        if (dt.Rows[0]["GuarantorIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["GuarantorIdName"].ToString(); }
                        er.LogicalName = "contact";

                        _contact.GuaContact = er;
                    }
                    #endregion

                    #region | FILL INTEREST PROJECTS |
                    MsCrmResultObject interestResult = ProductHelper.GetContactInterestedProjects(contactId, sda);
                    if (interestResult.Success && interestResult.ReturnObject != null)
                    {
                        _contact.Projects = (List<Product>)interestResult.ReturnObject;
                    }
                    #endregion

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _contact;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetContactShortDetail(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                 C.ContactId
                                    ,C.FullName
	                                ,C.FirstName
	                                ,C.LastName
                                    ,C.new_countryid CountryId
									,C.new_countryidName CountryIdName
									,C.new_addresscityid CityId
									,C.new_addresscityidName CityIdName
									,C.new_addresstownid TownId
									,C.new_addresstownidName TownIdName
                                    ,C.new_addressdistrictid DistrictId
									,C.new_addressdistrictidName DistrictIdName
                                    ,C.new_tcidentitynumber IdentityNumber
                                    ,C.new_addressdetail AddressDetail
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
	                                ,C.new_customertype CustomerType
	                                ,C.GenderCode
	                                ,C.FamilyStatusCode
	                                ,C.new_sourceofparticipationid ParticipationId
	                                ,C.new_sourceofparticipationidName ParticipationIdName
	                                ,C.new_subsourceofparticipationid SubParticipationId
	                                ,C.new_subsourceofparticipationidName SubParticipationIdName
                                FROM
	                                Contact C WITH(NOLOCK)
								WHERE
									C.ContactId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, contactId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET DETAIL |
                    Contact _contact = new Contact();
                    _contact.ContactId = (Guid)dt.Rows[0]["ContactId"];
                    _contact.FullName = dt.Rows[0]["FullName"] != DBNull.Value ? dt.Rows[0]["FullName"].ToString() : string.Empty;
                    _contact.FirstName = dt.Rows[0]["FirstName"] != DBNull.Value ? dt.Rows[0]["FirstName"].ToString() : string.Empty;
                    _contact.LastName = dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString() : string.Empty;
                    _contact.IdentityNumber = dt.Rows[0]["IdentityNumber"] != DBNull.Value ? dt.Rows[0]["IdentityNumber"].ToString() : string.Empty;
                    _contact.EmailAddress1 = dt.Rows[0]["EmailAddress1"] != DBNull.Value ? dt.Rows[0]["EmailAddress1"].ToString() : string.Empty;
                    _contact.MobilePhone = dt.Rows[0]["MobilePhone"] != DBNull.Value ? dt.Rows[0]["MobilePhone"].ToString() : string.Empty;
                    _contact.AddressDetail = dt.Rows[0]["AddressDetail"] != DBNull.Value ? dt.Rows[0]["AddressDetail"].ToString() : string.Empty;

                    #region | FILL ENUMS |
                    if (dt.Rows[0]["CustomerType"] != DBNull.Value)
                    {
                        _contact.ContactType = (ContactTypes)dt.Rows[0]["CustomerType"];
                    }

                    if (dt.Rows[0]["GenderCode"] != DBNull.Value)
                    {
                        _contact.GenderCode = (GenderCodes)dt.Rows[0]["GenderCode"];
                    }

                    if (dt.Rows[0]["FamilyStatusCode"] != DBNull.Value)
                    {
                        _contact.FamilyStatusCode = (FamilyStatusCodes)dt.Rows[0]["FamilyStatusCode"];
                    }
                    #endregion

                    #region | FILL ENTITY REFERENCES |
                    if (dt.Rows[0]["ParticipationId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["ParticipationId"];
                        if (dt.Rows[0]["ParticipationIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["ParticipationIdName"].ToString(); }
                        er.LogicalName = "new_sourceofparticipation";

                        _contact.Participation = er;
                    }

                    if (dt.Rows[0]["SubParticipationId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["SubParticipationId"];
                        if (dt.Rows[0]["SubParticipationIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["SubParticipationIdName"].ToString(); }
                        er.LogicalName = "new_subsourceofparticipation";

                        _contact.SubParticipation = er;
                    }

                    if (dt.Rows[0]["CountryId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CountryId"];
                        if (dt.Rows[0]["CountryIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CountryIdName"].ToString(); }
                        er.LogicalName = "new_country";

                        _contact.Country = er;
                    }

                    if (dt.Rows[0]["CityId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CityId"];
                        if (dt.Rows[0]["CityIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CityIdName"].ToString(); }
                        er.LogicalName = "new_city";

                        _contact.City = er;
                    }

                    if (dt.Rows[0]["TownId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["TownId"];
                        if (dt.Rows[0]["TownIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["TownIdName"].ToString(); }
                        er.LogicalName = "new_town";

                        _contact.Town = er;
                    }

                    if (dt.Rows[0]["DistrictId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["DistrictId"];
                        if (dt.Rows[0]["DistrictIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["DistrictIdName"].ToString(); }
                        er.LogicalName = "new_district";

                        _contact.District = er;
                    }
                    #endregion

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _contact;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreateOrUpdateContact(Contact _contact, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("contact");
                if (!string.IsNullOrEmpty(_contact.FirstName))
                    ent["firstname"] = _contact.FirstName;
                if (!string.IsNullOrEmpty(_contact.LastName))
                    ent["lastname"] = _contact.LastName;
                if (!string.IsNullOrEmpty(_contact.MobilePhone))
                    ent["mobilephone"] = _contact.MobilePhone;
                else
                    ent["mobilephone"] = null;

                if (!string.IsNullOrEmpty(_contact.EmailAddress1))
                {
                    if (GeneralHelper.CheckEmail(_contact.EmailAddress1))
                    {
                        ent["emailaddress1"] = _contact.EmailAddress1;
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen doğru bir email adresi giriniz!";
                        return returnValue;
                    }
                }
                if (_contact.InvestmentType.HasValue)
                {
                    ent["new_investmenttype"] = new OptionSetValue((int)_contact.InvestmentType.Value);
                }
                if (!string.IsNullOrEmpty(_contact.Telephone))
                    ent["telephone3"] = _contact.Telephone;
                else
                    ent["telephone3"] = null;

                if (!string.IsNullOrEmpty(_contact.AddressDetail))
                    ent["new_addressdetail"] = _contact.AddressDetail;
                else
                    ent["new_addressdetail"] = null;

                if (_contact.GenderCode != 0)
                    ent["gendercode"] = new OptionSetValue((int)_contact.GenderCode);

                if (!string.IsNullOrEmpty(_contact.IdentityNumber))
                    ent["new_tcidentitynumber"] = _contact.IdentityNumber;
                else
                    ent["new_tcidentitynumber"] = null;

                if (!string.IsNullOrEmpty(_contact.PassportNumber))
                    ent["new_passportnumber"] = _contact.PassportNumber;
                else
                    ent["new_passportnumber"] = null;

                if (_contact.FamilyStatusCode != 0)
                    ent["familystatuscode"] = new OptionSetValue((int)_contact.FamilyStatusCode);

                if (_contact.Participation != null && _contact.Participation.Id != Guid.Empty)
                    ent["new_sourceofparticipationid"] = _contact.Participation;
                else
                    ent["new_sourceofparticipationid"] = null;

                if (_contact.SubParticipation != null && _contact.SubParticipation.Id != Guid.Empty)
                    ent["new_subsourceofparticipationid"] = _contact.SubParticipation;
                else
                    ent["new_subsourceofparticipationid"] = null;

                if (_contact.Channel != null && _contact.Channel.Id != Guid.Empty)
                    ent["new_channelofawarenessid"] = _contact.Channel;
                else
                    ent["new_channelofawarenessid"] = null;

                if (_contact.Country != null && _contact.Country.Id != Guid.Empty)
                    ent["new_countryid"] = new EntityReference("new_country", _contact.Country.Id);
                else
                    ent["new_countryid"] = null;

                if (_contact.City != null && _contact.City.Id != Guid.Empty)
                    ent["new_addresscityid"] = new EntityReference("new_city", _contact.City.Id);
                else
                    ent["new_addresscityid"] = null;

                if (_contact.Town != null && _contact.Town.Id != Guid.Empty)
                    ent["new_addresstownid"] = new EntityReference("new_town", _contact.Town.Id);
                else
                    ent["new_addresstownid"] = null;

                if (_contact.District != null && _contact.District.Id != Guid.Empty)
                    ent["new_addressdistrictid"] = new EntityReference("new_district", _contact.District.Id);
                else
                    ent["new_addressdistrictid"] = null;

                if (_contact.JobTitle != null && _contact.JobTitle != 0)
                    ent["new_jobtitle"] = new OptionSetValue((int)_contact.JobTitle);
                else
                    ent["new_jobtitle"] = null;

                if (_contact.Owner != null && _contact.Owner.Id != Guid.Empty)
                    ent["ownerid"] = new EntityReference("systemuser", _contact.Owner.Id);
                else
                    ent["ownerid"] = null;

                if (_contact.Nationality != null && _contact.Nationality.Id != Guid.Empty)
                    ent["new_nationalityid"] = new EntityReference("new_nationality", _contact.Nationality.Id);
                else
                    ent["new_nationalityid"] = null;

                if (!string.IsNullOrEmpty(_contact.OverAddressDetail))
                    ent["new_nontcidentityaddress"] = _contact.OverAddressDetail;
                else
                    ent["new_nontcidentityaddress"] = null;

                if (!string.IsNullOrEmpty(_contact.Description))
                    ent["description"] = _contact.Description;
                else
                    ent["description"] = null;

                if (_contact.OverCountry != null && _contact.OverCountry.Id != Guid.Empty)
                    ent["new_address3countryid"] = new EntityReference("new_country", _contact.OverCountry.Id);
                else
                    ent["new_address3countryid"] = null;

                if (_contact.OverCity != null && _contact.OverCity.Id != Guid.Empty)
                    ent["new_address3cityid"] = new EntityReference("new_city", _contact.OverCity.Id);
                else
                    ent["new_address3cityid"] = null;

                if (_contact.GuaContact != null && _contact.GuaContact.Id != Guid.Empty)
                    ent["new_guarantorid"] = _contact.GuaContact;
                else
                    ent["new_guarantorid"] = null;

                if (_contact.RefContact != null && _contact.RefContact.Id != Guid.Empty)
                    ent["new_referencecontactid"] = _contact.RefContact;
                else
                    ent["new_referencecontactid"] = null;


                if (_contact.MarketingGrantValue != null && _contact.GrantUpdateUser != null && _contact.GrantUpdateUser.Id != Guid.Empty)
                {
                    ent["new_marketinggrant"] = new OptionSetValue(_contact.MarketingGrantValue);
                    ent["new_grantupdatesystemuserid"] = _contact.GrantUpdateUser; ;
                }

                if (_contact.SecondryPersonName != null)
                {
                    ent["new_secondrypersonname"] = _contact.SecondryPersonName;
                }

                if (_contact.SecondryPersonLastName != null)
                {
                    ent["new_secondrypersonlastname"] = _contact.SecondryPersonLastName;
                }

                if (_contact.SecondryPersonPhone != null)
                {
                    ent["new_secondrypersonphone"] = _contact.SecondryPersonPhone;
                }

                if (_contact.GuarantorName != null)
                {
                    ent["new_guarantorname"] = _contact.GuarantorName;
                }
                if (_contact.GuarantorPhone != null)
                {
                    ent["new_guarantorphone"] = _contact.GuarantorPhone;
                }

                ent["donotemail"] = !_contact.sendEmail;
                ent["donotfax"] = !_contact.sendFax;
                ent["donotsendmm"] = !_contact.sendSMS;
                ent["donotpostalmail"] = !_contact.sendMail;
                ent["donotphone"] = !_contact.contactTelephone;

                if (_contact.ContactId == Guid.Empty)
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Kayıt başarıyla eklendi";
                }
                else
                {
                    ent["contactid"] = _contact.ContactId;
                    service.Update(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Bilgiler başarıyla güncellendi";
                }

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult IsContactExist(Contact _contact, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT TOP 1
									S.FullName,
									S.InternalEMailAddress
                                FROM
	                                Contact C WITH (NOLOCK)
	                            INNER JOIN 
									SystemUser S WITH(NOLOCK)
								ON
									S.SystemUserId = C.OwnerId
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.new_tcidentitynumber = '{0}'
                                    OR 
                                    C.EmailAddress1 = '{1}'
                                    OR
                                    C.MobilePhone = '{2}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, _contact.IdentityNumber, _contact.EmailAddress1, _contact.MobilePhone));
                if (dt != null && dt.Rows.Count > 0)
                {
                    string fName = string.Empty;
                    string eMail = string.Empty;
                    if (dt.Rows[0]["FullName"] != DBNull.Value)
                    {
                        fName = dt.Rows[0]["FullName"].ToString();
                    }
                    if (dt.Rows[0]["InternalEMailAddress"] != DBNull.Value)
                    {
                        eMail = dt.Rows[0]["InternalEMailAddress"].ToString();
                    }
                    returnValue.Success = false;
                    returnValue.Result = "Bu müşteri kaydı NEF'e aittir. Lüften <b>" + fName + "</b> ile irtibata geçiniz. <br> " + eMail;
                }
                else
                {
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult IsAccountExist(Account _account, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT TOP 1
									s.FullName,
									s.InternalEMailAddress
                                FROM
	                                Account A WITH (NOLOCK)
	                            INNER JOIN 
									SystemUser S WITH(NOLOCK)
								ON
									S.SystemUserId = A.OwnerId
                                WHERE
	                                A.StateCode = 0
	                                AND
	                                A.Name = '{0}'
                                    OR 
                                    A.Telephone1 = '{1}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, _account.AccountName, _account.Telephone1));
                if (dt != null && dt.Rows.Count > 0)
                {
                    string fName = string.Empty;
                    string eMail = string.Empty;
                    if (dt.Rows[0]["FullName"] != DBNull.Value)
                    {
                        fName = dt.Rows[0]["FullName"].ToString();
                    }
                    if (dt.Rows[0]["InternalEMailAddress"] != DBNull.Value)
                    {
                        eMail = dt.Rows[0]["InternalEMailAddress"].ToString();
                    }
                    returnValue.Success = false;
                    returnValue.Result = "Bu müşteri kaydı NEF'e aittir. Lüften <b>" + fName + "</b> ile irtibata geçiniz. <br> " + eMail;
                }
                else
                {
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CheckDuplicateIdentity(string identityNumber, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.new_tcidentitynumber = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, identityNumber));
                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu TC Kimlik Numarasına ait kayıt bulunmaktadır!";
                }
                else
                {
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CheckDuplicateEmail(string email, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.EmailAddress1 = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, email));
                if (dt != null && dt.Rows.Count > 0)
                {
                    //Aleksi BEY 17.04.2015 Geçici kapatılmasını istedi.
                    returnValue.Success = false;
                    returnValue.Result = "Bu Email adresine ait kayıt bulunmaktadır!";
                    //returnValue.Success = true;

                }
                else
                {
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CheckDuplicatePhone(string mobilePhone, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                C.ContactId
	                                ,C.FullName
	                                ,C.EmailAddress1
	                                ,C.MobilePhone
									,C.new_customertype CustomerType
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.MobilePhone = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, mobilePhone));
                if (dt != null && dt.Rows.Count > 0)
                {
                    //Aleksi BEY 17.04.2015 Geçici kapatılmasını istedi.
                    returnValue.Success = false;
                    returnValue.Result = "Bu Cep Telefonu numarasına ait kayıt bulunmaktadır!";
                    // returnValue.Success = true;
                }
                else
                {
                    returnValue.Success = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetUserCustomers(Guid systemUserId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
									    *
								    FROM
								    (
									    SELECT
										    C.ContactId Id
										    ,C.FullName Name
										    ,C.MobilePhone Phone
										    ,C.EmailAddress1 Email
										    ,C.CreatedOn
										    ,2 ObjectTypeCode
										    ,C.new_customertype CustomerType
									    FROM
										    Contact AS C (NOLOCK)
									    WHERE
										    C.OwnerId=@ownerId
								
									    UNION

									    SELECT
										    AC.AccountId Id
										    ,AC.Name 
										    ,AC.Telephone1 Phone
										    ,AC.EmailAddress1 Email
										    ,AC.CreatedOn
										    ,1 ObjectTypeCode
										    ,1 CustomerType
									    FROM
										    Account AS AC (NOLOCK)
									    WHERE
										    AC.OwnerId=@ownerId
								    )A	
							
							        ORDER BY A.CreatedOn DESC";

                #endregion

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ownerId", systemUserId) };

                DataTable dt = sda.getDataTable(sqlQuery, parameters);

                if (dt.Rows.Count > 0)
                {
                    List<Contact> lst = new List<Contact>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Contact _c = new Contact();
                        _c.ContactId = (Guid)dt.Rows[i]["Id"];
                        _c.FullName = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;
                        _c.EmailAddress1 = dt.Rows[i]["Email"] != DBNull.Value ? dt.Rows[i]["Email"].ToString() : string.Empty;
                        _c.MobilePhone = dt.Rows[i]["Phone"] != DBNull.Value ? dt.Rows[i]["Phone"].ToString() : string.Empty;

                        if (dt.Rows[i]["CustomerType"] != DBNull.Value)
                        {
                            _c.ContactType = (ContactTypes)dt.Rows[i]["CustomerType"];
                        }

                        lst.Add(_c);

                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lst;
                    returnValue.Result = "Kullanıcı müşteri listesi çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message + "-GetUserComtacts";

            }

            return returnValue;
        }

        public static MsCrmResult ContactHasAddress(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                COUNT(0)
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.ContactId = '{0}'
	                                AND
	                                C.new_countryid IS NOT NULL
	                                AND
	                                C.new_addresscityid IS NOT NULL
	                                AND
	                                C.new_addresstownid IS NOT NULL
                                    AND
									C.new_secondrypersonname IS NOT NULL
									AND
									C.new_secondrypersonlastname IS NOT NULL
									AND
									C.new_secondrypersonphone IS NOT NULL";
                #endregion

                int result = (int)sda.ExecuteScalar(string.Format(query, contactId));

                if (result != 0)
                    returnValue.Success = true;
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşterinin adres ve ulaşılmadığında aranacak kişi alanlarını doldurunuz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult ContactHasMuhasebeAndImzaliSozlesme(Guid quoteId, Guid contactId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                Q.CustomerId
                                FROM
	                                Quote AS Q WITH (NOLOCK)
                                WHERE
	                                Q.statuscode = {0}
	                                OR
	                                Q.statuscode={1}
	                                AND
	                                Q.CustomerId='{2}'";

                query = string.Format(query, (int)QuoteStatus.Sözleşmeİmzalandı, (int)QuoteStatus.MuhasebeyeAktarıldı, contactId);
                if (quoteId != Guid.Empty)
                {
                    query += " AND Q.QuoteId !='" + quoteId + "'";
                }
                #endregion

                DataTable dt = sda.getDataTable(query);
                if (dt.Rows.Count > 0)
                    returnValue.Success = true;
                else
                    returnValue.Success = false;

            }
            catch (Exception ex)
            {
                returnValue.Success = false;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetContactGroupCodeByCharacter(char character, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    C.new_groupcodenumber Number
                                    FROM
	                                    Contact C WITH (NOLOCK)
                                    WHERE
	                                    C.new_groupcodecharacter = '{0}'
                                    ORDER BY
	                                    C.new_groupcodenumber DESC";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, character));

                returnValue.Success = true;
                returnValue.ReturnObject = dt.Rows.Count > 0 ? dt.Rows[0]["Number"] != DBNull.Value ? (int)dt.Rows[0]["Number"] : 0 : 0;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;

            }

            return returnValue;
        }



        public static MsCrmResult ContactHasAddressAndIdentity(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                COUNT(0)
                                FROM
	                                Contact C WITH (NOLOCK)
                                WHERE
	                                C.ContactId = '{0}'
	                                AND
	                                C.new_countryid IS NOT NULL
	                                AND
	                                C.new_addresscityid IS NOT NULL
	                                AND
	                                C.new_addresstownid IS NOT NULL
                                    AND
                                    ((C.new_tcidentitynumber IS NOT NULL AND C.new_nationalityidName='TC') OR C.new_nationalityidName!='TC')
                                    AND
									C.new_secondrypersonname IS NOT NULL
									AND
									C.new_secondrypersonlastname IS NOT NULL
									AND
									C.new_secondrypersonphone IS NOT NULL";
                #endregion

                int result = (int)sda.ExecuteScalar(string.Format(query, contactId));

                if (result != 0)
                    returnValue.Success = true;
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşterinin adres, T.C. No ve ulaşılmadığında aranacak kişi alanlarını doldurunuz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult CheckContactHasGroupCode(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    AC.new_groupcode Code
                                    FROM
	                                    Contact AC WITH (NOLOCK)
                                    WHERE
	                                    AC.ContactId = '{0}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, contactId));

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Code"] != DBNull.Value && !string.IsNullOrEmpty(dt.Rows[0]["Code"].ToString()))
                    {
                        returnValue.Success = true;
                        returnValue.Result = "Grup Kodu var.";
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Grup Kodu yok.";
                    }
                }
                else
                {
                    returnValue.Result = "Firma Bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static void SetCustomerNumber(Entity entity, SqlDataAccess sda)
        {
            #region SQL QUERY
            string sqlQuery = @"SELECT
                                TOP 1
                                CONVERT(int ,SUBSTRING(C.new_number,5,6)) lastNumber
                                FROM
                                CONTACT C(NOLOCK)
                                WHERE
                                CONVERT(int ,SUBSTRING(C.new_number,0,5))={1}
                                AND
                                C.new_number is not null
                                AND
                                C.ContactId !='{0}'
                                ORDER BY
                                CONVERT(int ,SUBSTRING(C.new_number,5,6))  DESC";
            sqlQuery = string.Format(sqlQuery, entity.Id, DateTime.Now.Year);
            DataTable dt = sda.getDataTable(sqlQuery);
            #endregion SQL QUERY
            string lastNumber = dt.Rows.Count > 0 ? dt.Rows[0]["lastNumber"] != DBNull.Value ? dt.Rows[0]["lastNumber"].ToString() : string.Empty : string.Empty;
            if (lastNumber != string.Empty)
            {
                entity["new_number"] = DateTime.Now.Year.ToString() + (Convert.ToInt32(lastNumber) + 1).ToString().PadLeft(6, '0');
            }
            else
            {
                entity["new_number"] = DateTime.Now.Year.ToString() + "000001";
            }
        }

        public static EntityReference GetContactAttachment(Guid contactId, SqlDataAccess sda)
        {
            EntityReference returnValue = new EntityReference();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT TOP 1
                                a.AnnotationId
                                ,a.FileName
                                FROM
                                Annotation AS a (NOLOCK)
                                WHERE
                                a.ObjectId='{0}' AND filename like '%CRM_NFZ%' ORDER BY  a.CreatedOn DESC";
            #endregion

            DataTable dt = sda.getDataTable(string.Format(sqlQuery, contactId.ToString()));

            if (dt.Rows.Count > 0)
            {
                returnValue.Id = (Guid)dt.Rows[0]["AnnotationId"];
                returnValue.Name = dt.Rows[0]["FileName"].ToString();
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static MsCrmResult DeleteAnnotionByContactId(string contactId, SqlDataAccess sda)
        {
            MsCrmResult retVal = new MsCrmResult();

            string sqlQuery = @"SELECT TOP 1
                                a.AnnotationId
                                ,a.FileName
                                FROM
                                Annotation AS a (NOLOCK)
                                WHERE
                                a.ObjectId='{0}' AND filename like '%CRM_NFZ%' ORDER BY  a.CreatedOn DESC";


            DataTable dt = sda.getDataTable(string.Format(sqlQuery, contactId));

            if (dt.Rows.Count > 0)
            {
                Entity ent = new Entity("annotation");
                MSCRM.AdminOrgService.Delete("annotation", (Guid)dt.Rows[0]["AnnotationId"]);
                retVal.Success = true;
                retVal.Result = "Nüfus cüzdanı Başarı ile silindi.";
            }
            else
            {
                retVal.Success = false;
                retVal.Result = "Nüfus cüzdanı bulunamadı.";
            }
            return retVal;
        }
        public static MsCrmResult DeleteAnnotionByRentalId(string contactId, SqlDataAccess sda)
        {
            MsCrmResult retVal = new MsCrmResult();

            string sqlQuery = @"SELECT TOP 1
                                a.AnnotationId
                                ,a.FileName
                                FROM
                                Annotation AS a (NOLOCK)
                                WHERE
                                a.ObjectId='{0}' AND filename like '%TT_%' ORDER BY  a.CreatedOn DESC";


            DataTable dt = sda.getDataTable(string.Format(sqlQuery, contactId));

            if (dt.Rows.Count > 0)
            {
                Entity ent = new Entity("annotation");
                MSCRM.AdminOrgService.Delete("annotation", (Guid)dt.Rows[0]["AnnotationId"]);
                retVal.Success = true;
                retVal.Result = "Belge Başarı ile silindi.";
            }
            else
            {
                retVal.Success = false;
                retVal.Result = "Belge bulunamadı.";
            }
            return retVal;
        }
    }
}
