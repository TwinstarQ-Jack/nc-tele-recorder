using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Style : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Cookies ReadCookies = new Cookies();
        string[] QueryArgu = new string[] { "RealName", "uid", "rank" };
        string[] CheckUser = ReadCookies.ReadCookies(QueryArgu);
        if (CheckUser[2] == "TeacherAdmin" || CheckUser[2] == "StudAdmin")
        {
            admin.Visible = true;
        }
        if (CheckUser[0] != null)
        {
            login.Visible = false;
            showuserlabel.Text = "欢迎您，" + CheckUser[0];
            showuser.Visible = true;
        }
        else
        {
            Response.AddHeader("REFRESH", "0;URL=login.aspx?unlogin=1");
        }
    }
}
