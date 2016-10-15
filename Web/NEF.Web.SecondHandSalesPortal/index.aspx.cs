using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NEF.Library.Business;
using NEF.Library.Utility;


namespace NEF.Web.SecondHandSalesPortal
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
                    //string userName = @"NEF\demet.durgut"; //SATIŞ
                    //string userName = @"NEF\KWBornova"; //DIŞ BAYİ
                    //string userName = @"NEF\indibox";
                    //string userName = @"NEF\erkan.ozvar";
                    //SystemUser sInfo = SystemUserHelper.GetSystemUserByDomainName(userName, sda);
                    SystemUser sInfo = SystemUserHelper.GetSystemUserByDomainName(Environment.UserName, sda);

                    if (sInfo != null && sInfo.SystemUserId != Guid.Empty)
                    {
                        if (((int)sInfo.UserType) == ((int)UserTypes.IkinciElSatisDanismani) || ((int)sInfo.UserType) == ((int)UserTypes.IkinciElSatisDirektoru) || ((int)sInfo.UserType) == ((int)UserTypes.IkinciElSatisYoneticisi))
                        {
                            lblUserId.Value = sInfo.SystemUserId.ToString();
                            lblUserType.Value = ((int)sInfo.UserType).ToString();
                        }
                        else
                        {
                            bdy.Style.Add("background-color", "white");
                            bdy.InnerHtml = "<center><br /> <img src='images/warning.png' height='144' /> <br /><h1>Kullanıcı bilgileriniz CRM'de tanımlı değildir.<br> BT ekibi ile irtibata geçebilirsiniz.<h1></center>";
                            return;
                        }
                    }
                    else
                    {
                        bdy.Style.Add("background-color", "white");
                        bdy.InnerHtml = "<center><br /> <img src='images/warning.png' height='144' /> <br /><h1>Kullanıcı bilgileriniz CRM'de tanımlı değildir.<br> BT ekibi ile irtibata geçebilirsiniz.<h1></center>";
                        return;
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