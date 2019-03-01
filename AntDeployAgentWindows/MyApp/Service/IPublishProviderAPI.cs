using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.WebApiCore;

namespace AntDeployAgentWindows.MyApp.Service
{
  
    public interface IPublishProviderAPI
    {
        string ProviderName { get; }
        string LoggerKey { get;  set;}

        /// <summary>
        /// 发布
        /// </summary>
        /// <returns></returns>
        string Deploy(FormHandler.FormItem fileItem);
        string RollBack();

        string Check(FormHandler formHandler);

       

    }
}
