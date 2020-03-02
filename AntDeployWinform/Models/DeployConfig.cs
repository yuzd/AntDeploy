using System.Collections.Generic;

namespace AntDeployWinform.Models
{
    public class GlobalConfig
    {
        public string MsBuildPath { get; set; }
        //public string DeployFolderPath { get; set; }
        public bool IsChinease { get; set; }
        public bool SaveLogs { get; set; }
        public bool MultiInstance { get; set; }
        public List<string> ProjectPathList  { get; set; }
    }

    /// <summary>
    /// 跟着项目来的
    /// </summary>
    public class PluginConfig
    {
        public string DeployFolderPath { get; set; }
        public string DeployHttpProxy { get; set; }
        public int LastTabIndex { get; set; }
        public bool IISEnableIncrement { get; set; }
        public bool IISEnableSelectDeploy { get; set; }
        public bool IISEnableNotStopSiteDeploy { get; set; }
        public bool IISEnableUseOfflineHtm { get; set; }
        public bool WindowsServiceEnableIncrement { get; set; }
        public bool WindowsServiceEnableSelectDeploy { get; set; }

        public bool DockerEnableIncrement { get; set; }
        public bool DockerServiceEnableSelectDeploy { get; set; }

        #region 镜像上传
        public bool DockerServiceEnableUpload { get; set; }
        public bool DockerServiceBuildImageOnly { get; set; }
        public string RepositoryUrl { get; set; }
        public string RepositoryUserName { get; set; }
        public string RepositoryUserPwd { get; set; }
        public string RepositoryNameSpace { get; set; }
        public string RepositoryImageName { get; set; }


        #endregion

        public string NetCorePublishMode { get; set; }


        public string GetNetCorePublishRuntimeArg()
        {
            if (string.IsNullOrEmpty(NetCorePublishMode))
            {
                return string.Empty;
            }

            if (NetCorePublishMode.Equals("FDD(runtime)"))
            {
                return string.Empty;
            }
            else
            {
                var runtime = NetCorePublishMode.Split('(')[1].Split(')')[0];
                return $" --runtime {runtime}";
            }

        }
    }

    public delegate void EnvChange(Env env, bool isServerChange,bool isRemove);
    public class DeployConfig
    {
        public event EnvChange EnvChangeEvent;
        public List<Env> Env { get; set; } = new List<Env>();

        public void AddEnv(Env env)
        {
            this.Env.Add(env);
            EnvChangeEvent?.Invoke(env, false,false);
        }

        public void RemoveEnv(int index)
        {
            var env = this.Env[index];
            this.Env.RemoveAt(index);
            EnvChangeEvent?.Invoke(env, false,true);
        }

        public void EnvServerChange(Env env)
        {
            EnvChangeEvent?.Invoke(env, true,false);
        }




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

        public List<EnvPairConfig> EnvPairList { get; set; } = new List<EnvPairConfig>();

    }


    public class EnvPairConfig
    {
        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }

        /// <summary>
        /// iis名称和环境走 windows服务名称和环境走
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// Docker的配置和环境走
        /// </summary>
        public string DockerPort { get; set; }
        public string DockerEnvName { get; set; }
        public string DockerVolume { get; set; }
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

        public List<EnvPairConfig> EnvPairList { get; set; } = new List<EnvPairConfig>();
    }


    public class DockerConfig
    {
        public string Prot { get; set; }
        public string AspNetCoreEnv { get; set; }
        public string LastEnvName { get; set; }
        public string RemoveDaysFromPublished { get; set; }
        public string Volume { get; set; }

        public List<EnvPairConfig> EnvPairList { get; set; } = new List<EnvPairConfig>();
    }

    public class Env
    {
        public string Name { get; set; }
        public List<Server> ServerList { get; set; } = new List<Server>();
        public List<LinuxServer> LinuxServerList { get; set; } = new List<LinuxServer>();

        public List<string> IgnoreList { get; set; } = new List<string>();
        public List<string> WindowsBackUpIgnoreList { get; set; } = new List<string>();
    }


    public class BaseServer
    {
        public string Host { get; set; }
        public string NickName { get; set; }
        public string IIsFireUrl { get; set; }
        public string DockerFireUrl { get; set; }
        public string WindowsServiceFireUrl { get; set; }
    }


    public class Server : BaseServer
    {

        public string Token { get; set; }


    }

    public class LinuxServer : BaseServer
    {
        public string UserName { get; set; }
        public string Pwd { get; set; }
    }

    public class DeployResult
    {
        public bool Success { get; set; }
        public string Msg { get; set; }

    }
}
