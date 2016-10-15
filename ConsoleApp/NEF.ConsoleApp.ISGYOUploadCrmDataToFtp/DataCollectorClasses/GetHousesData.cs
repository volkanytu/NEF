using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using NEF.Library.Business;
using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;

namespace NEF.ConsoleApp.ISGYOUploadCrmDataToFtp
{
    public class GetHousesData : ICollaborateData
    {
        CollaborateDataType _dataType;

        public MsCrmResult Process(SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                            p.ProductId
	                            ,p.Name
	                            ,pro.new_projectId AS ProjectId
	                            ,pro.new_name AS ProjectIdName
	                            ,p.Price AS ListPrice
	                            ,p.new_KDVratio AS Kdv
	                            ,p.new_persquaremeter AS PerSquareMeter
	                            ,p.new_taxofstamp AS TaxOfStamp
	                            ,p.TransactionCurrencyIdName
	                            ,p.ProductNumber
	                            ,p.new_blockidName AS Block
	                            ,p.new_floornumber AS FloorNumber
	                            ,p.new_homenumber AS HomeNumber
	                            ,p.new_blockofbuildingidName AS BlockOfBuilding
	                            ,p.new_threaderidName AS Threader
	                            ,p.new_parcelidName AS Parcel
	                            ,p.new_licencenumber AS LicenceNo
	                            ,p.new_flooroflicence AS FloorOfLicence
	                            ,p.new_unittypeidName AS UnitType
	                            ,p.new_generaltypeofhomeidName AS GeneralTypeOfHome
	                            ,p.new_typeofhomeidName AS TypeOfHome
	                            ,p.new_netm2 AS Netm2
	                            ,p.new_grossm2 AS GrossM2
	                            ,p.new_balconym2 AS BalconyM2
	                            ,p.new_terracegross AS TerraceGrossM2
	                            ,p.new_kitchenm2 AS KitchenM2
	                            ,p.new_aks AS Aks
	                            ,sm.Value AS Status
	                            ,p.new_north AS DirNorth
	                            ,p.new_south AS DirSouth
	                            ,p.new_west AS DirWest
	                            ,p.new_east AS DirEast
	                            ,p.new_northwest AS DirNorthWest
	                            ,p.new_northeast AS DirNorthEast
	                            ,p.new_southwest AS DirSouthWest
	                            ,p.new_southeast AS DirSouthEast
                            FROM
                            Product AS p (NOLOCK)
	                            JOIN
		                            new_project AS pro (NOLOCK)
			                            ON
			                            p.new_projectid=pro.new_projectId
	                            JOIN
		                            new_projectsalescollaborate AS pcol (NOLOCK)
			                            ON
			                            pro.new_projectId=pcol.new_projectid
	                            JOIN
		                            new_collaborateaccount AS col (NOLOCK)
			                            ON
			                            pcol.new_accountid=col.new_collaborateaccountId
	                            LEFT JOIN
		                            StringMap AS sm (NOLOCK)
			                            ON
			                            sm.ObjectTypeCode=10016
			                            AND
			                            sm.AttributeName='statuscode'
			                            AND
			                            sm.AttributeValue=p.StatusCode
                            WHERE
                            pro.new_projectId!='CB9CC4EF-1117-E011-817F-00123F4DA0F7'
                            AND
                            col.new_collaborateaccountId='B3A17FFD-C5B1-E411-80C7-005056A60603'";

            #endregion

            try
            {
                DataTable dt = sda.getDataTable(sqlQuery);

                if (dt.Rows.Count > 0)
                {
                    XLWorkbook wb = new XLWorkbook();

                    IXLWorksheet ws = wb.Worksheets.Add(dt, _dataType.ToString());

                    wb.SaveAs(@Environment.CurrentDirectory + @"\files\" + _dataType.ToString() + ".xlsx");

                }
                returnValue.Success = true;
                returnValue.Result = string.Format("[{0}] adet data gönderildi.[{1}]", dt.Rows.Count.ToString(), _dataType.ToString());
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public GetHousesData(CollaborateDataType dataType)
        {
            _dataType = dataType;
        }
    }
}
