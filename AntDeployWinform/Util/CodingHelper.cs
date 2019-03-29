using System;
using System.Security.Cryptography;
using System.Text;
using AntDeployWinform.Models;
using Newtonsoft.Json;

namespace AntDeployWinform.Util
{
    public static class CodingHelper
    {
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = string.Empty;
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
        public static T JsonToObject<T>(this string str)
        {
            try
            {
                var resultModel = JsonConvert.DeserializeObject<T>(str);
                return resultModel;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string AESDecrypt(string hexString, string key ="56dPz3VDYwGpJYqe7dFG0g==")
        {

            try
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.Key = Convert.FromBase64String(key);
                    aesProvider.Mode = CipherMode.ECB;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                    {
                        hexString = hexString.ToLower();
                        byte[] byteArray = new byte[hexString.Length >> 1];
                        int index = 0;
                        for (int i = 0; i < hexString.Length; i++)
                        {
                            if (index > hexString.Length - 1) continue;
                            byte highDit = (byte)(Digit(hexString[index], 16) & 0xFF);
                            byte lowDit = (byte)(Digit(hexString[index + 1], 16) & 0xFF);
                            byteArray[i] = (byte)(highDit << 4 | lowDit & 0xFF);
                            index += 2;
                        }

                        byte[] inputBuffers = byteArray;
                        byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                        aesProvider.Clear();
                        return Encoding.UTF8.GetString(results);
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        private static char[] HEX_CHAR_TABLE = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        public static string AESEncrypt(string hexString, string key ="56dPz3VDYwGpJYqe7dFG0g==")
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Convert.FromBase64String(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    var bytes = Encoding.UTF8.GetBytes(hexString);
                    byte[] data = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                    byte[] hex = new byte[data.Length * 2];
                    int index = 0;
                    foreach (byte b in data)
                    {
                        int v = b & 0xFF;
                        hex[index++] = (byte)HEX_CHAR_TABLE[(int)((uint)v >> 4)];
                        hex[index++] = (byte)HEX_CHAR_TABLE[v & 0xF];
                    }
                    return Encoding.UTF8.GetString(hex);
                }
            }
        }


        public static int Digit(char value, int radix)
        {
            if ((radix <= 0) || (radix > 36))
                return -1; // Or throw exception

            if (radix <= 10)
                if (value >= '0' && value < '0' + radix)
                    return value - '0';
                else
                    return -1;
            else if (value >= '0' && value <= '9')
                return value - '0';
            else if (value >= 'a' && value < 'a' + radix - 10)
                return value - 'a' + 10;
            else if (value >= 'A' && value < 'A' + radix - 10)
                return value - 'A' + 10;

            return -1;
        }
    }
}
