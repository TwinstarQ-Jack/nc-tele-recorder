using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class manage_questiontypes : System.Web.UI.Page
{
    static UsingMysql MysqlExe = new UsingMysql();
    static MySqlConnection Conn = MysqlExe.ConnMode;
    protected void Page_Load(object sender, EventArgs e)
    {
        string addtype = null;
        if (!IsPostBack)
        {
            if (HttpContext.Current.Request["addtype"] != null)
            {
                addtype = HttpContext.Current.Request["addtype"];
                switch (addtype)           //跳转
                {
                    case "1":
                        divAddType1.Visible = true;
                        divShowTypes.Visible = false;
                        break;
                    case "2":
                        divAddType2.Visible = true;
                        divShowTypes.Visible = false;
                        DDLBind_Type1Num();          //绑定DDLType1控件
                        break;
                    default:
                        Response.AddHeader("REFRESH", "0;URL=questiontypes.aspx");
                        break;
                }
            }
            else
            {
                int page = 1;
                if (HttpContext.Current.Request["page"] != null)
                {
                    page = Convert.ToInt32(HttpContext.Current.Request["page"].ToString());
                }
                ShowTypes(tableShowTypes, page);                     //展示现有表
            }
        }
    }
    /// <summary>
    /// 绑定到DDLType1Num
    /// </summary>
    protected void DDLBind_Type1Num()
    {
        string[] QueryString = { "*", "questiontypelv1" };
        DataSet ds = MysqlExe.getDataSet(Conn, QueryString);
        DataTable dt = ds.Tables[0];
        DropDownListType1Num.DataSource = dt.DefaultView;
        DropDownListType1Num.DataTextField = "Type1Name";
        DropDownListType1Num.DataValueField = "Type1ID";
        DropDownListType1Num.DataBind();
    }

    protected void PaginationShow(int NowPage, int SumRecords, string BID)
    {
        string BID_Para = "&addsub=1&BID=" + BID;
        if (BID == null)
        {
            BID_Para = null;
        }
        int dividedby = 20;   //默认分页条数
        int Pages_Num = SumRecords / dividedby + 1;    //总页数（最后一页导航）
        int list_Num = 5;   //当前最大显示分块
        int var_Page = 0;
        LiteralControl FirstPage = new LiteralControl(), LastPage = new LiteralControl();
        FirstPage.Text = "<li><a href=questiontypes.aspx?page=1" + BID_Para + ">&laquo;第一页</a></li>";
        LastPage.Text = "<li><a href=questiontypes.aspx?page=" + Pages_Num + BID_Para + ">最后一页&raquo;</a></li>";
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
                        ListPage.Text = "<li><a href=questiontypes.aspx?page=" + i + BID_Para + ">" + i + "</a></li>";
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
                        ListPage.Text = "<li><a href=questiontypes.aspx?page=" + var_Page + BID_Para + ">" + var_Page + "</a></li>";
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
    /// 显示所有分类（T1+T2）
    /// </summary>
    protected void ShowTypes(Table ShowTypesTable, int nowPage)
    {
        //获取父层数据
        string QS_Type1 = "select d2.Type2ID,d1.Type1Name,d2.Type2Name from questiontypelv1 as d1,questiontypelv2 as d2 where d1.Type1ID=d2.Type1ID order by cast(d2.Type1ID as UNSIGNED),cast(d2.Type2ID as UNSIGNED) limit " + (nowPage - 1) * 20 + ",20";
        DataSet ds = new DataSet();
        MySqlCommand cmd_Type1 = new MySqlCommand(QS_Type1, Conn);
        MySqlDataAdapter da = new MySqlDataAdapter();
        da.SelectCommand = cmd_Type1;
        //显示数据
        TableRow tRow = new TableRow();
        TableCell tCell = new TableCell();
        string[] Titles = { "子分类编号",  "父分类名称", "子分类名称" };
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            Conn.Open();
            da.Fill(ds);
            Conn.Close();
            //写入表格
            int numRows = ds.Tables[0].Rows.Count + 1;       //当期显示总行数
            int numCells = 3;               //列数
            for (int iRow = 0; iRow < numRows; iRow++)
            {
                tRow = new TableRow();
                if (iRow == 0)
                {
                    for (int jCol = 0; jCol < numCells; jCol++)         //表格首行
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
                    for (int jrow = 0; jrow < numCells; jrow++)
                    {
                        tCells[jrow].Text = dr[jrow].ToString();
                        tRow.Cells.Add(tCells[jrow]);
                    }
                }
                ShowTypesTable.Rows.Add(tRow);               //增加一行记录
            }
            //记录总数
            string ConnString = "select count(*) from questiontypelv2";
            Conn.Open();
            MySqlCommand cmd2 = new MySqlCommand(ConnString, Conn);
            DataSet ds2 = new DataSet();
            da.SelectCommand = cmd2;
            da.Fill(ds2);
            Conn.Close();
            int SumRecords = 0;
            if (ds2.Tables[0].Rows.Count > 0)
            {
                SumRecords = Convert.ToInt32(ds2.Tables[0].Rows[0][0].ToString());
            }
            if (SumRecords == 0) 
            {
                divAlterDanger.Visible = true;
                labelAlterDanger.Text = "当前还未有问题分类。";
            }
            else
            {
                //制作页码导航条
                PaginationShow(nowPage, SumRecords, null);
            }
        }
        catch (Exception ex)
        {
            labelAlterDanger.Text = "出现错误，请联系管理员。错误信息为：" + ex.Message;
            divAlterDanger.Visible = true;
        }
       
    }

    /// <summary>
    /// 显示当前分类按钮行为
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnShowTypes_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=questiontypes.aspx");
    }

    protected void btnAddType1_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=questiontypes.aspx?addtype=1");
    }

    protected void btnAddType2_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=questiontypes.aspx?addtype=2");
    }
    /// <summary>
    /// 增加父分类（T1）提交按钮，写入数据库
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmitType1_Click(object sender, EventArgs e)
    {
        divAlterDanger.Visible = false;
        divAlterSuccess.Visible = true;
        string serial = null;
        //检查输入
        string Type1Name = inputType1Name.Value;
        if(Type1Name == null || Type1Name == "")
        {
            labelAlterDanger.Text = "您还没填写表单，请确认填写无误。";
            divAlterDanger.Visible = true;
        }
        else
        {
            //获取数据库中最新记录的Type1ID
            string QueryType1LastestSerial = "select Type1ID from questiontypelv1 order by CAST(Type1ID as UNSIGNED) desc limit 0,1";
            DataSet ds_Serial = new DataSet();
            MySqlCommand cmd = new MySqlCommand(QueryType1LastestSerial, Conn);
            MySqlDataAdapter da = new MySqlDataAdapter();
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            da.SelectCommand = cmd;
            try
            {
                Conn.Open();
                da.Fill(ds_Serial);
                Conn.Close();
                if (ds_Serial.Tables[0].Rows.Count == 0)
                {
                    serial = "00001";
                }
                else
                {
                    serial = (Convert.ToInt32(ds_Serial.Tables[0].Rows[0][0].ToString()) + 1).ToString().PadLeft(5, '0');
                }
                //构造字符串，写入数据库
                string Insert_Type1 = "insert into questiontypelv1 (Type1ID,Type1Name) values(@Type1ID,@Type1Name)";
                MySqlCommand insertcmd = Conn.CreateCommand();
                insertcmd.CommandText = Insert_Type1;
                insertcmd.Parameters.Add(new MySqlParameter("@Type1ID", serial));
                insertcmd.Parameters.Add(new MySqlParameter("@Type1Name", Type1Name));
                Conn.Open();
                int result = insertcmd.ExecuteNonQuery();
                Conn.Close();
                if (result > 0)               //写入Type1表成功
                {
                    string Insert_TotalDB = "insert into totaldatabase (Name,Belongs) values(@Type1Name,'questiontypelv1')";
                    insertcmd.CommandText = Insert_TotalDB;
                    Conn.Open();
                    result = insertcmd.ExecuteNonQuery();
                    if(result > 0)            //写入totalDB表成功
                    {
                        labelAlterSuccess.Text = "添加记录<code>" + Type1Name + "</code>成功！";
                        divAlterSuccess.Visible = true;
                    }
                    else                     //写入totalDB表失败
                    {
                        labelAlterDanger.Text = "写入T1数据表成功，写入总统计表失败，请联系管理员。";
                        divAlterDanger.Visible = true;
                    }
                }
                else                          //写入Type1失败
                {
                    labelAlterDanger.Text = "写入T1数据表失败，请联系管理员。";
                    divAlterDanger.Visible = true;
                }
            }
            catch (Exception ex)
            {
                labelAlterDanger.Text = "出错了，请联系管理员。错误信息：" + ex.Message;
                divAlterDanger.Visible = true;
            }
        }
    }
    /// <summary>
    /// 增加子分类（T2）提交按钮，写入数据库
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmitType2_Click(object sender, EventArgs e)
    {
        divAlterDanger.Visible = false;
        divAlterSuccess.Visible = true;
        string serial = DropDownListType1Num.SelectedValue.ToString();
        string Type1ID = serial;
        //检查输入
        string Type2Name = inputType2Name.Value;
        if (Type2Name == null || Type2Name == "")
        {
            labelAlterDanger.Text = "您还没填写表单，请确认填写无误。";
            divAlterDanger.Visible = true;
        }
        else
        {
            //获取数据库中最新记录的Type1ID
            string QueryType2LastestSerial = "select Type2ID from questiontypelv2 where Type1ID=@Type1ID order by CAST(Type2ID as UNSIGNED) desc limit 0,1";
            DataSet ds_Serial = new DataSet();
            MySqlCommand cmd = new MySqlCommand(QueryType2LastestSerial, Conn);
            cmd.Parameters.Add(new MySqlParameter("@Type1ID", Type1ID));
            MySqlDataAdapter da = new MySqlDataAdapter();
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            da.SelectCommand = cmd;
            try
            {
                Conn.Open();
                da.Fill(ds_Serial);
                Conn.Close();
                if (ds_Serial.Tables[0].Rows.Count == 0)
                {
                    serial += "000001";
                }
                else
                {
                    serial = (Convert.ToInt32(ds_Serial.Tables[0].Rows[0][0].ToString()) + 1).ToString().PadLeft(11, '0');
                }
                //构造字符串，写入数据库
                string Insert_Type1 = "insert into questiontypelv2 (Type2ID,Type2Name,Type1ID) values(@Type2ID,@Type2Name,@Type1ID)";
                MySqlCommand insertcmd = Conn.CreateCommand();
                insertcmd.CommandText = Insert_Type1;
                insertcmd.Parameters.Add(new MySqlParameter("@Type2ID", serial));
                insertcmd.Parameters.Add(new MySqlParameter("@Type2Name", Type2Name));
                insertcmd.Parameters.Add(new MySqlParameter("@Type1ID", Type1ID));
                Conn.Open();
                int result = insertcmd.ExecuteNonQuery();
                Conn.Close();
                if (result > 0)               //写入Type2表成功
                {
                    string Insert_TotalDB = "insert into totaldatabase (Name,Belongs) values(@Type2Name,'questiontypelv2')";
                    insertcmd.CommandText = Insert_TotalDB;
                    Conn.Open();
                    result = insertcmd.ExecuteNonQuery();
                    if (result > 0)            //写入totalDB表成功
                    {
                        labelAlterSuccess.Text = "添加记录<code>" + Type2Name + "</code>成功！";
                        divAlterSuccess.Visible = true;
                    }
                    else                     //写入totalDB表失败
                    {
                        labelAlterDanger.Text = "写入T2数据表成功，写入总统计表失败，请联系管理员。";
                        divAlterDanger.Visible = true;
                    }
                }
                else                          //写入Type2失败
                {
                    labelAlterDanger.Text = "写入T2数据表失败，请联系管理员。";
                    divAlterDanger.Visible = true;
                }
            }
            catch (Exception ex)
            {
                labelAlterDanger.Text = "出错了，请联系管理员。错误信息：" + ex.Message;
                divAlterDanger.Visible = true;
            }
        }
    }
}