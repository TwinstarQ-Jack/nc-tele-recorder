using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MySql.Data;
using MySql.Web;
using MySql.Data.MySqlClient;

public partial class order : System.Web.UI.Page
{
    static UsingMysql MysqlExe = new UsingMysql();
    static MySqlConnection Conn = MysqlExe.ConnMode;
    protected void Page_Load(object sender, EventArgs e)
    {
        int orderid = 0; //初始化获取参数    
        if (HttpContext.Current.Request["page"] != null)
        {
            int NowPage = Convert.ToInt32(HttpContext.Current.Request["page"].ToString());
            InitMode_AddSubOrderShow(tableAddSubOrder, NowPage);
        }
        else
        {
            if (!IsPostBack)
            {

                //动态绑定四组DropDownList-SQL数据源
                DDLBind_BlockID();
                DDLBind_Appointer();
                DDLBind_QType1();
                //是否为追加订单参数
                if (HttpContext.Current.Request["orderid"] != null)
                {
                    orderid = Convert.ToInt32(HttpContext.Current.Request["orderid"].ToString());
                }
                if (HttpContext.Current.Request["addsub"] != null)
                {
                    if (HttpContext.Current.Request["addsub"].ToString() == "1")
                    {
                        if (orderid > 0)
                        {
                            InitMode_AddOrder(orderid, true);
                            //初始化子订单状态下的DDLQType2
                            DropDownListQType2.Items.Clear();
                            string Type1ID = DropDownListQType1.SelectedValue;
                            string[] QueryString = { "*", "questiontypelv2", "Type1ID", Type1ID };
                            DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
                            DataTable dt = ds.Tables[0];
                            DropDownListQType2.DataSource = dt.DefaultView;
                            DropDownListQType2.DataTextField = "Type2Name";
                            DropDownListQType2.DataValueField = "Type2ID";
                            DropDownListQType2.DataBind();
                        }
                    }
                }
                else
                {
                    InitMode_AddOrder(0, false);
                }
            }
        }
    }

    /// <summary>
    /// 增加用户模式主模块
    /// </summary>
    /// <param name="OrderID">订单号</param>
    /// <param name="SubMode">增加子订单标记</param>
    protected void InitMode_AddOrder(int OrderID, bool SubMode)
    {
        //增加子菜单模式
        if (SubMode)
        {
            string[] QString_records = { "*", "records", "OrderID", OrderID.ToString() };
            string[] QString_dealorder = { "*", "dealorder", "DealOrderID", OrderID.ToString() };
            DataSet ds_dealorder = MysqlExe.getDataSet(Conn, QString_dealorder);
            DataSet ds_records = MysqlExe.getDataSet(Conn, QString_records);
            DataRow dr_records = ds_records.Tables[0].Rows[0];
            DataRow dr_dealorder = ds_dealorder.Tables[0].Rows[0];
            //系统生成订单信息加载
            inputOrderID.Value = OrderID.ToString();
            inputSubOrderID.Value = ((Convert.ToInt32(dr_records["DealPeopleNum"].ToString()) + 1).ToString()).PadLeft(4, '0');
            selectStateCode.SelectedIndex = 2;
            //报单人信息加载，禁用部分模块的输入
            inputQID.Attributes.Add("disabled", "disabled");
            inputQID.Value = dr_records["QID"].ToString();
            inputQname.Attributes.Add("disabled", "disabled");
            inputQname.Value = dr_records["QName"].ToString();
            inputQtelephone.Value = dr_records["QTelephone"].ToString();
            inputQtelephone.Attributes.Add("disabled", "disabled");
            DropDownListBlockID.SelectedValue = dr_records["BID"].ToString();
            DropDownListBlockID.Attributes.Add("disabled", "disabled");
            inputBlockRoom.Attributes.Add("disabled", "disabled");
            inputBlockRoom.Value = dr_records["BlockRoom"].ToString();
            DropDownListQType1.SelectedValue = dr_dealorder["Type1ID"].ToString();
            DropDownListQType2.SelectedValue =dr_dealorder["Type2ID"].ToString();
            textareaQdescription.Value = dr_records["QDescription"].ToString();
            textareaQdescription.Attributes.Add("disabled", "disabled");
            //写入隐藏域
            HiddenField_BlockID.Value = dr_records["BID"].ToString();
            HiddenField_BlockID_Name.Value = DropDownListBlockID.SelectedItem.ToString();
        }
        //普通模式（增加新订单）
        else
        {
            string month_msg = DateTime.Now.ToString("yyyyMM");
            string QString_records = "select OrderID from records where OrderID like '%"+month_msg+ "%' order by CAST(OrderID as UNSIGNED) desc limit 0,1";
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = QString_records;
            string olderOrderID = null;
            try
            {
                Conn.Open();
                MySqlDataReader Feedback = cmd.ExecuteReader();
                if (Feedback.HasRows)
                {
                    if (Feedback.Read()) olderOrderID = Feedback[0].ToString();
                }
                Feedback.Close();   //关闭DataReader
            }
            catch (Exception ex)
            {
                labelAlterDanger.Text = "出错了，请联系管理员。出错信息为：" + ex.Message;
                divAlterDanger.Visible = true;
            }
            finally
            {
                Conn.Close();
            }
            if (olderOrderID != null) inputOrderID.Value = (Convert.ToInt32(olderOrderID) + 1).ToString();
            else inputOrderID.Value = month_msg + (1.ToString().PadLeft(4, '0'));
            inputSubOrderID.Value = 1.ToString().PadLeft(4, '0');
        }
        //共通模块：填表人信息
        Cookies ReadLocal = new Cookies();
        string[] QueryCookies = { "RealName", "CardID" };
        string[] ResultCookies = ReadLocal.ReadCookies(QueryCookies);
        inputLogRealName.Value = ResultCookies[0];
        inputLogCardID.Value = ResultCookies[1];
        inputLogTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    /// <summary>
    /// 显示页码导航的实现模块
    /// </summary>
    /// <param name="NowPage"></param>
    /// <param name="SumRecords"></param>
    /// <param name="OrderID"></param>
    protected void PaginationShow(int NowPage, int SumRecords, string OrderID)
    {
        string OrderID_Para = "&addsub=1&orderid=" + OrderID;
        if (OrderID == null)
        {
            OrderID_Para = null;
        }
        int dividedby = 10;   //默认分页条数
        int Pages_Num = SumRecords / dividedby + 1;    //总页数（最后一页导航）
        int list_Num = 5;   //当前最大显示分块
        int var_Page = 0;
        LiteralControl FirstPage = new LiteralControl(), LastPage = new LiteralControl();
        FirstPage.Text = "<li><a href=order.aspx?page=1" + OrderID_Para + ">&laquo;第一页</a></li>";
        LastPage.Text = "<li><a href=order.aspx?page=" + Pages_Num + OrderID_Para + ">最后一页&raquo;</a></li>";
        if (Pages_Num <= list_Num)
        {
            int i = 1; //Page-1
            if (NowPage == 1)
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
                        ListPage.Text = "<li class=active><a>" + NowPage + "</a></li>";
                    }
                    else
                    {
                        ListPage.Text = "<li><a href=order.aspx?page=" + i + OrderID_Para + ">" + i + "</a></li>";
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
                        ListPage.Text = "<li><a href=order.aspx?page=" + var_Page + OrderID_Para + ">" + var_Page + "</a></li>";
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
    /// 初始化追加订单状态显示表格模块
    /// </summary>
    /// <param name="ShowUser"></param>
    /// <param name="nowPage"></param>
    protected void InitMode_AddSubOrderShow(Table ShowUser,int nowPage)
    {
	labelAlterDanger.Text = null;
        labelAlterSuccess.Text = null;
        divAlterDanger.Visible = false;
	divAlterSuccess.Visible = false;
        divAddNewOrder.Visible = false;
        divAddSubOrder.Visible = true;          //显示初始化
        TableRow tRow = new TableRow();
        TableCell tCell = new TableCell();
        string[] Titles = { "订单ID", "最新解决时间", "订单状态", "处理人", "处理意见", "追加" };
        //读数据库
        UsingMysql MysqlExe = new UsingMysql();
        MySqlConnection Conn = MysqlExe.ConnMode;  //MySQL连接串
        string connstring_limitpage = "SELECT r.OrderID, d.DealKey, d.DealTime, r.StateCode, d.DealName, d.DealFeedback FROM dealorder as d , records as r where r.DealPeopleNum=CAST(d.DealID as UNSIGNED) and r.OrderID=d.DealOrderID and datediff('" + DateTime.Now.ToString("yyyy-MM-dd") + "',d.DealTime)<2 and (r.StateCode=3 OR r.StateCode=4) limit " + (nowPage - 1) * 10 + ",10";
        MySqlCommand cmd = new MySqlCommand(connstring_limitpage, Conn);
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            Conn.Close();
            //写入展示表格
            int numRows = ds.Tables[0].Rows.Count + 1;       //当期显示总行数
            int numCells = 6;               //列数
            for (int iRow = 0; iRow < numRows; iRow++)
            {
                tRow = new TableRow();
                if(iRow == 0)
                {    
                    for(int jCol = 0; jCol < numCells; jCol++)         //表格首行
                    {
                        tCell = new TableCell();
                        tCell.Text = Titles[jCol];
                        tRow.Cells.Add(tCell);
                    }
                }
                else
                {
                    DataRow dr = ds.Tables[0].Rows[iRow - 1];
                    TableCell[] tCells = new TableCell[numCells];
                    for (int i = 0; i < numCells; i++) tCells[i] = new TableCell();
                    for (int jrow = 0; jrow < (numCells - 1); jrow++)        //前4列（数据库输出时已经做好对应关系）
                    {
                        tCells[jrow].Text = dr[jrow + 1].ToString();
                        tRow.Cells.Add(tCells[jrow]);
                    }
                    tCells[numCells - 1].Text = "<a href=order.aspx?addsub=1&orderid=" + dr["OrderID"].ToString() + ">";
                    tCells[numCells - 1].Text += "<span class='glyphicon glyphicon-plus'></span></a>";
                    tRow.Cells.Add(tCells[numCells - 1]);
                }
                ShowUser.Rows.Add(tRow);               //增加一行记录
            }
            //计算记录总数
            string connstring_all = "SELECT count(*) FROM dealorder as d , records as r where r.DealPeopleNum=CAST(d.DealID as UNSIGNED) and r.OrderID=d.DealOrderID and datediff(d.DealTime.Now())<2 and (r.StateCode=3 OR r.StateCode=4) ";
            Conn.Open();
            cmd = new MySqlCommand(connstring_all, Conn);
            da.SelectCommand = cmd;
            DataSet ds_count = new DataSet();
            da.Fill(ds_count);
            Conn.Close();
            int SumRecords = 0;
            if (ds_count.Tables[0].Rows.Count > 0)
            {
                SumRecords = Convert.ToInt32(ds_count.Tables[0].Rows[0][0].ToString());
            }
            if (SumRecords == 0) 
            {
                divAlterDanger.Visible = true;
                labelAlterDanger.Text = "当前还未有两天内可以进行追加的订单。";
            }
            //制作页码导航条
            PaginationShow(nowPage, SumRecords, null);
        }
        catch (Exception ex)
            {
                labelAlterDanger.Text += "出错了，请联系管理员。错误提示：" + ex.Message;
                divAlterDanger.Visible = true;
            }
    }
    /// <summary>
    /// BlockID的下拉菜单数据绑定
    /// </summary>
    protected void DDLBind_BlockID()
    {
        string[] QueryString = { "*", "blocks" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DropDownListBlockID.DataSource = dt.DefaultView;
        DropDownListBlockID.DataTextField = "BlockName";
        DropDownListBlockID.DataValueField = "BID";
        DropDownListBlockID.DataBind();
        DropDownListBlockID.SelectedValue = "OTHER";
    }
    /// <summary>
    /// Appointer的下拉菜单数据绑定
    /// </summary>
    protected void DDLBind_Appointer()
    {
        string[] QueryString = { "RealName,UID", "account" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        ListItem NullSelect = new ListItem();
        DropDownListAppointer.DataSource = dt.DefaultView;
        DropDownListAppointer.DataTextField = "RealName";
        DropDownListAppointer.DataValueField = "UID";
        DropDownListAppointer.DataBind();
        NullSelect.Text = "不指派";
        NullSelect.Value = "0";
        DropDownListAppointer.Items.Add(NullSelect);
        DropDownListAppointer.SelectedValue = "0";
    }
    /// <summary>
    /// QType1的下拉菜单数据绑定
    /// </summary>
    protected void DDLBind_QType1()
    {
        string[] QueryString = { "*", "questiontypelv1" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DropDownListQType1.DataSource = dt.DefaultView;
        DropDownListQType1.DataTextField = "Type1Name";
        DropDownListQType1.DataValueField = "Type1ID";
        DropDownListQType1.DataBind();
        ListItem itemNull = new ListItem();
        itemNull.Text = "---请选择---";
        itemNull.Value = null;
        DropDownListQType1.Items.Insert(0, itemNull);
    }
    /// <summary>
    /// QType2的下拉菜单数据绑定(通过QType1回传结果更改）
    /// </summary>
    protected void DDLBind_QType1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownListQType2.Items.Clear();
        string Type1ID = DropDownListQType1.SelectedValue;
        string[] QueryString = { "*", "questiontypelv2", "Type1ID", Type1ID };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DropDownListQType2.DataSource = dt.DefaultView;
        DropDownListQType2.DataTextField = "Type2Name";
        DropDownListQType2.DataValueField = "Type2ID";
        DropDownListQType2.DataBind();
    }
/// <summary>
    /// 判断子订单原提交表单的有效性
    /// </summary>
    /// <param name="CheckParas"></param>
    /// <returns></returns>
    protected int Check_SubOrder_IsAvailable(string[] CheckParas)
    {
        //CheckParas数组参数：OrderID、DealKey、QID、QName、Type1ID、Type2ID
        string[] paras_records = { "DealPeopleNum,QID,QName", "records", "OrderID", CheckParas[0] };
        string[] paras_dealorder = { "DealID", "dealorder", "DealOrderID", CheckParas[0] };
        DataSet ds_records = MysqlExe.getDataSet(Conn, paras_records);
        DataSet ds_dealorder = MysqlExe.getDataSet(Conn, paras_dealorder);
        DataTable dt_records = ds_records.Tables[0], dt_dealorder = ds_dealorder.Tables[0];
        if (dt_dealorder.Rows.Count == 0 || dt_records.Rows.Count == 0) 
        {
            return 1;   //第一个订单
        }
        else
        {
            int DealKey = Convert.ToInt32(CheckParas[1]), DealPeople = Convert.ToInt32(dt_records.Rows[0]["DealPeopleNum"].ToString()) + 1;
            string QID = dt_records.Rows[0]["QID"].ToString(), QName = dt_records.Rows[0]["QName"].ToString();
            if (DealKey == DealPeople && QID == CheckParas[2] && QName == CheckParas[3])
            {
                return 2;   //子订单有效
            }
            else if(DealKey != DealPeople && QID == CheckParas[2] && QName == CheckParas[3])
            {
                labelAlterDanger.Text = "有数据更新，请重新进入页面，填写表格。";
                divAlterDanger.Visible = true;
                return 0;
            }
            else
            {
                labelAlterDanger.Text = "请勿修改旧数据，刷新页面后再重新填写。";
                divAlterDanger.Visible = true;
                return 0;
            }
        }
    }
    /// <summary>
    /// 判断订单是否为非空，为空则变红，返回判断结果
    /// </summary>
    /// <returns></returns>
    protected bool Check_Order_IsNotNull()
    {
        string QID = inputQID.Value, QName = inputQname.Value, Qtelephone = inputQtelephone.Value;
        string Qdescription = textareaQdescription.Value;
        string BlockID = DropDownListBlockID.SelectedValue, BlockRoom = inputBlockRoom.Value;
        string Type1ID = DropDownListQType1.SelectedValue;
        string Type2ID = DropDownListQType2.SelectedValue;
        if (QID != "" &&QName != "" && Qtelephone != "" && Qdescription != ""&&BlockID != ""&&BlockRoom != ""&&Type1ID != ""&&Type2ID != "")
        {
            return true;
        }
        else
        {
            if(QID == "" || QName == "" || Qtelephone == "")
            {
                labelAlterDanger.Text += "报单人<code>上网账号、姓名、联系方式</code> ";
            }
            if(Qdescription == "")
            {
                labelAlterDanger.Text += " <code>问题描述</code> ";
            }
            if(BlockID == "" || BlockRoom == "")
            {
                labelAlterDanger.Text += " <code>报单人地址</code> ";
            }
            if(Type1ID == "" || Type2ID == "")
            {
                labelAlterDanger.Text += " <code>问题分类</code> ";
            }
            labelAlterDanger.Text += "没有填写完整或者填写错误，请检查您的输入！";
            divAlterDanger.Visible = true;
            return false;
        }
    }
    /// <summary>
    /// 提交按钮操作，通过判断父订单是否存在、人数判断是否
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        labelAlterDanger.Text = null;
        divAlterDanger.Visible = false;
        //数组参数：OrderID、DealKey、QID、QName、Type1ID、Type2ID
        string[] judge = { inputOrderID.Value, inputSubOrderID.Value, inputQID.Value, inputQname.Value, DropDownListQType1.SelectedValue, DropDownListQType2.SelectedValue };
        //判断订单信息是否齐全
        if (Check_Order_IsNotNull())
        {
            //判断当前是否为子订单状态,1为初始订单，2为子订单，0为订单无效
            int result = Check_SubOrder_IsAvailable(judge);
            //公共获取部分
            string StateCode = selectStateCode.SelectedIndex.ToString();
            Cookies ReadCookies = new Cookies();
            string[] QueryString = { "CardID", "RealName", "uid" };
            string[] CookiesResult = ReadCookies.ReadCookies(QueryString);
            string CardID = CookiesResult[0], RealName = CookiesResult[1], UID = CookiesResult[2];
            string OrderID = inputOrderID.Value, DealID = inputSubOrderID.Value;
            string DealUID = DropDownListAppointer.SelectedValue.ToString();
            string DealName = DropDownListAppointer.SelectedItem.ToString();
            //Cookies判断
            if (CardID != inputLogCardID.Value || RealName != inputLogRealName.Value)
            {
                labelAlterDanger.Text = "信息有误，请刷新页面后再试！";
                divAlterDanger.Visible = true;
            }
            else
            {
                try
                {
                    string LogTime = inputLogTime.Value;
                    string QID = inputQID.Value, QName = inputQname.Value, Qtelephone = inputQtelephone.Value;
                    string Qdescription = textareaQdescription.Value;
                    string BlockID = DropDownListBlockID.SelectedValue, BlockRoom = inputBlockRoom.Value;
                    string Type1ID = DropDownListQType1.SelectedValue;
                    string Type2ID = DropDownListQType2.SelectedValue;
                    string BlockName = DropDownListBlockID.SelectedItem.ToString();
                    string Type1Name = DropDownListQType1.SelectedItem.ToString();
                    string Type2Name = DropDownListQType2.SelectedItem.ToString();
                    if (result == 1)    //初始订单
                    {
                        MySqlCommand cmd = Conn.CreateCommand();
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        //操作dealorder表
                        if (DealUID != "0")
                        {
                            cmd.CommandText = "insert into dealorder (DealKey,DealID,DealOrderID,LogUID,LogName,LogTime,DealUID,DealName,Type1ID,Type2ID) values(@DealKey,@DealID,@DealOrderID,@LogUID,@LogName,@LogTime,@DealUID,@DealName,@Type1ID,@Type2ID)";
                            cmd.Parameters.Add(new MySqlParameter("@DealUID", DealUID));
                            cmd.Parameters.Add(new MySqlParameter("@DealName", DealName));
                        }
                        else
                        {
                            cmd.CommandText = "insert into dealorder (DealKey,DealID,DealOrderID,LogUID,LogName,LogTime,Type1ID,Type2ID) values(@DealKey,@DealID,@DealOrderID,@LogUID,@LogName,@LogTime,@Type1ID,@Type2ID)";
                        }
                        //参数表
                        cmd.Parameters.Add(new MySqlParameter("@DealKey", OrderID + DealID));
                        cmd.Parameters.Add(new MySqlParameter("@DealID", DealID));
                        cmd.Parameters.Add(new MySqlParameter("@DealOrderID", OrderID));
                        cmd.Parameters.Add(new MySqlParameter("@LogUID", UID));
                        cmd.Parameters.Add(new MySqlParameter("@LogName", RealName));
                        cmd.Parameters.Add(new MySqlParameter("@LogTime", LogTime));
                        cmd.Parameters.Add(new MySqlParameter("@OrderID", OrderID));
                        cmd.Parameters.Add(new MySqlParameter("@QID", QID));
                        cmd.Parameters.Add(new MySqlParameter("@QTelephone", Qtelephone));
                        cmd.Parameters.Add(new MySqlParameter("@QName", QName));
                        cmd.Parameters.Add(new MySqlParameter("@QDescription", Qdescription));
                        cmd.Parameters.Add(new MySqlParameter("@BID", BlockID));
                        cmd.Parameters.Add(new MySqlParameter("@BlockRoom", BlockRoom));
                        cmd.Parameters.Add(new MySqlParameter("@Type1ID", Type1ID));
                        cmd.Parameters.Add(new MySqlParameter("@Type2ID", Type2ID));
                        Conn.Open();
                        int Feedback = cmd.ExecuteNonQuery();
                        Conn.Close();
                        if (Feedback <= 0)
                        {
                            labelAlterDanger.Text += "导入订单记录表失败！";
                            divAlterDanger.Visible = true;
                        }
                        else
                        {
                            if (Conn.State == ConnectionState.Open)
                            {
                                Conn.Close();
                            }
                            //写入records表
                            if (DealUID != "0")
                            {
                                cmd.CommandText = "insert into records (OrderID,QID,Qname,QTelephone,QDescription,BID,BlockRoom,DealPeopleNum,StateCode) values(@OrderID,@QID,@QName,@QTelephone,@QDescription,@BID,@BlockRoom,1,1)";
                            }
                            else
                            {
                                cmd.CommandText = "insert into records (OrderID,QID,Qname,QTelephone,QDescription,BID,BlockRoom,StateCode) values(@OrderID,@QID,@QName,@QTelephone,@QDescription,@BID,@BlockRoom,0)";
                            }

                            Conn.Open();
                            Feedback = cmd.ExecuteNonQuery();
                            Conn.Close();
                            if (Feedback <= 0)
                            {
                                labelAlterDanger.Text = "导入记录表失败！";
                                divAlterDanger.Visible = true;
                            }
                            else  //成功导入records表
                            {
                                labelAlterSuccess.Text = "新增子订单<strong>" + OrderID + DealID + "</strong>成功！点击<a href=ShowOrder.aspx?orderid=" + OrderID + "><code>这里</code></a>查看订单信息。";
                                divAlterSuccess.Visible = true;
                                //增加计数器
                                MysqlExe.UpdTotalSheets("blocks", BlockName, BlockID, false, true);
                                if (DealUID != "0") MysqlExe.UpdTotalSheets("account", DealName, DealUID, false, true);
                                MysqlExe.UpdTotalSheets("questiontypelv1", Type1Name, Type1ID, false, true);
                                MysqlExe.UpdTotalSheets("questiontypelv2", Type2Name, Type2ID, false, true);
                                MysqlExe.UpdTotalUser_Add(UID);  //用于统计登记单数

                            }
                        }
                    }
                    if (result == 2)    //子订单
                    {
                        string DealPeopleNum = Convert.ToInt32(DealID).ToString();
                        MySqlCommand cmd = Conn.CreateCommand();
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        if (DealUID != "0")
                        {
                            cmd.CommandText = "insert into dealorder (DealKey,DealID,DealOrderID,LogUID,LogName,LogTime,DealUID,DealName,Type1ID,Type2ID,StateCode) values(@DealKey,@DealID,@DealOrderID,@LogUID,@LogName,@LogTime,@DealUID,@DealName,@Type1ID,@Type2ID,1)";
                            cmd.Parameters.Add(new MySqlParameter("@DealUID", DealUID));
                            cmd.Parameters.Add(new MySqlParameter("@DealName", DealName));
                        }
                        else
                        {
                            cmd.CommandText = "insert into dealorder (DealKey,DealID,DealOrderID,LogUID,LogName,LogTime,Type1ID,Type2ID) values(@DealKey,@DealID,@DealOrderID,@LogUID,@LogName,@LogTime,@Type1ID,@Type2ID)";
                        }
                        cmd.Parameters.Add(new MySqlParameter("@DealKey", OrderID + DealID));
                        cmd.Parameters.Add(new MySqlParameter("@DealID", DealID));
                        cmd.Parameters.Add(new MySqlParameter("@DealOrderID", OrderID));
                        cmd.Parameters.Add(new MySqlParameter("@LogUID", UID));
                        cmd.Parameters.Add(new MySqlParameter("@LogName", RealName));
                        cmd.Parameters.Add(new MySqlParameter("@LogTime", LogTime));
                        cmd.Parameters.Add(new MySqlParameter("@Type1ID", Type1ID));
                        cmd.Parameters.Add(new MySqlParameter("@Type2ID", Type2ID));
                        cmd.Parameters.Add(new MySqlParameter("@OrderID", OrderID));
                        cmd.Parameters.Add(new MySqlParameter("@DealPeopleNum", DealPeopleNum));
                        Conn.Open();
                        int Feedback = cmd.ExecuteNonQuery();
                        Conn.Close();
                        if (Feedback <= 0)
                        {
                            labelAlterDanger.Text += "导入订单记录表失败！";
                            divAlterDanger.Visible = true;
                        }
                        else
                        {
                            cmd.CommandText = "update records set StateCode=2,DealPeopleNum=@DealPeopleNum where OrderID=@OrderID";
                            Conn.Open();
                            Feedback = cmd.ExecuteNonQuery();
                            Conn.Close();
                            if (Feedback <= 0)
                            {
                                labelAlterDanger.Text += "导入记录表失败！";
                                divAlterDanger.Visible = true;
                            }
                            else
                            {
                                labelAlterSuccess.Text = "新增子订单<strong>" + OrderID + DealID + "</strong>成功！点击<a href=ShowOrder.aspx?orderid=" + OrderID + "><code>这里</code></a>查看订单信息。";
                                divAlterSuccess.Visible = true;
                                //增加计数器
                                MysqlExe.UpdTotalSheets("blocks", HiddenField_BlockID_Name.Value, HiddenField_BlockID.Value, false, true);
                                if (DealUID != "0") MysqlExe.UpdTotalSheets("account", DealName, DealUID, false, true);
                                MysqlExe.UpdTotalSheets("questiontypelv1", Type1Name, Type1ID, false, true);
                                MysqlExe.UpdTotalSheets("questiontypelv2", Type2Name, Type2ID, false, true);
                                MysqlExe.UpdTotalUser_Add(UID);   //用于统计登记单数
                            }
                        }
                    }
                    if (DealID != "0")
                    {
                        MySqlCommand cmd = Conn.CreateCommand();
                        cmd.CommandText = "update records set StateCode=1 where OrderID=@OrderID";
                        cmd.Parameters.Add(new MySqlParameter("@OrderID", OrderID));
                        Conn.Open();
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                }

                catch (Exception ex)
                {
                    labelAlterDanger.Text += "出错了，请联系管理员。错误提示：" + ex.Message;
                    divAlterDanger.Visible = true;
                }
            }

        }
    }
    /// <summary>
    /// 添加新订单按钮操作
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddNewOrder_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=order.aspx");
    }
    /// <summary>
    /// 追加订单状态按钮操作
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdditionNewOrder_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL='order.aspx?page=1'");
    }
}