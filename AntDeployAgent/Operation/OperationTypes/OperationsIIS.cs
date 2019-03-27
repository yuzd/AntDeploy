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
            }
            else
            {
                var retryStopWebSite = 0;
                ReTryStopWebSiet:
                retryStopWebSite++;
                var siteResult = IISHelper.WebsiteStop(this.args.SiteName);
                if (!string.IsNullOrEmpty(siteResult))
                {

                    if (retryStopWebSite >= 3)
                    {
                        logger($"【Error】File to IIS WebsiteStop :{this.args.SiteName},Err:{siteResult}, Retry limit.");
                        return;
                    }

                    logger($"File to IIS WebsiteStop :{this.args.SiteName},Err:{siteResult}, wait 5seconds and Retry : " + retryStopWebSite);
                    Thread.Sleep(5000);
                    goto ReTryStopWebSiet;
                }
            }


            
            if (!IISHelper.IsApplicationPoolStop(this.args.ApplicationPoolName))
            {
                var retryStopPool = 0;
                ReTryStopPool:
                retryStopPool++;

                var stopPoolResult = IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
                if (!string.IsNullOrEmpty(stopPoolResult))
                {
                    if (retryStopPool >= 3)
                    {
                        logger($"【Error】File to Stop IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{stopPoolResult},  Retry limit.");
                        return;
                    }

                    logger($"File to Stop IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{stopPoolResult}, wait 5seconds and Retry : " + retryStopPool);
                    Thread.Sleep(5000);
                    goto ReTryStopPool;
                }
            }

           
            logger("wait for IIS WebsiteStop 5sencods :" + this.args.SiteName);
            Thread.Sleep(5000);
            logger("Success to IIS WebsiteStop :" + this.args.SiteName);
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
                    logger("【Error】Retry Copy Limit ");
                    throw;
                }

                Deploy();
            }
        }

        public override void Start()
        {
            logger("Start to ApplicationPool :" + this.args.ApplicationPoolName);

            var retryStartPool = 0;
            ReTryStartPool:
            retryStartPool++;

            var poolstartReult = IISHelper.ApplicationPoolStart(this.args.ApplicationPoolName);
            if (!string.IsNullOrEmpty(poolstartReult))
            {
                if (retryStartPool >= 3)
                {
                    logger($"【Error】File to Start IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{poolstartReult},  Retry limit.");
                    return;
                }

                logger($"File to Start IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{poolstartReult}, wait 5seconds and Retry : " + retryStartPool);
                Thread.Sleep(5000);
                goto ReTryStartPool;
            }

            logger("Success to Start ApplicationPool:" + this.args.ApplicationPoolName);

            logger("Start to IIS WebsiteStart wait 5senconds:" + this.args.SiteName);
            Thread.Sleep(5000);

            var retryStartSite = 0;
            ReTryStartSite:
            retryStartSite++;

            var websiteStartResult = IISHelper.WebsiteStart(this.args.SiteName);

            if (!string.IsNullOrEmpty(websiteStartResult))
            {

                if (retryStartSite >= 3)
                {
                    logger($"【Error】File to Start IIS Websit :{ this.args.SiteName},Err:{websiteStartResult}, Retry limit.");
                    return;
                }

                logger($"File to Start IIS Website :{ this.args.SiteName},Err:{websiteStartResult}, wait 5seconds and Retry : " + retryStartSite);
                Thread.Sleep(5000);
                goto ReTryStartSite;
            }

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
