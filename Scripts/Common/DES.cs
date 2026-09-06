using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;//引入命名空间，加密 
using System.IO;

/// <summary>
/// DES加密解密，默认有密钥，只需传入任意的字符串密钥
/// </summary>
public static class DES
{
    //默认密钥向量
    private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

    #region DES加密字符串

    /// <summary>
    /// DES加密字符串
    /// </summary>
    /// <param name="encryptString">待加密的字符串</param>
    /// <param name="encryptKey">加密密钥,要求为8位</param>
    /// <returns>加密成功返回加密后的16进制字符串，失败返回源串</returns>
    public static string EncryptDES(string encryptString, string encryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);//加密密钥          
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            byte[] tembyte = mStream.ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (byte item in tembyte)
            {
                sb.AppendFormat("{0:x2}", item);
            }
            return sb.ToString();
        }
        catch (Exception)
        {
            throw;
        }
    }

    #endregion

    #region DES解密字符串

    /// <summary>
    /// DES解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的16进制字符串</param>
    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
    public static string DecryptDES(string decryptString, string decryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);//解密密钥
            byte[] rgbIV = Keys;
            decryptString = decryptString.Replace(" ", "");
            decryptString = decryptString.Replace("\r\n", "");
            string txt16 = decryptString;
            byte[] inputByteArray = new byte[txt16.Length / 2];
            for (int i = 0; i < inputByteArray.Length; i++)
            {
                inputByteArray[i] = Convert.ToByte(txt16.Substring(i * 2, 2), 16);
            }
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion
}
