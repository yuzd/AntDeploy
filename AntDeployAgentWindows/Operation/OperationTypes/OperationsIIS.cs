using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgentWindows.Util;
using Microsoft.Web.Administration;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsIIS : OperationsBase
    {
        private int retryTimes = 0;
        public OperationsIIS(Arguments args,Action<string> log)
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
            logger("Start to IIS WebsiteStop :" + this.args.SiteName);
            if (IISHelper.IsWebsiteStop(this.args.SiteName))
            {
                logger("Success to IIS WebsiteStop :" + this.args.SiteName);
                return;
            }

            var site = IISHelper.WebsiteStop(this.args.SiteName);
            if (!IISHelper.IsApplicationPoolStop(this.args.ApplicationPoolName))
            {
                IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
            }

            while (site.State != ObjectState.Stopped)
            {
                logger("wait for IIS WebsiteStop :" + this.args.SiteName);
                Thread.Sleep(1000);
            }
            logger("wait for IIS WebsiteStop 5sencods :" + this.args.SiteName);
            Thread.Sleep(5000);
            logger("Success to IIS WebsiteStop :" + this.args.SiteName);
            //IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
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
                logger("Wait 5Sencond to Retry :" + retryTimes);
                Thread.Sleep(5000);
                if (retryTimes > 3)
                {
                    logger("Retry Copy Limit ");
                    throw;
                }

                Deploy();
            }
        }

        public override void Start()
        {
            logger("Start to ApplicationPool :" + this.args.ApplicationPoolName);
            IISHelper.ApplicationPoolStart(this.args.ApplicationPoolName);
            logger("Success to Start ApplicationPool:" + this.args.ApplicationPoolName);

            logger("Start to IIS WebsiteStart wait 5senconds:" + this.args.SiteName);
            Thread.Sleep(5000);
            IISHelper.WebsiteStart(this.args.SiteName);
            logger("Success to IIS WebsiteStart :" + this.args.SiteName);
            //IISHelper.WebsiteStart(this.args.SiteName);
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
