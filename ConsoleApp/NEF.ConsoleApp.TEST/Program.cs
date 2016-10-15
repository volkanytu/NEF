using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NEF.Library.Business;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Specialized;
using System.Xml;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Crm.Sdk.Messages;
using System.Drawing;
using NEF.ConsoleApp.TEST.com.euromsg.ws.Auth;

using NEF.ConsoleApp.TEST.WebFormTest;




namespace NEF.ConsoleApp.TEST
{



    class Program
    {

        public static string AuthenticationEM()
        {
            try
            {
                string AuthenticationServiceKey = string.Empty;

                Auth auth = new Auth();
                EmAuthResult authResult = auth.Login(Globals.EuroMessageUserNameLive, Globals.EuroMessagePasswordLive);
                if (authResult.Code == "00")
                {
                    AuthenticationServiceKey = authResult.ServiceTicket;
                    return AuthenticationServiceKey;
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void LogoutEM(string AuthenticationServiceKey)
        {
            if (AuthenticationServiceKey != "")
            {
                Auth auth = new Auth();
                auth.Logout(AuthenticationServiceKey);
            }
        }


        public NEF.Library.Utility.MsCrmResult CreateProductForRent(Product product)
        {
            NEF.Library.Utility.MsCrmResult returnValue = new NEF.Library.Utility.MsCrmResult();
            try
            {
                if (product != null)
                {
                    IOrganizationService service = MSCRM.GetOrgService(true);
                    returnValue = ProductHelper.CreateProductForRent(product, service);
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Beklenmedik bir hata oluştu";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static NEF.Library.Utility.MsCrmResult CreateRental(EntityReference account, EntityReference contact, EntityReference owner, string[] interestedHouses)
        {
            SqlDataAccess sda = null;
            NEF.Library.Utility.MsCrmResult returnValue = new NEF.Library.Utility.MsCrmResult();
            try
            {
                if (contact != null || account != null)
                {
                    if (interestedHouses != null && interestedHouses.Length > 0)
                    {
                        IOrganizationService service = MSCRM.GetOrgService(true);
                        sda = new SqlDataAccess();
                        sda.openConnection(Globals.ConnectionString);

                        for (int i = 0; i < interestedHouses.Length; i++)
                        {
                            MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
                            if (interestedResult.Success && interestedResult.ReturnObject != null)
                            {
                                Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
                                if (_proc.StatusCode.Value == 1)
                                {
                                    Rental _rental = new Rental();
                                    if (contact != null)
                                    {
                                        _rental.Contact = contact;
                                    }
                                    if (account != null)
                                    {
                                        _rental.Account = account;
                                    }
                                    _rental.Name = _proc.Name;
                                    _rental.Owner = owner;
                                    _rental.Product = new EntityReference("product", _proc.ProductId);
                                    returnValue = RentalHelper.UpdateOrCreateRental(_rental, service, sda);
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Result = "Lütfen konut seçiniz!";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }


        static void Main(string[] args)
        {
            SqlDataAccess sda = new SqlDataAccess();
            sda.openConnection(Globals.ConnectionString);
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                returnValue = ActivityHelper.GetPhonecallsByQueue(QueueTypes.CallCenter, sda);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            finally
            {
                if (sda != null)
                {
                    sda.closeConnection();
                }
            }

            //EntityReference contact = new EntityReference("contact", new Guid("5762469a-61b7-e411-80c9-005056a60603"));

            //EntityReference account = null;
            //List<string> asd = new List<string>();
            //asd.Add("5b0c2cf2-576b-e611-8100-005056a60603");
            //string[] interestedHouses = asd.ToArray();
            ////SqlDataAccess sda = new SqlDataAccess();
            ////sda.openConnection(Globals.ConnectionString);
            ////Library.Utility.MsCrmResult returnValue = new Library.Utility.MsCrmResult();
            //try
            //{
            //    if (contact != null || account != null)
            //    {
            //        if (interestedHouses != null && interestedHouses.Length > 0)
            //        {
            //            IOrganizationService service = MSCRM.GetOrgService(true);
            //            sda = new SqlDataAccess();
            //            sda.openConnection(Globals.ConnectionString);

            //            //for (int i = 0; i < interestedHouses.Length; i++)
            //            //{
            //            //    MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
            //            //    if (interestedResult.Success && interestedResult.ReturnObject != null)
            //            //    {
            //            //        Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
            //            //        if (_proc.UsedRentalSalesStatus != 5 && _proc.UsedRentalSalesStatus != 6)
            //            //        {
            //            //            returnValue.Success = false;
            //            //            returnValue.Result = "Lütfen 2.El Satış kayıdı seçiniz!";
            //            //            return returnValue;
            //            //        }
            //            //    }
            //            //}
            //            for (int i = 0; i < interestedHouses.Length; i++)
            //            {
            //                MsCrmResultObject interestedResult = InterestProductHelper.GetInterestedHouseDetail(new Guid(interestedHouses[i]), sda);
            //                if (interestedResult.Success && interestedResult.ReturnObject != null)
            //                {
            //                    Product _proc = ProductHelper.GetProductDetail(((InterestProduct)interestedResult.ReturnObject).InterestedProduct.ProductId, sda);
            //                    if (_proc.StatusCode.Value == 1)
            //                    {
            //                        SecondHand _secondHand = new SecondHand();
            //                        if (contact != null)
            //                        {
            //                            _secondHand.Contact = contact;
            //                        }
            //                        if (account != null)
            //                        {
            //                            _secondHand.Account = account;
            //                        }
            //                        _secondHand.Name = _proc.Name;
            //                        _secondHand.Owner = new EntityReference("systemuser", Globals.AdministratorId);
            //                        _secondHand.Product = new EntityReference("product", _proc.ProductId);
            //                        returnValue = SecondHandHelper.UpdateOrCreateSecondHand(_secondHand, service, sda);
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            returnValue.Success = false;
            //            returnValue.Result = "Lütfen konut seçiniz!";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    returnValue.Success = false;
            //    returnValue.Result = ex.Message;
            //}
           

            //SqlDataAccess sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);
            //string rentalId = "11BDE9B2-4969-E611-8100-005056A60603";
            //string userId = "5a49c200-5a97-e411-80c0-005056a60603";
            //Library.Utility.MsCrmResult returnValue = new Library.Utility.MsCrmResult();
            //try
            //{
            //    if (!string.IsNullOrEmpty(rentalId))
            //    {

            //        IOrganizationService service = MSCRM.GetOrgService(true);

            //        MsCrmResultObject productResult = RentalHelper.GetRentalProducts(new Guid(rentalId), sda);
            //        if (productResult.Success)
            //        {
            //            List<Product> products = (List<Product>)productResult.ReturnObject;

            //            Product _proc = ProductHelper.GetProductDetail(products[0].ProductId, sda);
            //            if (_proc.UsedRentalSalesStatus == 2 ||
            //                 _proc.UsedRentalSalesStatus.Value == 3)
            //            {
            //                Entity ent = new Entity("product");
            //                ent.Id = _proc.ProductId;
            //                ent["new_usedrentalandsalesstatus"] = new OptionSetValue(4);
            //                service.Update(ent);
            //                returnValue = RentalHelper.UpdateRentalStatus(new Guid(rentalId), RentalStatuses.Tamamlandi, new Guid(userId), service);

            //            }
            //            else
            //            {
            //                returnValue.Success = false;
            //                returnValue.Result = "Konut durumu kiralamaya uygun veya yönetici opsiyonlu olmalıdır!";
            //            }
            //        }
            //    }
            //    else
            //    {
            //        returnValue.Success = false;
            //        returnValue.Result = "Lütfen kiralama seçiniz!";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    returnValue.Success = false;
            //    returnValue.Result = ex.Message;
            //}
            //finally
            //{
            //    if (sda != null)
            //    {
            //        sda.closeConnection();
            //    }
            //}




            //string rentalid = "ee564f75-4269-e611-8100-005056a60603";
            //MsCrmResultObject returnValue = new MsCrmResultObject();
            //SqlDataAccess sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);
            //try
            //{


            //    if (!string.IsNullOrEmpty(rentalid))
            //    {
            //        returnValue = RentalHelper.GetRentalDetail(new Guid(rentalid), sda);
            //    }
            //    else
            //    {
            //        returnValue.Success = false;
            //        returnValue.Result = "Lütfen Kiralama seçiniz!";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    returnValue.Result = ex.Message;
            //}
            //finally
            //{
            //    if (sda != null)
            //        sda.closeConnection();
            //}



            //service = MSCRM.AdminOrgService;
            //string rentalId = "3d08fea0-1e6a-e611-8100-005056a60603";
            //SqlDataAccess sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);
            //Entity entity = service.Retrieve("new_resalerecord", new Guid(rentalId), new ColumnSet(true));
            //SqlDataAccess sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);
            //MsCrmResultObject returnValue = new MsCrmResultObject();
            //try
            //{
                
            //    IOrganizationService service = MSCRM.GetOrgService(true, Globals.AdministratorId.ToString());

            //    returnValue = SecondHandHelper.MakeAuthorityDocSearch(null, null, null, service, sda);
            //}
            //catch (Exception ex)
            //{
            //    returnValue.Success = false;
            //    returnValue.Result = ex.Message;
            //}
            //finally
            //{
            //    if (sda != null)
            //    {
            //        sda.closeConnection();
            //    }
            //}


            //Entity preImage = service.Retrieve("new_resalerecord", new Guid(rentalId), new ColumnSet(true));


            //if (entity.Attributes.Contains("new_issendingapproval"))
            //{
            //    if (entity.GetAttributeValue<bool>("new_issendingapproval"))
            //    {
            //        EntityReference currency = preImage.Attributes.Contains("transactioncurrencyid") && preImage["transactioncurrencyid"] != null ? (EntityReference)preImage["transactioncurrencyid"] : null;
            //        EntityReference projectId = preImage.Attributes.Contains("new_projectid") && preImage["new_projectid"] != null ? (EntityReference)preImage["new_projectid"] : null;


            //        MsCrmResultObject productResult = SecondHandHelper.GetSecondHandProducts(entity.Id, sda);
            //        if (productResult.Success)
            //        {
            //            //Ürün alındı
            //            Product product = ((List<Product>)productResult.ReturnObject)[0];
            //            //Rule alındı
            //            SecondHandControlSetting control = ProductHelper.GetSecondHandControlSettingByProject(product.Project.Id, sda);
            //            //Kiralama tutarı alındı
            //            decimal secondHandAmount = preImage.GetAttributeValue<Money>("new_salesfee").Value;
            //            if (control.SecondHandControlSettingId != Guid.Empty)
            //            {
            //                if (control.ConsultantRate != null)
            //                {
            //                    if (control.ConsultantRate != decimal.MaxValue)
            //                    {
            //                        decimal rate = (product.PaymentOfHire.Value * (control.ConsultantRate.Value / 100));
            //                        decimal minRate = product.PaymentOfHire.Value - rate;
            //                        decimal maxRate = product.PaymentOfHire.Value + rate;

            //                        if (secondHandAmount >= minRate && secondHandAmount <= maxRate)
            //                        {
            //                            //Opsiyona takılmaz.
            //                            Entity ent = new Entity("product");
            //                            ent.Id = product.ProductId;
            //                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(7);//Satıldı.
            //                            service.Update(ent);
            //                            entity["statuscode"] = new OptionSetValue((int)SecondHandStatuses.Onaylandi); //Kiralama Durumu
            //                        }
            //                        else
            //                        {
            //                            //Ürün kiralama opsiyonlu
            //                            //Onaya gönder
            //                            Entity ent = new Entity("product");
            //                            ent.Id = product.ProductId;
            //                            ent["new_usedrentalandsalesstatus"] = new OptionSetValue(6);//konut durumu 2.el satış opsiyonlu
            //                            service.Update(ent);
            //                            entity["statuscode"] = new OptionSetValue((int)SecondHandStatuses.OnayBekleniyor); //Kiralama Durumu
            //                            Library.Utility.MsCrmResult mailResult = SecondHandHelper.SendMailSecondHandToApproval(product, preImage, UserTypes.IkinciElSatisDirektoru, sda, service);
            //                            if (!mailResult.Success)
            //                            {
            //                                throw new Exception(mailResult.Result);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}



            //if (entity.Attributes.Contains("new_issendingapproval"))
            //{
            //    if (entity.GetAttributeValue<bool>("new_issendingapproval"))
            //    {
            //        //EntityReference currency = preImage.Attributes.Contains("transactioncurrencyid") && preImage["transactioncurrencyid"] != null ? (EntityReference)preImage["transactioncurrencyid"] : null;
            //        //EntityReference projectId = entity.Attributes.Contains("new_projectid") && entity["new_projectid"] != null ? (EntityReference)entity["new_projectid"] : preImage.Attributes.Contains("new_projectid") && preImage["new_projectid"] != null ? (EntityReference)preImage["new_projectid"] : null;
            //        //Guid ownerId = ((EntityReference)preImage["ownerid"]).Id;
            //        //string ownerName = ((EntityReference)preImage["ownerid"]).Name;

            //    }
            //}
            //MsCrmResultObject productResult = RentalHelper.GetRentalProducts(entity.Id, sda);
            //if (productResult.Success)
            //{
            //    //Ürün alındı
            //    Product product = ((List<Product>)productResult.ReturnObject)[0];
            //    //Rule alındı
            //    RentalControlSetting control = ProductHelper.GetRentalControlSettingByProject(product.Project.Id, sda);
            //    //Kiralama tutarı alındı
            //    decimal rentalAmount = entity.GetAttributeValue<Money>("new_rentalfee").Value;
            //    if (control.RentalControlSettingId != Guid.Empty)
            //    {
            //        if (control.ConsultantRate != null)
            //        {
            //            if (control.ConsultantRate != decimal.MaxValue)
            //            {
            //                decimal rate = (product.PaymentOfHire.Value * (control.ConsultantRate.Value / 100));
            //                decimal minRate = product.PaymentOfHire.Value - rate;
            //                decimal maxRate = product.PaymentOfHire.Value + rate;

            //                if (rentalAmount >= minRate && rentalAmount <= maxRate)
            //                {
            //                    //Opsiyona takılmaz.
            //                    Entity ent = new Entity("product");
            //                    ent.Id = product.ProductId;
            //                    ent["new_usedrentalandsalesstatus"] = new OptionSetValue(4);//Kiralandı.
            //                    service.Update(ent);
            //                    entity["statuscode"] = new OptionSetValue((int)RentalStatuses.Tamamlandi); //Kiralama Durumu
            //                }
            //                else
            //                {
            //                    //Ürün kiralama opsiyonlu
            //                    //Onaya gönder
            //                    Entity ent = new Entity("product");
            //                    ent.Id = product.ProductId;
            //                    ent["new_usedrentalandsalesstatus"] = new OptionSetValue(3);//konut durumu kiralama opsiyonlu
            //                    service.Update(ent);
            //                    entity["statuscode"] = new OptionSetValue((int)RentalStatuses.OnayBekleniyor); //Kiralama Durumu
            //                    Library.Utility.MsCrmResult mailResult = RentalHelper.SendMailRentalToApproval(entity.Id, UserTypes.IkinciElSatisDirektoru, sda, service);
            //                    if (!mailResult.Success)
            //                    {
            //                        throw new Exception(mailResult.Result);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}





            //MsCrmResultObject returnValue = new MsCrmResultObject();
            //SqlDataAccess sda = new SqlDataAccess();
            //Guid rentalid = new Guid("da1618a2-ba63-e611-80ff-005056a60603");
            //try
            //{
            //    sda = new SqlDataAccess();
            //    sda.openConnection(Globals.ConnectionString);

            //    if (rentalid != Guid.Empty)
            //    {
            //        returnValue = QuoteHelper.GetRentalDetail(rentalid, sda);
            //    }
            //    else
            //    {
            //        returnValue.Success = false;
            //        returnValue.Result = "Lütfen satış seçiniz!";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    returnValue.Result = ex.Message;
            //}
            //finally
            //{
            //    if (sda != null)
            //        sda.closeConnection();
            //}


            //EntityReference contact = new EntityReference("contact", new Guid("5762469a-61b7-e411-80c9-005056a60603"));
            //EntityReference account = null;
            //EntityReference owner = new EntityReference("systemuser", new Guid("5a49c200-5a97-e411-80c0-005056a60603"));
            //List<string> interestedHouse = new List<string>();
            //interestedHouse.Add("523fd769-b963-e611-80ff-005056a60603");
            //string[] sten = interestedHouse.ToArray();

            //CreateRental(account, contact, owner, sten);

            //NEF.Library.Utility.Product p = new NEF.Library.Utility.Product();
            //p.Name = "dene23";

            //p.City = new Guid("0501f070-2ed5-df11-9b56-00123f4da0f7");
            //p.County = new Guid("0601f070-2ed5-df11-9b56-00123f4da0f7");
            //p.GeneralHomeType = new EntityReference("new_generaltypeofhome", new Guid("add1948b-c8b2-e011-b668-0015171049d9"));
            //p.PhoneCall = new EntityReference("phonecall", new Guid("19a9b290-305a-e611-80ff-005056a60603"));
            //p.Project = new EntityReference("new_project", new Guid("F3C2C130-405E-E611-80FF-005056A60603"));

            //ProductHelper.CreateProductForRent(p, MSCRM.AdminOrgService);



            //NEF.Library.Utility.Product prod = new Product();
            //prod.Currency = new EntityReference("transactioncurreny", new Guid("822235bc-5a97-e411-80c0-005056a60603"));
            //prod.Project = new EntityReference("new_project", new Guid("f3c2c130-405e-e611-80ff-005056a60603"));
            //SqlDataAccess sda = new SqlDataAccess();
            //sda.openConnection(Globals.ConnectionString);
            //prod.StateCode = new StringMap();
            //prod.StateCode.Name = "StateCode";
            //prod.StateCode.Value = 0;
            //prod.StatusCode = new StringMap();
            //prod.StatusCode.Name = "StatusCode";
            //prod.StatusCode.Value = 100000008;
            //ProductHelper.MakeHouseSearchForActivity(prod, new Guid("4a26dbc1-f92b-e611-80ff-005056a60603"), new Guid("00000000-0000-0000-0000-000000000000"), MSCRM.AdminOrgService, sda);





            //WebFormClient c = new WebFormClient();

            //Webform f = new Webform();
            //f.ChannelOfAwareness = "15";
            //f.ContactPreferences = ContactPreferences.EMail;
            //f.CustomerEmail = "emrah.eroglu@innthebox.com";
            //f.CustomerMobilePhone = "+90-505-4102260";
            //f.CustomerName = "Emrah";
            //f.CustomerSurname = "Eroğlu";
            //f.InterestOfProjectCode = "G1-911-NY-911";
            //f.Message = "TEST KAYIT";
            //f.NefInformation = true;
            //f.SubParticipationSource = "Facebook";
            //f.UTM_Medium = "novu_post_05082016";
            //f.UTM_Campaign = "novu";


            // c.CreateWebForm(f);


            //Console.ReadLine();
            //try
            //{
            //    string authKey = AuthenticationEM();
            //    Console.WriteLine(authKey);
            //    Console.ReadLine();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Console.ReadLine();
            //}
        }


        private static IOrganizationService service;
        public static string projectNameGlobal { get; set; }
        public static string ExecuteContractCover(Guid QuoteId, string Path)
        {
            DateTime deliveryDate;
            string typeOfHome = string.Empty;
            string generalTypeOfHome = string.Empty;

            string deliveryDateString = string.Empty;
            string SalesAccountName = string.Empty;
            string SalesAccountAddress = string.Empty;
            string SalesAccountEmail = string.Empty;
            string SalesAccountMersisno = string.Empty;
            string SalesAccountTel = string.Empty;
            string secondCustomerFirstName = string.Empty;
            string secondCustomerLastName = string.Empty;
            string secondCustomerTc = string.Empty;
            string secondCustomerNumber = string.Empty;
            string projectName = string.Empty;
            string blok = string.Empty;
            string floor = string.Empty;
            string apartmentNo = string.Empty;
            string apartmentCity = string.Empty;
            decimal m2 = 0;
            decimal grossm2 = 0;
            string currencySymbol = string.Empty;
            Guid projectId = Guid.Empty;
            string city = string.Empty;
            string adaPaftaParsel = string.Empty;
            string unitType = string.Empty;
            string apartmentType = string.Empty;
            string location = string.Empty;
            string freeSectionIdNumber = string.Empty;
            string address = string.Empty;
            string passportNumber = string.Empty;
            string foreignAddress = string.Empty;
            string Nationality = string.Empty;
            string CustomerNumber = string.Empty;
            string bbnetalan = string.Empty;
            string bbbrutalan = string.Empty;
            string satisesasalan = string.Empty;
            string bahce = string.Empty;
            string teras = string.Empty;
            string balkon = string.Empty;
            string satisesasalanm2 = string.Empty;
            string bbgenelbrutalan = string.Empty;
            string totalPaymentString = string.Empty;
            string pool = string.Empty;
            string blocktype = string.Empty;
            string etap = string.Empty;
            string deck = string.Empty;
            string court = string.Empty;

            Entity contact = null;
            Entity account = null;
            Entity SalesAccount = null;
            service = MSCRM.AdminOrgService;
            Entity quote = service.Retrieve("quote", QuoteId, new ColumnSet(true));
            Entity currencyDetail = GetCurrencyDetail(((EntityReference)quote["transactioncurrencyid"]).Id, new string[1] { "currencysymbol" });
            if (currencyDetail != null && currencyDetail.Attributes.Contains("currencysymbol"))
                currencySymbol = currencyDetail["currencysymbol"].ToString();

            string folder = CreateFolder(QuoteId, Path);


            if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "contact")
            {
                if (quote.Contains("new_secondcontactid"))
                {
                    Entity secondContact = service.Retrieve("contact", ((EntityReference)quote.Attributes["new_secondcontactid"]).Id, new ColumnSet(true));
                    secondCustomerFirstName = secondContact.Contains("firstname") ? (string)secondContact.Attributes["firstname"] : string.Empty;
                    secondCustomerLastName = secondContact.Contains("lastname") ? (string)secondContact.Attributes["lastname"] : string.Empty;
                    secondCustomerTc = secondContact.Contains("new_tcidentitynumber") ? (string)secondContact.Attributes["new_tcidentitynumber"] : string.Empty;
                    if (secondContact.Contains("new_passportnumber"))
                    {
                        secondCustomerTc = secondCustomerTc + " / " + (string)secondContact.Attributes["new_passportnumber"];
                    }
                    secondCustomerNumber = secondContact.Contains("new_number") ? (string)secondContact.Attributes["new_number"] : string.Empty;
                }


                contact = service.Retrieve("contact", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                city = contact.Contains("new_addresscityid") ? ((EntityReference)contact.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                city += contact.Contains("new_addresstownid") ? ((EntityReference)contact.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                city += contact.Contains("new_addressdistrictid") ? ((EntityReference)contact.Attributes["new_addressdistrictid"]).Name : string.Empty;
                address = contact.Contains("new_addressdetail") ? contact.Attributes["new_addressdetail"].ToString() : string.Empty;
                address = address + " " + city;
                passportNumber = contact.Contains("new_passportnumber") ? (string)contact.Attributes["new_passportnumber"] : string.Empty;
                if (contact.Contains("new_address3countryid"))
                {
                    foreignAddress = contact.Contains("new_nontcidentityaddress") ? contact.Attributes["new_nontcidentityaddress"].ToString() : string.Empty;
                    foreignAddress += " " + ((EntityReference)contact.Attributes["new_address3cityid"]).Name + "/" + ((EntityReference)contact.Attributes["new_address3countryid"]).Name;
                }
                if (contact.Contains("new_nationalityid"))
                {
                    Nationality = ((EntityReference)contact.Attributes["new_nationalityid"]).Name;
                }
                CustomerNumber = contact.Contains("new_number") ? contact.Attributes["new_number"].ToString() : string.Empty;
                CustomerNumber += secondCustomerNumber != string.Empty ? " - " + secondCustomerNumber : string.Empty;

            }
            else if (((EntityReference)quote.Attributes["customerid"]).LogicalName.ToLower() == "account")
            {
                account = service.Retrieve("account", ((EntityReference)quote.Attributes["customerid"]).Id, new ColumnSet(true));
                city = account.Contains("new_addresscityid") ? ((EntityReference)account.Attributes["new_addresscityid"]).Name + "/" : string.Empty + "/";
                city += account.Contains("new_addresstownid") ? ((EntityReference)account.Attributes["new_addresstownid"]).Name + "/" : string.Empty + "/";
                city += account.Contains("new_addressdistrictid") ? ((EntityReference)account.Attributes["new_addressdistrictid"]).Name : string.Empty;
                address = account.Contains("new_addressdetail") ? account.Attributes["new_addressdetail"].ToString() : string.Empty;
                address = address + " " + city;
            }




            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);

            QueryExpression Query = new QueryExpression("quotedetail");
            Query.ColumnSet = new ColumnSet("productid");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);

            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                Entity product = service.Retrieve("product", ((EntityReference)Result.Entities[0].Attributes["productid"]).Id, new ColumnSet(true));
                projectId = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Id : Guid.Empty;
                projectName = product.Contains("new_projectid") ? ((EntityReference)product.Attributes["new_projectid"]).Name : string.Empty;
                projectNameGlobal = projectName;
                blok = product.Contains("new_blockid") ? ((EntityReference)product.Attributes["new_blockid"]).Name : string.Empty;
                floor = product.Contains("new_floornumber") ? product.Attributes["new_floornumber"].ToString() : string.Empty;
                apartmentNo = product.Contains("new_homenumber") ? (string)product.Attributes["new_homenumber"] : string.Empty;
                m2 = product.Contains("new_netm2") ? (decimal)product.Attributes["new_netm2"] : 0;
                grossm2 = product.Contains("new_grossm2") ? (decimal)product.Attributes["new_grossm2"] : 0;
                adaPaftaParsel = product.Contains("new_blockofbuildingid") ? ((EntityReference)product.Attributes["new_blockofbuildingid"]).Name + "/" : string.Empty + "/";
                adaPaftaParsel += product.Contains("new_threaderid") ? ((EntityReference)product.Attributes["new_threaderid"]).Name + "/" : string.Empty + "/";
                adaPaftaParsel += product.Contains("new_parcelid") ? ((EntityReference)product.Attributes["new_parcelid"]).Name : string.Empty;
                apartmentCity = product.Contains("new_city") ? (string)product.Attributes["new_city"] + "/" : string.Empty + "/";
                apartmentCity += product.Contains("new_district") ? (string)product.Attributes["new_district"] + "/" : string.Empty + "/";
                apartmentCity += product.Contains("new_quarter") ? (string)product.Attributes["new_quarter"] : string.Empty;
                unitType = product.Contains("new_unittypeid") ? ((EntityReference)product.Attributes["new_unittypeid"]).Name : string.Empty;
                apartmentType = product.Contains("new_generaltypeofhomeid") ? ((EntityReference)product.Attributes["new_generaltypeofhomeid"]).Name : string.Empty;
                location = product.Contains("new_locationid") ? ((EntityReference)product.Attributes["new_locationid"]).Name : string.Empty;
                freeSectionIdNumber = product.Contains("new_freesectionidnumber") ? (string)product.Attributes["new_freesectionidnumber"] : string.Empty;
                bbnetalan = product.Contains("new_bbnetarea") ? ((decimal)product.Attributes["new_bbnetarea"]).ToString("N2") : string.Empty;
                bbbrutalan = product.Contains("new_netm2") ? ((decimal)product.Attributes["new_netm2"]).ToString("N2") : string.Empty;
                satisesasalan = product.Contains("new_satisaesasalan") ? ((decimal)product.Attributes["new_satisaesasalan"]).ToString("N2") : string.Empty;
                bahce = product.Contains("new_garden") ? ((decimal)product.Attributes["new_garden"]).ToString("N2") : " - ";
                teras = product.Contains("new_terracegross") ? ((decimal)product.Attributes["new_terracegross"]).ToString("N2") : " - ";
                balkon = product.Contains("new_balconym2") ? ((decimal)product.Attributes["new_balconym2"]).ToString("N2") : " - ";
                satisesasalanm2 = product.Contains("new_grossm2") ? ((decimal)product.Attributes["new_grossm2"]).ToString("N2") : " - ";
                bbgenelbrutalan = product.Contains("new_bbgeneralgrossarea") ? ((decimal)product.Attributes["new_bbgeneralgrossarea"]).ToString("N2") : string.Empty;
                deliveryDateString = product.Contains("new_deliverydate") ? ((DateTime)product.Attributes["new_deliverydate"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                typeOfHome = product.Contains("new_typeofhomeid") ? ((EntityReference)product.Attributes["new_typeofhomeid"]).Name : string.Empty;
                generalTypeOfHome = product.Contains("new_generaltypeofhomeid") ? ((EntityReference)product.Attributes["new_generaltypeofhomeid"]).Name : string.Empty;
                blocktype = product.Contains("new_blocktypeid") ? ((EntityReference)product.Attributes["new_blocktypeid"]).Name : string.Empty;
                pool = product.Contains("new_poolm2") ? ((decimal)product.Attributes["new_poolm2"]).ToString("N2") : " - ";

                etap = product.Contains("new_etapid") ? ((EntityReference)product.Attributes["new_etapid"]).Name : string.Empty;
                deck = product.Contains("new_deckm2") ? ((decimal)product.Attributes["new_deckm2"]).ToString("N2") : string.Empty;
                court = product.Contains("new_courtm2") ? ((decimal)product.Attributes["new_courtm2"]).ToString("N2") : string.Empty;

                if (product.Contains("new_deliverydate"))
                {
                    deliveryDate = product.GetAttributeValue<DateTime>("new_deliverydate");
                    decimal totalPayment = GetTotalPaymentOnOrBeforeProjectDate(QuoteId, deliveryDate);
                    totalPaymentString = totalPayment.ToString("N2");
                }
            }
            Entity project = service.Retrieve("new_project", projectId, new ColumnSet(true));





            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(etap))
            {
                dictionary1.Add("New_etapidname", etap);
            }
            else
            {
                dictionary1.Add("New_etapidname", string.Empty);
            }


            if (!string.IsNullOrEmpty(deck))
            {
                dictionary1.Add("new_deckm2", deck);
            }
            else
            {
                dictionary1.Add("new_deckm2", string.Empty);
            }

            if (!string.IsNullOrEmpty(court))
            {
                dictionary1.Add("new_courtm2", court);
            }
            else
            {
                dictionary1.Add("new_courtm2", string.Empty);
            }

            if (!string.IsNullOrEmpty(bbnetalan))
                dictionary1.Add("bbnetalan", bbnetalan);
            else
                dictionary1.Add("bbnetalan", string.Empty);

            if (!string.IsNullOrEmpty(pool))
                dictionary1.Add("pool", pool);
            else
                dictionary1.Add("pool", string.Empty);

            if (!string.IsNullOrEmpty(blocktype))
                dictionary1.Add("blocktype", blocktype);
            else
                dictionary1.Add("blocktype", string.Empty);

            if (!string.IsNullOrEmpty(bbbrutalan))
                dictionary1.Add("bbbrutalan", bbbrutalan);
            else
                dictionary1.Add("bbbrutalan", string.Empty);

            if (!string.IsNullOrEmpty(satisesasalan))
                dictionary1.Add("satisesasalan", satisesasalan);
            else
                dictionary1.Add("satisesasalan", string.Empty);

            if (!string.IsNullOrEmpty(bahce))
                dictionary1.Add("bahce", bahce);
            else
                dictionary1.Add("bahce", string.Empty);

            if (!string.IsNullOrEmpty(teras))
                dictionary1.Add("teras", teras);
            else
                dictionary1.Add("teras", string.Empty);

            if (!string.IsNullOrEmpty(balkon))
                dictionary1.Add("balkon", balkon);
            else
                dictionary1.Add("balkon", string.Empty);

            if (!string.IsNullOrEmpty(satisesasalanm2))
                dictionary1.Add("satisesasalanm2", satisesasalanm2);
            else
                dictionary1.Add("satisesasalanm2", string.Empty);

            if (!string.IsNullOrEmpty(bbgenelbrutalan))
                dictionary1.Add("bbgenelbrutalan", bbgenelbrutalan);
            else
                dictionary1.Add("bbgenelbrutalan", string.Empty);


            if (quote.Contains("new_salesshareaccountid"))//Satışı Yapan Firma
            {
                SalesAccount = service.Retrieve("new_share", ((EntityReference)quote.Attributes["new_salesshareaccountid"]).Id, new ColumnSet(true));
                SalesAccountName = SalesAccount.Contains("new_name") ? SalesAccount.Attributes["new_name"].ToString() : string.Empty;
                SalesAccountAddress = SalesAccount.Contains("new_adressdetail") ? SalesAccount.Attributes["new_adressdetail"].ToString() : string.Empty;
                SalesAccountEmail = SalesAccount.Contains("new_emailaddress") ? SalesAccount.Attributes["new_emailaddress"].ToString() : string.Empty;
                SalesAccountMersisno = SalesAccount.Contains("new_mersisnumber") ? SalesAccount.Attributes["new_mersisnumber"].ToString() : string.Empty;
                SalesAccountTel = SalesAccount.Contains("new_phonenumber") ? SalesAccount.Attributes["new_phonenumber"].ToString() : string.Empty;
            }
            if (SalesAccount != null)
            {
                if (SalesAccountName != string.Empty)
                    dictionary1.Add("accountname", SalesAccountName);
                else
                    dictionary1.Add("accountname", string.Empty);
                if (SalesAccountAddress != string.Empty)
                    dictionary1.Add("accountaddress", SalesAccountAddress);
                else
                    dictionary1.Add("accountaddress", string.Empty);
                if (SalesAccountTel != string.Empty)
                    dictionary1.Add("accounttelephone", SalesAccountTel);
                else
                    dictionary1.Add("accounttelephone", string.Empty);
                if (SalesAccountEmail != string.Empty)
                    dictionary1.Add("accountemail", SalesAccountEmail);
                else
                    dictionary1.Add("accountemail", string.Empty);
                if (SalesAccountMersisno != string.Empty)
                    dictionary1.Add("accountmersisno", SalesAccountMersisno);
                else
                    dictionary1.Add("accountmersisno", string.Empty);
            }

            dictionary1.Add("teslimtarihi", deliveryDateString);
            dictionary1.Add("fiyatlabel", "Satış Fiyatı");
            dictionary1.Add("kdvlabel", "KDV Tutarı");
            dictionary1.Add("parabirimi", currencySymbol);
            dictionary1.Add("projeadi", projectName);
            dictionary1.Add("konutkimlik", projectName);
            dictionary1.Add("blok", blok);
            dictionary1.Add("kat", floor);
            dictionary1.Add("bulundugukat", floor);
            dictionary1.Add("no", apartmentNo);
            dictionary1.Add("daireno", apartmentNo);
            dictionary1.Add("m", grossm2.ToString("N2"));
            dictionary1.Add("satisdanismani", ((EntityReference)quote.Attributes["ownerid"]).Name.ToString());
            dictionary1.Add("musterino", CustomerNumber);
            dictionary1.Add("il", apartmentCity);
            dictionary1.Add("adapaftaparsel", adaPaftaParsel);
            dictionary1.Add("unitetipi", unitType);
            dictionary1.Add("oda", apartmentType);
            dictionary1.Add("konum", location);
            dictionary1.Add("bolumno", freeSectionIdNumber);
            dictionary1.Add("net", m2.ToString("N2"));
            dictionary1.Add("brut", grossm2.ToString("N2"));
            dictionary1.Add("onodemetutaritoplami", totalPaymentString);
            if (foreignAddress != string.Empty)
            {
                dictionary1.Add("evadresi", foreignAddress);
                dictionary1.Add("adres", foreignAddress);
                dictionary1.Add("isadresi", string.Empty);
            }
            else
            {
                dictionary1.Add("evadresi", address);
                dictionary1.Add("isadresi", string.Empty);
                dictionary1.Add("adres", address);
            }

            if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(Nationality) && Nationality != "TC")
            {
                dictionary1.Add("tcadresi", address);
                dictionary1.Add("tebligatadresi2", string.Empty);
            }
            else
            {
                dictionary1.Add("tcadresi", string.Empty);
                dictionary1.Add("tebligatadresi2", address);
            }







            if (quote.Contains("new_contractnumber"))
            {
                dictionary1.Add("sozlesmeno", quote.Attributes["new_contractnumber"].ToString());
            }
            else
            {
                dictionary1.Add("sozlesmeno", string.Empty);
            }
            if (contact != null)
            {
                dictionary1.Add("alicino", "T.C. Kimlik / Pasaport No:");
                if (contact.Contains("firstname"))
                {
                    if (secondCustomerFirstName != string.Empty)
                    {
                        dictionary1.Add("ad", contact.Attributes["firstname"].ToString() + " - " + secondCustomerFirstName);
                    }
                    else
                    {
                        dictionary1.Add("ad", contact.Attributes["firstname"].ToString());
                    }

                }
                else
                {
                    dictionary1.Add("ad", string.Empty);
                }
                if (contact.Contains("lastname"))
                {
                    if (secondCustomerLastName != string.Empty)
                    {
                        dictionary1.Add("soyad", contact.Attributes["lastname"].ToString() + " - " + secondCustomerLastName);
                    }
                    else
                    {
                        dictionary1.Add("soyad", contact.Attributes["lastname"].ToString());
                    }

                }
                else
                {
                    dictionary1.Add("soyad", string.Empty);
                }
                if (contact.Contains("new_tcidentitynumber"))
                {
                    if (secondCustomerTc != string.Empty)
                    {
                        if (passportNumber != string.Empty)
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString() + "/" + passportNumber + " - " + secondCustomerTc);
                        }
                        else
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString() + " - " + secondCustomerTc);
                        }
                    }
                    else
                    {
                        if (passportNumber != string.Empty)
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString() + "/" + passportNumber);
                        }
                        else
                        {
                            dictionary1.Add("tckimlik", contact.Attributes["new_tcidentitynumber"].ToString());
                        }
                    }

                }
                else
                {
                    dictionary1.Add("tckimlik", passportNumber);
                }

                if (contact.Contains("fullname"))
                {
                    if (secondCustomerFirstName != string.Empty)
                    {
                        dictionary1.Add("adsoyad", contact.Attributes["fullname"].ToString() + "- " + secondCustomerFirstName + " " + secondCustomerLastName);
                        dictionary1.Add("cari", contact.Attributes["fullname"].ToString() + "- " + secondCustomerFirstName + " " + secondCustomerLastName);
                    }
                    else
                    {
                        dictionary1.Add("adsoyad", contact.Attributes["fullname"].ToString());
                        dictionary1.Add("cari", contact.Attributes["fullname"].ToString());
                    }

                }
                else
                {
                    dictionary1.Add("adsoyad", string.Empty);
                    dictionary1.Add("cari", string.Empty);

                }
                if (contact.Contains("mobilephone"))
                {
                    dictionary1.Add("ceptelefonu", contact.Attributes["mobilephone"].ToString());
                }
                else
                {
                    dictionary1.Add("ceptelefonu", string.Empty);
                }
                if (contact.Contains("emailaddress1"))
                {
                    dictionary1.Add("epostaadresi3", contact.Attributes["emailaddress1"].ToString());
                }
                else
                {
                    dictionary1.Add("epostaadresi3", string.Empty);
                }
            }
            else if (account != null)
            {
                dictionary1.Add("alicino", "Vergi Dairesi / Vergi No:");
                dictionary1.Add("soyad", string.Empty);
                string taxOfficeNumber = string.Empty;
                if (account.Contains("telephone1"))
                {
                    dictionary1.Add("ceptelefonu", account.Attributes["telephone1"].ToString());
                }
                else
                {
                    dictionary1.Add("ceptelefonu", string.Empty);
                }
                if (account.Contains("emailaddress1"))
                {
                    dictionary1.Add("epostaadresi3", account.Attributes["emailaddress1"].ToString());
                }
                else
                {
                    dictionary1.Add("epostaadresi3", string.Empty);
                }
                if (account.Contains("name"))
                {
                    dictionary1.Add("ad", account.Attributes["name"].ToString());
                    dictionary1.Add("cari", account.Attributes["name"].ToString());
                    dictionary1.Add("adsoyad", account.Attributes["name"].ToString());
                }
                else
                {
                    dictionary1.Add("ad", string.Empty);
                    dictionary1.Add("cari", string.Empty);
                    dictionary1.Add("adsoyad", string.Empty);
                }

                if (account.Contains("new_taxofficeid"))
                {
                    taxOfficeNumber = ((EntityReference)account.Attributes["new_taxofficeid"]).Name.ToString() + "/";
                }
                if (account.Contains("new_taxnumber"))
                {
                    taxOfficeNumber += account.Attributes["new_taxnumber"].ToString();
                }
                dictionary1.Add("tckimlik", taxOfficeNumber);
            }

            if (quote.Contains("new_contractdate"))
            {
                dictionary1.Add("sozlemetarihi", ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
                dictionary1.Add("tarih", ((DateTime)quote.Attributes["new_contractdate"]).ToLocalTime().ToString("dd/MM/yyyy"));

            }
            else
            {
                dictionary1.Add("sozlemetarihi", string.Empty);
            }
            if (quote.Contains("totallineitemamount"))
            {
                dictionary1.Add("dairefiyat", ((Money)quote.Attributes["totallineitemamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("dairefiyat", string.Empty);
            }
            if (quote.Contains("totalamountlessfreight"))
            {
                dictionary1.Add("satisfiyati2", ((Money)quote.Attributes["totalamountlessfreight"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("satisfiyati2", string.Empty);
            }
            if (grossm2 > 0)
            {
                dictionary1.Add("satisfiyat", (((Money)quote.Attributes["totalamountlessfreight"]).Value / grossm2).ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("satisfiyat", string.Empty);
            }
            if (quote.Contains("discountpercentage"))
            {
                decimal totalAmount = ((Money)quote.Attributes["totallineitemamount"]).Value;
                dictionary1.Add("oran", ((decimal)quote.Attributes["discountpercentage"]).ToString("N2"));
                dictionary1.Add("miktar", ((totalAmount * (decimal)quote.Attributes["discountpercentage"]) / 100).ToString("N2") + " " + currencySymbol);
            }
            else if (quote.Contains("discountamount"))
            {
                decimal totalAmount = ((Money)quote.Attributes["totallineitemamount"]).Value;
                dictionary1.Add("miktar", ((Money)quote.Attributes["discountamount"]).Value.ToString("N2") + " " + currencySymbol);
                dictionary1.Add("oran", ((((Money)quote.Attributes["discountamount"]).Value / totalAmount) * 100).ToString("N2"));
            }
            else
            {
                dictionary1.Add("oran", string.Empty);
                dictionary1.Add("miktar", string.Empty);
            }

            if (quote.Contains("new_paymentplan") && (bool)quote.Attributes["new_paymentplan"])
            {
                dictionary1.Add("satissekli", "Kredisiz Ödeme Planı");
            }
            else
            {
                dictionary1.Add("satissekli", "Banka Kredili Ödeme Planı");
            }
            if (quote.Contains("new_taxrate") && quote.Contains("new_containstax"))
            {
                if (!(bool)quote["new_containstax"])
                {
                    dictionary1.Add("kdvyuzde", ((decimal)quote.Attributes["new_taxrate"]).ToString("N2"));
                }
                else
                {
                    dictionary1.Add("kdvyuzde", string.Empty);
                }
            }
            else
            {
                dictionary1.Add("kdvyuzde", string.Empty);
            }
            if (quote.Contains("new_taxamount"))
            {
                dictionary1.Add("kdvtutari", ((Money)quote.Attributes["new_taxamount"]).Value.ToString("N2") + " " + currencySymbol);
            }
            else
            {
                dictionary1.Add("kdvtutari", string.Empty);
            }

            //Topkapı projesinde çıktı ne olursa olsun yazması gerekiyor. Bu nedenle kontrolden çıkarıldı.
            if (projectName == "827 Inistanbul Topkapı")
            {
                if (quote.Contains("new_taxofstamp"))
                {
                    dictionary1.Add("damgavergisiyuzde", ((decimal)quote.Attributes["new_taxofstamp"]).ToString("N2"));
                }
                else
                {
                    dictionary1.Add("damgavergisiyuzde", string.Empty);
                }
                if (quote.Contains("new_taxofstamp"))
                {
                    decimal taxRate = ((decimal)quote.Attributes["new_taxofstamp"]);
                    decimal totalAmount = ((Money)quote.Attributes["totalamount"]).Value;

                    dictionary1.Add("damgavergisi", ((totalAmount * taxRate) / 100).ToString("N2") + " " + currencySymbol);
                }
                else
                {
                    dictionary1.Add("damgavergisi", "0.00" + " " + currencySymbol);
                }
            }
            else
            {
                if (quote.Contains("new_taxofstamp") && (!quote.Contains("new_isnotarizedsales") || !(bool)quote.Attributes["new_isnotarizedsales"]))
                {
                    dictionary1.Add("damgavergisiyuzde", ((decimal)quote.Attributes["new_taxofstamp"]).ToString("N2"));
                }
                else
                {
                    dictionary1.Add("damgavergisiyuzde", string.Empty);
                }
                if (quote.Contains("new_taxofstamp") && (!quote.Contains("new_isnotarizedsales") || !(bool)quote.Attributes["new_isnotarizedsales"]))
                {
                    decimal taxRate = ((decimal)quote.Attributes["new_taxofstamp"]);
                    decimal totalAmount = ((Money)quote.Attributes["totalamount"]).Value;

                    dictionary1.Add("damgavergisi", ((totalAmount * taxRate) / 100).ToString("N2") + " " + currencySymbol);
                }
                else
                {
                    dictionary1.Add("damgavergisi", "0.00" + " " + currencySymbol);
                }
            }

            decimal _taxAmount = quote.Contains("new_taxamount") ? ((Money)quote.Attributes["new_taxamount"]).Value : 0;
            decimal _totalAmount = quote.Contains("totalamount") ? ((Money)quote.Attributes["totalamount"]).Value : 0;
            decimal _taxStampRate = (!quote.Contains("new_isnotarizedsales") || !(bool)quote.Attributes["new_isnotarizedsales"]) ? quote.Contains("new_taxofstamp") ? ((decimal)quote.Attributes["new_taxofstamp"]) : 0 : 0;
            decimal _taxStampAmount = ((_totalAmount * _taxStampRate) / 100);





            if (project.Contains("new_annualratio"))
            {
                dictionary1.Add("yillikfaizoran", "%" + ((decimal)project.Attributes["new_annualratio"]).ToString("N2"));
            }
            else
            {
                dictionary1.Add("yillikfaizoran", "%0");
            }
            if (project.Contains("new_buildingpermitsdate"))
            {
                dictionary1.Add("yapiruhsattarihi", ((DateTime)project.Attributes["new_buildingpermitsdate"]).ToLocalTime().ToString("dd/MM/yyyy"));
            }
            else
            {
                dictionary1.Add("yapiruhsattarihi", string.Empty);
            }
            if (project.Contains("new_guaranteeinfo"))
            {
                dictionary1.Add("teminatbilgisi", project.Attributes["new_guaranteeinfo"].ToString());
            }
            else
            {
                dictionary1.Add("teminatbilgisi", string.Empty);
            }
            decimal totalBankCredit = GetPaymentTotalByType(QuoteId, 9);//Banka Kredisi
            dictionary1.Add("krediodemetutari", totalBankCredit.ToString("N2") + " " + currencySymbol);

            DataTable dtPaymentPlan = new DataTable();
            dtPaymentPlan.TableName = "snt";
            dtPaymentPlan.Columns.Add("tarih", System.Type.GetType("System.DateTime"));
            dtPaymentPlan.Columns.Add("text");
            dtPaymentPlan.Columns.Add("tip");
            dtPaymentPlan.Columns.Add("tutar");

            // EntityCollection Kapora = GetCollectionPaymentTotalByType(QuoteId, 4);//Kapora Ödemesi
            decimal _totalKapora = 0;
            if (quote.Contains("new_prepaymentdate") && quote.Contains("new_prepaymentamount"))
            {
                for (int i = 0; i < 1; i++)
                {
                    DataRow row = dtPaymentPlan.NewRow();
                    row["tarih"] = quote.Contains("new_prepaymentdate") ? Convert.ToDateTime(((DateTime)quote.Attributes["new_prepaymentdate"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                    row["text"] = "Tarihinde";
                    row["tip"] = "Ödenen Ön Ödeme Tutarı";
                    row["tutar"] = quote.Contains("new_prepaymentamount") ? ((Money)quote.Attributes["new_prepaymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                    _totalKapora = quote.Contains("new_prepaymentamount") ? ((Money)quote.Attributes["new_prepaymentamount"]).Value : 0;
                    dtPaymentPlan.Rows.Add(row);
                }
            }


            EntityCollection KDV = GetCollectionPaymentTotalByType(QuoteId, 6);//KDV
            decimal _totalKDV = 0;
            foreach (Entity p in KDV.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Tarihinde";
                row["tip"] = "Ödenecek KDV Tutarı";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalKDV += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection DamgaVergisi = GetCollectionPaymentTotalByType(QuoteId, 7);//Damga Vergisi
            decimal _totalDamgaVergisi = 0;
            foreach (Entity p in DamgaVergisi.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Tarihinde";
                row["tip"] = "Ödenecek Damga Vergisi Tutarı";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalDamgaVergisi += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection PesinOdeme = GetCollectionPaymentTotalByType(QuoteId, 3);//Peşin Ödeme
            decimal _totalPesinOdeme = 0;
            foreach (Entity p in PesinOdeme.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Tarihinde";
                row["tip"] = "Ödenen Peşin Ödeme Tutarı";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalPesinOdeme += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection BankaKredisi = GetCollectionPaymentTotalByType(QuoteId, 9);//Banka Kredisi
            decimal _totalBankaKredisi = 0;
            foreach (Entity p in BankaKredisi.Entities)
            {
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Tarihinde";
                row["tip"] = "Ödenecek Banka Kredisi Tutarı";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                _totalBankaKredisi += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                dtPaymentPlan.Rows.Add(row);
            }

            EntityCollection AraOdeme = GetCollectionPaymentTotalByType(QuoteId, 1);//Ara Ödeme
            decimal _totalAraOdeme = 0;
            string _dateAraOdeme = string.Empty;
            foreach (Entity p in AraOdeme.Entities)
            {
                _totalAraOdeme += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                //if (AraOdeme.Entities.IndexOf(p) == 0)
                //{
                //    _dateAraOdeme = p.Contains("new_date") ? ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                //}
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Tarihinde";
                row["tip"] = "Ödenecek Ara Ödeme Tutarı";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                dtPaymentPlan.Rows.Add(row);
            }
            //if (_totalAraOdeme > 0)
            //{
            //    DataRow row = dtPaymentPlan.NewRow();
            //    row["tarih"] = _dateAraOdeme;
            //    row["text"] = "Tarihinde";
            //    row["tip"] = "Ödenecek Ara Ödeme Tutarı";
            //    row["tutar"] = _totalAraOdeme.ToString("N2") + " " + currencySymbol;
            //    dtPaymentPlan.Rows.Add(row);
            //}

            EntityCollection DuzenliTaksit = GetCollectionPaymentTotalByType(QuoteId, 2);//Düzenli Taksit
            decimal _totalDuzenliTaksit = 0;
            string _dateDuzenliTaksit = string.Empty;
            foreach (Entity p in DuzenliTaksit.Entities)
            {
                _totalDuzenliTaksit += p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value : 0;
                //if (DuzenliTaksit.Entities.IndexOf(p) == 0)
                //{
                //    _dateDuzenliTaksit = p.Contains("new_date") ? ((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy") : string.Empty;
                //}
                DataRow row = dtPaymentPlan.NewRow();
                row["tarih"] = p.Contains("new_date") ? Convert.ToDateTime(((DateTime)p.Attributes["new_date"]).ToLocalTime().ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("tr-TR")) : DateTime.MinValue;
                row["text"] = "Tarihinde";
                row["tip"] = "Ödenecek Düzenli Taksit Tutarı";
                row["tutar"] = p.Contains("new_paymentamount") ? ((Money)p.Attributes["new_paymentamount"]).Value.ToString("N2") + " " + currencySymbol : string.Empty;
                dtPaymentPlan.Rows.Add(row);
            }
            //if (_totalDuzenliTaksit > 0)
            //{
            //    DataRow row = dtPaymentPlan.NewRow();
            //    row["tarih"] = _dateDuzenliTaksit;
            //    row["text"] = "Tarihinde";
            //    row["tip"] = "Ödenecek Düzenli Taksit Tutarı";
            //    row["tutar"] = _totalDuzenliTaksit.ToString("N2") + " " + currencySymbol;
            //    dtPaymentPlan.Rows.Add(row);
            //}

            dictionary1.Add("odemelertoplami", (_totalDuzenliTaksit + _totalAraOdeme + _totalBankaKredisi + _totalPesinOdeme + _totalDamgaVergisi + _totalKDV + _totalKapora).ToString("N2"));





            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dtPaymentPlan);
            dataSet.Tables[0].DefaultView.Sort = "tarih ASC";

            DataTable dt = dataSet.Tables[0].DefaultView.ToTable();
            dataSet.Tables[0].Rows.Clear();

            DataTable dtPaymentPlanSorted = new DataTable();
            dtPaymentPlanSorted.TableName = "snt";
            dtPaymentPlanSorted.Columns.Add("tarih", System.Type.GetType("System.String"));
            dtPaymentPlanSorted.Columns.Add("text");
            dtPaymentPlanSorted.Columns.Add("tip");
            dtPaymentPlanSorted.Columns.Add("tutar");
            DataSet dataSetSorted = new DataSet();
            dataSetSorted.Tables.Add(dtPaymentPlanSorted);

            foreach (DataRow row in dt.Rows)
            {
                dataSetSorted.Tables[0].Rows.Add(row.ItemArray);
            }
            foreach (DataRow row in dtPaymentPlanSorted.Rows)
            {
                dataSetSorted.Tables[0].Rows[dtPaymentPlanSorted.Rows.IndexOf(row)][0] = Convert.ToDateTime(row.ItemArray[0]).ToString("dd/MM/yyyy");
            }

            if (projectName == "895 NEF Yalıkavak")
            {
                byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\SozlesmeKapagiY.docx", dataSetSorted, dictionary1);
                string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\SozlesmeKapagiY.docx";
                if (path1 != string.Empty)
                    System.IO.File.WriteAllBytes(path1, bytes);
                return path1;
            }
            else
            {

                if (SalesAccount != null && projectName != "827 Inistanbul Topkapı")
                {
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\SozlesmeKapagi2.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\SozlesmeKapagi2.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;

                }
                else if (SalesAccount == null && projectName != "827 Inistanbul Topkapı")
                {
                    byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\SozlesmeKapagi.docx", dataSetSorted, dictionary1);
                    string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\SozlesmeKapagi.docx";
                    if (path1 != string.Empty)
                        System.IO.File.WriteAllBytes(path1, bytes);
                    return path1;

                }
                else if (projectName == "827 Inistanbul Topkapı")//TOPKAPI PROJESİ
                {
                    if (typeOfHome.Equals("G") && generalTypeOfHome.Equals("2+1"))
                    {
                        byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\TopkapiSozlesmeKapagiG.docx", dataSetSorted, dictionary1);
                        string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\TopkapiSozlesmeKapagiG.docx";
                        if (path1 != string.Empty)
                            System.IO.File.WriteAllBytes(path1, bytes);
                        return path1;
                    }
                    else
                    {
                        byte[] bytes = DocumentMerge.WordDokumanOlustur(Path + "DocumentMerge\\Templates\\TopkapiSozlesmeKapagi.docx", dataSetSorted, dictionary1);
                        string path1 = Path + "DocumentMerge\\Document\\" + folder + "\\TopkapiSozlesmeKapagi.docx";
                        if (path1 != string.Empty)
                            System.IO.File.WriteAllBytes(path1, bytes);
                        return path1;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        private static string CreateFolder(Guid QuoteId, string Path)
        {
            string str1 = QuoteId.ToString();
            if (!Directory.Exists(Path + "\\DocumentMerge"))
                Directory.CreateDirectory(Path + "\\DocumentMerge");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document"))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document");
            if (!Directory.Exists(Path + "\\DocumentMerge\\Document\\" + str1))
                Directory.CreateDirectory(Path + "\\DocumentMerge\\Document\\" + str1);
            return str1;
        }
        private static Entity GetCurrencyDetail(Guid id, string[] Columns)
        {
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = "transactioncurrencyid";
            conditionExpression.Operator = ConditionOperator.Equal;
            conditionExpression.Values.Add((object)id);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(conditionExpression);
            filterExpression.FilterOperator = LogicalOperator.And;
            ColumnSet columnSet = new ColumnSet();
            columnSet.AddColumns(Columns);
            RetrieveMultipleResponse multipleResponse = (RetrieveMultipleResponse)service.Execute((OrganizationRequest)new RetrieveMultipleRequest()
            {
                Query = (QueryBase)new QueryExpression()
                {
                    ColumnSet = columnSet,
                    Criteria = filterExpression,
                    EntityName = "transactioncurrency"
                }
            });
            if (multipleResponse.EntityCollection.Entities != null && multipleResponse.EntityCollection.Entities.Count > 0)
                return Enumerable.First<Entity>((IEnumerable<Entity>)multipleResponse.EntityCollection.Entities);
            else
                return (Entity)null;
        }
        private static decimal GetPaymentTotalByType(Guid QuoteId, int Type)
        {
            decimal bankCredit = 0;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);


            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_type";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(Type);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    bankCredit += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return bankCredit;
        }


        private static decimal GetTotalPaymentOnOrBeforeProjectDate(Guid QuoteId, DateTime date)
        {
            decimal bankCredit = 0;

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "new_date";
            con2.Operator = ConditionOperator.OnOrBefore;
            con2.Values.Add(date);

            //ConditionExpression con3 = new ConditionExpression();
            //con3.AttributeName = "new_type";
            //con3.Operator = ConditionOperator.NotEqual;
            //con3.Values.Add(6);

            //ConditionExpression con4 = new ConditionExpression();
            //con4.AttributeName = "new_type";
            //con4.Operator = ConditionOperator.NotEqual;
            //con4.Values.Add(7);


            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            //filter.Conditions.Add(con3);
            //filter.Conditions.Add(con4);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    bankCredit += ((Money)p.Attributes["new_paymentamount"]).Value;
                }
            }
            return bankCredit;
        }


        private static EntityCollection GetCollectionPaymentTotalByType(Guid QuoteId, int Type)
        {
            EntityCollection col = new EntityCollection();

            ConditionExpression con1 = new ConditionExpression();
            con1.AttributeName = "new_quoteid";
            con1.Operator = ConditionOperator.Equal;
            con1.Values.Add(QuoteId);

            ConditionExpression con2 = new ConditionExpression();
            con2.AttributeName = "statecode";
            con2.Operator = ConditionOperator.Equal;
            con2.Values.Add(0);


            ConditionExpression con3 = new ConditionExpression();
            con3.AttributeName = "new_type";
            con3.Operator = ConditionOperator.Equal;
            con3.Values.Add(Type);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(con1);
            filter.Conditions.Add(con2);
            filter.Conditions.Add(con3);

            QueryExpression Query = new QueryExpression("new_payment");
            Query.ColumnSet = new ColumnSet("new_paymentamount", "new_date");
            Query.Criteria.FilterOperator = LogicalOperator.And;
            Query.Criteria.Filters.Add(filter);
            Query.AddOrder("new_date", OrderType.Descending);
            EntityCollection Result = service.RetrieveMultiple(Query);
            if (Result.Entities.Count > 0)
            {
                foreach (Entity p in Result.Entities)
                {
                    col.Entities.Add(p);
                }
            }
            return col;
        }



    }
}

