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
            logger("Start to IIS ApplicationPoolStop :" + this.args.ApplicationPoolName);
            IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
            logger("Success to IIS ApplicationPoolStop :" + this.args.ApplicationPoolName);
            //IISHelper.WebsiteStop(this.args.SiteName);
        }

        public override void Deploy()
        {
            base.Deploy();
        }

        public override void Start()
        {
            logger("Start to IIS ApplicationPoolStart :" + this.args.ApplicationPoolName);
            IISHelper.ApplicationPoolStart(this.args.ApplicationPoolName);
            logger("Success to IIS ApplicationPoolStart :" + this.args.ApplicationPoolName);
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
