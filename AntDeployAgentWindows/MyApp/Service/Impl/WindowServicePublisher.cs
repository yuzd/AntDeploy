using AntDeployAgentWindows.WebApiCore;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;

namespace AntDeployAgentWindows.MyApp.Service.Impl
{
    public class WindowServicePublisher : PublishProviderBasicAPI
    {
      
        private string _serviceName;
        private int _waitForServiceStopTimeOut = 5;
     
        private string _projectPublishFolder;

        public override string ProviderName => "windowService";
        public override string ProjectName => _serviceName;

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, _serviceName);
            _projectPublishFolder = Path.Combine(projectPath, DateTime.Now.ToString("yyyyMMddHHmmss"));
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

                Log("upload success ==>" + filePath);
                //解压
                try
                {
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
                    return "unzip publish file error,Path not found:" + deployFolder;
                }

                //查找 Windows Service 里面是否存在该服务
                var service = WindowServiceHelper.GetWindowServiceByName(this._serviceName);
                if (service == null)
                {
                    return $"windowService : {_serviceName} not found" ;
                }


                var projectLocation = WindowServiceHelper.GetWindowServiceLocation(this._serviceName);
                if (string.IsNullOrEmpty(projectLocation))
                {
                    return $"can not find executable path of service:{_serviceName}";
                }


                Log("Start to deploy Windows Service:");
                Log("ServiceName ===>" + _serviceName);
                Log("ServiceFolder ===> " + projectLocation);

                Arguments args = new Arguments
                {
                    DeployType =  "WindowsService",
                    BackupFolder = Setting.BackUpWindowServicePathFolder,
                    AppName = _serviceName,
                    AppFolder = projectLocation,
                    DeployFolder = deployFolder,
                    WaitForWindowsServiceStopTimeOut = _waitForServiceStopTimeOut
                };

                var ops = new OperationsWINDOWSSERVICE(args, Log);

                try
                {
                    ops.Execute();

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

            var serviceNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("serviceName"));
            if (serviceNameItem == null || string.IsNullOrEmpty(serviceNameItem.TextValue))
            {
                return "webSiteName required";
            }

            _serviceName = serviceNameItem.TextValue.Trim();
           

            var stopTimeOut = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("stopTimeOut"));
            if (stopTimeOut != null && !string.IsNullOrEmpty(stopTimeOut.TextValue))
            {
                _waitForServiceStopTimeOut = int.Parse(stopTimeOut.TextValue);
            }

            return string.Empty;
        }
    }
}
