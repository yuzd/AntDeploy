#if NETSTANDARD

//
// System.Configuration.ConfigurationSectionCollection.cs
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
    public sealed class ConfigurationSectionCollection : NameObjectCollectionBase
    {
        private static readonly object LockObject = new object();
        private readonly Configuration _config;
        private readonly SectionGroupInfo _group;

        internal ConfigurationSectionCollection(Configuration config, SectionGroupInfo group)
            : base(StringComparer.Ordinal)
        {
            _config = config;
            _group = group;
        }

        public override KeysCollection Keys
        {
            get { return _group.Sections.Keys; }
        }

        public override int Count
        {
            get { return _group.Sections.Count; }
        }

        public ConfigurationSection this[string name]
        {
            get
            {
                var sec = BaseGet(name) as ConfigurationSection;
                if (sec == null)
                {
                    var secData = _group.Sections[name] as SectionInfo;
                    if (secData == null) return null;
                    sec = _config.GetSectionInstance(secData, true);
                    if (sec == null) return null;
                    lock (LockObject)
                    {
                        BaseSet(name, sec);
                    }
                }
                return sec;
            }
        }

        public ConfigurationSection this[int index]
        {
            get { return this[GetKey(index)]; }
        }

        public void Add(string name, ConfigurationSection section)
        {
            _config.CreateSection(_group, name, section);
        }

        public void Clear()
        {
            if (_group.Sections != null)
            {
                foreach (ConfigInfo data in _group.Sections)
                    _config.RemoveConfigInfo(data);
            }
        }

        public void CopyTo(ConfigurationSection[] array, int index)
        {
            for (var n = 0; n < _group.Sections.Count; n++)
                array[n + index] = this[n];
        }

        public ConfigurationSection Get(int index)
        {
            return this[index];
        }

        public ConfigurationSection Get(string name)
        {
            return this[name];
        }

        public override IEnumerator GetEnumerator()
        {
            foreach (string key in _group.Sections.AllKeys)
                yield return this[key];
        }

        public string GetKey(int index)
        {
            return _group.Sections.GetKey(index);
        }

        public void Remove(string name)
        {
            var secData = _group.Sections[name] as SectionInfo;
            if (secData != null)
                _config.RemoveConfigInfo(secData);
        }

        public void RemoveAt(int index)
        {
            var secData = _group.Sections[index] as SectionInfo;
            _config.RemoveConfigInfo(secData);
        }
    }
}

#endif