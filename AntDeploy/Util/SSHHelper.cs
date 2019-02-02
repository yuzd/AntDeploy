using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        private readonly Action<string> _logger;

        private readonly SftpClient _sftpClient;
        private readonly SshClient _sshClient;
        private long _lastProgressNumber;
        private object lockObject = new object();
        public SSHClient(string host, string userName, string pwd, Action<string> logger)
        {
            this.Host = host;
            this.UserName = userName;
            this.Pwd = pwd;
            _logger = logger;
            _sftpClient = new SftpClient(host.Split(':')[0], int.Parse(host.Split(':')[1]), userName, pwd);
            _sshClient = new SshClient(host.Split(':')[0], int.Parse(host.Split(':')[1]), userName, pwd);
            _sftpClient.BufferSize = 4 * 1024; // bypass Payload error large files
        }


        public bool Connect()
        {
            try
            {
                _logger("ssh Connecting " + Host + "... ");
                _sshClient.Connect();
                _sftpClient.Connect();
                if (_sshClient.IsConnected && _sftpClient.IsConnected)
                {
                    _logger($"ssh connect success:{Host}");
                    return true;

                }

                _logger($"ssh connect fail");
                return false;
            }
            catch (Exception ex)
            {
                _logger($"ssh connect fail:{ex.Message}");
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
            _logger($"Changed directory to {destinationFolder}");
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
                _logger($"uploaded {lastProgressNumber} %");

            }

        }


        public void PublishZip(Stream stream, string destinationFolder,
            string destinationfileName)
        {
            if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

            destinationFolder = destinationFolder + PorjectName + "/";

            Upload(stream, destinationFolder, destinationfileName);

            if (!_sftpClient.Exists(destinationfileName))
            {
                _logger($"upload fail, {destinationfileName} not exist!");
                return;
            }

            _logger($"unzip -q {destinationFolder + destinationfileName}");
            var unzipresult = _sshClient.RunCommand($"cd {destinationFolder} && unzip -q {destinationfileName}");
            if (unzipresult.ExitStatus != 0)
            {
                _logger($"excute unzip error,return status is not 0");
                return;
            }
            var publishFolder = $"{destinationFolder}publish/";
            var publishFolder2 = $"publish";
            if (!_sftpClient.Exists(publishFolder2))
            {
                _logger($"unzip fail: {publishFolder}");
                return;
            }
            _logger($"unzip success: {publishFolder}");
            //_sftpClient.ChangeDirectory(publishFolder);
            //_logger($"Changed directory to {publishFolder}");


            //执行Docker命令

            //先查看本地是否有dockerFile
            var dockFilePath = publishFolder2 + "/" + "Dockerfile";
            var isExistDockFile = _sftpClient.Exists(dockFilePath);
            //如果本地存在dockerfile 那么就根据此创建image
            //如果不存在的话 就根据当前的netcore sdk的版本 进行创建相对应的 dockfile
            if (!isExistDockFile)
            {
                var createDockerFileResult = CreateDockerFile(dockFilePath);
                if (!createDockerFileResult) return;
            }

            //执行docker build 生成一个镜像
            _logger($"sudo docker build --rm -t {PorjectName} -f {dockFilePath} {publishFolder} ");
            RunSheell($"sudo docker build --rm -t {PorjectName} -f {dockFilePath} {publishFolder} ");

            var continarName = "d_" + PorjectName;

            //查看容器有没有在runing 如果有就干掉它
            _logger($"sudo docker ps -q --filter \"name={continarName}\" | grep -q . && sudo docker rm -f {continarName} || true");
            var result = _sshClient.RunCommand($"sudo docker ps -q --filter \"name={continarName}\" | grep -q . && sudo docker rm -f {continarName} || true");
            if (result.ExitStatus != 0)
            {
                _logger($"excute command error,return status is not 0");
                return;
            }

            string port = NetCorePort;
            if (string.IsNullOrEmpty(port))
            {
                port = "5000";
            }

            // 根据image启动一个容器
            RunSheell($"sudo docker run --name {continarName} -d -p {port}:{port} {PorjectName}:latest");

            //查看是否有<none>的image 把它删掉 因为我们创建image的时候每次都会覆盖所以会产生一些没有的image

            _logger($"if sudo docker images -f \"dangling=true\" | grep ago --quiet; then sudo docker rmi -f $(sudo docker images -f \"dangling=true\" -q); fi");
            _sshClient.RunCommand($"if sudo docker images -f \"dangling=true\" | grep ago --quiet; then sudo docker rmi -f $(sudo docker images -f \"dangling=true\" -q); fi");
        }

        public void PublishZip(string zipFolder, List<string> ignorList, string destinationFolder, string destinationfileName)
        {
            using (var stream = ZipHelper.DoCreateFromDirectory2(zipFolder, CompressionLevel.Optimal, true, ignorList))
            {

                PublishZip(stream, destinationFolder, destinationfileName);

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

                _logger($"create docker file: {path}");
                using (var writer = _sftpClient.CreateText(path))
                {
                    writer.WriteLine($"FROM microsoft/dotnet:{sdkVersion}-aspnetcore-runtime");
                    _logger($"FROM microsoft/dotnet:{sdkVersion}-aspnetcore-runtime");

                    writer.WriteLine($"COPY . /publish");
                    _logger($"COPY . /publish");

                    writer.WriteLine($"WORKDIR /publish");
                    _logger($"WORKDIR /publish");

                    writer.WriteLine($"ENV ASPNETCORE_URLS=http://+:{port}");
                    _logger($"ENV ASPNETCORE_URLS=http://+:{port}");

                    if (!string.IsNullOrEmpty(environment))
                    {
                        writer.WriteLine($"ENV ASPNETCORE_ENVIRONMENT={environment}");
                        _logger($"ENV ASPNETCORE_ENVIRONMENT={environment}");
                    }

                    writer.WriteLine($"EXPOSE {port}");
                    _logger($"EXPOSE {port}");


                    writer.WriteLine($"ENTRYPOINT [\"dotnet\", \"{dllName}\"]");
                    _logger($"ENTRYPOINT [\"dotnet\", \"{dllName}\"]");

                    writer.Flush();
                }
                _logger($"create docker file success: {path}");
                return true;
            }
            catch (Exception ex)
            {
                _logger($"create docker file fail: {path},err:{ex.Message}");
                return false;
            }
        }


        public void RunSheell(string command)
        {
            SshCommand cmd = _sshClient.CreateCommand(command);
            var result = cmd.BeginExecute();
            _logger(command);
            using (var reader = new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
            {
                while (!result.IsCompleted || !reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        _logger(line);
                    }
                }
            }
            cmd.EndExecute(result);
        }

        public void DeleteFile(string path)
        {
            try
            {
                _logger($"delete file: {path}");
                _sftpClient.Delete(path);
                _logger($"delete file success: {path}");
            }
            catch (Exception ex)
            {
                _logger($"delete file fail: {path},err:{ex.Message}");
            }
        }


        public void DeleteFolder(string folder)
        {
            try
            {
                _logger($"delete folder: {folder}");
                DeleteDirectory(folder);
                _logger($"delete folder success: {folder}");
            }
            catch (Exception ex)
            {
                _logger($"delete folder fail: {folder},err:{ex.Message}");
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
}
