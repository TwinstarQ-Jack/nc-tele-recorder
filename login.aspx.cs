using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

public partial class login : System.Web.UI.Page
{
    bool WeixinSignal = false;
    string userAgent = HttpContext.Current.Request.UserAgent;
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
            logins.Visible = false;
            showuserlabel.Text = "欢迎您，" + CheckUser[0];
            showuser.Visible = true;
        }
        if (Request.QueryString["logout"] != null)
        {
            Logout();
        }
        if (Request.QueryString["notadmin"] != null)
        {
            NotAdmin();
        }
        if (Request.QueryString["unlogin"] != null)
        {
            unlogined.Visible = true;
        }
        if (userAgent.ToLower().Contains("micromessenger"))
        {
            WeixinSignal = true;
        }
    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        UsingMysql ReadUserPwDMD5 = new UsingMysql();        
        string CardID = inputUserName.Value.ToString().Trim();
        string Password = inputPassword.Value.ToString().Trim();
        string logintime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (Password == "" || CardID == "")
        {            
            PswGroup.Attributes.Add("class", "form-group has-error has-feedback");
            UserGroup.Attributes.Add("class", "form-group has-error has-feedback");
            stringisnull.Visible = true;
        }
        else
        {
            //对输入密码进行MD5加密
            Encrypt md5 = new Encrypt();
            string md5psw = md5.EncryptMD5(Password);
            //获取数据库密码
            string[] result = ReadUserPwDMD5.UserString(CardID);
            string RealName = result[1], UID = result[2], rank = result[3];
            if (RealName == null || UID == null || rank == null)  //对比
            {
                mysqlex.Visible = true;
            }
            else
            {
                inputPassword.Attributes.Add("disabled", "disabled");
                if (md5psw == result[0])
                {
                    PswGroup.Attributes.Add("class", "form-group has-success has-feedback");
                    UserGroup.Attributes.Add("class", "form-group has-success has-feedback");
                    Cookies Write = new Cookies();
                    string[] paras = { "RealName", RealName, "uid", UID, "CardID", CardID, "rank", rank, "logintime", logintime };
                    //更新数据库
                    try
                    {
                        MySqlConnection Conn = ReadUserPwDMD5.ConnMode;
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        Conn.Open();
                        MySqlCommand cmd = Conn.CreateCommand();
                        cmd.CommandText = "update account set LastLoginTime = '" + logintime + "' where CardID = " + CardID;
                        int feedback = cmd.ExecuteNonQuery();
                        if (feedback == 0)
                        {
                            updbfail.Visible = true;
                        }
                    }
                    catch (Exception)
                    {
                        mysqlex.Visible = true;
                        throw;
                    }
                    if (Write.WriteToCookies(paras,WeixinSignal))
                    {
                        success.Visible = true;
                        //跳回主页
                        Response.AddHeader("REFRESH", "0;URL=../index.aspx");
                    }
                    else
                    {
                        cookiesfail.Visible = true;
                    }
                }
                else //验证失败
                {
                    inputPassword.Value = null;
                    PswGroup.Attributes.Add("class", "form-group has-error has-feedback");
                    UserGroup.Attributes.Add("class", "form-group has-error has-feedback");
                    loginfail.Visible = true;
                    inputPassword.Attributes.Remove("disabled");
                }
            }            
        }
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        inputUserName.Value = "";
        inputPassword.Value = "";
        PswGroup.Attributes.Add("class", "form-group");
        UserGroup.Attributes.Add("class", "form-group");
        mysqlex.Visible = false;
        cookiesfail.Visible = false;
        loginfail.Visible = false;
        stringisnull.Visible = false;
        updbfail.Visible = false;
    }
    protected void Logout()
    {
        Cookies DelCookies = new Cookies();
        if (DelCookies.Logout())
        {
            success.InnerText = "注销成功，3秒后返回首页。";
            success.Visible = true;
            FormGroup.Visible = false;
            //跳回主页
            Response.AddHeader("REFRESH", "3;URL=../index.aspx");
        }
        else
        {
            unlogined.Visible = true;
        }
    }
    protected void NotAdmin()
    {
        loginfail.InnerText = "您没有权限，请咨询老师解决。5秒后跳回首页。";
        loginfail.Visible = true;
        FormGroup.Visible = false;
        Response.AddHeader("REFRESH", "5;URL=../index.aspx");
        //跳回主页
    }
}