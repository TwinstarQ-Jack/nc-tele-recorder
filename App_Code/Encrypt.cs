using System;
using System.Collections.Generic;
using System.Web;
using System.Security;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 实现加密（MD5）
/// </summary>
public class Encrypt
{
    public Encrypt()
    {

    }
    /// <summary>
    /// 使用MD5进行字串加密
    /// </summary>
    /// <param name="EncryptedString"></param>
    /// <returns>返回string类的加密字符串</returns>
    public string EncryptMD5(string EncryptedString)
    {
        string result = null; 
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] palindata = Encoding.Default.GetBytes(EncryptedString);//将要加密的字符串转换为字节数组
        byte[] encryptdata = md5.ComputeHash(palindata);//将字符串加密后也转换为字符数组
        result = Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为加密字符串
        return result;
    }

}