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
                case "windowservice":
                    return new WindowServicePublisher();
                default:
                    throw new Exception($"key:{key} not supported");
            }
        }
    }
}
