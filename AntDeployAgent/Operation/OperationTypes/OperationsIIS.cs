﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgent.Util;
using AntDeployAgentWindows.Util;
using Microsoft.Web.Administration;

namespace AntDeployAgentWindows.Operation.OperationTypes
{
    class OperationsIIS : OperationsBase
    {
        private int retryTimes = 0;
        public OperationsIIS(Arguments args, Action<string> log)
            : base(args, log)
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
            logger("Start to Stop IIS Website: " + this.args.SiteName);
            if (IISHelper.IsWebsiteStop(this.args.SiteName))
            {
                logger("Success to Stop IIS Website :" + this.args.SiteName);
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
                        logger($"【Error】Fail to Stop IIS Website :{this.args.SiteName},Err:{siteResult}, Retry limit.");
                        killPool();
                        return;
                    }

                    logger($"Fail to Stop IIS Website :{this.args.SiteName},Err:{siteResult}, wait 5seconds and Retry : " + retryStopWebSite);
                    Thread.Sleep(5000);
                    goto ReTryStopWebSiet;
                }
            }


            logger("Start to Stop IIS ApplicationPool :" + this.args.ApplicationPoolName);
            if (!IISHelper.IsApplicationPoolStop(this.args.ApplicationPoolName))
            {
                logger("Recycle IIS ApplicationPool :" + this.args.ApplicationPoolName);
                IISHelper.ApplicationPoolRecycle(this.args.ApplicationPoolName);

                var retryStopPool = 0;
                ReTryStopPool:
                retryStopPool++;

                var stopPoolResult = IISHelper.ApplicationPoolStop(this.args.ApplicationPoolName);
                if (!string.IsNullOrEmpty(stopPoolResult))
                {
                    if (retryStopPool >= 3)
                    {
                        logger($"【Error】Fail to Stop IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{stopPoolResult},  Retry limit.");
                        killPool();
                        return;
                    }

                    logger($"Fail to Stop IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{stopPoolResult}, wait 5seconds and Retry : " + retryStopPool);
                    Thread.Sleep(5000);
                    goto ReTryStopPool;
                }

                logger("Success to Stop ApplicationPool :" + this.args.ApplicationPoolName);
            }
            else
            {
                logger("ApplicationPool :" + this.args.ApplicationPoolName + " is already stoped！");
            }


            logger("wait for IIS WebsiteStop 5sencods :" + this.args.SiteName);
            Thread.Sleep(5000);
            logger("Success to IIS WebsiteStop :" + this.args.SiteName);
        }

        public override void Deploy()
        {
            if (args.UseTempPhysicalPath)
            {
                //先从iis站点 this.args.AppFolder 复制 到 当前 日期文件夹下的deploy文件夹  然后在 DeployFolder 复制到 日期文件夹下的deploy文件夹
                var isSuccess = CopyHelper.ProcessXcopy(this.args.AppFolder, this.args.TempPhysicalPath, logger);

                if (!isSuccess)
                {
                    logger($"【Error】Copy `{this.args.AppFolder}` to `{this.args.TempPhysicalPath}` fail");
                    throw new Exception($"Copy `{this.args.AppFolder}` to `{this.args.TempPhysicalPath}` fail");
                }

                isSuccess = CopyHelper.ProcessXcopy(this.args.DeployFolder, this.args.TempPhysicalPath, logger);

                if (!isSuccess)
                {
                    logger($"【Error】Copy `{this.args.DeployFolder}` to `{this.args.TempPhysicalPath}` fail");
                    throw new Exception($"Copy `{this.args.DeployFolder}` to `{this.args.TempPhysicalPath}` fail");
                }

                //修改物理路径
                var err = IISHelper.ChangePhysicalPath(this.args.Site1,this.args.Site2, this.args.TempPhysicalPath);
                if (!string.IsNullOrEmpty(err))
                {
                    logger($"【Error】Change `{this.args.SiteName}` physicalPath to `{this.args.TempPhysicalPath}` fail");
                    throw new Exception($"【Error】Change `{this.args.SiteName}` physicalPath to `{this.args.TempPhysicalPath}` fail");
                }

                //回收一下
                var r1 = ProcessHepler.RunAppCmd($"recycle apppool /apppool.name:\"{this.args.ApplicationPoolName}\"", logger);
                logger($"recycle apppool /apppool.name:{this.args.ApplicationPoolName} ===> {(r1 ? "Success" : "Fail")}");

                return;
            }


            try
            {
                if (args.UseOfflineHtm)
                {
                    try
                    {


                        var createErr = IISHelper.CreateAppOffineHtm(this.args.AppFolder);
                        if (!string.IsNullOrEmpty(createErr))
                        {
                            logger($"【Error】Create app_offline.htm to [{this.args.AppFolder}] Fail:[{createErr}]");
                            return;
                        }

                        logger($"create app_offline.htm to [{this.args.AppFolder}] Success");

                        //创建了app_offline.htm成功后 iis会解除占用

                        //执行copy

                        if (args.UseTempPhysicalPath)
                        {

                        }
                        else
                        {
                            base.Deploy();
                        }
                    }
                    finally
                    {
                        var deleteErr = IISHelper.DeleteAppOfflineHtm(this.args.AppFolder);
                        if (!string.IsNullOrEmpty(deleteErr))
                        {
                            logger($"【Error】delete app_offline.htm from [{this.args.AppFolder}] Fail:[{deleteErr}]");
                        }

                        logger($"delete app_offline.htm from [{this.args.AppFolder}] Success");
                    }
                }
                else
                {
                    base.Deploy();
                }
            }
            catch (Exception exception)
            {
                logger("Copy File Fail :" + exception.Message);
                retryTimes++;
                logger("Wait 5Senconds to Retry :" + retryTimes);
                Thread.Sleep(5000);
                if (retryTimes > 3)
                {
                    IISHelper.KillSiteProcess(args.SiteName);
                    killPool();
                    logger("Wait 5Senconds to Try deploy again");
                    Thread.Sleep(5000);
                    try
                    {
                        base.Deploy();
                        return;
                    }
                    catch (Exception)
                    {
                        logger("【Error】Retry Copy Limit ");
                        throw;
                    }

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
                    logger($"【Error】Fail to Start IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{poolstartReult},  Retry limit.");
                    return;
                }

                logger($"Fail to Start IIS ApplicationPool :{this.args.ApplicationPoolName },Err:{poolstartReult}, wait 5seconds and Retry : " + retryStartPool);
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
                    logger($"【Error】Fail to Start IIS Website :{ this.args.SiteName},Err:{websiteStartResult}, Retry limit max:3");
                    return;
                }

                logger($"Fail to Start IIS Website :{ this.args.SiteName},Err:{websiteStartResult}, wait 5seconds and Retry : " + retryStartSite);
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

        private void killPool()
        {

            //执行终极方法 执行pool的回收 因为每个应用的pool是唯一的情况下 不会影响别的站点
            var r1 = ProcessHepler.RunAppCmd($"recycle apppool /apppool.name:\"{this.args.ApplicationPoolName}\"", logger);
            logger($"recycle apppool /apppool.name:{this.args.ApplicationPoolName} ===> {(r1 ? "Success" : "Fail")}");
            var r2 = ProcessHepler.RunAppCmd($"stop apppool /apppool.name:\"{this.args.ApplicationPoolName}\"", logger);
            logger($"stop apppool /apppool.name:{this.args.ApplicationPoolName} ===> {(r2 ? "Success" : "Fail")}");

            // 直接关闭站点可能会影响其他的站点
            //var r3 = ProcessHepler.RunAppCmd($"stop site /site.name:\"{this.args.SiteName}\"", logger);
            //logger($"stop site /site.name:{this.args.SiteName} ===> {(r3 ? "Success" : "Fail")}");
        }
    }
}
