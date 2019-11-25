using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;
using LibGit2Sharp;

namespace AntDeployCommand.Operations
{
    public class DOCKERDEPLOY : OperationsBase
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
            if (!File.Exists(Arguments.PackageZipPath))
            {
                return $"{Name}{nameof(Arguments.PackageZipPath)} not found!";
            }
            if (string.IsNullOrEmpty(Arguments.LoggerId))
            {
                Arguments.LoggerId = Guid.NewGuid().ToString("N");
            }
            if (string.IsNullOrEmpty(Arguments.DeployFolderName))
            {
                Arguments.DeployFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            if (Arguments.RemoveDays < 1) Arguments.RemoveDays = 10;

            
            return string.Empty;
        }

        public override async Task<bool> Run()
        {
           
            byte[] zipBytes = File.ReadAllBytes(Arguments.PackageZipPath);
            if (zipBytes.Length < 1)
            {
                Error("package file is empty");
                return await Task.FromResult(false);
            }

            var hasError = false;
            using (var memory = new MemoryStream(zipBytes))
            {
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
                    ClientDateTimeFolderName = Arguments.DeployFolderName,
                    RemoveDaysFromPublished = Arguments.RemoveDays+"",
                    Volume = Arguments.Volume,
                    Remark = Arguments.Remark,
                    Email = Arguments.Email,
                    Increment = Arguments.IsSelectedDeploy ||
                                Arguments.IsIncrementDeploy,
                    IsSelect = Arguments.IsSelectedDeploy,
                    IsRuntime = string.IsNullOrEmpty(Arguments.BuildMode) || Arguments.BuildMode.Contains("FDD")
                })
                {
                    var connectResult = sshClient.Connect();
                    if (!connectResult)
                    {
                        this.Error($"Deploy Host:{Arguments.Host} Fail: connect fail");
                        return await Task.FromResult(false);
                    }

                    try
                    {
                        sshClient.PublishZip(memory, "antdeploy", "publish.zip", () => true);
                        if (hasError)
                        {
                            return await Task.FromResult(false);
                        }
                        this.Info($"【deploy success】Host:{Arguments.Host},Response:Success");
                        return await Task.FromResult(true);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Deploy Host:{Arguments.Host} Fail:" + ex.Message);
                        return await Task.FromResult(false);
                    }
                }
            }
        }
    }
}
