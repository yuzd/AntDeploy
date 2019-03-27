#if NETSTANDARD

using System.Collections.Generic;

namespace System.Configuration.System.Configuration
{
    internal static class MachineConfig
    {
        internal static readonly IReadOnlyDictionary<string, string> ConfigSections = new Dictionary<string, string>
        {
            {"appSettings","System.Configuration.AppSettingsSection, System.Configuration"},
            {"connectionStrings","System.Configuration.ConnectionStringsSection"},
            {"mscorlib","System.Configuration.IgnoreSection, System.Configuration"},
            {"runtime","System.Configuration.IgnoreSection, System.Configuration"},
            {"assemblyBinding","System.Configuration.IgnoreSection, System.Configuration"},
            {"satelliteassemblies","System.Configuration.IgnoreSection, System.Configuration"},
            {"startup","System.Configuration.IgnoreSection, System.Configuration"},
            {"windows","System.Configuration.IgnoreSection, System.Configuration"},
            {"system.webServer","System.Configuration.IgnoreSection, System.Configuration"},
            {"system.runtime.remoting","System.Configuration.IgnoreSection, System.Configuration"}
        };
    }
}


#endif