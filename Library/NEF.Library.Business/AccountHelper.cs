using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class AccountHelper
    {
        public static MsCrmResult CreateOrUpdateAccount(Account _account, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity ent = new Entity("account");
                if (!string.IsNullOrEmpty(_account.AccountName))
                    ent["name"] = _account.AccountName;
                if (!string.IsNullOrEmpty(_account.Telephone1))
                    ent["telephone1"] = _account.Telephone1;
                else
                    ent["telephone1"] = null;

                if (!string.IsNullOrEmpty(_account.EmailAddress1))
                    ent["emailaddress1"] = _account.EmailAddress1;
                else
                    ent["emailaddress1"] = null;

                if (!string.IsNullOrEmpty(_account.TaxNumber))
                    ent["new_taxnumber"] = _account.TaxNumber;
                else
                    ent["new_taxnumber"] = null;

                if (!string.IsNullOrEmpty(_account.TaxNumber))
                    ent["new_taxnumber"] = _account.TaxNumber;
                else
                    ent["new_taxnumber"] = null;

                if (_account.Contact != null && _account.Contact.Id != Guid.Empty)
                    ent["primarycontactid"] = _account.Contact;
                else
                    ent["primarycontactid"] = null;

                if (_account.TaxOffice != null && _account.TaxOffice.Id != Guid.Empty)
                    ent["new_taxofficeid"] = _account.TaxOffice;
                else
                    ent["new_taxofficeid"] = null;

                if (_account.Owner != null)
                    ent["ownerid"] = _account.Owner;
                else
                    ent["ownerid"] = null;

                if (_account.Country != null && _account.Country.Id != Guid.Empty)
                    ent["new_addresscountryid"] = new EntityReference("new_country", _account.Country.Id);
                else
                    ent["new_addresscountryid"] = null;

                if (_account.City != null && _account.City.Id != Guid.Empty)
                    ent["new_addresscityid"] = new EntityReference("new_city", _account.City.Id);
                else
                    ent["new_addresscityid"] = null;

                if (_account.Town != null && _account.Town.Id != Guid.Empty)
                    ent["new_addresstownid"] = new EntityReference("new_town", _account.Town.Id);
                else
                    ent["new_addresstownid"] = null;

                if (_account.District != null && _account.District.Id != Guid.Empty)
                    ent["new_addressdistrictid"] = new EntityReference("new_district", _account.District.Id);
                else
                    ent["new_addressdistrictid"] = null;

                if (!string.IsNullOrEmpty(_account.AddressDetail))
                    ent["new_addressdetail"] = _account.AddressDetail;
                else
                    ent["new_addressdetail"] = null;

                if (!string.IsNullOrEmpty(_account.Description))
                    ent["description"] = _account.Description;
                else
                    ent["description"] = null;

                if (_account.AccountId == Guid.Empty)
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Kayıt başarıyla eklendi";
                }
                else
                {
                    ent["accountid"] = _account.AccountId;
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

        public static MsCrmResult CheckDuplicateTaxNumber(string taxNumber, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                AC.AccountId 
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.new_taxnumber = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, taxNumber));
                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu Vergi Numarasına ait kayıt bulunmaktadır!";
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

        public static MsCrmResult CheckDuplicateName(string name, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                AC.AccountId 
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.StateCode = 0
	                                AND
	                                AC.name = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, name));
                if (dt != null && dt.Rows.Count > 0)
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu Ünvana ait kayıt bulunmaktadır!";
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

        public static MsCrmResultObject GetAccountDetail(Guid accountId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                AC.AccountId
	                                ,AC.Name
	                                ,AC.new_taxnumber TaxNumber
	                                ,AC.new_addresscountryid CountryId
	                                ,AC.new_addresscountryidName CountryIdName
	                                ,AC.new_addresscityid CityId
	                                ,AC.new_addresscityidName CityIdName
	                                ,AC.new_addresstownid TownId
	                                ,AC.new_addresstownidName TownIdName
                                    ,AC.new_addressdistrictid DistrictId
	                                ,AC.new_addressdistrictidName DistrictIdName
                                    ,AC.new_addressdetail AddressDetail
	                                ,AC.new_taxofficeid TaxOfficeId
	                                ,AC.new_taxofficeidName TaxOfficeIdName
                                    ,AC.OwnerId
                                    ,AC.OwnerIdName
	                                ,AC.EmailAddress1
	                                ,AC.Telephone1
                                    ,AC.PrimaryContactId
                                    ,AC.PrimaryContactIdName
                                    ,AC.Description
                                    ,AC.new_customerrelationspecialistid AS CrOwnerId
                                    ,AC.new_customerrelationspecialistidName AS CrOwnerIdName
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.AccountId = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, accountId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    Account _account = new Account();
                    _account.AccountId = (Guid)dt.Rows[0]["AccountId"];
                    _account.AccountName = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;
                    _account.EmailAddress1 = dt.Rows[0]["EmailAddress1"] != DBNull.Value ? dt.Rows[0]["EmailAddress1"].ToString() : string.Empty;
                    _account.Telephone1 = dt.Rows[0]["Telephone1"] != DBNull.Value ? dt.Rows[0]["Telephone1"].ToString() : string.Empty;
                    _account.TaxNumber = dt.Rows[0]["TaxNumber"] != DBNull.Value ? dt.Rows[0]["TaxNumber"].ToString() : string.Empty;
                    _account.AddressDetail = dt.Rows[0]["AddressDetail"] != DBNull.Value ? dt.Rows[0]["AddressDetail"].ToString() : string.Empty;
                    _account.Description = dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : string.Empty;

                    #region | FILL ENTITY REFERENCES |

                    if (dt.Rows[0]["PrimaryContactId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["PrimaryContactId"];
                        if (dt.Rows[0]["PrimaryContactIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["PrimaryContactIdName"].ToString(); }
                        er.LogicalName = "contact";

                        _account.Contact = er;
                    }

                    if (dt.Rows[0]["CountryId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CountryId"];
                        if (dt.Rows[0]["CountryIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CountryIdName"].ToString(); }
                        er.LogicalName = "new_country";

                        _account.Country = er;
                    }

                    if (dt.Rows[0]["CityId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CityId"];
                        if (dt.Rows[0]["CityIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CityIdName"].ToString(); }
                        er.LogicalName = "new_city";

                        _account.City = er;
                    }

                    if (dt.Rows[0]["TownId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["TownId"];
                        if (dt.Rows[0]["TownIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["TownIdName"].ToString(); }
                        er.LogicalName = "new_town";

                        _account.Town = er;
                    }

                    if (dt.Rows[0]["DistrictId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["DistrictId"];
                        if (dt.Rows[0]["DistrictIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["DistrictIdName"].ToString(); }
                        er.LogicalName = "new_district";

                        _account.District = er;
                    }

                    if (dt.Rows[0]["OwnerId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["OwnerId"];
                        if (dt.Rows[0]["OwnerIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["OwnerIdName"].ToString(); }
                        er.LogicalName = "systemuser";

                        _account.Owner = er;
                    }

                    if (dt.Rows[0]["TaxOfficeId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["TaxOfficeId"];
                        if (dt.Rows[0]["TaxOfficeIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["TaxOfficeIdName"].ToString(); }
                        er.LogicalName = "new_taxoffice";

                        _account.TaxOffice = er;
                    }

                    if (dt.Rows[0]["CrOwnerId"] != DBNull.Value)
                    {
                        EntityReference er = new EntityReference();
                        er.Id = (Guid)dt.Rows[0]["CrOwnerId"];
                        if (dt.Rows[0]["CrOwnerIdName"] != DBNull.Value) { er.Name = dt.Rows[0]["CrOwnerIdName"].ToString(); }
                        er.LogicalName = "systemuser";

                        _account.CrOwner = er;
                    }

                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = _account;
                }
                else
                {
                    returnValue.Result = "Firma bilgileri alınamadı!";
                    returnValue.Success = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult AccountHasAddress(Guid accountId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                COUNT(0)
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.AccountId = '{0}'
	                                AND
	                                AC.new_addresscountryid IS NOT NULL
	                                AND
	                                AC.new_addresscityid IS NOT NULL
	                                AND
	                                AC.new_addresstownid IS NOT NULL";
                #endregion

                int result = (int)sda.ExecuteScalar(string.Format(query, accountId));

                if (result != 0)
                    returnValue.Success = true;
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşterinin adres alanlarını doldurunuz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResultObject GetAccountGroupCodeByCharacter(char character, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    AC.new_groupcodenumber Number
                                    FROM
	                                    Account AC WITH (NOLOCK)
                                    WHERE
	                                    AC.new_groupcodecharacter = '{0}'
                                    ORDER BY
	                                    AC.new_groupcodenumber DESC";

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

        public static MsCrmResultObject GetTaxOffices(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    T.new_taxofficeId Id
	                                    ,T.new_name Name
                                    FROM
	                                    new_taxoffice T
                                    WHERE
	                                    T.StateCode = 0
                                    ORDER BY 
	                                    T.new_name ASC";

                #endregion

                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    List<TaxOffice> returnList = new List<TaxOffice>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        TaxOffice office = new TaxOffice();
                        office.TaxOfficeId = (Guid)dt.Rows[i]["Id"];
                        office.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;
                        returnList.Add(office);
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin vergi dairesi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;

            }

            return returnValue;
        }

        public static MsCrmResult AccountHasAddressAndTaxNo(Guid accountId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                COUNT(0)
                                FROM
	                                Account AC WITH (NOLOCK)
                                WHERE
	                                AC.AccountId = '{0}'
	                                AND
	                                AC.new_addresscountryid IS NOT NULL
	                                AND
	                                AC.new_addresscityid IS NOT NULL
	                                AND
	                                AC.new_addresstownid IS NOT NULL
                                    AND
                                    AC.new_taxnumber IS NOT NULL";
                #endregion

                int result = (int)sda.ExecuteScalar(string.Format(query, accountId));

                if (result != 0)
                    returnValue.Success = true;
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Lütfen müşterinin adres ve Vergi No alanlarını doldurunuz!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult CheckAccountHasGroupCode(Guid accountId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    AC.new_groupcode Code
                                    FROM
	                                    Account AC WITH (NOLOCK)
                                    WHERE
	                                    AC.AccountId = '{0}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, accountId));

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Code"] != DBNull.Value && !string.IsNullOrEmpty(dt.Rows[0]["Code"].ToString()))
                    {
                        returnValue.Success = true;
                        returnValue.Result = "Grup Kodu var.";
                    }
                    else
                    {
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
    }
}
