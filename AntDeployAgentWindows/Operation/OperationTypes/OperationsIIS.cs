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
            var site = IISHelper.WebsiteStop(this.args.SiteName);
            IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
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
            base.Deploy();
        }

        public override void Start()
        {
            logger("Start to IIS WebsiteStart :" + this.args.SiteName);
            IISHelper.WebsiteStart(this.args.SiteName);
            IISHelper.ApplicationPoolStart(this.args.ApplicationPoolName);
            logger("Success to IIS WebsiteStart :" + this.args.SiteName);
            //IISHelper.WebsiteStart(this.args.SiteName);
        }

        public override void Execute()
        {
            base.Execute();
        }

        public override void Rollback()
        {
            base.Rollback();
        }
    }
}
