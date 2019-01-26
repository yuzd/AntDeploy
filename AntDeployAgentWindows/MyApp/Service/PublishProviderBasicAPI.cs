using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgentWindows.WebApiCore;
using Newtonsoft.Json;

namespace AntDeployAgentWindows.MyApp.Service
{
    public abstract class PublishProviderBasicAPI : CommonProcessor, IPublishProviderAPI
    {
        private static readonly ConcurrentDictionary<string, ReaderWriterLockSlim> locker = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
        public abstract string ProviderName { get; }
        public abstract string ProjectName { get; }
        public abstract string DeployExcutor(FormHandler.FormItem fileItem);
        public abstract string CheckData(FormHandler formHandler);
        public string Deploy(FormHandler.FormItem fileItem)
        {
            //按照项目名称 不能并发发布
            if (!string.IsNullOrEmpty(ProjectName))
            {
                if (!locker.TryGetValue(ProjectName, out var ReaderWriterLockSlim))
                {
                    ReaderWriterLockSlim = new ReaderWriterLockSlim();
                    locker.TryAdd(ProjectName, ReaderWriterLockSlim);
                }

                if (ReaderWriterLockSlim.IsWriteLockHeld)
                {
                    return $"{ProjectName} is deploying!,please wait for senconds!";
                }

                ReaderWriterLockSlim.EnterWriteLock();

                try
                {
                    return DeployExcutor(fileItem);
                }
                finally
                {
                    ReaderWriterLockSlim.ExitWriteLock();
                }
               
            }
            else
            {
                return DeployExcutor(fileItem);
            }
        }


        public string Check(FormHandler formHandler)
        {
            return CheckData(formHandler);
        }
    }

   
}
