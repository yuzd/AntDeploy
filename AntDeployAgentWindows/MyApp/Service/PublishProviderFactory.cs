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
       

        public static IPublishProviderAPI GetProcessor(string key)
        {
            switch (key)
            {
                case "iis":
                    return new IIsPublisher();
                default:
                    throw new Exception($"key:{key} not supported");
            }
        }
    }
}
