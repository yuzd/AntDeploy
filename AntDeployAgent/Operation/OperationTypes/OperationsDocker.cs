using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgent.Model;
using AntDeployAgent.Util;
using AntDeployAgentWindows.Util;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsDocker : OperationsBase
    {
        private string volumeProfix = "# volume@";
        private string otherProfix = "# other@";
        private string serverPortProfix = "# server_port@";
        private int retryTimes = 0;
        private DockerParamModel model;
        public OperationsDocker(Arguments args, Action<string> log)
            : base(args, log)
        {
            model = (DockerParamModel)args.Extention;
        }

        public override void ValidateArguments()
        {
            base.ValidateArguments();
        }

        public override void Backup()
        {
            base.Backup();
        }

        public override void Restore()
        {
            base.Restore();
        }

        public override void Stop()
        {
        }

        public override void Deploy()
        {
            try
            {
                base.Deploy();
            }
            catch (Exception exception)
            {
                logger("Copy File Fail :" + exception.Message);
                retryTimes++;
                logger("Wait 5Senconds to Retry :" + retryTimes);
                Thread.Sleep(5000);
                if (retryTimes > 3)
                {
                    throw new Exception("【Error】Retry Copy Limit ");
                }

                Deploy();
            }
        }

        public override void Start()
        {
            // 先检查dockerfile
            var dockerFile = Path.Combine(args.AppFolder, "Dockerfile");
            var isExistDockFile = File.Exists(dockerFile);
            if (!isExistDockFile)
            {
                if (model.RollBack)
                {
                    throw new Exception("【Error】RollBack DokcerFile create fail ");
                }
                var createDockerRt = CreateDockerFile(dockerFile);
                if (!createDockerRt)
                {
                    throw new Exception("【Error】DokcerFile create fail ");
                }
            }
            else
            {
                logger($"dockerFile found in: [{dockerFile}]");
                CheckDockerFile(dockerFile);
            }
            if (string.IsNullOrEmpty(model.RealPort))
            {
                model.RealPort = model.ContainerPort;
            }

            if (string.IsNullOrEmpty(model.RealServerPort))
            {
                model.RealServerPort = model.ServerPort;
            }

            var specialName = model.PorjectName;
            var continarName = "d_" + model.PorjectName;
            var runContainerName = $"--name {continarName}";
            string volume = GetVolume();

            if (!string.IsNullOrEmpty(model.Other) && model.Other.Contains("--name "))
            {
                var arrOther = model.Other.Split(new string[] { "--name " }, StringSplitOptions.None);
                if (arrOther.Length == 2 && !string.IsNullOrEmpty(arrOther[1]))
                {
                    var specialName1 = arrOther[1].Split(' ').FirstOrDefault();
                    if (!string.IsNullOrEmpty(specialName1))
                    {
                        logger($"[Warn]--name in Other Args is : {specialName1}");
                        specialName = specialName1;
                        continarName = specialName;
                        runContainerName = $"--name {continarName}";
                    }
                }
                else
                {
                    logger($"--name in Other Args is invaild");
                }
            }
            else
            {
                logger($"--name in Other Args is not defined");
            }

            //执行docker build 生成一个镜像
            var dockerBuildResult = CopyHelper.RunExternalExe($"{model.Sudo} docker build --no-cache --rm -t {specialName}:{args.TempPhysicalPath} -f {dockerFile} {args.AppFolder} ", this.logger);
            if (!dockerBuildResult)
            {
                throw new Exception("build docker image fail");
            }

            if (model.DockerServiceBuildImageOnly)
            {
                goto DockerServiceBuildImageOnlyLEVEL;
            }


            //先发送退出命令
            //https://stackoverflow.com/questions/40742192/how-to-do-gracefully-shutdown-on-dotnet-with-docker

            try
            {
                if (CopyHelper.RunExternalExe($"{model.Sudo} docker stop -t 10 {continarName}", null))
                {
                    logger($"{model.Sudo} docker stop -t 10 {continarName}");
                }
                Thread.Sleep(5000);
            }
            catch (Exception)
            {
                //ignore
            }

            try
            {
                //查看容器有没有在runing 如果有就干掉它
                if (CopyHelper.RunExternalExe($"{model.Sudo} docker rm -f {continarName}", null))
                {
                    this.logger($"{model.Sudo} docker rm -f {continarName}");
                }
            }
            catch (Exception)
            {
                //ignore
            }

            // 根据image启动一个容器
            var dockerRunRt = ($"{model.Sudo} docker run -d {runContainerName}{volume}{(string.IsNullOrEmpty(model.Other) ? "" : $" {model.Other}")} --restart=always {(!model.RealServerPort.Equals("0") && !model.RealPort.Equals("0") ? $"-p {model.RealServerPort}:{model.RealPort}" : "")} {specialName}:{args.TempPhysicalPath}");

            var runSuccess = CopyHelper.RunExternalExe(dockerRunRt,  this.logger);
            if (!runSuccess)
            {
                throw new Exception("start docker run Fail ");
            }
//查看是否只打包镜像不允许
DockerServiceBuildImageOnlyLEVEL:
            if (model.DockerServiceBuildImageOnly)
            {
                logger($"[Warn]ignore docker run");
            }
            Tuple<string, string, string> currentImageInfo = null;
            //把旧的image给删除
            CopyHelper.RunExternalExe(model.Sudo + " docker images "+ specialName + " --format '{{.Repository}}:{{.Tag}}:{{.ID}}'", (msg) =>
                {
                    var deleteImageArr = msg.Replace("'","").Split('\n');
                    var clearOldImages = false;
                    foreach (var imageName in deleteImageArr)
                    {
                        if (imageName.StartsWith($"{specialName}:{args.TempPhysicalPath}:"))
                        {
                            var imageArr2 = imageName.Split(':');
                            if (imageArr2.Length == 3)
                            {
                                //当前的
                                currentImageInfo = new Tuple<string, string, string>(imageArr2[0], imageArr2[1], imageArr2[2]);
                            }
                            //当前版本
                            continue;
                        }

                        var imageArr = imageName.Split(':');
                        if (imageArr.Length == 3)
                        {
                            var r2 = CopyHelper.RunCommand($"{model.Sudo} docker rmi {imageArr[2]}");
                            if (r2)
                            {
                                if (!clearOldImages)
                                {
                                    logger($"start to clear old images of name:{specialName}");
                                    clearOldImages = true;
                                }
                                logger($"{model.Sudo} docker rmi {imageArr[2]} [{imageName}]");
                            }
                        }
                    }

                },false);


            try
            {
                //查看是否有<none>的image 把它删掉 因为我们创建image的时候每次都会覆盖所以会产生一些没有的image
                CopyHelper.RunExternalExe(model.Sudo + " docker images -f \"dangling=true\" --format '{{.Repository}}:{{.Tag}}:{{.ID}}'", (msg) =>
                {
                    var deleteImageArr = msg.Replace("'","").Split('\n');
                    foreach (var imageName in deleteImageArr)
                    {
                        var imageArr = imageName.Split(':');
                        if (imageArr.Length == 3)
                        {
                            CopyHelper.RunCommand($"{model.Sudo} docker rmi {imageArr[2]}");
                        }
                    }
                },false);
            }
            catch (Exception)
            {
                //igore
            }

            if (currentImageInfo != null && model.DockerServiceEnableUpload)
            {
                var uploadTag = currentImageInfo.Item2;
                var uploadImage = model.RepositoryImageName;
                if (uploadImage.Contains(":"))
                {
                    var arr = uploadImage.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2)
                    {
                        uploadImage = arr[0];
                        uploadTag = arr[1];
                    }
                    else
                    {
                        logger($"【Error】[upload image] - image name invaild");
                    }
                }

                uploadImage = uploadImage.Replace("：", ":");
                if (System.Text.RegularExpressions.Regex.IsMatch(uploadImage, @"[\u4e00-\u9fa5]"))
                {
                    logger($"【Error】[upload image] - image name invaild");
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(uploadTag, @"[\u4e00-\u9fa5]"))
                {
                    logger($"【Error】[upload image] - image tab name invaild");
                }

                var uploadImageName = $"{(string.IsNullOrEmpty(model.RepositoryUrl) ? "" : model.RepositoryUrl + "/")}{model.RepositoryNameSpace}/{uploadImage.ToLower()}:{uploadTag}";

                CopyHelper.RunCommand($"{model.Sudo} docker rmi {uploadImageName}");

                string uploadCommand;
                string uploadCommandLog;
                if (string.IsNullOrEmpty(model.RepositoryUrl))
                {
                    uploadCommandLog =
                        $"{model.Sudo} docker login -u {model.RepositoryUserName} -p {{PWD}}; {model.Sudo} docker tag {currentImageInfo.Item3} {uploadImageName};{model.Sudo} docker push {uploadImageName}";

                    uploadCommand =
                        $"{model.Sudo} docker login -u {model.RepositoryUserName} -p {model.RepositoryUserPwd} & {model.Sudo} docker tag {currentImageInfo.Item3} {uploadImageName} & {model.Sudo} docker push {uploadImageName}";
                }
                else
                {
                    uploadCommandLog =
                        $"{model.Sudo} docker login -u {model.RepositoryUserName} -p {{PWD}} {model.RepositoryUrl}; {model.Sudo} docker tag {currentImageInfo.Item3} {uploadImageName};{model.Sudo} docker push {uploadImageName}";
                    uploadCommand =
                        $"{model.Sudo} docker login -u {model.RepositoryUserName} -p {model.RepositoryUserPwd} {model.RepositoryUrl} & {model.Sudo} docker tag {currentImageInfo.Item3} {uploadImageName} & {model.Sudo} docker push {uploadImageName}";
                }

                logger($"[upload image] - " + uploadCommandLog);
                var rr11 = CopyHelper.RunExternalExe(uploadCommand, (msg) =>
                {
                    logger($"[upload image] - {msg}");
                });

                
                CopyHelper.RunCommand($"{model.Sudo} docker rmi {uploadImageName}");
                
                if (!rr11)
                {
                    throw new Exception("[upload image] - Fail");
                }
                else
                {
                    logger($"[upload image] - Success");
                }
            }

        }

        public override void Execute()
        {
            retryTimes = 0;
            base.Execute();
        }

        public override void Rollback()
        {
            base.Rollback();
        }


        private bool CreateDockerFile(string path)
        {
            try
            {
                logger($"dockerFile not found in: [{path}],will create default Dockerfile!");
                string dllName = model.NetCoreENTRYPOINT;

                string sdkVersion = model.NetCoreVersion;
                if (string.IsNullOrEmpty(sdkVersion))
                {
                    sdkVersion = "3.1";
                }


                string environment = model.NetCoreEnvironment;
                logger($"create docker file: {path}");
                using (var writer = File.CreateText(path))
                {
                    //超过3.1版本的docker hub地址有变化
                    //docker pull mcr.microsoft.com/dotnet/aspnet:5.0
                    var versionNumber = sdkVersion.Replace(".", "");
                    int.TryParse(versionNumber, out var number);
                    string baseDockerImage = number > 31
                        ? $"FROM mcr.microsoft.com/dotnet/aspnet:{sdkVersion}"
                        : $"FROM mcr.microsoft.com/dotnet/core/aspnet:{sdkVersion}";
                    writer.WriteLine(baseDockerImage);

                    logger(baseDockerImage);

                    writer.WriteLine($"COPY . /publish");
                    logger($"COPY . /publish");

                    writer.WriteLine($"WORKDIR /publish");
                    logger($"WORKDIR /publish");

                    writer.WriteLine($"ENV ASPNETCORE_URLS=http://*:{model.ContainerPort}");
                    logger($"ENV ASPNETCORE_URLS=http://*:{model.ContainerPort}");

                    if (model.UseAsiaShanghai)
                    {
                        writer.WriteLine("RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime");
                        logger("RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime");
                    }

                    if (!string.IsNullOrEmpty(environment))
                    {
                        writer.WriteLine($"ENV ASPNETCORE_ENVIRONMENT={environment}");
                        logger($"ENV ASPNETCORE_ENVIRONMENT={environment}");
                    }

                    if (!model.ContainerPort.Equals("0"))
                    {
                        writer.WriteLine($"EXPOSE {model.ContainerPort}");
                        logger($"EXPOSE {model.ContainerPort}");
                    }
                    else
                    {
                        logger($"Ignore EXPOSE");
                    }


                    var excuteLine = $"ENTRYPOINT [\"dotnet\", \"{dllName}\"]";
                    writer.WriteLine(excuteLine);
                    logger(excuteLine);

                    if (!string.IsNullOrEmpty(model.NetCorePort) && model.NetCorePort.Contains(":"))
                    {
                        writer.WriteLine(serverPortProfix + model.ServerPort + "@");
                        logger(serverPortProfix + model.ServerPort + "@");
                    }

                    if (!string.IsNullOrEmpty(model.Volume))
                    {
                        writer.WriteLine(volumeProfix + model.Volume + "@");
                        logger(volumeProfix + model.Volume + "@");
                    }

                    if (!string.IsNullOrEmpty(model.Other))
                    {
                        writer.WriteLine(otherProfix + model.Other + "@");
                        logger(otherProfix + model.Other + "@");
                    }

                    writer.Flush();
                }
                logger($"create docker file success: {path}");
                return true;
            }
            catch (Exception ex)
            {
                logger($"create docker file fail: {path},err:{ex.Message}");
                return false;
            }
        }

        private void CheckDockerFile(string dockFilePath)
        {
            //如果项目中存在dockerFile 那么check 该DockerFile的Expose是否配置了 没有配置就读界面配置的，界面没有配置就用默认的
            try
            {
                var dockerFileText = File.ReadAllText(dockFilePath);
                if (string.IsNullOrEmpty(dockerFileText))
                {
                    logger($"dockerFile is empty: {dockFilePath}");
                    return;
                }
                var needAddPort = false;
                var newPort = string.Empty;
                var newPortA = dockerFileText.Split(new string[] { "EXPOSE " }, StringSplitOptions.None);
                if (newPortA.Length != 2)
                {
                    needAddPort = true;
                }
                else
                {
                    if (!newPortA[0].EndsWith("#"))
                    {
                        foreach (var item in newPortA[1].Trim())
                        {
                            if (Char.IsDigit(item))
                            {
                                newPort += item;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(newPort))
                {
                    needAddPort = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.NetCorePort) && !model.ContainerPort.Equals(newPort))
                    {
                        logger($"[Warn]EXPOSE in dockerFile is defined,will use【{newPort}】replace【{model.ContainerPort}】");
                    }
                    else
                    {
                        logger($"EXPOSE in dockerFile is : 【{newPort}】");
                    }

                }

                //如果dockfile里面没有配置EXPOST 就用界面上提供的
                model.RealPort = needAddPort ? model.ContainerPort : newPort;
                model.RealServerPort = model.ServerPort;
                var volumeInDockerFile = string.Empty;
                var volumeExist = dockerFileText.Split(new string[] { volumeProfix }, StringSplitOptions.None);
                if (volumeExist.Length == 2)
                {
                    var temp2 = volumeExist[1].Split('@');
                    if (temp2.Length > 0)
                    {
                        volumeInDockerFile = temp2[0];
                    }
                }

                if (!string.IsNullOrEmpty(volumeInDockerFile))
                {
                    //dockerFIle里面有配置 volume
                    if (!string.IsNullOrEmpty(model.Volume) && !model.Volume.Equals(volumeInDockerFile))
                    {
                        logger($"[Warn]Volume in dockerFile is defined,will use【{volumeInDockerFile}】replace【{model.Volume}】");
                    }
                    else
                    {
                        logger($"Volume in dockerFile is : 【{volumeInDockerFile}】");
                    }

                    model.Volume = volumeInDockerFile;
                }
                else
                {
                    logger($"[Warn]Volume in dockerFile is not defined");
                }


                var otherInDockerFile = string.Empty;
                var otherExist = dockerFileText.Split(new string[] { otherProfix }, StringSplitOptions.None);
                if (otherExist.Length == 2)
                {
                    var temp2 = otherExist[1].Split('@');
                    if (temp2.Length > 0)
                    {
                        otherInDockerFile = temp2[0];
                    }
                }

                if (!string.IsNullOrEmpty(otherInDockerFile))
                {
                    //dockerFIle里面有配置 Other Args
                    if (!string.IsNullOrEmpty(model.Other) && !model.Other.Equals(otherInDockerFile))
                    {
                        logger($"[Warn]Docker Run Other Args in dockerFile is defined,will use【{otherInDockerFile}】replace【{model.Other}】");
                    }
                    else
                    {
                        logger($"Docker Run Other Args in dockerFile is : 【{otherInDockerFile}】");
                    }

                    model.Other = otherInDockerFile;
                }
                else
                {
                    logger($"[Warn]Docker Run Other Args in dockerFile is not defined");
                }

                var serverPostDockerFile = string.Empty;
                var serverPostDockerFileExist = dockerFileText.Split(new string[] { serverPortProfix }, StringSplitOptions.None);
                if (serverPostDockerFileExist.Length == 2)
                {
                    var temp2 = serverPostDockerFileExist[1].Split('@');
                    if (temp2.Length > 0)
                    {
                        serverPostDockerFile = temp2[0];
                    }
                }

                if (!string.IsNullOrEmpty(serverPostDockerFile))
                {
                    //dockerFIle里面有配置 ServerPort
                    if (!string.IsNullOrEmpty(model.NetCorePort) && !model.ServerPort.Equals(serverPostDockerFile))
                    {
                        logger($"[Warn]ServerPort in dockerFile is defined,will use【{serverPostDockerFile}】replace【{model.ServerPort}】");
                    }
                    else
                    {
                        logger($"ServerPort in dockerFile is : 【{serverPostDockerFile}】");
                    }

                    model.RealServerPort = serverPostDockerFile;
                }
                else
                {
                    model.RealServerPort = needAddPort ? model.ServerPort : model.RealPort;
                }

                //如果dockerfile没有指定EXPOSE需要加进去
                var addV = false;
                if (!string.IsNullOrEmpty(model.NetCoreEnvironment) || needAddPort)
                {
                    var allLines = File.ReadAllLines(dockFilePath).ToList();
                    var entryPointIndex = 0;
                    var haveEnv = false;
                    for (int i = 0; i < allLines.Count; i++)
                    {
                        var line = allLines[i];
                        if (line.Trim().StartsWith("ENTRYPOINT"))
                        {
                            entryPointIndex = i;
                        }

                        if (line.Trim().StartsWith("ENV ASPNETCORE_ENVIRONMENT"))
                        {
                            haveEnv = true;
                        }
                    }


                    if (entryPointIndex > 0)
                    {
                        var add = false;
                        if (needAddPort && !model.RealPort.Equals("0"))
                        {
                            add = true;
                            allLines.Insert(entryPointIndex, "EXPOSE " + model.RealPort);
                            logger($"Add EXPOSE " + model.RealPort + $" to dockerFile  : 【{dockFilePath}】");

                            // 如果有自定义dockerfile且没有配置EXPOSE，除了加上EXPOSE以外还check下有没有配置urls
                            if (!dockerFileText.Contains("ENV ASPNETCORE_URLS=") && dockerFileText.Contains("dotnet"))
                            {
                                allLines.Insert(entryPointIndex, "ENV ASPNETCORE_URLS=http://*:" + model.RealPort);
                            }
                        }

                        if (!haveEnv && !string.IsNullOrEmpty(model.NetCoreEnvironment))
                        {
                            add = true;
                            allLines.Insert(entryPointIndex, "ENV ASPNETCORE_ENVIRONMENT " + model.NetCoreEnvironment);
                            logger($"Add ENV ASPNETCORE_ENVIRONMENT " + model.NetCoreEnvironment + $" to dockerFile  : 【{dockFilePath}】");
                        }

                        if (add)
                        {
                            File.Delete(dockFilePath);
                            //没有发现包含环境变量 就添加进去
                            using (var writer = File.CreateText(dockFilePath))
                            {
                                foreach (var line in allLines)
                                {
                                    writer.WriteLine(line);
                                }
                                //发布的时候界面上有填volume 也存在dockerfile 要记录到dockerfile中 不然回滚的时候就没了
                                if (string.IsNullOrEmpty(volumeInDockerFile) && !string.IsNullOrEmpty(model.Volume))
                                {
                                    addV = true;
                                    writer.WriteLine(volumeProfix + model.Volume + "@");
                                    logger(volumeProfix + model.Volume + "@");
                                }

                                if (string.IsNullOrEmpty(otherInDockerFile) && !string.IsNullOrEmpty(model.Other))
                                {
                                    addV = true;
                                    writer.WriteLine(otherProfix + model.Other + "@");
                                    logger(otherProfix + model.Other + "@");
                                }
                                writer.Flush();
                            }
                        }
                    }
                }
                //发布的时候界面上有填volume 也存在dockerfile 要记录到dockerfile中 不然回滚的时候就没了
                if (!addV)
                {

                    if ((string.IsNullOrEmpty(volumeInDockerFile) && !string.IsNullOrEmpty(model.Volume)) ||
                        (string.IsNullOrEmpty(otherInDockerFile) && !string.IsNullOrEmpty(model.Other)))
                    {
                        //发布的时候界面上有填volume 也存在dockerfile 要记录到dockerfile中 不然回滚的时候就没了
                        var allLines = File.ReadAllLines(dockFilePath).ToList();
                        File.Delete(dockFilePath);
                        using (var writer = File.CreateText(dockFilePath))
                        {
                            foreach (var line in allLines)
                            {
                                writer.WriteLine(line);
                            }

                            if (string.IsNullOrEmpty(volumeInDockerFile) && !string.IsNullOrEmpty(model.Volume))
                            {
                                writer.WriteLine(volumeProfix + model.Volume + "@");
                                logger(volumeProfix + model.Volume);
                            }


                            if (string.IsNullOrEmpty(otherInDockerFile) && !string.IsNullOrEmpty(model.Other))
                            {
                                writer.WriteLine(otherProfix + model.Other + "@");
                                logger(otherInDockerFile + model.Other + "@");
                            }
                            writer.Flush();
                        }
                    }

                }

                //需要将修改过的DockerFile 移动到 发布文件夹的publish 目录下 不然会导致回滚的时候失败
                var oldFile = Path.Combine(args.DeployFolder, "Dockerfile");
                File.Copy(dockFilePath, oldFile);
                logger($"Update DockerFile 【{oldFile}】");
            }
            catch (Exception ex)
            {
                logger($"parse param in dockerFile fail: {dockFilePath},err:{ex.Message}");
                return;
            }
        }

        private string GetVolume(bool ignoreFirstSpace = false)
        {
            if (string.IsNullOrEmpty(model.Volume)) return string.Empty;
            var volume = model.Volume.Replace('；', ';');
            var arr = volume.Split(';');
            var result = new List<string>();
            foreach (var item in arr)
            {
                if (!item.Contains(':'))
                {
                    logger("[Warn]ignore volume set,because is not correct!");
                    return string.Empty;
                }
                result.Add($"-v {item}");
            }
            if (!result.Any()) return string.Empty;
            return (!ignoreFirstSpace ? " " : "") + string.Join(" ", result);
        }
    }
}
