using Microsoft.Web.Administration;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Security.AccessControl;

namespace AntDeployAgentWindows.Util
{
    public static class IISHelper
    {
        public static int GetIISVersion()
        {
            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters");
                int MajorVersion = (int)parameters.GetValue("MajorVersion");
                return MajorVersion;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static void ApplicationPoolRecycle(string applicationPoolName)
        {
            using (ServerManager iis = new ServerManager())
            {
                iis.ApplicationPools[applicationPoolName].Recycle();
            }
        }

        public static bool IsApplicationPoolExist(string applicationPoolName)
        {
            try
            {
                using (ServerManager iis = new ServerManager())
                {
                    var pool = iis.ApplicationPools[applicationPoolName];
                    if (pool != null) return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string ApplicationPoolStop(string applicationPoolName)
        {
            try
            {
                using (ServerManager iis = new ServerManager())
                    iis.ApplicationPools[applicationPoolName].Stop();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static bool IsApplicationPoolStop(string applicationPoolName)
        {
            using (ServerManager iis = new ServerManager())
                return iis.ApplicationPools[applicationPoolName].State == ObjectState.Stopped;
        }

        public static string ApplicationPoolStart(string applicationPoolName)
        {
            try
            {
                using (ServerManager iis = new ServerManager())
                    iis.ApplicationPools[applicationPoolName].Start();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string WebsiteStart(string siteName)
        {
            try
            {
                using (ServerManager iis = new ServerManager())
                    iis.Sites[siteName].Start();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string WebsiteStop(string siteName)
        {
            try
            {
                using (ServerManager iis = new ServerManager())
                {
                    var site = iis.Sites[siteName];
                    site.Stop();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static bool IsWebsiteStop(string siteName)
        {
            using (ServerManager iis = new ServerManager())
            {
                var site = iis.Sites[siteName];
                return site.State == ObjectState.Stopped;
            }
        }

        public static bool IsDefaultWebSite(string name)
        {
            return name.ToLower().Equals("default web site");
        }


        public static string InstallSite(string name, string siteLocation, string port = "80",string poolName=null,bool isnetcore = false)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    var mySite = iisManager.Sites.Add(name, "http", $"*:{port}:", siteLocation);
                    if (!string.IsNullOrEmpty(poolName))
                    {
                        //看pool是否存在 
                        var isPoolExist = IsApplicationPoolExist(poolName);
                        if (!isPoolExist)
                        {
                            //不存在就创建
                            ApplicationPool newPool = iisManager.ApplicationPools.Add(poolName);
                            newPool.ManagedRuntimeVersion = !isnetcore ? "v4.0" : null;
                            newPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                        }
                    }
                    mySite.ApplicationDefaults.ApplicationPoolName = string.IsNullOrEmpty(poolName) ? "DefaultAppPool" : poolName;
                    iisManager.CommitChanges();
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string InstallVirtualSite(string sitename,string virtualPath, string siteLocation, string poolName = null,bool isnetcore=false)
        {
            try
            {
                if (!virtualPath.StartsWith("/"))
                {
                    virtualPath = "/" + virtualPath;
                }

                using (ServerManager iisManager = new ServerManager())
                {
                    var app = iisManager.Sites[sitename].Applications;
                    var application = app.Add(virtualPath, siteLocation);
                    if (!string.IsNullOrEmpty(poolName))
                    {
                        //看pool是否存在 
                        var isPoolExist = IsApplicationPoolExist(poolName);
                        if (!isPoolExist)
                        {
                            //不存在就创建
                            ApplicationPool newPool = iisManager.ApplicationPools.Add(poolName);
                            newPool.ManagedRuntimeVersion = !isnetcore ? "v4.0" : null;
                            newPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                        }

                        application.ApplicationPoolName = poolName;
                    }
                    //app.VirtualDirectories.Add(virtualPath, siteLocation);
                    iisManager.CommitChanges();
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        public static Tuple<bool, bool> IsSiteExist(string name, string sitename)
        {
            try
            {
                if (string.IsNullOrEmpty(sitename))
                {
                    sitename = "/";
                }

                if (!sitename.StartsWith("/"))
                {
                    sitename = "/" + sitename;
                }


                if (IsDefaultWebSite(name))
                {
                    name = "Default Web Site";
                }
                var siteExist = false;
                var visualExist = false;
                using (ServerManager iis = new ServerManager())
                {
                    var siteList = iis.Sites.ToList();
                    var site = siteList.Where(r => r.Name.ToLower().Equals(name.ToLower())).ToList();
                    if (site.Count == 1)
                    {
                        siteExist = true;
                        var target = site[0];
                        var applicationRoot = target.Applications.FirstOrDefault(r => r.Path.ToLower().Equals(sitename.ToLower()));
                        if (applicationRoot != null)
                        {
                            visualExist = true;
                        }

                    }
                    return new Tuple<bool, bool>(siteExist, visualExist);
                }
            }
            catch (Exception)
            {
                return new Tuple<bool, bool>(false, false);
            }
        }



        public static Tuple<string, string, string> GetWebSiteLocationInIIS(string name, string sitename, Action<string> log)
        {
            try
            {
                if (string.IsNullOrEmpty(sitename))
                {
                    sitename = "/";
                }

                if (!sitename.StartsWith("/"))
                {
                    sitename = "/" + sitename;
                }

                if (IsDefaultWebSite(name))
                {
                    name = "Default Web Site";
                }

                using (ServerManager iis = new ServerManager())
                {
                    var siteList = iis.Sites.ToList();
                    var site = siteList.Where(r => r.Name.ToLower().Equals(name.ToLower())).ToList();
                    if (site.Count == 0)
                    {
                        return null;
                    }
                    if (site.Count > 1)
                    {
                        throw new Exception($"get website by name:{name} but found {site.Count} in iis.");
                    }
                    else
                    {
                        var target = site[0];

                        var applicationRoot = target.Applications.FirstOrDefault(r => r.Path.ToLower().Equals(sitename.ToLower()));
                        if (applicationRoot == null)
                        {
                            return null;
                        }

                        var virtualRoot = applicationRoot.VirtualDirectories.Single(v => v.Path == "/");

                        return new Tuple<string, string, string>(virtualRoot.PhysicalPath, target.Name, applicationRoot.ApplicationPoolName);

                    }
                }
            }
            catch (Exception ex)
            {
                log("get iis info err:" + ex.Message);
                return null;
            }
        }
    }
}
