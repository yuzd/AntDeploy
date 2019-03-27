#if NETSTANDARD

//
// System.Configuration.ConfigurationErrorsException.cs
//
// Authors:
// 	Duncan Mak (duncan@ximian.com)
// 	Chris Toshok (toshok@ximian.com)
//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//

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

using System.Collections;
using System.Xml;

namespace System.Configuration
{
    /* disable the obsolete warnings about ConfigurationException */
#pragma warning disable 618

    public class ConfigurationErrorsException : Exception
    {

        public string Filename { get; private set; }

        public int LineNumber { get; private set; }

        public ICollection Errors
        {
            get { throw new NotImplementedException(); }
        }

        public ConfigurationErrorsException()
        {
        }

        public ConfigurationErrorsException(string message)
            : base(message)
        {
        }

        public ConfigurationErrorsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public ConfigurationErrorsException(string message, XmlNode node)
            : this(message, null, GetFilename(node), GetLineNumber(node))
        {
        }

        public ConfigurationErrorsException(string message, Exception inner, XmlNode node)
            : this(message, inner, GetFilename(node), GetLineNumber(node))
        {
        }

        public ConfigurationErrorsException(string message, XmlReader reader)
            : this(message, null, GetFilename(reader), GetLineNumber(reader))
        {
        }

        public ConfigurationErrorsException(string message, Exception inner, XmlReader reader)
            : this(message, inner, GetFilename(reader), GetLineNumber(reader))
        {
        }

        public ConfigurationErrorsException(string message, string filename, int line)
            : this(message, null, filename, line)
        {
        }

        public ConfigurationErrorsException(string message, Exception inner, string filename, int line)
            : base(message, inner)
        {
            Filename = filename;
            LineNumber = line;
        }

        public static string GetFilename(XmlReader reader)
        {
            return reader != null ? reader.BaseURI : null;
        }

        public static int GetLineNumber(XmlReader reader)
        {
            var li = reader as IXmlLineInfo;
            return li != null ? li.LineNumber : 0;
        }

        public static string GetFilename(XmlNode node)
        {
            return null;
        }

        public static int GetLineNumber(XmlNode node)
        {
            return 0;
        }
    }
#pragma warning restore
}

#endif