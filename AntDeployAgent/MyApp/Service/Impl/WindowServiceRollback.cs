using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System;
using System.IO;
using System.Linq;

namespace AntDeployAgentWindows.MyApp.Service.Impl
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
                Log("netcore agent version ==>" + AntDeployAgentWindows.Version.VERSION);
#else
                Log("netframework agent version ==>" + AntDeployAgentWindows.Version.VERSION);
#endif
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
