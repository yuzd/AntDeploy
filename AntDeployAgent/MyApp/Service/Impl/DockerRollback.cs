using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AntDeployAgent.Model;

namespace AntDeployAgent.MyApp.Service.Impl
{
    public class DockerRollback : PublishProviderBasicAPI
    {
        private string _serviceName;

        private string _projectPublishFolder;
        private string _dateTimeFolderName;
        public override string ProviderName => "docker";
        public override string ProjectName => _serviceName;
        public override string ProjectPublishFolder => _projectPublishFolder;
        public override string RollBack()
        {
            try
            {
                var projectPath = Path.Combine(Setting.PublishDockerPathFolder, _serviceName);
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
                var deployFolder = findUploadFolder(_projectPublishFolder,docker:true);

                var incrementFolder = Path.Combine(_projectPublishFolder, "increment");
                if (Directory.Exists(incrementFolder))
                {
                    deployFolder = incrementFolder;
                }

                if (!Directory.Exists(deployFolder))
                {
                    return "rollback folder not found:" + deployFolder;
                }

                var projectLocationFolder = Path.Combine(projectPath, "deploy");
                Log("rollback from folder ==>" + deployFolder);


                Log("Start to rollback Docker Service:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + deployFolder);

                Arguments args = new Arguments
                {
                    DeployType = "Docker",
                    BackupFolder = Setting.BackUpDockerPathFolder,
                    AppName = _serviceName,
                    AppFolder = projectLocationFolder,
                    TempPhysicalPath = _dateTimeFolderName,
                    DeployFolder = deployFolder,
                    NoBackup = true,
                    Extention = new DockerParamModel
                    {
                        NetCoreENTRYPOINT = _serviceName,
                        RollBack = true
                    }
                };
                var ops = new OperationsDocker(args, Log);
                try
                {
                    ops.Execute();
                    SaveCurrentVersion(new DirectoryInfo(deployFolder).Parent.FullName);
                    Log("Rollback Docker Service Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        return $"Rollback to Docker Service err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"Rollback to Docker Service err:{ex.Message}, fail:{ex2.Message}";
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
