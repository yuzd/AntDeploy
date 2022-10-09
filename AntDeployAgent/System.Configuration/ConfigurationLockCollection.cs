#if NETSTANDARD

//
// System.Configuration.ConfigurationLockCollection.cs
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
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//

using System.Collections;

namespace System.Configuration
{
    [Flags]
    internal enum ConfigurationLockType
    {
        Attribute = 0x01,
        Element = 0x02,

        Exclude = 0x10
    }

    public sealed class ConfigurationLockCollection : ICollection
    {
        private readonly ConfigurationElement _element;
        private readonly ConfigurationLockType _lockType;
        private readonly ArrayList _names;
        private Hashtable _validNameHash;
        private string _validNames;

        internal ConfigurationLockCollection(ConfigurationElement element,
            ConfigurationLockType lockType)
        {
            _names = new ArrayList();
            _element = element;
            _lockType = lockType;
        }

        public string AttributeList
        {
            get
            {
                var nameArr = new string[_names.Count];
                _names.CopyTo(nameArr, 0);
                return string.Join(",", nameArr);
            }
        }


        public bool HasParentElements
        {
            get { return false; /* XXX */ }
        }

        public IEnumerator GetEnumerator()
        {
            return _names.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            _names.CopyTo(array, index);
        }

        public int Count
        {
            get { return _names.Count; }
        }


        public bool IsSynchronized
        {
            get { return false; /* XXX */ }
        }


        public object SyncRoot
        {
            get { return this; /* XXX */ }
        }

        private void CheckName(string name)
        {
            var isAttribute = (_lockType & ConfigurationLockType.Attribute) == ConfigurationLockType.Attribute;

            if (_validNameHash == null)
            {
                _validNameHash = new Hashtable();
                foreach (ConfigurationProperty prop in _element.Properties)
                {
                    if (isAttribute == prop.IsElement)
                        continue;
                    _validNameHash.Add(prop.Name, true);
                }

                /* add the add/remove/clear names of the
				 * default collection if there is one */
                if (!isAttribute)
                {
                    var c = _element.GetDefaultCollection();
                    _validNameHash.Add(c.AddElementName, true);
                    _validNameHash.Add(c.ClearElementName, true);
                    _validNameHash.Add(c.RemoveElementName, true);
                }

                var validNameArray = new string[_validNameHash.Keys.Count];
                _validNameHash.Keys.CopyTo(validNameArray, 0);

                _validNames = string.Join(",", validNameArray);
            }

            if (_validNameHash[name] == null)
                throw new ConfigurationErrorsException(
                    string.Format(
                        "The {2} '{0}' is not valid in the locked list for this section.  The following {3} can be locked: '{1}'",
                        name, _validNames, isAttribute ? "attribute" : "element",
                        isAttribute ? "attributes" : "elements"));
        }

        public void Add(string name)
        {
            CheckName(name);
            if (!_names.Contains(name))
            {
                _names.Add(name);
            }
        }

        public void Clear()
        {
            _names.Clear();
        }

        public bool Contains(string name)
        {
            return _names.Contains(name);
        }

        public void CopyTo(string[] array, int index)
        {
            _names.CopyTo(array, index);
        }

        public bool IsReadOnly(string name)
        {
            for (var i = 0; i < _names.Count; i ++)
            {
                if ((string) _names[i] == name)
                {
                    /* this test used to switch off whether the collection was 'Exclude' or not
					 * (the LockAll*Except collections), but that doesn't seem to be the crux of
					 * it.  maybe this returns true if the element/attribute is locked in a parent
					 * element's lock collections? */
                    return false;
                }
            }

            throw new ConfigurationErrorsException(string.Format("The entry '{0}' is not in the collection.", name));
        }

        public void Remove(string name)
        {
            _names.Remove(name);
        }

        public void SetFromList(string attributeList)
        {
            Clear();

            char[] split = {','};
            var attrs = attributeList.Split(split);
            foreach (var a in attrs)
            {
                Add(a.Trim());
            }
        }
    }
}

#endif