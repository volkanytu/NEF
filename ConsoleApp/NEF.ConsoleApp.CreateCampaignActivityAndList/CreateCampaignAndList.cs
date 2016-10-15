using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.CreateCampaignActivityAndList
{
    public class CreateCampaignAndList
    {
        public static void Execute(IOrganizationService service)
        {
            try
            {
                //DeleteCampaignActivityAndList(service); //Gerekli olursa açılacaktır.

               CreateCampaignActivityForEmail(service);

                CreateCampaignActivityForSms(service);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Read();
                throw new Exception("Execute");
            }
        }



        /// <summary>
        /// E-mail ve Sms için bir gün önce yaratılmış olan Kampanya Aktiviteleri'ni ve Pazarlama Listeleri'ni siler.
        /// </summary>
        /// <param name="service"></param>
        private static void DeleteCampaignActivityAndList(IOrganizationService service)
        {
            //try
            //{
            //    #region | Members |

            //    DataTable ListForDelete = new DataTable();
            //    ListForDelete = GetListForDelete();

            //    DataTable CampaignActivityForDelete = new DataTable();
            //    CampaignActivityForDelete = GetCampaignActivityForDelete();

            //    #endregion | Members |

            //    if (ListForDelete.Rows.Count > 0)
            //    {
            //        foreach (DataRow list in ListForDelete.Rows)
            //        {
            //            service.Delete("list", new Guid(list["ListId"].ToString()));
            //        }
            //    }

            //    if (CampaignActivityForDelete.Rows.Count > 0)
            //    {
            //        foreach (DataRow campaignActivity in CampaignActivityForDelete.Rows)
            //        {
            //            service.Delete("campaignactivity", new Guid(campaignActivity["CampaignId"].ToString()));
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("DeleteCampaignActivityAndList");
            //}
        }

        /// <summary>
        /// Euromessage için gerekli olan Kampanya Aktivitesi(E-mail)'ni yaratır.
        /// </summary>
        /// <param name="service"></param>
        private static void CreateCampaignActivityForEmail(IOrganizationService service)
        {
            try
            {
                #region | Members |
                Console.WriteLine("test");
                DataTable Campaign = new DataTable();
                Campaign = GetCampaignId();
                Console.WriteLine("test1");
                Entity CampaignActivity = new Entity("campaignactivity");

                var today = DateTime.Now;
                string today2 = String.Format("{0:dd/MM/yyyy}", today);          

                #endregion | Members |

                if (Campaign.Rows.Count > 0)
                {
                    #region | E-mail |
                    Console.WriteLine("test1");
                    CampaignActivity["regardingobjectid"] = new EntityReference("campaign", new Guid(Campaign.Rows[0]["CampaignId"].ToString())); //Ana Kampanya

                    CampaignActivity["subject"] = today2 + " – Doğum Gününüz Kutlu Olsun"; //Konu
                    
                    CampaignActivity["channeltypecode"] = new OptionSetValue(7); //Kanal Tipi(Email)
                   
                    CampaignActivity["scheduledstart"] = Convert.ToDateTime(today2).AddHours(10);
                  
                    CampaignActivity["scheduledend"] = Convert.ToDateTime(today2).AddHours(21);
                   
                    CampaignActivity["new_sendingtype"] = new OptionSetValue(2); // Periyodik
                    Console.WriteLine("test8");
                    CampaignActivity.Id = service.Create(CampaignActivity);
                    Console.WriteLine("test3");
                    CreateListForEmailCampaignActivity(service, new Guid(Campaign.Rows[0]["CampaignId"].ToString()), CampaignActivity.Id);

                    #endregion | E-mail |
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Euromessage için gerekli olan Kampanya Aktivitesi(SMS)'ni yaratır.
        /// </summary>
        /// <param name="service"></param>
        private static void CreateCampaignActivityForSms(IOrganizationService service)
        {
            try
            {
                #region | Members |

                DataTable Campaign = new DataTable();
                Campaign = GetCampaignId();

                Entity CampaignActivity = new Entity("campaignactivity");

                var today = DateTime.Now;
                string today2 = String.Format("{0:dd/MM/yyyy}", today);

                #endregion | Members |

                if (Campaign.Rows.Count > 0)
                {
                    #region | SMS |

                    CampaignActivity["regardingobjectid"] = new EntityReference("campaign", new Guid(Campaign.Rows[0]["CampaignId"].ToString())); //Ana Kampanya

                    CampaignActivity["subject"] = today2 + " – Birthday SMS Activity"; //Konu

                    CampaignActivity["channeltypecode"] = new OptionSetValue(3); //Kanal Tipi(Sms)

                    CampaignActivity["scheduledstart"] = Convert.ToDateTime(today2).AddHours(10);

                    CampaignActivity["scheduledend"] = Convert.ToDateTime(today2).AddHours(21);

                    CampaignActivity["new_sendingtype"] = new OptionSetValue(2); // Periyodik

                    CampaignActivity.Id = service.Create(CampaignActivity);

                    CreateListForSmsCampaignActivity(service, new Guid(Campaign.Rows[0]["CampaignId"].ToString()), CampaignActivity.Id);

                    #endregion | SMS |
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        /// <summary>
        /// Kampanya Aktiviteleri'nin yaratılacağı ana Kampanya bilgilerini getirir.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCampaignId()
        {
            DataTable GetCampaignId = new DataTable();
         
         
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string sqlQuery = @"SELECT
	                                C.CampaignId
                                FROM
	                                Campaign C WITH(NOLOCK)
                                WHERE
	                                C.Name = 'Birthday Mailing & SMS'";

            GetCampaignId = sda.getDataTable(sqlQuery);
            sda.closeConnection();
            return GetCampaignId;
        }



        /// <summary>
        /// Kampanya Aktivitesi(E-mail) için Pazarlama Listesi(E-mail)'ni yaratır.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="CampaignId"></param>
        /// <param name="CampaignActivityId"></param>
        private static void CreateListForEmailCampaignActivity(IOrganizationService service, Guid CampaignId, Guid CampaignActivityId)
        {
            #region | Members |

            Entity List = new Entity("list"); //MarketingList

            var today = DateTime.Now;
            string today2 = String.Format("{0:dd/MM/yyyy}", today);

            #endregion | Members |

            try
            {
                #region | Create List |

                List.Attributes["listname"] = today2 + " – Birthday Mailing Marketing List";

                List.Attributes["createdfromcode"] = new OptionSetValue(2); //Üye Tipi(Contact)

                List.Id = service.Create(List);

                #endregion | Create List |   
             
                CreateListCompaignActivityConnectionForEmailCampaignActivity(service, CampaignId, CampaignActivityId, List.Id);

                CreateListContactConnectionForEmailList(service, List.Id, CampaignActivityId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Pazarlama Listesi(E-mail)'ni ana Kmapanya'ya ve Kampanya Listesi(E-mail)'ne bağlar.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="CampaignId"></param>
        /// <param name="CampaignActivityId"></param>
        /// <param name="listId"></param>
        private static void CreateListCompaignActivityConnectionForEmailCampaignActivity(IOrganizationService service, Guid CampaignId, Guid CampaignActivityId, Guid listId)
        {
            #region | Members |

            Entity List = new Entity("list"); //MarketingList

            #endregion | Members |

            try
            {
                #region | Create List - CampaignActivity Connection |

                AddItemCampaignRequest req = new AddItemCampaignRequest();
                req.CampaignId = CampaignId;
                req.EntityName = List.LogicalName;
                req.EntityId = listId;
                AddItemCampaignResponse resp = (AddItemCampaignResponse)service.Execute(req);

                AddItemCampaignActivityRequest req2 = new AddItemCampaignActivityRequest();
                req2.CampaignActivityId = CampaignActivityId;
                req2.EntityName = List.LogicalName;
                req2.ItemId = listId;
                AddItemCampaignActivityResponse resp2 = (AddItemCampaignActivityResponse)service.Execute(req2);

                #endregion | Create List - CampaignActivity Connection |
            }
            catch (Exception ex)
            {
                throw new Exception("CreateListCompaignActivityConnectionForEmailCampaignActivity");
            }
        }

        /// <summary>
        /// O gün doğum günü olan Kişi'leri yaratılan Pazarlama Listesi(E-mail)'ne ekler.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ListId"></param>
        /// <param name="CampaignActivityId"></param>
        private static void CreateListContactConnectionForEmailList(IOrganizationService service, Guid ListId, Guid CampaignActivityId)
        {
            #region | Members |

            DataTable GetContacts = new DataTable();
            GetContacts = GetContactsForEmailCampaignActivityList();

            #endregion | Members |

            try
            {
                if (GetContacts.Rows.Count > 0)
                {
                    foreach (DataRow contact in GetContacts.Rows)
                    {
                        Guid ContactId = new Guid(contact["ContactId"].ToString());

                        AddListMembersListRequest req = new AddListMembersListRequest();
                        req.ListId = ListId;
                        req.MemberIds = new Guid[] { ContactId };
                        AddListMembersListResponse response = (AddListMembersListResponse)service.Execute(req);
                    }
                }

               // SendMailIntegration(service, ListId, CampaignActivityId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Kampanya Aktivitesi(SMS) için Pazarlama Listesi(SMS)'ni yaratır.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="CampaignId"></param>
        /// <param name="CampaignActivityId"></param>
        private static void CreateListForSmsCampaignActivity(IOrganizationService service, Guid CampaignId, Guid CampaignActivityId)
        {
            #region | Members |

            Entity List = new Entity("list");

            var today = DateTime.Now;
            string today2 = String.Format("{0:dd/MM/yyyy}", today);

            #endregion | Members |

            try
            {
                #region | Create List |

                List.Attributes["listname"] = today2 + " – Birthday SMS Marketing List";

                List.Attributes["createdfromcode"] = new OptionSetValue(2); //Üye Tipi - Contact

                List.Id = service.Create(List);

                #endregion | Create List |

                CreateListCompaignActivityConnectionForSmsCampaignActivity(service, CampaignId, CampaignActivityId, List.Id);

                CreateListContactConnectionForSmsList(service, List.Id, CampaignActivityId);
            }
            catch (Exception ex)
            {
                throw new Exception("CreateListForSmsCampaignActivity");
            }
        }

        /// <summary>
        /// Pazarlama Listesi(SMS)'ni ana Kmapanya'ya ve Kampanya Listesi(SMS)'ne bağlar.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="CampaignId"></param>
        /// <param name="CampaignActivityId"></param>
        /// <param name="listId"></param>
        private static void CreateListCompaignActivityConnectionForSmsCampaignActivity(IOrganizationService service, Guid CampaignId, Guid CampaignActivityId, Guid listId)
        {
            #region | Members |

            Entity List = new Entity("list"); //MarketingList

            #endregion | Members |

            try
            {
                #region | Create List - CampaignActivity Connection |

                AddItemCampaignRequest req = new AddItemCampaignRequest();
                req.CampaignId = CampaignId;
                req.EntityName = List.LogicalName;
                req.EntityId = listId;
                AddItemCampaignResponse resp = (AddItemCampaignResponse)service.Execute(req);

                AddItemCampaignActivityRequest req2 = new AddItemCampaignActivityRequest();
                req2.CampaignActivityId = CampaignActivityId;
                req2.EntityName = List.LogicalName;
                req2.ItemId = listId;
                AddItemCampaignActivityResponse resp2 = (AddItemCampaignActivityResponse)service.Execute(req2);

                #endregion | Create List - CampaignActivity Connection |
            }
            catch (Exception ex)
            {
                throw new Exception("CreateListCompaignActivityConnectionForSmsCampaignActivity");
            }
        }

        /// <summary>
        /// O gün doğum günü olan Kişi'leri yaratılan Pazarlama Listesi(SMS)'ne ekler.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ListId"></param>
        /// <param name="CampaignActivityId"></param>
        private static void CreateListContactConnectionForSmsList(IOrganizationService service, Guid ListId, Guid CampaignActivityId)
        {
            #region | Members |

            DataTable GetContacts = new DataTable();
            GetContacts = GetContactsForSmsCampaignActivityList();

            #endregion | Members |

            try
            {
                if (GetContacts.Rows.Count > 0)
                {
                    foreach (DataRow contact in GetContacts.Rows)
                    {
                        Guid ContactId = new Guid(contact["ContactId"].ToString());

                        AddListMembersListRequest req = new AddListMembersListRequest();
                        req.ListId = ListId;
                        req.MemberIds = new Guid[] { ContactId };
                        AddListMembersListResponse response = (AddListMembersListResponse)service.Execute(req);
                    }
                }

               // SendSmsIntegration(service, ListId, CampaignActivityId);
            }
            catch (Exception ex)
            {
                throw new Exception("CreateListContactConnectionForSmsList");
            }
        }



        /// <summary>
        /// Pazarlama Listesi(E-mail) için gerekli olan o gün doğum günü olan Kişi'lerin bilgisini getirir.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetContactsForEmailCampaignActivityList()
        {
            DataTable GetContactsForEmailCampaignActivityList = new DataTable();
          
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string sqlQuery = @"SELECT
	                                C.ContactId [ContactId]
                                FROM
	                                Contact C WITH(NOLOCK)
                                WHERE
	                                C.StateCode = 0
                                AND
	                               C.birthdate IS NOT NULL
                                AND
	                                LEFT((RIGHT(CONVERT(VARCHAR(10), dbo.fn_UTCToTzSpecificLocalTime(C.birthdate,-120,-60,0,3,5,3,0,0,0,1,0,0,1,5,4,0,0,0,0), 103), 10)), 5) = LEFT((RIGHT(CONVERT(VARCHAR(10), GETDATE(), 103), 10)), 5)
                                AND
	                                C.donotemail = 0
                                AND
	                               C.donotbulkemail = 0
                                AND
	                                C.emailaddress1 IS NOT NULL
                                AND
	                                (
			                                C.new_email = 0
		                                OR
			                                C.new_email IS NULL
	                                )
                               AND
                                   C.new_cardnumber IS NOT NULL
                                AND
                                    C.new_currentflg = 1";

            GetContactsForEmailCampaignActivityList = sda.getDataTable(sqlQuery);
            sda.closeConnection();

            return GetContactsForEmailCampaignActivityList;
        }

        /// <summary>
        /// Pazarlama Listesi(SMS) için gerekli olan o gün doğum günü olan Kişi'lerin bilgisini getirir.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetContactsForSmsCampaignActivityList()
        {
            DataTable GetContactsForSmsCampaignActivityList = new DataTable();
          
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string sqlQuery = @"SELECT
	                                C.ContactId [ContactId]
                                FROM
	                                Contact C WITH(NOLOCK)
                                WHERE
	                               C.StateCode = 0
                               AND
	                               C.birthdate IS NOT NULL
                                AND
	                               LEFT((RIGHT(CONVERT(VARCHAR(10), dbo.fn_UTCToTzSpecificLocalTime(C.birthdate,-120,-60,0,3,5,3,0,0,0,1,0,0,1,5,4,0,0,0,0), 103), 10)), 5) = LEFT((RIGHT(CONVERT(VARCHAR(10), GETDATE(), 103), 10)), 5)
                               AND
	                                C.donotphone = 0
                                AND
	                               C.mobilephone IS NOT NULL
                                AND
	                                (
			                               C.new_invalidmobilephone = 0 
		                               OR
			                                C.new_invalidmobilephone IS NULL
	                               )
                                AND
	                                C.new_cardnumber IS NOT NULL
                                AND
                                   C.new_currentflg = 1 ";

            GetContactsForSmsCampaignActivityList = sda.getDataTable(sqlQuery);
            sda.closeConnection();
            return GetContactsForSmsCampaignActivityList;
        }



        /// <summary>
        /// Silinecek Pazarlama Listeleri'ni getirir.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetListForDelete()
        {
            DataTable GetListForDelete = new DataTable();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string sqlQuery = @"SELECT
	                                L.ListId [ListId]
                                FROM
	                                List L WITH(NOLOCK)
                                WHERE
	                                L.ListName LIKE '%Birthday Mailing Marketing List'
                                OR
	                                L.ListName LIKE '%Birthday SMS Marketing List' 
                                AND
	                                L.StateCode = 1
                                AND
	                                L.StatusCode = 1";

            GetListForDelete = sda.getDataTable(sqlQuery);
            sda.closeConnection();
            return GetListForDelete;
        }

        /// <summary>
        /// Silinecek Kampanya Aktiviteleri'ni getirir.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCampaignActivityForDelete()
        {
            DataTable GetCampaignActivityForDelete = new DataTable();
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            string sqlQuery = @"SELECT
	                                CA.ActivityId [CampaignId]
                                FROM
	                                CampaignActivity CA WITH(NOLOCK)
                                WHERE
	                                CA.Subject LIKE '%Birthday Mailing Activity'
                                OR
	                                CA.Subject LIKE '%Birthday SMS Activity' 
                                AND
	                                CA.StateCode = 1
                                AND
	                                CA.StatusCode = 100000004";

            GetCampaignActivityForDelete = sda.getDataTable(sqlQuery);
            sda.closeConnection();
            return GetCampaignActivityForDelete;
        }



        /// <summary>
        /// Euromessage sistemine Pazarlama Listesi(E-mail)'ni gönderir.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ListId"></param>
        /// <param name="CampaignActivityId"></param>
        private static void SendMailIntegration(IOrganizationService service, Guid ListId, Guid CampaignActivityId)
        {
            //SqlDataAccess sdaCustom = sdaCustom = new SqlDataAccess(new SqlConnection(Globals.PrimaryConnectionString));
            //TEMPEventLog logMe = new TEMPEventLog(service, "Tefal.ConsoleApp.CreateCampaignActivityAndList", sdaCustom);

            //#region |   Euromessage Authentication  |

            //EmailFunctions eFunc = new EmailFunctions();
            //string authentication = eFunc.AuthenticationEM();

            //#endregion |   Euromessage Authentication  |

            //var today = DateTime.Now;
            //string today2 = String.Format("{0:dd/MM/yyyy}", today);

            //var listName = today2 + " - Birthday Mail ML TEST";
            //Guid campaignActivityID = CampaignActivityId;

            //try
            //{
            //    ProcessResult result = eFunc.CreateSendListOnEuroMessage(listName, authentication);

            //    if (result.IsSuccess == true)
            //    {
            //        Console.WriteLine("Pazarlama listesi oluşturuldu.");

            //        result = eFunc.AddToSendLists(campaignActivityID, authentication, listName, service);

            //        #region | UPDATE |
            //        if (result.IsSuccess == true)
            //        {
            //            //Kampanya aktivitesi 100000004 -> EuroMsg Gönderim Yapıldı olarak UPDATE Ediliyor.
            //            //SetStateRequest stateRequest = new SetStateRequest()
            //            //{
            //            //    EntityMoniker = new EntityReference("campaignactivity", campaignActivityID),
            //            //    State = new OptionSetValue(1),
            //            //    Status = new OptionSetValue(100000004)
            //            //};
            //            //SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);

            //            //Pazarlama Listesini Diactive eder.
            //            //SetStateRequest stateRequest2 = new SetStateRequest()
            //            //{
            //            //    EntityMoniker = new EntityReference("list", ListId),
            //            //    State = new OptionSetValue(1),
            //            //    Status = new OptionSetValue(1)
            //            //};
            //            //SetStateResponse stateResponse2 = (SetStateResponse)service.Execute(stateRequest2);

            //            //Console.WriteLine("Kampanya aktivitesi Euromsg'a başarıyla gönderildi. Aktivite Id : " + campaignActivityID.ToString());
            //        }
            //        else
            //        {
            //            Console.WriteLine("Liste üyeleri EuroMsg tarafına iletilemedi.Detail: " + result.Result + " - " + result.Message);
            //            logMe.Log("CreateCampaignAndList/SendMailIntegration - Execute", "Liste üyeleri EuroMsg tarafına iletilemedi. Detail: " + result.Result + " - " + result.Message, TEMPEventLog.EventType.Info);
            //        }
            //        #endregion | UPDATE |
            //    }
            //    else
            //    {
            //        Console.WriteLine("EuroMsg tarafında liste oluşturulurken hata alındı.");
            //        logMe.Log("CreateCampaignAndList/SendMailIntegration - Execute", "EuroMsg tarafında liste oluşturulurken hata alındı. Hata mesajı = " + result.Message, TEMPEventLog.EventType.Info);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// Euromessage sistemine Pazarlama Listesi(SMS)'ni gönderir.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ListId"></param>
        /// <param name="CampaignActivityId"></param>
        private static void SendSmsIntegration(IOrganizationService service, Guid ListId, Guid CampaignActivityId)
        {
            //SqlDataAccess sdaCustom = sdaCustom = new SqlDataAccess(new SqlConnection(Globals.PrimaryConnectionString));
            //TEMPEventLog logMe = new TEMPEventLog(service, "Tefal.ConsoleApp.CreateCampaignActivityAndList", sdaCustom);

            //#region |   Euromessage Authentication  |

            //SmsFunctions sFunc = new SmsFunctions();
            //string authentication = sFunc.AuthenticationEM();

            //#endregion |   Euromessage Authentication  |

            //var today = DateTime.Now;
            //string today2 = String.Format("{0:dd/MM/yyyy}", today);

            //var listName = today2 + " - Birthday SMS ML TEST";
            //Guid campaignActivityID = CampaignActivityId;

            //try
            //{
            //    ProcessResult result = sFunc.CreateSendListOnEuroMessage(listName, authentication);
            //    if (result.IsSuccess == true)
            //    {
            //        Console.WriteLine("Pazarlama listesi oluşturuldu.");

            //        result = sFunc.AddToSendLists(campaignActivityID, authentication, listName, service);

            //        #region | UPDATE |
            //        if (result.IsSuccess == true)
            //        {
            //            Kampanya aktivitesi 100000004->EuroMsg Gönderim Yapıldı olarak UPDATE Ediliyor.
            //            SetStateRequest stateRequest = new SetStateRequest()
            //            {
            //                EntityMoniker = new EntityReference("campaignactivity", campaignActivityID),
            //                State = new OptionSetValue(1),
            //                Status = new OptionSetValue(100000004)
            //            };
            //            SetStateResponse stateResponse = (SetStateResponse)service.Execute(stateRequest);

            //            Pazarlama Listesini Diactive eder.
            //            SetStateRequest stateRequest2 = new SetStateRequest()
            //            {
            //                EntityMoniker = new EntityReference("list", ListId),
            //                State = new OptionSetValue(1),
            //                Status = new OptionSetValue(1)
            //            };
            //            SetStateResponse stateResponse2 = (SetStateResponse)service.Execute(stateRequest2);

            //            Console.WriteLine("Kampanya aktivitesi Euromsg'a başarıyla gönderildi. Aktivite Id : " + campaignActivityID.ToString());
            //        }
            //        else
            //        {
            //            Console.WriteLine("Liste üyeleri EuroMsg tarafına iletilemedi.Detail: " + result.Result + " - " + result.Message);
            //            logMe.Log("CreateCampaignAndList/SendSmsIntegration - Execute", "Liste üyeleri EuroMsg tarafına iletilemedi. Detail: " + result.Result + " - " + result.Message, TEMPEventLog.EventType.Info);
            //        }
            //        #endregion | UPDATE |
            //    }
            //    else
            //    {
            //        Console.WriteLine("EuroMsg tarafında liste oluşturulurken hata alındı.");
            //        logMe.Log("CreateCampaignAndList/SendSmsIntegration - Execute", "EuroMsg tarafında liste oluşturulurken hata alındı. Hata mesajı = " + result.Message, TEMPEventLog.EventType.Info);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("SendSmsIntegration");
            //}
        }
    }
}
