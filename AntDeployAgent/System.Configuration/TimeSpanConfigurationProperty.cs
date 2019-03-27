#if NETSTANDARD

//
// System.Configuration.TimeSpanConfigurationProperty.cs
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
    public class TimeSpanConfigurationProperty : ConfigurationProperty
    {
        private readonly TimeSpanSerializedFormat _format;
        private readonly TimeSpanPropertyFlags _tsflags;

        public TimeSpanConfigurationProperty(string name, TimeSpan defaultValue, ConfigurationPropertyOptions flags)
            : base(name, typeof(TimeSpan), defaultValue, flags)
        {
        }

        public TimeSpanConfigurationProperty(string name, TimeSpan defaultValue, TimeSpanSerializedFormat format,
            ConfigurationPropertyOptions flags)
            : base(name, typeof(TimeSpan), defaultValue, flags)
        {
            _format = format;
        }

        public TimeSpanConfigurationProperty(string name, TimeSpan defaultValue, TimeSpanSerializedFormat format,
            TimeSpanPropertyFlags tsflags, ConfigurationPropertyOptions flags)
            : base(name, typeof(TimeSpan), defaultValue, flags)
        {
            _format = format;
            _tsflags = tsflags;
        }

        internal override object ConvertFromString(string value)
        {
            TimeSpan span;
            switch (_format)
            {
                case TimeSpanSerializedFormat.Seconds:
                    span = TimeSpan.FromSeconds(int.Parse(value));
                    break;
                case TimeSpanSerializedFormat.Minutes:
                    span = TimeSpan.FromMinutes(int.Parse(value));
                    break;
                default:
                    span = TimeSpan.Parse(value);
                    break;
            }
            Validate(span);
            return span;
        }

        internal override string ConvertToString(object value)
        {
            var span = (TimeSpan)value;
            Validate(span);

            switch (_format)
            {
                case TimeSpanSerializedFormat.Seconds:
                    return span.TotalSeconds.ToString();
                case TimeSpanSerializedFormat.Minutes:
                    return span.TotalMinutes.ToString();
                default:
                    return value.ToString();
            }
        }

        private void Validate(TimeSpan span)
        {
            if (span.Ticks < 0 && (_tsflags & TimeSpanPropertyFlags.AllowNegative) == 0)
                throw new ConfigurationErrorsException("TimeSpan value can't be negative");

            if (span == TimeSpan.Zero && (_tsflags & TimeSpanPropertyFlags.ProhibitZero) != 0)
                throw new ConfigurationErrorsException("TimeSpan value can't be zero");

            if (span == TimeSpan.MaxValue && (_tsflags & TimeSpanPropertyFlags.AllowInfinite) == 0)
                throw new ConfigurationErrorsException("TimeSpan value can't be infinite");
        }
    }
}

#endif