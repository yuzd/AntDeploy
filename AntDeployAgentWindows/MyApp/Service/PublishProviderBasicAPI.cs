using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.WebApiCore;
using Newtonsoft.Json;

namespace AntDeployAgentWindows.MyApp.Service
{
    public abstract class PublishProviderBasicAPI : CommonProcessor, IPublishProviderAPI
    {
        public abstract string ProviderName { get; }
        public abstract string DeployExcutor(FormHandler.FormItem fileItem);
        public abstract string CheckData(FormHandler formHandler);
        public string Deploy(FormHandler.FormItem fileItem)
        {
            return DeployExcutor(fileItem);
        }


        public string Check(FormHandler formHandler)
        {
            return CheckData(formHandler);
        }
    }

   
}
