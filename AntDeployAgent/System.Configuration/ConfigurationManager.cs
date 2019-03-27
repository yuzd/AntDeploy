#if NETSTANDARD

//
// System.Configuration.ConfigurationManager.cs
//
// Authors:
//	Duncan Mak (duncan@ximian.com)
// 	Lluis Sanchez Gual (lluis@novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//

using System.Collections.Specialized;
using System.Configuration.Internal;
using System.IO;
using System.Reflection;

namespace System.Configuration
{
    public static class ConfigurationManager
    {
        private static readonly InternalConfigurationFactory ConfigFactory = new InternalConfigurationFactory();

        internal static IInternalConfigConfigurationFactory ConfigurationFactory
        {
            get { return ConfigFactory; }
        }

        internal static IInternalConfigSystem ConfigurationSystem { get; private set; } =
            new ClientConfigurationSystem();

        public static NameValueCollection AppSettings
        {
            get { return (NameValueCollection)GetSection("appSettings"); }
        }

        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                var connectionStrings = (ConnectionStringsSection)GetSection("connectionStrings");
                return connectionStrings.ConnectionStrings;
            }
        }

        /// <summary>
        /// Initialize explicitly the configuration manager with the specified file.
        /// </summary>
        /// <param name="exePath"></param>
        public static void Initialize(string exePath)
        {
            if (string.IsNullOrEmpty(exePath))
            {
                throw new ArgumentException("A valid path must be supplied", "exePath");
            }
            if (!Path.IsPathRooted(exePath))
                exePath = Path.GetFullPath(exePath);
            if (!File.Exists(exePath))
            {
                Exception cause = new ArgumentException("The specified path does not exist.", "exePath");
                throw new ConfigurationErrorsException("Error Initializing the configuration system:", cause);
            }

            if (!exePath.EndsWith(".config"))
            {
                exePath += ".config";
            }

            ConfigurationSystem = new ClientConfigurationSystem(exePath);
        }

        /// <summary>
        /// Reset the configuration manager to its original state prior to Initialize().
        /// </summary>
        public static void Reset()
        {
            ConfigurationSystem = new ClientConfigurationSystem();
        }

        internal static Configuration OpenExeConfigurationInternal(ConfigurationUserLevel userLevel,
            Assembly callingAssembly, string exePath)
        {
            var map = new ExeConfigurationFileMap();

            if (string.IsNullOrEmpty(exePath))
            {
                exePath = TryAutoDetectConfigFile(callingAssembly);
                if (exePath == null)
                {
                    throw new ArgumentException("A valid path must be supplied", "exePath");
                }
            }

            if (!Path.IsPathRooted(exePath))
                exePath = Path.GetFullPath(exePath);
            if (!File.Exists(exePath))
            {
                Exception cause = new ArgumentException("The specified path does not exist.", "exePath");
                throw new ConfigurationErrorsException("Error Initializing the configuration system:", cause);
            }
            if (!exePath.EndsWith(".config"))
            {
                map.ExeConfigFilename = exePath + ".config";
            }
            else
            {
                map.ExeConfigFilename = exePath;
            }

            return ConfigurationFactory.Create(typeof(ExeConfigurationHost), map, userLevel);
        }

        public static Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
        {
            return OpenExeConfigurationInternal(userLevel, Assembly.GetEntryAssembly(), null);
        }

        public static Configuration OpenExeConfiguration(string exePath)
        {
            return OpenExeConfigurationInternal(ConfigurationUserLevel.None, Assembly.GetEntryAssembly(), exePath);
        }

        public static Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap,
            ConfigurationUserLevel userLevel)
        {
            return ConfigurationFactory.Create(typeof(ExeConfigurationHost), fileMap, userLevel);
        }

        public static object GetSection(string sectionName)
        {
            var o = ConfigurationSystem.GetSection(sectionName);
            if (o is ConfigurationSection)
                return ((ConfigurationSection)o).GetRuntimeObject();
            return o;
        }

        public static void RefreshSection(string sectionName)
        {
            ConfigurationSystem.RefreshConfig(sectionName);
        }

        public static string TryAutoDetectConfigFile(Assembly callingAssembly)
        {
            var appDirectoryFromContext = AppContext.BaseDirectory;
            var firstTry = TryAutoDetectConfigFile(appDirectoryFromContext, callingAssembly);
            if (firstTry != null)
            {
                return firstTry;
            }

            var appDirectoryFromAssembly = Path.GetDirectoryName(callingAssembly.Location);
            return TryAutoDetectConfigFile(appDirectoryFromAssembly, callingAssembly);
        }

        private static string TryAutoDetectConfigFile(string appDirectory, Assembly callingAssembly)
        {
            var exeConfigFile = Path.Combine(appDirectory, callingAssembly.GetName().Name + ".exe.config");
            if (File.Exists(exeConfigFile))
            {
                return exeConfigFile;
            }

            var appConfigFile = Path.Combine(appDirectory, "App.config");
            if (File.Exists(appConfigFile))
            {
                return appConfigFile;
            }

            var webConfigFile = Path.Combine(appDirectory, "Web.config");
            if (File.Exists(webConfigFile))
            {
                return webConfigFile;
            }

            return null;
        }
    }
}

#endif