using Microsoft.Web.Administration;
using Microsoft.Win32;
using System;
using System.Linq;

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
            ServerManager iis = new ServerManager();
            iis.ApplicationPools[applicationPoolName].Recycle();
        }

        public static void ApplicationPoolStop(string applicationPoolName)
        {
            ServerManager iis = new ServerManager();
            iis.ApplicationPools[applicationPoolName].Stop();
        }

        public static bool IsApplicationPoolStop(string applicationPoolName)
        {
            ServerManager iis = new ServerManager();
            return iis.ApplicationPools[applicationPoolName].State == ObjectState.Stopped;
        }

        public static void ApplicationPoolStart(string applicationPoolName)
        {
            ServerManager iis = new ServerManager();
            iis.ApplicationPools[applicationPoolName].Start();
        }

        public static void WebsiteStart(string siteName)
        {
            ServerManager iis = new ServerManager();
            iis.Sites[siteName].Start();
        }

        public static Site WebsiteStop(string siteName)
        {
            ServerManager iis = new ServerManager();
            var site = iis.Sites[siteName];
            site.Stop();
            return site;
        }
        public static bool IsWebsiteStop(string siteName)
        {
            ServerManager iis = new ServerManager();
            var site = iis.Sites[siteName];
            return site.State == ObjectState.Stopped;
        }

        public static bool IsDefaultWebSite(string name)
        {
            return name.ToLower().Equals("default web site");
        }


        public static string InstallSite(string name, string siteLocation, string port = "80",string poolName=null)
        {
            try
            {
                ServerManager iisManager = new ServerManager();
                var mySite = iisManager.Sites.Add(name, "http", $"*:{port}:", siteLocation);
                mySite.ApplicationDefaults.ApplicationPoolName = string.IsNullOrEmpty(poolName) ? "DefaultAppPool": poolName;
                iisManager.CommitChanges();
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string InstallVirtualSite(string sitename,string virtualPath, string siteLocation, string poolName = null)
        {
            try
            {
                if (!virtualPath.StartsWith("/"))
                {
                    virtualPath = "/" + virtualPath;
                }
                ServerManager iisManager = new ServerManager();
                var app = iisManager.Sites[sitename].Applications;
                var application = app.Add(virtualPath, siteLocation);
                if (!string.IsNullOrEmpty(poolName))
                {
                    application.ApplicationPoolName = poolName;
                }
                //app.VirtualDirectories.Add(virtualPath, siteLocation);
                iisManager.CommitChanges();
                return string.Empty;
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
                ServerManager iis = new ServerManager();
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

                ServerManager iis = new ServerManager();
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
            catch (Exception ex)
            {
                log("get iis info err:" + ex.Message);
                return null;
            }
        }
    }
}
