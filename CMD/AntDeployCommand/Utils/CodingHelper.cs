using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using LibGit2Sharp;
using Newtonsoft.Json;

namespace AntDeployCommand.Utils
{
    public static class CodingHelper
    {
        public static string HttpPost(string url, string json, Action<string, LogLevel> logger)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest WReq = (HttpWebRequest)WebRequest.Create(url);
                WReq.Method = "POST";
                WReq.Timeout = 5000;
                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                WReq.ContentType = "application/json";
                WReq.ContentLength = byteArray.Length;
                using (var newStream = WReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);
                }
                HttpWebResponse WResp = (HttpWebResponse)WReq.GetResponse();
                if (WResp != null)
                {
                    Stream stream = WResp.GetResponseStream();
                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);
                        result = reader.ReadToEnd();
                        reader.Close();
                        stream.Close();
                    }
                    WResp.Close();
                }

                return result;
            }
            catch (Exception ex1)
            {
                logger(ex1.Message, LogLevel.Fatal);
                //ignore
            }
            return null;
        }
        public static string MD5(this string str)
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

        /// <summary>
        /// 获取本机的mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            var firstMacAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            return firstMacAddress;
        }

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
                            if (string.IsNullOrEmpty(ipStr)) continue;
                            localIpAddress.Add(ipStr);
                            GatewayIPAddressInformationCollection gateways = ipProperties.GatewayAddresses;
                            var getway = gateways.FirstOrDefault();
                            if (getway != null && getway.Address != null)
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
    }
}
