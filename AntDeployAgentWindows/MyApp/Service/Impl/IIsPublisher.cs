using AntDeployAgentWindows.WebApiCore;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AntDeployAgentWindows.Model;

namespace AntDeployAgentWindows.MyApp.Service.Impl
{
    public class IIsPublisher : PublishProviderBasicAPI
    {
        private object obj = new object();
        private string _webSiteName;
        private string _sdkTypeName;
        private string _projectName;
        private string _projectPublishFolder;
        private FormHandler _formHandler;

        public override string ProviderName => "iis";

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

                //解压
                try
                {
                    ZipFile.ExtractToDirectory(filePath, _projectPublishFolder);
                }
                catch (Exception ex)
                {
                    return "unzip publish file error:" + ex.Message;
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

            if (sdkTypeValue.Equals("netcore"))
            {
                //检查是否安装了netcore iis 的host包
                //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis

                var webConfig = Path.Combine(publishPath, "Web.Config");
                if (!File.Exists(webConfig))
                {
                    this.Logger.Error("publish success ==> " + publishPath);
                }
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
            if (_webSiteName.Contains("Default Web Site\\"))
            {
                _projectName = _webSiteName.Replace("Default Web Site\\", "");
            }
            else
            {
                _projectName = _webSiteName;
            }

            return string.Empty;
        }
    }
}
