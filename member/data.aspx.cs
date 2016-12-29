using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class member_data : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Cookies LocalUser = new Cookies();
        string[] QueryCookies = { "uid" };
        string uid = LocalUser.ReadCookies(QueryCookies)[0];
        link_alterUserData(uid);
    }
    protected void link_alterUserData(string uid)
    {
        //初始化数据库基本实体
        UsingMysql MysqlDB = new UsingMysql();
        MySqlConnection Conn = MysqlDB.ConnMode;
        string[] QueryParas = { "*", "account", "UID", uid };
        MySqlCommand cmd = Conn.CreateCommand();
        //调用数据库查询（DataSet）
        DataSet ds = MysqlDB.getDataSet(Conn, QueryParas);
        DataRow dr = ds.Tables[0].Rows[0];
        //获取数据库的表单参数
        string DB_RealName = dr["RealName"].ToString(), DB_CardID = dr["CardID"].ToString();
        string DB_Telephone = dr["Telephone"].ToString(), DB_Rank = dr["Rank"].ToString();
        string DB_RegUser = dr["RegUser"].ToString(), DB_RegTime = dr["RegisterTime"].ToString();
        string DB_Telephone2 = dr["Telephone2"].ToString();
        //填写表单进行展示
        inputCardID.Value = DB_CardID;
        inputRealName.Value = DB_RealName;
        inputTelephone.Value = DB_Telephone;
        selectUserLevel.Value = DB_Rank;
        inputDealingUser.Value = DB_RegUser;
        inputRegisterTime.Value = DB_RegTime;
        inputTelephone2.Value = DB_Telephone2;
    }
    public bool IsAddUserData_NOTnull(bool Create_Signal)
    {
        string RealName = inputRealName.Value, CardID = inputCardID.Value;
        string Password = inputPasswordSet.Value, PasswordRepeat = inputPasswordRepeat.Value;
        string Telephone = inputTelephone.Value;
        string RegisterTime = inputRegisterTime.Value, DealingUser = inputDealingUser.Value;
        bool status = true;
        labelAlterDanger.Text = "您输入的";
        if (CardID == "" || RealName == "")
        {
            labelAlterDanger.Text += "一卡通卡号或者真实姓名为空，";
            divChangeUserDataGroup.Attributes.Add("class", "form-group has-error has-feedback");
            status = false;
        }
        if (Create_Signal)
        {
            if (Password != PasswordRepeat || Password == "" || PasswordRepeat == "")                      //密码判断
            {
                labelAlterDanger.Text += "密码有误或者为空,";
                divChangeUserDataPasswordGroup.Attributes.Add("class", "form-group has-error has-feedback");
                status = false;
            }
        }
        else
        {
            if (Password != PasswordRepeat)                      //密码判断
            {
                labelAlterDanger.Text += "密码有误,";
                divChangeUserDataPasswordGroup.Attributes.Add("class", "form-group has-error has-feedback");
                status = false;
            }
        }
        if (Telephone == "")
        {
            labelAlterDanger.Text += "手机号码为空，";
            divChangeUserDataTelephoneGroup.Attributes.Add("class", "form-group has-error has-feedback");
            status = false;
        }
        if (!status)
        {
            labelAlterDanger.Text += "请检查您的输入！";
            divAlterDanger.Visible = true;
        }
        return status;
    }

    protected void btnAlter_Click(object sender, EventArgs e)
    {
        //获取当前用户Cookies
        Cookies LocalUser = new Cookies();
        string[] QueryCookies = { "RealName", "Rank" };
        string DealingUser = LocalUser.ReadCookies(QueryCookies)[0];
        //获取当前填写的表单参数
        string RealName = inputRealName.Value, CardID = inputCardID.Value;
        string Password = inputPasswordSet.Value, PasswordRepeat = inputPasswordRepeat.Value;
        string Telephone = inputTelephone.Value;
        string RegisterTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string Rank = selectUserLevel.Items[selectUserLevel.SelectedIndex].Value;
        string Telephone2 = inputTelephone2.Value;
        if (DealingUser == "") DealingUser = "RegPage";   //初始化注册处理用户
        //初始化数据库基本实体
        UsingMysql MysqlDB = new UsingMysql();
        MySqlConnection Conn = MysqlDB.ConnMode;
        string[] QueryParas = { "*", "account", "CardID", CardID };
        MySqlCommand cmd = Conn.CreateCommand();
        //调用数据库查询（DataSet）
        DataSet ds = MysqlDB.getDataSet(Conn, QueryParas);
        DataRow dr = ds.Tables[0].Rows[0];
        //获取数据库的表单参数
        string DB_RealName = dr["RealName"].ToString(), DB_CardID = dr["CardID"].ToString();
        string DB_Telephone = dr["Telephone"].ToString(), DB_Psw = dr["Password"].ToString();
        string DB_Rank = dr["Rank"].ToString(), DB_Telephone2 = dr["Telephone2"].ToString();
        //判断非空
        if (IsAddUserData_NOTnull(false))
        {
            //密码加密
            Encrypt PswMD5Encrypt = new Encrypt();
            Password = PswMD5Encrypt.EncryptMD5(Password);
            divAlterDanger.Visible = false;
            //数据比较
            if (DB_RealName == RealName && DB_CardID == CardID && DB_Telephone == Telephone && DB_Rank == Rank && DB_Telephone2 == Telephone2)
            {
                //弹出提示框，再次确认信息
                labelAlterDanger.Text = "您填写的信息与原来的相同，请检查您的填写。";
                divAlterDanger.Visible = true;
            }
            else         //提交更改
            {
                string pswString = "',Password='" + Password;
                if (Password == "1B2M2Y8AsgTpgAmY7PhCfg==") //判断密码未输入，为空，不提交到数据库里修改。
                {
                    pswString = null;
                }
                string connstring = "update account set RealName='" + RealName + pswString;
                connstring += "',CardID='" + CardID + "',Telephone='" + Telephone + "',Rank='" + Rank + "',Telephone2='" + Telephone2;
                connstring += "' where CardID='" + DB_CardID + "'";
                cmd.CommandText = connstring;
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                try
                {
                    Conn.Open();
                    int feedback = cmd.ExecuteNonQuery();
                    if (feedback > 0)   //修改成功
                    {
                        labelAlterSuccess.Text = "操作成功，修改了" + feedback + "个用户。";
                        divAlterSuccess.Visible = true;
                    }
                    else   //没有任何修改
                    {
                        labelAlterDanger.Text = "用户信息修改失败！";
                        divAlterDanger.Visible = true;
                    }

                }
                catch (Exception ex)   //数据库异常
                {
                    labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
                    divAlterDanger.Visible = true;
                    //throw;
                }
            }
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "URL;data.aspx");
    }
}