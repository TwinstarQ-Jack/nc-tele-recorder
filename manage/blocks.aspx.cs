using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class manage_blocks : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        divAlterSuccess.Visible = false;
        divAlterDanger.Visible = false;
        if (!IsPostBack)
        {
            if (HttpContext.Current.Request["batchadd"] != null)
            {
                if (HttpContext.Current.Request["batchadd"] == "1")
                {   //进入批量导入模式
                    BatchAddBlock.Visible = true;
                    ReadList.Visible = false;
                    AddBlock.Visible = false;
                }
            }
            else
            {  //正常模式
                int page = 1;
                if (HttpContext.Current.Request["page"] != null)
                {
                    page = Convert.ToInt32(HttpContext.Current.Request["page"].ToString());
                }
                InitMode_ViewOfBlocks(tableShowBlocks, page);
            }
        }
    }
    protected void PaginationShow(int NowPage, int SumRecords, string BID)
    {
        string BID_Para = "&addsub=1&BID=" + BID;
        if (BID == null)
        {
            BID_Para = null;
        }
        int dividedby = 10;   //默认分页条数
        int Pages_Num = SumRecords / dividedby + 1;    //总页数（最后一页导航）
        int list_Num = 5;   //当前最大显示分块
        int var_Page = 0;
        LiteralControl FirstPage = new LiteralControl(), LastPage = new LiteralControl();
        FirstPage.Text = "<li><a href=blocks.aspx?page=1" + BID_Para + ">&laquo;第一页</a></li>";
        LastPage.Text = "<li><a href=blocks.aspx?page=" + Pages_Num + BID_Para + ">最后一页&raquo;</a></li>";
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
                        ListPage.Text = "<li><a href=blocks.aspx?page=" + i + BID_Para + ">" + i + "</a></li>";
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
                        ListPage.Text = "<li><a href=blocks.aspx?page=" + var_Page + BID_Para + ">" + var_Page + "</a></li>";
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
    /// 初始化显示表格模块
    /// </summary>
    /// <param name="ShowUser"></param>
    /// <param name="nowPage"></param>
    protected void InitMode_ViewOfBlocks(Table ShowBlock,int nowPage)
    {
        TableRow tRow = new TableRow();
        TableCell tCell = new TableCell();
        string[] Titles = { "地区编号", "地区名称" };
        //数据库
        UsingMysql MysqlExe = new UsingMysql();
        MySqlConnection Conn = MysqlExe.ConnMode;
        string ConnString = "select * from blocks limit " + (nowPage - 1) * 10 + ",10";
        MySqlCommand cmd = new MySqlCommand(ConnString, Conn);
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
            //写入表格
            int numRows = ds.Tables[0].Rows.Count + 1;       //当期显示总行数
            int numCells = 2;               //列数
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
                    TableCell[] tCells = new TableCell[2];
                    for (int i = 0; i < 2; i++) tCells[i] = new TableCell();
                    for (int jrow = 0; jrow < 2; jrow++)       
                    {
                        tCells[jrow].Text = dr[jrow].ToString();
                        tRow.Cells.Add(tCells[jrow]);
                    }
                }
                ShowBlock.Rows.Add(tRow);               //增加一行记录
            }
            //记录总数
            ConnString = "select count(*) from blocks";
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
            else
            {
                divAlterDanger.Visible = true;
                labelAlterDanger.Text = "当前还未有区域。";
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
    /// 批量导入按钮行为
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBatchAddBlock_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=blocks.aspx?batchadd=1");
    }
    /// <summary>
    /// 添加单个地区行为
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddBlock_Click(object sender, EventArgs e)
    {
        Response.AddHeader("REFRESH", "0;URL=blocks.aspx");
    }
    /// <summary>
    /// 添加区域按钮操作
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string BID = inputBID.Value, BlockName = inputBlockName.Value;
        //判断非空
        if(BID == "" || BlockName == "")
        {
            labelAlterDanger.Text = "输入有误，请检查您的输入！";
            divAlterDanger.Visible = true;
        }
        else
        {
            UsingMysql MysqlExe = new UsingMysql();
            MySqlConnection Conn = MysqlExe.ConnMode;
            string select_for_repeated = "select count(*) from blocks where BID=@BID OR BlockName=@BlockName";
            string insert_into_blocks = "insert into blocks (BID,BlockName) values (@BID,@BlockName)";
            string insert_into_totaldb = "insert into totaldatabase (Name,Belongs) values(@BlockName,'blocks')";            
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            try       //数据库操作
            {
                //查询是否有重复记录
                MySqlCommand cmd_selectforrepeated = Conn.CreateCommand();
                cmd_selectforrepeated.CommandText = select_for_repeated;
                cmd_selectforrepeated.Parameters.Add(new MySqlParameter("@BID", BID));
                cmd_selectforrepeated.Parameters.Add(new MySqlParameter("@BlockName", BlockName));
                Conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd_selectforrepeated;
                da.Fill(ds);
                Conn.Close();
                int RepeatNum = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                if(RepeatNum == 0)         //无重复记录
                {
                    //插入blocks语句
                    MySqlCommand cmd_insertblocks = Conn.CreateCommand();
                    cmd_insertblocks.CommandText = insert_into_blocks;
                    cmd_insertblocks.Parameters.Add(new MySqlParameter("@BID", BID));
                    cmd_insertblocks.Parameters.Add(new MySqlParameter("@BlockName", BlockName));
                    Conn.Open();
                    int Result_Insert_Blocks = cmd_insertblocks.ExecuteNonQuery();
                    Conn.Close();
                    if (Result_Insert_Blocks > 0)        //成功插入Blocks表
                    {
                        MySqlCommand cmd_inserttotaldb = Conn.CreateCommand();
                        cmd_inserttotaldb.CommandText = insert_into_totaldb;
                        cmd_inserttotaldb.Parameters.Add(new MySqlParameter("@BlockName", BlockName));
                        Conn.Open();
                        int Result_Insert_TotalDB = cmd_inserttotaldb.ExecuteNonQuery();
                        Conn.Close();
                        if (Result_Insert_TotalDB > 0)  //成功插入总统计记录表
                        {
                            labelAlterSuccess.Text = "操作成功，加入了记录<code>" + BlockName + "</code>，编号为<code>" + BID.ToString() + "</code>。";
                            divAlterSuccess.Visible = true;
                            InitMode_ViewOfBlocks(tableShowBlocks, 1);
                        }
                        else
                        {
                            labelAlterDanger.Text = "无法访问总统计记录表，请联系管理员处理。";
                            divAlterDanger.Visible = true;
                        }
                    }
                    else                                  //失败插入Blocks表
                    {
                        labelAlterDanger.Text = "无法访问分区记录表，请联系管理员处理。";
                        divAlterDanger.Visible = true;
                    }
                }
                else                       //有重复记录
                {
                    labelAlterDanger.Text = "您输入的编号或者名称已经存在，请检查后再添加。";
                    divAlterDanger.Visible = true;
                }
            }
            catch (Exception ex)
            {
                labelAlterDanger.Text += "出错了，请联系管理员。错误提示：" + ex.Message;
                divAlterDanger.Visible = true;
            }
        }
    }
    /// <summary>
    /// 上传模块
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void XlsFileUploadSubmit_Click(object sender, EventArgs e)
    {
        divAlterDanger.Visible = false;
        divAlterSuccess.Visible = false;
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
                if (FileSize > 2.0)       //判断文件大小
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
                    XlsWriteToDataBase(SaveFilePath, RealName);
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
    protected int XlsWriteToDataBase(string filePath, string DealName)
    {
        try
        {
            string BID = null, BlockName = null;     //初始化输入参数
            string Repeat_Fail = null, Import_ToDB_Fail = null;                   //初始化导入失败名单
            int Fail_Counter = 0, Success_Counter = 0, Fail_ToDB_Counter = 0;   //初始化计数器
            //Excel表操作(ACE.OleDb.12.0版本可防止数字变为科学计数法）
            string ExcelConnStr = "Provider=Microsoft.ACE.OleDb.12.0;" + "Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
            OleDbConnection ExcelConn = new OleDbConnection(ExcelConnStr);
            ExcelConn.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter ODda = new OleDbDataAdapter("select * from [Sheet1$]", ExcelConn);
            ODda.Fill(ds);
            ExcelConn.Close();     //关闭OleDb连接
            DataTable dt = ds.Tables[0];
            //数据库参数初始化
            UsingMysql MysqlExe = new UsingMysql();
            MySqlConnection Conn = MysqlExe.ConnMode;
            foreach (DataRow dr in dt.Rows)              //数据库操作（读取比较、存入数据库）
            {
                BID = dr["地区编号"].ToString();
                BlockName = dr["地区名称"].ToString();
                string select_for_repeated = "select count(*) from blocks where BID=@BID OR BlockName=@BlockName";
                MySqlCommand cmd = Conn.CreateCommand();
                cmd.CommandText = select_for_repeated;
                cmd.Parameters.Add(new MySqlParameter("@BID", BID));
                cmd.Parameters.Add(new MySqlParameter("@BlockName", BlockName));
                Conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter();
                DataSet ds_CheckRepeated = new DataSet();
                da.SelectCommand = cmd;
                da.Fill(ds_CheckRepeated);
                Conn.Close();
                int RepeatNum = Convert.ToInt32(ds_CheckRepeated.Tables[0].Rows[0][0].ToString());
                if (RepeatNum > 0)         //重复项
                {
                    if (Fail_Counter > 0)
                    {
                        Repeat_Fail += ",";
                    }
                    Repeat_Fail += BlockName;
                    Fail_Counter++;
                }
                else                       //非重复项
                {
                    string QS_Import_To_blocks = "insert into blocks (BID,BlockName) values(@BID,@BlockName)";
                    cmd.CommandText = QS_Import_To_blocks;
                    //cmd.Parameters.Add(new MySqlParameter("@BID", BID));
                    //cmd.Parameters.Add(new MySqlParameter("@BlockName", BlockName));
                    Conn.Open();
                    int Import_Blocks = cmd.ExecuteNonQuery();
                    Conn.Close();
                    if (Import_Blocks > 0)          //插入地区表成功
                    {
                        string QS_Import_To_TotalDB = "insert into totaldatabase (Name,Belongs) values (@BlockName,'blocks')";
                        cmd.CommandText = QS_Import_To_TotalDB;
                        //cmd.Parameters.Add(new MySqlParameter("@BlockName", BlockName));
                        Conn.Open();
                        int Import_TotalDB = cmd.ExecuteNonQuery();
                        Conn.Close();
                        if(Import_TotalDB > 0)      //插入总统计表成功
                        {
                            Success_Counter++;
                        }
                        else                        //插入统计表失败
                        {
                            if (Fail_ToDB_Counter > 0)
                            {
                                Import_ToDB_Fail += ",";
                            }
                            Import_ToDB_Fail += BlockName;
                            Fail_ToDB_Counter++;
                        }
                    }
                    else                            //插入地区表失败
                    {
                        if (Fail_ToDB_Counter > 0)
                        {
                            Import_ToDB_Fail += ",";
                        }
                        Import_ToDB_Fail += BlockName;
                        Fail_ToDB_Counter++;
                    }
                }
            }
            labelAlterSuccess.Text = "处理完毕，共处理成功" + Success_Counter + "条记录。";
            if(Fail_ToDB_Counter > 0) labelAlterSuccess.Text += "其中<code>" + Import_ToDB_Fail + "</code>导入失败。";
            if(Fail_Counter > 0) labelAlterSuccess.Text += Repeat_Fail+"含有重复冲突记录，请检查导入表。";
            divAlterSuccess.Visible = true;
            return 0;
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