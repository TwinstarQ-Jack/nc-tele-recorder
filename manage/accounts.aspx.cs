using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.OleDb;


public partial class manage_accounts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //参数初始化
        divAlterDanger.Visible = false;
        divAlterSuccess.Visible = false;
        string uid = null, delete_sign = null, CardID = null ;
        int page = 1;//默认页码
        bool searchmode = false;//是否为搜索模式
        if (!IsPostBack)   //非回调
        {
            if (HttpContext.Current.Request.QueryString["page"] != null)
            {
                page = Convert.ToInt32(HttpContext.Current.Request.QueryString["page"]);
            }
            if (HttpContext.Current.Request.QueryString["uid"] != null)
            {
                uid = HttpContext.Current.Request.QueryString["uid"].ToString();
            }
            if (HttpContext.Current.Request.QueryString["delete"] != null)
            {
                delete_sign = HttpContext.Current.Request.QueryString["delete"].ToString();
            }
            if (HttpContext.Current.Request.QueryString["CardID"] != null)
            {
                CardID = HttpContext.Current.Request.QueryString["CardID"].ToString();
            }
            if (HttpContext.Current.Request.QueryString["add"] != null)
            {
                link_AddUser();
            }
            if (uid != null && uid != "" && delete_sign == null)
            {
                link_alterUserData(uid);
            }
            if (uid != null && delete_sign != null && delete_sign == "1")
            {
                link_delUser(uid);
            }
            if (HttpContext.Current.Request.QueryString["search"] == "1")
            {
                searchmode = true;
            }
            if (Revise.Visible == true && searchmode == false)
            {
                UserManageTable(tableShowUser, page);   //default，第一页
            }
            //if (Revise.Visible == true && searchmode == true)
            //{
            //    Search_UserManageTable(tableShowUser, page, CardID);
            //}

        }
    }
    protected DataSet getNowShowUserPage(int nowPage)
    {
        UsingMysql MysqlExe = new UsingMysql();
        MySqlConnection Conn = MysqlExe.ConnMode;  //MySQL连接串
        string connstring = "select UID,CardID,RealName from account order by CAST(UID as UNSIGNED) limit " + (nowPage - 1) * 10 + ",10";
        MySqlCommand cmd = new MySqlCommand(connstring, Conn);
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds, "account");
            Conn.Close();
            return ds;
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
            divAlterDanger.Visible = true;
            throw;
        }
    }
    protected DataSet getSearchResult(int nowPage, string uid)
    {
        UsingMysql MysqlExe = new UsingMysql();
        MySqlConnection Conn = MysqlExe.ConnMode;  //MySQL连接串
        string connstring = "select UID,CardID,RealName from account where CardID like '%" + uid + "%' order by CAST(UID as UNSIGNED) limit " + (nowPage - 1) * 10 + ",10 ";
        MySqlCommand cmd = new MySqlCommand(connstring, Conn);
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds, "account");
            Conn.Close();
            return ds;
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
            divAlterDanger.Visible = true;
            throw;
        }
    }

    protected void UserManage_PaginationShow(int NowPage, int SumRecords, string CardID)
    {
        string CardID_Para = "&search=1&CardID=" + CardID;
        if (CardID == null)
        {
            CardID_Para = null;
        }
        int dividedby = 10;   //默认分页条数
        int Pages_Num = SumRecords / dividedby + 1;    //总页数（最后一页导航）
        int list_Num = 5;   //当前最大显示分块
        int var_Page = 0;
        LiteralControl FirstPage = new LiteralControl(), LastPage = new LiteralControl();
        FirstPage.Text = "<li><a href=accounts.aspx?page=1" + CardID_Para + ">&laquo;第一页</a></li>";
        LastPage.Text = "<li><a href=accounts.aspx?page=" + Pages_Num + CardID_Para + ">最后一页&raquo;</a></li>";
        if (Pages_Num <= list_Num)
        {
            int i = 1; //Page-1
            if(NowPage == 1)
            {
                FirstPage.Text = "<li class=disabled><a>&laquo;第一页</a></li>";
            }
            PaginationPage.Controls.Add(FirstPage);  //添加到ul中
            //处理中间数字导航模块
            if (Pages_Num <= list_Num)
            //总页数<=最大分块
            {
                for (; i <= Pages_Num; i++)
                {
                    LiteralControl ListPage = new LiteralControl();
                    if (i == NowPage)
                    {
                        ListPage.Text= "<li class=active><a>" + NowPage + "</a></li>";
                    }
                    else
                    {
                        ListPage.Text = "<li><a href=accounts.aspx?page=" + i + CardID_Para + ">" + i + "</a></li>";
                    }
                    PaginationPage.Controls.Add(ListPage);
                }
            }
            else
            //适合于总页数>5
            {
                int index_offset = list_Num / 2;    //处于中间位置时的偏移量
                //处于过半之后位置的偏移量计算
                if (NowPage + index_offset > SumRecords) index_offset = -(NowPage - SumRecords - 1 + list_Num);
                for (int j_add = -index_offset; i < list_Num; i++, j_add++)
                {
                    LiteralControl ListPage = new LiteralControl();
                    var_Page = NowPage + j_add;
                    if (j_add == 0)
                    {
                        ListPage.Text = "<li class=active><a>" + NowPage + "</a></li>";
                    }
                    else
                    {
                        ListPage.Text = "<li><a href=accounts.aspx?page=" + var_Page + CardID_Para + ">" + var_Page + "</a></li>";
                    }
                    PaginationPage.Controls.Add(ListPage);
                }
            }
            if (NowPage == Pages_Num)
            {
                LastPage.Text = "<li class=disabled><a>最后一页&raquo;</a></li>";
            }
            PaginationPage.Controls.Add(LastPage);
        }
    }
    /// <summary>
    /// 展示用户表格
    /// </summary>
    protected void UserManageTable(Table ShowTable, int NowPage)
    {       
        //表格相关
        TableRow tRow;     //定义数据表行
        TableCell tCell;   //定义数据表列
        string[] Titles = { "一卡通卡号", "姓名", "修改", "删除" };
        int numCells = 4;   //总列数     
        string UID = "0";       //UID
        //数据库相关
        DataSet ds = getNowShowUserPage(NowPage);     //提取列：UID,CardID,RealName
        int numRows = ds.Tables[0].Rows.Count + 1;    //总行数，默认为11
        for (int iRow = 0; iRow < numRows; iRow++)    //行循环
        {
            tRow = new TableRow();
            if (iRow == 0)                          //表格首行
            {
                for (int jCol = 0; jCol < numCells; jCol++)
                {
                    tCell = new TableCell();
                    tCell.Text = Titles[jCol];
                    tRow.Cells.Add(tCell);
                }
            }
            else
            {
                DataRow dr = ds.Tables[0].Rows[iRow - 1];
                TableCell[] tCells = new TableCell[4];
                for (int i = 0; i < 4; i++) tCells[i] = new TableCell();
                UID = dr["UID"].ToString();
                tCells[0].Text = dr["CardID"].ToString();
                tRow.Cells.Add(tCells[0]);//第一列
                tCells[1].Text = dr["RealName"].ToString();
                tRow.Cells.Add(tCells[1]);//第二列
                tCells[2].Text = "<a href=accounts.aspx?uid=" + UID + ">";
                tCells[2].Text += "<span class='glyphicon glyphicon-cog'></span></a>";
                tRow.Cells.Add(tCells[2]);//第三列
                tCells[3].Text = "<a href=accounts.aspx?uid=" + UID + "&delete=1>";
                tCells[3].Text += "<span class='glyphicon glyphicon-trash'></span></a>";
                tRow.Cells.Add(tCells[3]);//第四列
            }
            ShowTable.Rows.Add(tRow);  //添加一行
        }
        //获取总数
        UsingMysql MySqlExe = new UsingMysql();
        MySqlConnection Conn = MySqlExe.ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        int SumRecords = 0;
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        cmd.CommandText = "select count(*) from account";
        try
        {
            Conn.Open();
            MySqlDataReader dbReader = cmd.ExecuteReader();
            if (dbReader.Read())
            {
                SumRecords = Convert.ToInt32(dbReader[0].ToString());
            }
            dbReader.Close();
            Conn.Close();
            //生成分页导航
            ReviseShowMemberNum.InnerText = SumRecords.ToString();
            UserManage_PaginationShow(NowPage, SumRecords, null);
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
            divAlterDanger.Visible = true;
            //throw;
        }
    }
    /// <summary>
    /// 搜索用户名时显示表格
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void Search_UserManageTable(Table ShowTable, int NowPage, string CardID)
    //{
    //    //表格相关
    //    TableRow tRow;     //定义数据表行
    //    TableCell tCell;   //定义数据表列
    //    string[] Titles = { "一卡通卡号", "姓名", "修改", "删除" };
    //    int numCells = 4;   //总列数   
    //    string UID = null;
    //    inputSearchByCardID.Value = CardID;
    //    //数据库相关
    //    DataSet ds = getSearchResult(NowPage, CardID);     //提取列：UID,CardID,RealName
    //    int numRows = ds.Tables[0].Rows.Count + 1;    //总行数，默认为11
    //    for (int iRow = 0; iRow < numRows; iRow++)    //行循环
    //    {
    //        tRow = new TableRow();
    //        if (iRow == 0)                          //表格首行
    //        {
    //            for (int jCol = 0; jCol < numCells; jCol++)
    //            {

    //                tCell = new TableCell();
    //                tCell.Text = Titles[jCol];
    //                tRow.Cells.Add(tCell);
    //            }
    //        }
    //        else
    //        {
    //            DataRow dr = ds.Tables[0].Rows[iRow - 1];
    //            TableCell[] tCells = new TableCell[4];
    //            for (int i = 0; i < 4; i++) tCells[i] = new TableCell();
    //            UID = dr["UID"].ToString();
    //            tCells[0].Text = dr["CardID"].ToString();
    //            tRow.Cells.Add(tCells[0]);//第一列
    //            tCells[1].Text = dr["RealName"].ToString();
    //            tRow.Cells.Add(tCells[1]);//第二列
    //            tCells[2].Text = "<a href=accounts.aspx?uid=" + UID + ">";
    //            tCells[2].Text += "<span class='glyphicon glyphicon-cog'></span></a>";
    //            tRow.Cells.Add(tCells[2]);//第三列
    //            tCells[3].Text = "<a href=accounts.aspx?uid=" + UID + "&delete=1>";
    //            tCells[3].Text += "<span class='glyphicon glyphicon-trash'></span></a>";
    //            tRow.Cells.Add(tCells[3]);//第四列
    //        }
    //        ShowTable.Rows.Add(tRow);  //添加一行
    //    }
    //    //获取总数
    //    UsingMysql MySqlExe = new UsingMysql();
    //    MySqlConnection Conn = MySqlExe.ConnMode;
    //    MySqlCommand cmd = Conn.CreateCommand();
    //    int SumRecords = 0;
    //    if (Conn.State == ConnectionState.Open)
    //    {
    //        Conn.Close();
    //    }
    //    cmd.CommandText = "select count(*) from account where CardID like '%" + CardID + "%'";
    //    try
    //    {
    //        Conn.Open();
    //        MySqlDataReader dbReader = cmd.ExecuteReader();
    //        if (dbReader.Read())
    //        {
    //            SumRecords = Convert.ToInt32(dbReader[0].ToString());
    //        }
    //        dbReader.Close();
    //        Conn.Close();
    //        //生成分页导航
    //        ReviseShowMemberNum.InnerText = SumRecords.ToString();
    //        UserManage_PaginationShow(NowPage, SumRecords, CardID);
    //    }
    //    catch (Exception ex)
    //    {
    //        labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
    //        divAlterDanger.Visible = true;
    //        //throw;
    //    }
    //}
    protected void btnRevise_Click(object sender, EventArgs e)
    {
        Response.Redirect("accounts.aspx");
    }

    protected void link_AddUser()
    {
        Cookies getUserState = new Cookies();
        string[] QueryCookies = { "RealName", "Rank" };
        Revise.Visible = false;
        ImportUserSheet.Visible = false;
        AddUser.Visible = true;
        inputRegisterTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string DealingUser = getUserState.ReadCookies(QueryCookies)[0];
        string Rank = getUserState.ReadCookies(QueryCookies)[1];
        inputDealingUser.Value = DealingUser;
        if (Rank == "Stud")
        {
            selectUserLevel.Attributes.Add("disabled", "disabled");
        }
    }
    protected void btnAddUser_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=accounts.aspx?add=1");
    }
    /// <summary>
    /// 重置按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        if(divbtnAlter.Visible == true)
        {
            Response.AddHeader("REFRESH", "0");
        }
        else
        {
            divAlterDanger.Visible = false;
            divAlterSuccess.Visible = false;
            inputCardID.Value = null;
            inputRealName.Value = null;
            inputPasswordRepeat.Value = null;
            inputPasswordSet.Value = null;
            inputTelephone.Value = null;
            divAddPasswordGroup.Attributes.Add("class", "form-group");
            divAddSystemAutoMake.Attributes.Add("class", "form-group");
            divAddTelephoneGroup.Attributes.Add("class", "form-group");
            divAddUserGroup.Attributes.Add("class", "form-group");
        }
    }
    /// <summary>
    /// 检查表单是否有空白项目
    /// </summary>
    /// <returns></returns>
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
            divAddUserGroup.Attributes.Add("class", "form-group has-error has-feedback");
            status = false;
        }
        if (Create_Signal)
        {
            if (Password != PasswordRepeat || Password == "" || PasswordRepeat == "")                      //密码判断
            {
                labelAlterDanger.Text += "密码有误或者为空,";
                divAddPasswordGroup.Attributes.Add("class", "form-group has-error has-feedback");
                status = false;
            }
        }
        else
        {
            if (Password != PasswordRepeat)                      //密码判断
            {
                labelAlterDanger.Text += "密码有误,";
                divAddPasswordGroup.Attributes.Add("class", "form-group has-error has-feedback");
                status = false;
            }
        }
        if (Telephone == "")
        {
            labelAlterDanger.Text += "手机号码为空，";
            divAddTelephoneGroup.Attributes.Add("class", "form-group has-error has-feedback");
            status = false;
        }
        if (!status)
        {
            labelAlterDanger.Text += "请检查您的输入！";
            divAlterDanger.Visible = true;
        }
        return status;
    }
    /// <summary>
    /// 点击注册按钮生成用户
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnRegister_Click(object sender, EventArgs e)
    {
        //初始化获取数据
        string QUID = null, UID = null, RealName = inputRealName.Value, CardID = inputCardID.Value;
        string Password = inputPasswordSet.Value, PasswordRepeat = inputPasswordRepeat.Value;
        string Telephone = inputTelephone.Value, Telephone2 = inputTelephone2.Value;
        string RegisterTime = inputRegisterTime.Value ,DealingUser=inputDealingUser.Value;
        string Rank = selectUserLevel.Value;
        if (DealingUser == "") DealingUser = "RegPage";   //初始化注册处理用户
        if (IsAddUserData_NOTnull(true))
        {
            Encrypt PswMD5Encrypt = new Encrypt();
            Password = PswMD5Encrypt.EncryptMD5(Password);
            UsingMysql InsertAccount = new UsingMysql();
            MySqlConnection Conn = InsertAccount.ConnMode;
            try
            {
                MySqlCommand cmd = Conn.CreateCommand();
                cmd.CommandText = "select UID from account order by CAST(UID as UNSIGNED) desc limit 0,1";
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                Conn.Open();
                MySqlDataReader dbreader = cmd.ExecuteReader();
                if (dbreader.Read())
                {
                    QUID = dbreader["UID"].ToString();
                }
                else
                {
                    QUID = "0";
                }
                dbreader.Close();
                UID = (Convert.ToInt64(QUID) + 1).ToString();
                cmd.CommandText = "insert into account values('" + UID + "','" + RealName;
                cmd.CommandText += "','" + Password + "','" + CardID + "','" + Telephone;
                cmd.CommandText += "','" + RegisterTime + "','" + RegisterTime;
                cmd.CommandText += "','" + Rank + "','" + DealingUser + "','" + Telephone2 + "')";
                int feedback = cmd.ExecuteNonQuery();
                Conn.Close();
                if (feedback == 0)
                {
                    labelAlterDanger.Text = "用户增加失败！";
                    divAlterDanger.Visible = true;
                }
                else
                {
                    labelAlterSuccess.Text = "操作成功，增加了" + feedback + "个用户。";
                    divAlterSuccess.Visible = true;
                    cmd.CommandText = "insert into totaldatabase (Name,Belongs) values(@Name,'account')";
                    cmd.Parameters.Add(new MySqlParameter("@Name", RealName));
                    Conn.Open();
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
                divAlterDanger.Visible = true;
                //throw;
            }
        }
    }
    /// <summary>
    /// 通过传递UID修改用户资料
    /// </summary>
    /// <param name="uid"></param>
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
        string DB_RegUser = dr["RegUser"].ToString(),DB_RegTime = dr["RegisterTime"].ToString();
        string DB_Telephone2 = dr["Telephone2"].ToString();
        //页面展示
        Revise.Visible = false;
        AddUser.Visible = true;
        divAddUserHeader.InnerText = "修改用户 " + DB_RealName + " 信息";
        //填写表单进行展示
        inputCardID.Value = DB_CardID;
        inputRealName.Value = DB_RealName;
        inputTelephone.Value = DB_Telephone;
        selectUserLevel.Value = DB_Rank;
        inputDealingUser.Value = DB_RegUser;
        inputRegisterTime.Value = DB_RegTime;
        inputTelephone2.Value = DB_Telephone2;
        //更换显示操作按钮
        divbtnRegister.Visible = false;
        divbtnAlter.Visible = true;
    }
    /// <summary>
    /// 通过传递UID、del参数删除选定用户
    /// </summary>
    /// <param name="uid"></param>
    protected void link_delUser(string uid)
    {
        Revise.Visible = false;
        DelUser.Visible = true;
        UsingMysql MysqlExe = new UsingMysql();
        MySqlConnection Conn = MysqlExe.ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        string[] QueryUser = { "RealName,CardID", "account", "UID", uid };
        string RealName = null, CardID = null;
        try
        {
            MySqlDataReader dbReader = MysqlExe.getDataReader(Conn, QueryUser);
            if (dbReader.Read())
            {
                RealName = dbReader["RealName"].ToString();
                CardID = dbReader["CardID"].ToString();
            }
            dbReader.Close();
            Conn.Close();
            DelUserRealName.Text = RealName;
            DelUserCardID.Text = CardID;
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
            divAlterDanger.Visible = true;
            //throw;
        }
    }
    /// <summary>
    /// 按卡号搜索用户
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void btnSearchByCardID_Click(object sender, EventArgs e)
    //{
    //    string searchCardID = inputSearchByCardID.Value;
    //    Response.AddHeader("REFRESH", "0;url=accounts.aspx?CardID=" + searchCardID + "&search=1");
    //}
    /// <summary>
    /// 实现用户信息修改
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
            if (Password == "1B2M2Y8AsgTpgAmY7PhCfg==" && DB_RealName == RealName && DB_CardID == CardID && DB_Telephone == Telephone && DB_Rank == Rank && DB_Telephone2 == Telephone2) 
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
                connstring += "' where CardID='" + DB_CardID +"'";
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
    /// <summary>
    /// 用户点击确认删除按钮，删除本条记录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelUserConfirm_Click(object sender, EventArgs e)
    {
        string uid = null;
        if (HttpContext.Current.Request.QueryString["uid"] != null)
        {
            uid = HttpContext.Current.Request.QueryString["uid"].ToString();
        }
        UsingMysql MysqlExe = new UsingMysql();
        MySqlConnection Conn = MysqlExe.ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = "delete from account where UID=@UID";
        cmd.Parameters.Add(new MySqlParameter("@UID", uid));
        if(Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            int Feedback = cmd.ExecuteNonQuery();
            if (Feedback > 0)
            {
                labelAlterSuccess.Text = "操作成功，删除了" + Feedback + "个用户。将在5秒后返回列表。";
                divAlterSuccess.Visible = true;
                Response.AddHeader("REFRESH", "5;URL=accounts.aspx");
            }
            else
            {
                labelAlterDanger.Text = "用户信息删除失败！";
                divAlterDanger.Visible = true;
            }
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text += "数据库连接失败，请联系管理员。" + ex.Message;
            divAlterDanger.Visible = true;
            //throw;
        }
    }
    protected void btnDelUserReturn_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=accounts.aspx?");
    }

    //批量增加用户
    protected void btnBatchAdd_Click(object sender, EventArgs e)
    {
        AddUser.Visible = false;
        ImportUserSheet.Visible = true;
        Revise.Visible = false;
    }
    /// <summary>
    /// 上传模块
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void XlsFileUploadSubmit_Click(object sender, EventArgs e)
    {
        Cookies ReadCookies = new Cookies();
        string[] QueryArgu = new string[] { "CardID", "RealName" };
        string[] CheckUser = ReadCookies.ReadCookies(QueryArgu);
        string SaveFilePath = null;
        string Path = null, RealName = CheckUser[1];
        if (XlsFileUpload.HasFile)
        {            
            string FileName = XlsFileUpload.FileName.Substring(0, XlsFileUpload.FileName.LastIndexOf("."));
            string ExtenName = XlsFileUpload.FileName.Substring(XlsFileUpload.FileName.LastIndexOf(".")).ToLower();
            if (ExtenName.Equals(".xls"))
            {
                SaveFilePath = System.IO.Path.Combine(HttpContext.Current.Request.MapPath("../attachment/excel/" + DateTime.Now.ToString("yyyyMM")), CheckUser[0] + DateTime.Now.ToString("ddHHmmss") + ExtenName);
                float FileSize = (float)System.Math.Round((float)XlsFileUpload.PostedFile.ContentLength / 1024000, 1); //获取文件大小并保留小数点后一位,单位是M
                if(FileSize > 2.0)       //判断文件大小
                {
                    labelAlterDanger.Text = "您的文件超出了限制文件大小（2M），请拆分后再试。";
                    divAlterDanger.Visible = true;
                }
                else //后缀名正确且大小无误
                {
                    try
                    {
                        Path = Server.MapPath("~/") + "attachment/excel/" + DateTime.Now.ToString("yyyyMM");
                        if (!System.IO.Directory.Exists(Path))
                        {
                            System.IO.DirectoryInfo DirInfo = new System.IO.DirectoryInfo(Path);
                            DirInfo.Create();
                        }
                        XlsFileUpload.PostedFile.SaveAs(SaveFilePath);
                        labelAlterSuccess.Text = "文件已经上传成功，请稍等，后台正在处理数据库。";
                        divAlterSuccess.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        labelAlterDanger.Text = "请联系管理员，出错信息为" + ex.Message;
                        divAlterDanger.Visible = true;
                    }
                    //执行数据库导入方法
                    XlsWriteToDataBase(SaveFilePath,RealName);
                }
            }
            else  //后缀名非xls
            {
                labelAlterDanger.Text = "您上传的文件后缀名为<code>" + ExtenName + "</code>，请检查文件类型。";
                divAlterDanger.Visible = true;
            }
        }
        else       //没有上传文件
        {
            labelAlterDanger.Text = "您还没有选择上传文件，请在下方选择。";
            divAlterDanger.Visible = true;
        }
    }
    /// <summary>
    /// XLS导入数据库
    /// </summary>
    /// <param name="filePath">xls文件地址</param>
    /// <param name="RealName">当前处理人</param>
    /// <returns>状态码0(success),100(ExcelError),101(MySQLError),102(Ex)</returns>
    protected int XlsWriteToDataBase(string filePath,string DealName)
    {
        Encrypt PswEncrypt = new Encrypt();
        string RealName =null, UID = null, Password = null, CardID = null, Telephone = null,  Rank = null, Telephone2 = null;
        string RegisterTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        try
        {
            int success = 0, fail = 0, CardIDConflict = 0;     //计数器
            string failpeople = null, conflictpeople = null;
            //Excel表操作(ACE.OleDb.12.0版本可防止数字变为科学计数法）
            string ExcelConnStr = "Provider=Microsoft.ACE.OleDb.12.0;" + "Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
            OleDbConnection ExcelConn = new OleDbConnection(ExcelConnStr);
            ExcelConn.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter ODda = new OleDbDataAdapter("select * from [Sheet1$]", ExcelConn);
            ODda.Fill(ds);
            ExcelConn.Close();     //关闭OleDb连接
            //数据库操作
            DataTable dt = ds.Tables[0];
            int row_Num = dt.Rows.Count;              //行数
            for (int i = 0; i < row_Num; i++)
            {
                DataRow dr = dt.Rows[i];
                Password = dr["密码"].ToString();
                CardID = Convert.ToInt64(dr["一卡通卡号"].ToString()).ToString();
                Telephone = Convert.ToInt64(dr["手机号码1"].ToString()).ToString();
                if (dr["手机号码2"].ToString() != "") Telephone2 = Convert.ToInt64(dr["手机号码2"].ToString()).ToString();
                Rank = dr["用户等级"].ToString();
                RealName = dr["姓名"].ToString();
                UsingMysql MysqlExe = new UsingMysql();
                MySqlConnection MysqlConn = MysqlExe.ConnMode;
                MySqlCommand MysqlCmd = MysqlConn.CreateCommand();
                //获取当前UID序号
                MysqlCmd.CommandText = "select UID from account order by CAST(UID as UNSIGNED) desc limit 0,1";
                if (MysqlConn.State == ConnectionState.Open)
                {
                    MysqlConn.Close();
                }
                MysqlConn.Open();
                MySqlDataReader dreader = MysqlCmd.ExecuteReader();
                if (dreader.Read())
                {
                    UID = (Convert.ToInt64(dreader["UID"].ToString()) + 1).ToString();
                }
                dreader.Close();     //关闭reader，释放资源
                //判断一卡通号是否相同
                MysqlCmd.CommandText = "select count(*) from account where CardID = @CardID";
                MysqlCmd.Parameters.Add(new MySqlParameter("@CardID", CardID));
                dreader = MysqlCmd.ExecuteReader();
                int Existed = 0;   //是否存在该用户
                if (dreader.Read())
                {
                    Existed = Convert.ToInt32(dreader[0].ToString());
                }
                dreader.Close();
                MysqlConn.Close();
                if (Existed > 0)
                {
                    if (CardIDConflict > 0) conflictpeople += ", ";
                    CardIDConflict++;
                    conflictpeople += CardID;
                }
                else
                {
                    Password = PswEncrypt.EncryptMD5(Password);
                    MysqlCmd.CommandText = "insert into account (UID,RealName,Password,CardID,Telephone,RegisterTime,Rank,Telephone2,RegUser)";
                    MysqlCmd.CommandText += "values('" + UID + "','" + RealName + "','" + Password + "','" + CardID + "','" + Telephone + "','" + RegisterTime + "','" + Rank + "','" + Telephone2 + "','" + DealName + "')";
                    MysqlConn.Open();
                    int Feedback = MysqlCmd.ExecuteNonQuery();
                    MysqlConn.Close();
                    if (Feedback > 0)
                    {
                        success++;
                        MysqlCmd.CommandText = "insert into totaldatabase (Name,Belongs) values(@Name,'account')";
                        MysqlCmd.Parameters.Add(new MySqlParameter("@Name", RealName));
                        MysqlConn.Open();
                        MysqlCmd.ExecuteNonQuery();
                        MysqlConn.Close();
                    }
                    else
                    {
                        if (fail > 0) failpeople += ", ";
                        fail++;
                        failpeople += CardID;
                    }
                }
                labelAlterSuccess.Text = "共成功导入了" + success + "个人,失败导入" + (CardIDConflict + fail).ToString() + "人。";
                if (fail > 0) labelAlterSuccess.Text += "其中" + failpeople + "无法导入。";
                if (CardIDConflict > 0) labelAlterSuccess.Text += conflictpeople + "已经存在。";
                divAlterSuccess.Visible = true;
            }
            return 0;         //成功返回
        }
        catch (OleDbException OleEx)
        {
            labelAlterDanger.Text = "请联系管理员，出错信息为" + OleEx.Message;
            divAlterDanger.Visible = true;
            return 100;      //打开excel表出错
        }
        catch (MySqlException MysqlEx)
        {
            labelAlterDanger.Text = "请联系管理员，出错信息为" + MysqlEx.Message;
            divAlterDanger.Visible = true;
            return 101;     //访问数据库出错
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text = "请联系管理员，出错信息为" + ex.Message;
            divAlterDanger.Visible = true;
            return 102;     //其他异常
        }
    }
}