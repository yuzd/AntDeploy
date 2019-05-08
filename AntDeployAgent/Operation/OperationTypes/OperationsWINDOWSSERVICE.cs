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
    class OperationsWINDOWSSERVICE : OperationsBase
    {
        private int retryTimes = 0;
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
            var serviceR = WindowServiceHelper.GetWindowServiceByName(this.args.AppName);
            if (!string.IsNullOrEmpty(serviceR.Item2))
            {
                logger($"【Error】{serviceR.Item2}");
            }
            var service = serviceR.Item1;
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
                    //执行终极方法 用sc命令执行
                    var r1 = ProcessHepler.RuSCCmd($"stop \"{this.args.AppName}\"", logger);
                    logger($"sc stop {this.args.AppName} ===> {(r1 ? "Success" : "Fail")}");
                    logger("Wait 5Senconds to Try deploy again");
                    Thread.Sleep(5000);
                    try
                    {
                        base.Deploy();
                        return;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            logger("Wait 5Senconds to Try deploy again+1");
                            Thread.Sleep(5000);
                            base.Deploy();
                            return;
                        }
                        catch (Exception)
                        {
                            logger("【Error】Retry Copy Limit ");
                            throw;
                        }
                    }
                }

                Deploy();
            }
        }

        public override void Start()
        {
            logger("Start to Windows Service Start :" + this.args.AppName);
            var serviceR = WindowServiceHelper.GetWindowServiceByName(this.args.AppName);
            if (!string.IsNullOrEmpty(serviceR.Item2))
            {
                logger($"【Error】{serviceR.Item2}");
                return;
            }
            var service = serviceR.Item1;
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                var re = WindowServiceHelper.StartService(this.args.AppName, 300);
                if (string.IsNullOrEmpty(re))
                {
                    service = WindowServiceHelper.GetWindowServiceByName(this.args.AppName).Item1;
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
                    service = WindowServiceHelper.GetWindowServiceByName(this.args.AppName).Item1;
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
            retryTimes = 0;
            base.Execute();
        }

        public override void Rollback()
        {
            base.Rollback();
        }
    }
}
