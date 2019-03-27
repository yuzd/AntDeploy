#if NETSTANDARD

//
// System.Configuration.SectionInformation.cs
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
    public sealed class SectionInformation
    {
        private string _configSource = string.Empty;
        private ConfigurationSection _parent;
        private string _rawXml;
        private string _typeName;

        internal SectionInformation()
        {
            AllowDefinition = ConfigurationAllowDefinition.Everywhere;
            AllowLocation = true;
            AllowOverride = true;
            InheritInChildApplications = true;
            RestartOnExternalChanges = true;
        }

        internal string ConfigFilePath { get; set; }

        public ConfigurationAllowDefinition AllowDefinition { get; set; } = ConfigurationAllowDefinition.Everywhere;

        public ConfigurationAllowExeDefinition AllowExeDefinition { get; set; } =
            ConfigurationAllowExeDefinition.MachineToApplication;

        public bool AllowLocation { get; set; }

        public bool AllowOverride { get; set; }

        public string ConfigSource
        {
            get { return _configSource; }
            set
            {
                if (value == null)
                    value = string.Empty;

                _configSource = value;
            }
        }

        public bool InheritInChildApplications { get; set; }

        public bool IsDeclarationRequired
        {
            get { throw new NotImplementedException(); }
        }


        public bool IsDeclared
        {
            get { return false; }
        }


        public bool IsLocked
        {
            get { return false; }
        }

        public string Name { get; private set; }

        public bool RequirePermission { get; set; }


        public bool RestartOnExternalChanges { get; set; }


        public string SectionName
        {
            get { return Name; }
        }

        public string Type
        {
            get { return _typeName; }
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException("Value cannot be null or empty.");

                _typeName = value;
            }
        }

        public ConfigurationSection GetParentSection()
        {
            return _parent;
        }

        internal void SetParentSection(ConfigurationSection parent)
        {
            _parent = parent;
        }

        public string GetRawXml()
        {
            return _rawXml;
        }


        public void ForceDeclaration(bool require)
        {
        }

        public void ForceDeclaration()
        {
            ForceDeclaration(true);
        }


        public void RevertToParent()
        {
            throw new NotImplementedException();
        }

        public void SetRawXml(string xml)
        {
            _rawXml = xml;
        }


        internal void SetName(string name)
        {
            Name = name;
        }
    }
}

#endif