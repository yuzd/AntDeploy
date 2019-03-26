#if NETSTANDARD

//
// System.Configuration.ConfigurationSectionGroupCollection.cs
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

using System.Collections;
using System.Collections.Specialized;

namespace System.Configuration
{
    public sealed class ConfigurationSectionGroupCollection : NameObjectCollectionBase
    {
        private readonly Configuration _config;
        private readonly SectionGroupInfo _group;

        internal ConfigurationSectionGroupCollection(Configuration config, SectionGroupInfo group)
            : base(StringComparer.Ordinal)
        {
            _config = config;
            _group = group;
        }

        public override KeysCollection Keys
        {
            get { return _group.Groups.Keys; }
        }

        public override int Count
        {
            get { return _group.Groups.Count; }
        }

        public ConfigurationSectionGroup this[string name]
        {
            get
            {
                var sec = BaseGet(name) as ConfigurationSectionGroup;
                if (sec == null)
                {
                    var secData = _group.Groups[name] as SectionGroupInfo;
                    if (secData == null) return null;
                    sec = _config.GetSectionGroupInstance(secData);
                    BaseSet(name, sec);
                }
                return sec;
            }
        }

        public ConfigurationSectionGroup this[int index]
        {
            get { return this[GetKey(index)]; }
        }

        public void Add(string name, ConfigurationSectionGroup sectionGroup)
        {
            _config.CreateSectionGroup(_group, name, sectionGroup);
        }

        public void Clear()
        {
            if (_group.Groups != null)
            {
                foreach (ConfigInfo data in _group.Groups)
                    _config.RemoveConfigInfo(data);
            }
        }

        public void CopyTo(ConfigurationSectionGroup[] array, int index)
        {
            for (var n = 0; n < _group.Groups.Count; n++)
                array[n + index] = this[n];
        }

        public ConfigurationSectionGroup Get(int index)
        {
            return this[index];
        }

        public ConfigurationSectionGroup Get(string name)
        {
            return this[name];
        }

        public override IEnumerator GetEnumerator()
        {
            return _group.Groups.AllKeys.GetEnumerator();
        }

        public string GetKey(int index)
        {
            return _group.Groups.GetKey(index);
        }

        public void Remove(string name)
        {
            var secData = _group.Groups[name] as SectionGroupInfo;
            if (secData != null)
                _config.RemoveConfigInfo(secData);
        }

        public void RemoveAt(int index)
        {
            var secData = _group.Groups[index] as SectionGroupInfo;
            _config.RemoveConfigInfo(secData);
        }
    }
}

#endif