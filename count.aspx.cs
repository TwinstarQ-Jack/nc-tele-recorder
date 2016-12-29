using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.UI.HtmlControls;

public partial class count : System.Web.UI.Page
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
        Init_CounterNum();         //初始化数据显示
        if (!IsPostBack)
        {
            ds_Type1 = MysqlExe.GetTables_All("questiontypelv1");
            ds_Type2 = MysqlExe.GetTables_All("questiontypelv2");
            ds_Block = MysqlExe.GetTables_All("blocks");           
            string page = null;
            if (HttpContext.Current.Request["page"] != null)
            {
                page = HttpContext.Current.Request["page"].ToString();
            }
            if (page != null)
            {
                if (HttpContext.Current.Request["month"] != null) Init_ShowOrders(tableShowOrder_Month, "Month", page);
                if (HttpContext.Current.Request["history"] != null) Init_ShowOrders(tableShowOrder_History, "History", page);
            }
            else
            {
                //控制是否显示未解决订单
                if (Badge_Unfinished.Visible == true)
                {
                    Init_ShowOrders("Unfinish");
                }
                else
                {
                    label_UnfinishedIsNULL.Visible = true;
                }
            }
        }
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
    /// 初始化当前订单数目显示
    /// </summary>
    protected void Init_CounterNum()
    {
        string Now_Month = DateTime.Now.ToString("yyyyMM");
        string QueryString_Month = "select t.AddOrders,t.DealOrder,t.Orders,d.Orders from totaluser as t,totaldatabase as d where t.Month ='" + Now_Month + "' and t.ID='" + UID + "' and d.Name='" + RealName + "'";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = QueryString_Month;
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            DataSet ds = new DataSet();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Conn.Close();
            if (ds.Tables[0].Rows.Count != 0)          //空数据表
            {
                DataRow dr = ds.Tables[0].Rows[0];
                labelAddOrderNum.Text = dr["AddOrders"].ToString();
                labelDealOrderNum.Text = dr["DealOrder"].ToString();
                labelTotalOrderNum.Text = dr[3].ToString();
                int unfinish = Convert.ToInt32(dr[2].ToString()) - Convert.ToInt32(dr["DealOrder"].ToString());
                if (unfinish > 0)
                {
                    Badge_Unfinished.InnerText = unfinish.ToString();
                    Badge_Unfinished.Visible = true;
                    BadgeAtDeal_Unfinished.InnerText = "未处理：" + unfinish.ToString();
                    BadgeAtDeal_Unfinished.Visible = true;
                }
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
        for(int i = 0; i < Rows; i++)
        {
            if (dt.Rows[i][Query_col_name].ToString() == QueryID) return dt.Rows[i][Result_col_name].ToString();
        }
        return null;
    }
    protected void PaginationShow(HtmlGenericControl PaginationID, int NowPage, int SumRecords, int dividedby, string Type)
    {
        string ID_Para = "&" + Type + "=" + Type;      //构造当前制作的页码导航条对应的网页参数
        int Pages_Num = SumRecords / dividedby + 1;    //总页数（最后一页导航）
        int list_Num = 5;   //当前最大显示分块
        int var_Page = 0;
        LiteralControl FirstPage = new LiteralControl(), LastPage = new LiteralControl();
        FirstPage.Text = "<li><a href=count.aspx?page=1" + ID_Para + ">&laquo;第一页</a></li>";
        LastPage.Text = "<li><a href=count.aspx?page=" + Pages_Num + ID_Para + ">最后一页&raquo;</a></li>";
        if (Pages_Num <= list_Num)
        {
            int i = 1; //Page-1
            if (NowPage == 1)
            {
                FirstPage.Text = "<li class=disabled><a>&laquo;第一页</a></li>";
            }
            PaginationID.Controls.Add(FirstPage);  //添加到ul中
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
                        ListPage.Text = "<li><a href=count.aspx?page=" + i + ID_Para + ">" + i + "</a></li>";
                    }
                    PaginationID.Controls.Add(ListPage);
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
                        ListPage.Text = "<li><a href=count.aspx?page=" + var_Page + ID_Para + ">" + var_Page + "</a></li>";
                    }
                    PaginationID.Controls.Add(ListPage);
                }
            }
            if (NowPage == Pages_Num)
            {
                LastPage.Text = "<li class=disabled><a>最后一页&raquo;</a></li>";
            }
            PaginationID.Controls.Add(LastPage);
        }
    }

    /// <summary>
    /// 初始化订单（未完成）部分
    /// </summary>
    /// <param name="Mode"></param>
    protected void Init_ShowOrders(string Mode)
    {
        tableShowOrder_Unfinish.Visible = true;
        //表格相关
        TableRow tRow;     //定义数据表行
        TableCell tCell;   //定义数据表列
        string[] Titles = { "订单类型", "接单时间", "维修地址", "联系方式", "详细信息" };
        int numCells = 5;   //总列数     
        //数据库相关
        string Query_result = "select d.Type1ID,d.Type2ID,d.DealTime,r.BID,r.BlockRoom,r.QName,r.QTelephone,r.OrderID from dealorder as d,records as r where r.OrderID=d.DealOrderID and DealUID=@UID and (d.StateCode=0 OR d.StateCode = 1 OR d.StateCode = 2) order by d.DealTime,CAST(d.DealID AS UNSIGNED) desc";
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = Query_result;
        cmd.Parameters.Add(new MySqlParameter("@UID", UID));
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
                    tCells[4].Text = "<a href=ShowOrder.aspx?orderid="+dr["OrderID"].ToString()+ "><span class='glyphicon glyphicon-th-list'></span></a>";
                    tRow.Cells.Add(tCells[4]);
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
    /// <summary>
    /// 初始化订单（已接手、历史订单）部分
    /// </summary>
    /// <param name="Mode"></param>
    /// <param name="page"></param>
    protected void Init_ShowOrders(Table ShowTable, string Mode, string page)
    {
        List_Of_Unfinished.Visible = false;             //关闭未完成列表展示
        //表格相关
        TableRow tRow;
        TableCell tCell;
        string[] Titles = { "订单类型", "接单时间", "维修地址", "联系方式", "详细信息" };
        int numCells = 5;   //总列数  
        int Lists_PerPage = 10;   //每页展示数量 
        int pages = Convert.ToInt32(page);
        //数据库相关
        string where_paras = null;
        switch (Mode)
        {
            case "Month":where_paras = " and month(d.DealTime)=" + DateTime.Now.ToString("MM") + " and d.DealUID=@UID";
                List_Of_Month.Visible = true;
                break;
            case "History":where_paras = "and d.StateCode = 3 and d.DealUID=@UID";
                List_Of_History.Visible = true;
                break;
        }
        string Query_result = "select d.Type1ID,d.Type2ID,d.DealTime,r.BID,r.BlockRoom,r.QName,r.QTelephone,r.OrderID from dealorder as d,records as r where r.OrderID=d.DealOrderID " + where_paras + " order by d.DealTime desc,CAST(d.DealID AS UNSIGNED) desc limit " + (pages - 1) * Lists_PerPage + "," + Lists_PerPage;
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = Query_result;
        cmd.Parameters.Add(new MySqlParameter("@UID", UID));
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
                    tCells[4].Text = "<a href=ShowOrder.aspx?orderid=" + dr["OrderID"].ToString() + "><span class='glyphicon glyphicon-th-list'></span></a>";
                    tRow.Cells.Add(tCells[4]);
                }
                ShowTable.Rows.Add(tRow);  //添加一行
            }
            //创建页码
            string QueryItems = "select count(*) from dealorder as d,records as r where r.OrderID=d.DealOrderID " + where_paras;
            cmd.CommandText = QueryItems;
            Conn.Open();
            int Sum_Of_DBCounters = Convert.ToInt32(cmd.ExecuteScalar());
            Conn.Close();
            switch (Mode)
            {
                case "Month":PaginationShow(PaginationPage_Month, pages, Sum_Of_DBCounters, Lists_PerPage, Mode);
                    label_Month.InnerText = Sum_Of_DBCounters.ToString();
                    break;
                case "History":PaginationShow(PaginationPage_History, pages, Sum_Of_DBCounters, Lists_PerPage, Mode);
                    label_History.InnerText = Sum_Of_DBCounters.ToString();
                    break;
            }
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text = "数据表查询出错，请联系管理员。(Init_ShowOrdersERROR)错误提示：" + ex.Message;
            divAlterDanger.Visible = true;
            Conn.Close();
        }
    }
    /// <summary>
    /// 功能区“未解决订单”按钮功能事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOrderUnfinished_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=count.aspx");
    }
    /// <summary>
    /// 功能区“当月已接订单”按钮功能事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOrderThisMonth_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=count.aspx?month=month&page=1");
    }
    /// <summary>
    /// 功能区“以往处理订单”按钮功能事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOrderHistory_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=count.aspx?history=history&page=1");
    }
}