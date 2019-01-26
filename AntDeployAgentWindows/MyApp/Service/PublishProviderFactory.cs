using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.MyApp.Service.Impl;

namespace AntDeployAgentWindows.MyApp.Service
{
    public class PublishProviderFactory
    {
        private static List<IPublishProviderAPI> providerApis = new List<IPublishProviderAPI>();
        static PublishProviderFactory()
        {
            providerApis.Add(new IIsPublisher());
           
        }
       

        public static IPublishProviderAPI GetProcessor(string key)
        {
            return providerApis.FirstOrDefault(r => r.ProviderName.Equals(key));
        }
    }
}
