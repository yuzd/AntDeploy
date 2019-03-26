#if NETSTANDARD

//
// System.Configuration.InternalConfigurationHost.cs
//
// Authors:
//  Lluis Sanchez Gual (lluis@novell.com)
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
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//

using System.Configuration.Internal;
using System.IO;

namespace System.Configuration
{
    internal abstract class InternalConfigurationHost : IInternalConfigHost
    {
        public virtual object CreateConfigurationContext(string configPath, string locationSubPath)
        {
            return null;
        }

        public virtual object CreateDeprecatedConfigContext(string configPath)
        {
            return null;
        }

        public virtual string GetConfigPathFromLocationSubPath(string configPath, string locationSubPath)
        {
            return configPath;
        }

        public virtual Type GetConfigType(string typeName, bool throwOnError)
        {
            var type = Type.GetType(typeName.Split(',')[0]);

            // This code is in System.Configuration.dll, but some of the classes we might want to load here are in System.dll.
            if (type == null)
                type = Type.GetType(typeName + ",System");

            if (type == null && throwOnError)
                throw new ConfigurationErrorsException("Type '" + typeName + "' not found.");
            return type;
        }

        public virtual string GetConfigTypeName(Type t)
        {
            return t.AssemblyQualifiedName;
        }

        public abstract string GetStreamName(string configPath);
        public abstract void Init(IInternalConfigRoot root, params object[] hostInitParams);

        public abstract void InitForConfiguration(ref string locationSubPath, out string configPath,
            out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams);

        public virtual bool IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition,
            ConfigurationAllowExeDefinition allowExeDefinition)
        {
            if (allowDefinition == ConfigurationAllowDefinition.MachineOnly)
            {
                return false;
            }
            return true;
        }

        public virtual Stream OpenStreamForRead(string streamName)
        {
            if (!File.Exists(streamName))
                return null;

            return new FileStream(streamName, FileMode.Open, FileAccess.Read);
        }

        public virtual void VerifyDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition,
            ConfigurationAllowExeDefinition allowExeDefinition)
        {
            if (!IsDefinitionAllowed(configPath, allowDefinition, allowExeDefinition))
                throw new ConfigurationErrorsException(
                    "The section can't be defined in this file (the allowed definition context is '" + allowDefinition +
                    "').");
        }

        public virtual bool SupportsChangeNotifications
        {
            get { return false; }
        }

        public virtual bool SupportsLocation
        {
            get { return false; }
        }

        public virtual bool SupportsPath
        {
            get { return false; }
        }

        public virtual bool SupportsRefresh
        {
            get { return false; }
        }
    }

    internal class ExeConfigurationHost : InternalConfigurationHost
    {
        private ConfigurationUserLevel _level;
        private ExeConfigurationFileMap _map;

        public override void Init(IInternalConfigRoot root, params object[] hostInitParams)
        {
            _map = (ExeConfigurationFileMap)hostInitParams[0];
            _level = (ConfigurationUserLevel)hostInitParams[1];
            CheckFileMap(_level, _map);
        }

        private static void CheckFileMap(ConfigurationUserLevel level, ExeConfigurationFileMap map)
        {
            switch (level)
            {
                case ConfigurationUserLevel.None:
                    if (string.IsNullOrEmpty(map.ExeConfigFilename))
                        throw new ArgumentException(
                            "The 'ExeConfigFilename' argument cannot be null.");
                    break;
            }
        }

        public override string GetStreamName(string configPath)
        {
            return _map.ExeConfigFilename;
        }

        public override void InitForConfiguration(ref string locationSubPath, out string configPath,
            out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams)
        {
            _map = (ExeConfigurationFileMap)hostInitConfigurationParams[0];

            if (hostInitConfigurationParams.Length > 1 &&
                hostInitConfigurationParams[1] is ConfigurationUserLevel)
                _level = (ConfigurationUserLevel)hostInitConfigurationParams[1];

            CheckFileMap(_level, _map);

            configPath = null;

            locationConfigPath = null;

            if (locationSubPath == "exe" || locationSubPath == null && _map.ExeConfigFilename != null)
            {
                configPath = "exe";
                locationConfigPath = _map.ExeConfigFilename;
            }

            locationSubPath = null;
        }
    }
}

#endif