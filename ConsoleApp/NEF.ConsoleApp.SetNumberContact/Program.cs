using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.SetNumberContact
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            int[] years = new int[] { 2011, 2012, 2013, 2014, 2015 };
            for (int j = 0; j < years.Length; j++)
            {


                string sql = @"SELECT
                        C.ContactId
                        ,C.new_number Number
                        ,YEAR( C.CreatedOn) Year
                        ,C.CreatedOn
                        FROM
                        CONTACT C(NOLOCK)
                        WHERE
                        YEAR( C.CreatedOn)=@YEAR                        
                        ORDER BY
                        CreatedOn ASC";

                DataTable dt = sda.getDataTable(sql, new SqlParameter[] { new SqlParameter("YEAR", years[j]) });

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        string update = @"UPDATE
	                                   Contact
                                    SET
	                                   new_number=@new_number                                                                           
                                    WHERE
	                                    ContactId='{0}'";
                       
                        int number = i + 1;
                        string numberStr = number.ToString().PadLeft(6, '0');
                        sda.ExecuteNonQuery(string.Format(update, dt.Rows[i]["ContactId"].ToString()), new SqlParameter[] { new SqlParameter("new_number", years[j].ToString() + numberStr) });
                        Console.Clear();
                        Console.Write(i + 1 + "/" + dt.Rows.Count);
                    }
                }
            }
        }
    }
}
