using NLog;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AntDeployWinform.Util
{
    public class SSHClient : IDisposable
    {
        private string volumeProfix = "# volume@";
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string NetCoreVersion { get; set; }

        public string PorjectName
        {
            get
            {
                if (!string.IsNullOrEmpty(NetCoreENTRYPOINT))
                {
                    return GetSafeProjectName(NetCoreENTRYPOINT.Replace(".dll", "")).ToLower();
                }

                return string.Empty;
            }
        }

        public string NetCorePort { get; set; }
        public string NetCoreEnvironment { get; set; }
        public string NetCoreENTRYPOINT { get; set; }
        public string ClientDateTimeFolderName { get; set; }
        public string RemoveDaysFromPublished { get; set; }
        public string Remark { get; set; }
        public string Volume { get; set; }
        public string RootFolder { get; set; }

        private readonly Action<string, NLog.LogLevel> _logger;
        private readonly Action<int> _uploadLogger;

        private readonly SftpClient _sftpClient;
        private readonly SshClient _sshClient;
        private long _lastProgressNumber;
        private object lockObject = new object();
        public SSHClient(string host, string userName, string pwd, Action<string, NLog.LogLevel> logger, Action<int> uploadLogger)
        {
            this.UserName = userName;
            this.Pwd = pwd;
            _logger = logger;
            _uploadLogger = uploadLogger;
            var harr = host.Split(':');
            this.Host = harr[0];
            var hPort = 22;
            if (harr.Length == 2)
            {
                hPort = int.Parse(harr[1]);
            }
            _sftpClient = new SftpClient(this.Host, hPort, userName, pwd);
            _sshClient = new SshClient(this.Host, hPort, userName, pwd);
            _sftpClient.BufferSize = 6 * 1024; // bypass Payload error large files
        }


        public bool Connect(bool ignoreLog = false)
        {
            try
            {
                if (!ignoreLog) _logger("ssh connecting " + Host + "... ", NLog.LogLevel.Info);
                _sshClient.Connect();
                _sftpClient.Connect();
                if (_sshClient.IsConnected && _sftpClient.IsConnected)
                {
                    if (!ignoreLog)
                    {
                        RootFolder = _sftpClient.WorkingDirectory;
                        _logger($"ssh connect success:{Host}", NLog.LogLevel.Info);
                    }
                    return true;

                }

                if (!ignoreLog) _logger($"ssh connect fail", NLog.LogLevel.Error);
                return false;
            }
            catch (Exception ex)
            {
                if (!ignoreLog) _logger($"ssh connect fail:{ex.Message}", NLog.LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="destinationFolder">服务器目录</param>
        /// <param name="fileName">保存的文件名</param>
        public void Upload(Stream stream, string destinationFolder, string fileName)
        {
            if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";



            //创建文件夹
            CreateServerDirectoryIfItDoesntExist(destinationFolder);


            //按照项目来区分文件夹 例如
            //  /publisher/aaa
            //  /publisher/aaa/publish.zip
            //  /publisher/aaa/deploy
            //删除zip文件
            if (_sftpClient.Exists(destinationFolder + fileName))
            {
                _sftpClient.Delete(destinationFolder + fileName);
            }

            var publishUnzipFolder = destinationFolder + "publish";
            //删除发布文件夹
            if (_sftpClient.Exists(publishUnzipFolder))
            {
                DeleteDirectory(publishUnzipFolder);
            }

            CreateServerDirectoryIfItDoesntExist(publishUnzipFolder + "/");

            ChangeToFolder(destinationFolder);

            var fileSize = stream.Length;
            _sftpClient.UploadFile(stream, fileName, (uploaded) => { uploadProgress(fileSize, uploaded); });




        }

        public void ChangeToFolder(string changeTo)
        {
            if (changeTo.StartsWith("/"))
            {
                changeTo = changeTo.Substring(1);
            }
            _sftpClient.ChangeDirectory(changeTo);
            _logger($"Changed directory to {changeTo}", NLog.LogLevel.Info);
        }

        private void uploadProgress(long size, ulong uploadedSize)
        {
            lock (lockObject)
            {
                var lastProgressNumber = (((long)uploadedSize * 100 / size));

                if (lastProgressNumber < 1)
                {
                    return;
                }

                if (lastProgressNumber <= _lastProgressNumber)
                {
                    return;
                }

                if (lastProgressNumber.ToString().Length == 2 && _lastProgressNumber.ToString().First() == lastProgressNumber.ToString().First())
                {
                    return;
                }
                _lastProgressNumber = lastProgressNumber;
                _logger($"uploaded {lastProgressNumber} %", NLog.LogLevel.Info);

                _uploadLogger((int)lastProgressNumber);

            }

        }

        /// <summary>
        /// 获取发布的历史
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="pageNumber">数量</param>
        /// <returns></returns>
        public List<string> GetDeployHistoryWithOutRemark(string destinationFolder, int pageNumber = 0)
        {
            var result = new List<Tuple<string, DateTime>>();
            try
            {

                if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

                destinationFolder = destinationFolder + PorjectName + "/";


                if (!_sftpClient.Exists(destinationFolder))
                {
                    return new List<string>();
                }

                //获取该目录下的所有日期文件夹
                List<SftpFile> folderList;
                if (pageNumber < 1)
                {
                    folderList = _sftpClient.ListDirectory(destinationFolder).Where(r => r.IsDirectory).
                        OrderByDescending(r => r.LastWriteTime).ToList();
                }
                else
                {
                    folderList = _sftpClient.ListDirectory(destinationFolder).Where(r => r.IsDirectory).
                        OrderByDescending(r => r.LastWriteTime).Take(pageNumber).ToList();
                }

                foreach (var folder in folderList)
                {
                    if ((folder.Name == ".") || (folder.Name == "..")) continue;
                    if (DateTime.TryParseExact(folder.Name, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
                    {
                        result.Add(new Tuple<string, DateTime>(folder.Name, d));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger(ex.Message, LogLevel.Error);
            }

            return result.OrderByDescending(r => r.Item2).Select(r => r.Item1).ToList();
        }

        /// <summary>
        /// 获取发布的历史
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="pageNumber">数量</param>
        /// <returns></returns>
        public List<Tuple<string, string>> GetDeployHistory(string destinationFolder, int pageNumber = 0)
        {
            var result = new List<Tuple<string, string, DateTime>>();
            try
            {

                if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

                destinationFolder = destinationFolder + PorjectName + "/";


                if (!_sftpClient.Exists(destinationFolder))
                {
                    return new List<Tuple<string, string>>();
                }

                //获取该目录下的所有日期文件夹
                List<SftpFile> folderList;
                if (pageNumber < 1)
                {
                    folderList = _sftpClient.ListDirectory(destinationFolder).Where(r => r.IsDirectory).
                        OrderByDescending(r => r.LastWriteTime).ToList();
                }
                else
                {
                    folderList = _sftpClient.ListDirectory(destinationFolder).Where(r => r.IsDirectory).
                        OrderByDescending(r => r.LastWriteTime).Take(pageNumber).ToList();
                }

                foreach (var folder in folderList)
                {
                    if ((folder.Name == ".") || (folder.Name == "..")) continue;
                    if (DateTime.TryParseExact(folder.Name, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
                    {
                        string remark = string.Empty;
                        try
                        {
                            var path = folder.FullName + (folder.FullName.EndsWith("/") ? "antdeploy_args" : "/antdeploy_args");
                            remark = _sftpClient.ReadAllText(path);
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                        result.Add(new Tuple<string, string, DateTime>(folder.Name, remark, d));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger(ex.Message, LogLevel.Error);
            }

            return result.OrderByDescending(r => r.Item3).Select(r => new Tuple<string, string>(r.Item1, r.Item2)).ToList();
        }

        public void PublishZip(Stream stream, string destinationFolder, string destinationfileName)
        {
            if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

            var projectPath = destinationFolder + PorjectName + "/";
            destinationFolder = projectPath + ClientDateTimeFolderName + "/";

            Upload(stream, destinationFolder, destinationfileName);

            if (!_sftpClient.Exists(destinationfileName))
            {
                _logger($"upload fail, {destinationfileName} not exist!", NLog.LogLevel.Error);
                return;
            }

            //创建args文件 antdeploy_args
            var argsFilePath = destinationFolder + "antdeploy_args";
            CreateArgsFile(argsFilePath);

            _logger($"tar -xf {destinationFolder + destinationfileName} -C publish", NLog.LogLevel.Info);
            var unzipresult = _sshClient.RunCommand($"cd {destinationFolder} && tar -xf {destinationfileName} -C publish");
            if (unzipresult.ExitStatus != 0)
            {
                _logger($"excute tar command error,return status is not 0", NLog.LogLevel.Error);
                _logger($"please check 【tar】 is installed in your server!", NLog.LogLevel.Error);
                return;
            }
            var publishFolder = $"{destinationFolder}publish/";
            if (!_sftpClient.Exists("publish"))
            {
                _logger($"tar fail: {publishFolder}", NLog.LogLevel.Error);
                return;
            }
            _logger($"tar success: {publishFolder}", NLog.LogLevel.Info);


            //执行Docker命令

            DoDockerCommand(publishFolder);


        }

        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="version">具体的日期文件夹路径</param>
        public void RollBack(string version)
        {
            var path = "antdeploy/" + PorjectName + "/" + version + "/";

            ChangeToFolder(path);

            var publishFolder = $"{path}publish/";
            if (!_sftpClient.Exists("publish"))
            {
                _logger($"rollback fail: {publishFolder} not exist", NLog.LogLevel.Error);
                return;
            }

            ClientDateTimeFolderName = version;
            DoDockerCommand(publishFolder, true);
        }

        public void DoDockerCommand(string publishFolder, bool isrollBack = false)
        {
            if (!publishFolder.EndsWith("/")) publishFolder = publishFolder + "/";
            //先查看本地是否有dockerFile
            var dockFilePath = publishFolder + "Dockerfile";
            var dockFilePath2 = "publish/Dockerfile";
            var isExistDockFile = _sftpClient.Exists(dockFilePath2);
            //如果本地存在dockerfile 那么就根据此创建image
            //如果不存在的话 就根据当前的netcore sdk的版本 进行创建相对应的 dockfile
            if (!isExistDockFile)
            {
                if (isrollBack)
                {
                    _logger($"dockerFile is not exist: {dockFilePath}", NLog.LogLevel.Error);
                    return;
                }
                var createDockerFileResult = CreateDockerFile(dockFilePath);
                if (!createDockerFileResult) return;
            }
            else
            {
                //如果项目中存在dockerFile 那么check 该DockerFile的Expose是否配置了 没有配置就报错
                try
                {
                    var dockerFileText = _sftpClient.ReadAllText(dockFilePath);
                    if (string.IsNullOrEmpty(dockerFileText))
                    {
                        _logger($"dockerFile is empty: {dockFilePath}", NLog.LogLevel.Error);
                        return;
                    }

                    var newPortA = dockerFileText.Split(new string[] { "EXPOSE " }, StringSplitOptions.None);
                    if (newPortA.Length != 2)
                    {
                        _logger($"EXPOSE param in dockerFile is empty: {dockFilePath}", NLog.LogLevel.Error);
                        return;
                    }
                    var newPort = string.Empty;
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
                    if (string.IsNullOrEmpty(newPort))
                    {
                        _logger($"EXPOSE in dockerFile is invalid: {dockFilePath}", NLog.LogLevel.Error);
                        return;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(NetCorePort) && !NetCorePort.Equals(newPort))
                        {
                            _logger($"EXPOSE in dockerFile is defined,will use【{newPort}】replace【{NetCorePort}】", NLog.LogLevel.Warn);
                        }
                        else
                        {
                            _logger($"EXPOSE in dockerFile is : 【{newPort}】", NLog.LogLevel.Info);
                        }

                    }

                    NetCorePort = newPort;

                    var volumeInDockerFile = string.Empty;
                    var volumeExist = dockerFileText.Split(new string[] { volumeProfix }, StringSplitOptions.None);
                    if (volumeExist.Length == 2)
                    {
                        var temp2 = volumeExist[1].Split('@');
                        if (temp2.Length == 2)
                        {
                            volumeInDockerFile = temp2[0];
                        }
                    }

                    if (!string.IsNullOrEmpty(volumeInDockerFile))
                    {
                        //dockerFIle里面有配置 volume
                        if (!string.IsNullOrEmpty(Volume) && !Volume.Equals(volumeInDockerFile))
                        {
                            _logger($"Volume in dockerFile is defined,will use【{volumeInDockerFile}】replace【{Volume}】", NLog.LogLevel.Warn);
                        }
                        else
                        {
                            _logger($"Volume in dockerFile is : 【{volumeInDockerFile}】", NLog.LogLevel.Info);
                        }

                        Volume = volumeInDockerFile;
                    }
                }
                catch (Exception)
                {
                    _logger($"Get EXPOSE param in dockerFile fail: {dockFilePath}", NLog.LogLevel.Error);
                    return;
                }
            }

            //执行docker build 生成一个镜像
            var dockerBuildResult = RunSheell($"sudo docker build --no-cache --rm -t {PorjectName}:{ClientDateTimeFolderName} -f {dockFilePath} {publishFolder} ");
            if (!dockerBuildResult)
            {
                _logger($"build image fail", NLog.LogLevel.Error);
                return;
            }

            var continarName = "d_" + PorjectName;


            //先发送退出命令
            //https://stackoverflow.com/questions/40742192/how-to-do-gracefully-shutdown-on-dotnet-with-docker

            SshCommand r1 = null;
            try
            {
                r1 = _sshClient.RunCommand($"sudo docker stop -t 10 {continarName}");
                if (r1.ExitStatus == 0)
                {
                    _logger($"sudo docker stop -t 10 {continarName}", LogLevel.Info);
                }

                Thread.Sleep(5000);
            }
            catch (Exception)
            {

            }

            try
            {
                //查看容器有没有在runing 如果有就干掉它
                r1 = _sshClient.RunCommand($"sudo docker rm -f {continarName}");
                if (r1.ExitStatus == 0)
                {
                    _logger($"sudo docker rm -f {continarName}", LogLevel.Info);
                }
            }
            catch (Exception)
            {

            }


            string port = NetCorePort;
            if (string.IsNullOrEmpty(port))
            {
                port = "5000";
            }

            string volume = GetVolume();

            // 根据image启动一个容器
            var dockerRunRt = RunSheell($"sudo docker run --name {continarName}{volume} -d --restart=always -p {port}:{port} {PorjectName}:{ClientDateTimeFolderName}");

            if (!dockerRunRt)
            {
                _logger($"docker run fail", NLog.LogLevel.Error);
                return;
            }

            //把旧的image给删除
            r1 = _sshClient.RunCommand("docker images --format '{{.Repository}}:{{.Tag}}:{{.ID}}' | grep '^" + PorjectName + ":'");
            if (r1.ExitStatus == 0 && !string.IsNullOrEmpty(r1.Result))
            {
                var deleteImageArr = r1.Result.Split('\n');
                var clearOldImages = false;
                foreach (var imageName in deleteImageArr)
                {
                    if (imageName.StartsWith($"{PorjectName}:{ClientDateTimeFolderName}:"))
                    {
                        //当前版本
                        continue;
                    }

                    var imageArr = imageName.Split(':');
                    if (imageArr.Length == 3)
                    {
                        var r2 = _sshClient.RunCommand($"sudo docker rmi {imageArr[2]}");
                        if (r2.ExitStatus == 0)
                        {
                            if (!clearOldImages)
                            {
                                _logger($"start to clear old images of name:{PorjectName}", LogLevel.Info);
                                clearOldImages = true;
                            }
                            _logger($"sudo docker rmi {imageArr[2]} [{imageName}]", LogLevel.Info);
                        }
                    }
                }

            }


            //查看是否有<none>的image 把它删掉 因为我们创建image的时候每次都会覆盖所以会产生一些没有的image
            //_sshClient.RunCommand($"if sudo docker images -f \"dangling=true\" | grep ago --quiet; then sudo docker rmi -f $(sudo docker images -f \"dangling=true\" -q); fi");


            ClearOldHistroy();

        }


        public void ClearOldHistroy()
        {
            if (string.IsNullOrEmpty(RemoveDaysFromPublished)) return;

            if (!int.TryParse(RemoveDaysFromPublished, out var _removeDays))
            {
                return;
            }
            //删除超过10天以上的发布版本目录
            if (string.IsNullOrEmpty(RootFolder))
            {
                return;
            }

            _sftpClient.ChangeDirectory(RootFolder);
            var now = DateTime.Now.Date;
            var histroryList = GetDeployHistoryWithOutRemark("antdeploy");
            if (histroryList.Count <= 10) return;
            var oldFolderList = new List<OldFolder>();
            foreach (var histroy in histroryList)
            {
                if (!DateTime.TryParseExact(histroy, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime createDate))
                {
                    continue;
                }

                oldFolderList.Add(new OldFolder
                {
                    DateTime = createDate,
                    Name = histroy,
                    DiffDays = (now - createDate.Date).TotalDays
                });
            }

            var targetList = oldFolderList.OrderByDescending(r => r.DateTime)
                .Where(r => r.DiffDays >= _removeDays)
                .ToList();

            var diff = histroryList.Count - targetList.Count;

            if (diff >= 0 && diff < 10)
            {
                targetList = targetList.Skip(10 - diff).ToList();
            }

            if (targetList.Any())
            {
                _logger($"Remove backup version that have been published for more than:{_removeDays} days", LogLevel.Info);
            }
            foreach (var target in targetList)
            {
                try
                {
                    var toDelete = $"antdeploy/{PorjectName}/{target.Name}/";
                    this.DeleteDirectory(toDelete);
                    _logger($"Remove backup version success: {toDelete}", LogLevel.Info);
                }
                catch
                {
                    //ignore
                }
            }
        }





        public void DeletePublishFolder(string destinationFolder)
        {
            if (string.IsNullOrEmpty(RootFolder))
            {
                return;
            }

            _sftpClient.ChangeDirectory(RootFolder);

            if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

            if (string.IsNullOrEmpty(PorjectName) || string.IsNullOrEmpty(ClientDateTimeFolderName)) return;

            destinationFolder = destinationFolder + PorjectName + "/" + ClientDateTimeFolderName + "/";

            this.DeleteDirectory(destinationFolder);
        }


        private bool CreateArgsFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(Remark)) return true;
                using (var writer = _sftpClient.CreateText(path))
                {
                    writer.WriteLine($"{Remark}");
                    writer.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger($"create deploy remark file fail: {path},err:{ex.Message}", NLog.LogLevel.Error);
                return false;
            }
        }


        private bool CreateDockerFile(string path)
        {
            try
            {
                string dllName = NetCoreENTRYPOINT;

                string sdkVersion = NetCoreVersion;
                if (string.IsNullOrEmpty(sdkVersion))
                {
                    sdkVersion = "2.1";
                }

                string port = NetCorePort;
                if (string.IsNullOrEmpty(port))
                {
                    port = "5000";
                }


                string environment = NetCoreEnvironment;

                _logger($"create docker file: {path}", NLog.LogLevel.Info);
                using (var writer = _sftpClient.CreateText(path))
                {
                    writer.WriteLine($"FROM microsoft/dotnet:{sdkVersion}-aspnetcore-runtime");
                    _logger($"FROM microsoft/dotnet:{sdkVersion}-aspnetcore-runtime", NLog.LogLevel.Info);

                    writer.WriteLine($"COPY . /publish");
                    _logger($"COPY . /publish", NLog.LogLevel.Info);

                    writer.WriteLine($"WORKDIR /publish");
                    _logger($"WORKDIR /publish", NLog.LogLevel.Info);

                    writer.WriteLine($"ENV ASPNETCORE_URLS=http://*:{port}");
                    _logger($"ENV ASPNETCORE_URLS=http://*:{port}", NLog.LogLevel.Info);

                    if (!string.IsNullOrEmpty(environment))
                    {
                        writer.WriteLine($"ENV ASPNETCORE_ENVIRONMENT={environment}");
                        _logger($"ENV ASPNETCORE_ENVIRONMENT={environment}", NLog.LogLevel.Info);
                    }

                    writer.WriteLine($"EXPOSE {port}");
                    _logger($"EXPOSE {port}", NLog.LogLevel.Info);

                    var excuteLine = $"ENTRYPOINT [\"dotnet\", \"{dllName}\"]";
                    //var excuteCMDLine = $"CMD [\"--server.urls\", \"http://*:{port}\"";

                    //if (!string.IsNullOrEmpty(environment))
                    //{
                    //     excuteCMDLine+= $",\"--environment\", \"{environment}\"";
                    //}
                    //excuteCMDLine+="]";
                    writer.WriteLine(excuteLine);
                    _logger(excuteLine, NLog.LogLevel.Info);
                    //writer.WriteLine(excuteCMDLine);
                    //_logger(excuteCMDLine);

                    if (!string.IsNullOrEmpty(this.Volume))
                    {
                        writer.WriteLine(volumeProfix + this.Volume + "@");
                        _logger(volumeProfix + this.Volume + "@", NLog.LogLevel.Info);
                    }

                    writer.Flush();
                }
                _logger($"create docker file success: {path}", NLog.LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _logger($"create docker file fail: {path},err:{ex.Message}", NLog.LogLevel.Error);
                return false;
            }
        }


        public bool RunSheell(string command)
        {
            SshCommand cmd = _sshClient.CreateCommand(command);
            var result = cmd.BeginExecute();
            _logger(command, LogLevel.Info);
            using (var reader = new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
            {
                while (!result.IsCompleted || !reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        _logger(line, LogLevel.Info);
                    }
                }
            }

            cmd.EndExecute(result);

            if (!string.IsNullOrEmpty(cmd.Error))
            {
                if (cmd.Error.Contains("unable to resolve host 127.0.0.1localhost.localdomainlocalhost"))
                {
                    _logger(cmd.Error, LogLevel.Warn);
                    return true;
                }
                _logger(cmd.Error, LogLevel.Error);
                return false;
            }

            return true;
        }

        public void DeleteFile(string path)
        {
            try
            {
                _logger($"delete file: {path}", NLog.LogLevel.Info);
                _sftpClient.Delete(path);
                _logger($"delete file success: {path}", NLog.LogLevel.Info);
            }
            catch (Exception ex)
            {
                _logger($"delete file fail: {path},err:{ex.Message}", NLog.LogLevel.Error);
            }
        }


        public void DeleteFolder(string folder)
        {
            try
            {
                _logger($"delete folder: {folder}", NLog.LogLevel.Info);
                DeleteDirectory(folder);
                _logger($"delete folder success: {folder}", NLog.LogLevel.Info);
            }
            catch (Exception ex)
            {
                _logger($"delete folder fail: {folder},err:{ex.Message}", NLog.LogLevel.Error);
            }
        }


        public void Dispose()
        {
            try
            {
                _sftpClient.Disconnect();
                _sftpClient.Dispose();
            }
            catch (Exception)
            {

            }

            try
            {
                _sshClient.Disconnect();
                _sshClient.Dispose();
            }
            catch (Exception)
            {

            }
        }
        private void DeleteDirectory(string path)
        {
            foreach (SftpFile file in _sftpClient.ListDirectory(path))
            {
                if ((file.Name != ".") && (file.Name != ".."))
                {
                    if (file.IsDirectory)
                    {
                        DeleteDirectory(file.FullName);
                    }
                    else
                    {
                        _sftpClient.DeleteFile(file.FullName);
                    }
                }
            }

            _sftpClient.DeleteDirectory(path);
        }

        /// <summary>
        /// 递归创建文件夹
        /// </summary>
        /// <param name="serverDestinationPath"></param>
        private void CreateServerDirectoryIfItDoesntExist(string serverDestinationPath)
        {
            if (serverDestinationPath[0] == '/')
                serverDestinationPath = serverDestinationPath.Substring(1);

            string[] directories = serverDestinationPath.Split('/');
            for (int i = 0; i < directories.Length; i++)
            {
                string dirName = string.Join("/", directories, 0, i + 1);
                if (!_sftpClient.Exists(dirName))
                    _sftpClient.CreateDirectory(dirName);
            }
        }


        private string GetSafeProjectName(string projectName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                projectName = projectName.Replace(System.Char.ToString(c), "");
            }
            var aa = Regex.Replace(projectName, "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
            aa = aa.Replace(" ", "").Replace("　", "");
            aa = Regex.Replace(aa, @"[~!@#\$%\^&\*\(\)\+=\|\\\}\]\{\[:;<,>\?\/""]+", "");
            return aa;
        }

        private string GetVolume(bool ignoreFirstSpace = false)
        {
            if (string.IsNullOrEmpty(this.Volume)) return string.Empty;
            var volume = Volume.Replace('；', ';');
            var arr = volume.Split(';');
            var result = new List<string>();
            foreach (var item in arr)
            {
                if (!item.Contains(':'))
                {
                    this._logger("ignore volume set,because is not correct!", LogLevel.Error);
                    return string.Empty;
                }
                result.Add($"-v {item}");
            }
            if (!result.Any()) return string.Empty;
            return (!ignoreFirstSpace ? " " : "") + string.Join(" ", result);
        }
    }



    class OldFolder
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public double DiffDays { get; set; }
    }



}
