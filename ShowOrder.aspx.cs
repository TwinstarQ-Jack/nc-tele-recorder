using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using MySql.Data;
using MySql.Web;

public partial class ShowOrder : System.Web.UI.Page
{
    static UsingMysql MysqlExe = new UsingMysql();
    MySqlConnection Conn = MysqlExe.ConnMode;
    int SubTableCreateTimes = 1;     //是否写入最新隐藏域标记
    protected void Page_Load(object sender, EventArgs e)
    {
        int SubOrder_Counter = 0;
        string OrderID = null;
        if (HttpContext.Current.Request["orderid"] != null)
        {
            OrderID = HttpContext.Current.Request["orderid"].ToString();
            SubOrder_Counter = Num_Of_SubOrder(OrderID);
        }
        if (!IsPostBack)         //非回调
        {
            DDLBind_QType1();
            //加载确认订单页
            Init_ConfirmPage(OrderID + SubOrder_Counter.ToString().PadLeft(4, '0'));
            DDLBind_TransferUser();
            //第一次进入，加载QType2DDL
            string Type1ID = DropDownListQType1.SelectedValue;
            string[] QueryString = { "*", "questiontypelv2", "Type1ID", Type1ID };
            DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
            DataTable dt = ds.Tables[0];
            DropDownListQType2.DataSource = dt.DefaultView;
            DropDownListQType2.DataTextField = "Type2Name";
            DropDownListQType2.DataValueField = "Type2ID";
            DropDownListQType2.DataBind();
        }
        if (SubOrder_Counter > 0)
        {
            for (int i = SubOrder_Counter; i > 0; i--)
            {
                string SubID = i.ToString().PadLeft(4, '0');
                Dynamic_Create_SubOrder_Table(OrderID + SubID);
                SubTableCreateTimes++;
            }
         }
        //加载父订单消息
        Init_FatherOrderPage(OrderID);
    }
    /// <summary>
    /// 清空所有错误信息提示
    /// </summary>
    protected void ClearErrorMsg()
    {
        divAlterSuccess.Visible = false;
        divAlterDanger.Visible = false;
        labelAlterDanger.Text = null;
        labelAlterSuccess.Text = null;
    }

    /// <summary>
    /// 初始化订单确认页数据
    /// </summary>
    /// <param name="DealKey"></param>
    protected void Init_ConfirmPage(string DealKey)
    {
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        string QueryString = "select Type1ID,Type2ID from dealorder where DealKey=@DealKey";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = QueryString;
        cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
        MySqlDataAdapter da = new MySqlDataAdapter();
        DataSet ds = new DataSet();
        da.SelectCommand = cmd;
        try
        {
            Conn.Open();
            da.Fill(ds);
            Conn.Close();
            //填充
            modal_SubOrder_Deal.InnerText = DealKey;
            modal_Cancel_Deal.InnerText = DealKey;
            modal_Remarks_Deal.InnerText = DealKey;
            DataRow dr = ds.Tables[0].Rows[0];
            DropDownListQType1.SelectedValue = dr["Type1ID"].ToString();
            DropDownListQType2.SelectedValue = dr["Type2ID"].ToString();
            modal_SubOrder_DealTime.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            modal_Cancel_DealTime.InnerText = modal_SubOrder_DealTime.InnerText;
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text = "发生错误，请联系管理员。(InitConfirmError)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
    protected void Init_FatherOrderPage(string OrderID)
    {
        ClearErrorMsg();
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        string QueryString = "select * from records where OrderID=@OrderID";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = QueryString;
        cmd.Parameters.Add(new MySqlParameter("@OrderID", OrderID));
        MySqlDataAdapter da = new MySqlDataAdapter();
        DataSet ds = new DataSet();
        da.SelectCommand = cmd;
        try
        {
            Conn.Open();
            da.Fill(ds);
            Conn.Close();
            string BlockName = null;
            DataRow dr = ds.Tables[0].Rows[0];
            string[] QueryBlock = { "BlockName", "blocks", "BID", "'" + dr["BID"].ToString() + "'" };
            DataSet ds_Block = MysqlExe.getDataSet(Conn, QueryBlock);
            BlockName = DataSet_row0_0_value(ds_Block);
            //写入隐藏域
            HiddenField_BlocksID.Value = dr["BID"].ToString();
            HiddenField_Blocks_Name.Value = BlockName;
            //订单状态
            string[] Status = { "未处理订单", "正在处理", "需要跟进", "已解决", "已取消", "已转让" };
            string Status_Name = Status[Convert.ToInt32(dr["StateCode"].ToString())];
            //功能按钮区初始化
            Cookies ReadRank = new Cookies();
            String[] QueryCookies = { "rank", "RealName" };
            string[] Result_cookies = ReadRank.ReadCookies(QueryCookies);
            string rank_from_cookies = Result_cookies[0];
            string realname_from_cookies = Result_cookies[1];
            if (rank_from_cookies == "StudAdmin" || rank_from_cookies == "TeacherAdmin")
            {
                if (Status_Name != "已解决" && Status_Name != "已取消")
                {
                    abtnSubOrderCancel.Visible = true;
                    abtnSubOrderRemarks.Visible = true;
                }
            }
            if (realname_from_cookies == HiddenField_DealName.Value)
            {
                abtnSubOrderRemarks.Visible = true;
            }
            if (Status_Name != "已解决" && Status_Name != "已取消")
            {
                abtnSubOrderAdd.Visible = false;
            }
            else
            {
                abtnSubOrderConfirm.Visible = false;
            }
            if (Status_Name == "正在处理")
            {
                abtnSubOrderTransfer.Visible = true;
            }
            if (Status_Name == "未处理订单" || HiddenField_DealName.Value == "")
            {
                //仅开放接单申请
		abtnSubOrderRemarks.Visible = true;
                abtnSubOrderReceive.Visible = true;
                abtnSubOrderCancel.Visible = false;
                abtnSubOrderTransfer.Visible = false;
                abtnSubOrderConfirm.Visible = false;
            }
            //填充
            labelOrderID_heading.InnerText = OrderID;
            labelOrderID_OrderID.InnerText = OrderID;
            labelOrder_DealNum.InnerText = dr["DealPeopleNum"].ToString();
            labelOrder_QName.InnerText = dr["QName"].ToString();
            labelOrder_QID.InnerText = dr["QID"].ToString();
            labelOrder_Tele.InnerText = dr["QTelephone"].ToString();
            labelOrder_Description.InnerText = dr["QDescription"].ToString();
            labelOrder_Blocks.InnerText = BlockName + " - " + dr["BlockRoom"].ToString();
            labelOrder_StampTime.InnerText = dr["StampTime"].ToString();
            labelOrder_Status.InnerText = Status_Name;
	    if(labelOrder_QName.InnerText == "待接")	abtnSubOrderTransfer.Visible = true;
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text = "发生错误，请联系管理员。(InitFatherOtherError)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
    protected string DataSet_row0_0_value (DataSet ds)
    {
        try
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    protected string getRemarks(string DealKey)
    {
        MySqlConnection Conn = MysqlExe.ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        MySqlDataAdapter da = new MySqlDataAdapter();
        DataSet ds = new DataSet();
        cmd.CommandText = "select * from dealorderremark where DealKey=@DealKey order by CAST(RemarkID AS UNSIGNED) desc";
        cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Conn.Close();
            int Counts = ds.Tables[0].Rows.Count;
            if (Counts > 0)
            {
                string result = "<ul>";
                for (int i = 0; i < Counts; i++) 
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    result += "<li><div>" + dr["Content"].ToString() + "</div><div>" + dr["RemarkerName"].ToString() + "," + dr["LogTime"].ToString() + "。</div></li>";
                }
                result += "</ul>";
                return result;
            }
            else
            {
                return "本订单还没有备注。";
            }
        }
        catch (Exception ex)
        {
            Conn.Close();
            return "错误提示为：(getRemarksERROR)" + ex.Message;
        }
    }
    /// <summary>
    /// 动态创建子订单表格
    /// </summary>
    /// <param name="DealKey"></param>
    /// <returns></returns>
    protected void Dynamic_Create_SubOrder_Table(string DealKey)
    {
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        string QueryString = "select * from dealorder where DealKey=@DealKey";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = QueryString;
        cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
        MySqlDataAdapter da = new MySqlDataAdapter();
        DataSet ds = new DataSet();
        da.SelectCommand = cmd;
        try
        {
            Conn.Open();
            da.Fill(ds);
            Conn.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                //子表格处理
                DataRow tr = ds.Tables[0].Rows[0];
                string Type1ID = tr["Type1ID"].ToString(), Type2ID = tr["Type2ID"].ToString();
                string LogName = tr["LogName"].ToString(), LogTime = tr["LogTime"].ToString();
                string DealName = tr["DealName"].ToString(), CheckUserName = tr["CheckUserName"].ToString();
                string DealTime = tr["DealTime"].ToString(), StampTime = tr["StampTime"].ToString();
                string DealFeedback = tr["DealFeedback"].ToString();
                //加载子订单备注
                string Remarks = getRemarks(DealKey);
                //数据库查询字段
                string[] QueryType1 = { "Type1Name", "questiontypelv1", "Type1ID", Type1ID };
                string[] QueryType2 = { "Type2Name", "questiontypelv2", "Type2ID", Type2ID };
                string[] QueryUID = { "UID", "account", "RealName", "'" + DealName + "'" };
                //订单状态
                string[] Status = { "未处理订单", "正在处理", "需要跟进", "已解决", "已取消", "已转让" };
                string Status_Name = Status[Convert.ToInt32(tr["StateCode"].ToString())];
                DataSet ds_Type1 = MysqlExe.getDataSet(Conn, QueryType1);
                DataSet ds_Type2 = MysqlExe.getDataSet(Conn, QueryType2);
                DataSet ds_UID = MysqlExe.getDataSet(Conn, QueryUID);
                string T1Name = DataSet_row0_0_value(ds_Type1), T2Name = DataSet_row0_0_value(ds_Type2);
                //TypeID填入隐藏域，供确认订单时确认是否修改，且在第一次创建时有效
                if (SubTableCreateTimes == 1)
                {
                    HiddenField_Type1ID_Name.Value = T1Name;
                    HiddenField_Type2ID_Name.Value = T2Name;
                    HiddenField_Type1ID.Value = Type1ID;
                    HiddenField_Type2ID.Value = Type2ID;
                    HiddenField_DealName.Value = DealName;
                    HiddenField_DealName_UID.Value = DataSet_row0_0_value(ds_UID);
                }
                //处理动态创建内容
                string Header = "<div class='panel-heading'>子订单&nbsp;<code>" + DealKey + "</code>&nbsp;详细情况</div>";
                string Feedback = "<div class='table-responsive'><table class='table table-bordered'><tr><td>订单号</td><td>" + DealKey + "</td><td>问题分类</td><td>" + T1Name + " - " + T2Name + "</td></tr>";
                Feedback += "<tr><td>登记人</td><td>" + LogName + "</td><td>登记时间</td><td>" + LogTime + "</td><td>接单人</td><td>" + DealName + "</td></tr>";
                Feedback += "<tr><td>订单确认人</td><td>" + CheckUserName + "</td><td>确认时间</td><td>" + DealTime + "</td><td>子订单状态</td><td>" + Status_Name + "</td></tr>";
                Feedback += "<tr><td>处理意见</td><td colspan='5'>" + DealFeedback + "</td></tr>";
                Feedback += "<tr><td>经手人备注</td><td colspan='5'>" + Remarks + "</td></tr>";
                //图片区域处理
                int Images_Num = Convert.ToInt32(tr["DealPicNum"].ToString());
                string Img_Insert_String = null;
                if (Images_Num > 0)
                {
                    string DealPicPath = tr["DealPicPath"].ToString();
                    string[] PicPathArray = DealPicPath.Trim(',').Split(',');   //拆分存放图片的string
                    for (int i = 0; i < Images_Num; i++)
                    {
                        Img_Insert_String += "<img src='..attachment/" + PicPathArray[i] + "' />";
                    }
                }
                Feedback += "<tr><td>图片</td><td colspan='5'>" + Img_Insert_String + "</td></tr></table></div>";
                string Footer = "<div class='panel-footer'>本记录更新时间："+StampTime+"</div>";
                string div_InnerHtml = Header + Feedback + Footer;
                //动态加载子订单
                HtmlGenericControl div_dynamic_panel = new HtmlGenericControl();
                div_dynamic_panel.TagName = "div";
                div_dynamic_panel.InnerHtml = div_InnerHtml;
                switch (Status_Name)
                {
                    case "已解决": div_dynamic_panel.Attributes["class"] = "panel panel-success";
                        break;
                    case "已取消": div_dynamic_panel.Attributes["class"] = "panel panel-danger";
                        break;
                    case "正在处理": div_dynamic_panel.Attributes["class"] = "panel panel-info";
                        break;
                    case "已转让":div_dynamic_panel.Attributes["class"] = "panel panel-warning";
                        break;
                    default: div_dynamic_panel.Attributes["class"] = "panel panel-primary";
                        break;
                }
                CreateSubOrder.Controls.Add(div_dynamic_panel);
            }
            else
            {
                labelAlterDanger.Text = "系统没有获取到子订单信息。";
                divAlterDanger.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text = "发生错误，请联系管理员。错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
    /// <summary>
    /// 读取数据库处理人数，以显示当前子订单的条数
    /// </summary>
    /// <param name="OrderID">父订单编号</param>
    /// <returns>父订单共处理人数（子订单数目）</returns>
    protected int Num_Of_SubOrder(string OrderID)
    {
        if(Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        string QueryNum = "select DealPeopleNum from records where OrderID=@OrderID";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = QueryNum;
        cmd.Parameters.Add(new MySqlParameter("@OrderID", OrderID));
        MySqlDataAdapter da = new MySqlDataAdapter();
        DataSet ds = new DataSet();
        try
        {
            da.SelectCommand = cmd;
            Conn.Open();
            da.Fill(ds);
            Conn.Close();
            int Feedback = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            if (Feedback == 0)
            {
                labelAlterDanger.Text = "暂无该父订单的子订单消息。";
                divAlterDanger.Visible = true;
                Feedback = 1;
            }
            return Feedback;
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text = "发生错误，请联系管理员。错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
            return -1;
        }
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
    }
    /// <summary>
    /// QType2的下拉菜单数据绑定（通过QType1更改确定内容）
    /// </summary>
    protected void DDLBind_QType2_SelectedIndexChanged(object sender, EventArgs e)
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
    /// TransferUser的下拉菜单数据绑定
    /// </summary>
    protected void DDLBind_TransferUser()
    {
        string[] QueryString = { "UID,RealName", "account" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DropDownList_TransferUser.DataSource = dt.DefaultView;
        DropDownList_TransferUser.DataTextField = "RealName";
        DropDownList_TransferUser.DataValueField = "UID";
        DropDownList_TransferUser.DataBind();
    }
    /// <summary>
    /// 确认订单按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void modal_SubOrder_Submit_Click(object sender, EventArgs e)
    {
        ClearErrorMsg();
        //获取当前用户
        Cookies CookiesExe = new Cookies();
        string[] QueryLoginUser = { "RealName" };
        string CheckUserName = CookiesExe.ReadCookies(QueryLoginUser)[0];
        string DealKey = modal_SubOrder_Deal.InnerText;
        string Description = modal_SubOrder_DealFeedback.Value;
        string Type1ID_Origin = HiddenField_Type1ID.Value, Type2ID_Origin = HiddenField_Type2ID.Value;
        string Type1ID_After = DropDownListQType1.SelectedValue, Type2ID_After = DropDownListQType2.SelectedValue;
        MySqlCommand cmd = Conn.CreateCommand();
        string UpdateToDealOrder = null;
        bool Type1ID_IsChange = (Type1ID_Origin != Type1ID_After);
        bool Type2ID_IsChange = (Type2ID_Origin != Type2ID_After);
        //更新dealorder表
        if (Type1ID_IsChange || Type2ID_IsChange) 
        {
            UpdateToDealOrder = "update dealorder set DealFeedback=@DealFeedBack,StateCode=3,Type1ID=@Type1ID,Type2ID=@Type2ID,CheckUserName=@CheckUserName,DealTime=NOW() where DealKey=@DealKey";
        }
        else
        {
            UpdateToDealOrder = "update dealorder set DealFeedback=@DealFeedBack,StateCode=3,CheckUserName=@CheckUserName,DealTime=NOW() where DealKey=@DealKey";
        }
        cmd.CommandText = UpdateToDealOrder;
        cmd.Parameters.Add(new MySqlParameter("@DealFeedBack", Description));
        cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
        cmd.Parameters.Add(new MySqlParameter("@Type1ID", Type1ID_After));
        cmd.Parameters.Add(new MySqlParameter("@Type2ID", Type2ID_After));
        cmd.Parameters.Add(new MySqlParameter("@CheckUserName", CheckUserName));
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            Conn.Open();
            int Feedback = cmd.ExecuteNonQuery();
            Conn.Close();
            if (Feedback > 0)
            {
                //更新records表
                string UpdateToRecords = "update records set StateCode=3 where OrderID=@OrderID";
                cmd.CommandText = UpdateToRecords;
                cmd.Parameters.Add(new MySqlParameter("@OrderID", labelOrderID_OrderID.InnerText));
                Conn.Open();
                Feedback = cmd.ExecuteNonQuery();
                Conn.Close();
                if (Feedback > 0)
                {
                    if (Type1ID_IsChange)
                    {
                        MysqlExe.UpdTotalSheets("questiontypelv1", HiddenField_Type1ID_Name.Value, Type1ID_Origin, false, false);
                        MysqlExe.UpdTotalSheets("questiontypelv1", DropDownListQType1.SelectedItem.ToString(), Type1ID_After, false, true);
                    }
                    if (Type2ID_IsChange)
                    {
                        MysqlExe.UpdTotalSheets("questiontypelv2", HiddenField_Type2ID_Name.Value, Type2ID_Origin, false, false);
                        MysqlExe.UpdTotalSheets("questiontypelv2", DropDownListQType2.SelectedItem.ToString(), Type2ID_After, false, true);
                    }
                    MysqlExe.UpdTotalSheets("questiontypelv1", DropDownListQType1.SelectedItem.ToString(), Type1ID_After, true, true);
                    MysqlExe.UpdTotalSheets("questiontypelv2", DropDownListQType2.SelectedItem.ToString(), Type2ID_After, true, true);
                    MysqlExe.UpdTotalSheets("account", HiddenField_DealName.Value, HiddenField_DealName_UID.Value, true, true);
                    MysqlExe.UpdTotalSheets("blocks", HiddenField_Blocks_Name.Value, HiddenField_BlocksID.Value, true, true);
                    labelAlterSuccess.Text = "子订单确认成功！点<code><a href=ShowOrder.aspx?orderid=" + labelOrderID_OrderID.InnerText + ">此</a></code>可刷新页面查看。";
                    divAlterSuccess.Visible = true;
                }
                else
                {
                    labelAlterDanger.Text = "更新父订单记录表失败，请联系管理员(FatherOrderUpdateError)。";
                    divAlterDanger.Visible = true;
                }
            }
            else
            {
                labelAlterDanger.Text = "更新子订单记录表失败，请联系管理员(SubOrderUpdateError)。";
                divAlterDanger.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text += "出错了，请联系管理员。(Submit_Confrim_Error)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
    /// <summary>
    /// 转让子订单确认按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void modal_SubOrder_Transfer_Click(object sender,EventArgs e)
    {
        ClearErrorMsg();
        //原始订单数据
        string DealName_Origin = HiddenField_DealName.Value;
        string DealID_Origin = HiddenField_DealName_UID.Value;
        //读取最新
        string DealName_Submit = DropDownList_TransferUser.SelectedItem.ToString();
        string DealID_Submit = DropDownList_TransferUser.SelectedValue;
        string DealKey = modal_Cancel_Deal.InnerText;
        if (DealID_Origin == DealID_Submit || DealName_Origin == DealName_Submit)
        {
            labelAlterDanger.Text = "订单转让失败，失败原因为：转让的对象与原来接单人一致。";
            divAlterDanger.Visible = true;            
        }
        else
        {
            MySqlCommand cmd = Conn.CreateCommand();
            //修改dealorder表
            string UpdateToDealorder = "update dealorder set DealUID=@DealUID,DealName=@DealName,StateCode=5 where DealKey=@DealKey";
            cmd.CommandText = UpdateToDealorder;
            cmd.Parameters.Add(new MySqlParameter("@DealName", DealName_Submit));
            cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
            cmd.Parameters.Add(new MySqlParameter("@DealUID", DealID_Submit));
            try
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                Conn.Open();
                int Feedback = cmd.ExecuteNonQuery();
                Conn.Close();
                if (Feedback > 0)
                {
                    //减少原接单人的接单数
                    MysqlExe.UpdTotalSheets("account", DealName_Origin, DealID_Origin, false, false);
                    //增加转让人的接单数
                    MysqlExe.UpdTotalSheets("account", DealName_Submit, DealID_Submit, false, true);
                    //更新备注
                    Cookies Read = new Cookies();
                    string[] QueryCookies = { "RealName" };
                    string RealName = Read.ReadCookies(QueryCookies)[0];
                    string backMsg = MysqlExe.UpdRemarks(DealKey, RealName, "原处理人" + DealName_Origin + "转让给" + DealName_Submit + "，处理人为" + RealName + "。");
                    labelAlterSuccess.Text = "转让成功！点<code><a href=ShowOrder.aspx?orderid=" + labelOrderID_OrderID.InnerText + ">此</a></code>可刷新页面查看。";
                    divAlterSuccess.Visible = true;
                    if (backMsg != null)
                    {
                        labelAlterDanger.Text += backMsg;
                        divAlterDanger.Visible = true;
                    }
                }
                else
                {
                    labelAlterDanger.Text = "更新子订单记录表失败，请联系管理员(SubOrderUpdateError)。";
                    divAlterDanger.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Conn.Close();
                labelAlterDanger.Text += "出错了，请联系管理员。(Submit_Confrim_Error)错误提示：" + ex.Message;
                divAlterDanger.Visible = true;
            }
        }
    }
    /// <summary>
    /// 取消订单确认按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void modal_SubOrder_Cancel_Click(object sender,EventArgs e)
    {
        ClearErrorMsg();
        //获取当前用户
        Cookies CookiesExe = new Cookies();
        string[] QueryLoginUser = { "RealName" };
        string CheckUserName = CookiesExe.ReadCookies(QueryLoginUser)[0];
        string DealKey = modal_Cancel_Deal.InnerText;
        string Description = modal_Cancel_DealFeedback.Value;
        string UpdateToDealOrder = null;
        MySqlCommand cmd = Conn.CreateCommand();
        //更新dealorder表
        UpdateToDealOrder = "update dealorder set DealFeedback=@DealFeedBack,StateCode=4,CheckUserName=@CheckUserName,DealTime=NOW() where DealKey=@DealKey";
        cmd.CommandText = UpdateToDealOrder;
        cmd.Parameters.Add(new MySqlParameter("@DealFeedBack", Description));
        cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
        cmd.Parameters.Add(new MySqlParameter("@CheckUserName", CheckUserName));
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            Conn.Open();
            int Feedback = cmd.ExecuteNonQuery();
            Conn.Close();
            if (Feedback > 0)
            {
                //更新records表
                string UpdateToRecords = "update records set StateCode=4 where OrderID=@OrderID";
                cmd.CommandText = UpdateToRecords;
                cmd.Parameters.Add(new MySqlParameter("@OrderID", labelOrderID_OrderID.InnerText));
                Conn.Open();
                Feedback = cmd.ExecuteNonQuery();
                Conn.Close();
                if (Feedback > 0)
                {
                    MysqlExe.UpdTotalSheets("account", HiddenField_DealName.Value, HiddenField_DealName_UID.Value, false, false);
                    labelAlterSuccess.Text = "子订单取消成功！点<code><a href=ShowOrder.aspx?orderid=" + labelOrderID_OrderID.InnerText + ">此</a></code>可刷新页面查看。";
                    divAlterSuccess.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text += "出错了，请联系管理员。(Submit_Confrim_Error)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
    /// <summary>
    /// 提交备注确认按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void modal_SubOrder_Remarks_Click(object sender,EventArgs e)
    {
        string DealKey = modal_Cancel_Deal.InnerText;
        string Remarks_Content = modal_Remarks_Content.Value;
        //更新备注
        Cookies Read = new Cookies();
        string[] QueryCookies = { "RealName" };
        string RealName = Read.ReadCookies(QueryCookies)[0];
        string backMsg = MysqlExe.UpdRemarks(DealKey, RealName, Remarks_Content);
        ClearErrorMsg();
        if (backMsg != null)
        {
            labelAlterDanger.Text += backMsg;
            divAlterDanger.Visible = true;
        }
        else
        {
            labelAlterSuccess.Text = "添加备注成功，点<code><a href=ShowOrder.aspx?orderid=" + labelOrderID_OrderID.InnerText + ">此</a></code>可刷新页面查看。";
            divAlterSuccess.Visible = true;
        }

    }
    /// <summary>
    /// 接收订单按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubOrderReceive_Click(object sender, EventArgs e)
    {
        ClearErrorMsg();
        MySqlConnection Conn = MysqlExe.ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        //获取接单人信息
        string[] QueryCookies = { "RealName", "uid" };
        Cookies Read = new Cookies();
        string[] result = Read.ReadCookies(QueryCookies);
        string RealName = result[0], ID = result[1];
        //更新dealorder表
        cmd.CommandText = "update dealorder set DealUID=@ID,DealName=@Name where DealKey=@DealKey";
        cmd.Parameters.Add(new MySqlParameter("@ID", ID));
        cmd.Parameters.Add(new MySqlParameter("@Name", RealName));
        cmd.Parameters.Add(new MySqlParameter("@DealKey", modal_SubOrder_Deal.InnerText));
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            int UpdFeedBack = cmd.ExecuteNonQuery();
            Conn.Close();
            if (UpdFeedBack > 0)
            {
                cmd.CommandText = "update records set StateCode=1,DealPeopleNum=1 where OrderID=@OrderID";
                cmd.Parameters.Add(new MySqlParameter("@OrderID", labelOrderID_OrderID.InnerText));
                Conn.Open();
                cmd.ExecuteNonQuery();
                Conn.Close();
                int feedback = MysqlExe.UpdTotalSheets("account", RealName, ID, false, true);
                if (feedback == 100)
                {
                    labelAlterSuccess.Text = "接单成功，点<code><a href=ShowOrder.aspx?orderid=" + labelOrderID_OrderID.InnerText + ">此</a></code>可刷新页面查看。";
                    divAlterSuccess.Visible = true;
                }
                else
                {
                    labelAlterDanger.Text = "出错了，请联系管理员。(ReceiveUpdDB_ERROR)，错误代码为：" + feedback.ToString();
                    divAlterDanger.Visible = true;
                }
            }
            else
            {
                labelAlterDanger.Text = "接单失败！";
                divAlterDanger.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text = "出错了，请联系管理员。(ReceiveUpdDB_ERROR)，错误提示为：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
}