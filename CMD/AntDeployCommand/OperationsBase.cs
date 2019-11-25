using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;
using LibGit2Sharp;

namespace AntDeployCommand
{
    public abstract class OperationsBase : IOperations
    {
      
        public Arguments Arguments;


        


        public abstract string ValidateArgument();
        public abstract Task<bool> Run();

        public void ValidateArguments()
        {
        

            var re = ValidateArgument();
            if (!string.IsNullOrEmpty(re))
            {
                throw new ArgumentException(re);
            }
        }

        public async Task<bool> Execute()
        {
            return await Run();
        }

        public virtual string Name => $"【{this.GetType().Name}】";

        protected void Log(string msg, LogLevel level)
        {
            msg = Name + msg;
            if (level == LogLevel.Warning)
            {
                LogHelper.Warn(msg);
            }
            else if (level == LogLevel.Error)
            {
                LogHelper.Error(msg);
            }
            else if (level == LogLevel.Debug)
            {
                LogHelper.Debug(msg);
            }
            else
            {
                LogHelper.Info(msg);
            }
        }
        protected void Info(string msg)
        {
            Log(msg, LogLevel.Info);
        }
        protected void Debug(string msg)
        {
            Log(msg, LogLevel.Debug);
        }
        protected void Warn(string msg)
        {
            Log(msg, LogLevel.Warning);
        }
        protected void Error(string msg)
        {
            Log(msg, LogLevel.Error);
        }
        protected IWebProxy GetProxy()
        {
            try
            {
                if (string.IsNullOrEmpty(Arguments.Proxy)) return null;
                var proxy = new WebProxy(Arguments.Proxy);
                Warn($"UseProxy:【{Arguments.Proxy}】");
                return proxy;
            }
            catch (Exception e)
            {
                Warn("UseProxy Fail:" + Arguments.Proxy + ",Err:" + e.Message);
                return null;
            }
        }
    }

}
