using NLog;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AntDeployWinform.Models;
using Newtonsoft.Json;

namespace AntDeployWinform.Util
{
    public class SSHClient : IDisposable
    {
        private string volumeProfix = "# volume@";
        private string serverPortProfix = "# server_port@";
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string NetCoreVersion { get; set; }

        public event EventHandler<UploadEventArgs> UploadEvent;

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

        /// <summary>
        /// 宿主机开启的端口
        /// </summary>
        public string ServerPort
        {
            get
            {
                if (string.IsNullOrEmpty(this.NetCorePort)) return "5000";

                if (this.NetCorePort.Contains(":"))
                {
                    return this.NetCorePort.Split(':')[0];
                }
                else
                {
                    return NetCorePort;
                }
            }
        }

        /// <summary>
        /// 容器暴露的端口
        /// </summary>
        public string ContainerPort
        {
            get
            {
                if (string.IsNullOrEmpty(this.NetCorePort)) return "5000";

                if (this.NetCorePort.Contains(":"))
                {
                    return this.NetCorePort.Split(':')[1];
                }
                else
                {
                    return NetCorePort;
                }
            }
        }

        #region 镜像上传
        public bool DockerServiceEnableUpload { get; set; }
        public bool DockerServiceBuildImageOnly { get; set; }
        public string RepositoryUrl { get; set; }
        public string RepositoryUserName { get; set; }
        public string RepositoryUserPwd { get; set; }
        public string RepositoryNameSpace { get; set; }
        public string RepositoryImageName { get; set; }


        #endregion

        public string NetCoreEnvironment { get; set; }
        public string NetCoreENTRYPOINT { get; set; }
        public bool Increment { get; set; }
        public bool IsSelect { get; set; }
        public string ClientDateTimeFolderName { get; set; }
        public string RemoveDaysFromPublished { get; set; }
        public string Remark { get; set; }
        public string Volume { get; set; }
        public string RootFolder { get; set; }

        private readonly Func<string, NLog.LogLevel, bool> _logger;
        private readonly Action<int> _uploadLogger;

        private readonly SftpClient _sftpClient;
        private readonly SshClient _sshClient;
        private long _lastProgressNumber;
        private readonly IDisposable _subscribe;
        public SSHClient(string host, string userName, string pwd,string proxy ,Func<string, NLog.LogLevel,bool> logger, Action<int> uploadLogger)
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
            var useProxy = false;
            if (!string.IsNullOrEmpty(proxy))
            {
                var arr = proxy.Split(':');
                if (arr.Length == 2)
                {
                    ConnectionInfo infoConnection = new ConnectionInfo(this.Host, hPort, userName, ProxyTypes.Http, arr[0], int.Parse(arr[1]), null, null, new PasswordAuthenticationMethod(userName, pwd));
                    _sftpClient = new SftpClient(infoConnection);
                    _sshClient = new SshClient(infoConnection);
                    useProxy = true;
                }
            }

            if (!useProxy)
            {
                _sftpClient = new SftpClient(this.Host, hPort, userName, pwd);
                _sshClient = new SshClient(this.Host, hPort, userName, pwd);
            }

            _sftpClient.BufferSize = 6 * 1024; // bypass Payload error large files


            _subscribe = System.Reactive.Linq.Observable
                .FromEventPattern<UploadEventArgs>(this, "UploadEvent")
                .Sample(TimeSpan.FromMilliseconds(50))
                .Subscribe(arg => { OnUploadEvent(arg.Sender, arg.EventArgs); });
        }

        private void OnUploadEvent(object sender, UploadEventArgs e)
        {
            this.uploadProgress(e.size,e.uploadedSize);
        }

        public SSHClient(string host, string userName, string pwd,string proxy = null)
        {
            this.UserName = userName;
            this.Pwd = pwd;
            _logger = (a,b) => { Console.WriteLine(a);
                return false;
            };
            _uploadLogger = Console.WriteLine;
            var harr = host.Split(':');
            this.Host = harr[0];
            var hPort = 22;
            if (harr.Length == 2)
            {
                hPort = int.Parse(harr[1]);
            }

            var useProxy = false;
            if (!string.IsNullOrEmpty(proxy))
            {
                var arr = proxy.Split(':');
                if (arr.Length == 2)
                {
                    ConnectionInfo infoConnection = new ConnectionInfo(this.Host, hPort, userName, ProxyTypes.Http, arr[0], int.Parse(arr[1]), null, null, new PasswordAuthenticationMethod(userName, pwd));
                    _sftpClient = new SftpClient(infoConnection);
                    _sshClient = new SshClient(infoConnection);
                    useProxy = true;
                }
            }
            
            if (!useProxy)
            {
                _sftpClient = new SftpClient(this.Host, hPort, userName, pwd);
                _sshClient = new SshClient(this.Host, hPort, userName, pwd);
            }
            _sftpClient.BufferSize = 6 * 1024; // bypass Payload error large files

            _subscribe = System.Reactive.Linq.Observable
                .FromEventPattern<UploadEventArgs>(this, "UploadEvent")
                .Sample(TimeSpan.FromMilliseconds(100))
                .Subscribe(arg => { OnUploadEvent(arg.Sender, arg.EventArgs); });

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
                        var _connectionInfo = _sshClient.ConnectionInfo;
                        if (_connectionInfo != null && !string.IsNullOrEmpty(_connectionInfo.Host))
                        {
                            _logger($"Connected to {_connectionInfo.Username}@{_connectionInfo.Host}:{_connectionInfo.Port} via SSH", NLog.LogLevel.Info);
                        }
                        else
                        {
                            _logger($"ssh connect success:{Host}", NLog.LogLevel.Info);
                        }
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
            _lastProgressNumber = 0;

            _sftpClient.UploadFile(stream, fileName, (uploaded) =>
            {
                if (UploadEvent != null)
                {
                    UploadEvent(this, new UploadEventArgs
                    {
                        size = fileSize,
                        uploadedSize = uploaded
                    });
                }
                else
                {
                    uploadProgress(fileSize, uploaded);
                }
            });

            if (_lastProgressNumber != 100)
            {
                _uploadLogger(100);
            }

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
            var lastProgressNumber = (((long)uploadedSize * 100 / size));

            if (lastProgressNumber < 1)
            {
                return;
            }

            if (lastProgressNumber <= _lastProgressNumber)
            {
                return;
            }

            _lastProgressNumber = lastProgressNumber;
            var isCanceled = _logger($"uploaded {lastProgressNumber} %", NLog.LogLevel.Info);
            if (isCanceled)
            {
               this.Dispose();
            }
            _uploadLogger((int)_lastProgressNumber);
        }

        /// <summary>
        /// 获取发布的历史 删除过期的
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
                    var temp = folder.Name.Replace("_", "");
                    if (DateTime.TryParseExact(temp, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
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
        /// 获取发布的历史 用来展示给用户的
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="pageNumber">数量</param>
        /// <returns></returns>
        public List<Tuple<string, string>> GetDeployHistory(string destinationFolder, int pageNumber = 0)
        {
            var dic = new Dictionary<string,Tuple<string,string,DateTime>>();
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
                    var temp = folder.Name.Replace("_", "");
                    if (DateTime.TryParseExact(temp, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
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

                        if (dic.ContainsKey(temp))
                        {
                            var value = dic[temp];
                            if (value.Item1.Length < folder.Name.Length)
                            {
                                dic[temp] = new Tuple<string, string, DateTime>(folder.Name,remark,d);
                            }
                        }
                        else
                        {
                            dic.Add(temp,new Tuple<string, string, DateTime>(folder.Name,remark,d));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger(ex.Message, LogLevel.Error);
            }

            return dic.Values.ToList().OrderByDescending(r => r.Item3).Select(r => new Tuple<string, string>(r.Item1, r.Item2)).ToList();
        }

        public void PublishZip(Stream stream, string destinationFolder, string destinationfileName,Func<bool> continuetask = null)
        {
            bool CheckCancel()
            {
                if (continuetask != null)
                {
                    var canContinue = continuetask();
                    if (!canContinue)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";
            //按照项目分文件夹
            var projectPath = destinationFolder + PorjectName + "/";
            //再按发布版本分文件夹
            destinationFolder = projectPath + ClientDateTimeFolderName + "/";

            //创建项目根目录
            var deploySaveFolder = projectPath + "deploy/";
            try
            {
                if (IsSelect)
                {
                    if (!_sftpClient.Exists(deploySaveFolder))
                    {
                        _logger($"The first time Can not use select file deploy", NLog.LogLevel.Error);
                        return;
                    }
                }
                CreateServerDirectoryIfItDoesntExist(deploySaveFolder);
                Upload(stream, destinationFolder, destinationfileName);
            }
            catch (Exception)
            {
                if (CheckCancel()) return;

                throw;
            }

            if (!_sftpClient.Exists(destinationfileName))
            {
                _logger($"upload fail, {destinationfileName} not exist!", NLog.LogLevel.Error);
                return;
            }

            if (CheckCancel()) return;
            //创建args文件 antdeploy_args
            var argsFilePath = destinationFolder + "antdeploy_args";
            CreateArgsFile(argsFilePath);

            _logger($"unzip -o -q {destinationFolder + destinationfileName} -d publish/", LogLevel.Info);
            var unzipresult = _sshClient.RunCommand($"cd {destinationFolder} && unzip -o -q {destinationfileName} -d publish/");
            if (unzipresult.ExitStatus != 0)
            {
                _logger($"excute zip command error,return status is not 0", LogLevel.Error);
                _logger($"please check 【zip】 is installed in your server!", LogLevel.Error);
                return;
            }

            if (CheckCancel()) return;

            var publishFolder = $"{destinationFolder}publish/";
          

            if (!_sftpClient.Exists("publish"))
            {
                _logger($"unzip fail: {publishFolder}", NLog.LogLevel.Error);
                return;
            }
            _logger($"unzip success: {publishFolder}", NLog.LogLevel.Info);

            var isExistDockFile = true;
            if (!Increment)
            {
                //如果没有DockerFile 创建默认的
                var dockFilePath = publishFolder + "Dockerfile";
                var dockFilePath2 = "publish/Dockerfile";
                isExistDockFile = _sftpClient.Exists(dockFilePath2);
                if (!isExistDockFile)
                {
                    var createDockerFileResult = CreateDockerFile(dockFilePath);
                    if (!createDockerFileResult) return;
                }

                if (CheckCancel()) return;
            }

            //复制 覆盖 文件到项目下的 deploy目录
            CopyCpFolder(publishFolder, deploySaveFolder);
            if (CheckCancel()) return;
            var temp = publishFolder;
            if (Increment)
            {
                var incrementFoler = $"{destinationFolder}increment/";

                //增量发布 备份全部文件到 increment 文件夹
                _logger($"Increment deploy start backup to [{incrementFoler}]", NLog.LogLevel.Info);

                CopyCpFolder(deploySaveFolder, incrementFoler);

                _logger($"Increment deploy success backup to [{incrementFoler}]", NLog.LogLevel.Info);
                if (CheckCancel()) return;
                temp = incrementFoler;
            }


            //执行Docker命令
            _sftpClient.ChangeDirectory(RootFolder);
            ChangeToFolder(deploySaveFolder);
            DoDockerCommand(deploySaveFolder,false,!isExistDockFile,publishName:"",fromFolder: temp);
            
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

            var isIncrement = false;
            var increment = $"{path}increment/";
            try
            {
                if (_sftpClient.Exists("increment"))
                {
                    isIncrement = true;
                }
            }
            catch (Exception)
            {
                //ignore
            }

            if (isIncrement)
            {
                DoDockerCommand(increment, true, publishName: "increment");
                return;
            }
        
            DoDockerCommand(publishFolder, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishFolder">deploy文件目录</param>
        /// <param name="isrollBack"></param>
        /// <param name="isDefaultDockfile">上传的时候没有DockerFile要创建</param>
        public void DoDockerCommand(string publishFolder, bool isrollBack = false,bool isDefaultDockfile = false,string publishName = "publish",string fromFolder = null)
        {
            string port = string.Empty;
            string server_port = string.Empty;
            if (!publishFolder.EndsWith("/")) publishFolder = publishFolder + "/";
            //先查看本地是否有dockerFile
            var dockFilePath = publishFolder + "Dockerfile";
            var dockFilePath2 = $"{(string.IsNullOrEmpty(publishName)?"": publishName+"/")}Dockerfile";
            // ReSharper disable once SimplifyConditionalTernaryExpression
            var isExistDockFile = isDefaultDockfile? false: _sftpClient.Exists(dockFilePath2);
            //如果本地存在dockerfile 那么就根据此创建image
            //如果不存在的话 就根据当前的netcore sdk的版本 进行创建相对应的 dockfile

            if (!isExistDockFile)
            {
                if (isrollBack)
                {
                    _logger($"dockerFile is not exist: {dockFilePath}", NLog.LogLevel.Error);
                    return;
                }
              

                if (!isDefaultDockfile)
                {
                    var createDockerFileResult = CreateDockerFile(dockFilePath);
                    if (!createDockerFileResult) return;
                }
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
                        if (!string.IsNullOrEmpty(NetCorePort) && !this.ContainerPort.Equals(newPort))
                        {
                            _logger($"EXPOSE in dockerFile is defined,will use【{newPort}】replace【{ContainerPort}】", NLog.LogLevel.Warn);
                        }
                        else
                        {
                            _logger($"EXPOSE in dockerFile is : 【{newPort}】", NLog.LogLevel.Info);
                        }

                    }

                    //如果dockfile里面没有配置EXPOST 就用界面上提供的
                    port = needAddPort? ContainerPort:newPort;

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
                    else
                    {
                        _logger($"Volume in dockerFile is not defined", NLog.LogLevel.Warn);
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
                        if (!string.IsNullOrEmpty(NetCorePort) && !ServerPort.Equals(serverPostDockerFile))
                        {
                            _logger($"ServerPort in dockerFile is defined,will use【{serverPostDockerFile}】replace【{ServerPort}】", NLog.LogLevel.Warn);
                        }
                        else
                        {
                            _logger($"ServerPort in dockerFile is : 【{serverPostDockerFile}】", NLog.LogLevel.Info);
                        }

                        server_port = serverPostDockerFile;
                    }
                    else
                    {
                        server_port = needAddPort ? ServerPort : port;
                    }

                    var addV = false;
                    if (!string.IsNullOrEmpty(NetCoreEnvironment) || needAddPort)
                    {
                        var allLines = _sftpClient.ReadAllLines(dockFilePath).ToList();
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
                      

                        if (entryPointIndex>0)
                        {
                            var add = false;
                            if (needAddPort)
                            {
                                add = true;
                                allLines.Insert(entryPointIndex, "EXPOSE " + port);
                                _logger($"Add EXPOSE " + port + $" to dockerFile  : 【{dockFilePath}】", NLog.LogLevel.Info);
                            }

                            if (!haveEnv && !string.IsNullOrEmpty(NetCoreEnvironment))
                            {
                                add = true;
                                allLines.Insert(entryPointIndex, "ENV ASPNETCORE_ENVIRONMENT " + NetCoreEnvironment);
                                _logger($"Add ENV ASPNETCORE_ENVIRONMENT " + NetCoreEnvironment + $" to dockerFile  : 【{dockFilePath}】", NLog.LogLevel.Info);
                            }

                            if (add)
                            {
                                _sshClient.RunCommand($"set -e;cd ~;\\rm -rf \"{dockFilePath}\";");
                                //没有发现包含环境变量 就添加进去
                                using (var writer = _sftpClient.CreateText(dockFilePath))
                                {
                                    foreach (var line in allLines)
                                    {
                                        writer.WriteLine(line);
                                    }
                                    //发布的时候界面上有填volume 也存在dockerfile 要记录到dockerfile中 不然回滚的时候就没了
                                    if (string.IsNullOrEmpty(volumeInDockerFile) && !string.IsNullOrEmpty(this.Volume))
                                    {
                                        addV = true;
                                        writer.WriteLine(volumeProfix + this.Volume + "@");
                                        _logger(volumeProfix + this.Volume + "@", LogLevel.Info);
                                    }
                                    writer.Flush();
                                }
                            }
                        }
                    }

                    if (!addV && string.IsNullOrEmpty(volumeInDockerFile) && !string.IsNullOrEmpty(this.Volume))
                    {
                        //发布的时候界面上有填volume 也存在dockerfile 要记录到dockerfile中 不然回滚的时候就没了
                        var allLines = _sftpClient.ReadAllLines(dockFilePath).ToList();
                        _sshClient.RunCommand($"set -e;cd ~;\\rm -rf \"{dockFilePath}\";");
                        using (var writer = _sftpClient.CreateText(dockFilePath))
                        {
                            foreach (var line in allLines)
                            {
                                writer.WriteLine(line);
                            }

                            writer.WriteLine(volumeProfix + this.Volume + "@");
                            _logger(volumeProfix + this.Volume + "@", LogLevel.Info);
                            writer.Flush();
                        }
                    }

                    if (!string.IsNullOrEmpty(fromFolder))
                    {
                        //需要将修改过的DockerFile 移动到 发布文件夹的publish 目录下 不然会导致回滚的时候失败
                        var command = $"\\cp -rf {dockFilePath} {fromFolder}";
                        _logger($"Update DockerFile 【{command}】", NLog.LogLevel.Info);
                        _sshClient.RunCommand(command);
                    }

                }
                catch (Exception ex)
                {
                    _logger($"parse param in dockerFile fail: {dockFilePath},err:{ex.Message}", NLog.LogLevel.Error);
                    return;
                }
            }

            //执行docker build 生成一个镜像
            var dockerBuildResult = RunSheell($"docker build --no-cache --rm -t {PorjectName}:{ClientDateTimeFolderName} -f {dockFilePath} {publishFolder} ");
            if (!dockerBuildResult)
            {
                _logger($"build image fail", NLog.LogLevel.Error);
                return;
            }

            if (this.DockerServiceBuildImageOnly)
            {
                goto DockerServiceBuildImageOnlyLEVEL;
            }

            var continarName = "d_" + PorjectName;


            //先发送退出命令
            //https://stackoverflow.com/questions/40742192/how-to-do-gracefully-shutdown-on-dotnet-with-docker

            SshCommand r1 = null;
            try
            {
                r1 = _sshClient.RunCommand($"docker stop -t 10 {continarName}");
                if (r1.ExitStatus == 0)
                {
                    _logger($"docker stop -t 10 {continarName}", LogLevel.Info);
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
                r1 = _sshClient.RunCommand($"docker rm -f {continarName}");
                if (r1.ExitStatus == 0)
                {
                    _logger($"docker rm -f {continarName}", LogLevel.Info);
                }
            }
            catch (Exception)
            {
                //ignore
            }


            if (string.IsNullOrEmpty(port))
            {
                port = ContainerPort;
            }

            if (string.IsNullOrEmpty(server_port))
            {
                server_port = ServerPort;
            }

            string volume = GetVolume();

            // 根据image启动一个容器
            var dockerRunRt = RunSheell($"docker run --name {continarName}{volume} -d --restart=always -p {server_port}:{port} {PorjectName}:{ClientDateTimeFolderName}");

            if (!dockerRunRt)
            {
                _logger($"docker run fail", NLog.LogLevel.Error);
                return;
            }

            //查看是否只打包镜像不允许
            DockerServiceBuildImageOnlyLEVEL:
            if (DockerServiceBuildImageOnly)
            {
                _logger($"ignore docker run", NLog.LogLevel.Warn);
            }
            //把旧的image给删除
            r1 = _sshClient.RunCommand("docker images --format '{{.Repository}}:{{.Tag}}:{{.ID}}' | grep '^" + PorjectName + ":'");
            Tuple<string, string, string> currentImageInfo = null;
            if (r1.ExitStatus == 0 && !string.IsNullOrEmpty(r1.Result))
            {
                var deleteImageArr = r1.Result.Split('\n');
                var clearOldImages = false;
                foreach (var imageName in deleteImageArr)
                {
                    if (imageName.StartsWith($"{PorjectName}:{ClientDateTimeFolderName}:"))
                    {
                        var imageArr2 = imageName.Split(':');
                        if (imageArr2.Length == 3)
                        {
                            currentImageInfo = new Tuple<string, string, string>(imageArr2[0], imageArr2[1], imageArr2[2]);
                        }
                        //当前版本
                        continue;
                    }

                    var imageArr = imageName.Split(':');
                    if (imageArr.Length == 3)
                    {
                        var r2 = _sshClient.RunCommand($"docker rmi {imageArr[2]}");
                        if (r2.ExitStatus == 0)
                        {
                            if (!clearOldImages)
                            {
                                _logger($"start to clear old images of name:{PorjectName}", LogLevel.Info);
                                clearOldImages = true;
                            }
                            _logger($"docker rmi {imageArr[2]} [{imageName}]", LogLevel.Info);
                        }
                    }
                }

            }


            try
            {
                //查看是否有<none>的image 把它删掉 因为我们创建image的时候每次都会覆盖所以会产生一些没有的image
                _sshClient.RunCommand($"if docker images -f \"dangling=true\" | grep ago --quiet; then docker rmi -f $(docker images -f \"dangling=true\" -q); fi");

            }
            catch (Exception )
            {
                //igore
            }

            //镜像上传
            if (!isrollBack && currentImageInfo!=null && this.DockerServiceEnableUpload)
            {
                //第一步 登录
                //第二步 重新tag
                //第三步 推送
                //第四步 删除
                //第五步 退出登录
                //万一已经存在就删除
                var uploadImageName =$"{this.RepositoryUrl}/{this.RepositoryNameSpace}/{this.RepositoryImageName}:{currentImageInfo.Item2}";
                _sshClient.RunCommand($"docker rmi {uploadImageName}");
                var rr11 = _sshClient.CreateCommand($"set -e;docker login -u {this.RepositoryUserName} -p {this.RepositoryUserPwd} {this.RepositoryUrl}; docker tag {currentImageInfo.Item3} {uploadImageName};docker push {uploadImageName}");
                var result = rr11.BeginExecute();
                using (var reader = new StreamReader(rr11.OutputStream, Encoding.UTF8, true, 1024, true))
                {
                    while (!result.IsCompleted || !reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            _logger($"[upload image] - {line}", LogLevel.Warn);
                        }
                    }
                }
                rr11.EndExecute(result);
                if (rr11.ExitStatus != 0)
                {
                    _logger($"[upload image] - Fail", LogLevel.Error);
                }
                else
                {
                    _logger($"[upload image] - Success", LogLevel.Info);
                }
                _sshClient.RunCommand($"docker rmi {uploadImageName}");
            }

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
                var temp = histroy.Replace("_", "");
                if (!DateTime.TryParseExact(temp, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime createDate))
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



        private bool CreateArgsFile(string path)
        {
            try
            {
                using (var writer = _sftpClient.CreateText(path))
                {
                    var obj = new LinuxArgModel
                    {
                        Remark = Remark??string.Empty,
                        Mac = CodingHelper.GetMacAddress(),
                        Ip = CodingHelper.GetLocalIPAddress(),
                        Pc = Environment.MachineName
                    };
                    writer.WriteLine(JsonConvert.SerializeObject(obj));
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
                _logger($"dockerFile not found in: [{path}],will create default Dockerfile!", NLog.LogLevel.Info);
                string dllName = NetCoreENTRYPOINT;

                string sdkVersion = NetCoreVersion;
                if (string.IsNullOrEmpty(sdkVersion))
                {
                    sdkVersion = "2.1";
                }

              

                string environment = NetCoreEnvironment;

                _logger($"create docker file: {path}", NLog.LogLevel.Info);
                using (var writer = _sftpClient.CreateText(path))
                {
                    writer.WriteLine($"FROM mcr.microsoft.com/dotnet/core/aspnet:{sdkVersion}");
                    _logger($"FROM mcr.microsoft.com/dotnet/core/aspnet:{sdkVersion}", NLog.LogLevel.Info);// microsoft/dotnet:{sdkVersion}-aspnetcore-runtime

                    writer.WriteLine($"COPY . /publish");
                    _logger($"COPY . /publish", NLog.LogLevel.Info);

                    writer.WriteLine($"WORKDIR /publish");
                    _logger($"WORKDIR /publish", NLog.LogLevel.Info);

                    writer.WriteLine($"ENV ASPNETCORE_URLS=http://*:{this.ContainerPort}");
                    _logger($"ENV ASPNETCORE_URLS=http://*:{this.ContainerPort}", NLog.LogLevel.Info);

                    if (!string.IsNullOrEmpty(environment))
                    {
                        writer.WriteLine($"ENV ASPNETCORE_ENVIRONMENT={environment}");
                        _logger($"ENV ASPNETCORE_ENVIRONMENT={environment}", NLog.LogLevel.Info);
                    }

                    writer.WriteLine($"EXPOSE {this.ContainerPort}");
                    _logger($"EXPOSE {this.ContainerPort}", NLog.LogLevel.Info);

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

                    if (!string.IsNullOrEmpty(this.NetCorePort) && this.NetCorePort.Contains(":"))
                    {
                        writer.WriteLine(serverPortProfix + this.ServerPort + "@");
                        _logger(serverPortProfix + this.ServerPort + "@", NLog.LogLevel.Info);
                    }

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
                if (cmd.Error.Contains("unable to resolve host"))
                {
                    _logger(cmd.Error, LogLevel.Warn);
                    return true;
                }
                _logger(cmd.Error, LogLevel.Error);
                return false;
            }

            return true;
        }



        public void Dispose()
        {
            try
            {
                _subscribe.Dispose();
            }
            catch (Exception)
            {

            }
          
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


        private void CopyCpFolder(string from ,string to)
        {
            this._logger($"Start Copy Files From [{from}] To [{to}]", LogLevel.Info);
            if (!from.EndsWith("/")) from = from + "/";
            if (!to.EndsWith("/")) from = to + "/";
            var command = $"\\cp -rf {from}. {to}";
            SshCommand cmd = _sshClient.RunCommand(command);
            if (cmd.ExitStatus != 0)
            {
                this._logger($"Copy Files From [{from}] To [{to}] fail", LogLevel.Error);
            }
            else
            {
                this._logger($"Success Copy Files From [{from}] To [{to}]", LogLevel.Info);
            }
        }

        private void DeleteDirectory(string path,bool useCommand = true)
        {
            if (useCommand)
            {
                //目前有2种会用到删除文件夹
                // 1.文件上传发现要上传的目录已存在
                // 2.旧的发布版本文件夹
                SshCommand cmd = _sshClient.RunCommand($"set -e;cd ~;\\rm -rf \"{path}\";");
                if (cmd.ExitStatus != 0)
                {
                    this._logger($"Delete directory:{path} fail", LogLevel.Warn);
                }
                return;
            }

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

    public class UploadEventArgs: EventArgs
    {
        //long size, ulong uploadedSize

        public long size { get; set; }
        public ulong uploadedSize { get; set; }
        
    }

}
