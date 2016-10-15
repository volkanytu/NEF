using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using CsvParser;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;

namespace NEF.Library.Business
{
    public static class ProductHelper
    {
        public static MsCrmResultObject GetQuoteProducts(Guid quoteId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
	                                ,P.new_homenumber HomeNumber	
                                FROM
	                                QuoteDetail QD WITH (NOLOCK)
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                QD.QuoteId = '{0}'
	                                AND
	                                P.ProductId = QD.ProductId";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, quoteId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET QUOTE PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);


                        returnList.Add(_product);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Satışa ait ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetContactInterestedProjects(Guid contactId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                    ,P.Name
	                                ,P.new_projectid ProjectId
	                                ,P.new_projectidName ProjectIdName
                                FROM
	                                new_interestedproductsid IP WITH (NOLOCK)
                                INNER JOIN
	                                new_new_interestedproductsid_product PP WITH (NOLOCK)
	                                ON
	                                PP.new_interestedproductsidid = IP.new_interestedproductsidId
                                INNER JOIN
	                                Product P WITH (NOLOCK)
	                                ON
	                                P.ProductId = PP.productid
                                WHERE
	                                IP.new_contactid = '{0}'
	                                AND
	                                IP.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, contactId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET INTEREST PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product _product = new Product();
                        _product.ProductId = (Guid)dt.Rows[i]["ProductId"];


                        if (dt.Rows[i]["ProjectId"] != DBNull.Value)
                        {
                            EntityReference er = new EntityReference();
                            er.Id = (Guid)dt.Rows[i]["ProjectId"];
                            if (dt.Rows[i]["ProjectIdName"] != DBNull.Value) { er.Name = dt.Rows[i]["ProjectIdName"].ToString(); }
                            er.LogicalName = "new_project";

                            _product.Project = er;
                        }

                        returnList.Add(_product);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Müşterinin ilgilendiği bir konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProjects(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_projectId Id
	                                ,P.new_name Name
                                FROM
	                                new_project P WITH (NOLOCK)
                                WHERE
	                                P.StateCode = 0";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PROJECTS |
                    List<Project> returnList = new List<Project>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Project _project = new Project();
                        _project.ProjectId = (Guid)dt.Rows[i]["Id"];
                        _project.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_project);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin proje bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProjects(OrganizationServiceContext orgServiceContext)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                var linqQuery = (from a in orgServiceContext.CreateQuery("new_project")
                                 where
                                 ((OptionSetValue)a["statecode"]).Value == 0
                                 select new
                                 {
                                     Id = a.Id,
                                     Name = a["new_name"].ToString(),
                                     IsGyo = a.Contains("new_iswithisgyo") ? (bool)a["new_iswithisgyo"] : false
                                 }).ToList();

                if (linqQuery != null && linqQuery.Count > 0)
                {
                    #region | GET PROJECTS |
                    List<Project> returnList = new List<Project>();

                    for (int i = 0; i < linqQuery.Count; i++)
                    {
                        Project _project = new Project();
                        _project.ProjectId = linqQuery[i].Id;
                        _project.Name = linqQuery[i].Name;
                        _project.IsProjectGyo = linqQuery[i].IsGyo;

                        returnList.Add(_project);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin proje bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProjectBlocks(Guid projectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                    DISTINCT
	                                    pro.new_blockid AS Id
	                                    ,pro.new_blockidName Name
                                    FROM
                                    Product AS pro (NOLOCK)
	                                    JOIN
		                                    new_project AS p
			                                    ON
			                                    pro.new_projectid=p.new_projectId
                                    WHERE
                                    p.new_projectId='{0}'
                                    AND
                                    pro.new_blockid IS NOT NULL";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET BLOCKS |
                    List<Block> returnList = new List<Block>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Block _block = new Block();
                        _block.BlockId = (Guid)dt.Rows[i]["Id"];
                        _block.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_block);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Projeye ait blok bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetGeneralHomeTypes(Guid projectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                    DISTINCT
	                                    pro.new_generaltypeofhomeid AS Id
	                                    ,pro.new_generaltypeofhomeidName Name
                                    FROM
                                    Product AS pro (NOLOCK)
	                                    JOIN
		                                    new_project AS p
			                                    ON
			                                    pro.new_projectid=p.new_projectId
                                    WHERE
                                    p.new_projectId='{0}'
                                    AND
                                    pro.new_generaltypeofhomeid IS NOT NULL AND pro.StateCode=0";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET GENERAL HOME TYPES |
                    List<GeneralHomeType> returnList = new List<GeneralHomeType>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GeneralHomeType _home = new GeneralHomeType();
                        _home.GeneralHomeTypeId = (Guid)dt.Rows[i]["Id"];
                        _home.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_home);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde genel daire tipi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetGeneralHomeTypesForRent(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"select new_generaltypeofhomeId AS Id , new_name AS Name from new_generaltypeofhome with(Nolock)";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET GENERAL HOME TYPES |
                    List<GeneralHomeType> returnList = new List<GeneralHomeType>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GeneralHomeType _home = new GeneralHomeType();
                        _home.GeneralHomeTypeId = (Guid)dt.Rows[i]["Id"];
                        _home.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_home);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde genel daire tipi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetHomeTypesByGeneralType(Guid generalHomeTypeId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TH.new_typeofhomeId Id
	                                ,TH.new_name Name
                                FROM
	                                new_typeofhome TH WITH (NOLOCK)
                                WHERE
	                                TH.StateCode = 0
	                                AND
	                                TH.new_generaltypeofhomeid = '{0}'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, generalHomeTypeId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET HOME TYPES |
                    List<HomeType> returnList = new List<HomeType>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        HomeType _home = new HomeType();
                        _home.HomeTypeId = (Guid)dt.Rows[i]["Id"];
                        _home.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_home);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Genel daire tipine ait daire tipi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetHomeTypesByGeneralTypeAndProject(Guid generalHomeTypeId, Guid projectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT DISTINCT
	                                pro.new_typeofhomeid Id
	                                ,pro.new_typeofhomeidName Name
                                FROM 
	                                new_project AS p WITH(NOLOCK)
                                JOIN 
	                                Product AS pro  WITH(NOLOCK) 
                                ON 
	                                p.new_projectId = pro.new_projectid
                                WHERE 
	                                pro.new_generaltypeofhomeid  = '{0}' 
                                AND 
	                                p.new_projectId = '{1}'
                                AND 
	                                pro.new_typeofhomeid is not null";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, generalHomeTypeId, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET HOME TYPES |
                    List<HomeType> returnList = new List<HomeType>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        HomeType _home = new HomeType();
                        _home.HomeTypeId = (Guid)dt.Rows[i]["Id"];
                        _home.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_home);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Genel daire tipine ait daire tipi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetHomeTypesByGeneralTypeForRent(Guid generalHomeTypeId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"select new_typeofhomeId AS Id, new_name AS Name from new_typeofhome with(nolock) where new_generaltypeofhomeid  = '{0}' ";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, generalHomeTypeId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET HOME TYPES |
                    List<HomeType> returnList = new List<HomeType>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        HomeType _home = new HomeType();
                        _home.HomeTypeId = (Guid)dt.Rows[i]["Id"];
                        _home.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_home);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Genel daire tipine ait daire tipi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProjectLocations(Guid projectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                    DISTINCT
	                                    pro.new_locationid AS Id
	                                    ,pro.new_locationidName Name
                                    FROM
                                    Product AS pro (NOLOCK)
	                                    JOIN
		                                    new_project AS p
			                                    ON
			                                    pro.new_projectid=p.new_projectId
                                    WHERE
                                    p.new_projectId='{0}'
                                    AND
                                    pro.new_locationid IS NOT NULL";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET LOCATIONS |
                    List<Location> returnList = new List<Location>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Location _location = new Location();
                        _location.LocationId = (Guid)dt.Rows[i]["Id"];
                        _location.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_location);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Projeye ait konum bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetBlockTypes(SqlDataAccess sda, Guid projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                    DISTINCT
	                                    pro.new_blocktypeid AS Id
	                                    ,pro.new_blocktypeidName Name
                                    FROM
                                    Product AS pro (NOLOCK)
	                                    JOIN
		                                    new_project AS p
			                                    ON
			                                    pro.new_projectid=p.new_projectId
                                    WHERE
                                    p.new_projectId='{0}'
                                    AND
                                    pro.new_blocktypeid IS NOT NULL";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET BLOCKS |
                    List<BlockType> returnList = new List<BlockType>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        BlockType _block = new BlockType();
                        _block.BlockTypeId = (Guid)dt.Rows[i]["Id"];
                        _block.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_block);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Projeye ait blok tipi bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetEtaps(SqlDataAccess sda, Guid projectId)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT DISTINCT
	                                    pro.new_Etapid AS Id
	                                    ,pro.new_EtapidName Name
                                    FROM
                                    Product AS pro (NOLOCK)
	                                    JOIN
		                                    new_project AS p
			                                    ON
			                                    pro.new_projectid=p.new_projectId
                                    WHERE
                                    p.new_projectId='{0}'
                                    AND
                                    pro.new_Etapid IS NOT NULL";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET BLOCKS |
                    List<Etap> returnList = new List<Etap>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Etap _etap = new Etap();
                        _etap.EtapId = (Guid)dt.Rows[i]["Id"];
                        _etap.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_etap);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Projeye ait etap bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult UpdateProductAuthorityDoc(AuthorityDocument document, IOrganizationService service)
        {
            MsCrmResult retVal = new MsCrmResult();
            try
            {
                Entity ent = new Entity("product");
                ent.Id = document.Product.Id;
                if (document.Contact != null)
                {
                    ent["new_authorizingpersonid"] = document.Contact;
                }
                if (document.StartDate != null)
                {
                    ent["new_startofauthority"] = document.StartDate;
                }
                if (document.EndDate != null)
                {
                    ent["new_endofauthority"] = document.EndDate;
                }
                ent["new_isimportauthoritydoc"] = true;
                service.Update(ent);
                retVal.Success = true;
                retVal.Result = "Ürüne ait yetki bilgileri başarıyla güncellenmiştir.";
            }
            catch (Exception)
            {
                retVal.Success = false;
                retVal.Result = "Hata" + Environment.NewLine + "Ürün yetki belgeleri güncellenmesinde hata alındı";
            }
            return retVal;
        }

        public static MsCrmResultObject MakeHouseSearch(Product _product, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                SqlParameter[] parameters = null;
                #region | SQL QUERY |
                string query = @"SELECT 
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
	                                P.StateCode = " + _product.StateCode.Value;

                if (_product.Project != null && _product.Project.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_projectid = '{0}'";
                    query = string.Format(query, _product.Project.Id);
                }
                else
                {
                    OrganizationServiceContext orgServiceContext = new OrganizationServiceContext(service);

                    var linqQuery = (from a in orgServiceContext.CreateQuery("new_project")
                                     where
                                     ((OptionSetValue)a["statecode"]).Value == 0
                                     select new
                                     {
                                         Id = a.Id
                                     }).ToList();
                    if (linqQuery != null && linqQuery.Count > 0)
                    {
                        query += @"	AND
	                            P.new_projectid IN(";

                        for (int i = 0; i < linqQuery.Count; i++)
                        {
                            if (i != linqQuery.Count - 1)
                            {
                                query += "'" + linqQuery[i].Id + "',";
                            }
                            else
                            {
                                query += "'" + linqQuery[i].Id + "'";
                            }
                        }
                        query += ")";
                    }

                }

                if (_product.Block != null && _product.Block.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blockid = '{0}'";
                    query = string.Format(query, _product.Block.Id);
                }

                if (_product.BlockType != null && _product.BlockType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blocktypeid = '{0}'";
                    query = string.Format(query, _product.BlockType.Id);
                }

                if (_product.Etap != null && _product.Etap.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_Etapid = '{0}'";
                    query = string.Format(query, _product.Etap.Id);
                }

                if (_product.GeneralHomeType != null && _product.GeneralHomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_generaltypeofhomeid = '{0}'";
                    query = string.Format(query, _product.GeneralHomeType.Id);
                }

                if (_product.HomeType != null && _product.HomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_typeofhomeid = '{0}'";
                    query = string.Format(query, _product.HomeType.Id);
                }

                if (_product.Location != null && _product.Location.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_locationid = '{0}'";
                    query = string.Format(query, _product.Location.Id);
                }

                if (_product.StatusCode != null && _product.StatusCode.Value != null)
                {
                    if (_product.StatusCode.Value == 100000008)
                    {
                        query += @"	AND
	                                P.StatusCode IN (100000008,1,100000003,100000007)";
                        query = string.Format(query, _product.StatusCode.Value);
                    }
                    else
                    {
                        query += @"	AND
	                            P.StatusCode = {0}";
                        query = string.Format(query, _product.StatusCode.Value);
                    }

                }

                if (!string.IsNullOrEmpty(_product.LicenceNumber))
                {
                    query += @"	AND
	                            P.new_licencenumber = '{0}'";
                    query = string.Format(query, _product.LicenceNumber);
                }

                if (_product.FloorNumber != null)
                {
                    query += @"	AND
	                            P.new_floornumber = {0}";
                    query = string.Format(query, _product.FloorNumber);
                }

                if (_product.HomeNumber != null)
                {
                    query += @"	AND
	                            P.new_homenumber = '{0}'";
                    query = string.Format(query, _product.HomeNumber);
                }

                if (_product.Aks != null)
                {
                    query += @"	AND
	                            P.new_aks = {0}";
                    query = string.Format(query, _product.Aks);
                }

                if (_product.Direction != null)
                {
                    query += @"	AND
	                            P.new_generalway = {0}";
                    query = string.Format(query, _product.Direction);
                }

                if (_product.HasTerrace)
                {
                    query += @"	AND
	                            P.new_terracegross IS NOT NULL AND P.new_terracegross>0";
                }

                if (_product.HasBalcony)
                {
                    query += @"	AND
	                            P.new_balconym2 IS NOT NULL AND P.new_balconym2>0";
                }

                if (_product.HasKitchen)
                {
                    query += @"	AND
	                            P.new_kitchenm2 IS NOT NULL AND P.new_kitchenm2>0";
                }

                if (_product.MinValue != null || _product.MaxValue != null)
                {

                    if (_product.MinValue != null && _product.MaxValue != null)
                    {
                        query += @"	AND
	                                    p.Price BETWEEN @minValue AND @maxValue";

                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue), new SqlParameter("@maxValue", _product.MaxValue) };

                    }
                    else if (_product.MinValue != null)
                    {
                        query += @"	AND
	                                    p.Price >= @minValue";
                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue) };
                    }
                    else
                    {
                        query += @"	AND
	                                    p.Price <= @maxValue";
                        parameters = new SqlParameter[] { new SqlParameter("@maxValue", _product.MaxValue) };
                    }
                }
                #endregion

                DataTable dt = null;
                if (parameters == null)
                {
                    dt = sda.getDataTable(query);
                }
                else
                {
                    dt = sda.getDataTable(query, parameters);
                }


                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product proc = GetProductDetailByCurrency((Guid)dt.Rows[i]["ProductId"], _product.Currency.Id, sda);
                        returnList.Add(proc);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere ait konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject MakeRentalHouseSearch(Product _product, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                SqlParameter[] parameters = null;
                #region | SQL QUERY |
                string query = @"SELECT 
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
	                                P.StateCode = 0";

                if (_product.Project != null && _product.Project.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_projectid = '{0}'";
                    query = string.Format(query, _product.Project.Id);
                }
                else
                {
                    OrganizationServiceContext orgServiceContext = new OrganizationServiceContext(service);

                    var linqQuery = (from a in orgServiceContext.CreateQuery("new_project")
                                     where
                                     ((OptionSetValue)a["statecode"]).Value == 0
                                     select new
                                     {
                                         Id = a.Id
                                     }).ToList();
                    if (linqQuery != null && linqQuery.Count > 0)
                    {
                        query += @"	AND
	                            P.new_projectid IN(";

                        for (int i = 0; i < linqQuery.Count; i++)
                        {
                            if (i != linqQuery.Count - 1)
                            {
                                query += "'" + linqQuery[i].Id + "',";
                            }
                            else
                            {
                                query += "'" + linqQuery[i].Id + "'";
                            }
                        }
                        query += ")";
                    }

                }

                if (_product.Block != null && _product.Block.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blockid = '{0}'";
                    query = string.Format(query, _product.Block.Id);
                }

                if (_product.BlockType != null && _product.BlockType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blocktypeid = '{0}'";
                    query = string.Format(query, _product.BlockType.Id);
                }

                if (_product.Etap != null && _product.Etap.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_Etapid = '{0}'";
                    query = string.Format(query, _product.Etap.Id);
                }

                if (_product.GeneralHomeType != null && _product.GeneralHomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_generaltypeofhomeid = '{0}'";
                    query = string.Format(query, _product.GeneralHomeType.Id);
                }

                if (_product.HomeType != null && _product.HomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_typeofhomeid = '{0}'";
                    query = string.Format(query, _product.HomeType.Id);
                }

                if (_product.Location != null && _product.Location.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_locationid = '{0}'";
                    query = string.Format(query, _product.Location.Id);
                }
                if (_product.StatusCode != null && _product.StatusCode.Value != null)
                {
                    query += @"	AND
	                            P.new_usedrentalandsalesstatus = {0}";
                    query = string.Format(query, _product.StatusCode.Value);
                }
                else
                {
                    query += @"	AND
	                                P.new_usedrentalandsalesstatus IN (2,5)";
                }

                if (!string.IsNullOrEmpty(_product.LicenceNumber))
                {
                    query += @"	AND
	                            P.new_licencenumber = '{0}'";
                    query = string.Format(query, _product.LicenceNumber);
                }

                if (_product.FloorNumber != null)
                {
                    query += @"	AND
	                            P.new_floornumber = {0}";
                    query = string.Format(query, _product.FloorNumber);
                }

                if (_product.HomeNumber != null)
                {
                    query += @"	AND
	                            P.new_homenumber = '{0}'";
                    query = string.Format(query, _product.HomeNumber);
                }

                if (_product.Aks != null)
                {
                    query += @"	AND
	                            P.new_aks = {0}";
                    query = string.Format(query, _product.Aks);
                }

                if (_product.Direction != null)
                {
                    query += @"	AND
	                            P.new_generalway = {0}";
                    query = string.Format(query, _product.Direction);
                }

                if (_product.HasTerrace)
                {
                    query += @"	AND
	                            P.new_terracegross IS NOT NULL AND P.new_terracegross>0";
                }

                if (_product.HasBalcony)
                {
                    query += @"	AND
	                            P.new_balconym2 IS NOT NULL AND P.new_balconym2>0";
                }

                if (_product.HasKitchen)
                {
                    query += @"	AND
	                            P.new_kitchenm2 IS NOT NULL AND P.new_kitchenm2>0";
                }

                if (_product.MinValue != null || _product.MaxValue != null)
                {

                    if (_product.MinValue != null && _product.MaxValue != null)
                    {
                        query += @"	AND
	                                    p.new_paymentofhire BETWEEN @minValue AND @maxValue";

                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue), new SqlParameter("@maxValue", _product.MaxValue) };

                    }
                    else if (_product.MinValue != null)
                    {
                        query += @"	AND
	                                    p.new_paymentofhire >= @minValue";
                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue) };
                    }
                    else
                    {
                        query += @"	AND
	                                    p.new_paymentofhire <= @maxValue";
                        parameters = new SqlParameter[] { new SqlParameter("@maxValue", _product.MaxValue) };
                    }
                }
                if (_product.RentalMonths != null)
                {
                    query += @"	AND
	                            P.new_rentalmonths = '{0}'";
                    query = string.Format(query, _product.RentalMonths.Value);
                }
                if (_product.GoodsStatus != null)
                {
                    query += @"	AND
	                            P.new_goodsstatus = '{0}'";
                    query = string.Format(query, _product.GoodsStatus);
                }
                if (_product.HasAuthority)
                {
                    query += @"	AND
	                            P.new_isimportauthoritydoc IS NOT NULL AND P.new_isimportauthoritydoc > 0";
                }
                #endregion

                DataTable dt = null;
                if (parameters == null)
                {
                    dt = sda.getDataTable(query);
                }
                else
                {
                    dt = sda.getDataTable(query, parameters);
                }


                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product proc = GetProductDetailByCurrency((Guid)dt.Rows[i]["ProductId"], _product.Currency.Id, sda);
                        returnList.Add(proc);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere ait konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject MakeHouseSearchForActivity(Product _product, Guid phonecallId, Guid appointmentId, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                SqlParameter[] parameters = null;
                #region | PRODUCTS |
                string query = @"SELECT
                                    *
                                INTO
                                    #Products
                                FROM
                                (
                                   SELECT 
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
	                             P.StateCode = " + _product.StateCode.Value;

                if (_product.Project != null && _product.Project.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_projectid = '{0}'";
                    query = string.Format(query, _product.Project.Id);
                }
                else
                {
                    OrganizationServiceContext orgServiceContext = new OrganizationServiceContext(service);

                    var linqQuery = (from a in orgServiceContext.CreateQuery("new_project")
                                     where
                                     ((OptionSetValue)a["statecode"]).Value == 0
                                     select new
                                     {
                                         Id = a.Id
                                     }).ToList();
                    if (linqQuery != null && linqQuery.Count > 0)
                    {
                        query += @"	AND
	                            P.new_projectid IN(";

                        for (int i = 0; i < linqQuery.Count; i++)
                        {
                            if (i != linqQuery.Count - 1)
                            {
                                query += "'" + linqQuery[i].Id + "',";
                            }
                            else
                            {
                                query += "'" + linqQuery[i].Id + "'";
                            }
                        }
                        query += ")";
                    }

                }

                if (_product.Block != null && _product.Block.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blockid = '{0}'";
                    query = string.Format(query, _product.Block.Id);
                }

                if (_product.BlockType != null && _product.BlockType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blocktypeid = '{0}'";
                    query = string.Format(query, _product.BlockType.Id);
                }

                if (_product.Etap != null && _product.Etap.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_Etapid = '{0}'";
                    query = string.Format(query, _product.Etap.Id);
                }

                if (_product.GeneralHomeType != null && _product.GeneralHomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_generaltypeofhomeid = '{0}'";
                    query = string.Format(query, _product.GeneralHomeType.Id);
                }

                if (_product.HomeType != null && _product.HomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_typeofhomeid = '{0}'";
                    query = string.Format(query, _product.HomeType.Id);
                }

                if (_product.Location != null && _product.Location.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_locationid = '{0}'";
                    query = string.Format(query, _product.Location.Id);
                }

                if (_product.StatusCode != null && _product.StatusCode.Value != null)
                {
                    if (_product.StatusCode.Value == 100000008)
                    {
                        query += @"	AND
	                                P.StatusCode IN (100000008,1,100000003,100000007)";
                        query = string.Format(query, _product.StatusCode.Value);
                    }
                    else
                    {
                        query += @"	AND
	                            P.StatusCode = {0}";
                        query = string.Format(query, _product.StatusCode.Value);
                    }
                }

                if (!string.IsNullOrEmpty(_product.LicenceNumber))
                {
                    query += @"	AND
	                            P.new_licencenumber = '{0}'";
                    query = string.Format(query, _product.LicenceNumber);
                }


                if (_product.FloorNumber != null)
                {
                    query += @"	AND
	                            P.new_floornumber = {0}";
                    query = string.Format(query, _product.FloorNumber);
                }

                if (_product.HomeNumber != null)
                {
                    query += @"	AND
	                            P.new_homenumber = '{0}'";
                    query = string.Format(query, _product.HomeNumber);
                }

                if (_product.Aks != null)
                {
                    query += @"	AND
	                            P.new_aks = {0}";
                    query = string.Format(query, _product.Aks);
                }

                if (_product.Direction != null)
                {
                    query += @"	AND
	                            P.new_generalway = {0}";
                    query = string.Format(query, _product.Direction);
                }

                if (_product.HasTerrace)
                {
                    query += @"	AND
	                            P.new_terracegross IS NOT NULL AND P.new_terracegross>0";
                }

                if (_product.HasBalcony)
                {
                    query += @"	AND
	                            P.new_balconym2 IS NOT NULL AND P.new_balconym2>0";
                }

                if (_product.HasKitchen)
                {
                    query += @"	AND
	                            P.new_kitchenm2 IS NOT NULL AND P.new_kitchenm2>0";
                }

                if (_product.MinValue != null || _product.MaxValue != null)
                {

                    if (_product.MinValue != null && _product.MaxValue != null)
                    {
                        query += @"	AND
	                                    p.Price BETWEEN @minValue AND @maxValue";

                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue), new SqlParameter("@maxValue", _product.MaxValue) };

                    }
                    else if (_product.MinValue != null)
                    {
                        query += @"	AND
	                                    p.Price >= @minValue";
                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue) };
                    }
                    else
                    {
                        query += @"	AND
	                                    p.Price <= @maxValue";
                        parameters = new SqlParameter[] { new SqlParameter("@maxValue", _product.MaxValue) };
                    }
                }

                query += ")A";
                query += System.Environment.NewLine;
                #endregion

                #region | INTERESTED HOUSES |
                query += @"SELECT
                            *
                            INTO
	                            #InterestedHouses
                            FROM
                            (
	                            SELECT
		                            IP.new_productid
	                            FROM
		                            new_interestedproducts IP WITH (NOLOCK)
		                            WHERE";
                if (phonecallId != Guid.Empty)
                {
                    query += System.Environment.NewLine;
                    query += "      IP.new_phonecallid = '{0}'";
                    query = string.Format(query, phonecallId);
                }
                else
                {
                    query += System.Environment.NewLine;
                    query += "      IP.new_appointmentid = '{0}'";
                    query = string.Format(query, appointmentId);
                }

                query += System.Environment.NewLine;
                query += @"           AND
		                            IP.statecode = 0
                            )B";

                #endregion

                query += System.Environment.NewLine;
                query += @" SELECT
	                            *
                            FROM
	                            #Products T
                            WHERE
	                            T.ProductId NOT IN (SELECT * FROM #InterestedHouses)

                            DROP TABLE #Products
                            DROP TABLE #InterestedHouses";
                #endregion

                DataTable dt = null;
                if (parameters == null)
                {
                    dt = sda.getDataTable(query);
                }
                else
                {
                    dt = sda.getDataTable(query, parameters);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product proc = GetProductDetailByCurrency((Guid)dt.Rows[i]["ProductId"], _product.Currency.Id, sda);
                        returnList.Add(proc);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere ait konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject MakeRentalHouseSearchForActivity(Product _product, Guid phonecallId, Guid appointmentId, IOrganizationService service, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                SqlParameter[] parameters = null;
                #region | PRODUCTS |
                string query = @"SELECT
                                    *
                                INTO
                                    #Products
                                FROM
                                (
                                   SELECT 
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
                                       P.StateCode = 0";

                if (_product.Project != null && _product.Project.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_projectid = '{0}'";
                    query = string.Format(query, _product.Project.Id);
                }
                else
                {
                    OrganizationServiceContext orgServiceContext = new OrganizationServiceContext(service);

                    var linqQuery = (from a in orgServiceContext.CreateQuery("new_project")
                                     where
                                     ((OptionSetValue)a["statecode"]).Value == 0
                                     select new
                                     {
                                         Id = a.Id
                                     }).ToList();
                    if (linqQuery != null && linqQuery.Count > 0)
                    {
                        query += @"	AND
	                            P.new_projectid IN(";

                        for (int i = 0; i < linqQuery.Count; i++)
                        {
                            if (i != linqQuery.Count - 1)
                            {
                                query += "'" + linqQuery[i].Id + "',";
                            }
                            else
                            {
                                query += "'" + linqQuery[i].Id + "'";
                            }
                        }
                        query += ")";
                    }

                }

                if (_product.Block != null && _product.Block.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blockid = '{0}'";
                    query = string.Format(query, _product.Block.Id);
                }

                if (_product.BlockType != null && _product.BlockType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_blocktypeid = '{0}'";
                    query = string.Format(query, _product.BlockType.Id);
                }

                if (_product.Etap != null && _product.Etap.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_Etapid = '{0}'";
                    query = string.Format(query, _product.Etap.Id);
                }

                if (_product.GeneralHomeType != null && _product.GeneralHomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_generaltypeofhomeid = '{0}'";
                    query = string.Format(query, _product.GeneralHomeType.Id);
                }

                if (_product.HomeType != null && _product.HomeType.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_typeofhomeid = '{0}'";
                    query = string.Format(query, _product.HomeType.Id);
                }

                if (_product.Location != null && _product.Location.Id != Guid.Empty)
                {
                    query += @"	AND
	                            P.new_locationid = '{0}'";
                    query = string.Format(query, _product.Location.Id);
                }

                if (_product.StatusCode != null && _product.StatusCode.Value != null)
                {
                    query += @"	AND
	                            P.new_usedrentalandsalesstatus = {0}";
                    query = string.Format(query, _product.StatusCode.Value);
                }
                else
                {
                    query += @"	AND
	                                P.new_usedrentalandsalesstatus IN (2,5)";
                }


                if (!string.IsNullOrEmpty(_product.LicenceNumber))
                {
                    query += @"	AND
	                            P.new_licencenumber = '{0}'";
                    query = string.Format(query, _product.LicenceNumber);
                }

                if (_product.RentalMonths != null)
                {
                    query += @"	AND
	                            P.new_rentalmonths = '{0}'";
                    query = string.Format(query, _product.RentalMonths.Value);
                }
                if (_product.GoodsStatus != null)
                {
                    query += @"	AND
	                            P.new_goodsstatus = '{0}'";
                    query = string.Format(query, _product.GoodsStatus);
                }

                if (_product.FloorNumber != null)
                {
                    query += @"	AND
	                            P.new_floornumber = {0}";
                    query = string.Format(query, _product.FloorNumber);
                }

                if (_product.HomeNumber != null)
                {
                    query += @"	AND
	                            P.new_homenumber = '{0}'";
                    query = string.Format(query, _product.HomeNumber);
                }

                if (_product.Aks != null)
                {
                    query += @"	AND
	                            P.new_aks = {0}";
                    query = string.Format(query, _product.Aks);
                }

                if (_product.Direction != null)
                {
                    query += @"	AND
	                            P.new_generalway = {0}";
                    query = string.Format(query, _product.Direction);
                }

                if (_product.HasTerrace)
                {
                    query += @"	AND
	                            P.new_terracegross IS NOT NULL AND P.new_terracegross>0";
                }

                if (_product.HasBalcony)
                {
                    query += @"	AND
	                            P.new_balconym2 IS NOT NULL AND P.new_balconym2>0";
                }

                if (_product.HasKitchen)
                {
                    query += @"	AND
	                            P.new_kitchenm2 IS NOT NULL AND P.new_kitchenm2>0";
                }
                if (_product.HasAuthority)
                {
                    query += @"	AND
	                            P.new_isimportauthoritydoc IS NOT NULL AND P.new_isimportauthoritydoc > 0";
                }
                if (_product.MinValue != null || _product.MaxValue != null)
                {

                    if (_product.MinValue != null && _product.MaxValue != null)
                    {
                        query += @"	AND
	                                    p.new_paymentofhire BETWEEN @minValue AND @maxValue";

                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue), new SqlParameter("@maxValue", _product.MaxValue) };

                    }
                    else if (_product.MinValue != null)
                    {
                        query += @"	AND
	                                    p.new_paymentofhire >= @minValue";
                        parameters = new SqlParameter[] { new SqlParameter("@minValue", _product.MinValue) };
                    }
                    else
                    {
                        query += @"	AND
	                                    p.new_paymentofhire <= @maxValue";
                        parameters = new SqlParameter[] { new SqlParameter("@maxValue", _product.MaxValue) };
                    }
                }

                query += ")A";
                query += System.Environment.NewLine;
                #endregion

                #region | INTERESTED HOUSES |
                query += @"SELECT
                            *
                            INTO
	                            #InterestedHouses
                            FROM
                            (
	                            SELECT
		                            IP.new_productid
	                            FROM
		                            new_interestedproducts IP WITH (NOLOCK)
		                            WHERE";
                if (phonecallId != Guid.Empty)
                {
                    query += System.Environment.NewLine;
                    query += "      IP.new_phonecallid = '{0}'";
                    query = string.Format(query, phonecallId);
                }
                else
                {
                    query += System.Environment.NewLine;
                    query += "      IP.new_appointmentid = '{0}'";
                    query = string.Format(query, appointmentId);
                }

                query += System.Environment.NewLine;
                query += @"           AND
		                            IP.statecode = 0
                            )B";

                #endregion

                query += System.Environment.NewLine;
                query += @" SELECT
	                            *
                            FROM
	                            #Products T
                            WHERE
	                            T.ProductId NOT IN (SELECT * FROM #InterestedHouses)

                            DROP TABLE #Products
                            DROP TABLE #InterestedHouses";
                #endregion

                DataTable dt = null;
                if (parameters == null)
                {
                    dt = sda.getDataTable(query);
                }
                else
                {
                    dt = sda.getDataTable(query, parameters);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product proc = GetProductDetailByCurrency((Guid)dt.Rows[i]["ProductId"], _product.Currency.Id, sda);
                        returnList.Add(proc);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Aradığınız kriterlere ait konut bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static Product GetProductDetail(Guid productId, SqlDataAccess sda)
        {
            Product returnValue = new Product();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                p.ProductId
	                                ,p.Name
	                                ,p.new_projectid AS ProjectId
	                                ,p.new_projectidName AS ProjectIdName
	                                ,p.new_blockid AS BlockId
	                                ,p.new_blockidName AS BlockIdName
	                                ,p.new_generaltypeofhomeid AS GeneralHomeTypeId
	                                ,p.new_generaltypeofhomeidName AS GeneralHomeTypeIdName
	                                ,p.new_typeofhomeid AS HomeTypeId
	                                ,p.new_typeofhomeidName AS HomeTypeIdName
	                                ,p.new_locationid AS LocationId
	                                ,p.new_locationidName AS LocationIdName
	                                ,p.new_floornumber AS FloorNumber
	                                ,p.new_homenumber AS HomeNumber
	                                ,p.Price AS HousePrice
	                                ,p.new_KDVratio AS KDV
	                                ,p.new_taxofstamp AS TaxOfStamp
	                                ,p.TransactionCurrencyId
	                                ,p.TransactionCurrencyIdName
                                    ,p.StatusCode
                                    ,p.new_grossm2 Brut
                                    ,p.new_netm2 Net
                                    ,p.defaultuomId UomId
                                    ,p.defaultuomIdName UomIdName
                                    ,sm.Value AS StatusCodeName
                                    ,u.CurrencySymbol
									,p.PriceLevelId
									,p.PriceLevelIdName
                                    ,p.new_aks AS Aks
                                    ,p.ProductNumber
                                    ,P.new_persquaremeter UnitPrice
                                    ,p.ModifiedOn
                                    ,SM1.Value GeneralWay
                                    ,p.new_terracegross Terrace
                                    ,p.new_balconym2 Balcony
                                    ,p.new_kitchenm2 Kitchen
                                    ,P.new_licencenumber LicenceNumber
                                    ,p.new_bbnetarea BBNetArea
                                    ,p.new_bbgeneralgrossarea BBGeneralGrossArea
                                    ,p.new_goodsstatus as GoodsStatus
                                    ,p.new_usedrentalandsalesstatus UsedRentalSalesStatus
                                    ,p.new_paymentofhire PaymentOfHire
                                    ,p.new_rentalnotes RentalNotes
                                    ,p.new_rentalmonths RentalMonths
                                    ,p.new_deckm2 Deck
                                    ,p.new_courtm2 Court
                                    ,p.new_garden Garden
                                    ,p.new_poolm2 As Pool
                                    ,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'USD'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId= p.ProductId
									)DolarPrice
									,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'TL'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId=  p.ProductId
									)TLPrice
									,(
										SELECT
											PR.new_iswithisgyo
										FROM
											new_project PR WITH (NOLOCK)
										WHERE
											PR.new_projectId = P.new_projectid
									)ProjectIsGyo
                                FROM
	                                Product AS p (NOLOCK)
                                        JOIN
                                            StringMap AS sm (NOLOCK)
                                                ON
                                                sm.ObjectTypeCode=1024
                                                AND
                                                sm.AttributeName='statuscode'
                                                AND
                                                sm.AttributeValue=p.StatusCode
                                        JOIN
                                            TransactionCurrency AS u (NOLOCK)
                                                ON
                                                u.TransactionCurrencyId=p.TransactionCurrencyId
                                        LEFT JOIN
                                            StringMap AS SM1 (NOLOCK)
                                                ON
                                                SM1.ObjectTypeCode=1024
                                                AND
                                                SM1.AttributeName='new_generalway'
                                                AND
                                                SM1.AttributeValue= p.new_generalway
                                            
                                WHERE
	                                p.ProductId=@productId";

            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@productId", productId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                #region | FILL PRODUCT INFO |

                returnValue.ProductId = (Guid)dt.Rows[0]["ProductId"];
                returnValue.Name = dt.Rows[0]["Name"].ToString();
                returnValue.ModifiedOn = (DateTime)dt.Rows[0]["ModifiedOn"];
                returnValue.IsProjectGyo = dt.Rows[0]["ProjectIsGyo"] != DBNull.Value ? (bool)dt.Rows[0]["ProjectIsGyo"] : false;

                if (dt.Rows[0]["ProjectId"] != DBNull.Value)
                {
                    returnValue.Project = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["ProjectId"],
                        Name = dt.Rows[0]["ProjectIdName"].ToString(),
                        LogicalName = "new_project"
                    };
                }

                if (dt.Rows[0]["BlockId"] != DBNull.Value)
                {
                    returnValue.Block = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["BlockId"],
                        Name = dt.Rows[0]["BlockIdName"].ToString(),
                        LogicalName = "new_block"
                    };
                }

                if (dt.Rows[0]["GeneralHomeTypeId"] != DBNull.Value)
                {
                    returnValue.GeneralHomeType = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["GeneralHomeTypeId"],
                        Name = dt.Rows[0]["GeneralHomeTypeIdName"].ToString(),
                        LogicalName = "new_generaltypeofhome"
                    };
                }

                if (dt.Rows[0]["HomeTypeId"] != DBNull.Value)
                {
                    returnValue.HomeType = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["HomeTypeId"],
                        Name = dt.Rows[0]["HomeTypeIdName"].ToString(),
                        LogicalName = "new_typeofhome"
                    };
                }

                if (dt.Rows[0]["LocationId"] != DBNull.Value)
                {
                    returnValue.Location = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["LocationId"],
                        Name = dt.Rows[0]["LocationIdName"].ToString(),
                        LogicalName = "new_location"
                    };
                }

                if (dt.Rows[0]["TransactionCurrencyId"] != DBNull.Value)
                {
                    returnValue.Currency = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["TransactionCurrencyId"],
                        Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(),
                        LogicalName = "transactioncurrency"
                    };
                }

                if (dt.Rows[0]["UomId"] != DBNull.Value)
                {
                    returnValue.Uom = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["UomId"],
                        Name = dt.Rows[0]["UomIdName"].ToString(),
                        LogicalName = "uom"
                    };
                }

                if (dt.Rows[0]["Aks"] != DBNull.Value)
                {
                    returnValue.Aks = (int)dt.Rows[0]["Aks"];
                }

                if (dt.Rows[0]["PriceLevelId"] != DBNull.Value)
                {
                    returnValue.PriceList = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["PriceLevelId"],
                        Name = dt.Rows[0]["PriceLevelIdName"].ToString(),
                        LogicalName = "pricelevel"
                    };
                }

                if (dt.Rows[0]["LicenceNumber"] != DBNull.Value)
                {
                    returnValue.LicenceNumber = dt.Rows[0]["LicenceNumber"].ToString();
                }

                if (dt.Rows[0]["FloorNumber"] != DBNull.Value)
                {
                    returnValue.FloorNumber = (int)dt.Rows[0]["FloorNumber"];
                }

                if (dt.Rows[0]["HomeNumber"] != DBNull.Value)
                {
                    returnValue.HomeNumber = dt.Rows[0]["HomeNumber"].ToString();
                }

                if (dt.Rows[0]["HousePrice"] != DBNull.Value)
                {
                    returnValue.Price = (decimal)dt.Rows[0]["HousePrice"];
                    returnValue.PriceString = ((decimal)dt.Rows[0]["HousePrice"]).ToString("N2", CultureInfo.CurrentCulture) + " " + dt.Rows[0]["CurrencySymbol"].ToString();
                }

                if (dt.Rows[0]["KDV"] != DBNull.Value)
                {
                    returnValue.KdvRatio = (decimal)dt.Rows[0]["KDV"];
                }

                if (dt.Rows[0]["TaxOfStamp"] != DBNull.Value)
                {
                    returnValue.TaxofStampRatio = (decimal)dt.Rows[0]["TaxOfStamp"];
                }

                if (dt.Rows[0]["Brut"] != DBNull.Value)
                {
                    returnValue.Brut = (decimal)dt.Rows[0]["Brut"];
                }

                if (dt.Rows[0]["Net"] != DBNull.Value)
                {
                    returnValue.Net = (decimal)dt.Rows[0]["Net"];
                }

                if (dt.Rows[0]["BBNetArea"] != DBNull.Value)
                {
                    returnValue.BBNetArea = (decimal)dt.Rows[0]["BBNetArea"];
                }

                if (dt.Rows[0]["BBGeneralGrossArea"] != DBNull.Value)
                {
                    returnValue.BBGeneralGrossArea = (decimal)dt.Rows[0]["BBGeneralGrossArea"];
                }

                if (dt.Rows[0]["Terrace"] != DBNull.Value)
                {
                    returnValue.Terrace = (decimal)dt.Rows[0]["Terrace"];
                }

                if (dt.Rows[0]["Balcony"] != DBNull.Value)
                {
                    returnValue.Balcony = (decimal)dt.Rows[0]["Balcony"];
                }


                if (dt.Rows[0]["Court"] != DBNull.Value)
                {
                    returnValue.Court = (decimal)dt.Rows[0]["Court"];
                }

                if (dt.Rows[0]["Deck"] != DBNull.Value)
                {
                    returnValue.Deck = (decimal)dt.Rows[0]["Deck"];
                }

                if (dt.Rows[0]["Kitchen"] != DBNull.Value)
                {
                    returnValue.Kitchen = (decimal)dt.Rows[0]["Kitchen"];
                }
                if (dt.Rows[0]["Garden"] != DBNull.Value)
                {
                    returnValue.Garden = (decimal)dt.Rows[0]["Garden"];
                }
                if (dt.Rows[0]["Pool"] != DBNull.Value)
                {
                    returnValue.Pool = (decimal)dt.Rows[0]["Pool"];
                }
                if (dt.Rows[0]["UnitPrice"] != DBNull.Value)
                {
                    returnValue.UnitPrice = (decimal)dt.Rows[0]["UnitPrice"];
                    returnValue.UnitPriceString = ((decimal)dt.Rows[0]["UnitPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["TLPrice"] != DBNull.Value)
                {
                    returnValue.TLPrice = (decimal)dt.Rows[0]["TLPrice"];
                    returnValue.TLPriceString = ((decimal)dt.Rows[0]["TLPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["DolarPrice"] != DBNull.Value)
                {
                    returnValue.DollarPrice = (decimal)dt.Rows[0]["DolarPrice"];
                    returnValue.DollarPriceString = ((decimal)dt.Rows[0]["DolarPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["GeneralWay"] != DBNull.Value)
                {
                    returnValue.DirectionName = dt.Rows[0]["GeneralWay"].ToString();
                }

                if (dt.Rows[0]["GoodsStatus"] != DBNull.Value)
                {
                    returnValue.GoodsStatus = (int)dt.Rows[0]["GoodsStatus"];
                }
                if (dt.Rows[0]["UsedRentalSalesStatus"] != DBNull.Value)
                {
                    returnValue.UsedRentalSalesStatus = (int)dt.Rows[0]["UsedRentalSalesStatus"];
                }

                if (dt.Rows[0]["PaymentOfHire"] != DBNull.Value)
                {
                    returnValue.PaymentOfHire = (decimal)dt.Rows[0]["PaymentOfHire"];
                }
                if (dt.Rows[0]["RentalNotes"] != DBNull.Value)
                {
                    returnValue.RentalNotes = dt.Rows[0]["RentalNotes"].ToString();
                }
                if (dt.Rows[0]["RentalMonths"] != DBNull.Value)
                {
                    returnValue.RentalMonths = (int)dt.Rows[0]["RentalMonths"];
                }

                returnValue.StatusCode = new StringMap()
                {
                    Name = dt.Rows[0]["StatusCodeName"].ToString(),
                    Value = (int)dt.Rows[0]["StatusCode"]
                };
                #endregion
            }

            return returnValue;
        }

        public static Product GetProductDetailForSR(Guid productId, SqlDataAccess sda)
        {
            Product returnValue = new Product();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                p.ProductId
	                                ,p.Name
	                                ,p.new_projectid AS ProjectId
	                                ,p.new_projectidName AS ProjectIdName
	                                ,p.new_blockid AS BlockId
	                                ,p.new_blockidName AS BlockIdName
	                                ,p.new_generaltypeofhomeid AS GeneralHomeTypeId
	                                ,p.new_generaltypeofhomeidName AS GeneralHomeTypeIdName
	                                ,p.new_typeofhomeid AS HomeTypeId
	                                ,p.new_typeofhomeidName AS HomeTypeIdName
	                                ,p.new_locationid AS LocationId
	                                ,p.new_locationidName AS LocationIdName
	                                ,p.new_floornumber AS FloorNumber
	                                ,p.new_homenumber AS HomeNumber
	                                ,p.Price AS HousePrice
	                                ,p.new_KDVratio AS KDV
	                                ,p.new_taxofstamp AS TaxOfStamp
	                                ,p.TransactionCurrencyId
	                                ,p.TransactionCurrencyIdName
                                    ,p.StatusCode
                                    ,p.new_grossm2 Brut
                                    ,p.new_netm2 Net
                                    ,p.defaultuomId UomId
                                    ,p.defaultuomIdName UomIdName
                                    ,sm.Value AS StatusCodeName
                                    ,u.CurrencySymbol
									,p.PriceLevelId
									,p.PriceLevelIdName
                                    ,p.new_aks AS Aks
                                    ,p.ProductNumber
                                    ,P.new_persquaremeter UnitPrice
                                    ,p.ModifiedOn
                                    ,SM1.Value GeneralWay
                                    ,p.new_terracegross Terrace
                                    ,p.new_balconym2 Balcony
                                    ,p.new_kitchenm2 Kitchen
                                    ,P.new_licencenumber LicenceNumber
                                    ,p.new_bbnetarea BBNetArea
                                    ,p.new_bbgeneralgrossarea BBGeneralGrossArea
                                    ,p.new_goodsstatus as GoodsStatus
                                    ,p.new_usedrentalandsalesstatus UsedRentalSalesStatus
                                    ,p.new_paymentofhire PaymentOfHire
                                    ,p.new_rentalnotes RentalNotes
                                    ,p.new_rentalmonths RentalMonths
                                    ,p.new_deckm2 Deck
                                    ,p.new_courtm2 Court
                                    ,prodStatusCode.Value as pStatusName
									,prodStatusCode.AttributeValue as pStatusCode
                                    ,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'USD'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId= p.ProductId
									)DolarPrice
									,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'TL'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId=  p.ProductId
									)TLPrice
									,(
										SELECT
											PR.new_iswithisgyo
										FROM
											new_project PR WITH (NOLOCK)
										WHERE
											PR.new_projectId = P.new_projectid
									)ProjectIsGyo
                                FROM
	                                Product AS p (NOLOCK)
                                        JOIN
                                            StringMap AS sm (NOLOCK)
                                                ON
                                                sm.ObjectTypeCode=1024
                                                AND
                                                sm.AttributeName='statuscode'
                                                AND
                                                sm.AttributeValue=p.StatusCode
                                        JOIN
                                            TransactionCurrency AS u (NOLOCK)
                                                ON
                                                u.TransactionCurrencyId=p.TransactionCurrencyId
                                        LEFT JOIN
                                            StringMap AS SM1 (NOLOCK)
                                                ON
                                                SM1.ObjectTypeCode=1024
                                                AND
                                                SM1.AttributeName='new_generalway'
                                                AND
                                                SM1.AttributeValue= p.new_generalway
                                        LEFT JOIN
		                                    StringMap AS prodStatusCode (NOLOCK)
			                                    ON
			                                    prodStatusCode.ObjectTypeCode=1024
			                                    AND
			                                    prodStatusCode.AttributeName='new_usedrentalandsalesstatus'
			                                    AND
			                                    prodStatusCode.AttributeValue=p.new_usedrentalandsalesstatus
                                            
                                WHERE
	                                p.ProductId=@productId";

            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@productId", productId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                #region | FILL PRODUCT INFO |

                returnValue.ProductId = (Guid)dt.Rows[0]["ProductId"];
                returnValue.Name = dt.Rows[0]["Name"].ToString();
                returnValue.ModifiedOn = (DateTime)dt.Rows[0]["ModifiedOn"];
                returnValue.IsProjectGyo = dt.Rows[0]["ProjectIsGyo"] != DBNull.Value ? (bool)dt.Rows[0]["ProjectIsGyo"] : false;

                if (dt.Rows[0]["ProjectId"] != DBNull.Value)
                {
                    returnValue.Project = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["ProjectId"],
                        Name = dt.Rows[0]["ProjectIdName"].ToString(),
                        LogicalName = "new_project"
                    };
                }

                if (dt.Rows[0]["BlockId"] != DBNull.Value)
                {
                    returnValue.Block = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["BlockId"],
                        Name = dt.Rows[0]["BlockIdName"].ToString(),
                        LogicalName = "new_block"
                    };
                }

                if (dt.Rows[0]["GeneralHomeTypeId"] != DBNull.Value)
                {
                    returnValue.GeneralHomeType = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["GeneralHomeTypeId"],
                        Name = dt.Rows[0]["GeneralHomeTypeIdName"].ToString(),
                        LogicalName = "new_generaltypeofhome"
                    };
                }

                if (dt.Rows[0]["HomeTypeId"] != DBNull.Value)
                {
                    returnValue.HomeType = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["HomeTypeId"],
                        Name = dt.Rows[0]["HomeTypeIdName"].ToString(),
                        LogicalName = "new_typeofhome"
                    };
                }

                if (dt.Rows[0]["LocationId"] != DBNull.Value)
                {
                    returnValue.Location = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["LocationId"],
                        Name = dt.Rows[0]["LocationIdName"].ToString(),
                        LogicalName = "new_location"
                    };
                }

                if (dt.Rows[0]["TransactionCurrencyId"] != DBNull.Value)
                {
                    returnValue.Currency = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["TransactionCurrencyId"],
                        Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(),
                        LogicalName = "transactioncurrency"
                    };
                }

                if (dt.Rows[0]["UomId"] != DBNull.Value)
                {
                    returnValue.Uom = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["UomId"],
                        Name = dt.Rows[0]["UomIdName"].ToString(),
                        LogicalName = "uom"
                    };
                }

                if (dt.Rows[0]["Aks"] != DBNull.Value)
                {
                    returnValue.Aks = (int)dt.Rows[0]["Aks"];
                }

                if (dt.Rows[0]["PriceLevelId"] != DBNull.Value)
                {
                    returnValue.PriceList = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["PriceLevelId"],
                        Name = dt.Rows[0]["PriceLevelIdName"].ToString(),
                        LogicalName = "pricelevel"
                    };
                }

                if (dt.Rows[0]["LicenceNumber"] != DBNull.Value)
                {
                    returnValue.LicenceNumber = dt.Rows[0]["LicenceNumber"].ToString();
                }

                if (dt.Rows[0]["FloorNumber"] != DBNull.Value)
                {
                    returnValue.FloorNumber = (int)dt.Rows[0]["FloorNumber"];
                }

                if (dt.Rows[0]["HomeNumber"] != DBNull.Value)
                {
                    returnValue.HomeNumber = dt.Rows[0]["HomeNumber"].ToString();
                }

                if (dt.Rows[0]["HousePrice"] != DBNull.Value)
                {
                    returnValue.Price = (decimal)dt.Rows[0]["HousePrice"];
                    returnValue.PriceString = ((decimal)dt.Rows[0]["HousePrice"]).ToString("N2", CultureInfo.CurrentCulture) + " " + dt.Rows[0]["CurrencySymbol"].ToString();
                }

                if (dt.Rows[0]["KDV"] != DBNull.Value)
                {
                    returnValue.KdvRatio = (decimal)dt.Rows[0]["KDV"];
                }

                if (dt.Rows[0]["TaxOfStamp"] != DBNull.Value)
                {
                    returnValue.TaxofStampRatio = (decimal)dt.Rows[0]["TaxOfStamp"];
                }

                if (dt.Rows[0]["Brut"] != DBNull.Value)
                {
                    returnValue.Brut = (decimal)dt.Rows[0]["Brut"];
                }

                if (dt.Rows[0]["Net"] != DBNull.Value)
                {
                    returnValue.Net = (decimal)dt.Rows[0]["Net"];
                }

                if (dt.Rows[0]["BBNetArea"] != DBNull.Value)
                {
                    returnValue.BBNetArea = (decimal)dt.Rows[0]["BBNetArea"];
                }

                if (dt.Rows[0]["BBGeneralGrossArea"] != DBNull.Value)
                {
                    returnValue.BBGeneralGrossArea = (decimal)dt.Rows[0]["BBGeneralGrossArea"];
                }

                if (dt.Rows[0]["Terrace"] != DBNull.Value)
                {
                    returnValue.Terrace = (decimal)dt.Rows[0]["Terrace"];
                }

                if (dt.Rows[0]["Balcony"] != DBNull.Value)
                {
                    returnValue.Balcony = (decimal)dt.Rows[0]["Balcony"];
                }


                if (dt.Rows[0]["Court"] != DBNull.Value)
                {
                    returnValue.Court = (decimal)dt.Rows[0]["Court"];
                }

                if (dt.Rows[0]["Deck"] != DBNull.Value)
                {
                    returnValue.Deck = (decimal)dt.Rows[0]["Deck"];
                }

                if (dt.Rows[0]["Kitchen"] != DBNull.Value)
                {
                    returnValue.Kitchen = (decimal)dt.Rows[0]["Kitchen"];
                }

                if (dt.Rows[0]["UnitPrice"] != DBNull.Value)
                {
                    returnValue.UnitPrice = (decimal)dt.Rows[0]["UnitPrice"];
                    returnValue.UnitPriceString = ((decimal)dt.Rows[0]["UnitPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["TLPrice"] != DBNull.Value)
                {
                    returnValue.TLPrice = (decimal)dt.Rows[0]["TLPrice"];
                    returnValue.TLPriceString = ((decimal)dt.Rows[0]["TLPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["DolarPrice"] != DBNull.Value)
                {
                    returnValue.DollarPrice = (decimal)dt.Rows[0]["DolarPrice"];
                    returnValue.DollarPriceString = ((decimal)dt.Rows[0]["DolarPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["GeneralWay"] != DBNull.Value)
                {
                    returnValue.DirectionName = dt.Rows[0]["GeneralWay"].ToString();
                }

                if (dt.Rows[0]["GoodsStatus"] != DBNull.Value)
                {
                    returnValue.GoodsStatus = (int)dt.Rows[0]["GoodsStatus"];
                }
                if (dt.Rows[0]["UsedRentalSalesStatus"] != DBNull.Value)
                {
                    returnValue.UsedRentalSalesStatus = (int)dt.Rows[0]["UsedRentalSalesStatus"];
                }

                if (dt.Rows[0]["PaymentOfHire"] != DBNull.Value)
                {
                    returnValue.PaymentOfHire = (decimal)dt.Rows[0]["PaymentOfHire"];
                }
                if (dt.Rows[0]["RentalNotes"] != DBNull.Value)
                {
                    returnValue.RentalNotes = dt.Rows[0]["RentalNotes"].ToString();
                }
                if (dt.Rows[0]["RentalMonths"] != DBNull.Value)
                {
                    returnValue.RentalMonths = (int)dt.Rows[0]["RentalMonths"];
                }

                if (dt.Rows[0]["pStatusName"] != DBNull.Value)
                {
                    returnValue.RentalStatus = new StringMap()
                    {
                        Name = dt.Rows[0]["pStatusName"].ToString(),
                        Value = (int)dt.Rows[0]["pStatusCode"]
                    };
                }
                returnValue.StatusCode = new StringMap()
                {
                    Name = dt.Rows[0]["StatusCodeName"].ToString(),
                    Value = (int)dt.Rows[0]["StatusCode"]
                };
                #endregion
            }

            return returnValue;
        }

        public static Product GetProductDetailByCurrency(Guid productId, Guid currencyId, SqlDataAccess sda)
        {
            Product returnValue = new Product();

            #region | SQL QUERY |

            string sqlQuery = @"
                                SELECT
	                                p.ProductId
	                                ,p.Name
	                                ,p.new_projectid AS ProjectId
	                                ,p.new_projectidName AS ProjectIdName
	                                ,p.new_blockid AS BlockId
	                                ,p.new_blockidName AS BlockIdName
	                                ,p.new_generaltypeofhomeid AS GeneralHomeTypeId
	                                ,p.new_generaltypeofhomeidName AS GeneralHomeTypeIdName
	                                ,p.new_typeofhomeid AS HomeTypeId
	                                ,p.new_typeofhomeidName AS HomeTypeIdName
	                                ,p.new_locationid AS LocationId
	                                ,p.new_locationidName AS LocationIdName
	                                ,p.new_floornumber AS FloorNumber
	                                ,p.new_homenumber AS HomeNumber
	                                ,ppl.Amount AS HousePrice
	                                ,p.new_KDVratio AS KDV
	                                ,p.new_taxofstamp AS TaxOfStamp
	                                ,pl.TransactionCurrencyId
	                                ,pl.TransactionCurrencyIdName
                                    ,p.StatusCode
                                    ,p.new_grossm2 Brut
                                    ,p.new_netm2 Net
                                    ,p.defaultuomId UomId
                                    ,p.defaultuomIdName UomIdName
                                    ,sm.Value AS StatusCodeName
                                    ,u.CurrencySymbol
									,p.PriceLevelId
									,p.PriceLevelIdName
                                    ,p.new_aks AS Aks
                                    ,p.ProductNumber
                                    ,P.new_persquaremeter UnitPrice
                                    ,p.ModifiedOn
                                    ,SM1.Value GeneralWay
                                    ,p.new_terracegross Terrace
                                    ,p.new_balconym2 Balcony
                                    ,p.new_kitchenm2 Kitchen
                                    ,P.new_licencenumber LicenceNumber
                                    ,P.new_poolm2 pool
                                    ,P.new_garden garden
                                    ,SMW.AttributeValue as RentalStatusCode
									,SMW.Value as RentalStatusName
                                    ,p.new_blocktypeidName AS BlockTypeName
                                    ,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'USD'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId= p.ProductId
									)DolarPrice
									,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'TL'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId=  p.ProductId
									)TLPrice
                                    ,(
										SELECT	
											ppl.Amount AS HousePrice
										FROM
										PriceLevel AS pl (NOLOCK)
										JOIN
											ProductPriceLevel AS ppl (NOLOCK)
												ON
												pl.TransactionCurrencyIdName = 'Euro'
												AND
												ppl.PriceLevelId=pl.PriceLevelId
												AND
												ppl.ProductId=  p.ProductId
									)EuroPrice
                                    ,(
										SELECT
											PR.new_iswithisgyo
										FROM
											new_project PR WITH (NOLOCK)
										WHERE
											PR.new_projectId = P.new_projectid
									)ProjectIsGyo
                                FROM
	                                Product AS p (NOLOCK)
                                        JOIN
			                                PriceLevel AS pl (NOLOCK)
				                                ON
				                                pl.TransactionCurrencyId= @Currency
		                                JOIN
			                                ProductPriceLevel AS ppl (NOLOCK)
				                                ON
				                                ppl.PriceLevelId=pl.PriceLevelId
				                                AND
				                                ppl.ProductId=p.ProductId
                                        JOIN
                                            StringMap AS sm (NOLOCK)
                                                ON
                                                sm.ObjectTypeCode=1024
                                                AND
                                                sm.AttributeName='statuscode'
                                                AND
                                                sm.AttributeValue=p.StatusCode
                                        JOIN
                                            TransactionCurrency AS u (NOLOCK)
                                                ON
                                                u.TransactionCurrencyId=p.TransactionCurrencyId 
                                        LEFT JOIN
                                            StringMap AS SM1 (NOLOCK)
                                                ON
                                                SM1.ObjectTypeCode=1024
                                                AND
                                                SM1.AttributeName='new_generalway'
                                                AND
                                                SM1.AttributeValue= p.new_generalway
                                        LEFT JOIN
                                            StringMap AS SMW (NOLOCK)
                                                ON
                                                SMW.ObjectTypeCode=1024
                                                AND
                                                SMW.AttributeName='new_usedrentalandsalesstatus'
                                                AND
                                                SMW.AttributeValue=p.new_usedrentalandsalesstatus     
                                            WHERE
	                                            p.ProductId=@productId";

            #endregion

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@productId", productId), new SqlParameter("@Currency", currencyId) };

            DataTable dt = sda.getDataTable(sqlQuery, parameters);

            if (dt.Rows.Count > 0)
            {
                #region | FILL PRODUCT INFO |

                returnValue.ProductId = (Guid)dt.Rows[0]["ProductId"];
                returnValue.Name = dt.Rows[0]["Name"].ToString();
                returnValue.ModifiedOn = (DateTime)dt.Rows[0]["ModifiedOn"];
                returnValue.IsProjectGyo = dt.Rows[0]["ProjectIsGyo"] != DBNull.Value ? (bool)dt.Rows[0]["ProjectIsGyo"] : false;

                if (dt.Rows[0]["ProjectId"] != DBNull.Value)
                {
                    returnValue.Project = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["ProjectId"],
                        Name = dt.Rows[0]["ProjectIdName"].ToString(),
                        LogicalName = "new_project"
                    };
                }

                if (dt.Rows[0]["BlockId"] != DBNull.Value)
                {
                    returnValue.Block = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["BlockId"],
                        Name = dt.Rows[0]["BlockIdName"].ToString(),
                        LogicalName = "new_block"
                    };
                }

                if (dt.Rows[0]["GeneralHomeTypeId"] != DBNull.Value)
                {
                    returnValue.GeneralHomeType = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["GeneralHomeTypeId"],
                        Name = dt.Rows[0]["GeneralHomeTypeIdName"].ToString(),
                        LogicalName = "new_generaltypeofhome"
                    };
                }

                if (dt.Rows[0]["HomeTypeId"] != DBNull.Value)
                {
                    returnValue.HomeType = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["HomeTypeId"],
                        Name = dt.Rows[0]["HomeTypeIdName"].ToString(),
                        LogicalName = "new_typeofhome"
                    };
                }

                if (dt.Rows[0]["LocationId"] != DBNull.Value)
                {
                    returnValue.Location = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["LocationId"],
                        Name = dt.Rows[0]["LocationIdName"].ToString(),
                        LogicalName = "new_location"
                    };
                }

                if (dt.Rows[0]["TransactionCurrencyId"] != DBNull.Value)
                {
                    returnValue.Currency = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["TransactionCurrencyId"],
                        Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(),
                        LogicalName = "transactioncurrency"
                    };
                }

                if (dt.Rows[0]["UomId"] != DBNull.Value)
                {
                    returnValue.Uom = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["UomId"],
                        Name = dt.Rows[0]["UomIdName"].ToString(),
                        LogicalName = "uom"
                    };
                }

                if (dt.Rows[0]["Aks"] != DBNull.Value)
                {
                    returnValue.Aks = (int)dt.Rows[0]["Aks"];
                }

                if (dt.Rows[0]["PriceLevelId"] != DBNull.Value)
                {
                    returnValue.PriceList = new EntityReference()
                    {
                        Id = (Guid)dt.Rows[0]["PriceLevelId"],
                        Name = dt.Rows[0]["PriceLevelIdName"].ToString(),
                        LogicalName = "pricelevel"
                    };
                }

                if (dt.Rows[0]["LicenceNumber"] != DBNull.Value)
                {
                    returnValue.LicenceNumber = dt.Rows[0]["LicenceNumber"].ToString();
                }

                if (dt.Rows[0]["FloorNumber"] != DBNull.Value)
                {
                    returnValue.FloorNumber = (int)dt.Rows[0]["FloorNumber"];
                }

                if (dt.Rows[0]["HomeNumber"] != DBNull.Value)
                {
                    returnValue.HomeNumber = dt.Rows[0]["HomeNumber"].ToString();
                }
                if (dt.Rows[0]["garden"] != DBNull.Value)
                {
                    returnValue.Garden = (decimal)dt.Rows[0]["garden"];
                }

                if (dt.Rows[0]["HousePrice"] != DBNull.Value)
                {
                    returnValue.Price = (decimal)dt.Rows[0]["HousePrice"];
                    returnValue.PriceString = ((decimal)dt.Rows[0]["HousePrice"]).ToString("N2", CultureInfo.CurrentCulture) + " " + dt.Rows[0]["TransactionCurrencyIdName"].ToString();
                }

                if (dt.Rows[0]["pool"] != DBNull.Value)
                {
                    returnValue.Pool = (decimal)dt.Rows[0]["pool"];
                }

                if (dt.Rows[0]["KDV"] != DBNull.Value)
                {
                    returnValue.KdvRatio = (decimal)dt.Rows[0]["KDV"];
                }

                if (dt.Rows[0]["TaxOfStamp"] != DBNull.Value)
                {
                    returnValue.TaxofStampRatio = (decimal)dt.Rows[0]["TaxOfStamp"];
                }

                if (dt.Rows[0]["Brut"] != DBNull.Value)
                {
                    returnValue.Brut = (decimal)dt.Rows[0]["Brut"];
                }

                if (dt.Rows[0]["Net"] != DBNull.Value)
                {
                    returnValue.Net = (decimal)dt.Rows[0]["Net"];
                }

                if (dt.Rows[0]["Terrace"] != DBNull.Value)
                {
                    returnValue.Terrace = (decimal)dt.Rows[0]["Terrace"];
                }

                if (dt.Rows[0]["Balcony"] != DBNull.Value)
                {
                    returnValue.Balcony = (decimal)dt.Rows[0]["Balcony"];
                }

                if (dt.Rows[0]["Kitchen"] != DBNull.Value)
                {
                    returnValue.Kitchen = (decimal)dt.Rows[0]["Kitchen"];
                }

                if (dt.Rows[0]["UnitPrice"] != DBNull.Value)
                {
                    returnValue.UnitPrice = (decimal)dt.Rows[0]["UnitPrice"];
                    returnValue.UnitPriceString = ((decimal)dt.Rows[0]["UnitPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["TLPrice"] != DBNull.Value)
                {
                    returnValue.TLPrice = (decimal)dt.Rows[0]["TLPrice"];
                    returnValue.TLPriceString = ((decimal)dt.Rows[0]["TLPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["EuroPrice"] != DBNull.Value)
                {
                    returnValue.EuroPrice = (decimal)dt.Rows[0]["EuroPrice"];
                    returnValue.EuroPriceString = ((decimal)dt.Rows[0]["EuroPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["DolarPrice"] != DBNull.Value)
                {
                    returnValue.DollarPrice = (decimal)dt.Rows[0]["DolarPrice"];
                    returnValue.DollarPriceString = ((decimal)dt.Rows[0]["DolarPrice"]).ToString("N0", CultureInfo.CurrentCulture);
                }

                if (dt.Rows[0]["GeneralWay"] != DBNull.Value)
                {
                    returnValue.DirectionName = dt.Rows[0]["GeneralWay"].ToString();
                }

                if (dt.Rows[0]["RentalStatusName"] != DBNull.Value)
                {
                    returnValue.RentalStatus = new StringMap()
                    {
                        Name = dt.Rows[0]["RentalStatusName"].ToString(),
                        Value = (int)dt.Rows[0]["RentalStatusCode"]
                    };
                }
                if (dt.Rows[0]["StatusCodeName"] != DBNull.Value)
                {
                    returnValue.StatusCode = new StringMap()
                    {
                        Name = dt.Rows[0]["StatusCodeName"].ToString(),
                        Value = (int)dt.Rows[0]["StatusCode"]
                    };
                }

                if (dt.Rows[0]["BlockTypeName"] != DBNull.Value)
                {
                    returnValue.BlockTypeName = dt.Rows[0]["BlockTypeName"].ToString();
                }
                #endregion
            }

            return returnValue;
        }

        public static MsCrmResult UpdateHouseStatus(Guid productId, ProductStatuses status, IOrganizationService service, DateTime? optionDate = null)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                Entity proc = new Entity("product");
                proc["productid"] = productId;
                proc["statuscode"] = new OptionSetValue((int)status);

                if (status == ProductStatuses.YoneticiOpsiyonlu)
                {
                    proc["new_optionlastvaliditydate"] = (DateTime)optionDate;
                }

                service.Update(proc);

                returnValue.Success = true;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        #region | HOUSE IMPORT FUNCTIONS |

        public static List<HouseImport> GetNotImportedList(SqlDataAccess sda)
        {
            List<HouseImport> lst = new List<HouseImport>();
            try
            {
                #region |HOUSE IMPORT QUERY|
                string sqlQuery = @"SELECT
	                                    si.new_name AS Name
	                                    ,si.OwnerId
	                                    ,si.new_houseimportId AS Id
                                    FROM
	                                    new_houseimport AS si (NOLOCK)
                                    WHERE
	                                    si.statecode=0
                                    AND
	                                    si.statuscode=1";
                #endregion

                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        HouseImport ri = new HouseImport()
                        {
                            Name = dt.Rows[i]["Name"].ToString(),
                            OwnerId = (Guid)dt.Rows[i]["OwnerId"],
                            HouseImportId = (Guid)dt.Rows[i]["Id"]
                        };

                        lst.Add(ri);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return lst;
        }

        public static MsCrmResult UpdateImportListToProcessing(Guid houseImportId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("new_houseimport");
                ent["new_houseimportid"] = houseImportId;
                ent["statuscode"] = new OptionSetValue(100000000); //Devam ediyor

                service.Update(ent);

                returnValue.Success = true;
                returnValue.Result = "Kayıt Devam ediliyora çekildi.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static DataTable GetHouseImportListDetail(Guid houseImportId, SqlDataAccess sda)
        {
            DataTable dt_res = new DataTable();

            #region |SQL QUERY|
            string sqlQuery = @"SELECT
	                                si.new_houseimportId AS HouseImportId
	                                ,an.DocumentBody
                                FROM
                                new_houseimport AS si (NOLOCK)
	                                JOIN
		                                Annotation AS an (NOLOCK)
			                                ON
			                                si.new_houseimportId=an.ObjectId
			                                AND
			                                an.FileName LIKE '%csv%'
			                                AND
			                                an.DocumentBody IS NOT NULL
                                WHERE
	                                si.new_houseimportId='{0}'";
            #endregion

            try
            {
                DataTable dt = sda.getDataTable(string.Format(sqlQuery, houseImportId.ToString()));

                if (dt.Rows.Count > 0)
                {

                    CsvParse pars = new CsvParser.CsvParse(dt.Rows[0]["DocumentBody"].ToString(), CsvParser.CsvParse.delimiter.NoktalıVirgül);
                    dt_res = pars.GetDataTableFromCSV();
                }
            }
            catch (Exception ex)
            {

            }

            return dt_res;
        }

        public static void UpdateHouseImportResultsAndClose(Guid houseImportId, int errorCount, int successCount, int totalCount, IOrganizationService service)
        {
            try
            {
                Entity ent = new Entity("new_houseimport");
                ent["new_houseimportid"] = houseImportId;
                ent["new_faultcount"] = errorCount;
                ent["new_successcount"] = successCount;
                ent["new_totalcount"] = totalCount;

                service.Update(ent);

                SetStateRequest stateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference("new_houseimport", houseImportId),
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(2)
                };

                SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);
            }
            catch (Exception ex)
            {

            }

        }

        public static MsCrmResult CheckProductExists(Guid productId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                p.ProductId
                                FROM
	                                Product AS p (NOLOCK)
                                WHERE
	                                p.Name='{0}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, productId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Konut içeride tanımlıdır";

                    returnValue.CrmId = (Guid)dt.Rows[0]["ProductId"];
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResult CheckBlockOfBuildingExists(string blockOfBuildingName, Guid threaderId, Guid projectId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    bob.new_blockofbuildingId AS Id
                                    FROM
	                                    new_blockofbuilding AS bob (NOLOCK)
                                    WHERE
	                                    bob.new_name='{0}'
                                    AND
                                    	bob.new_threaderid='{1}'    
                                    AND
	                                    bob.new_projectid='{2}'
                                    AND
	                                    bob.statecode=0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, blockOfBuildingName, threaderId, projectId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Ada içeride tanımlıdır";

                    returnValue.CrmId = (Guid)dt.Rows[0]["Id"];
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResult CreateBlockOfBuilding(string name, Guid threaderId, Guid projectId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("new_blockofbuilding");
                ent["new_name"] = name;
                ent["new_threaderid"] = new EntityReference("new_threader", threaderId);
                ent["new_projectid"] = new EntityReference("new_project", projectId);

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Ada kaydı oluşturuldu.";

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CheckParcelExists(string parcelName, Guid blockOfBuildingId, Guid projectId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    bob.new_parcelId AS Id
                                    FROM
	                                    new_parcel AS bob (NOLOCK)
                                    WHERE
	                                    bob.new_name='{0}'
                                    AND
                                    	bob.new_blockofbuildingid='{1}'    
                                    AND
	                                    bob.new_projectid='{2}'
                                    AND
	                                    bob.statecode=0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, parcelName, blockOfBuildingId, projectId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Parsel içeride tanımlıdır";

                    returnValue.CrmId = (Guid)dt.Rows[0]["Id"];
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResult CreateParcel(string name, Guid blockOfBuildingId, Guid projectId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("new_parcel");
                ent["new_name"] = name;
                ent["new_blockofbuildingid"] = new EntityReference("new_blockofbuilding", blockOfBuildingId);
                ent["new_projectid"] = new EntityReference("new_project", projectId);

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Parsel kaydı oluşturuldu.";

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult GetPriceListIdByCurrencySymbol(string currencySymbol, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                curr.CurrencyName
	                                ,pl.PriceLevelId
                                    ,curr.TransactionCurrencyId
                                FROM
                                TransactionCurrency AS curr (NOLOCK)
	                                JOIN
		                                PriceLevel AS pl (NOLOCK)
			                                ON
			                                curr.TransactionCurrencyId=pl.TransactionCurrencyId
                                WHERE
                                curr.CurrencyName='{0}'
                                AND
                                curr.StateCode=0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, currencySymbol));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.CrmId = (Guid)dt.Rows[0]["PriceLevelId"];
                    returnValue.Result = dt.Rows[0]["TransactionCurrencyId"].ToString();
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CheckProductHasPriceLevel(Guid priceLevelId, Guid productId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                ppl.ProductPriceLevelId
	                                ,ppl.ProductIdName
                                FROM
	                                ProductPriceLevel AS ppl (NOLOCK)
                                WHERE
                                ppl.PriceLevelId='{0}'
                                AND
                                ppl.ProductId='{1}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, priceLevelId, productId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.CrmId = (Guid)dt.Rows[0]["ProductPriceLevelId"];
                    returnValue.Success = true;
                    returnValue.Result = "Fiyat listesi kalemi vardır.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreateOrUpdateProductPriceLevel(ProductPriceLevel productPriceLevel, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("productpricelevel");

                if (productPriceLevel.PriceLevel != null && productPriceLevel.PriceLevel.Id != Guid.Empty)
                    ent["pricelevelid"] = productPriceLevel.PriceLevel;

                if (productPriceLevel.Product != null && productPriceLevel.Product.Id != Guid.Empty)
                    ent["productid"] = productPriceLevel.Product;

                if (productPriceLevel.Currency != null && productPriceLevel.Currency.Id != Guid.Empty)
                    ent["transactioncurrencyid"] = productPriceLevel.Currency;
                if (productPriceLevel.Price != 0)
                    ent["amount"] = new Money(productPriceLevel.Price);

                ent["uomid"] = new EntityReference("uom", Globals.DefaultUoMId);
                ent["quantitysellingcode"] = new OptionSetValue(1); //Denetim yok
                ent["pricingmethodcode"] = new OptionSetValue(1); //Para birimi tutarı

                if (productPriceLevel.Id != null && productPriceLevel.Id != Guid.Empty)
                {
                    ent["productpricelevelid"] = productPriceLevel.Id;

                    service.Update(ent);

                    returnValue.CrmId = productPriceLevel.Id;
                    returnValue.Success = true;
                    returnValue.Result = "Fiyat listesi kalemi güncellendi.";
                }
                else
                {
                    returnValue.CrmId = service.Create(ent);
                    returnValue.Success = true;
                    returnValue.Result = "Fiyat listesi kalemi oluşturuldu.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetActivePriceLists(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                pl.Name
	                                ,pl.PriceLevelId
	                                ,pl.TransactionCurrencyId
	                                ,pl.TransactionCurrencyIdName
                                FROM
	                                PriceLevel AS pl (NOLOCK)
                                WHERE
	                                pl.StateCode=0";

                #endregion

                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    List<PriceLevel> lstPl = new List<PriceLevel>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        PriceLevel pl = new PriceLevel()
                        {
                            Id = (Guid)dt.Rows[i]["PriceLevelId"],
                            Name = dt.Rows[i]["Name"].ToString(),
                            Currency = new EntityReference()
                            {
                                Id = (Guid)dt.Rows[i]["TransactionCurrencyId"],
                                Name = dt.Rows[i]["TransactionCurrencyIdName"].ToString(),
                                LogicalName = "transactioncurrency"
                            }
                        };

                        lstPl.Add(pl);
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = lstPl;
                    returnValue.Result = "Fiyat listeleri alındı.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResult CreateProductPriceLists(decimal price, EntityReference currency, Guid productId, SqlDataAccess sda, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                EntityReference priceLevel = null;
                MsCrmResultObject resultPriceLevel = ProductHelper.GetPriceLevelByCurrencyId(currency.Id, sda);

                if (resultPriceLevel.Success)
                {
                    PriceLevel pl = (PriceLevel)resultPriceLevel.ReturnObject;

                    priceLevel = new EntityReference()
                    {
                        Id = pl.Id,
                        Name = pl.Name,
                        LogicalName = "pricelevel"
                    };
                }

                MsCrmResultObject resultPriceLists = ProductHelper.GetActivePriceLists(sda);

                if (resultPriceLists.Success)
                {
                    List<PriceLevel> lstPriceLists = (List<PriceLevel>)resultPriceLists.ReturnObject;

                    if (lstPriceLists.Count > 0)
                    {
                        for (int i = 0; i < lstPriceLists.Count; i++)
                        {
                            ProductPriceLevel ppl = new ProductPriceLevel()
                            {
                                Currency = lstPriceLists[i].Currency,
                                Product = new EntityReference("product", productId),
                                PriceLevel = new EntityReference("pricelevel", lstPriceLists[i].Id)
                            };

                            if (lstPriceLists[i].Id == priceLevel.Id)
                            {
                                ppl.Price = price;
                            }
                            else
                            {
                                MsCrmResultObject resultConvertion = CurrencyHelper.ConvertCurrency(currency.Id, price, lstPriceLists[i].Currency.Id, DateTime.Now, sda);

                                if (resultConvertion.Success)
                                {
                                    ppl.Price = (decimal)resultConvertion.ReturnObject;
                                }
                                else
                                {
                                    returnValue.Success = false;
                                    returnValue.Result = "Döviz kuru bilgisi tanımlı değil.";

                                    return returnValue;
                                }
                            }

                            ProductHelper.CreateOrUpdateProductPriceLevel(ppl, service);
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Fiyat listeleri tanımlı değildir.";

                    return returnValue;
                }

                returnValue.Success = true;
            }
            catch (Exception ex)
            {

                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResult UpdateProductPriceLists(decimal price, EntityReference currency, EntityReference priceLevel, Guid productId, SqlDataAccess sda, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                MsCrmResultObject resultPriceLists = ProductHelper.GetActivePriceLists(sda);

                if (resultPriceLists.Success)
                {
                    List<PriceLevel> lstPriceLists = (List<PriceLevel>)resultPriceLists.ReturnObject;

                    if (lstPriceLists.Count > 0)
                    {
                        for (int i = 0; i < lstPriceLists.Count; i++)
                        {
                            MsCrmResultObject resultPpl = ProductHelper.GetProductPriceLevel(lstPriceLists[i].Id, productId, sda);

                            if (resultPpl.Success)
                            {
                                ProductPriceLevel ppl = (ProductPriceLevel)resultPpl.ReturnObject;

                                //if (lstPriceLists[i].Id == priceLevel.Id)
                                if (lstPriceLists[i].Currency.Id == currency.Id)
                                {
                                    ppl.Price = price;
                                }
                                else
                                {
                                    MsCrmResultObject resultConvertion = CurrencyHelper.ConvertCurrency(currency.Id, price, lstPriceLists[i].Currency.Id, DateTime.Now, sda);

                                    if (resultConvertion.Success)
                                    {
                                        ppl.Price = (decimal)resultConvertion.ReturnObject;
                                    }
                                    else
                                    {
                                        returnValue.Success = false;
                                        returnValue.Result = "Döviz kuru bilgisi tanımlı değil.";

                                        return returnValue;
                                    }
                                }

                                ProductHelper.CreateOrUpdateProductPriceLevel(ppl, service);
                            }
                        }
                    }
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Fiyat listeleri tanımlı değildir.";

                    return returnValue;
                }

                returnValue.Success = true;
            }
            catch (Exception ex)
            {

                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResultObject GetProductPriceLevel(Guid priceLevelId, Guid productId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                ppl.ProductPriceLevelId
	                                ,ppl.PriceLevelId
	                                ,ppl.PriceLevelIdName
	                                ,ppl.ProductId
	                                ,ppl.ProductIdName
	                                ,ppl.TransactionCurrencyId
	                                ,ppl.TransactionCurrencyIdName
	                                ,ppl.Amount
                                FROM
                                ProductPriceLevel AS ppl (NOLOCK)
                                WHERE
	                                ppl.PriceLevelId='{0}'
                                AND
	                                ppl.ProductId='{1}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, priceLevelId, productId));

                if (dt.Rows.Count > 0)
                {
                    ProductPriceLevel ppl = new ProductPriceLevel()
                    {
                        Id = (Guid)dt.Rows[0]["ProductPriceLevelId"],
                        Currency = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["TransactionCurrencyId"],
                            Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(),
                            LogicalName = "transactioncurrency"
                        },
                        PriceLevel = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["PriceLevelId"],
                            Name = dt.Rows[0]["PriceLevelIdName"].ToString(),
                            LogicalName = "pricelevel"
                        },
                        Product = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["ProductId"],
                            Name = dt.Rows[0]["ProductIdName"].ToString(),
                            LogicalName = "product"
                        },
                        Price = (decimal)dt.Rows[0]["Amount"]

                    };

                    returnValue.ReturnObject = ppl;
                    returnValue.Success = true;
                    returnValue.Result = "Fiyat listesi kalemi çekildi.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetPriceLevelByCurrencyId(Guid currencyId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                pl.Name
	                                ,pl.PriceLevelId
	                                ,pl.TransactionCurrencyId
	                                ,pl.TransactionCurrencyIdName
                                FROM
	                                PriceLevel AS pl (NOLOCK)
                                WHERE
	                                pl.TransactionCurrencyId='{0}' AND pl.StateCode=0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, currencyId));

                if (dt.Rows.Count > 0)
                {
                    PriceLevel pl = new PriceLevel()
                    {
                        Id = (Guid)dt.Rows[0]["PriceLevelId"],
                        Name = dt.Rows[0]["Name"].ToString(),
                        Currency = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["TransactionCurrencyId"],
                            Name = dt.Rows[0]["TransactionCurrencyIdName"].ToString(),
                            LogicalName = "transactioncurrency"
                        }
                    };

                    returnValue.Success = true;
                    returnValue.ReturnObject = pl;
                    returnValue.Result = "Fiyat listesi alındı.";
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        #endregion

        public static void SetPerSquareMeter(Entity entity)
        {
            if (entity.Contains("price") && entity.Contains("new_grossm2"))
            {
                decimal price = ((Money)entity.Attributes["price"]).Value;
                decimal grossM2 = (decimal)entity.Attributes["new_grossm2"];
                decimal perSquareMeter = price / grossM2;
                entity.Attributes.Add("new_persquaremeter", new Money(perSquareMeter));
            }
        }

        public static void SetPerSquareMeter(Entity postImage, IOrganizationService service)
        {
            if (postImage.Contains("price") && postImage.Contains("new_grossm2"))
            {
                decimal price = ((Money)postImage.Attributes["price"]).Value;
                decimal grossM2 = (decimal)postImage.Attributes["new_grossm2"];
                decimal perSquareMeter = price / grossM2;
                Entity product = new Entity("product");
                product.Id = postImage.Id;
                product.Attributes["new_persquaremeter"] = new Money(perSquareMeter);
                service.Update(product);
            }
        }

        public static void SetLogoTransmission(Entity entity, IOrganizationService service)
        {
            if (entity.Contains("new_islogointegration") && entity["new_islogointegration"] != null)
            {
                entity.Attributes["new_islogointegration"] = (bool)entity["new_islogointegration"];
            }
            else if (entity.Contains("name") || entity.Contains("productnumber") ||
                entity.Contains("price") || entity.Contains("transactioncurrencyid") ||
                entity.Contains("new_blockid") || entity.Contains("new_homenumber") ||
                entity.Contains("new_floornumber") || entity.Contains("new_aks")
                 || entity.Contains("new_netm2") || entity.Contains("new_grossm2")
                || entity.Contains("statuscode"))
            {
                entity.Attributes["new_islogointegration"] = true;


            }

        }

        public static MsCrmResultObject GetProductSByStatus(ProductStatuses status, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
	                                P.StatusCode = {0}";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)status));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);


                        returnList.Add(_product);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu statüde ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetRentalProductSByStatus(ProductStatuses status, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
	                                P.new_usedrentalandsalesstatus = {0}";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)status));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);


                        returnList.Add(_product);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu statüde ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetOutOfOptionProducts(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                FROM
	                                Product P WITH (NOLOCK)
                                WHERE
	                                P.StatusCode IN({0},{1}) AND P.new_optionlastvaliditydate<GETUTCDATE()";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)ProductStatuses.Opsiyonlu, (int)ProductStatuses.YoneticiOpsiyonlu));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PRODUCTS |
                    List<Product> returnList = new List<Product>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Product _product = ProductHelper.GetProductDetail((Guid)dt.Rows[i]["ProductId"], sda);


                        returnList.Add(_product);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Bu statüde ürün bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static QuoteControlSetting GetControlSettingByProject(Guid projectId, SqlDataAccess sda)
        {
            QuoteControlSetting returnValue = new QuoteControlSetting();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TOP 1
	                                SCR.new_salescontrollruleId Id
	                                ,SCR.new_name Name
	                                ,SCR.new_project ProjectId
	                                ,SCR.new_projectName ProjectIdName
	                                ,SCR.transactioncurrencyId CurrencyId
	                                ,SCR.transactioncurrencyIdName CurrencyIdName
	                                ,SCR.new_salesconsultantdiscountrate ConsultantRate
	                                ,SCR.new_salesconsultantoptionday ConsultantOptionDay
	                                ,SCR.new_salesconsultantpersquaremeter ConsultantUnitPrice
	                                ,SCR.new_salesmanagerdiscountrate ManagerRate
	                                ,SCR.new_salesmanageroptionday ManagerOptionDay
	                                ,SCR.new_salesmanagerpersquaremeter ManagerUnitPrice
	                                ,SCR.new_salesdirectordiscountrate DirectorRate
	                                ,SCR.new_salesdirectoroptionday DirectorOptionDay
	                                ,SCR.new_salesdirectorpersquaremeter DirectorUnitPrice
                                FROM
	                                new_salescontrollrule SCR WITH (NOLOCK)
                                WHERE
	                                SCR.new_project = '{0}'
	                                AND
	                                SCR.StateCode = 0
                                ORDER BY
	                                SCR.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.QuoteControlSettingId = (Guid)dt.Rows[0]["Id"];
                    returnValue.Name = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;
                    if (dt.Rows[0]["ProjectId"] != DBNull.Value) { returnValue.Project = new EntityReference() { LogicalName = "new_project", Id = (Guid)dt.Rows[0]["ProjectId"], Name = dt.Rows[0]["ProjectIdName"].ToString() }; }
                    if (dt.Rows[0]["CurrencyId"] != DBNull.Value) { returnValue.Project = new EntityReference() { LogicalName = "transactioncurrency", Id = (Guid)dt.Rows[0]["CurrencyId"], Name = dt.Rows[0]["CurrencyIdName"].ToString() }; }

                    if (dt.Rows[0]["ConsultantRate"] != DBNull.Value) { returnValue.ConsultantRate = (decimal)dt.Rows[0]["ConsultantRate"]; }
                    if (dt.Rows[0]["ConsultantOptionDay"] != DBNull.Value) { returnValue.ConsultantOptionDay = (int)dt.Rows[0]["ConsultantOptionDay"]; }
                    if (dt.Rows[0]["ConsultantUnitPrice"] != DBNull.Value) { returnValue.ConsultantUnitPrice = (decimal)dt.Rows[0]["ConsultantUnitPrice"]; }

                    if (dt.Rows[0]["ManagerRate"] != DBNull.Value) { returnValue.ManagerRate = (decimal)dt.Rows[0]["ManagerRate"]; }
                    if (dt.Rows[0]["ManagerOptionDay"] != DBNull.Value) { returnValue.ManagerOptionDay = (int)dt.Rows[0]["ManagerOptionDay"]; }
                    if (dt.Rows[0]["ManagerUnitPrice"] != DBNull.Value) { returnValue.ManagerUnitPrice = (decimal)dt.Rows[0]["ManagerUnitPrice"]; }

                    if (dt.Rows[0]["DirectorRate"] != DBNull.Value) { returnValue.DirectorRate = (decimal)dt.Rows[0]["DirectorRate"]; }
                    if (dt.Rows[0]["DirectorOptionDay"] != DBNull.Value) { returnValue.DirectorOptionDay = (int)dt.Rows[0]["DirectorOptionDay"]; }
                    if (dt.Rows[0]["DirectorUnitPrice"] != DBNull.Value) { returnValue.DirectorUnitPrice = (decimal)dt.Rows[0]["DirectorUnitPrice"]; }
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        public static RentalControlSetting GetRentalControlSettingByProject(Guid projectId, SqlDataAccess sda)
        {
            RentalControlSetting returnValue = new RentalControlSetting();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TOP 1
	                                SCR.new_salescontrollruleId Id
	                                ,SCR.new_name Name
	                                ,SCR.new_project ProjectId
	                                ,SCR.new_projectName ProjectIdName
	                                ,SCR.transactioncurrencyId CurrencyId
	                                ,SCR.transactioncurrencyIdName CurrencyIdName
	                                ,SCR.new_rentalsalesdiscountrate ConsultantRate	                                
	                                ,SCR.new_rentaldirectordiscountrate DirectorRate
	                                
	                                
                                FROM
	                                new_salescontrollrule SCR WITH (NOLOCK)
                                WHERE
	                                SCR.new_project = '{0}'
	                                AND
	                                SCR.StateCode = 0
                                ORDER BY
	                                SCR.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.RentalControlSettingId = (Guid)dt.Rows[0]["Id"];
                    returnValue.Name = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;
                    if (dt.Rows[0]["ProjectId"] != DBNull.Value) { returnValue.Project = new EntityReference() { LogicalName = "new_project", Id = (Guid)dt.Rows[0]["ProjectId"], Name = dt.Rows[0]["ProjectIdName"].ToString() }; }
                    if (dt.Rows[0]["CurrencyId"] != DBNull.Value) { returnValue.Project = new EntityReference() { LogicalName = "transactioncurrency", Id = (Guid)dt.Rows[0]["CurrencyId"], Name = dt.Rows[0]["CurrencyIdName"].ToString() }; }

                    if (dt.Rows[0]["ConsultantRate"] != DBNull.Value) { returnValue.ConsultantRate = (decimal)dt.Rows[0]["ConsultantRate"]; }

                    if (dt.Rows[0]["DirectorRate"] != DBNull.Value) { returnValue.DirectorRate = (decimal)dt.Rows[0]["DirectorRate"]; }
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }


        public static SecondHandControlSetting GetSecondHandControlSettingByProject(Guid projectId, SqlDataAccess sda)
        {
            SecondHandControlSetting returnValue = new SecondHandControlSetting();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TOP 1
	                                SCR.new_salescontrollruleId Id
	                                ,SCR.new_name Name
	                                ,SCR.new_project ProjectId
	                                ,SCR.new_projectName ProjectIdName
	                                ,SCR.transactioncurrencyId CurrencyId
	                                ,SCR.transactioncurrencyIdName CurrencyIdName
	                                ,SCR.new_secondhandsalesdiscountrate ConsultantRate	                                
	                                ,SCR.new_secondhanddirectordiscountrate DirectorRate
                                FROM
	                                new_salescontrollrule SCR WITH (NOLOCK)
                                WHERE
	                                SCR.new_project = '{0}'
	                                AND
	                                SCR.StateCode = 0
                                ORDER BY
	                                SCR.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, projectId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.SecondHandControlSettingId = (Guid)dt.Rows[0]["Id"];
                    returnValue.Name = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : string.Empty;
                    if (dt.Rows[0]["ProjectId"] != DBNull.Value) { returnValue.Project = new EntityReference() { LogicalName = "new_project", Id = (Guid)dt.Rows[0]["ProjectId"], Name = dt.Rows[0]["ProjectIdName"].ToString() }; }
                    if (dt.Rows[0]["CurrencyId"] != DBNull.Value) { returnValue.Project = new EntityReference() { LogicalName = "transactioncurrency", Id = (Guid)dt.Rows[0]["CurrencyId"], Name = dt.Rows[0]["CurrencyIdName"].ToString() }; }
                    if (dt.Rows[0]["ConsultantRate"] != DBNull.Value) { returnValue.ConsultantRate = (decimal)dt.Rows[0]["ConsultantRate"]; }
                    if (dt.Rows[0]["DirectorRate"] != DBNull.Value) { returnValue.DirectorRate = (decimal)dt.Rows[0]["DirectorRate"]; }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        public static List<Guid> GetOutOfDateOptions(SqlDataAccess sda)
        {
            List<Guid> returnValue = new List<Guid>();
            try
            {
                #region | SQL QUERY |
                string query = @"DECLARE @Date DATETIME = GETUTCDATE()
                                SELECT
	                                OH.new_optionofhomeId Id
                                FROM
	                                new_optionofhome OH WITH (NOLOCK)
                                WHERE
	                                OH.StateCode = 0
                                    AND
	                                OH.new_optiondate < @Date";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        returnValue.Add((Guid)dt.Rows[i]["Id"]);
                    }
                }
            }
            catch (Exception)
            {

            }

            return returnValue;
        }

        public static MsCrmResult CreateHomeOption(HomeOption option, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("new_optionofhome");

                if (!string.IsNullOrEmpty(option.Name))
                {
                    ent["new_name"] = option.Name;
                }

                if (option.Contact != null && option.Contact.Id != Guid.Empty)
                {
                    ent["new_contactid"] = option.Contact;
                }

                if (option.Account != null && option.Account.Id != Guid.Empty)
                {
                    ent["new_accountid"] = option.Account;
                }

                if (option.Product != null && option.Product.Id != Guid.Empty)
                {
                    ent["new_productid"] = option.Product;
                }

                if (option.OptionDate != null)
                {
                    ent["new_optiondate"] = option.OptionDate;
                }

                if (option.Oppotunity != null && option.Oppotunity.Id != Guid.Empty)
                {
                    ent["new_opporutnity"] = option.Oppotunity;
                }

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Konut opsiyonu oluşturuldu.";

            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult UpdateProductOptionInfo(HomeOption option, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("product");
                ent.Id = option.Product.Id;

                if (option.Id != null && option.Id != Guid.Empty)
                {
                    ent["new_validoptionid"] = new EntityReference("new_optionofhome", option.Id);
                }

                if (option.Contact != null && option.Contact.Id != Guid.Empty)
                {
                    ent["new_contactid"] = option.Contact;
                }

                if (option.Account != null && option.Account.Id != Guid.Empty)
                {
                    ent["new_accountid"] = option.Account;
                }

                if (option.OptionDate != null)
                {
                    ent["new_optionlastvaliditydate"] = option.OptionDate;
                }

                if (option.Owner != null && option.Owner.Id != Guid.Empty)
                {
                    ent["new_salespersonofoptionid"] = option.Owner;
                }

                if (option.AdministratorOption != null)
                {
                    ent["new_ismanagementoption"] = option.AdministratorOption;
                }

                ent["statuscode"] = new OptionSetValue((int)ProductStatuses.Opsiyonlu);

                service.Update(ent);

                returnValue.Success = true;
                returnValue.Result = "Ürün opsiyon bilgileri güncellendi";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        internal static void CreateHomePriceChanging(Entity entity, Entity preImage, IOrganizationService adminService)
        {
            if (preImage == null || !preImage.Contains("price"))
                return;
            Entity e = new Entity("new_homepricechanging");
            e.Attributes["new_name"] = preImage.Attributes["name"] + " - " + DateTime.Now.ToString("dd.MM.yyyy hh:mm");
            e.Attributes["new_productid"] = new EntityReference("product", entity.Id);
            e.Attributes["new_pastamount"] = new Money(preImage.Contains("price") ? ((Money)preImage.Attributes["price"]).Value : 0);
            e.Attributes["new_newamount"] = new Money(entity.Contains("price") ? ((Money)entity.Attributes["price"]).Value : 0);
            if (entity.Contains("modifiedby"))
            {
                e.Attributes["new_changesystemuserid"] = entity.Attributes["modifiedby"];
            }
            adminService.Create(e);


        }

        public static MsCrmResult CheckTypeOfHomeExists(string typeOfHomeName, Guid generalTypeOfHomeId, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                    bob.new_typeofhomeId AS Id
                                    FROM
	                                    new_typeofhome AS bob (NOLOCK)
                                    WHERE
	                                    bob.new_name='{0}'
                                    AND
                                    	bob.new_generaltypeofhomeid='{1}'    
                                    AND
	                                    bob.statecode=0";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, typeOfHomeName, generalTypeOfHomeId));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Daire Tipi içeride tanımlıdır";

                    returnValue.CrmId = (Guid)dt.Rows[0]["Id"];
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;

        }

        public static MsCrmResultObject GetProductOptionInfo(Guid productId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.ProductId
                                    ,OH.new_contactid ContactId
                                    ,OH.new_contactidName ContactIdName
                                    ,OH.new_accountid AccountId
                                    ,OH.new_accountidName AccountIdName
                                    ,OH.new_optiondate OptionDate
                                FROM
	                                Product P WITH (NOLOCK)
                                INNER JOIN
                                    new_optionofhome OH WITH (NOLOCK)
                                    ON
                                    OH.new_optionofhomeId = P.new_validoptionid    
                                WHERE
                                    P.ProductId = '{0}'
                                    AND
	                                P.StatusCode IN({1},{2}) AND P.new_optionlastvaliditydate<GETUTCDATE()";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, productId, (int)ProductStatuses.Opsiyonlu, (int)ProductStatuses.YoneticiOpsiyonlu));

                if (dt != null && dt.Rows.Count > 0)
                {
                    HomeOption option = new HomeOption();
                    if (dt.Rows[0]["ContactId"] != DBNull.Value)
                    {
                        option.Contact = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["ContactId"],
                            Name = dt.Rows[0]["ContactIdName"].ToString(),
                            LogicalName = "contact"
                        };

                        option.Account = new EntityReference()
                        {
                            Id = (Guid)dt.Rows[0]["AccountId"],
                            Name = dt.Rows[0]["AccountIdName"].ToString(),
                            LogicalName = "account"
                        };

                        option.OptionDateString = dt.Rows[0]["OptionDate"] != DBNull.Value ? ((DateTime)dt.Rows[0]["OptionDate"]).ToLocalTime().ToString("dd.MM.yyyy") : "";
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = option;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Konut opsiyonu bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult UpdateProductRentInfo(HomeRentOption option, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("product");
                ent.Id = option.Product.Id;
                //ikinci el ve kiralama durumu
                if (option.UsedRenatAndSalesStatus != null)
                {
                    ent["new_usedrentalandsalesstatus"] = new OptionSetValue(option.UsedRenatAndSalesStatus.Value);
                }
                //eşya durumu
                if (option.GoodsStatus != null)
                {
                    ent["new_goodsstatus"] = new OptionSetValue(option.GoodsStatus.Value);
                }
                //Peşin ödenecek ay
                if (option.RentalMonths != null)
                {
                    ent["new_rentalmonths"] = new OptionSetValue(option.RentalMonths.Value);
                }
                //Kiralama notu
                if (!string.IsNullOrEmpty(option.RentalNot))
                {
                    ent["new_rentalnotes"] = option.RentalNot;
                }
                //kiralama ücreti
                if (option.PaymentOfHire != null)
                {
                    ent["new_paymentofhire"] = new Money(option.PaymentOfHire.Value);
                }

                ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", option.Currency.Id);

                service.Update(ent);

                returnValue.Success = true;
                returnValue.Result = "Ürün kiralama bilgileri güncellendi";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreateProductForRent(Product product, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                Entity ent = new Entity("product");
                ent["new_projectid"] = product.Project;
                //ikinci el ve kiralama durumu
                if (product.UsedRentalSalesStatus != null)
                {
                    ent["new_usedrentalandsalesstatus"] = new OptionSetValue(product.UsedRentalSalesStatus.Value);
                }

                //eşya durumu
                if (product.GoodsStatus != null)
                {
                    ent["new_goodsstatus"] = new OptionSetValue(product.GoodsStatus.Value);
                }
                //Peşin ödenecek ay
                if (product.RentalMonths != null)
                {
                    ent["new_rentalmonths"] = new OptionSetValue(product.RentalMonths.Value);
                }
                //Kiralama notu
                if (!string.IsNullOrEmpty(product.RentalNotes))
                {
                    ent["new_rentalnotes"] = product.RentalNotes;
                }
                //kiralama ücreti

                if (product.Brut != null)
                {
                    ent["new_grossm2"] = product.Brut.Value;
                }
                if (product.Net != null)
                {
                    ent["new_netm2"] = product.Net.Value;
                }
                if (product.Terrace != null)
                {
                    ent["new_terracem2"] = product.Terrace.Value;
                }
                if (product.Balcony != null)
                {
                    ent["new_balconym2"] = product.Balcony.Value;
                }
                if (product.BBNetArea != null)
                {
                    ent["new_bbnetarea"] = product.BBNetArea.Value;
                }
                if (product.BBGeneralGrossArea != null)
                {
                    ent["new_bbgeneralgrossarea"] = product.BBGeneralGrossArea.Value;
                }
                if (product.City != null)
                {
                    ent["new_cityid"] = new EntityReference("new_city", product.City.Value);
                }
                if (product.County != null)
                {
                    ent["new_townid"] = new EntityReference("new_town", product.County.Value);
                }
                if (!string.IsNullOrEmpty(product.Name))
                {
                    ent["name"] = product.Name;
                    ent["productnumber"] = product.Name;
                }

                if (!string.IsNullOrEmpty(product.Name))
                {
                    ent["name"] = product.Name;
                    ent["productnumber"] = product.Name;
                }

                if (product.GeneralHomeType != null)
                {
                    ent["new_generaltypeofhomeid"] = new EntityReference("new_generaltypeofhome", product.GeneralHomeType.Id);
                }
                if (product.HomeType != null)
                {
                    ent["new_typeofhomeid"] = new EntityReference("new_typeofhome", product.HomeType.Id);
                }

                ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", product.Currency.Id);
                ent["defaultuomid"] = new EntityReference("uom", Globals.DefaultUoMId);
                ent["defaultuomscheduleid"] = new EntityReference("uomschedule", Globals.DefaultUoMScheduleId);


                //pricelevel id tanımlanır
                ProductPriceLevel pPriceLevel = new ProductPriceLevel();


                if (product.PaymentOfHire != null)
                {
                    ent["new_paymentofhire"] = new Money(product.PaymentOfHire.Value);
                    pPriceLevel.Price = product.PaymentOfHire.Value;
                }

                product.ProductId = service.Create(ent);
                pPriceLevel.Currency = new EntityReference("transactioncurrency", product.Currency.Id);
                PriceLevel productPriceLevel = (PriceLevel)(ProductHelper.GetPriceLevelByCurrencyId(product.Currency.Id, sda).ReturnObject);
                pPriceLevel.PriceLevel = new EntityReference("pricelevel", productPriceLevel.Id);
                pPriceLevel.Product = new EntityReference("product", product.ProductId);

                CreateOrUpdateProductPriceLevel(pPriceLevel, service);

                if (product.Appointment != null || product.PhoneCall != null)
                {
                    InterestProduct interestedProduct = new InterestProduct();
                    if (product.Appointment != null)
                    {
                        interestedProduct.Appointment = product.Appointment;
                    }
                    else if (product.PhoneCall != null)
                    {
                        interestedProduct.PhoneCall = product.PhoneCall;
                    }
                    interestedProduct.InterestedProduct = product;
                    NEF.Library.Utility.MsCrmResult interestedProRes = NEF.Library.Business.InterestProductHelper.CreateInterestHouse(interestedProduct, service);
                    return interestedProRes;
                }
                returnValue.CrmId = product.ProductId;
                returnValue.Success = true;
                returnValue.Result = "Ürün kiralama bilgileri Oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

    }
}
