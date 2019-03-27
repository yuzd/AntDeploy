#if NETSTANDARD

//
// System.Configuration.AppSettingsSection.cs
//
// Authors:
//	Duncan Mak (duncan@ximian.com)
//	Chris Toshok (toshok@ximian.com)
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

using System.ComponentModel;
using System.IO;
using System.Xml;

namespace System.Configuration
{
    public sealed class AppSettingsSection : ConfigurationSection
    {
        private static readonly ConfigurationPropertyCollection _properties;
        private static readonly ConfigurationProperty PropFile;
        private static readonly ConfigurationProperty PropSettings;

        static AppSettingsSection()
        {
            PropFile = new ConfigurationProperty("file", typeof(string), "",
                new StringConverter(), null, ConfigurationPropertyOptions.None);
            PropSettings = new ConfigurationProperty("", typeof(KeyValueConfigurationCollection), null,
                null, null, ConfigurationPropertyOptions.IsDefaultCollection);

            _properties = new ConfigurationPropertyCollection { PropFile, PropSettings };

        }

        [ConfigurationProperty("file", DefaultValue = "")]
        public string File
        {
            get { return (string)base[PropFile]; }
            set { base[PropFile] = value; }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public KeyValueConfigurationCollection Settings
        {
            get { return (KeyValueConfigurationCollection)base[PropSettings]; }
        }

        protected internal override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            /* need to do this so we pick up the File attribute */
            base.DeserializeElement(reader, serializeCollectionKey);

            if (File != "")
            {
                try
                {
                    var filePath = File;
                    if (!Path.IsPathRooted(filePath))
                        filePath = Path.Combine(Path.GetDirectoryName(Configuration.FilePath), filePath);

                    Stream s = IO.File.OpenRead(filePath);
                    var subreader = XmlReader.Create(s);
                    base.DeserializeElement(subreader, serializeCollectionKey);
                    s.Dispose();
                }
                catch
                {
                    // ignore a missing/unreadable file
                }
            }
        }

        protected internal override void Reset(ConfigurationElement parentSection)
        {
            var psec = parentSection as AppSettingsSection;
            if (psec != null)
                Settings.Reset(psec.Settings);
        }

        protected internal override object GetRuntimeObject()
        {
            var col = new KeyValueInternalCollection();

            foreach (var key in Settings.AllKeys)
            {
                var ele = Settings[key];
                col.Add(ele.Key, ele.Value);
            }

            if (!ConfigurationManager.ConfigurationSystem.SupportsUserConfig)
                col.SetReadOnly();

            return col;
        }
    }
}


#endif