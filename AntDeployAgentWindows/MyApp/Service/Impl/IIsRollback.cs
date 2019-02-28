using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntDeployAgentWindows.MyApp.Service.Impl
{
    public class IIsRollback : PublishProviderBasicAPI
    {

        private string _webSiteName;
        private string _sdkTypeName;
        private string _projectName;
        private string _port;
        private string _poolName;
        private string _dateTimeFolderName;

        private string _projectPublishFolder;
        private FormHandler _formHandler;

        public override string ProviderName => "iis";
        public override string ProjectName => _projectName;

        public override string RollBack()
        {
            //获取到版本
            //check版本是会否存在
            //如果存在那么执行重新覆盖
            //读取该项目是否存在 不存在就报错
            //读取该项目的地址 地址不存在就报错
            //执行覆盖
            return base.RollBack();
        }

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            return null;
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

            var portItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("port"));
            if (portItem != null && !string.IsNullOrEmpty(portItem.TextValue))
            {
                _port = portItem.TextValue;
            }

            var poolNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("poolName"));
            if (poolNameItem != null && !string.IsNullOrEmpty(poolNameItem.TextValue))
            {
                _poolName = poolNameItem.TextValue;
            }

            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }

            _projectName = getCorrectFolderName(_webSiteName);
            return string.Empty;
        }

        
    }
}
