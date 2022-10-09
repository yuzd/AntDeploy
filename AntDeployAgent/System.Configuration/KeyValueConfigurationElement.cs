﻿#if NETSTANDARD

//
// System.Configuration.KeyValueConfigurationElement.cs
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

namespace System.Configuration
{
    public class KeyValueConfigurationElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty KeyProp;
        private static readonly ConfigurationProperty ValueProp;
        private static readonly ConfigurationPropertyCollection properties;

        static KeyValueConfigurationElement()
        {
            KeyProp = new ConfigurationProperty("key", typeof(string), "", ConfigurationPropertyOptions.IsKey);
            ValueProp = new ConfigurationProperty("value", typeof(string), "");

            properties = new ConfigurationPropertyCollection {KeyProp, ValueProp};
        }

        internal KeyValueConfigurationElement()
        {
        }

        public KeyValueConfigurationElement(string key, string value)
        {
            this[KeyProp] = key;
            this[ValueProp] = value;
        }

        [ConfigurationProperty("key", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
        public string Key
        {
            get { return (string) this[KeyProp]; }
        }

        [ConfigurationProperty("value", DefaultValue = "")]
        public string Value
        {
            get { return (string) this[ValueProp]; }
            set { this[ValueProp] = value; }
        }

        protected internal override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }


        protected internal override void Init()
        {
        }
    }
}

#endif