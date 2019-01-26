using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.Util;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsIIS : OperationsBase
    {
        private Action<string> logger;
        public OperationsIIS(Arguments args,Action<string> log)
            : base(args)
        {
            // do nothing
            logger = log;
        }

        public override void ValidateArguments()
        {
            base.ValidateArguments();
        }

        public override void Backup()
        {
            logger("Start to Backup");
            string destDir = Path.Combine(this.args.BackupFolder, this.args.AppName);
            destDir = Path.Combine(destDir, DateTime.Now.ToString("Backup_yyyyMMdd_HHmmss"));
            this.args.RestorePath = destDir;
            CopyHelper.DirectoryCopy(this.args.AppFolder, destDir, true);
            logger("Success Backup to folder:" + destDir);
        }

        public override void Restore()
        {
            base.Restore();
        }

        public override void Stop()
        {
            logger("Start to IIS WebsiteStop :" + this.args.SiteName);
            IISHelper.WebsiteStop(this.args.SiteName);
            logger("Success to IIS WebsiteStop :" + this.args.SiteName);
            //IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
        }

        public override void Deploy()
        {
            logger("Start to Deploy," + this.args.DeployFolder + "=>" + this.args.AppFolder);

            base.Deploy();
            logger("End to Deploy");
        }

        public override void Start()
        {
            logger("Start to IIS WebsiteStart :" + this.args.SiteName);
            IISHelper.WebsiteStart(this.args.SiteName);
            logger("Success to IIS WebsiteStart :" + this.args.SiteName);
            //IISHelper.WebsiteStart(this.args.SiteName);
        }

        public override void Execute()
        {
            logger("Start to Execute");
            base.Execute();
            logger("End to Execute");
        }

        public override void Rollback()
        {
            base.Rollback();
        }
    }
}
