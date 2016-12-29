using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ManageStyle : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Cookies ReadCookies = new Cookies();
        string[] QueryArgu = new string[] { "RealName", "uid", "rank" };
        string[] CheckUser = ReadCookies.ReadCookies(QueryArgu);
        if (CheckUser[2] == "TeacherAdmin" || CheckUser[2] == "StudAdmin")
        {           
        }
        else
        {
            HttpContext.Current.Response.Redirect("../login.aspx?notadmin=1");
        }
    }
}
