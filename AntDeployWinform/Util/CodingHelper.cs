using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using AntDeployWinform.Models;
using AntDeployWinform.Winform;
using Newtonsoft.Json;

namespace AntDeployWinform.Util
{
    public static class CodingHelper
    {


        /// <summary>
        /// 获取本机的Ip地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return string.Empty;
            }

            var withGetWayAddress = string.Empty;
            var localIpAddress = new List<string>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    var ipProperties = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            var ipStr = ip.Address.ToString();
                            if(string.IsNullOrEmpty(ipStr)) continue;
                            localIpAddress.Add(ipStr);
                            GatewayIPAddressInformationCollection gateways = ipProperties.GatewayAddresses;
                            var getway = gateways.FirstOrDefault();
                            if (getway != null && getway.Address!=null)
                            {
                                var getwayAddress = getway.Address.ToString();
                                if (!string.IsNullOrEmpty(getwayAddress))
                                {
                                    withGetWayAddress = ipStr;
                                }
                            }
                        }
                    }
                }
            }

            //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            //var ipaddress = host
            //    .AddressList
            //    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            //if (ipaddress == null) return string.Empty;
            if (!string.IsNullOrEmpty(withGetWayAddress)) return withGetWayAddress;
            return localIpAddress.FirstOrDefault();
        }

        /// <summary>
        /// 获取本机的mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            try
            {
                var firstMacAddress = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic =>
                        nic.OperationalStatus == OperationalStatus.Up &&
                        nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return firstMacAddress;
            }
            catch (Exception)
            {
                return string.Empty;
            }
           
        }


        /// <summary>
        /// 根据deps json文件解析runtime 里面配置的netcore 版本
        /// </summary>
        /// <param name="depsJsonFile"></param>
        /// <returns></returns>
        public static string GetSdkInDepsJson(string depsJsonFile)
        {
            try
            {
                var content = File.ReadAllText(depsJsonFile);
                if (!content.Contains("runtimeTarget")) return string.Empty;
                var temp1 = content.Split(new string[] {"runtimeTarget"}, StringSplitOptions.None);
                var temp2 = temp1[1].Split(new string[] {"Version=v"}, StringSplitOptions.None)[1];
                var ver = "";

                foreach (var c in temp2)
                {
                    if (c == '.')
                    {
                        ver += c;
                        continue;
                    }

                    if (char.IsDigit(c))
                    {
                        ver += c;continue;
                    }
                    break;
                }

                return ver;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 选择一个dll文件
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetDockerServiceExe(string folderName)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = !string.IsNullOrEmpty(lang) && lang.StartsWith("zh-") ?"选择程序运行的入口DLL": "Choose ENTRYPOINT DLL";
            fdlg.Filter = "(.dll)|*.dll";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            fdlg.InitialDirectory = folderName;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                if (!fdlg.FileName.ToLower().EndsWith(".dll"))
                {
                    return string.Empty;
                }

                return fdlg.FileName;

            }

            return string.Empty;
        }

        /// <summary>
        /// 选择一个exe文件
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetWindowsServiceExe(string folderName)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = !string.IsNullOrEmpty(lang) && lang.StartsWith("zh-")?"选择服务的运行程序Exe": "Choose Service EXE";
            fdlg.Filter = "(.exe)|*.exe";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            fdlg.InitialDirectory = folderName;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                if (!fdlg.FileName.ToLower().EndsWith(".exe") )
                {
                    return string.Empty;
                }

                return fdlg.FileName;

            }

            return string.Empty;
        }


        public static string FindDepsJsonFile(string folderName)
        {
            var fileList = Directory.GetFiles(folderName, "*.deps.json").Where(r=>!r.ToLower().Contains(".runtimeconfig.")).ToList();
            if (fileList.Count != 1) return string.Empty;
            return fileList[0];
        }

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


        public static List<string> ParseVersionlist(List<string> list)
        {
            var result = new List<string>();

            foreach (var li in list)
            {
                var version = string.Empty;
                var content = li.JsonToObject<RollBackVersion>();
                if (content == null)
                {
                    version = li;
                }
                else
                {
                    if (!string.IsNullOrEmpty(content.Version))
                    {
                        version = content.Version;
                    }
                }

                if (string.IsNullOrEmpty(version)) continue;

                result.Add(version);
            }

            return result;
        }


        public static string AppendRetryStrings(this int retryTimes)
        {
            var str = "";
            for (int i = 0; i < retryTimes; i++)
            {
                str += "_";
            }

            return str;
        }
    }
}
