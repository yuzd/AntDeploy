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
                case "docker":
                    return new DockerPublisher();
                case "docker_rollback":
                    return new DockerRollback();
                case "linux":
                    return new LinuxPublisher();
                case "linux_rollback":
                    return new LinuxRollback();
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
