using System.Collections.Generic;

namespace AntDeploy.Models
{
    public delegate void EnvChange(Env env,bool isRemove);
    public class DeployConfig
    {
        public event EnvChange EnvChangeEvent;
        public List<Env> Env { get; set; } = new List<Env>();

        public void AddEnv(Env env)
        {
            this.Env.Add(env);
            EnvChangeEvent?.Invoke(env, false);
        }

        public void RemoveEnv(int index)
        {
            var env = this.Env[index];
            this.Env.RemoveAt(index);
            EnvChangeEvent?.Invoke(env, true);
        }
       
        public List<string> IgnoreList { get; set; } = new List<string>();


        #region IIS
        public IIsConfig IIsConfig { get; set; } = new IIsConfig();
        #endregion

        public WindowsServiveConfig WindowsServiveConfig { get; set; } = new WindowsServiveConfig();

    }

    public class IIsConfig
    {
        public string SdkType { get; set; }

        public string WebSiteName { get; set; }

        public string LastEnvName { get; set; }
    }


    public class WindowsServiveConfig
    {
        public string ServiceName { get; set; }
        public string LastEnvName { get; set; }
    }

    public class Env
    {
        public string Name { get; set; }
        public List<Server> ServerList { get; set; } = new List<Server>();
    }


    public class Server
    {
        public string Host { get; set; }
        public string Token { get; set; }
    }
}
