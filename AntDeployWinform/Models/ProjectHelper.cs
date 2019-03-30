using AntDeployWinform.Util;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AntDeployWinform.Models
{
    internal static class ProjectHelper
    {


        /// <summary>
        /// 获取Windows服务的名称
        /// </summary>
        /// <param name="serviceFileName">文件路径</param>
        /// <returns>服务名称</returns>
        public static string GetServiceNameByFile(string serviceFileName)
        {
            try
            {

                Assembly assembly = Assembly.LoadFrom(serviceFileName);
                Type[] types = assembly.GetTypes();
                foreach (Type myType in types)
                {
                    if (myType.IsClass && myType.BaseType == typeof(System.Configuration.Install.Installer))
                    {
                        FieldInfo[] fieldInfos = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static);
                        foreach (FieldInfo myFieldInfo in fieldInfos)
                        {
                            if (myFieldInfo.FieldType == typeof(System.ServiceProcess.ServiceInstaller))
                            {
                                var re = "";
                                var obj = Activator.CreateInstance(myType);
                                using (ServiceInstaller serviceInstaller = (ServiceInstaller)myFieldInfo.GetValue(obj))
                                {
                                    re = serviceInstaller.ServiceName;
                                }

                                var dis = obj as IDisposable;
                                if (dis != null)
                                {
                                    dis.Dispose();
                                }
                                return re;
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }


        public static ProjectParam GetNetCoreParamInCsprojectFile(string projectPath)
        {
            try
            {
                var project = new ProjectParam();
                var info = File.ReadAllLines(projectPath);
                var firstLine = info.FirstOrDefault();
                if (!string.IsNullOrEmpty(firstLine))
                {
                    project.IsNetcorePorject = firstLine.Contains("Sdk=\"Microsoft.NET.Sdk.Web\"") || firstLine.Contains("Sdk=\"Microsoft.NET.Sdk\"");
                }

                var assembly = info.FirstOrDefault(r => r.Contains("<AssemblyName>") && r.Contains("</AssemblyName>"));
                if (!string.IsNullOrEmpty(assembly))
                {
                    project.OutPutName = assembly.Split(new string[] {"<AssemblyName>"}, StringSplitOptions.None)[1]
                        .Split(new string[]{ "<" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (project.IsNetcorePorject)
                    {
                        project.OutPutName += ".dll";
                    }
                    else
                    {
                        project.OutPutName += ".exe";
                    }
                }

                var framework = info.FirstOrDefault(r => r.Contains("TargetFramework>"));
                if (!string.IsNullOrEmpty(framework))
                {
                    var TargetFramework = framework.Split(new string[] { "TargetFramework>" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "<" }, StringSplitOptions.None)[0];
                    var version = Regex.Replace(TargetFramework, "[a-zA-Z]+", "").Trim();
                    var temp = version.Replace(".", "");
                    if (int.TryParse(temp, out _))
                    {
                        project.NetCoreSDKVersion = version;
                    }
                }

                return project;
            }
            catch (Exception)
            {
                return new ProjectParam();
            }
        }

        /// <summary>
        /// 检查工程文件里面是否含有 WebProjectProperties字样
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static bool IsWebProject(string projectPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(projectPath) && File.Exists(projectPath))
                {
                    var fileInfo = File.ReadAllText(projectPath);
                    if (fileInfo.Contains("<WebProjectProperties>") && fileInfo.Contains("</WebProjectProperties>"))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static string GetProjectSkdInNetCoreProject(string projectPath)
        {
            try
            {
                var info = File.ReadAllText(projectPath);
                var TargetFramework = info.Split(new string[] { "TargetFramework>" }, StringSplitOptions.None)[1]
                    .Split(new string[] { "<" }, StringSplitOptions.None)[0];
                var version = Regex.Replace(TargetFramework, "[a-zA-Z]+", "").Trim();
                var temp = version.Replace(".", "");
                if (int.TryParse(temp, out _))
                {
                    return version;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public static string GetPluginInstallPath()
        {
            try
            {
                var consoleAssemblyLocation = new Uri(typeof(ProjectHelper).Assembly.CodeBase);
                var file = new FileInfo(consoleAssemblyLocation.LocalPath);
                if (file.Exists)
                {
                    return file.Directory.FullName;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetPluginConfigPath(string projectName = null)
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var folderName = Path.Combine(path, "AntDeploy");
                if (!string.IsNullOrEmpty(folderName))
                {
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    return Path.Combine(folderName, string.IsNullOrEmpty(projectName) ? "AntDeploy.json" : CodingHelper.MD5(projectName) + ".json");
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public static string GetCultureFromAppFileNew()
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                var se = config.AppSettings.Settings["Culture"];
                return se.Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static bool SetCultureFromAppFileNew(string culture)
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                config.AppSettings.Settings["Culture"].Value = culture;
                config.Save(ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public const string SolutionItemsFolder = "Solution Items";


        public static string GetDefaultDestinationFilename(string fileName)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(fileName);
            string extension = "cs";
            return Path.ChangeExtension(baseFileName, extension);
        }




        public static string FixAbsolutePath(string absolutePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                return absolutePath;

            var uniformlySeparated = absolutePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var doubleSlash = new string(Path.DirectorySeparatorChar, 2);
            var prependSeparator = uniformlySeparated.StartsWith(doubleSlash, StringComparison.Ordinal);
            uniformlySeparated = uniformlySeparated.Replace(doubleSlash, new string(Path.DirectorySeparatorChar, 1));

            if (prependSeparator)
                uniformlySeparated = Path.DirectorySeparatorChar + uniformlySeparated;

            return uniformlySeparated;
        }

    }
}