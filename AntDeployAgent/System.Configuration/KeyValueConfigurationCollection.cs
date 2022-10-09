#if NETSTANDARD

//
// System.Configuration.KeyValueConfigurationCollection.cs
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
    [ConfigurationCollection(typeof(KeyValueConfigurationElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class KeyValueConfigurationCollection : ConfigurationElementCollection
    {
        private ConfigurationPropertyCollection _properties;

        public string[] AllKeys
        {
            get
            {
                var keys = new string[Count];
                var n = 0;
                foreach (KeyValueConfigurationElement kv in this)
                    keys[n++] = kv.Key;
                return keys;
            }
        }

        public new KeyValueConfigurationElement this[string key]
        {
            get { return (KeyValueConfigurationElement) BaseGet(key); }
        }

        protected internal override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new ConfigurationPropertyCollection();

                return _properties;
            }
        }

        protected override bool ThrowOnDuplicate
        {
            get { return false; }
        }

        public void Add(KeyValueConfigurationElement keyValue)
        {
            keyValue.Init();
            BaseAdd(keyValue);
        }

        public void Add(string key, string value)
        {
            Add(new KeyValueConfigurationElement(key, value));
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(string key)
        {
            BaseRemove(key);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyValueConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyValueConfigurationElement) element).Key;
        }
    }
}

#endif