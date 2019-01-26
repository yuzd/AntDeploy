using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsWINDOWSSERVICE : OperationsBase
    {
        public OperationsWINDOWSSERVICE(Arguments args)
            : base(args)
        {
            // do nothing
        }

        public override void ValidateArguments()
        {
            base.ValidateArguments();

            if (!this.args.NoStop)
            {
                try
                {
                    ServiceController service = new ServiceController(this.args.AppName);
                    ServiceControllerStatus status = service.Status;
                }
                catch (Exception)
                {
                    throw new ArgumentException(this.args.AppName + " windows service could not be found", "appName");
                }
            }
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
            ServiceController service = new ServiceController(this.args.AppName);
            if (service.CanStop)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(120));
            }
            else
            {
                Console.WriteLine(this.args.AppName + " windows service could not be stopped");
            }
        }

        public override void Deploy()
        {
            base.Deploy();
        }

        public override void Start()
        {
            ServiceController service = new ServiceController(this.args.AppName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(120));
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
