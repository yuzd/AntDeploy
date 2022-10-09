﻿#if NETSTANDARD

//
// System.Configuration.NameValueConfigurationElement.cs
//
// Authors:
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

namespace System.Configuration
{
    public sealed class NameValueConfigurationElement : ConfigurationElement
    {
        private static readonly ConfigurationPropertyCollection _properties;
        private static readonly ConfigurationProperty PropName;
        private static readonly ConfigurationProperty PropValue;

        static NameValueConfigurationElement()
        {
            _properties = new ConfigurationPropertyCollection();

            PropName = new ConfigurationProperty("name", typeof(string), "", ConfigurationPropertyOptions.IsKey);
            PropValue = new ConfigurationProperty("value", typeof(string), "");

            _properties.Add(PropName);
            _properties.Add(PropValue);
        }

        public NameValueConfigurationElement(string name, string value)
        {
            this[PropName] = name;
            this[PropValue] = value;
        }

        [ConfigurationProperty("name", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
        public string Name
        {
            get { return (string) this[PropName]; }
        }

        [ConfigurationProperty("value", DefaultValue = "", Options = ConfigurationPropertyOptions.None)]
        public string Value
        {
            get { return (string) this[PropValue]; }
            set { this[PropValue] = value; }
        }

        protected internal override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }
    }
}

#endif