using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MySql.Data.MySqlClient;

public partial class query : System.Web.UI.Page
{
    static UsingMysql MysqlExe = new UsingMysql();
    MySqlConnection Conn = MysqlExe.ConnMode;
    DataSet ds_Type1 = new DataSet();
    DataSet ds_Type2 = new DataSet();
    DataSet ds_Block = new DataSet();

    protected void Page_Load(object sender, EventArgs e)
    {
        ds_Type1 = MysqlExe.GetTables_All("questiontypelv1");
        ds_Type2 = MysqlExe.GetTables_All("questiontypelv2");
        ds_Block = MysqlExe.GetTables_All("blocks");
        if (!IsPostBack)
        {
            DDLBind_QType1();
            DDLBind_Deal();
            DDLBind_BlockID();
            //初始化DDLType2
            ListItem itemNull = new ListItem();
            itemNull.Text = "";
            itemNull.Value = null;
            DDL_Search_Type2.Items.Insert(0, itemNull);
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
        DDL_Search_Type1.DataSource = dt.DefaultView;
        DDL_Search_Type1.DataTextField = "Type1Name";
        DDL_Search_Type1.DataValueField = "Type1ID";
        DDL_Search_Type1.DataBind();
        ListItem itemNull = new ListItem();
        itemNull.Text = "";
        itemNull.Value = null;
        DDL_Search_Type1.Items.Insert(0, itemNull);
    }
    /// <summary>
    /// QType2的下拉菜单数据绑定（通过异步处理绑定Qtype1的变化
    /// </summary>
    protected void DDLBind_QType1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_Search_Type2.Items.Clear();
        string Type1ID = DDL_Search_Type1.SelectedValue;
        if (Type1ID == "")
        {
            ListItem itemNull = new ListItem();
            itemNull.Text = "";
            itemNull.Value = null;
            DDL_Search_Type2.Items.Insert(0, itemNull);
        }
        else
        {
            string[] QueryString = { "*", "questiontypelv2", "Type1ID", Type1ID };
            DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
            DataTable dt = ds.Tables[0];
            DDL_Search_Type2.DataSource = dt.DefaultView;
            DDL_Search_Type2.DataTextField = "Type2Name";
            DDL_Search_Type2.DataValueField = "Type2ID";
            DDL_Search_Type2.DataBind();
        }

    }
    /// <summary>
    /// Deal处理人的下拉菜单数据绑定
    /// </summary>
    protected void DDLBind_Deal()
    {
        string[] QueryString = { "UID,RealName", "account" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DDL_Search_Deal.DataSource = dt.DefaultView;
        DDL_Search_Deal.DataTextField = "RealName";
        DDL_Search_Deal.DataValueField = "UID";
        DDL_Search_Deal.DataBind();
        ListItem itemNull = new ListItem();
        itemNull.Text = "";
        itemNull.Value = null;
        DDL_Search_Deal.Items.Insert(0, itemNull);
    }
    /// <summary>
    /// 用户地区的下拉菜单数据绑定
    /// </summary>
    protected void DDLBind_BlockID()
    {
        string[] QueryString = { "*", "blocks" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DDL_Search_Block.DataSource = dt.DefaultView;
        DDL_Search_Block.DataTextField = "BlockName";
        DDL_Search_Block.DataValueField = "BID";
        DDL_Search_Block.DataBind();
        ListItem itemNull = new ListItem();
        itemNull.Text = "";
        itemNull.Value = null;
        DDL_Search_Block.Items.Insert(0, itemNull);
    }
    /// <summary>
    /// 遍历datatable，返回需要的列的对应值
    /// </summary>
    /// <param name="Query_col_name">寻找列的列名</param>
    /// <param name="Result_col_name">结果列的列名</param>
    /// <param name="QueryID">寻找列的参数</param>
    /// <param name="ds">寻找的目标数据集</param>
    /// <returns>结果列的内容</returns>
    protected string Read_DB_Name(string Query_col_name, string Result_col_name, string QueryID, DataSet ds)
    {
        DataTable dt = ds.Tables[0];
        int Rows = dt.Rows.Count;
        for (int i = 0; i < Rows; i++)
        {
            if (dt.Rows[i][Query_col_name].ToString() == QueryID) return dt.Rows[i][Result_col_name].ToString();
        }
        return null;
    }
    protected string Read_State_Name(string StateCode)
    {
        string[] StateName = { "未处理", "正在处理", "需要跟进", "已解决", "已取消", "已转让" };
        return StateName[Convert.ToInt32(StateCode)];
    }
    /// <summary>
    /// 初始化搜索结果表格
    /// </summary>
    /// <param name="Mode"></param>
    protected void Init_SearchResultTable(DataSet ds_result)
    {
        table_Update_ShowResult.Visible = true;
        //表格相关
        TableRow tRow;     //定义数据表行
        TableCell tCell;   //定义数据表列
        string[] Titles = { "订单类型", "处理人", "接单时间", "维修地址", "联系方式", "状态", "详细信息" };
        int numCells = 7;   //总列数     
        //创建表格
        int numRows = ds_result.Tables[0].Rows.Count + 1;
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
                DataRow dr = ds_result.Tables[0].Rows[iRow - 1];
                TableCell[] tCells = new TableCell[numCells];
                for (int i = 0; i < numCells; i++) tCells[i] = new TableCell();
                tCells[0].Text = Read_DB_Name("Type1ID", "Type1Name", dr["Type1ID"].ToString(), ds_Type1) + " - ";
                tCells[0].Text += Read_DB_Name("Type2ID", "Type2Name", dr["Type2ID"].ToString(), ds_Type2);
                tCells[1].Text = dr["DealName"].ToString();
                tCells[2].Text = dr["DealTime"].ToString();
                tCells[3].Text = Read_DB_Name("BID", "BlockName", dr["BID"].ToString(), ds_Block) + " - ";
                tCells[3].Text += dr["BlockRoom"].ToString();
                tCells[4].Text = dr["QName"].ToString() + "," + dr["QTelephone"].ToString();
                tCells[5].Text = Read_State_Name(dr["StateCode"].ToString());
                tCells[6].Text = "<a href=ShowOrder.aspx?orderid=" + dr["OrderID"].ToString() + "><span class='glyphicon glyphicon-th-list'></span></a>";
                for (int i = 0; i < numCells; i++) tRow.Cells.Add(tCells[i]);
            }
            table_Update_ShowResult.Rows.Add(tRow);  //添加一行
        }
    }

    /// <summary>
    /// 搜索按钮行为，异步更新到搜索结果框内
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        divAlterDanger.Visible = false;
        string Day_Begin = input_SearchDay_Begin.Value, Day_End = input_SearchDay_End.Value;
        //检查查询日期输入
        if (Day_Begin == "" || Day_End == "")
        {
            //空
            labelAlterDanger.Text = "日期尚未填写完整，请检查。";
            divAlterDanger.Visible = true;
        }
        else
        {
            int DateTimeValid = DateTime.Compare(Convert.ToDateTime(Day_Begin), Convert.ToDateTime(Day_End));
            if (DateTimeValid > 0)
            {
                //无效
                labelAlterDanger.Text = "日期填写错误，请检查日期范围的有效性。";
                divAlterDanger.Visible = true;
            }
            else       //日期有效
            {
                Day_Begin += " 00:00:00";
                Day_End += " 23:59:59";
                string OrderName = input_Search_Name.Value, OrderTele = input_Search_Tele.Value;
                string BID = DDL_Search_Block.SelectedValue, BlockRoom = input_Search_BlockRoom.Value;
                string QType1 = DDL_Search_Type1.SelectedValue, QType2 = DDL_Search_Type2.SelectedValue;
                string DealName = DDL_Search_Deal.SelectedItem.ToString();
                string AddedString = null;// 可选择填写的附加条件SQL语句
                MySqlCommand cmd = Conn.CreateCommand();
                //增加附加语句
                if (OrderName != "")
                {
                    AddedString += " and r.QName=@QName";
                    cmd.Parameters.Add(new MySqlParameter("@QName", OrderName));
                }
                if (OrderTele != "")
                {
                    AddedString += " and r.QTelephone=@QTele";
                    cmd.Parameters.Add(new MySqlParameter("@QTele", OrderTele));
                }
                if (QType1 != "")
                {
                    AddedString += " and d.Type2ID=@QType2";
                    cmd.Parameters.Add(new MySqlParameter("@QType2", QType2));
                }
                if (DealName != "")
                {
                    AddedString += " and d.DealName=@DName";
                    cmd.Parameters.Add(new MySqlParameter("@DName", DealName));
                }
                if (BID != "")
                {
                    AddedString += " and r.BID=@BID";
                    cmd.Parameters.Add(new MySqlParameter("@BID", BID));
                }
                if (BlockRoom != "")
                {
                    AddedString += " and r.BlockRoom=@BlockRoom";
                    cmd.Parameters.Add(new MySqlParameter("@BlockRoom", BlockRoom));
                }
                //正文查询语句
                string Query_result = "select d.Type1ID,d.Type2ID,d.DealTime,r.BID,r.BlockRoom,r.QName,r.QTelephone,r.OrderID,d.DealName,d.StateCode from dealorder as d,records as r where r.OrderID=d.DealOrderID and (d.DealTime Between @DayBegin And @DayEnd)" + AddedString + " order by d.DealTime desc,CAST(d.DealID AS UNSIGNED) desc";
                cmd.CommandText = Query_result;
                cmd.Parameters.Add(new MySqlParameter("@DayBegin", Day_Begin));
                cmd.Parameters.Add(new MySqlParameter("@DayEnd", Day_End));
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                try
                {
                    //写入ds
                    Conn.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    Conn.Close();
                    //制作表格
                    label_Update_DBResult.InnerText = ds.Tables[0].Rows.Count.ToString();
                    Init_SearchResultTable(ds);
                }
                catch (Exception ex)
                {
                    labelAlterDanger.Text = "读取数据库失败，请联系管理员。错误提示为：" + ex.Message;
                    divAlterDanger.Visible = true;
                }
            }

        }
    }
}