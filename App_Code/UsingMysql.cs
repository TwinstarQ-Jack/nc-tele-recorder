using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

/// <summary>
/// UsingMysql 的摘要说明
/// 实现所有与Mysql数据库相关的一切操作
/// </summary>
public class UsingMysql
{
    //数据库连接
    private readonly MySqlConnection _ConnDefault = new MySqlConnection("");
    public UsingMysql()
    {
        MySqlConnection Conn = _ConnDefault;
        Conn.Close();
    }
    ///连接数据库设置

/// <summary>
/// ConnMode只读取
/// </summary>
    public MySqlConnection ConnMode
    {
        get { return _ConnDefault; }
    }
    /// <summary>
    /// 获取所需要的表的全部内容
    /// </summary>
    /// <param name="QueryTable">所需表表名</param>
    /// <returns></returns>
    public DataSet GetTables_All(string QueryTable)
    {
        string QueryString = "select * from " + QueryTable;
        MySqlDataAdapter da = new MySqlDataAdapter();
        MySqlConnection Conn = ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        cmd.CommandText = QueryString;
        da.SelectCommand = cmd;
        Conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        Conn.Close();
        return ds;
    }
    /// <summary>
    /// 创建查询，并将导出的数据库保存到内存中
    /// </summary>
    /// <param name="Conn">MySql连接串</param>
    /// <param name="paras">参数字符数组paras要求详询手册</param>
    /// <returns></returns>
    public DataSet getDataSet(MySqlConnection Conn,string[] paras)
    {
        //paras[0]-选择输出字段,paras[1]-选择表
        //paras[2,*] 条件+条件值
        int paras_NUM = paras.Length;
        MySqlDataAdapter da = new MySqlDataAdapter();
        string connstring = "select " + paras[0] + " from " + paras[1];
        if(paras_NUM > 2)
        {
            connstring += " where ";
            for (int i = 2; i < paras_NUM;)
            {
                if (i != 2)
                {
                    connstring += (",");
                }
                string temp = paras[i] + "=" + paras[i + 1];
                connstring += temp;
                i = i + 2;
            }
        }
        MySqlCommand cmd = new MySqlCommand(connstring, Conn);
        da.SelectCommand = cmd;
        DataSet ds = new DataSet();
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            da.Fill(ds, paras[1]);
            Conn.Close();
            return ds;
        }
        catch (Exception)
        {
            Conn.Close();
            throw;
        }     
    }

    /// <summary>
    /// 创建查询，返回所需要的数据
    /// </summary>
    /// <param name="Conn">MySql连接串</param>
    /// <param name="paras">参数字符数组paras要求详询手册</param>
    /// <returns>DataReader对象</returns>
    public MySqlDataReader getDataReader(MySqlConnection Conn,string[] paras)
    {
        int paras_NUM = paras.Length;
        if (Conn.State == ConnectionState.Open)
        {
            Conn.Close();
        }
        try
        {
            Conn.Open();
            MySqlCommand cmd = Conn.CreateCommand();
            //paras[0]-选择输出字段,paras[1]-选择表
            cmd.CommandText = "select " + paras[0] + " from " + paras[1];
            //paras[2,*] 条件+条件值
            if (paras_NUM > 2)
            {
                cmd.CommandText += " where ";
                for (int i = 2; i < paras_NUM;)
                {
                    if(i != 2)
                    {
                        cmd.CommandText += (",");
                    }
                    cmd.CommandText += (paras[i] + "=@" + paras[i]);
                    cmd.Parameters.Add(new MySqlParameter("@" + paras[i], paras[i + 1]));
                    i = i + 2;
                }
            }
            //执行查询并输出到DataReader实例
            MySqlDataReader getValueReader = cmd.ExecuteReader();
            return getValueReader;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 通过用户名查询用户表（DataReader方式）
    /// </summary>
    /// <param name="CardID">用户名查询参数</param>
    /// <returns>返回数据库密码，错误返回NULL</returns>
    public string[] UserString(string CardID)
    {
        string[] result = new string[4];
        string[] paras = { "RealName,Password,UID,Rank", "account", "CardID", CardID };
        MySqlConnection Conn = ConnMode;
        try
        {
            MySqlDataReader getValueReader = getDataReader(Conn,paras);
            if (getValueReader.Read())
            {
                result[0] = getValueReader["Password"].ToString();
                result[1] = getValueReader["RealName"].ToString();
                result[2] = getValueReader["UID"].ToString();
                result[3] = getValueReader["Rank"].ToString();
            }
            getValueReader.Close();
            return result;
        }
        catch (Exception)
        {
            return result;
            throw;
        }
        finally
        {
            Conn.Close();
        }        
    }
    /// <summary>
    /// 更新数据库统计表
    /// </summary>
    /// <param name="Belongs">TotalDB表归属</param>
    /// <param name="TOTDB_Name">TotalDB表Name主键</param>
    /// <param name="ID">其他Total表的搜索index</param>
    /// <param name="Is_dealOrder">是否增加dealOrder项</param>
    /// <param name="addMode">是否增加DealOrder//Order值</param>
    /// <returns>写入反馈状态码</returns>
    public int UpdTotalSheets (string Belongs, string TOTDB_Name,string ID,bool Is_dealOrder,bool addMode)
    {
        MySqlConnection Conn = ConnMode;
        string Months = DateTime.Now.ToString("yyyyMM");
        string TOTKey = ID + Months;  //查询total子表primary Key
        int addnum = (addMode == true) ? 1 : -1;    //初始化更新量
        MySqlCommand cmd = Conn.CreateCommand();    //初始化cmd
        MySqlDataAdapter da = new MySqlDataAdapter();     //初始化MySQL适配器
        int Orders = 0, DealOrder = 0;               //初始订单数变量与已解决数统计变量
            //更新TotalDB数据表
        string TotalDB_Update = null, TotalDB_Query = null;
        if(Conn.State == ConnectionState.Open)     //关闭已经开启的数据库连接
        {
            Conn.Close();
        }
        TotalDB_Query = "select * from totaldatabase where Name=@Name and Belongs=@Belongs";
        cmd.CommandText = TotalDB_Query;
        cmd.Parameters.Add(new MySqlParameter("@Name", TOTDB_Name));
        cmd.Parameters.Add(new MySqlParameter("@Belongs", Belongs));
        try
        {
            da.SelectCommand = cmd;
            Conn.Open();
            DataSet ds_TotalDB = new DataSet();
            da.Fill(ds_TotalDB);
            Conn.Close();
            DataRow dr_TotalDB = ds_TotalDB.Tables[0].Rows[0];
            if (Is_dealOrder)                  //增加已解决数目统计模式
            {
                DealOrder = Convert.ToInt32(dr_TotalDB["DealOrder"].ToString()) + addnum;
                TotalDB_Update = "update totaldatabase set DealOrder=@DealOrder where Name=@Name and Belongs=@Belongs";
            }
            else
            {
                Orders = Convert.ToInt32(dr_TotalDB["Orders"].ToString()) + addnum;
                TotalDB_Update = "update totaldatabase set Orders=@Orders where Name=@Name and Belongs=@Belongs";
            }
            cmd.CommandText = TotalDB_Update;
            cmd.Parameters.Add(new MySqlParameter("@DealOrder", DealOrder));
            cmd.Parameters.Add(new MySqlParameter("@Orders", Orders));
            Conn.Open();
            int Feedback = cmd.ExecuteNonQuery();
            Conn.Close();
            if (Feedback <= 0) return 101;            //插入TotalDB失败

            //更新其他total表
            string[] Belongs_Sum = { "account", "questiontypelv1", "questiontypelv2", "blocks" };
            string[] Total_Sub_Sum = { "totaluser", "totalordertype1", "totalordertype2", "totalblock" };
            int Sum_Index = 4, Search_Index = 0;
            MySqlCommand cmdsub = Conn.CreateCommand();
            for (; Search_Index < Sum_Index; Search_Index++)
            {
                if (Belongs == Belongs_Sum[Search_Index]) break;
            }               
            string Query_TotalSub = "select * from " + Total_Sub_Sum[Search_Index] + " where TOTKey=@TOTKey";  //根据TOTKey查询对应的表
            cmdsub.CommandText = Query_TotalSub;
            cmdsub.Parameters.Add(new MySqlParameter("@TOTKey", TOTKey));
            da.SelectCommand = cmdsub;
            Conn.Open();
            DataSet ds_TotalSub = new DataSet();
            da.Fill(ds_TotalSub);
            Conn.Close();
            if (ds_TotalSub.Tables.Count == 0 || ds_TotalSub.Tables[0].Rows.Count == 0) 
            {
                string TotalSub_Insert = null;
                TotalSub_Insert = "insert into " + Total_Sub_Sum[Search_Index] + " (TOTKey,ID,Month,Orders) values(@TOTKey,@ID,@Month,1)";
                cmdsub.CommandText = TotalSub_Insert;
                cmdsub.Parameters.Add(new MySqlParameter("@ID", ID));
                cmdsub.Parameters.Add(new MySqlParameter("@Month", Months));
                Conn.Open();
                Feedback = cmdsub.ExecuteNonQuery();
                Conn.Close();
                if (Feedback <= 0) return 103;                   //创建Total子表记录失败
                return 100;                                      //写入两个表成功
            }
            else
            {
                DataRow dr_TotalSub = ds_TotalSub.Tables[0].Rows[0];
                string TotalSub_Update = null; 
                if (Is_dealOrder)
                {
                    DealOrder = Convert.ToInt32(dr_TotalSub["DealOrder"].ToString()) + addnum;
                    TotalSub_Update = "update " + Total_Sub_Sum[Search_Index] + " set DealOrder=@DealOrder where TOTKey=@TOTKey";
                }
                else
                {
                    Orders = Convert.ToInt32(dr_TotalSub["Orders"].ToString()) + addnum;
                    TotalSub_Update = "update " + Total_Sub_Sum[Search_Index] + " set Orders=@Orders where TOTKey=@TOTKey";
                }
                cmdsub.CommandText = TotalSub_Update;
                cmdsub.Parameters.Add(new MySqlParameter("@DealOrder", DealOrder));
                cmdsub.Parameters.Add(new MySqlParameter("@Orders", Orders));
                Conn.Open();
                Feedback = cmdsub.ExecuteNonQuery();
                Conn.Close();
                if (Feedback <= 0) return 102;                    //写入Total各项子表失败
                return 100;                                       //写入两个表成功
            }
        }
        catch (Exception)
        {
            Conn.Close();
            return -1;                          //数据库操作失败
            throw;
        }
    }
    /// <summary>
    /// 更新totalUser表的AddOrder数（唯一功能）
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public int UpdTotalUser_Add(string ID)
    {
        MySqlConnection Conn = ConnMode;
        MySqlCommand cmd = Conn.CreateCommand();
        string Months = DateTime.Now.ToString("yyyyMM");
        string TOTKey = ID + Months;
        string QueryString = "select * from totaluser where TOTKey=" + TOTKey;
        DataSet ds = new DataSet();
        MySqlDataAdapter da = new MySqlDataAdapter();
        cmd.CommandText = QueryString;
        try
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            Conn.Open();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Conn.Close();
            string db_Command = null;
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) 
            {
                db_Command = "insert into totaluser (TOTkey,ID,Month,AddOrders,Orders) values(@TOTKey,@ID,@Month,1,0)";
                cmd.CommandText = db_Command;
                cmd.Parameters.Add(new MySqlParameter("@TOTKey", TOTKey));
                cmd.Parameters.Add(new MySqlParameter("@ID", ID));
                cmd.Parameters.Add(new MySqlParameter("@Month", Months));
            }
            else
            {
                db_Command = "update totaluser set AddOrders=@AddOrders where TOTKey=@TOTKey";
                int AddOrders = Convert.ToInt32(ds.Tables[0].Rows[0]["AddOrders"].ToString()) + 1;
                cmd.CommandText = db_Command;
                cmd.Parameters.Add(new MySqlParameter("@AddOrders", AddOrders));
                cmd.Parameters.Add(new MySqlParameter("@TOTKey", TOTKey));
            }
            Conn.Open();
            int Feedback = cmd.ExecuteNonQuery();
            Conn.Close();
            if (Feedback <= 0) return 102;              //创建子段出错
            return 100;                                 //创建子段成功
        }
        catch (Exception)
        {
            Conn.Close();
            return 101;                     //数据库出错
            throw;
        }
    }
    public string UpdRemarks(string DealKey,string RealName ,string remarks)
    {
        MySqlConnection Conn = ConnMode;
        MySqlDataAdapter da = new MySqlDataAdapter();
        MySqlCommand cmd = Conn.CreateCommand();
        //获取原来表对应DealKey是否有值
        cmd.CommandText = "select RemarkID from dealorderremark where DealKey=@DealKey order by CAST(RemarkID as UNSIGNED) desc";
        da.SelectCommand = cmd;
        cmd.Parameters.Add(new MySqlParameter("@DealKey", DealKey));
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
            DataTable dt = ds.Tables[0];
            //添加记录
            string Insert_DB = "insert into dealorderremark (RemarkKey,RemarkID,DealKey,RemarkerName,Content) values(@RKey,@RID,@DKey,@RName,@Content)";
            cmd.CommandText = Insert_DB;
            if (dt.Rows.Count == 0)           //新纪录
            {
                cmd.Parameters.Add(new MySqlParameter("@RKey", DealKey + "01"));
                cmd.Parameters.Add(new MySqlParameter("@RID", "01"));
            }
            else
            {
                string newID = (Convert.ToInt32(dt.Rows[0][0].ToString()) + 1).ToString();
                cmd.Parameters.Add(new MySqlParameter("@RKey", DealKey + newID));
                cmd.Parameters.Add(new MySqlParameter("@RID", newID));
            }
            cmd.Parameters.Add(new MySqlParameter("@DKey", DealKey));
            cmd.Parameters.Add(new MySqlParameter("@RName", RealName));
            cmd.Parameters.Add(new MySqlParameter("@Content", remarks));
            Conn.Open();
            int Feedback = cmd.ExecuteNonQuery();
            Conn.Close();
            if (Feedback > 0)
            {
                return null;                   //没有错误信息，正常运行
            }
            else
            {
                return "错误信息：更新备注表失败，请联系管理员。";
            }
        }
        catch (Exception ex)
        {
            return "错误信息：(UpdRemarks_DBConn_ERROR)"+ex.Message;
        }
    }
}