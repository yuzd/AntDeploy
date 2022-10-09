#if NETSTANDARD

//
// System.Configuration.ConfigurationProperty.cs
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
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//

namespace System.Configuration
{
    public class IntegerConfigurationProperty : ConfigurationProperty
    {
        private readonly int _max;
        private readonly int _min;

        public IntegerConfigurationProperty(string name, int defaultValue, ConfigurationPropertyOptions flags)
            : base(name, typeof(int), defaultValue, flags)
        {
            _min = int.MinValue;
            _max = int.MaxValue;
        }

        public IntegerConfigurationProperty(string name, int defaultValue, int minimumValue, int maximumValue,
            ConfigurationPropertyOptions flags)
            : base(name, typeof(int), defaultValue, flags)
        {
            _min = minimumValue;
            _max = maximumValue;
        }

        internal override object ConvertFromString(string value)
        {
            return int.Parse(value);
        }

        internal override string ConvertToString(object value)
        {
            var val = (int) value;
            if (val < _min || val > _max)
                throw new ConfigurationErrorsException("The property '" + Name + "' must have a value between '" + _min +
                                                       "' and '" + _max + "'");
            return value.ToString();
        }
    }
}

#endif