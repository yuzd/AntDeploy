#if NETSTANDARD

//
// System.Configuration.InternalConfigurationFactory.cs
//
// Authors:
// 	Lluis Sanchez Gual (lluis@novell.com)
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

using System.Configuration.Internal;

namespace System.Configuration
{
    internal class InternalConfigurationFactory : IInternalConfigConfigurationFactory
    {
        public Configuration Create(Type typeConfigHost, params object[] hostInitConfigurationParams)
        {
            var system = new InternalConfigurationSystem();
            system.Init(typeConfigHost, hostInitConfigurationParams);
            return new Configuration(system, null);
        }

        public string NormalizeLocationSubPath(string subPath)
        {
            return subPath;
        }
    }

    internal class InternalConfigurationSystem : IConfigSystem
    {
        private object[] _hostInitParams;

        public void Init(Type typeConfigHost, params object[] hostInitParams)
        {
            _hostInitParams = hostInitParams;
            Host = (IInternalConfigHost) Activator.CreateInstance(typeConfigHost);
            Root = new InternalConfigurationRoot();
        }

        public IInternalConfigHost Host { get; private set; }

        public IInternalConfigRoot Root { get; private set; }

        public void InitForConfiguration(ref string locationConfigPath, out string parentConfigPath,
            out string parentLocationConfigPath)
        {
            Host.InitForConfiguration(ref locationConfigPath, out parentConfigPath, out parentLocationConfigPath, Root,
                _hostInitParams);
        }
    }
}

#endif