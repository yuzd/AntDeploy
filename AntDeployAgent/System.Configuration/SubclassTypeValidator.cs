#if NETSTANDARD

//
// System.Configuration.SubclassTypeValidator.cs
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

using System.Reflection;

namespace System.Configuration
{
    public sealed class SubclassTypeValidator : ConfigurationValidatorBase
    {
        private readonly Type _baseClass;

        public SubclassTypeValidator(Type baseClass)
        {
            _baseClass = baseClass;
        }

        public override bool CanValidate(Type type)
        {
            return type == typeof(Type);
        }

        public override void Validate(object value)
        {
            var type = (Type) value;

            if (!_baseClass.GetTypeInfo().IsAssignableFrom(type))
                throw new ArgumentException("The value must be a subclass");
        }
    }
}

#endif