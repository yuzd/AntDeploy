using AntDeployAgentWindows.Util;
using System;
using System.IO;

namespace AntDeployAgentWindows.Operation
{
    class OperationsBase : IOperations
    {
        protected Arguments args;
        protected OperationStep step = OperationStep.NONE;
        protected Action<string> logger;
        public OperationsBase(Arguments args, Action<string> log)
        {
            this.args = args;
            this.logger = log;
        }

        public virtual void ValidateArguments()
        {
            if (string.IsNullOrEmpty(this.args.DeployType))
                throw new ArgumentNullException("deployType", "deployType can not be null or empty");

            if (!this.args.NoBackup && !this.args.Restore && string.IsNullOrEmpty(this.args.BackupFolder))
                throw new ArgumentNullException("backupFolder", "backupFolder can not be null or empty");

            if (string.IsNullOrEmpty(this.args.AppName))
                throw new ArgumentNullException("appName", "appName can not be null or empty");

            if (string.IsNullOrEmpty(this.args.AppFolder))
                throw new ArgumentNullException("appFolder", "appFolder can not bu null or empty");

            if (!this.args.Restore && string.IsNullOrEmpty(this.args.DeployFolder))
                throw new ArgumentNullException("deployFolder", "deployFolder can not be null or empty");

            if (this.args.Restore && string.IsNullOrEmpty(this.args.RestorePath))
                throw new ArgumentNullException("restorePath", "restorePath can not be null or empty");

            if (!Directory.Exists(this.args.AppFolder))
                throw new ArgumentException((this.args.AppFolder ?? string.Empty) + " folder does not exist", "appFolder");

            if (!this.args.Restore && !Directory.Exists(this.args.DeployFolder))
                throw new ArgumentException((this.args.DeployFolder ?? string.Empty) + " folder does not exist", "deployFolder");

            if (this.args.Restore && !Directory.Exists(this.args.RestorePath))
                throw new ArgumentException((this.args.RestorePath ?? string.Empty) + " folder does not exist", "restorePath");
        }

        public virtual void Backup()
        {
            logger("Start to Backup");
            string destDir = Path.Combine(this.args.BackupFolder, this.args.AppName);
            destDir = Path.Combine(destDir, DateTime.Now.ToString("Backup_yyyyMMdd_HHmmss"));
            this.args.RestorePath = destDir;
            DirectoryInfo directoryInfo = new DirectoryInfo(this.args.AppFolder);
            string fullName = directoryInfo.FullName;
            if (directoryInfo.Parent != null)
                fullName = directoryInfo.Parent.FullName;
            CopyHelper.DirectoryCopy(this.args.AppFolder, destDir, true, fullName,directoryInfo.Name, this.args.BackUpIgnoreList);
            logger("Success Backup to folder:" + destDir);
        }

        public virtual void Restore()
        {
            logger("Start to Restore");
            CopyHelper.DirectoryCopy(this.args.RestorePath, this.args.AppFolder, true);
            logger("Success Restore from folder:[" + this.args.RestorePath + "] to folder:[" + this.args.AppFolder + "]");
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }

        public virtual void Deploy()
        {
            logger("Start to Deploy");
            CopyHelper.DirectoryCopy(this.args.DeployFolder, this.args.AppFolder, true);
            logger("Success Deploy from folder:[" + this.args.DeployFolder + "] to folder [" + this.args.AppFolder + "]");
        }

        public virtual void Start()
        {
            throw new NotImplementedException();
        }

        public virtual void Execute()
        {
            logger("Start to Execute");
            this.args.Restore = true;

            if (!this.args.NoBackup)
            {
                this.Backup();
                //Console.WriteLine(this.args.DeployType + " backup complete...");
            }
            step = OperationStep.BACKEDUP;

            if (!this.args.NoStop)
            {
                this.Stop();
                //Console.WriteLine(this.args.DeployType + " stop complete...");
            }
            step = OperationStep.STOPPED;

            this.Deploy();
            //Console.WriteLine(this.args.DeployType + " deploy complete...");
            step = OperationStep.DEPLOYED;

            if (!this.args.NoStart)
            {
                this.Start();
                //Console.WriteLine(this.args.DeployType + " start complete...");
            }
            step = OperationStep.STARTED;

            logger("End to Execute");
        }

        public virtual void Rollback()
        {
            if (this.args.Restore)
                this.step = OperationStep.STARTED;

            if (this.step == OperationStep.NONE || this.step == OperationStep.BACKEDUP)
            {
                //Console.WriteLine("No rollback required");
                return;
            }

            if (this.step == OperationStep.STARTED && !this.args.NoStart)
            {
                this.Stop();
                //Console.WriteLine(this.args.DeployType + " stop complete...");
            }

            if (this.step == OperationStep.STARTED || this.step == OperationStep.DEPLOYED)
            {
                this.Restore();
                //Console.WriteLine(this.args.DeployType + " restore complete...");
            }

            if ((this.step == OperationStep.STARTED || this.step == OperationStep.DEPLOYED || this.step == OperationStep.STOPPED) && !this.args.NoStop)
            {
                this.Start();
                //Console.WriteLine(this.args.DeployType + " start complete...");
            }
        }
    }
}
