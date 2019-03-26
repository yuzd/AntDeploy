using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace AntDeployAgentWindows.MyApp.Service.Impl
{
    public class WindowServicePublisher : PublishProviderBasicAPI
    {
        private string _sdkTypeName;
        private bool _isProjectInstallService;
        private string _serviceName;
        private string _serviceExecName;
        private int _waitForServiceStopTimeOut = 15;
        private List<string> _backUpIgnoreList = new List<string>();
        private string _projectPublishFolder;
        private string _dateTimeFolderName;
        private bool _isIncrement;//是否增量
        private string _physicalPath;//指定的创建的时候用的服务器路径
        public override string ProviderName => "windowService";
        public override string ProjectName => _serviceName;

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, _serviceName);
            _projectPublishFolder = Path.Combine(projectPath, !string.IsNullOrEmpty(_dateTimeFolderName) ? _dateTimeFolderName : DateTime.Now.ToString("yyyyMMddHHmmss"));
            EnsureProjectFolder(projectPath);
            EnsureProjectFolder(_projectPublishFolder);

            try
            {

                var filePath = Path.Combine(_projectPublishFolder, fileItem.FileName);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileItem.FileBody, 0, fileItem.FileBody.Length);
                }


                if (!File.Exists(filePath))
                {
                    return "publish file save fail";
                }
                Log("agent version ==>" + AntDeployAgentWindows.Version.VERSION);
                Log("upload success ==>" + filePath);
                //解压
                try
                {
                    Log("start unzip file");
                    ZipFile.ExtractToDirectory(filePath, _projectPublishFolder);
                }
                catch (Exception ex)
                {
                    return "unzip publish file error:" + ex.Message;
                }

                Log("unzip success ==>" + _projectPublishFolder);

                var deployFolder = Path.Combine(_projectPublishFolder, "publish");

                if (!Directory.Exists(deployFolder))
                {

                    if (Directory.Exists(_projectPublishFolder))
                    {
                        var temp = new DirectoryInfo(_projectPublishFolder);
                        var tempFolderList = temp.GetDirectories();
                        if (tempFolderList.Length == 1)
                        {
                            deployFolder = tempFolderList.First().FullName;
                        }
                    }
                }

                if (!Directory.Exists(deployFolder))
                {
                    return "unzip publish file error,Path not found:" + deployFolder;
                }

                //查找 Windows Service 里面是否存在该服务
                var service = WindowServiceHelper.GetWindowServiceByName(this._serviceName);
                if (service == null)
                {
                    //if (!_isProjectInstallService)
                    //{
                    //    return $"windowService : {_serviceName} not found";
                    //}

                    Log($"windowService : {_serviceName} not found,start to create!");

                    //创建发布目录
                    var firstDeployFolder = string.IsNullOrEmpty(_physicalPath)? Path.Combine(projectPath, "deploy"):_physicalPath;
                    EnsureProjectFolder(firstDeployFolder);
                    if (Directory.Exists(firstDeployFolder))
                    {
                        Log($"deploy folder create success : {firstDeployFolder} ");
                    }
                    else
                    {
                        return $"DeployFolder : {firstDeployFolder} create error!";
                    }
                  

                    //复制文件到发布目录
                    CopyHelper.DirectoryCopy(deployFolder, firstDeployFolder, true);

                    Log($"copy files success from [{deployFolder}] to [{firstDeployFolder}]");

                    //部署windows service
                    var execFullPath = Path.Combine(firstDeployFolder, _serviceExecName);
                    if (!File.Exists(execFullPath))
                    {
                        try { Directory.Delete(firstDeployFolder, true); } catch (Exception) { }
                        return $"windows service exec file not found : {execFullPath} ";
                    }

                    //安装服务
                    Log($"start to install windows service");
                    Log($"service name:{_serviceName}");
                    Log($"service path:{execFullPath}");
                    try
                    {
                        ServiceInstaller.InstallAndStart(_serviceName, _serviceName, execFullPath);
                        Log($"install windows service success");
                        Log($"start windows service success");
                        return string.Empty;
                    }
                    catch (Exception e2)
                    {
                        return $"install windows service fail:" + e2.Message;
                    }

                    //var installResult = WindowServiceHelper.InstallWindowsService(execFullPath);
                    //if (!string.IsNullOrEmpty(installResult))
                    //{
                    //    try{ Directory.Delete(firstDeployFolder, true);}catch (Exception) {}
                    //    return installResult;
                    //}


                    ////部署成功 启动服务
                    //Log($"start windows service : " + _serviceName);
                    //var startResult = WindowServiceHelper.StartService(_serviceName,120);
                    //if (!string.IsNullOrEmpty(startResult))
                    //{
                    //    try{ Directory.Delete(firstDeployFolder, true);}catch (Exception) {}
                    //    return startResult;
                    //}
                    //Log($"start windows service success");
                    //return string.Empty;
                }

                var projectLocationFolder = string.Empty;
                var projectLocation = WindowServiceHelper.GetWindowServiceLocation(this._serviceName);
                if (string.IsNullOrEmpty(projectLocation))
                {
                    return $"can not find executable path of service:{_serviceName}";
                }

                try
                {
                    projectLocation = projectLocation.Replace("\"", "");
                    projectLocationFolder = new FileInfo(projectLocation).DirectoryName;
                    if (!Directory.Exists(projectLocationFolder))
                    {
                        //如果目录不存在 那么就重新建立
                        EnsureProjectFolder(projectLocationFolder);
                    }
                }
                catch (Exception)
                {
                    return "ServiceFolder is not correct ===> " + projectLocationFolder;
                }

                Log("Start to deploy Windows Service:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + projectLocationFolder);

                Arguments args = new Arguments
                {
                    DeployType = "WindowsService",
                    BackupFolder = Setting.BackUpWindowServicePathFolder,
                    AppName = _serviceName,
                    AppFolder = projectLocationFolder,
                    DeployFolder = deployFolder,
                    WaitForWindowsServiceStopTimeOut = _waitForServiceStopTimeOut,
                    BackUpIgnoreList = this._backUpIgnoreList
                };

                var ops = new OperationsWINDOWSSERVICE(args, Log);

                try
                {
                    ops.Execute();

                    try
                    {
                        //如果是增量的话 要把复制过来
                        if (_isIncrement)
                        {
                            Log("Increment deploy start to backup...");
                            //projectLocation.Item1 转到 increment 的目录
                            var incrementFolder = Path.Combine(_projectPublishFolder, "increment");
                            EnsureProjectFolder(incrementFolder);
                            DirectoryInfo directoryInfo = new DirectoryInfo(projectLocationFolder);
                            string fullName = directoryInfo.FullName;
                            if (directoryInfo.Parent != null)
                                fullName = directoryInfo.Parent.FullName;
                            CopyHelper.DirectoryCopy(projectLocationFolder, incrementFolder, true,fullName,directoryInfo.Name,this._backUpIgnoreList);
                            Log("Increment deploy backup success...");
                        }
                    }
                    catch (Exception ex3)
                    {
                        Log("Increment deploy folder backup fail:" + ex3.Message);
                    }

                    Log("Deploy WindowsService Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        //ops.Rollback();

                        return $"publish to WindowsService err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"publish to WindowsService err:{ex.Message},rollback fail:{ex2.Message}";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }







        public override string CheckData(FormHandler formHandler)
        {

            var sdkType = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("sdkType"));
            if (sdkType == null || string.IsNullOrEmpty(sdkType.TextValue))
            {
                return "sdkType required";
            }

            var sdkTypeValue = sdkType.TextValue.ToLower();

            if (!new string[] { "netframework", "netcore" }.Contains(sdkTypeValue))
            {
                return $"sdkType value :{sdkTypeValue} is not suppored";
            }

            _sdkTypeName = sdkTypeValue;


            var serviceNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("serviceName"));
            if (serviceNameItem == null || string.IsNullOrEmpty(serviceNameItem.TextValue))
            {
                return "serviceName required";
            }

            _serviceName = serviceNameItem.TextValue.Trim();


            var serviceExecItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("execFilePath"));
            if (serviceExecItem == null || string.IsNullOrEmpty(serviceExecItem.TextValue))
            {
                return "execFilePath required";
            }

            _serviceExecName = serviceExecItem.TextValue.Trim();



            var isProjectInstallServiceItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isProjectInstallService"));
            if (isProjectInstallServiceItem != null && !string.IsNullOrEmpty(isProjectInstallServiceItem.TextValue))
            {
                _isProjectInstallService = isProjectInstallServiceItem.TextValue.Equals("yes");
            }

            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }

            var isIncrement = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isIncrement"));
            if (isIncrement != null && !string.IsNullOrEmpty(isIncrement.TextValue) && isIncrement.TextValue.ToLower().Equals("true"))
            {
                _isIncrement = true;
            }

            var physicalPath = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("physicalPath"));
            if (physicalPath != null && !string.IsNullOrEmpty(physicalPath.TextValue))
            {
                _physicalPath = physicalPath.TextValue;
            }

            var backUpIgnoreList = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("backUpIgnore"));
            if (backUpIgnoreList != null && !string.IsNullOrEmpty(backUpIgnoreList.TextValue))
            {
                this._backUpIgnoreList = backUpIgnoreList.TextValue.Split(new string[] { "@_@" }, StringSplitOptions.None).ToList();
            }
            return string.Empty;
        }
    }
}
