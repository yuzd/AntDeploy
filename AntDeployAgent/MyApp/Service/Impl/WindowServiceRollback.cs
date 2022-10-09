﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class WindowServiceRollback : PublishProviderBasicAPI
    {
        private string _serviceName;
        private int _waitForServiceStopTimeOut = 15;

        private string _projectPublishFolder;
        private string _dateTimeFolderName;

        public override string ProviderName => "windowService";
        public override string ProjectName => _serviceName;
        public override string ProjectPublishFolder => _projectPublishFolder;
        public override string RollBack()
        {
            try
            {
                var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, _serviceName);
                _projectPublishFolder = Path.Combine(projectPath, _dateTimeFolderName);
                if (!Directory.Exists(_projectPublishFolder))
                {
                    return "rollback folder not found:" + _projectPublishFolder;
                }

#if NETCORE
                Log("netcore agent version ==>" + Version.VERSION);
#else
                Log("netframework agent version ==>" + Version.VERSION);
#endif
                var deployFolder = findUploadFolder(_projectPublishFolder);

                var incrementFolder = Path.Combine(_projectPublishFolder, "increment");
                if (Directory.Exists(incrementFolder))
                {
                    deployFolder = incrementFolder;
                }

                if (!Directory.Exists(deployFolder))
                {
                    return "rollback folder not found:" + deployFolder;
                }

                Log("rollback from folder ==>" + deployFolder);


                var service = WindowServiceHelper.GetWindowServiceByName(this._serviceName);
                if (!string.IsNullOrEmpty(service.Item2))
                {
                     return service.Item2;
                }
                if (service.Item1 == null)
                {
                    return "service not found:" + _serviceName;
                }

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
                    ProcessHepler.RunExternalExe(projectLocation, $"get {_serviceName} Application", output =>
                    {
                        _nssmOutput += Regex.Replace(output, @"\0", "");
                    });

                    if (string.IsNullOrEmpty(_nssmOutput.Trim()))
                    {
                        return $"can not find real executable path of nssm service:{_serviceName}";
                    }
                    projectLocation = _nssmOutput;
                }


                var projectLocationFolder = string.Empty;
                try
                {
                    projectLocation = projectLocation.Replace("\"", "");
                    projectLocationFolder = new FileInfo(projectLocation).DirectoryName;
                    if (!Directory.Exists(projectLocationFolder))
                    {
                        //如果目录不存在 那么就重新建立
                        return $"can not find executable path of service:{_serviceName}";
                    }
                }
                catch (Exception)
                {
                    return "ServiceFolder is not correct ===> " + projectLocationFolder;
                }

                Log("Start to rollback Windows Service:");
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
                    NoBackup = true,
                };
                var ops = new OperationsWINDOWSSERVICE(args, Log);
                try
                {
                    ops.Execute();
                    SaveCurrentVersion(new DirectoryInfo(deployFolder).Parent.FullName);
                    Log("Rollback WindowsService Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        return $"Rollback to WindowsService err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"Rollback to WindowsService err:{ex.Message}, fail:{ex2.Message}";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex1)
            {
                return ex1.Message;
            }
            finally
            {
                cleanRollbackTemp();
            }
        }


        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            return null;
        }


        public override string CheckData(FormHandler formHandler)
        {

            var serviceNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("serviceName"));
            if (serviceNameItem == null || string.IsNullOrEmpty(serviceNameItem.TextValue))
            {
                return "serviceName required";
            }

            _serviceName = serviceNameItem.TextValue.Trim();

            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }
            else
            {
                return "rollback version is required";
            }

            return string.Empty;
        }
    }
}
