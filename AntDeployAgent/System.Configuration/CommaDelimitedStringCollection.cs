#if NETSTANDARD

//
// System.Configuration.CommaDelimitedStringCollection.cs
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

using System.Collections.Specialized;

namespace System.Configuration
{
    /* i really hate all these "new"s... maybe
	 * StringCollection marks these methods as virtual in
	 * 2.0? */

    public sealed class CommaDelimitedStringCollection : StringCollection
    {
        private int _originalStringHash;

        public new bool IsReadOnly { get; private set; }

        public new string this[int index]
        {
            get { return base[index]; }
            set
            {
                if (IsReadOnly) throw new ConfigurationErrorsException("The configuration is read only");

                base[index] = value;
            }
        }

        public new void Add(string value)
        {
            if (IsReadOnly) throw new ConfigurationErrorsException("The configuration is read only");

            base.Add(value);
        }

        public new void AddRange(string[] range)
        {
            if (IsReadOnly) throw new ConfigurationErrorsException("The configuration is read only");

            base.AddRange(range);
        }

        public new void Clear()
        {
            if (IsReadOnly) throw new ConfigurationErrorsException("The configuration is read only");

            base.Clear();
        }

        public CommaDelimitedStringCollection Clone()
        {
            var col = new CommaDelimitedStringCollection();
            var contents = new string[Count];
            CopyTo(contents, 0);

            col.AddRange(contents);
            col._originalStringHash = _originalStringHash;

            return col;
        }

        public new void Insert(int index, string value)
        {
            if (IsReadOnly) throw new ConfigurationErrorsException("The configuration is read only");

            base.Insert(index, value);
        }

        public new void Remove(string value)
        {
            if (IsReadOnly) throw new ConfigurationErrorsException("The configuration is read only");

            base.Remove(value);
        }

        public void SetReadOnly()
        {
            IsReadOnly = true;
        }

        public override string ToString()
        {
            if (Count == 0)
                return null;

            var contents = new string[Count];

            CopyTo(contents, 0);

            return string.Join(",", contents);
        }

        internal void UpdateStringHash()
        {
            var str = ToString();
            if (str == null)
                _originalStringHash = 0;
            else
                _originalStringHash = str.GetHashCode();
        }
    }
}

#endif