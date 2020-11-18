using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgent.Util;
using AntDeployAgentWindows.Util;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsLinux : OperationsBase
    {
        private int retryTimes = 0;
        public OperationsLinux(Arguments args,Action<string> log)
            : base(args,log)
        {
        }

        public override void ValidateArguments()
        {
            base.ValidateArguments();
        }

        public override void Backup()
        {
            base.Backup();
        }

        public override void Restore()
        {
            base.Restore();
        }

        public override void Stop()
        {
            logger("Start to Linux Service Stop :" + this.args.AppName);
            logger($"【Command】sudo systemctl stop {this.args.AppName}");

            var result = CopyHelper.RunCommand($"sudo systemctl stop {this.args.AppName}", null, null);
            if (!result)
            {
                logger?.Invoke("【Command】" + $"sudo systemctl stop {this.args.AppName}" + "--->Fail");
            }

        }

        public override void Deploy()
        {
            try
            {
                base.Deploy();
            }
            catch (Exception exception)
            {
                logger("Copy File Fail :" + exception.Message);
                retryTimes++;
                logger("Wait 5Senconds to Retry :" + retryTimes);
                Thread.Sleep(5000);
                if (retryTimes > 3)
                {
                    throw new Exception("【Error】Retry Copy Limit ");
                }

                Deploy();
            }
        }

        public override void Start()
        {
            logger("Start to linux Service Start :" + this.args.AppName);

            LinuxServiceHelper.ServiceRun(this.args.AppName, this.args.TempPhysicalPath, this.logger);

            var runSuccess = false;

            Thread.Sleep(5000);

            CopyHelper.RunCommand($"systemctl status {this.args.AppName}", null, (msg) =>
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    this.logger.Invoke("【Command】"+msg);
                    var msg1 = msg.ToLower();
                    if (msg1.Contains("active:") && msg1.Contains("running"))
                    {
                        runSuccess = true;
                    }
                }
            });

            if (!runSuccess)
            {
                throw new Exception("【Error】Start service Fail ");
            }
        }

        public override void Execute()
        {
            retryTimes = 0;
            base.Execute();
        }

        public override void Rollback()
        {
            base.Rollback();
        }
    }
}
