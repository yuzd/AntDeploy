using AntDeployAgentWindows.MyApp.Service.Impl;
using System;

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
                case "iis_rollback":
                    return new IIsRollback();
                case "windowservice":
                    return new WindowServicePublisher();
                case "windowservice_rollback":
                    return new WindowServiceRollback();
                default:
                    throw new Exception($"key:{key} not supported");
            }
        }
    }
}
