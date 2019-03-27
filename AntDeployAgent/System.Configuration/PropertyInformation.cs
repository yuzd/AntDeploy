#if NETSTANDARD

//
// System.Configuration.PropertyInformation.cs
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

using System.ComponentModel;

namespace System.Configuration
{
    public sealed class PropertyInformation
    {
        private readonly ConfigurationElement _owner;
        private object _val;

        internal PropertyInformation(ConfigurationElement owner, ConfigurationProperty property)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (property == null)
                throw new ArgumentNullException("property");
            _owner = owner;
            Property = property;
        }

        public TypeConverter Converter
        {
            get { return Property.Converter; }
        }

        public object DefaultValue
        {
            get { return Property.DefaultValue; }
        }

        public string Description
        {
            get { return Property.Description; }
        }

        public bool IsKey
        {
            get { return Property.IsKey; }
        }


        public bool IsLocked { get; internal set; }

        public bool IsRequired
        {
            get { return Property.IsRequired; }
        }

        public int LineNumber { get; internal set; }

        public string Name
        {
            get { return Property.Name; }
        }

        public string Source { get; internal set; }

        public Type Type
        {
            get { return Property.Type; }
        }

        public ConfigurationValidatorBase Validator
        {
            get { return Property.Validator; }
        }

        public object Value
        {
            get
            {
                if (ValueOrigin == PropertyValueOrigin.Default)
                {
                    if (Property.IsElement)
                    {
                        var elem = (ConfigurationElement) Activator.CreateInstance(Type, true);
                        elem.InitFromProperty(this);
                        if (_owner != null && _owner.IsReadOnly())
                            elem.SetReadOnly();
                        _val = elem;
                        ValueOrigin = PropertyValueOrigin.Inherited;
                    }
                    else
                    {
                        return DefaultValue;
                    }
                }
                return _val;
            }
            set
            {
                _val = value;
                ValueOrigin = PropertyValueOrigin.SetHere;
            }
        }

        internal bool IsElement
        {
            get { return Property.IsElement; }
        }

        public PropertyValueOrigin ValueOrigin { get; private set; }

        internal ConfigurationProperty Property { get; }

        internal void Reset(PropertyInformation parentProperty)
        {
            if (parentProperty != null)
            {
                if (Property.IsElement)
                {
                    ((ConfigurationElement) Value).Reset((ConfigurationElement) parentProperty.Value);
                }
                else
                {
                    _val = parentProperty.Value;
                    ValueOrigin = PropertyValueOrigin.Inherited;
                }
            }
            else
            {
                ValueOrigin = PropertyValueOrigin.Default;
            }
        }

        internal string GetStringValue()
        {
            return Property.ConvertToString(Value);
        }

        internal void SetStringValue(string value)
        {
            _val = Property.ConvertFromString(value);
            if (!Equals(_val, DefaultValue))
                ValueOrigin = PropertyValueOrigin.SetHere;
        }
    }
}

#endif