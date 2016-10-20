﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.Web.SalesPortal
{
    public partial class index : System.Web.UI.Page
    {
        SqlDataAccess sda;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblUserId.Value = Environment.UserName;

                if (!string.IsNullOrEmpty(Environment.UserName))
                {
                    sda = new SqlDataAccess();
                    sda.openConnection(Globals.ConnectionString);
                    //string userName = @"NEF\zeynep.bulbul"; // RESEPSIYONIST
                    string userName = @"NEF\demet.durgut"; //SATIŞ
                    //string userName = @"NEF\KWBornova"; //DIŞ BAYİ
                    //string userName = @"NEF\indibox";
                    SystemUser sInfo = SystemUserHelper.GetSystemUserByDomainName(userName, sda);
                    //SystemUser sInfo = SystemUserHelper.GetSystemUserByDomainName(Environment.UserName, sda);

                    if (sInfo != null && sInfo.SystemUserId != Guid.Empty)
                    {
                        lblUserId.Value = sInfo.SystemUserId.ToString();
                        lblUserType.Value = ((int)sInfo.UserType).ToString();
                    }
                    else
                    {
                        bdy.Style.Add("background-color", "white");
                        bdy.InnerHtml = "<center><br /> <img src='images/warning.png' height='144' /> <br /><h1>Kullanıcı bilgileriniz CRM'de tanımlı değildir.<br> BT ekibi ile irtibata geçebilirsiniz.<h1></center>";
                        return;


                        //lblUserId.Value = "12979C4A-368C-E411-80C1-005056A62B8A";
                        //lblUserId.Value = "8B126813-8A8B-E411-80C1-005056A62B8A"; //Nef Test
                        //lblUserId.Value = "246d808b-8774-e411-80e4-005056b0203a"; //ınnthebox test
                        // lblUserId.Value = "8A5D461F-A6A7-E411-80C5-005056A62B8A"; //ınnthebox test Pilot
                        //lblUserId.Value = "19C13874-4FAB-E311-BC40-00155D010409"; // pilot iş gyo

                        //lblUserId.Value = "5A49C200-5A97-E411-80C0-005056A60603"; //ınnthebox test Live
                        //lblUserType.Value = ((int)UserTypes.SatisDanismani).ToString();
                        //lblUserId.Value = "359BA910-ACC0-E311-B9D9-00155D011C0A";
                        //lblUserType.Value = ((int)UserTypes.CallCenter).ToString();


                        //is GYO Satis Test User
                        //lblUserId.Value = "0F3DA178-CDEC-E411-80D0-005056A60603";
                        //lblUserType.Value = ((int)UserTypes.IsGyoCallCenter).ToString();


                        //lblUserId.Value = "B0E2D6C9-7D7B-E311-9E00-00155D5A1714"; //Paris
                        //lblUserType.Value = ((int)UserTypes.MusteriIliskileri).ToString();
                        //lblUserType.Value = ((int)UserTypes.CallCenter).ToString();
                        //lblUserType.Value = ((int)UserTypes.Resepsiyonist).ToString();
                        //lblUserType.Value = ((int)UserTypes.CallCenter).ToString();
                        // return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sda.closeConnection();
            }
        }
    }
}