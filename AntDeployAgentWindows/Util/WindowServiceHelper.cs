using Microsoft.Web.Administration;
using System;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;

namespace AntDeployAgentWindows.Util
{
    public static class WindowServiceHelper
    {
        public static ServiceController GetWindowServiceByName(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                return service;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetWindowServiceLocation(string serviceName)
        {
            try
            {
                var service = GetWindowServiceByName(serviceName);
                if (service == null) return null;
                var machineName = Environment.MachineName;
                var registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
                var keyHKLM = Registry.LocalMachine;

                RegistryKey key;
                if (machineName != "")
                {
                    key = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName).OpenSubKey(registryPath);
                }
                else
                {
                    key = keyHKLM.OpenSubKey(registryPath);
                }

                if (key == null) return null;
                var value = key.GetValue("ImagePath").ToString();
                key.Close();
                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string StartService(string serviceName, int timeouSeconds)
        {
            try
            {
                var service = new ServiceController(serviceName);
                var timeout = TimeSpan.FromSeconds(timeouSeconds);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string StopService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                var service = new ServiceController(serviceName);
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
