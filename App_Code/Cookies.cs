using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

/// <summary>
/// Cookies:与网站的Cookies相关的功能
/// </summary>
public class Cookies
{
    public Cookies()
    {
    }
    /// <summary>
    /// IfLogin函数：检查当前是否已经登录，登录返回true
    /// </summary>
    public bool IfLogin()
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies["NC"];
        if (cookie != null)
            return true;
        else
            return false;
    }
    public bool Logout()
    {
        if (IfLogin())
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["NC"];
            cookie.Expires = DateTime.Now.AddMinutes(-1);
            HttpContext.Current.Response.Cookies.Remove("NC");
            HttpContext.Current.Response.Cookies.Add(cookie);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 将当前登录信息写入cookies，默认有效期1天，微信7天。
    /// </summary>
    /// <param name="paras">存储参数</param>
    /// <param name="WeixinSign">是否微信登录</param>
    /// <returns>写入是否成功的结果(bool)</returns>
    public bool WriteToCookies(string[] paras,bool WeixinSign)
    {
        try
        {
            int paras_NUM = paras.Length;
            HttpCookie writein = new HttpCookie("NC");
            //设定cookies的生命周期
            if (WeixinSign)
            {
                writein.Expires = DateTime.Now.AddDays(7);
            }
            else
            {
                writein.Expires = DateTime.Now.AddDays(1);
            }
            //增加cookies数组内容、索引
            for(int i = 0; i < paras_NUM;)
            {
                writein.Values[paras[i]] = HttpUtility.UrlEncode(paras[i + 1], Encoding.GetEncoding("UTF-8"));
                i = i + 2;
            }
            //Using System.Text 才能使用Encoding
            writein["logintime"] = DateTime.Now.ToString();
            //writein.Value = HttpUtility.UrlEncode(writein.Value, Encoding.GetEncoding("UTF-8"));
            //将Cookies保存到用户端
            HttpContext.Current.Response.Cookies.Add(writein);
            return true;
        }
        catch (Exception)
        {
            throw;            
        }
    }
    /// <summary>
    /// 读取Cookies
    /// </summary>
    /// <param name="paras">查询参数数组</param>
    /// <returns>查询参数对应的值</returns>
    public string[] ReadCookies(string[] paras)
    {
        int paras_Num = paras.Length;
        string[] result = new string[paras_Num];
        try
        {
            HttpCookie read = HttpContext.Current.Request.Cookies["NC"];
            //read.Value = HttpUtility.UrlDecode(read.Value, Encoding.GetEncoding("UTF-8"));
            for (int i = 0; i < paras_Num; i++)
            {
                result[i] = HttpUtility.UrlDecode(read.Values[paras[i]], Encoding.GetEncoding("UTF-8"));

            }
            return result;
        }
        catch (Exception)
        {
            return result;
            throw;
        }
    }
}