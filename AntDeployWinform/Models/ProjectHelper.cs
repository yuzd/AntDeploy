using AntDeployWinform.Util;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace AntDeployWinform.Models
{
    internal static class ProjectHelper
    {

        public static string Culture = string.Empty;
        public static string CultureValue = string.Empty;

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
            catch (Exception ex)
            {
                return "";
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

        public static string GetCultureFromAppFile()
        {
            try
            {
                // var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

                var location = typeof(ProjectHelper).Assembly.Location;
                var fileInfo = new FileInfo(location);
                var appFileName = Path.Combine(fileInfo.DirectoryName, "AntDeploy.dll.config");
                if (!File.Exists(appFileName))
                {
                    return string.Empty;
                }

                var text = File.ReadAllLines(appFileName);
                var culture = string.Empty;
                foreach (var line in text)
                {
                    if (line.Contains("Culture"))
                    {
                        var arr = line.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 2)
                        {
                            culture = arr[1].Split('\"')[0];
                            CultureValue = $"\"{culture}\"";
                            Culture = line;
                            break;
                        }
                    }
                }

                return location;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public static bool SetCultureFromAppFile(string culture)
        {
            try
            {
                if (string.IsNullOrEmpty(Culture)) return false;

                var location = typeof(ProjectHelper).Assembly.Location;
                var fileInfo = new FileInfo(location);
                var appFileName = Path.Combine(fileInfo.DirectoryName, "AntDeploy.dll.config");
                if (!File.Exists(appFileName))
                {
                    return false;
                }

                var text = File.ReadAllText(appFileName);
                if (string.IsNullOrEmpty(text)) return false;

                text = text.Replace(Culture, Culture.Replace(CultureValue, $"\"{culture}\""));

                File.WriteAllText(appFileName, text, System.Text.Encoding.UTF8);

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