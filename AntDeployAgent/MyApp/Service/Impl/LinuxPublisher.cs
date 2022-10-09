using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System.Runtime.InteropServices;
namespace AntDeployAgent.MyApp.Service.Impl
{
    public class LinuxPublisher : PublishProviderBasicAPI
    {
        private bool _isNoStopWebSite; //是否不重新启动服务
        private string _serviceName; //服务名称
        private string _serviceExecName; //服务可执行程序名称
        private string _serviceDescription; //服务描述
        private string _serviceStartType; //是否设置重启机器自动重启
        private string _env; //环境变量？
        private bool _notify; //是否设置需要通知systemd
        private List<string> _backUpIgnoreList = new List<string>(); //需要排除backup的列表
        private string _projectPublishFolder; //发布目录
        private string _dateTimeFolderName; //版本
        private bool _isIncrement; //是否增量
        private string _physicalPath; //指定的创建的时候用的服务器路径
        public override string ProviderName => "linux";
        public override string ProjectName => _serviceName;
        public override string ProjectPublishFolder => _projectPublishFolder;

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishLinuxPathFolder, _serviceName);
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
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Log("linux agent version ==>" + Version.VERSION);
                }
                else
                {
                    Log("netcore agent version ==>" + Version.VERSION);
                }

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

                //查找  Service 里面是否存在该服务
                var service = LinuxServiceHelper.GetLinuxService(this._serviceName);
                if (!string.IsNullOrEmpty(service.Item1))
                {
                    //运行命令出错了
                    return service.Item1;
                }

                if (string.IsNullOrEmpty(service.Item2)) //没有找到该服务的workingFolder 可能是service描述文件内容不对，可能是服务不存在
                {
                    Log($"systemctlService : {_serviceName} not found,start to create!");

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

                    //linux service
                    var execFullPath = Path.Combine(firstDeployFolder, _serviceExecName);
                    if (!File.Exists(execFullPath))
                    {
                        //try { Directory.Delete(firstDeployFolder, true); } catch (Exception) { }
                        return $"systemctl service exec file not found : {execFullPath}";
                    }

                    //安装服务
                    Log($"start to install systemctl service");
                    Log($"service name:{_serviceName}");
                    Log($"service path:{execFullPath}");
                    Log($"service description:{_serviceDescription ?? string.Empty}");

                    try
                    {
                        var err = LinuxServiceHelper.CreateServiceFileAndRun(this._serviceName, firstDeployFolder, _serviceExecName,
                            (_serviceDescription ?? string.Empty), _env, execFullPath,
                            string.IsNullOrEmpty(_serviceStartType) || _serviceStartType.Equals("Auto"), _notify, Log);
                        if (!string.IsNullOrEmpty(err))
                        {
                            return err;
                        }

                        //检查是否成功了？
                        var runSuccess = false;

                        Thread.Sleep(5000);

                        CopyHelper.RunCommand($"sudo systemctl status {this._serviceName}", null, (msg) =>
                        {
                            if (!string.IsNullOrEmpty(msg))
                            {
                                this.Log(msg);
                                var msg1 = msg.ToLower();
                                if (msg1.Contains("activating (start)"))
                                {
                                    runSuccess = true;
                                }
                                else if (msg1.Contains("active:") && msg1.Contains("running"))
                                {
                                    runSuccess = true;
                                }
                            }
                        });

                        if (!runSuccess)
                        {
                            return $"install linux service fail";
                        }

                        Log($"install systemctl service success");
                        return string.Empty;
                    }
                    catch (Exception e2)
                    {
                        return $"install linux service fail:" + e2.Message;
                    }
                }

                var projectLocationFolder = service.Item2;

                try
                {
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

                if (string.IsNullOrEmpty(projectLocationFolder))
                {
                    return "ServiceFolder is not correct ===> " + projectLocationFolder;
                }

                var fullExcutePath = Path.Combine(projectLocationFolder, _serviceExecName);
                if (!File.Exists(fullExcutePath))
                {
                    return $"systemctl service exec file not found : {fullExcutePath}";
                }

                //保证有service描述文件 等后面实际要用到
                LinuxServiceHelper.CreateServiceFile(this._serviceName, projectLocationFolder, _serviceExecName,
                    (_serviceDescription ?? string.Empty), _env, this._notify, Log);

                Arguments args = new Arguments
                {
                    DeployType = "Linux",
                    BackupFolder = Setting.BackUpLinuxPathFolder,
                    AppName = _serviceName,
                    AppFolder = projectLocationFolder,
                    TempPhysicalPath = Path.Combine(projectLocationFolder, $"{_serviceName}.service"), //服务文件描述
                    DeployFolder = deployFolder,
                    ApplicationPoolName = fullExcutePath,
                    BackUpIgnoreList = this._backUpIgnoreList,
                    UseOfflineHtm = string.IsNullOrEmpty(_serviceStartType) || _serviceStartType.Equals("Auto"),
                    NoBackup = !Setting.NeedBackUp,
                    Site1 = _env
                };

                Log("Start to deploy linux Service:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + projectLocationFolder);

                if (_isNoStopWebSite)
                {
                    args.NoStop = true;
                    args.NoStart = true;
                }

                var ops = new OperationsLinux(args, Log);

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

                            if (this._backUpIgnoreList != null && this._backUpIgnoreList.Any())
                            {
                                var excludFrom = Path.Combine(_projectPublishFolder, "exclude.log");
                                File.WriteAllLines(excludFrom, this._backUpIgnoreList);
                                //存到文件里面去 要指定排除
                                CopyHelper.ProcessXcopy(projectLocationFolder, incrementFolder, excludFrom, Log);
                            }
                            else
                            {
                                CopyHelper.ProcessXcopy(projectLocationFolder, incrementFolder, Log);
                            }

                            Log("Increment deploy backup success...");
                        }
                    }
                    catch (Exception ex3)
                    {
                        Log("Increment deploy folder backup fail:" + ex3.Message);
                    }

                    Log("Deploy linux service Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        //ops.Rollback();

                        return $"publish to linux service err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"publish to linux service err:{ex.Message},rollback fail:{ex2.Message}";
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
            //linux服务的名称
            var serviceNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("serviceName"));
            if (serviceNameItem == null || string.IsNullOrEmpty(serviceNameItem.TextValue))
            {
                return "serviceName required";
            }

            _serviceName = serviceNameItem.TextValue.Trim();


            //要运行的可执行程序的名称
            var serviceExecItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("execFilePath"));
            if (serviceExecItem == null || string.IsNullOrEmpty(serviceExecItem.TextValue))
            {
                return "execFilePath required";
            }

            _serviceExecName = serviceExecItem.TextValue.Trim();


            //发布版本
            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }

            //是否是增量发布
            var isIncrement = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isIncrement"));
            if (isIncrement != null && !string.IsNullOrEmpty(isIncrement.TextValue) && isIncrement.TextValue.ToLower().Equals("true"))
            {
                _isIncrement = true;
            }

            //是否不重新启动
            var isNoStopWebSite = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isNoStopWebSite"));
            if (isNoStopWebSite != null && !string.IsNullOrEmpty(isNoStopWebSite.TextValue) && isNoStopWebSite.TextValue.ToLower().Equals("true"))
            {
                _isNoStopWebSite = true;
            }

            //指定的物理路径
            var physicalPath = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("physicalPath"));
            if (physicalPath != null && !string.IsNullOrEmpty(physicalPath.TextValue))
            {
                _physicalPath = physicalPath.TextValue;
            }

            //linux服务的描述
            var desc = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("desc"));
            if (desc != null && !string.IsNullOrEmpty(desc.TextValue))
            {
                _serviceDescription = desc.TextValue;
            }

            //设置环境变量？
            var startType = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("startType"));
            if (startType != null && !string.IsNullOrEmpty(startType.TextValue))
            {
                _serviceStartType = startType.TextValue;
            }

            var envType = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("env"));
            if (envType != null && !string.IsNullOrEmpty(envType.TextValue))
            {
                _env = envType.TextValue;
            }

            var notify = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("notify"));
            if (notify != null && !string.IsNullOrEmpty(notify.TextValue) && notify.TextValue.ToLower() == "true")
            {
                _notify = true;
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