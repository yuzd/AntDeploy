using Microsoft.Win32;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace AntDeployAgentWindows.Util
{
    public static class WindowServiceHelper
    {

       


        //public static string InstallWindowsService(string exePath)
        //{
        //    try
        //    {
        //        if (!exePath.Trim().ToLower().EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        //        {
        //            return $"{exePath} is not exe!";
        //        }

        //        string serviceName = GetServiceNameByFile(exePath);
        //        if (string.IsNullOrEmpty(serviceName))
        //        {
        //            return $"{exePath} is not windows service!";
        //        }

        //        if (ServiceIsExisted(serviceName))
        //        {
        //            return $"{serviceName} is exist!";
        //        }

        //        string[] cmdline = { };
        //        using (TransactedInstaller transactedInstaller = new TransactedInstaller())
        //        {
        //            using (AssemblyInstaller assemblyInstaller = new AssemblyInstaller(exePath, cmdline)
        //            {
        //                UseNewContext = true
        //            })
        //            {
        //                transactedInstaller.Installers.Add(assemblyInstaller);
        //                transactedInstaller.Install(new System.Collections.Hashtable());
        //            }

        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}


        /// <summary>
        /// 判断服务是否已经存在
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>bool</returns>
        public static bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(s => s.ServiceName == serviceName);
        }

        /// <summary>
        /// 获取Windows服务的名称
        /// </summary>
        /// <param name="serviceFileName">文件路径</param>
        /// <returns>服务名称</returns>
        //public static string GetServiceNameByFile(string serviceFileName)
        //{
        //    try
        //    {

        //        Assembly assembly = Assembly.LoadFrom(serviceFileName);
        //        Type[] types = assembly.GetTypes();
        //        foreach (Type myType in types)
        //        {
        //            if (myType.IsClass && myType.BaseType == typeof(System.Configuration.Install.Installer))
        //            {
        //                FieldInfo[] fieldInfos = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static);
        //                foreach (FieldInfo myFieldInfo in fieldInfos)
        //                {
        //                    if (myFieldInfo.FieldType == typeof(System.ServiceProcess.ServiceInstaller))
        //                    {
        //                        using (ServiceInstaller serviceInstaller = (ServiceInstaller)myFieldInfo.GetValue(Activator.CreateInstance(myType)))
        //                        {
        //                            return serviceInstaller.ServiceName;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static Tuple<ServiceController,string> GetWindowServiceByName(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                return new Tuple<ServiceController, string>(services.FirstOrDefault(s => s.ServiceName == serviceName),null);
            }
            catch (Exception ex)
            {
                return new Tuple<ServiceController, string>(null,ex.Message);
            }
        }

        public static string GetWindowServiceLocation(string serviceName)
        {
            try
            {
                var serviceR = GetWindowServiceByName(serviceName);
                if(!string.IsNullOrEmpty(serviceR.Item2)) return null;
                if (serviceR.Item1 == null) return null;
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
                using (var service = new ServiceController(serviceName))
                {

                    var timeout = TimeSpan.FromSeconds(timeouSeconds);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    service.Refresh();
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static bool IsStart(string serviceName)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    service.Refresh();
                    return service.Status == ServiceControllerStatus.Running;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string StopService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    return string.Empty;
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
