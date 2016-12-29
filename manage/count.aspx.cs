using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

public partial class manage_count : System.Web.UI.Page
{
    static UsingMysql MysqlExe = new UsingMysql();
    MySqlConnection Conn = MysqlExe.ConnMode;
    string[] Mode = { "用户", "问题父分类", "问题子分类", "地区", "总体情况" };
    string[] DBName = { "totaluser", "totalordertype1", "totalordertype2", "totalblock", "totaldatabase" };
    string[] OriginalDBName = { "account", "questiontypelv1", "questiontypelv2", "blocks" };
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DDLBind_QType1();
            for (int year = 2016; year < 2020; year++)
            {
                ListItem item = new ListItem();
                item.Text = year.ToString();
                item.Value = year.ToString();
                DDL_ChooseYear.Items.Insert(year - 2016, item);
            } 
            for (int month = 1; month < 13; month++)
            {
                ListItem item = new ListItem();
                item.Text = month.ToString().PadLeft(2, '0');
                item.Value = item.Text;
                DDL_ChooseMonth.Items.Insert(month - 1, item);
            }
            DDL_ChooseYear.SelectedValue = DateTime.Now.ToString("yyyy");
            DDL_ChooseMonth.SelectedValue = DateTime.Now.ToString("MM").PadLeft(2, '0');
        }
    }
    /// <summary>
    /// 绑定问题父分类选择下拉菜单
    /// </summary>
    protected void DDLBind_QType1()
    {
        string[] QueryString = { "*", "questiontypelv1"};
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DDL_QType1.DataSource = dt.DefaultView;
        DDL_QType1.DataTextField = "Type1Name";
        DDL_QType1.DataValueField = "Type1ID";
        DDL_QType1.DataBind();
        ListItem itemNull = new ListItem();
        itemNull.Text = "---请选择---";
        itemNull.Value = null;
        DDL_QType1.Items.Insert(0, itemNull);
    }
    /// <summary>
    /// 初始化显示搜索条件、搜索按钮
    /// </summary>
    /// <param name="btnActiveNum"></param>
    protected void Init_PanelTop(int btnActiveNum)
    {
        label_QueryType.InnerText = Mode[btnActiveNum];
        HiddenField_ChooseType.Value = btnActiveNum.ToString();
        if (btnActiveNum == 2) DDL_QType1.Enabled = true;
        else DDL_QType1.Enabled = false;
        btnQuery.Enabled = true;
    }

    /// <summary>
    /// 按用户 按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UserTable_Click(object sender, EventArgs e)
    {
        Init_PanelTop(0);
    }
    /// <summary>
    /// 按问题父分类 按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void QType1Table_Click(object sender, EventArgs e)
    {
        Init_PanelTop(1);
    }
    /// <summary>
    /// 按问题子分类 按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void QType2Table_Click(object sender, EventArgs e)
    {
        Init_PanelTop(2);
    }
    protected void Write_To_TableResult(DataSet Result,string[] QueryColumn, string[] ShowColumn)
    {
        int Num_QueryColumn = QueryColumn.Length;   //列数
        DataTable dt_result = Result.Tables[0];
        Table ShowTable = this.ShowResultTable;
        TableRow tRow = new TableRow();
        //列头
        for (int i = 0; i < Num_QueryColumn; i++)
        {
            TableCell tCell = new TableCell();
            tCell.Text = ShowColumn[i];
            tRow.Cells.Add(tCell);       
        }
        ShowTable.Rows.Add(tRow);
        //填充内容
        foreach(DataRow dRow in dt_result.Rows)
        {
            tRow = new TableRow();
            for(int i = 0; i < Num_QueryColumn; i++)
            {
                TableCell tCell = new TableCell();
                tCell.Text = dRow[QueryColumn[i]].ToString();
                tRow.Cells.Add(tCell);               
            }
            ShowTable.Rows.Add(tRow);
        }
    }
    /// <summary>
    /// 按地区 按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BlockTable_Click(object sender, EventArgs e)
    {
        Init_PanelTop(3);
    }
    /// <summary>
    /// 查询按钮事件（进行查询工作，生成DS，调用写表程序）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        labelShowTable_Body.Visible = false;
        divAlterDanger.Visible = false;
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = "select * from " + DBName[Convert.ToInt32(HiddenField_ChooseType.Value)]+" as t,";
        cmd.CommandText += OriginalDBName[Convert.ToInt32(HiddenField_ChooseType.Value)] + " as o where t.Month=@Month";
        //与各统计表与初始原表联合
        switch (HiddenField_ChooseType.Value)
        {
            case "0":
                cmd.CommandText += " and t.ID=o.UID";
                break;
            case "1":
                cmd.CommandText += " and t.ID=o.Type1ID";
                break;
            case "2":
                cmd.CommandText += " and t.ID=o.Type2ID";
                break;
            case "3":
                cmd.CommandText += " and t.ID=o.BID";
                break;
        }
        if (HiddenField_ChooseType.Value == "2")
        {
            if(DDL_QType1.SelectedValue != "")
            {
                cmd.CommandText += " and t.TOTKey LIKE @TOTKey";
                cmd.Parameters.Add(new MySqlParameter("@TOTKey", DDL_QType1.SelectedValue + "%"));
            }
        }
        cmd.CommandText += " order by CAST(t.ID AS UNSIGNED)";
        cmd.Parameters.Add(new MySqlParameter("@Month", DDL_ChooseYear.SelectedItem.ToString() + DDL_ChooseMonth.SelectedItem.ToString()));
        MySqlDataAdapter da = new MySqlDataAdapter();
        da.SelectCommand = cmd;
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            DataSet ds = new DataSet();
            da.Fill(ds);
            Conn.Close();
            //组件可视化
            divShowTable.Visible = true;
            if (ds.Tables[0].Rows.Count == 0)      //没有行
            {
                labelShowTable_Body.Visible = true;
            }
            else
            {
                string[] TableColumnHeader = null;
                string[] TableColumnDBName = null;
                switch (HiddenField_ChooseType.Value)
                {
                    case "0":
                        TableColumnHeader = new string[] { "用户名", "创建订单数", "接单数", "解决单数" };
                        TableColumnDBName = new string[] { "RealName", "AddOrders", "Orders", "DealOrder" };
                        break;
                    case "1":
                        TableColumnHeader = new string[] { "分类名", "订单数", "已解决数" };
                        TableColumnDBName = new string[] { "Type1Name", "Orders", "DealOrder" };
                        break;
                    case "2":
                        TableColumnHeader = new string[] { "分类名", "订单数", "已解决数" };
                        TableColumnDBName = new string[] { "Type2Name", "Orders", "DealOrder" };
                        break;
                    case "3":
                        TableColumnHeader = new string[] { "地区", "订单数", "已解决数" };
                        TableColumnDBName = new string[] { "BlockName", "Orders", "DealOrder" };
                        break;
                }
                Write_To_TableResult(ds, TableColumnDBName, TableColumnHeader);
            }
            label_ResultPanel_Footer.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text = "读取数据库失败，请联系管理员。错误提示为：" + ex.Message;
            divAlterDanger.Visible = true;
        }
    }
}