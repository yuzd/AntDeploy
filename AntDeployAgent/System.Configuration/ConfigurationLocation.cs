#if NETSTANDARD

//
// System.Configuration.ConfigurationLocation.cs
//
// Authors:
//	Duncan Mak (duncan@ximian.com)
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
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//

using System.IO;
using System.Reflection;
using System.Xml;

namespace System.Configuration
{
    public class ConfigurationLocation
    {
        private static readonly char[] PathTrimChars = {'/'};

        private Configuration _parent;
        private bool _parentResolved;

        internal ConfigurationLocation()
        {
        }

        internal ConfigurationLocation(string path, string xmlContent, Configuration parent, bool allowOverride)
        {
            if (!string.IsNullOrEmpty(path))
            {
                switch (path[0])
                {
                    case ' ':
                  
                    case '/':
                    case '\\':
                        throw new ConfigurationErrorsException(
                            "<location> path attribute must be a relative virtual path.  It cannot start with any of ' ' '.' '/' or '\\'.");
                    case '.':
                        path = ConfigurationManager.TryAutoDetectConfigFile(Assembly.GetEntryAssembly());
                        break;
                }

                path = path.TrimEnd(PathTrimChars);
            }

            Path = path;
            XmlContent = xmlContent;
            _parent = parent;
            AllowOverride = allowOverride;
        }

        public string Path { get; }

        internal bool AllowOverride { get; }

        internal string XmlContent { get; private set; }

        internal Configuration OpenedConfiguration { get; private set; }

        public Configuration OpenConfiguration()
        {
            if (OpenedConfiguration == null)
            {
                if (!_parentResolved)
                {
                    var parentFile = _parent.GetParentWithFile();
                    if (parentFile != null)
                    {
                        var parentRelativePath =
                            _parent.ConfigHost.GetConfigPathFromLocationSubPath(_parent.LocationConfigPath, Path);
                        _parent = parentFile.FindLocationConfiguration(parentRelativePath, _parent);
                    }
                }

                OpenedConfiguration = new Configuration(_parent, Path);
                using (var tr = XmlReader.Create(new StringReader(XmlContent)))
                    OpenedConfiguration.ReadData(tr, AllowOverride);

                XmlContent = null;
            }
            return OpenedConfiguration;
        }

        internal void SetParentConfiguration(Configuration parent)
        {
            if (_parentResolved)
                return;

            _parentResolved = true;
            _parent = parent;
            if (OpenedConfiguration != null)
                OpenedConfiguration.Parent = parent;
        }
    }
}

#endif