#if NETSTANDARD

//
// System.Configuration.ConfigurationSection.cs
//
// Authors:
//	Duncan Mak (duncan@ximian.com)
//	Lluis Sanchez Gual (lluis@novell.com)
//	Martin Baulig <martin.baulig@xamarin.com>
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
// Copyright (c) 2012 Xamarin Inc. (http://www.xamarin.com)
//

using System.IO;
using System.Xml;

namespace System.Configuration
{
    public abstract class ConfigurationSection : ConfigurationElement
    {
        private SectionInformation _sectionInformation;

        internal string ExternalDataXml { get; private set; }

        internal IConfigurationSectionHandler SectionHandler { get; set; }


        public SectionInformation SectionInformation
        {
            get
            {
                if (_sectionInformation == null)
                    _sectionInformation = new SectionInformation();
                return _sectionInformation;
            }
        }

        internal object ConfigContext { get; set; }

        protected internal virtual object GetRuntimeObject()
        {
            if (SectionHandler != null)
            {
                var parentSection = _sectionInformation != null ? _sectionInformation.GetParentSection() : null;
                var parent = parentSection != null ? parentSection.GetRuntimeObject() : null;
                if (RawXml == null)
                    return parent;

                try
                {
                    // This code requires some re-thinking...
                    var reader = XmlReader.Create(new StringReader(RawXml));

                    DoDeserializeSection(reader);

                    if (!string.IsNullOrEmpty(SectionInformation.ConfigSource))
                    {
                        var fileDir = SectionInformation.ConfigFilePath;
                        if (!string.IsNullOrEmpty(fileDir))
                            fileDir = Path.GetDirectoryName(fileDir);
                        else
                            fileDir = string.Empty;

                        string path = Path.Combine(fileDir, SectionInformation.ConfigSource);
                        if (File.Exists(path))
                        {
                            RawXml = File.ReadAllText(path);
                            SectionInformation.SetRawXml(RawXml);
                        }
                    }
                }
                catch
                {
                    // ignore, it can fail - we deserialize only in order to get
                    // the configSource attribute
                }
                XmlDocument doc = new ConfigurationXmlDocument();
                doc.LoadXml(RawXml);
                return SectionHandler.Create(parent, ConfigContext, doc.DocumentElement);
            }
            return this;
        }

        private void DoDeserializeSection(XmlReader reader)
        {
            reader.MoveToContent();

            string configSource = null;

            while (reader.MoveToNextAttribute())
            {
                var localName = reader.LocalName;
                if (localName == "configSource")
                    configSource = reader.Value;
            }

            if (configSource != null)
                SectionInformation.ConfigSource = configSource;

            SectionInformation.SetRawXml(RawXml);
            if (SectionHandler == null)
                DeserializeElement(reader, false);
        }

        protected internal virtual void DeserializeSection(XmlReader reader)
        {
            try
            {
                DoDeserializeSection(reader);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Error deserializing configuration section {0}: {1}", SectionInformation.Name,
                        ex.Message));
            }
        }

        internal void DeserializeConfigSource(string basePath)
        {
            var configSource = SectionInformation.ConfigSource;

            if (string.IsNullOrEmpty(configSource))
                return;

            if (Path.IsPathRooted(configSource))
                throw new ConfigurationErrorsException("The configSource attribute must be a relative physical path.");

            if (HasLocalModifications())
                throw new ConfigurationErrorsException(
                    "A section using 'configSource' may contain no other attributes or elements.");

            string path = Path.Combine(basePath, configSource);
            if (!File.Exists(path))
            {
                RawXml = null;
                SectionInformation.SetRawXml(null);
                throw new ConfigurationErrorsException(string.Format("Unable to open configSource file '{0}'.", path));
            }

            RawXml = File.ReadAllText(path);
            SectionInformation.SetRawXml(RawXml);
            DeserializeElement(XmlReader.Create(new StringReader(RawXml)), false);
        }
    }
}

#endif