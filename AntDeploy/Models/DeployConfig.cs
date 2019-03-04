using System.Collections.Generic;

namespace AntDeploy.Models
{
    public class PluginConfig
    {
        public int LastTabIndex { get; set; }
        public bool IISEnableIncrement { get; set; }
        public bool WindowsServiceEnableIncrement { get; set; }
    }

    public delegate void EnvChange(Env env,bool isServerChange);
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
            EnvChangeEvent?.Invoke(env, false);
        }
       
       public void EnvServerChange(Env env)
       {
           EnvChangeEvent?.Invoke(env, true);
        }

        public List<string> IgnoreList { get; set; } = new List<string>();


        #region IIS
        public IIsConfig IIsConfig { get; set; } = new IIsConfig();
        #endregion

        public WindowsServiveConfig WindowsServiveConfig { get; set; } = new WindowsServiveConfig();
        public DockerConfig DockerConfig { get; set; } = new DockerConfig();

    }

    public class IIsConfig
    {
        public string SdkType { get; set; }

        public string WebSiteName { get; set; }

        public string LastEnvName { get; set; }
    }


    /// <summary>
    /// 如果是netcore的话 需要 dotnet publish -c Release --runtime win-x64 （第一次会需要很长时间）
    /// 然后目录会有一个 win-x64 的文件夹 这个是需要publish的目录 里面会有exe 可以作为windowsservice发布
    /// 
    /// 如果是netframework的话 需要调用msbuild 来进行 release 编译
    /// 需要取一个参数来看一看是否是 用了projectInstall技术开发的。如果是的话是可以 远程服务器上没有的话可以直接创建
    /// 否则的话不可以
    /// </summary>
    public class WindowsServiveConfig
    {
        public string ServiceName { get; set; }

        public string SdkType { get; set; }

        public string LastEnvName { get; set; }
    }


    public class DockerConfig
    {
        public string Prot { get; set; }
        public string AspNetCoreEnv { get; set; }
        public string LastEnvName { get; set; }
        public string RemoveDaysFromPublished { get; set; }
    }

    public class Env
    {
        public string Name { get; set; }
        public List<Server> ServerList { get; set; } = new List<Server>();
        public List<LinuxServr> LinuxServerList { get; set; } = new List<LinuxServr>();
    }


    public class Server
    {
        public string Host { get; set; }
        public string Token { get; set; }
    }

    public class LinuxServr
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
    }

    public class DeployResult
    {
        public bool Success { get; set; }
        public string Msg { get; set; }

    }
}
