#if NETSTANDARD

//
// System.Configuration.TimeSpanValidator.cs
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

namespace System.Configuration
{
    public class TimeSpanValidator : ConfigurationValidatorBase
    {
        private readonly TimeSpan _maxValue;
        private readonly TimeSpan _minValue;
        private readonly bool _rangeIsExclusive;
        private readonly long _resolutionInSeconds;

        public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue) : this(minValue, maxValue, false, 0)
        {
        }

        public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive)
            : this(minValue, maxValue, rangeIsExclusive, 0)
        {
        }

        public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive, long resolutionInSeconds)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _rangeIsExclusive = rangeIsExclusive;
            _resolutionInSeconds = resolutionInSeconds;
        }

        public override bool CanValidate(Type type)
        {
            return type == typeof(TimeSpan);
        }

        public override void Validate(object value)
        {
            var s = (TimeSpan) value;
            if (!_rangeIsExclusive)
            {
                if (s < _minValue || s > _maxValue)
                    throw new ArgumentException("The value must be in the range " + _minValue + " - " + _maxValue);
            }
            else
            {
                if (s >= _minValue && s <= _maxValue)
                    throw new ArgumentException("The value must not be in the range " + _minValue + " - " + _maxValue);
            }
            if (_resolutionInSeconds != 0 && s.Ticks%(TimeSpan.TicksPerSecond*_resolutionInSeconds) != 0)
                throw new ArgumentException("The value must have a resolution of " +
                                            TimeSpan.FromTicks(TimeSpan.TicksPerSecond*_resolutionInSeconds));
        }
    }
}

#endif