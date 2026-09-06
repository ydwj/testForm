using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public class AES
{
    /// <summary>
    /// 默认向量
    /// </summary>
    private const string Iv = "zombiestestIV123";

    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="str">需要加密的字符串</param>
    /// <param name="key">32位密钥</param>
    /// <returns>加密后的字符串</returns>
    public static string Encrypt_ECB(string str, string key)
    {
        Byte[] keyArray = Encoding.UTF8.GetBytes(key);
        Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
        var rijndael = new RijndaelManaged();
        rijndael.Key = keyArray;
        rijndael.Mode = CipherMode.ECB;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.IV = Encoding.UTF8.GetBytes(Iv);
        ICryptoTransform cTransform = rijndael.CreateEncryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="str">需要解密的字符串</param>
    /// <param name="key">32位密钥</param>
    /// <returns>解密后的字符串</returns>
    public static string Decrypt_ECB(string str, string key)
    {
        Byte[] keyArray = Encoding.UTF8.GetBytes(key);
        Byte[] toEncryptArray = Convert.FromBase64String(str);
        var rijndael = new RijndaelManaged();
        rijndael.Key = keyArray;
        rijndael.Mode = CipherMode.ECB;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.IV = Encoding.UTF8.GetBytes(Iv);
        ICryptoTransform cTransform = rijndael.CreateDecryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Encoding.UTF8.GetString(resultArray).Trim();
    }

    /// <summary>
    /// CBC加密
    /// </summary>
    /// <param name="content"></param>
    /// <param name="aesKey"></param>
    /// <param name="aesIV"></param>
    /// <returns></returns>
    public static string Encrypt_CBC(string content, string aesKey, string aesIV)
    {

        byte[] byteKEY = Encoding.UTF8.GetBytes(aesKey);
        byte[] byteIV = Encoding.UTF8.GetBytes(aesIV);

        byte[] byteContnet = Encoding.UTF8.GetBytes(content);

        var _aes = new RijndaelManaged();
        _aes.Padding = PaddingMode.PKCS7;
        _aes.Mode = CipherMode.CBC;

        _aes.Key = byteKEY;
        _aes.IV = byteIV;

        var _crypto = _aes.CreateEncryptor(byteKEY, byteIV);
        byte[] decrypted = _crypto.TransformFinalBlock(
            byteContnet, 0, byteContnet.Length);

        _crypto.Dispose();

        return Convert.ToBase64String(decrypted);
    }

    /// <summary>
    /// CBC解密
    /// </summary>
    /// <param name="decryptStr">要解密的串</param>
    /// <param name="aesKey">密钥</param>
    /// <param name="aesIV">IV</param>
    /// <returns></returns>
    public static string Decrypt_CBC(string decryptStr, string aesKey, string aesIV)
    {

        byte[] byteKEY = Encoding.UTF8.GetBytes(aesKey);
        byte[] byteIV = Encoding.UTF8.GetBytes(aesIV);

        byte[] byteDecrypt = Convert.FromBase64String(decryptStr);

        var _aes = new RijndaelManaged();
        _aes.Padding = PaddingMode.PKCS7;
        _aes.Mode = CipherMode.CBC;

        _aes.Key = byteKEY;
        _aes.IV = byteIV;

        var _crypto = _aes.CreateDecryptor(byteKEY, byteIV);
        byte[] decrypted = _crypto.TransformFinalBlock(
            byteDecrypt, 0, byteDecrypt.Length);

        _crypto.Dispose();

        return Encoding.UTF8.GetString(decrypted).Trim();
    }

    ///// <summary> 
    ///// 有密码的AES加密   
    ///// </summary> 
    ///// <param name="text">加密字符</param> 
    ///// <param name="password">加密的密码</param> 
    ///// <param name="iv">密钥</param> 
    ///// <returns></returns> 
    //public static string Encrypt(string text, string password)
    //{
    //    RijndaelManaged rijndaelCipher = new RijndaelManaged();

    //    rijndaelCipher.Mode = CipherMode.CBC;

    //    rijndaelCipher.Padding = PaddingMode.PKCS7;

    //    rijndaelCipher.KeySize = 128;

    //    rijndaelCipher.BlockSize = 128;

    //    byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);

    //    byte[] keyBytes = new byte[16];

    //    int len = pwdBytes.Length;

    //    if (len > keyBytes.Length) len = keyBytes.Length;

    //    System.Array.Copy(pwdBytes, keyBytes, len);

    //    rijndaelCipher.Key = keyBytes;


    //    byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(Iv);
    //    rijndaelCipher.IV = ivBytes;

    //    ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

    //    byte[] plainText = Encoding.UTF8.GetBytes(text);

    //    byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

    //    return Convert.ToBase64String(cipherBytes);

    //}

    ///// <summary> 
    ///// AES解密  
    ///// </summary> 
    ///// <param name="text"></param> 
    ///// <param name="password"></param> 
    ///// <param name="iv"></param> 
    ///// <returns></returns> 
    //public static string Decrypt(string text, string password)
    //{
    //    RijndaelManaged rijndaelCipher = new RijndaelManaged();

    //    rijndaelCipher.Mode = CipherMode.CBC;

    //    rijndaelCipher.Padding = PaddingMode.PKCS7;

    //    rijndaelCipher.KeySize = 128;

    //    rijndaelCipher.BlockSize = 128;

    //    byte[] encryptedData = Convert.FromBase64String(text);

    //    byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);

    //    byte[] keyBytes = new byte[16];

    //    int len = pwdBytes.Length;

    //    if (len > keyBytes.Length) len = keyBytes.Length;

    //    System.Array.Copy(pwdBytes, keyBytes, len);

    //    rijndaelCipher.Key = keyBytes;

    //    byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(Iv);
    //    rijndaelCipher.IV = ivBytes;

    //    ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

    //    byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

    //    return Encoding.UTF8.GetString(plainText);

    //}
}
