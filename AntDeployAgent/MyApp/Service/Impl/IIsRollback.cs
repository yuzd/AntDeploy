﻿using System;
using System.IO;
using System.Linq;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System.Runtime.InteropServices;
namespace AntDeployAgent.MyApp.Service.Impl
{
    public class IIsRollback : PublishProviderBasicAPI
    {

        private string _webSiteName;
        private string _projectName;
        private string _dateTimeFolderName;

        private string _projectPublishFolder;
        private FormHandler _formHandler;

        public override string ProviderName => "iis";
        public override string ProjectName => _projectName;
        public override string ProjectPublishFolder => _projectPublishFolder;

        public override string RollBack()
        {
            try
            {
                //获取到版本
                //check版本是会否存在
                //如果存在那么执行重新覆盖
                //读取该项目是否存在 不存在就报错
                //读取该项目的地址 地址不存在就报错
                //执行覆盖
                var projectPath = Path.Combine(Setting.PublishIIsPathFolder, _projectName);
                _projectPublishFolder = Path.Combine(projectPath, _dateTimeFolderName);

                if (!Directory.Exists(_projectPublishFolder))
                {
                    return "rollback folder not found:" + _projectPublishFolder;
                }

#if NETCORE
                Log("netcore agent version ==>" + Version.VERSION);
#else
                Log("netframework agent version ==>" + Version.VERSION);
#endif

                var deployFolder = findUploadFolder(_projectPublishFolder);


                var isUseTempPhysicalPath = false;
                var ddeploy = Path.Combine(_projectPublishFolder, "_deploy_");

                if (Directory.Exists(ddeploy))
                {
                    deployFolder = ddeploy;
                    //这里需要直接修改物理路径就可以了
                    isUseTempPhysicalPath = true;
                }
                else
                {
                    var incrementFolder = Path.Combine(_projectPublishFolder, "increment");

                    if (Directory.Exists(incrementFolder))
                    {
                        deployFolder = incrementFolder;
                    }
                }



                if (!Directory.Exists(deployFolder))
                {
                    return "rollback folder not found:" + deployFolder;
                }

                Log("rollback from folder ==>" + deployFolder);

                //查找 IIS 里面是否存在
                var siteArr = _webSiteName.Split('/');
                if (siteArr.Length > 2)
                {
                    return $"website level limit is 2！";
                }

                var level1 = siteArr[0];
                var level2 = siteArr.Length == 2 ? siteArr[1] : string.Empty;
                var isSiteExistResult = IISHelper.IsSiteExist(level1, level2);
                if (!string.IsNullOrEmpty(isSiteExistResult.Item3))
                {
                    return $"【Error】 : {isSiteExistResult.Item3}";
                }

                if (!isSiteExistResult.Item1) //一级都不存在
                {
                    return $"website: ${level1} not found";
                }

                if (!isSiteExistResult.Item2) //一级都不存在
                {
                    return $"website:${level2} not found in ${level1}";
                }

                var projectLocation = IISHelper.GetWebSiteLocationInIIS(level1, level2, Log);
                if (projectLocation == null)
                {
                    return $"website:${_webSiteName} location not found";
                }

                if (string.IsNullOrEmpty(projectLocation.Item1))
                {
                    return $"website : {_webSiteName} not found in iis";
                }

                if (!Directory.Exists(projectLocation.Item1))
                {
                    return $"website:${_webSiteName} location not found";
                }

                Log("Start to rollback IIS:");
                Log("SiteName ===>" + _webSiteName);
                Log("SiteFolder ===> " + projectLocation.Item1);
                Log("SiteApplicationPoolName ===> " + projectLocation.Item3);

                if (isUseTempPhysicalPath)
                {
                    var err = IISHelper.ChangePhysicalPath(level1, level2, deployFolder);
                    if (string.IsNullOrEmpty(err))
                    {
                        Log("Change Site PhysicalPath Success:" + (level1 + "/" + level2));
                        Log("NewSiteFolder ===> " + deployFolder);
                        SaveCurrentVersion(new DirectoryInfo(deployFolder).Parent.FullName);
                    }
                    else
                    {
                        Log("Change Site PhysicalPath Fail:" + (level1 + "/" + level2));
                    }

                    return err;
                }

                Arguments args = new Arguments
                {
                    DeployType = "IIS",
                    BackupFolder = Setting.BackUpIIsPathFolder,
                    AppName = _projectName,
                    ApplicationPoolName = projectLocation.Item3,
                    AppFolder = projectLocation.Item1,
                    DeployFolder = deployFolder,
                    SiteName = projectLocation.Item2,
                    NoBackup = true
                };


                var ops = new OperationsIIS(args, Log);

                try
                {
                    ops.Execute();
                    SaveCurrentVersion(new DirectoryInfo(deployFolder).Parent.FullName);
                    Log("Rollback IIS Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        //ops.Rollback();

                        return $"Rollback to iis err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"Rollback to iis err:{ex.Message},fail:{ex2.Message}";
                    }
                }

                return string.Empty;
            }
            catch (Exception exx)
            {
                return exx.Message;
            }
            finally
            {
                cleanRollbackTemp();
            }
        }

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            return null;
        }




        public override string CheckData(FormHandler formHandler)
        {
            _formHandler = formHandler;


            var website = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("webSiteName"));
            if (website == null || string.IsNullOrEmpty(website.TextValue))
            {
                return "webSiteName required";
            }

            if (website.TextValue.Length > 100)
            {
                return "webSiteName is too long";
            }

            _webSiteName = website.TextValue.Trim();
            var siteNameArr = _webSiteName.Split('/');
            if (siteNameArr.Length > 2)
            {
                return "webSiteName level limit is 2";
            }


            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }
            else
            {
                return "rollback version is required";
            }

            _projectName = IISHelper.GetCorrectFolderName(_webSiteName);
            return string.Empty;
        }


    }
}
