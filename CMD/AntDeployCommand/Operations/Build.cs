using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;

namespace AntDeployCommand.Operations
{
    public class Build: OperationsBase
    {
        public override string ValidateArgument()
        {
            if (string.IsNullOrEmpty(Arguments.EnvName))
            {
                return $"{nameof(Arguments.EnvName)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.EnvType))
            {
                return $"{nameof(Arguments.EnvType)} required!";
            }

            if (string.IsNullOrEmpty(Arguments.ProjectPath))
            {
                return $"{nameof(Arguments.ProjectPath)} required!";
            }

            if (!File.Exists(Arguments.ProjectPath))
            {
                return $"{(Arguments.ProjectPath)} not found!";
            }

            return string.Empty;
        }

        public override void Run()
        {
            var file = new FileInfo(Arguments.ProjectPath);
            var ProjectFolderPath = file.DirectoryName;
            var buildMode = GetNetCorePublishRuntimeArg();
            var envType = string.Empty;
            if (Arguments.EnvType.Equals("IIS"))
            {
                envType = "deploy_iis";
            }
            else if (Arguments.EnvType.Equals("DOCKER"))
            {
                envType = "deploy_docker";
            }
            else if (Arguments.EnvType.Equals("SERVICE"))
            {
                envType = "deploy_winservice";
            }
            var publishPath = Path.Combine(ProjectFolderPath, "bin", "Release", envType, Arguments.EnvName);
            var path = publishPath + "\\";
            //执行 publish
            var isSuccess = CommandHelper.RunDotnetExe(Arguments.ProjectPath, path, $"publish \"{Arguments.ProjectPath}\" -c Release{buildMode}");
            if (!isSuccess)
            {
                LogHelper.Error("【publish error】please check build log");
            }
            else
            {
                LogHelper.Info($"【publish Success】{publishPath}");
            }
        }


        public string GetNetCorePublishRuntimeArg()
        {
            if (string.IsNullOrEmpty(Arguments.BuildMode))
            {
                return string.Empty;
            }

            if (Arguments.BuildMode.Equals("FDD(runtime)"))
            {
                return string.Empty;
            }
            else
            {
                var runtime = Arguments.BuildMode.Split('(')[1].Split(')')[0];
                return $" --runtime {runtime}";
            }

        }
    }
}
