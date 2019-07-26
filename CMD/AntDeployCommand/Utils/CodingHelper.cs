using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Newtonsoft.Json;

namespace AntDeployCommand.Utils
{
    public static class CodingHelper
    {
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
