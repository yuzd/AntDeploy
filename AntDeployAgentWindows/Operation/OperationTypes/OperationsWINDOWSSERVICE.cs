﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgentWindows.Util;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsWINDOWSSERVICE : OperationsBase
    {
        
        public OperationsWINDOWSSERVICE(Arguments args,Action<string> log)
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
            logger("Start to Windows Service Stop :" + this.args.AppName);
            var service = WindowServiceHelper.GetWindowServiceByName(this.args.AppName);
            if (service!=null)
            {
                if(service.Status == ServiceControllerStatus.Stopped)
                {
                    logger("Success to Windows Service Stop :" + this.args.AppName);
                }
                else
                {
                    if (service.CanStop)
                    {
                        var timeout = (this.args.WaitForWindowsServiceStopTimeOut > 0
                            ? this.args.WaitForWindowsServiceStopTimeOut
                            : 10);
                        logger("Start to Windows Service Stop wait for " + timeout + "senconds :" + this.args.AppName);
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(timeout));
                        logger("Success to Windows Service Stop :" + this.args.AppName);
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        logger("【Error】 Windows Service Stop :" + this.args.AppName + ",Err: windows service could not be stopped,please check the service status!");
                    }
                }
               
            }
            else
            {
                logger("【Error】 Windows Service Stop :" + this.args.AppName + ",Err: service can not exist!");
            }
        }

        public override void Deploy()
        {
            base.Deploy();
        }

        public override void Start()
        {
            logger("Start to Windows Service Start :" + this.args.AppName);
            var service = WindowServiceHelper.GetWindowServiceByName(this.args.AppName);
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                var re = WindowServiceHelper.StartService(this.args.AppName, 300);
                if (string.IsNullOrEmpty(re))
                {
                    service = WindowServiceHelper.GetWindowServiceByName(this.args.AppName);
                    service.Refresh();
                    if (service.Status != ServiceControllerStatus.Running&&service.Status != ServiceControllerStatus.StartPending)
                    {
                        logger("【Error】 Windows Service Start :" + this.args.AppName + ",Err: service can not start");
                    }
                    else
                    {
                        logger("Success to Windows Service Start :" + this.args.AppName);
                    }
                }
                else
                {
                    service = WindowServiceHelper.GetWindowServiceByName(this.args.AppName);
                    service.Refresh();

                    if (service.Status != ServiceControllerStatus.Running&&service.Status != ServiceControllerStatus.StartPending)
                    {
                        throw new Exception("【Error】 Windows Service Start :" + this.args.AppName + ",Err:" + re + "=>[suggestion]if this is your first time,you can try again!");
                    }
                    else
                    {
                        logger("Success to Windows Service Start :" + this.args.AppName);
                    }
                }
            }
            else if (service.Status == ServiceControllerStatus.Running)
            {
                logger("Success to Windows Service Start :" + this.args.AppName);
            }
            else
            {
                logger($"Windows Service:{this.args.AppName} Status is not Stopped ");
            }
           
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
