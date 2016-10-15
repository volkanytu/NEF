using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;
using System.Data;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
namespace NEF.ConsoleApp.TEST
{
    public class updatecontacttitlecase
    {
        public static MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            Console.WriteLine("Update Contact Title Case working....");

            int errorCount = 0;
            int successCount = 0;

            IOrganizationService service = MSCRM.GetOrgService(true);
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);

            #region | SQL QUERY |

            string sqlQuery = @"SELECT
	                                c.ContactId
	                                ,c.FirstName
	                                ,c.LastName
                                FROM
                                Contact AS c (NOLOCK)
                                WHERE
                                c.StateCode=0
                                --AND
                                --c.ContactId IN ('09055AF4-95F4-E411-80D0-005056A60603','B40A2ADB-94F4-E411-80D0-005056A60603')
                                ORDER BY 
	                                c.CreatedOn DESC";
            #endregion

            DataTable dt = sda.getDataTable(sqlQuery);

            Console.SetCursorPosition(0, 2);
            Console.WriteLine("Data Count:" + dt.Rows.Count.ToString());

            if (dt.Rows.Count > 0)
            {
                OrganizationRequestCollection reqList = new OrganizationRequestCollection();

                int packetCount = dt.Rows.Count % 1000;

                int packetCounter = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.SetCursorPosition(0, 3);
                    Console.WriteLine("COUNTER:" + (i + 1).ToString());

                    try
                    {
                        string firstName = dt.Rows[i]["FirstName"] != DBNull.Value ? dt.Rows[i]["FirstName"].ToString() : string.Empty;
                        string lastName = dt.Rows[i]["LastName"] != DBNull.Value ? dt.Rows[i]["LastName"].ToString() : string.Empty;

                        string newFirstName = string.Empty;
                        string newLastName = string.Empty;

                        if (firstName != string.Empty)
                            newFirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(firstName.ToLower());

                        if (lastName != string.Empty)
                            newLastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lastName.ToLower());


                        Entity ent = new Entity("contact");
                        ent.Id = (Guid)dt.Rows[i]["ContactId"];
                        ent["firstname"] = newFirstName;
                        ent["lastname"] = newLastName;

                        UpdateRequest updateRequest = new UpdateRequest();
                        updateRequest.Target = ent;

                        reqList.Add(updateRequest);

                        if (reqList.Count == 1000)
                        {
                            MigrationHelper.executeMultipleWithRequests(reqList, "contacttitlecase", service);
                            reqList = new OrganizationRequestCollection();

                            packetCounter++;

                            Console.SetCursorPosition(0, 8);
                            Console.WriteLine("Packet:" + packetCounter.ToString());
                        }

                        if (packetCounter > packetCount && (i + 1) == dt.Rows.Count)
                        {
                            MigrationHelper.executeMultipleWithRequests(reqList, "contacttitlecase", service);
                            reqList = new OrganizationRequestCollection();
                        }
                        //service.Update(ent);

                        successCount++;

                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("Success:" + successCount.ToString());
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        Console.SetCursorPosition(0, 6);
                        Console.WriteLine("Error:" + errorCount.ToString());
                    }
                }
            }

            return returnValue;
        }
    }
}
