#if NETSTANDARD

//
// System.Configuration.ConfigurationPropertyCollection.cs
//
// Authors:
//	Duncan Mak (duncan@ximian.com)
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
using System.Collections.Generic;

namespace System.Configuration
{
    public class ConfigurationPropertyCollection : ICollection
    {
        private readonly List<ConfigurationProperty> _collection;

        public ConfigurationPropertyCollection()
        {
            _collection = new List<ConfigurationProperty>();
        }

        public ConfigurationProperty this[string name]
        {
            get
            {
                foreach (var cp in _collection)
                    if (cp.Name == name)
                        return cp;

                return null;
            }
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return _collection; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_collection).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public void Add(ConfigurationProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            _collection.Add(property);
        }

        public bool Contains(string name)
        {
            var property = this[name];

            if (property == null)
                return false;

            return _collection.Contains(property);
        }

        public void CopyTo(ConfigurationProperty[] array, int index)
        {
            _collection.CopyTo(array, index);
        }

        public bool Remove(string name)
        {
            return _collection.Remove(this[name]);
        }

        public void Clear()
        {
            _collection.Clear();
        }
    }
}

#endif