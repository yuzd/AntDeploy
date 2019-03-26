#if NETSTANDARD

//
// System.Configuration.ConfigurationSection.cs
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

namespace System.Configuration
{
    public class ConfigurationSectionGroup
    {
        private Configuration _config;
        private SectionGroupInfo _group;
        private ConfigurationSectionGroupCollection _groups;

        private bool _initialized;

        private ConfigurationSectionCollection _sections;

        private Configuration Config
        {
            get
            {
                if (_config == null)
                    throw new InvalidOperationException(
                        "ConfigurationSectionGroup cannot be edited until it is added to a Configuration instance as its descendant");
                return _config;
            }
        }


        public bool IsDeclared
        {
            get { return false; }
        }


        public bool IsDeclarationRequired { get; private set; }

        public string Name { get; private set; }

        public string SectionGroupName
        {
            get { return _group.XPath; }
        }

        public ConfigurationSectionGroupCollection SectionGroups
        {
            get
            {
                if (_groups == null) _groups = new ConfigurationSectionGroupCollection(Config, _group);
                return _groups;
            }
        }

        public ConfigurationSectionCollection Sections
        {
            get
            {
                if (_sections == null) _sections = new ConfigurationSectionCollection(Config, _group);
                return _sections;
            }
        }

        public string Type { get; set; }

        internal void Initialize(Configuration config, SectionGroupInfo group)
        {
            if (_initialized)
                throw new Exception("INTERNAL ERROR: this configuration section is being initialized twice: " +
                                    GetType());
            _initialized = true;
            _config = config;
            _group = group;
        }

        internal void SetName(string name)
        {
            Name = name;
        }


        public void ForceDeclaration(bool require)
        {
            IsDeclarationRequired = require;
        }

        public void ForceDeclaration()
        {
            ForceDeclaration(true);
        }
    }
}

#endif