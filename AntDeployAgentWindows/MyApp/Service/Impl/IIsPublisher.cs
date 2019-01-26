using AntDeployAgentWindows.WebApiCore;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebSocketApp;

namespace AntDeployAgentWindows.MyApp.Service.Impl
{
    public class IIsPublisher : PublishProviderBasicAPI
    {
        private object obj = new object();
        private string _webSiteName;
        private string _sdkTypeName;
        private string _projectName;
        private WebSocketApp.WebSocket WebSocket;
        private string _projectPublishFolder;
        private FormHandler _formHandler;

        public override string ProviderName => "iis";
        public override string ProjectName => _projectName;

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishIIsPathFolder, _projectName);
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

                //查找 IIS 里面是否存在
                var siteArr = _webSiteName.Split('/');
                var level1 = siteArr[0];
                var level2 = siteArr.Length==2? siteArr[1]:string.Empty;


                var projectLocation = IISHelper.GetWebSiteLocationInIIS(level1, level2,Log);
                if (projectLocation == null)
                {
                    return $"read info from iis error";
                }

                if (string.IsNullOrEmpty(projectLocation.Item1))
                {
                    return $"website : {_webSiteName} not found in iis" ;
                }

                Log("Start to deploy IIS:");
                Log("SiteName ===>" + projectLocation.Item2);
                Log("SiteFolder ===> " + projectLocation.Item1);
                Log("SiteApplicationPoolName ===> " + projectLocation.Item3);

                Arguments args = new Arguments
                {
                    DeployType =  "IIS",
                    BackupFolder = Setting.BackUpIIsPathFolder,
                    AppName = _projectName,
                    ApplicationPoolName = projectLocation.Item3,
                    AppFolder = projectLocation.Item1,
                    DeployFolder = deployFolder,
                    SiteName = projectLocation.Item2
                };

                var ops = new OperationsIIS(args, Log);

                try
                {
                    ops.Execute();

                    Log("Deploy IIS Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        ops.Rollback();

                        return $"publish to iis err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"publish to iis err:{ex.Message},rollback fail:{ex2.Message}";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private void EnsureProjectFolder(string path)
        {
            try
            {
                lock (obj)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }


        private void Log(string str)
        {
            try
            {
                if (WebSocket != null)
                {
                    WebSocket.Send(str + "@_@" + str.Length);
                }
            }
            catch (Exception)
            {
                //ignore

            }
        }

        public override string CheckData(FormHandler formHandler)
        {
            _formHandler = formHandler;
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

            var website = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("webSiteName"));
            if (website == null || string.IsNullOrEmpty(website.TextValue))
            {
                return "webSiteName required";
            }

            if (website.TextValue.Length > 100)
            {
                return "webSiteName is too long";
            }

            _webSiteName = website.TextValue.Trim();
            var siteNameArr = _webSiteName.Split('/');
            if (siteNameArr.Length > 2)
            {
                return "webSiteName level limit is 2";
            }

            //var projectName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("projectName"));
            //if (projectName == null || string.IsNullOrEmpty(projectName.TextValue))
            //{
            //    return "projectName required";
            //}

            _projectName = siteNameArr.Last();

            var wsKey = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("wsKey"));
            if (wsKey != null  && !string.IsNullOrEmpty(wsKey.TextValue))
            {
                var _wsKey = wsKey.TextValue;
                MyWebSocketWork.WebSockets.TryGetValue(_wsKey, out WebSocket);
            }

            return string.Empty;
        }
    }
}
