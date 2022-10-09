#if NETSTANDARD

//
// System.Configuration.Configuration.cs
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

using System.Collections;
using System.Configuration.Internal;
using System.Configuration.System.Configuration;
using System.IO;
using System.Xml;

namespace System.Configuration
{
    // For configuration document, use this XmlDocument instead of the standard one. This ignores xmlns attribute for MS.
    internal class ConfigurationXmlDocument : XmlDocument
    {
        public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
        {
            if (namespaceUri == "http://schemas.microsoft.com/.NetConfiguration/v2.0")
                return base.CreateElement(string.Empty, localName, string.Empty);
            return base.CreateElement(prefix, localName, namespaceUri);
        }
    }

    public sealed class Configuration
    {
        private readonly Hashtable _elementData = new Hashtable();
        private readonly string _locationConfigPath;
        private readonly string _locationSubPath;

        private string _configPath;

        private ContextInformation _evaluationContext;
        private ConfigurationLocationCollection _locations;
        private SectionGroupInfo _rootGroup;
        private string _rootNamespace;
        private ConfigurationSectionGroup _rootSectionGroup;
        private IConfigSystem _system;

        internal Configuration(Configuration parent, string locationSubPath)
        {
            Parent = parent;
            _system = parent._system;
            _rootGroup = parent._rootGroup;
            _locationSubPath = locationSubPath;
            _configPath = parent.ConfigPath;
        }

        internal Configuration(InternalConfigurationSystem system, string locationSubPath)
        {
            HasFile = true;
            _system = system;

            system.InitForConfiguration(ref locationSubPath, out _configPath, out _locationConfigPath);

            Configuration parent = null;

            if (locationSubPath != null)
            {
                parent = new Configuration(system, locationSubPath);
                if (_locationConfigPath != null)
                    parent = parent.FindLocationConfiguration(_locationConfigPath, parent);
            }

            Init(system, _configPath, parent);
        }

        internal Configuration Parent { get; set; }

        internal string FileName { get; private set; }

        internal IInternalConfigHost ConfigHost
        {
            get { return _system.Host; }
        }

        internal string LocationConfigPath
        {
            get { return _locationConfigPath; }
        }

        internal string ConfigPath
        {
            get { return _configPath; }
        }

        public AppSettingsSection AppSettings
        {
            get { return (AppSettingsSection)GetSection("appSettings"); }
        }

        public ConnectionStringsSection ConnectionStrings
        {
            get { return (ConnectionStringsSection)GetSection("connectionStrings"); }
        }

        // MSDN: If the value for this FilePath property represents a merged view and 
        // no actual file exists for the application, the path to the parent configuration 
        // file is returned.
        public string FilePath
        {
            get
            {
                if (FileName == null && Parent != null)
                    return Parent.FilePath;
                return FileName;
            }
        }

        public bool HasFile { get; }

        public ContextInformation EvaluationContext
        {
            get
            {
                if (_evaluationContext == null)
                {
                    var ctx = _system.Host.CreateConfigurationContext(_configPath, GetLocationSubPath());
                    _evaluationContext = new ContextInformation(this, ctx);
                }


                return _evaluationContext;
            }
        }

        public ConfigurationLocationCollection Locations
        {
            get
            {
                if (_locations == null) _locations = new ConfigurationLocationCollection();
                return _locations;
            }
        }

        public bool NamespaceDeclared
        {
            get { return _rootNamespace != null; }
            set { _rootNamespace = value ? "http://schemas.microsoft.com/.NetConfiguration/v2.0" : null; }
        }

        public ConfigurationSectionGroup RootSectionGroup
        {
            get
            {
                if (_rootSectionGroup == null)
                {
                    _rootSectionGroup = new ConfigurationSectionGroup();
                    _rootSectionGroup.Initialize(this, _rootGroup);
                }
                return _rootSectionGroup;
            }
        }

        public ConfigurationSectionGroupCollection SectionGroups
        {
            get { return RootSectionGroup.SectionGroups; }
        }

        public ConfigurationSectionCollection Sections
        {
            get { return RootSectionGroup.Sections; }
        }

        internal Configuration FindLocationConfiguration(string relativePath, Configuration defaultConfiguration)
        {
            var parentConfig = defaultConfiguration;

            if (!string.IsNullOrEmpty(LocationConfigPath))
            {
                var parentFile = GetParentWithFile();
                if (parentFile != null)
                {
                    var parentRelativePath = _system.Host.GetConfigPathFromLocationSubPath(_configPath, relativePath);
                    parentConfig = parentFile.FindLocationConfiguration(parentRelativePath, defaultConfiguration);
                }
            }

            var relConfigPath = _configPath.Substring(1) + "/";
            if (relativePath.StartsWith(relConfigPath, StringComparison.Ordinal))
                relativePath = relativePath.Substring(relConfigPath.Length);

            var loc = Locations.FindBest(relativePath);
            if (loc == null)
                return parentConfig;

            loc.SetParentConfiguration(parentConfig);
            return loc.OpenConfiguration();
        }

        internal void Init(IConfigSystem system, string configPath, Configuration parent)
        {
            _system = system;
            _configPath = configPath;
            FileName = system.Host.GetStreamName(configPath);
            Parent = parent;
            if (parent != null)
                _rootGroup = parent._rootGroup;
            else
            {
                _rootGroup = new SectionGroupInfo { StreamName = FileName };

                // Addd Machine.config sections
                foreach (var keyValuePair in MachineConfig.ConfigSections)
                {
                    var sectionInfo = CreateSectionInfo(system, configPath, keyValuePair.Key, keyValuePair.Value);
                    _rootGroup.AddChild(sectionInfo);
                }
            }

            try
            {
                if (FileName != null)
                    Load();
            }
            catch (XmlException ex)
            {
                throw new ConfigurationErrorsException(ex.Message, ex, FileName, 0);
            }
        }

        private SectionInfo CreateSectionInfo(IConfigSystem system, string configPath, string sectionName, string sectionType)
        {
            var sectionInformation = new SectionInformation
            {
                Type = sectionType,
                ConfigFilePath = configPath
            };
            sectionInformation.SetName(sectionName);

            var sectionInfo = new SectionInfo(sectionName, sectionInformation)
            {
                StreamName = FileName,
                ConfigHost = system.Host
            };

            return sectionInfo;
        }

        internal Configuration GetParentWithFile()
        {
            var parentFile = Parent;
            while (parentFile != null && !parentFile.HasFile)
                parentFile = parentFile.Parent;
            return parentFile;
        }

        internal string GetLocationSubPath()
        {
            var confg = Parent;
            string path = null;
            while (confg != null)
            {
                path = confg._locationSubPath;
                if (!string.IsNullOrEmpty(path))
                    return path;
                confg = confg.Parent;
            }
            return path;
        }

        public ConfigurationSection GetSection(string path)
        {
            var parts = path.Split('/');
            if (parts.Length == 1)
                return Sections[parts[0]];

            var group = SectionGroups[parts[0]];
            for (var n = 1; group != null && n < parts.Length - 1; n++)
                group = group.SectionGroups[parts[n]];

            if (group != null)
                return group.Sections[parts[parts.Length - 1]];
            return null;
        }

        public ConfigurationSectionGroup GetSectionGroup(string path)
        {
            var parts = path.Split('/');
            var group = SectionGroups[parts[0]];
            for (var n = 1; group != null && n < parts.Length; n++)
                group = group.SectionGroups[parts[n]];
            return group;
        }

        internal ConfigurationSection GetSectionInstance(SectionInfo config, bool createDefaultInstance)
        {
            object data = _elementData[config];
            var sec = data as ConfigurationSection;
            if (sec != null || !createDefaultInstance) return sec;

            var secObj = config.CreateInstance();
            sec = secObj as ConfigurationSection;
            if (sec == null)
            {
                var ds = new DefaultSection();
                ds.SectionHandler = secObj as IConfigurationSectionHandler;
                sec = ds;
            }
            sec.Configuration = this;

            ConfigurationSection parentSection = null;
            if (Parent != null)
            {
                parentSection = Parent.GetSectionInstance(config, true);
                sec.SectionInformation.SetParentSection(parentSection);
            }
            sec.SectionInformation.ConfigFilePath = FilePath;

            sec.ConfigContext = _system.Host.CreateDeprecatedConfigContext(_configPath);

            var xml = data as string;
            sec.RawXml = xml;
            sec.Reset(parentSection);

            if (xml != null)
            {
                using (var r = XmlReader.Create(new StringReader(xml)))
                {
                    sec.DeserializeSection(r);
                }

                if (!string.IsNullOrEmpty(sec.SectionInformation.ConfigSource) && !string.IsNullOrEmpty(FilePath))
                    sec.DeserializeConfigSource(Path.GetDirectoryName(FilePath));
            }

            _elementData[config] = sec;
            return sec;
        }

        internal ConfigurationSectionGroup GetSectionGroupInstance(SectionGroupInfo group)
        {
            var gr = group.CreateInstance() as ConfigurationSectionGroup;
            if (gr != null) gr.Initialize(this, group);
            return gr;
        }

        internal void SetConfigurationSection(SectionInfo config, ConfigurationSection sec)
        {
            _elementData[config] = sec;
        }

        internal void SetSectionXml(SectionInfo config, string data)
        {
            _elementData[config] = data;
        }

        internal string GetSectionXml(SectionInfo config)
        {
            return _elementData[config] as string;
        }

        internal void CreateSection(SectionGroupInfo group, string name, ConfigurationSection sec)
        {
            if (group.HasChild(name))
                throw new ConfigurationErrorsException(
                    "Cannot add a ConfigurationSection. A section or section group already exists with the name '" +
                    name + "'");

            if (!HasFile && !sec.SectionInformation.AllowLocation)
                throw new ConfigurationErrorsException("The configuration section <" + name +
                                                       "> cannot be defined inside a <location> element.");

            if (
                !_system.Host.IsDefinitionAllowed(_configPath, sec.SectionInformation.AllowDefinition,
                    sec.SectionInformation.AllowExeDefinition))
            {
                var ctx = sec.SectionInformation.AllowExeDefinition !=
                          ConfigurationAllowExeDefinition.MachineToApplication
                    ? sec.SectionInformation.AllowExeDefinition
                    : (object)sec.SectionInformation.AllowDefinition;
                throw new ConfigurationErrorsException("The section <" + name +
                                                       "> can't be defined in this configuration file (the allowed definition context is '" +
                                                       ctx + "').");
            }

            if (sec.SectionInformation.Type == null)
                sec.SectionInformation.Type = _system.Host.GetConfigTypeName(sec.GetType());

            var section = new SectionInfo(name, sec.SectionInformation)
            {
                StreamName = FileName,
                ConfigHost = _system.Host
            };
            group.AddChild(section);
            _elementData[section] = sec;
            sec.Configuration = this;
        }

        internal void CreateSectionGroup(SectionGroupInfo parentGroup, string name, ConfigurationSectionGroup sec)
        {
            if (parentGroup.HasChild(name))
                throw new ConfigurationErrorsException(
                    "Cannot add a ConfigurationSectionGroup. A section or section group already exists with the name '" +
                    name + "'");
            if (sec.Type == null) sec.Type = _system.Host.GetConfigTypeName(sec.GetType());
            sec.SetName(name);

            var section = new SectionGroupInfo(name, sec.Type)
            {
                StreamName = FileName,
                ConfigHost = _system.Host
            };
            parentGroup.AddChild(section);
            _elementData[section] = sec;

            sec.Initialize(this, section);
        }

        internal void RemoveConfigInfo(ConfigInfo config)
        {
            _elementData.Remove(config);
        }

        private void ResetModified()
        {
            foreach (ConfigurationLocation loc in Locations)
            {
                if (loc.OpenedConfiguration == null)
                    continue;
                loc.OpenedConfiguration.ResetModified();
            }

            _rootGroup.ResetModified(this);
        }

        private void Load()
        {
            if (string.IsNullOrEmpty(FileName))
                return;

            Stream stream;
            try
            {
                stream = _system.Host.OpenStreamForRead(FileName);
                if (stream == null)
                    return;
            }
            catch
            {
                return;
            }

            using (var reader = XmlReader.Create(stream))
            {
                ReadConfigFile(reader, FileName);
            }
            ResetModified();
        }

        private void ReadConfigFile(XmlReader reader, string fileName)
        {
            reader.MoveToContent();

            if (reader.NodeType != XmlNodeType.Element || reader.Name != "configuration")
                ThrowException("Configuration file does not have a valid root element", reader);

            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.LocalName == "xmlns")
                    {
                        _rootNamespace = reader.Value;
                        continue;
                    }
                    ThrowException(string.Format("Unrecognized attribute '{0}' in root element", reader.LocalName),
                        reader);
                }
            }

            reader.MoveToElement();

            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            if (reader.LocalName == "configSections")
            {
                if (reader.HasAttributes)
                    ThrowException("Unrecognized attribute in <configSections>.", reader);

                _rootGroup.ReadConfig(this, fileName, reader);
            }

            _rootGroup.ReadRootData(reader, this, true);
        }

        internal void ReadData(XmlReader reader, bool allowOverride)
        {
            _rootGroup.ReadData(this, reader, allowOverride);
        }


        private void ThrowException(string text, XmlReader reader)
        {
            var li = reader as IXmlLineInfo;
            throw new ConfigurationErrorsException(text, FileName, li != null ? li.LineNumber : 0);
        }
    }
}

#endif