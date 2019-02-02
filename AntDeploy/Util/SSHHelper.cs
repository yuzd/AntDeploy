using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AntDeploy.Util
{
    public class SSHClient : IDisposable
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }

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
                if (_sshClient.IsConnected &&  _sftpClient.IsConnected)
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




            _sftpClient.ChangeDirectory(destinationFolder);
            _logger($"Changed directory to {destinationFolder}");
            var fileSize = stream.Length;
            _sftpClient.UploadFile(stream, fileName, (uploaded) => { uploadProgress(fileSize, uploaded); });



        }

        private void uploadProgress(long size,ulong uploadedSize )
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




        public void PublishZip(string zipFolder,List<string> ignorList, string destinationFolder, string fileName)
        {
            using (var stream = ZipHelper.DoCreateFromDirectory2(zipFolder,CompressionLevel.Optimal,true, ignorList))
            {

                if (!destinationFolder.EndsWith("/")) destinationFolder = destinationFolder + "/";

                Upload(stream, destinationFolder, fileName);

                var zipPath = destinationFolder + fileName;
                if (!_sftpClient.Exists(zipPath))
                {
                    _logger($"upload fail, {zipPath} not exist!");
                    return;
                }

                _logger($"unzip start: {zipPath}");
                RunSheell($"cd {destinationFolder} && unzip publish.zip");
                var publishFolder = $"{destinationFolder}publish/";
                _logger($"unzip success: {publishFolder}");
                //_sftpClient.ChangeDirectory(publishFolder);
                //_logger($"Changed directory to {publishFolder}");


                //执行Docker命令

                //先查看本地是否有dockerFile

                

                //

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
        private  void DeleteDirectory(string path)
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
    }
}
