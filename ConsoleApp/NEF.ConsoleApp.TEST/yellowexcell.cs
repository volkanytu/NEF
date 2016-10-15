using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using System.Data;
using System.IO;
using System.Data.OleDb;
namespace NEF.ConsoleApp.TEST
{
    public static class yellowexcell
    {
        static string filePath = string.Empty;
        static string fileName = string.Empty;
        public static void Process()
        {
            filePath = Environment.CurrentDirectory;
            fileName = filePath + @"\84CA3BC3-5BF2-E411-80D0-005056A60603.xlsx";

            GetExcellTable(new Guid("84CA3BC3-5BF2-E411-80D0-005056A60603"));

            DataTable dt = GetExcelDataTable(fileName, "Sheet1");

            File.Delete(fileName);
        }

        public static DataTable GetExcellTable(Guid quoteId)
        {
            DataTable dt = null;

            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            DataTable dt_res = new DataTable();

            #region |SQL QUERY|
            string sqlQuery = @"SELECT TOP 1
	                                si.QuoteId
	                                ,an.DocumentBody
                                FROM
                                Quote AS si (NOLOCK)
	                                JOIN
		                                Annotation AS an (NOLOCK)
			                                ON
			                                si.QuoteId=an.ObjectId
			                                AND
			                                an.FileName LIKE '%xlsx%'
			                                AND
			                                an.DocumentBody IS NOT NULL
                                WHERE
	                                si.QuoteId='{0}' ORDER BY an.CreatedOn DESC";
            #endregion

            try
            {
                dt = sda.getDataTable(string.Format(sqlQuery, quoteId.ToString()));

                if (dt.Rows.Count > 0)
                {
                    var bytes = Convert.FromBase64String(dt.Rows[0]["DocumentBody"].ToString());
                    using (var imageFile = new FileStream(@fileName, FileMode.CreateNew))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    //File.WriteAllBytes(@"C:\Users\innthebox\Desktop\NEF CSV", Convert.FromBase64String(dt.Rows[0]["DocumentBody"].ToString()));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return dt;

        }

        public static DataTable GetExcelDataTable(string filePath, string sheetName)
        {

            OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + "; Extended Properties=Excel 12.0");
            baglanti.Open();
            string sorgu = "select * from [" + sheetName + "$] ";
            OleDbDataAdapter data_adaptor = new OleDbDataAdapter(sorgu, baglanti);
            baglanti.Close();

            DataTable dt = new DataTable();
            data_adaptor.Fill(dt);
            return dt;

        }
    }
}
