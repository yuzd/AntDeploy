using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;

namespace AntDeployAgent.MyApp.Service.Impl
{
    public class LinuxRollback : PublishProviderBasicAPI
    {
        private string _serviceName;

        private string _projectPublishFolder;
        private string _dateTimeFolderName;

        public override string ProviderName => "linux";
        public override string ProjectName => _serviceName;
        public override string ProjectPublishFolder => _projectPublishFolder;
        public override string RollBack()
        {
            try
            {
                var projectPath = Path.Combine(Setting.PublishLinuxPathFolder, _serviceName);
                _projectPublishFolder = Path.Combine(projectPath, _dateTimeFolderName);
                if (!Directory.Exists(_projectPublishFolder))
                {
                    return "rollback folder not found:" + _projectPublishFolder;
                }

#if NETCORE
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
            Log("linux agent version ==>" + Version.VERSION);
        }else{
            Log("netcore agent version ==>" + Version.VERSION);
        }
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

                var service = LinuxServiceHelper.GetLinuxService(this._serviceName);
                if (!string.IsNullOrEmpty(service.Item1))
                {
                    //运行命令出错了
                    return service.Item1;
                }

                var projectLocationFolder = service.Item2;
                if (string.IsNullOrEmpty(projectLocationFolder))
                {
                    return $"can not find executable folder of service:{_serviceName}";
                }

                if (!Directory.Exists(projectLocationFolder))
                {
                    //如果目录不存在 那么就重新建立
                    return $"can not find executable folder of service:{_serviceName}";
                }

                if (string.IsNullOrEmpty(service.Item3))
                {
                    return $"can not find executable path of service:{_serviceName}";
                }
                if (!File.Exists(service.Item3))
                {
                    return $"can not find executable path of service:{_serviceName}";
                }

                Log("Start to rollback Linux Service:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + projectLocationFolder);

                Arguments args = new Arguments
                {
                    DeployType = "Linux",
                    BackupFolder = Setting.BackUpLinuxPathFolder,
                    AppName = _serviceName,
                    AppFolder = projectLocationFolder,
                    DeployFolder = deployFolder,
                    ApplicationPoolName = service.Item3,//执行文件
                    NoBackup = true,
                };
                var ops = new OperationsLinux(args, Log);
                try
                {
                    ops.Execute();
                    SaveCurrentVersion(new DirectoryInfo(deployFolder).Parent.FullName);
                    Log("Rollback Linux Service Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        return $"Rollback to Linux Service err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"Rollback to Linux Service err:{ex.Message}, fail:{ex2.Message}";
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
