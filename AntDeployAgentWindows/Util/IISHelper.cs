using Microsoft.Web.Administration;
using System;
using System.Linq;

namespace AntDeployAgentWindows.Util
{
    public static class IISHelper
    {
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


        public static bool IsDefaultWebSite(string name)
        {
            return name.ToLower().Equals("default web site");
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
