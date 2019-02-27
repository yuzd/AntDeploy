using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace AntDeploy.Util
{
    public class SSHClient : IDisposable
    {
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

            //删除发布文件夹
            if (_sftpClient.Exists(destinationFolder + "publish"))
            {
                DeleteDirectory(destinationFolder + "publish");
            }



            var changeTo = destinationFolder;
            if (changeTo.StartsWith("/"))
            {
                changeTo = changeTo.Substring(1);
            }
            _sftpClient.ChangeDirectory(changeTo);
            _logger($"Changed directory to {destinationFolder}", NLog.LogLevel.Info);
            var fileSize = stream.Length;
            _sftpClient.UploadFile(stream, fileName, (uploaded) => { uploadProgress(fileSize, uploaded); });



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


        public async Task PublishZip(Stream stream, string destinationFolder,
            string destinationfileName)
        {
            if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

            destinationFolder = destinationFolder + PorjectName + "/";

            Upload(stream, destinationFolder, destinationfileName);

            if (!_sftpClient.Exists(destinationfileName))
            {
                _logger($"upload fail, {destinationfileName} not exist!", NLog.LogLevel.Error);
                return;
            }

            _logger($"unzip -q {destinationFolder + destinationfileName}", NLog.LogLevel.Info);
            var unzipresult = _sshClient.RunCommand($"cd {destinationFolder} && unzip -q {destinationfileName}");
            if (unzipresult.ExitStatus != 0)
            {
                _logger($"excute unzip error,return status is not 0", NLog.LogLevel.Error);
                _logger($"please check 【unzip】 is installed in your server!", NLog.LogLevel.Error);
                return;
            }
            var publishFolder = $"{destinationFolder}publish/";
            var publishFolder2 = $"publish";
            if (!_sftpClient.Exists(publishFolder2))
            {
                _logger($"unzip fail: {publishFolder}", NLog.LogLevel.Error);
                return;
            }
            _logger($"unzip success: {publishFolder}", NLog.LogLevel.Info);
            //_sftpClient.ChangeDirectory(publishFolder);
            //_logger($"Changed directory to {publishFolder}");


            //执行Docker命令

            //先查看本地是否有dockerFile
            var dockFilePath = publishFolder + "Dockerfile";
            var dockFilePath2 = publishFolder2 + "/" + "Dockerfile";
            var isExistDockFile = _sftpClient.Exists(dockFilePath2);
            //如果本地存在dockerfile 那么就根据此创建image
            //如果不存在的话 就根据当前的netcore sdk的版本 进行创建相对应的 dockfile
            if (!isExistDockFile)
            {
                var createDockerFileResult = CreateDockerFile(dockFilePath);
                if (!createDockerFileResult) return;
            }

            //执行docker build 生成一个镜像
            await RunSheell($"sudo docker build --no-cache --rm -t {PorjectName} -f {dockFilePath} {publishFolder} ");

            var continarName = "d_" + PorjectName;


            //先发送退出命令
            //https://stackoverflow.com/questions/40742192/how-to-do-gracefully-shutdown-on-dotnet-with-docker
            _sshClient.RunCommand($"sudo docker stop -t 10 {continarName}");
            //_logger($"wait for continar stop 5seconds: {continarName}");
            Thread.Sleep(5000);

            //查看容器有没有在runing 如果有就干掉它
            _sshClient.RunCommand($"sudo docker rm -f {continarName}");

            string port = NetCorePort;
            if (string.IsNullOrEmpty(port))
            {
                port = "5000";
            }

            // 根据image启动一个容器
            await RunSheell($"sudo docker run --name {continarName} -d --restart=always -p {port}:{port} {PorjectName}:latest");

            //查看是否有<none>的image 把它删掉 因为我们创建image的时候每次都会覆盖所以会产生一些没有的image

            _sshClient.RunCommand($"if sudo docker images -f \"dangling=true\" | grep ago --quiet; then sudo docker rmi -f $(sudo docker images -f \"dangling=true\" -q); fi");


        }

        public async Task PublishZip(string zipFolder, List<string> ignorList, string destinationFolder, string destinationfileName)
        {
            using (var stream = ZipHelper.DoCreateFromDirectory2(zipFolder, CompressionLevel.Optimal, true, ignorList))
            {

                await PublishZip(stream, destinationFolder, destinationfileName);
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


        public async Task RunSheell(string command)
        {
            SshCommand cmd = _sshClient.CreateCommand(command);
            _logger(command, NLog.LogLevel.Info);
            await cmd.ExecuteAsync(new SShReport(this._logger), CancellationToken.None);
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
    }

    public class SShReport:IProgress<ScriptOutputLine>
    {
        private Action<string, NLog.LogLevel> _logger;
        public SShReport(Action<string, NLog.LogLevel> logger)
        {
            _logger = logger;
        }
        public void Report(ScriptOutputLine value)
        {
            if (value.IsErrorLine)
            {
                _logger(value.Line,LogLevel.Error);
            }
            else
            {
                _logger(value.Line,LogLevel.Info);
            }
        }
    }
   


    public static class SshCommandExtensions
    {
        public static async Task ExecuteAsync(
            this SshCommand sshCommand,
            IProgress<ScriptOutputLine> progress,
            CancellationToken cancellationToken)
        {
            var asyncResult = sshCommand.BeginExecute();
            var stdoutStreamReader = new StreamReader(sshCommand.OutputStream);
            var stderrStreamReader = new StreamReader(sshCommand.ExtendedOutputStream);

            while (!asyncResult.IsCompleted)
            {
                await CheckOutputAndReportProgress(
                    sshCommand,
                    stdoutStreamReader,
                    stderrStreamReader,
                    progress,
                    cancellationToken);
            }

            sshCommand.EndExecute(asyncResult);

            await CheckOutputAndReportProgress(
                sshCommand,
                stdoutStreamReader,
                stderrStreamReader,
                progress,
                cancellationToken);
        }

        private static async Task CheckOutputAndReportProgress(
            SshCommand sshCommand,
            TextReader stdoutStreamReader,
            TextReader stderrStreamReader,
            IProgress<ScriptOutputLine> progress,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                sshCommand.CancelAsync();
            }
            cancellationToken.ThrowIfCancellationRequested();

            await CheckStdoutAndReportProgressAsync(stdoutStreamReader, progress);
            await CheckStderrAndReportProgressAsync(stderrStreamReader, progress);
        }

        private static async Task CheckStdoutAndReportProgressAsync(
            TextReader stdoutStreamReader,
            IProgress<ScriptOutputLine> stdoutProgress)
        {
            var stdoutLine = await stdoutStreamReader.ReadToEndAsync();

            if (!string.IsNullOrEmpty(stdoutLine))
            {
                stdoutProgress.Report(new ScriptOutputLine(
                    line: stdoutLine,
                    isErrorLine: false));
            }
        }

        private static async Task CheckStderrAndReportProgressAsync(
            TextReader stderrStreamReader,
            IProgress<ScriptOutputLine> stderrProgress)
        {
            var stderrLine = await stderrStreamReader.ReadToEndAsync();

            if (!string.IsNullOrEmpty(stderrLine))
            {
                stderrProgress.Report(new ScriptOutputLine(
                    line: stderrLine,
                    isErrorLine: true));
            }
        }
    }

    public class ScriptOutputLine
    {
        public ScriptOutputLine(string line, bool isErrorLine)
        {
            Line = line;
            IsErrorLine = isErrorLine;
        }

        public string Line { get; private set; }

        public bool IsErrorLine { get; private set; }
    }
}
