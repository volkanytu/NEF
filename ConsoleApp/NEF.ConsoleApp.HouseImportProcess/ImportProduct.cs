using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using NEF.Library.Business;
using Microsoft.Xrm.Sdk;
using System.Data;
using Microsoft.Crm.Sdk.Messages;

namespace NEF.ConsoleApp.HouseImportProcess
{
    public static class ImportProduct
    {

        public static MsCrmResult Process()
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                int totalCount = 0;
                int errorCount = 0;
                int successCount = 0;
                StringBuilder sb = new StringBuilder();
                IOrganizationService service = MSCRM.GetOrgService(true);

                SqlDataAccess sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                List<HouseImport> lstImports = ProductHelper.GetNotImportedList(sda);

                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Listeler çekildi. Adet:" + lstImports.Count.ToString());

                if (lstImports.Count > 0)
                {
                    for (int i = 0; i < lstImports.Count; i++)
                    {
                        Console.SetCursorPosition(0, 2);
                        Console.WriteLine((i + 1).ToString() + ". Liste İşleniyor.");

                        totalCount = 0;
                        errorCount = 0;
                        successCount = 0;

                        MsCrmResult resultProcessing = ProductHelper.UpdateImportListToProcessing(lstImports[i].HouseImportId, service);

                        if (resultProcessing.Success)
                        {
                            DataTable dt = ProductHelper.GetHouseImportListDetail(lstImports[i].HouseImportId, sda);

                            Console.SetCursorPosition(0, 3);
                            Console.WriteLine((i + 1).ToString() + ". Liste Kayıt sayısı: " + dt.Rows.Count.ToString());

                            if (dt.Rows.Count > 0)
                            {
                                totalCount = dt.Rows.Count;

                                for (int j = 0; j < totalCount; j++)
                                {
                                    Console.SetCursorPosition(0, 4);
                                    Console.WriteLine("Sayaç:" + (j + 1).ToString());

                                    #region | PRODUCT IMPORT OPERATIONS |
                                    try
                                    {
                                        Guid existProductId = Guid.Empty;
                                        Guid newProductId = Guid.Empty;

                                        MsCrmResult resultProductCheck = GeneralHelper.GetEntityIdByAttributeName("Product", "Name", dt.Rows[j]["Daire Kimlik No"].ToString(), sda);

                                        if (resultProductCheck.Success)
                                        {
                                            existProductId = resultProductCheck.CrmId;
                                        }

                                        Entity ent = new Entity("product");

                                        #region | PROJECT |

                                        if (dt.Rows[j]["Proje"] != DBNull.Value && dt.Rows[j]["Proje"] != string.Empty)
                                        {
                                            MsCrmResult resultProject = GeneralHelper.GetEntityIdByAttributeName("new_project", "new_name", dt.Rows[j]["Proje"].ToString(), sda);

                                            if (resultProject.Success)
                                            {
                                                ent["new_projectid"] = new EntityReference("new_project", resultProject.CrmId);
                                            }
                                            else
                                            {
                                                resultProject = GeneralHelper.CreateEntity("new_project", "new_name", dt.Rows[j]["Proje"].ToString(), service);
                                                ent["new_projectid"] = new EntityReference("new_project", resultProject.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | BLOCK |

                                        if (dt.Rows[j]["Blok"] != DBNull.Value && dt.Rows[j]["Blok"] != string.Empty)
                                        {
                                            MsCrmResult resultBlock = GeneralHelper.GetEntityIdByAttributeName("new_block", "new_name", dt.Rows[j]["Blok"].ToString(), sda);

                                            if (resultBlock.Success)
                                            {
                                                ent["new_blockid"] = new EntityReference("new_block", resultBlock.CrmId);
                                            }
                                            else
                                            {
                                                resultBlock = GeneralHelper.CreateEntity("new_block", "new_name", dt.Rows[j]["Blok"].ToString(), service);
                                                ent["new_blockid"] = new EntityReference("new_block", resultBlock.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | FLOOR NUMBER |

                                        if (dt.Rows[j]["Kat"] != DBNull.Value && dt.Rows[j]["Kat"] != string.Empty)
                                        {
                                            ent["new_floornumber"] = Convert.ToInt32(dt.Rows[j]["Kat"]);
                                        }

                                        #endregion

                                        #region | HOME NUMBER |

                                        if (dt.Rows[j]["Daire No"] != DBNull.Value && dt.Rows[j]["Daire No"] != string.Empty)
                                        {
                                            ent["new_homenumber"] = dt.Rows[j]["Daire No"].ToString();
                                        }

                                        #endregion

                                        #region | NAME && PRODUCT NUMBER |

                                        if (dt.Rows[j]["Daire Kimlik No"] != DBNull.Value && dt.Rows[j]["Daire Kimlik No"] != string.Empty)
                                        {
                                            ent["name"] = dt.Rows[j]["Daire Kimlik No"].ToString();
                                            ent["productnumber"] = dt.Rows[j]["Daire Kimlik No"].ToString();
                                        }

                                        #endregion

                                        #region | UNIT TYPE |

                                        if (dt.Rows[j]["Ünite Tipi"] != DBNull.Value && dt.Rows[j]["Ünite Tipi"] != string.Empty)
                                        {
                                            MsCrmResult resultUnitType = GeneralHelper.GetEntityIdByAttributeName("new_unittype", "new_name", dt.Rows[j]["Ünite Tipi"].ToString(), sda);

                                            if (resultUnitType.Success)
                                            {
                                                ent["new_unittypeid"] = new EntityReference("new_unittype", resultUnitType.CrmId);
                                            }
                                            else
                                            {
                                                resultUnitType = GeneralHelper.CreateEntity("new_unittype", "new_name", dt.Rows[j]["Ünite Tipi"].ToString(), service);
                                                ent["new_unittypeid"] = new EntityReference("new_unittype", resultUnitType.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | GENERAL TYPE OF HOME |

                                        if (dt.Rows[j]["Genel Daire Tipi"] != DBNull.Value && dt.Rows[j]["Genel Daire Tipi"] != string.Empty)
                                        {
                                            MsCrmResult resultGeneralTypeHome = GeneralHelper.GetEntityIdByAttributeName("new_generaltypeofhome", "new_name", dt.Rows[j]["Genel Daire Tipi"].ToString(), sda);

                                            if (resultGeneralTypeHome.Success)
                                            {
                                                ent["new_generaltypeofhomeid"] = new EntityReference("new_generaltypeofhome", resultGeneralTypeHome.CrmId);
                                            }
                                            else
                                            {
                                                resultGeneralTypeHome = GeneralHelper.CreateEntity("new_generaltypeofhome", "new_name", dt.Rows[j]["Genel Daire Tipi"].ToString(), service);
                                                ent["new_generaltypeofhomeid"] = new EntityReference("new_generaltypeofhome", resultGeneralTypeHome.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | TYPE OF HOME-???-OK |

                                        if (dt.Rows[j]["Daire Tipi"] != DBNull.Value && dt.Rows[j]["Daire Tipi"] != string.Empty && ent.Contains("new_generaltypeofhomeid") && ent["new_generaltypeofhomeid"] != null)
                                        {
                                            MsCrmResult resultTypeHome = ProductHelper.CheckTypeOfHomeExists(dt.Rows[j]["Daire Tipi"].ToString(), ((EntityReference)ent["new_generaltypeofhomeid"]).Id, sda);

                                            if (resultTypeHome.Success)
                                            {
                                                ent["new_typeofhomeid"] = new EntityReference("new_typeofhome", resultTypeHome.CrmId);
                                            }
                                            else
                                            {
                                                //resultTypeHome = GeneralHelper.CreateEntity("new_typeofhome", "new_name", dt.Rows[j]["Daire Tipi"].ToString(), service);
                                                resultTypeHome = GeneralHelper.CreateTypeOfHome(dt.Rows[j]["Daire Tipi"].ToString(), ((EntityReference)ent["new_generaltypeofhomeid"]).Id, service);

                                                ent["new_typeofhomeid"] = new EntityReference("new_typeofhome", resultTypeHome.CrmId);
                                            }
                                        }

                                        //if (dt.Rows[j]["Daire Tipi"] != DBNull.Value && dt.Rows[j]["Daire Tipi"] != string.Empty)
                                        //{
                                        //    MsCrmResult resultTypeHome = GeneralHelper.GetEntityIdByAttributeName("new_typeofhome", "new_name", dt.Rows[j]["Daire Tipi"].ToString(), sda);

                                        //    if (resultTypeHome.Success)
                                        //    {
                                        //        ent["new_typeofhomeid"] = new EntityReference("new_typeofhome", resultTypeHome.CrmId);
                                        //    }
                                        //    else
                                        //    {
                                        //        resultTypeHome = GeneralHelper.CreateEntity("new_typeofhome", "new_name", dt.Rows[j]["Daire Tipi"].ToString(), service);
                                        //        ent["new_typeofhomeid"] = new EntityReference("new_typeofhome", resultTypeHome.CrmId);
                                        //    }
                                        //}

                                        #endregion

                                        #region | AKS |

                                        if (dt.Rows[j]["Aks"] != DBNull.Value && dt.Rows[j]["Aks"] != string.Empty)
                                        {
                                            ent["new_aks"] = Convert.ToInt32(dt.Rows[j]["Aks"]);
                                        }

                                        #endregion

                                        #region | LOCATION |

                                        if (dt.Rows[j]["Konum"] != DBNull.Value && dt.Rows[j]["Konum"] != string.Empty)
                                        {
                                            MsCrmResult resultLocation = GeneralHelper.GetEntityIdByAttributeName("new_location", "new_name", dt.Rows[j]["Konum"].ToString(), sda);

                                            if (resultLocation.Success)
                                            {
                                                ent["new_locationid"] = new EntityReference("new_location", resultLocation.CrmId);
                                            }
                                            else
                                            {
                                                resultLocation = GeneralHelper.CreateEntity("new_location", "new_name", dt.Rows[j]["Konum"].ToString(), service);
                                                ent["new_locationid"] = new EntityReference("new_location", resultLocation.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | LICENCE NUMBER |

                                        if (dt.Rows[j]["Ruhsat No"] != DBNull.Value && dt.Rows[j]["Ruhsat No"] != string.Empty)
                                        {
                                            ent["new_licencenumber"] = dt.Rows[j]["Ruhsat No"].ToString();
                                        }

                                        #endregion

                                        #region | DESCRIPTION |

                                        if (dt.Rows[j]["Açıklama"] != DBNull.Value && dt.Rows[j]["Açıklama"] != string.Empty)
                                        {
                                            ent["description"] = dt.Rows[j]["Açıklama"].ToString();
                                        }

                                        #endregion


                                        #region | NET M2 |

                                        if (dt.Rows[j]["Net M2"] != DBNull.Value && dt.Rows[j]["Net M2"] != string.Empty)
                                        {
                                            ent["new_netm2"] = Convert.ToDecimal(dt.Rows[j]["Net M2"]);
                                        }

                                        #endregion

                                        #region | BALCONY M2 |

                                        if (dt.Rows[j]["Balkon M2"] != DBNull.Value && dt.Rows[j]["Balkon M2"] != string.Empty)
                                        {
                                            ent["new_balconym2"] = Convert.ToDecimal(dt.Rows[j]["Balkon M2"]);
                                        }

                                        #endregion

                                        #region | TERRACE M2 |

                                        if (dt.Rows[j]["Teras M2"] != DBNull.Value && dt.Rows[j]["Teras M2"] != string.Empty)
                                        {
                                            ent["new_terracegross"] = Convert.ToDecimal(dt.Rows[j]["Teras M2"]);
                                        }

                                        #endregion

                                        #region | WAREHOUSE M2 |

                                        if (dt.Rows[j]["Depo M2"] != DBNull.Value && dt.Rows[j]["Depo M2"] != string.Empty)
                                        {
                                            ent["new_warehousem2"] = Convert.ToDecimal(dt.Rows[j]["Depo M2"]);
                                        }

                                        #endregion

                                        #region | PRICE OPERATIONS |

                                        if (dt.Rows[j]["Liste Fiyatı"] != DBNull.Value && dt.Rows[j]["Liste Fiyatı"] != string.Empty
                                            && dt.Rows[j]["Para Birimi"] != DBNull.Value && dt.Rows[j]["Para Birimi"] != string.Empty)
                                        {
                                            MsCrmResult resultPriceList = ProductHelper.GetPriceListIdByCurrencySymbol(dt.Rows[j]["Para Birimi"].ToString(), sda);

                                            if (resultPriceList.Success)
                                            {
                                                ent["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid(resultPriceList.Result));
                                                ent["price"] = new Money(Convert.ToDecimal(dt.Rows[j]["Liste Fiyatı"].ToString()));
                                                ent["defaultuomid"] = new EntityReference("uom", Globals.DefaultUoMId);
                                                ent["defaultuomscheduleid"] = new EntityReference("uomschedule", Globals.DefaultUoMScheduleId);
                                                //ent["pricelevelid"] = new EntityReference("pricelevel", resultPriceList.CrmId);
                                                ent["quantitydecimal"] = 2;
                                            }
                                            else
                                            {
                                                errorCount++;
                                                continue;
                                            }
                                        }

                                        #endregion

                                        #region | GROSS M2 |

                                        if (dt.Rows[j]["Brüt M2"] != DBNull.Value && dt.Rows[j]["Brüt M2"] != string.Empty)
                                        {
                                            ent["new_grossm2"] = Convert.ToDecimal(dt.Rows[j]["Brüt M2"]);
                                        }


                                        #endregion

                                        #region | KDV RATIO |

                                        if (dt.Rows[j]["Kdv"] != DBNull.Value && dt.Rows[j]["Kdv"] != string.Empty)
                                        {
                                            ent["new_kdvratio"] = Convert.ToDecimal(dt.Rows[j]["Kdv"]);
                                        }

                                        #endregion

                                        #region | TAX OF STAMP RATIO |

                                        if (dt.Rows[j]["Damga Vergisi"] != DBNull.Value && dt.Rows[j]["Damga Vergisi"] != string.Empty)
                                        {
                                            ent["new_taxofstamp"] = Convert.ToDecimal(dt.Rows[j]["Damga Vergisi"]);
                                        }

                                        #endregion

                                        #region | SHARE |

                                        if (dt.Rows[j]["Paylaşım"] != DBNull.Value && dt.Rows[j]["Paylaşım"] != string.Empty)
                                        {
                                            MsCrmResult resultShare = GeneralHelper.GetEntityIdByAttributeName("new_share", "new_name", dt.Rows[j]["Paylaşım"].ToString(), sda);

                                            if (resultShare.Success)
                                            {
                                                ent["new_shareid"] = new EntityReference("new_share", resultShare.CrmId);
                                            }
                                            else
                                            {
                                                resultShare = GeneralHelper.CreateEntity("new_share", "new_name", dt.Rows[j]["Paylaşım"].ToString(), service);
                                                ent["new_shareid"] = new EntityReference("new_share", resultShare.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | CITY |

                                        if (dt.Rows[j]["İl"] != DBNull.Value && dt.Rows[j]["İl"] != string.Empty)
                                        {
                                            ent["new_city"] = dt.Rows[j]["İl"].ToString();
                                        }

                                        #endregion

                                        #region | DISTRICT |

                                        if (dt.Rows[j]["İlçe"] != DBNull.Value && dt.Rows[j]["İlçe"] != string.Empty)
                                        {
                                            ent["new_district"] = dt.Rows[j]["İlçe"].ToString();
                                        }

                                        #endregion

                                        #region | QUARTER |

                                        if (dt.Rows[j]["Mahalle"] != DBNull.Value && dt.Rows[j]["Mahalle"] != string.Empty)
                                        {
                                            ent["new_quarter"] = dt.Rows[j]["Mahalle"].ToString();
                                        }

                                        #endregion

                                        #region | THREADER (PAFTA) |

                                        if (dt.Rows[j]["Pafta"] != DBNull.Value && dt.Rows[j]["Pafta"] != string.Empty)
                                        {
                                            MsCrmResult resultThreader = GeneralHelper.GetEntityIdByAttributeName("new_threader", "new_name", dt.Rows[j]["Pafta"].ToString(), sda);

                                            if (resultThreader.Success)
                                            {
                                                ent["new_threaderid"] = new EntityReference("new_threader", resultThreader.CrmId);
                                            }
                                            else
                                            {
                                                resultThreader = GeneralHelper.CreateEntity("new_threader", "new_name", dt.Rows[j]["Pafta"].ToString(), service);
                                                ent["new_threaderid"] = new EntityReference("new_threader", resultThreader.CrmId);
                                            }
                                        }

                                        #endregion

                                        #region | BLOCK OF BUILDING (ADA)-???-OK |

                                        if (dt.Rows[j]["Ada"] != DBNull.Value && dt.Rows[j]["Ada"] != string.Empty)
                                        {
                                            MsCrmResult resultBlockOfBuilding = GeneralHelper.GetEntityIdByAttributeName("new_blockofbuilding", "new_name", dt.Rows[j]["Ada"].ToString(), sda);

                                            if (resultBlockOfBuilding.Success)
                                            {
                                                ent["new_blockofbuildingid"] = new EntityReference("new_blockofbuilding", resultBlockOfBuilding.CrmId);
                                            }
                                            else
                                            {
                                                resultBlockOfBuilding = GeneralHelper.CreateEntity("new_blockofbuilding", "new_name", dt.Rows[j]["Ada"].ToString(), service);
                                                ent["new_blockofbuildingid"] = new EntityReference("new_blockofbuilding", resultBlockOfBuilding.CrmId);
                                            }
                                        }

                                        //if (dt.Rows[j]["Ada"] != DBNull.Value && dt.Rows[j]["Ada"] != string.Empty)
                                        //{
                                        //    MsCrmResult resultBlockOfBuilding = ProductHelper.CheckBlockOfBuildingExists(dt.Rows[j]["Ada"].ToString(), ((EntityReference)ent["new_threaderid"]).Id, ((EntityReference)ent["new_projectid"]).Id, sda);

                                        //    if (resultBlockOfBuilding.Success)
                                        //    {
                                        //        ent["new_blockofbuildingid"] = new EntityReference("new_blockofbuilding", resultBlockOfBuilding.CrmId);
                                        //    }
                                        //    else
                                        //    {
                                        //        resultBlockOfBuilding = ProductHelper.CreateBlockOfBuilding(dt.Rows[j]["Ada"].ToString(), ((EntityReference)ent["new_threaderid"]).Id, ((EntityReference)ent["new_projectid"]).Id, service);
                                        //        ent["new_blockofbuildingid"] = new EntityReference("new_blockofbuilding", resultBlockOfBuilding.CrmId);
                                        //    }
                                        //}

                                        #endregion

                                        #region | PARCEL-???-OK |
                                        if (dt.Rows[j]["Parsel"] != DBNull.Value && dt.Rows[j]["Parsel"] != string.Empty)
                                        {
                                            MsCrmResult resultParcel = GeneralHelper.GetEntityIdByAttributeName("new_parcel", "new_name", dt.Rows[j]["Parsel"].ToString(), sda);

                                            if (resultParcel.Success)
                                            {
                                                ent["new_parcelid"] = new EntityReference("new_parcel", resultParcel.CrmId);
                                            }
                                            else
                                            {
                                                resultParcel = GeneralHelper.CreateEntity("new_parcel", "new_name", dt.Rows[j]["Parsel"].ToString(), service);
                                                ent["new_parcelid"] = new EntityReference("new_parcel", resultParcel.CrmId);
                                            }
                                        }

                                        //if (dt.Rows[j]["Parsel"] != DBNull.Value && dt.Rows[j]["Parsel"] != string.Empty)
                                        //{
                                        //    MsCrmResult resultParcel = ProductHelper.CheckParcelExists(dt.Rows[j]["Parsel"].ToString(), ((EntityReference)ent["new_blockofbuildingid"]).Id, ((EntityReference)ent["new_projectid"]).Id, sda);

                                        //    if (resultParcel.Success)
                                        //    {
                                        //        ent["new_parcelid"] = new EntityReference("new_parcel", resultParcel.CrmId);
                                        //    }
                                        //    else
                                        //    {
                                        //        resultParcel = ProductHelper.CreateParcel(dt.Rows[j]["Parsel"].ToString(), ((EntityReference)ent["new_blockofbuildingid"]).Id, ((EntityReference)ent["new_projectid"]).Id, service);
                                        //        ent["new_parcelid"] = new EntityReference("new_parcel", resultParcel.CrmId);
                                        //    }
                                        //}

                                        #endregion

                                        #region BB Genel Brüt Alan
                                        if (dt.Rows[j]["BB Genel Brüt Alan"] != DBNull.Value && dt.Rows[j]["BB Genel Brüt Alan"] != string.Empty)
                                        {
                                            ent["new_bbgeneralgrossarea"] = Convert.ToDecimal(dt.Rows[j]["BB Genel Brüt Alan"]);
                                        }
                                        #endregion

                                        #region Satışa Esas Alan
                                        if (dt.Rows[j]["Satışa Esas Alan"] != DBNull.Value && dt.Rows[j]["Satışa Esas Alan"] != string.Empty)
                                        {
                                            ent["new_satisaesasalan"] = Convert.ToDecimal(dt.Rows[j]["Satışa Esas Alan"]);
                                        }
                                        #endregion

                                        #region Bahçe M2
                                        if (dt.Rows[j]["Bahçe M2"] != DBNull.Value && dt.Rows[j]["Bahçe M2"] != string.Empty)
                                        {
                                            ent["new_garden"] = Convert.ToDecimal(dt.Rows[j]["Bahçe M2"]);
                                        }

                                        #endregion

                                        #region BB NET M2
                                        if (dt.Rows[j]["BB NET M2"] != DBNull.Value && dt.Rows[j]["BB NET M2"] != string.Empty)
                                        {
                                            ent["new_bbnetarea"] = Convert.ToDecimal(dt.Rows[j]["BB NET M2"]);
                                        }
                                        #endregion

                                        #region Yön
                                        if (dt.Rows[j]["Yön"] != DBNull.Value && dt.Rows[j]["Yön"] != string.Empty)
                                        {
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("BATI"))
                                            {
                                                ent["new_west"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000000);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("DOĞU"))
                                            {
                                                ent["new_east"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000001);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("GÜNEY"))
                                            {
                                                ent["new_south"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000002);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("GÜNEY BATI"))
                                            {
                                                ent["new_southwest"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000003);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("GÜNEY DOĞU"))
                                            {
                                                ent["new_southeast"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000004);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("KUZEY"))
                                            {
                                                ent["new_north"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000005);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("KUZEY BATI"))
                                            {
                                                ent["new_northwest"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000006);
                                            }
                                            if (Convert.ToString(dt.Rows[j]["Yön"]).ToUpper().Equals("KUZEY DOĞU"))
                                            {
                                                ent["new_northeast"] = true;
                                                ent["new_generalway"] = new OptionSetValue(100000007);
                                            }
                                        }
                                        #endregion

                                        #region Teslim Tarihi
                                        if (dt.Rows[j]["Teslim Tarihi"] != DBNull.Value && dt.Rows[j]["Teslim Tarihi"] != string.Empty)
                                        {
                                            string deliveryDateString = Convert.ToString(dt.Rows[j]["Teslim Tarihi"]);
                                            DateTime deliveryDate = new DateTime();
                                            DateTime.TryParse(deliveryDateString, out deliveryDate);
                                            ent["new_deliverydate"] = deliveryDate;
                                        }
                                        #endregion


                                        if (existProductId != Guid.Empty)
                                        {
                                            ent["productid"] = existProductId;
                                            service.Update(ent);
                                        }
                                        else
                                        {
                                            newProductId = service.Create(ent);
                                        }

                                        successCount++;
                                    }
                                    catch (Exception ex)
                                    {
                                        errorCount++;
                                    }

                                    sb.AppendLine("Merhaba");
                                    sb.AppendLine("Konut ekleme işlemi tamamlanmıştır.");
                                    #endregion
                                    sb.AppendLine("Hatalı:" + errorCount.ToString());
                                    Console.SetCursorPosition(0, 5);
                                    Console.WriteLine("Hata:" + errorCount.ToString());

                                    sb.AppendLine("Başarılı:" + successCount.ToString());
                                    Console.SetCursorPosition(0, 6);
                                    Console.WriteLine("Başarılı:" + successCount.ToString());

                                }
                            }
                        }

                        ProductHelper.UpdateHouseImportResultsAndClose(lstImports[i].HouseImportId, errorCount, successCount, totalCount, service);
                    }

                    #region Create Email

                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = new EntityReference("systemuser", Globals.AdministratorId);



                    //Entity toParty = new Entity("activityparty");
                    //toParty["addressused"] = "Aleksi.Komorosano@nef.com.tr";
                    Entity toParty1 = new Entity("activityparty");
                    toParty1["addressused"] = "erkan.ozvar@nef.com.tr";
                    Entity toParty2 = new Entity("activityparty");
                    toParty2["addressused"] = "erhan.serter@nef.com.tr";

                    Entity email = new Entity("email");
                    email["to"] = new Entity[] {  toParty1, toParty2 };

                    email["from"] = new Entity[] { fromParty };
                    email["subject"] = DateTime.Now.ToShortDateString() + "Konut import";
                    email["description"] = sb.ToString();
                    email["directioncode"] = true;
                    Guid id = service.Create(email);

                    #endregion

                    #region Send Email
                    var req = new SendEmailRequest
                    {
                        EmailId = id,
                        TrackingToken = string.Empty,
                        IssueSend = true
                    };

                    try
                    {
                        var res = (SendEmailResponse)service.Execute(req);
                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.Result = "Listeler işlendi...";
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
