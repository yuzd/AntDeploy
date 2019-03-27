#if NETSTANDARD

//
// System.Configuration.ConfigurationProperty.cs
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

using System.ComponentModel;
using System.Reflection;

namespace System.Configuration
{
    public class ConfigurationProperty
    {
        internal static readonly object NoDefaultValue = new object();
        private readonly ConfigurationPropertyOptions _flags;

        public ConfigurationProperty(string name, Type type)
            : this(
                name, type, NoDefaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(),
                ConfigurationPropertyOptions.None, null)
        {
        }

        public ConfigurationProperty(string name, Type type, object defaultValue)
            : this(
                name, type, defaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(),
                ConfigurationPropertyOptions.None, null)
        {
        }

        public ConfigurationProperty(
            string name, Type type, object defaultValue,
            ConfigurationPropertyOptions flags)
            : this(name, type, defaultValue, TypeDescriptor.GetConverter(type), new DefaultValidator(), flags, null)
        {
        }

        public ConfigurationProperty(
            string name, Type type, object defaultValue,
            TypeConverter converter,
            ConfigurationValidatorBase validation,
            ConfigurationPropertyOptions flags)
            : this(name, type, defaultValue, converter, validation, flags, null)
        {
        }

        public ConfigurationProperty(
            string name, Type type, object defaultValue,
            TypeConverter converter,
            ConfigurationValidatorBase validation,
            ConfigurationPropertyOptions flags,
            string description)
        {
            Name = name;
            Converter = converter ?? TypeDescriptor.GetConverter(type);
            if (defaultValue != null)
            {
                if (defaultValue == NoDefaultValue)
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Object:
                            defaultValue = null;
                            break;
                        case TypeCode.String:
                            defaultValue = string.Empty;
                            break;
                        default:
                            defaultValue = Activator.CreateInstance(type);
                            break;
                    }
                }
                else if (!type.GetTypeInfo().IsAssignableFrom(defaultValue.GetType()))
                {
                    if (!Converter.CanConvertFrom(defaultValue.GetType()))
                        throw new ConfigurationErrorsException(
                            string.Format(
                                "The default value for property '{0}' has a different type than the one of the property itself: expected {1} but was {2}",
                                name, type, defaultValue.GetType()));

                    defaultValue = Converter.ConvertFrom(defaultValue);
                }
            }
            DefaultValue = defaultValue;
            _flags = flags;
            Type = type;
            Validator = validation ?? new DefaultValidator();
            Description = description;
        }

        public TypeConverter Converter { get; }

        public object DefaultValue { get; }

        public bool IsKey
        {
            get { return (_flags & ConfigurationPropertyOptions.IsKey) != 0; }
        }

        public bool IsRequired
        {
            get { return (_flags & ConfigurationPropertyOptions.IsRequired) != 0; }
        }

        public bool IsDefaultCollection
        {
            get { return (_flags & ConfigurationPropertyOptions.IsDefaultCollection) != 0; }
        }

        public string Name { get; }

        public string Description { get; }

        public Type Type { get; }

        public ConfigurationValidatorBase Validator { get; }

        internal bool IsElement
        {
            get { return typeof(ConfigurationElement).IsAssignableFrom(Type); }
        }

        internal ConfigurationCollectionAttribute CollectionAttribute { get; set; }

        internal virtual object ConvertFromString(string value)
        {
            if (Converter != null)
                return Converter.ConvertFromInvariantString(value);
            throw new NotImplementedException();
        }

        internal virtual string ConvertToString(object value)
        {
            if (Converter != null)
                return Converter.ConvertToInvariantString(value);
            throw new NotImplementedException();
        }

        internal void Validate(object value)
        {
            if (Validator != null)
                Validator.Validate(value);
        }
    }
}

#endif