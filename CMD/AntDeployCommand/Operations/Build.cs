using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;

namespace AntDeployCommand.Operations
{
    public class Build: OperationsBase
    {
        public string PublishPath { get; set; }
        public override string ValidateArgument()
        {
            if (string.IsNullOrEmpty(Arguments.EnvName))
            {
                return $"【Build】{nameof(Arguments.EnvName)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.EnvType))
            {
                return $"【Build】{nameof(Arguments.EnvType)} required!";
            }

            if (string.IsNullOrEmpty(Arguments.ProjectPath))
            {
                return $"【Build】{nameof(Arguments.ProjectPath)} required!";
            }

            if (!File.Exists(Arguments.ProjectPath))
            {
                return $"【Build】{(Arguments.ProjectPath)} not found!";
            }

            return string.Empty;
        }

        public override async Task<bool> Run()
        {
            var file = new FileInfo(Arguments.ProjectPath);
            var ProjectFolderPath = file.DirectoryName;
            var buildMode = GetNetCorePublishRuntimeArg();
            var envType = "unknow";
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
                PublishPath = publishPath;
                LogHelper.Info($"【publish success】{publishPath}");
            }

            return  await Task.FromResult(isSuccess);
        }


        public string GetNetCorePublishRuntimeArg()
        {
            if (string.IsNullOrEmpty(Arguments.BuildMode))
            {
                return string.Empty;
            }

            try
            {
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
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
