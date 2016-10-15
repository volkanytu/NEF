using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class AddressHelper
    {
        public static MsCrmResultObject GetCountries(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT 
	                                C.new_countryId Id
	                                ,C.new_name Name
                                FROM
	                                new_country C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
								ORDER BY
									C.new_isdefault DESC,C.new_name ASC";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET COUNTRIES |
                    List<Country> returnList = new List<Country>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Country _country = new Country();
                        _country.CountryId = (Guid)dt.Rows[i]["Id"];
                        _country.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_country);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin ülke bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetCities(Guid countryId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT 
	                                C.new_cityId Id
	                                ,C.new_name Name
                                FROM
	                                new_city C WITH (NOLOCK)
                                WHERE
	                                C.StateCode = 0
	                                AND
	                                C.new_countryid = '{0}'
                                    ORDER BY
									C.new_isdefault DESC,C.new_name ASC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, countryId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET CITIES |
                    List<City> returnList = new List<City>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        City _city = new City();
                        _city.CityId = (Guid)dt.Rows[i]["Id"];
                        _city.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_city);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu ülkeye ait şehir bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetTowns(Guid cityId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT 
	                                T.new_townId Id
	                                ,T.new_name Name
                                FROM
	                                new_town T WITH (NOLOCK)
                                WHERE
	                                T.StateCode = 0
	                                AND
	                                T.new_cityid = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, cityId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET TOWNS |
                    List<Town> returnList = new List<Town>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Town _town = new Town();
                        _town.TownId = (Guid)dt.Rows[i]["Id"];
                        _town.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_town);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu şehire ait ilçe bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetDistricts(Guid townId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT 
	                                D.new_districtId Id
	                                ,D.new_name Name
                                FROM
	                                new_district D WITH (NOLOCK)
                                WHERE
	                                D.StateCode = 0
	                                AND
	                                D.new_townid = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, townId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET DISTRICTS |
                    List<District> returnList = new List<District>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        District _district = new District();
                        _district.DistrictId = (Guid)dt.Rows[i]["Id"];
                        _district.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_district);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu ilçeye ait semt bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetNationalities(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                N.new_nationalityId Id
	                                ,N.new_name Name
                                FROM
	                                new_nationality N WITH (NOLOCK)
                                WHERE
	                                N.StateCode = 0
                                ORDER BY
	                                N.new_isdefault DESC, N.new_name ASC";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET NATIONALITIES |
                    List<Nationality> returnList = new List<Nationality>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Nationality _nationality = new Nationality();
                        _nationality.NationalityId = (Guid)dt.Rows[i]["Id"];
                        _nationality.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_nationality);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin uyruk bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }
    }
}
