using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;
using LibGit2Sharp;

namespace AntDeployCommand.Operations
{
    public class DOCKERROLL : OperationsBase
    {
        public override string ValidateArgument()
        {
            if (string.IsNullOrEmpty(Arguments.Host))
            {
                return $"{Name}{nameof(Arguments.Host)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.Root))
            {
                return $"{Name}{nameof(Arguments.Root)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.Pwd))
            {
                return $"{Name}{nameof(Arguments.Pwd)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.EnvType))
            {
                return $"{Name}{nameof(Arguments.EnvType)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.DeployFolderName))
            {
                return $"{Name}{nameof(Arguments.DeployFolderName)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.Token)) //NetCoreSDKVersion
            {
                return $"{Name}NetCoreSDKVersion required!";
            }
            if (string.IsNullOrEmpty(Arguments.PackageZipPath))
            {
                return $"{Name}{nameof(Arguments.PackageZipPath)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.PackagePath))//entry point
            {
                return $"{Name}{nameof(Arguments.PackagePath)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.LoggerId))
            {
                Arguments.LoggerId = Guid.NewGuid().ToString("N");
            }

            return string.Empty;
        }

        public override async Task Run()
        {
            if (Arguments.EnvType.Equals("TEST"))
            {
                TEST();
                return;
            }

            if (Arguments.EnvType.Equals("ROLL"))
            {
                RollBack();
                return;
            }

            if (File.Exists(Arguments.PackageZipPath))
            {
                File.Delete(Arguments.PackageZipPath);
            }

            using (SSHClient sshClient = new SSHClient(Arguments.Host, Arguments.Root, Arguments.Pwd, Arguments.Proxy,
                (str, logLevel) =>
                {
                    if (logLevel == LogLevel.Error)
                    {
                        this.Error("【Server】" + str);
                    }
                    else if (logLevel == LogLevel.Warning)
                    {
                        this.Warn("【Server】" + str);
                    }
                    else
                    {
                        this.Info("【Server】" + str);
                    }

                    return false;
                }, (uploadValue) => { })
            {
                NetCoreENTRYPOINT = Arguments.PackagePath,
                NetCoreVersion = Arguments.Token,
                NetCorePort = Arguments.Port,
                NetCoreEnvironment = Arguments.AspEnv,
            })
            {
                var connectResult = sshClient.Connect();
                if (!connectResult)
                {
                    this.Error($"connect rollBack Host:{Arguments.Host} Fail");
                    return;
                }

                var versionList = sshClient.GetDeployHistory("antdeploy", 11);

                if (versionList.Count <= 1)
                {
                    this.Error($"Host:{Arguments.Host} get rollBack version list count:0");
                    return;
                }

                this.Info($"Host:{Arguments.Host} get rollBack version list count:{versionList.Count}");

                File.WriteAllLines(Arguments.PackageZipPath,versionList.Select(r=>r.Item1+"@_@"+r.Item2),Encoding.UTF8);

            }
            await Task.CompletedTask;
        }

        private void TEST()
        {
            this.Info($"Host:{Arguments.Host} ");

            using (SSHClient sshClient = new SSHClient(Arguments.Host, Arguments.Root, Arguments.Pwd,
                Arguments.Proxy,  (str, logLevel) =>
                {

                    if (logLevel == LogLevel.Error)
                    {
                        this.Error("【Server】" + str);
                    }
                    else if (logLevel == LogLevel.Warning)
                    {
                        this.Warn("【Server】" + str);
                    }
                    else
                    {
                        this.Info("【Server】" + str);
                    }

                    return false;
                }, (progress) => { })
            {
            })
            {
                var connectResult = sshClient.Connect();
                if (!connectResult)
                {
                    throw new Exception($"RollBack Host:{Arguments.Host} Fail: connect fail");
                }

                this.Info($"Host:{Arguments.Host} Connect Success");
            }
        }

        private void RollBack()
        {
            var hasError = false;
            using (SSHClient sshClient = new SSHClient(Arguments.Host, Arguments.Root, Arguments.Pwd,
                Arguments.Proxy, (str, logLevel) =>
                {

                    if (logLevel == LogLevel.Error)
                    {
                        hasError = true;
                        this.Error("【Server】" + str);
                    }
                    else if (logLevel == LogLevel.Warning)
                    {
                        this.Warn("【Server】" + str);
                    }
                    else
                    {
                        this.Info("【Server】" + str);
                    }

                    return false;
                }, (progress) => { })
            {
                NetCoreENTRYPOINT = Arguments.PackagePath,
                NetCoreVersion = Arguments.Token,
                NetCorePort = Arguments.Port,
                NetCoreEnvironment = Arguments.AspEnv,
            })
            {
                var connectResult = sshClient.Connect();
                if (!connectResult)
                {
                    this.Error($"RollBack Host:{Arguments.Host} Fail: connect fail");
                    return;
                }

                try
                {
                    sshClient.RollBack(Arguments.DeployFolderName);
                    if (hasError)
                    {
                        return;
                    }

                    this.Info($"【rollback success】Host:{Arguments.Host},Response:Success");
                }
                catch (Exception ex)
                {
                    this.Error($"RollBack Host:{Arguments.Host} Fail:" + ex.Message);
                }
            }
        }
    }
}
