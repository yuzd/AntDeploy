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
    public class WindowServiceRollback : PublishProviderBasicAPI
    {
        private string _sdkTypeName;
        private bool _isProjectInstallService;
        private string _serviceName;
        private string _serviceExecName;
        private int _waitForServiceStopTimeOut = 15;
     
        private string _projectPublishFolder;
        private string _dateTimeFolderName;

        public override string ProviderName => "windowService";
        public override string ProjectName => _serviceName;

        public override string RollBack()
        {
            return base.RollBack();
        }


        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            return null;
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

            return string.Empty;
        }
    }
}
