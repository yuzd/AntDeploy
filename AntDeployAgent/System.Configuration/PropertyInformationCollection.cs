#if NETSTANDARD

//
// System.Configuration.PropertyInformationCollection.cs
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

using System.Collections;
using System.Collections.Specialized;

namespace System.Configuration
{
    public sealed class PropertyInformationCollection : NameObjectCollectionBase
    {
        internal PropertyInformationCollection()
            : base(StringComparer.Ordinal)
        {
        }

        public PropertyInformation this[string propertyName]
        {
            get { return (PropertyInformation) BaseGet(propertyName); }
        }

        public void CopyTo(PropertyInformation[] array, int index)
        {
            ((ICollection) this).CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return new PropertyInformationEnumerator(this);
        }

        internal void Add(PropertyInformation pi)
        {
            BaseAdd(pi.Name, pi);
        }

        private class PropertyInformationEnumerator : IEnumerator
        {
            private readonly PropertyInformationCollection _collection;
            private int _position;

            public PropertyInformationEnumerator(PropertyInformationCollection collection)
            {
                _collection = collection;
                _position = -1;
            }

            public object Current
            {
                get
                {
                    if ((_position < _collection.Count) && (_position >= 0))
                        return _collection.BaseGet(_position);
                    throw new InvalidOperationException();
                }
            }

            public bool MoveNext()
            {
                return ++_position < _collection.Count;
            }

            public void Reset()
            {
                _position = -1;
            }
        }
    }
}

#endif