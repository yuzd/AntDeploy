﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgent.Util;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System.Runtime.InteropServices;
namespace AntDeployAgent.MyApp.Service.Impl
{
    public class WindowServicePublisher : PublishProviderBasicAPI
    {
        private string _sdkTypeName;
        private bool _isProjectInstallService;
        private bool _useNssm;
        private string _param;
        private bool _isNoStopWebSite;
        private string _serviceName;
        private string _serviceExecName;
        private string _serviceDescription;
        private string _serviceStartType;
        private int _waitForServiceStopTimeOut = 15;
        private List<string> _backUpIgnoreList = new List<string>();
        private string _projectPublishFolder;
        private string _dateTimeFolderName;
        private bool _isIncrement; //是否增量
        private string _physicalPath; //指定的创建的时候用的服务器路径
        public override string ProviderName => "windowService";
        public override string ProjectName => _serviceName;
        public override string ProjectPublishFolder => _projectPublishFolder;



        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, _serviceName);
            _projectPublishFolder = Path.Combine(projectPath,
                !string.IsNullOrEmpty(_dateTimeFolderName) ? _dateTimeFolderName : DateTime.Now.ToString("yyyyMMddHHmmss"));
            EnsureProjectFolder(projectPath);
            EnsureProjectFolder(_projectPublishFolder);
            var deployFolder = string.Empty;
            try 
            {
                var _zipFile = Path.Combine(_projectPublishFolder, fileItem.FileName);
                using (var fs = new FileStream(_zipFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileItem.FileBody, 0, fileItem.FileBody.Length);
                }


                if (!File.Exists(_zipFile))
                {
                    return "publish file save fail";
                }
#if NETCORE
                Log("netcore agent version ==>" + Version.VERSION);
#else
                Log("netframework agent version ==>" + Version.VERSION);
#endif
                Log("upload success ==>" + _zipFile);
                //解压
                try
                {
                    Log("start unzip file");
                    ZipFile.ExtractToDirectory(_zipFile, _projectPublishFolder);
                }
                catch (Exception ex)
                {
                    return "unzip publish file error:" + ex.Message;
                }

                Log("unzip success ==>" + _projectPublishFolder);

                deployFolder = findUploadFolder(_projectPublishFolder, true);

                if (!Directory.Exists(deployFolder))
                {
                    return "unzip publish file error,Path not found:" + deployFolder;
                }

                //查找 Windows Service 里面是否存在该服务
                var service = WindowServiceHelper.GetWindowServiceByName(this._serviceName);
                if (!string.IsNullOrEmpty(service.Item2))
                {
                    return service.Item2;
                }

                if (service.Item1 == null)
                {
                    Log($"windowService : {_serviceName} not found,start to create!");

                    //创建发布目录
                    var firstDeployFolder = string.IsNullOrEmpty(_physicalPath) ? Path.Combine(projectPath, "deploy") : _physicalPath;
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
                    CopyHelper.ProcessXcopy(deployFolder, firstDeployFolder, Log);

                    Log($"copy files success from [{deployFolder}] to [{firstDeployFolder}]");

                    //部署windows service
                    var execFullPath = Path.Combine(firstDeployFolder, _serviceExecName);
                    if (!File.Exists(execFullPath))
                    {
                        try
                        {
                            Directory.Delete(firstDeployFolder, true);
                        }
                        catch (Exception)
                        {
                        }

                        return $"windows service exec file not found : {execFullPath} ";
                    }

                    //安装服务
                    Log($"start to install windows service");
                    Log($"service name:{_serviceName}");
                    Log($"service path:{execFullPath}");
                    Log($"service startType:{(!string.IsNullOrEmpty(_serviceStartType) ? _serviceStartType : "Auto")}");
                    Log($"service description:{_serviceDescription ?? string.Empty}");


                    try
                    {
                        if (_useNssm)
                        {
                            var rt = ServiceInstaller.NssmInstallAndStart(_serviceName, _param, execFullPath, _serviceStartType, _serviceDescription, Log);
                            if (!rt)
                            {
                                return "use nssm install windows service fail";
                            }
                        }
                        else
                        {
                            ServiceInstaller.InstallAndStart(_serviceName, _serviceName, execFullPath + (string.IsNullOrEmpty(_param) ? "" : " " + _param),
                                _serviceStartType, _serviceDescription);
                        }

                        Log($"install windows service success");
                        Log($"start windows service success");
                        return string.Empty;
                    }
                    catch (Exception e2)
                    {
                        Thread.Sleep(5000);
                        var isStart = WindowServiceHelper.IsStart(_serviceName);
                        if (isStart)
                        {
                            Log($"install windows service success");
                            Log($"start windows service success");
                            return string.Empty;
                        }

                        return $"install windows service fail:" + e2.Message;
                    }
                }

                var projectLocationFolder = string.Empty;
                var projectLocation = WindowServiceHelper.GetWindowServiceLocation(this._serviceName);
                if (string.IsNullOrEmpty(projectLocation))
                {
                    return $"can not find executable path of service:{_serviceName}";
                }

                //处理使用 nssm 安装的 Windows 服务程序
                if (projectLocation.EndsWith("nssm.exe", true, CultureInfo.CurrentCulture))
                {
                    Log("service is installed by NSSM process.");

                    var _nssmOutput = "";
                    var rt = ProcessHepler.RunExternalExe(projectLocation, $"get {_serviceName} Application",
                        output => { _nssmOutput += Regex.Replace(output, @"\0", ""); });

                    if (!rt || string.IsNullOrEmpty(_nssmOutput.Trim()))
                    {
                        return $"can not find real executable path of nssm service:{_serviceName}";
                    }

                    projectLocation = _nssmOutput;
                }

                Log($"project location:{projectLocation}");

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

                Arguments args = new Arguments
                {
                    DeployType = "WindowsService",
                    BackupFolder = Setting.BackUpWindowServicePathFolder,
                    AppName = _serviceName,
                    AppFolder = projectLocationFolder,
                    DeployFolder = deployFolder,
                    WaitForWindowsServiceStopTimeOut = _waitForServiceStopTimeOut,
                    BackUpIgnoreList = this._backUpIgnoreList,
                    NoBackup = !Setting.NeedBackUp
                };

                if (_serviceName.ToLower().Equals("antdeployagentwindowsservice"))
                {
                    return UpdateSelft(args);
                }


                Log("Start to deploy Windows Service:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + projectLocationFolder);

                if (_isNoStopWebSite)
                {
                    args.NoStop = true;
                    args.NoStart = true;
                }

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
                            CopyHelper.DirectoryCopy(projectLocationFolder, incrementFolder, true, fullName, directoryInfo.Name, this._backUpIgnoreList);
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
            finally
            {
                if (!string.IsNullOrEmpty(deployFolder) && Directory.Exists(deployFolder))
                {
                    new Task(() =>
                    {
                        try
                        {
                            Directory.Delete(deployFolder, true);
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }).Start();
                }
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

            var isUseNssm = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("useNssm"));
            if (isUseNssm != null && !string.IsNullOrEmpty(isUseNssm.TextValue))
            {
                _useNssm = isUseNssm.TextValue.Equals("yes");
            }

            var serviceParam = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("param"));
            if (serviceParam != null && !string.IsNullOrEmpty(serviceParam.TextValue))
            {
                _param = serviceParam.TextValue;
            }

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

            var isNoStopWebSite = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isNoStopWebSite"));
            if (isNoStopWebSite != null && !string.IsNullOrEmpty(isNoStopWebSite.TextValue) && isNoStopWebSite.TextValue.ToLower().Equals("true"))
            {
                _isNoStopWebSite = true;
            }

            var physicalPath = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("physicalPath"));
            if (physicalPath != null && !string.IsNullOrEmpty(physicalPath.TextValue))
            {
                _physicalPath = physicalPath.TextValue;
            }

            var desc = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("desc"));
            if (desc != null && !string.IsNullOrEmpty(desc.TextValue))
            {
                _serviceDescription = desc.TextValue;
            }

            var startType = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("startType"));
            if (startType != null && !string.IsNullOrEmpty(startType.TextValue))
            {
                _serviceStartType = startType.TextValue;
            }

            var backUpIgnoreList = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("backUpIgnore"));
            if (backUpIgnoreList != null && !string.IsNullOrEmpty(backUpIgnoreList.TextValue))
            {
                this._backUpIgnoreList = backUpIgnoreList.TextValue.Split(new string[] { "@_@" }, StringSplitOptions.None).ToList();
            }

            return string.Empty;
        }

        /// <summary>
        /// 更新自己
        /// </summary>
        /// <returns></returns>
        private string UpdateSelft(Arguments args)
        {
            try
            {
                Log("Start to update AntDeploy Agent:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + args.AppFolder);


                var selftCmd = Path.Combine(args.DeployFolder, "_deploy_end_.bat");
                if (!File.Exists(selftCmd))
                {
                    Log($"【Error】File can not found : {selftCmd}");
                    return $"File can not found : {selftCmd}";
                }

                Log("_deploy_end_.bat load success");

                var fileText = File.ReadAllText(selftCmd);
                fileText = fileText.Replace("$DeployFolder$", args.DeployFolder)
                    .Replace("$AppFolder$", args.AppFolder);
                File.WriteAllText(selftCmd, fileText);

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = selftCmd;
                    p.StartInfo.CreateNoWindow = true; //不创建该进程的窗口
                    p.StartInfo.UseShellExecute = false; //不使用shell壳运行
                    p.Start();
                }

                Log("_deploy_end_.bat process start!!!");
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}