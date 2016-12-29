using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.UI.HtmlControls;

public partial class index : System.Web.UI.Page
{
    static UsingMysql MysqlExe = new UsingMysql();
    MySqlConnection Conn = MysqlExe.ConnMode;
    static Cookies Read = new Cookies();
    static string[] QueryCookies = { "RealName", "uid" };
    static string[] ReturnFromCookies = Read.ReadCookies(QueryCookies);
    static string RealName = ReturnFromCookies[0], UID = ReturnFromCookies[1];
    DataSet ds_Type1 = new DataSet();
    DataSet ds_Type2 = new DataSet();
    DataSet ds_Block = new DataSet();
    protected void Page_Load(object sender, EventArgs e)
    {
        ds_Type1 = MysqlExe.GetTables_All("questiontypelv1");
        ds_Type2 = MysqlExe.GetTables_All("questiontypelv2");
        ds_Block = MysqlExe.GetTables_All("blocks");
        Init_CounterNum();
        if (Badge_Unfinished.Visible == true)
        {
            Init_ShowOrders_UnFinished();
        }
        else
        {
            label_UnfinishedIsNULL.Visible = true;
        }
        Init_ShowOrders_TodayFinished();
    }
    protected void ClearErrorMsg()
    {
        divAlterSuccess.Visible = false;
        divAlterDanger.Visible = false;
        labelAlterDanger.Text = null;
        labelAlterSuccess.Text = null;
    }
    /// <summary>
    /// 初始化当前订单数目显示
    /// </summary>
    protected void Init_CounterNum()
    {
        string Now_Month = DateTime.Now.ToString("yyyyMM");
        MySqlCommand cmd = Conn.CreateCommand();
        string Today = "select count(*) from dealorder where year(LogTime)=" + DateTime.Now.ToString("yyyy") + " and month(LogTime)=" + DateTime.Now.ToString("MM") + " and day(LogTime)=" + DateTime.Now.ToString("dd");
        string Month = "select count(*) from dealorder where year(LogTime)=" + DateTime.Now.ToString("yyyy") + " and month(LogTime)=" + DateTime.Now.ToString("MM");
        string UnFinished = "select count(*) from dealorder where (StateCode = 0 OR StateCode = 1 OR StateCode = 2 OR StateCode = 5)";
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            cmd.CommandText = Today;
            int Today_NUM = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = Month;
            int Month_NUM = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = UnFinished;
            int UnFinished_NUM = Convert.ToInt32(cmd.ExecuteScalar());
            Conn.Close();
            labelTodayOrder.Text = Today_NUM.ToString();
            labelMonthOrder.Text = Month_NUM.ToString();
            labelUnFinishedOrder.Text = UnFinished_NUM.ToString();
            if (UnFinished_NUM > 0)
            {
                Badge_Unfinished.InnerText = UnFinished_NUM.ToString();
                Badge_Unfinished.Visible = true;
            }

        }
        catch (Exception ex)
        {
            Conn.Close();
            labelAlterDanger.Text = "读取数据出错，请联系管理员。(Init_Counter_ERROR)错误提示为：" + ex.Message;
            divAlterDanger.Visible = true;
        }
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
    /// <summary>
    /// 初始化订单（未完成）部分
    /// </summary>
    /// <param name="Mode"></param>
    protected void Init_ShowOrders_UnFinished()
    {
        tableShowOrder_Unfinish.Visible = true;
        //表格相关
        TableRow tRow;     //定义数据表行
        TableCell tCell;   //定义数据表列
        string[] Titles = { "订单类型", "接单时间", "维修地址", "联系方式","接单人", "详细信息" };
        int numCells = Titles.Length;   //总列数     
        //数据库相关
        string Query_result = "select d.Type1ID,d.Type2ID,d.DealTime,r.BID,r.BlockRoom,r.QName,r.QTelephone,r.OrderID,d.DealName from dealorder as d,records as r where r.OrderID=d.DealOrderID and (d.StateCode=0 OR d.StateCode = 1 OR d.StateCode = 2 OR d.StateCode = 5) order by d.DealTime,CAST(d.DealID AS UNSIGNED) desc";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = Query_result;
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds_result = new DataSet();
            da.Fill(ds_result);
            Conn.Close();
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
                    tRow.Cells.Add(tCells[0]);//第一列
                    tCells[1].Text = dr["DealTime"].ToString();
                    tRow.Cells.Add(tCells[1]);//第二列
                    tCells[2].Text = Read_DB_Name("BID", "BlockName", dr["BID"].ToString(), ds_Block) + " - ";
                    tCells[2].Text += dr["BlockRoom"].ToString();
                    tRow.Cells.Add(tCells[2]);//第三列
                    tCells[3].Text = dr["QName"].ToString() + "," + dr["QTelephone"].ToString();
                    tRow.Cells.Add(tCells[3]);//第四列
                    tCells[4].Text = dr["DealName"].ToString();
                    tRow.Cells.Add(tCells[4]);
                    tCells[5].Text = "<a href=ShowOrder.aspx?orderid=" + dr["OrderID"].ToString() + "><span class='glyphicon glyphicon-th-list'></span></a>";
                    tRow.Cells.Add(tCells[5]);
                }
                tableShowOrder_Unfinish.Rows.Add(tRow);  //添加一行
            }
        }
        catch (Exception ex)                   //查询数据库失败
        {
            labelAlterDanger.Text = "数据表查询出错，请联系管理员。(Init_ShowOrdersERROR)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
            Conn.Close();
        }

    }
    protected void Init_ShowOrders_TodayFinished()
    {
        //表格相关
        TableRow tRow;     //定义数据表行
        TableCell tCell;   //定义数据表列
        string[] Titles = { "订单类型", "接单时间", "维修地址", "联系方式", "接单人", "详细信息" };
        int numCells = Titles.Length;   //总列数     
        //数据库相关
        string Today = DateTime.Now.ToString("dd");
        string Query_result = "select d.Type1ID,d.Type2ID,d.DealTime,r.BID,r.BlockRoom,r.QName,r.QTelephone,r.OrderID,d.DealName from dealorder as d,records as r where r.OrderID=d.DealOrderID and d.StateCode = 3 and day(DealTime) = " + Today + " order by d.DealTime,CAST(d.DealID AS UNSIGNED) desc";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = Query_result;
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds_result = new DataSet();
            da.Fill(ds_result);
            Conn.Close();
            //创建表格
            int numRows = ds_result.Tables[0].Rows.Count + 1;
            if(numRows == 1)
            {
                panel_TodayFinished_Body.InnerText = "今日还没有已完成的订单。";
            }
            else
            {
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
                        tRow.Cells.Add(tCells[0]);//第一列
                        tCells[1].Text = dr["DealTime"].ToString();
                        tRow.Cells.Add(tCells[1]);//第二列
                        tCells[2].Text = Read_DB_Name("BID", "BlockName", dr["BID"].ToString(), ds_Block) + " - ";
                        tCells[2].Text += dr["BlockRoom"].ToString();
                        tRow.Cells.Add(tCells[2]);//第三列
                        tCells[3].Text = dr["QName"].ToString() + "," + dr["QTelephone"].ToString();
                        tRow.Cells.Add(tCells[3]);//第四列
                        tCells[4].Text = dr["DealName"].ToString();
                        tRow.Cells.Add(tCells[4]);
                        tCells[5].Text = "<a href=ShowOrder.aspx?orderid=" + dr["OrderID"].ToString() + "><span class='glyphicon glyphicon-th-list'></span></a>";
                        tRow.Cells.Add(tCells[5]);
                    }
                    tableShowOrder_TodayFinished.Rows.Add(tRow);  //添加一行
                }
            }
        }
        catch (Exception ex)                   //查询数据库失败
        {
            labelAlterDanger.Text = "数据表查询出错，请联系管理员。(Init_ShowOrders_Todays_ERROR)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
            Conn.Close();
        }
    }

}