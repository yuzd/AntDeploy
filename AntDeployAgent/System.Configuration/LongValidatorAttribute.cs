#if NETSTANDARD

//
// System.Configuration.LongValidatorAttribute.cs
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
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//

namespace System.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LongValidatorAttribute : ConfigurationValidatorAttribute
    {
        private bool _excludeRange;
        private ConfigurationValidatorBase _instance;
        private long _maxValue;
        private long _minValue;

        public bool ExcludeRange
        {
            get { return _excludeRange; }
            set
            {
                _excludeRange = value;
                _instance = null;
            }
        }

        public long MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                _instance = null;
            }
        }

        public long MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                _instance = null;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new LongValidator(_minValue, _maxValue, _excludeRange);

                return _instance;
            }
        }
    }
}

#endif