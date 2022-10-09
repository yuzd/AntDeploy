﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using System.Runtime.InteropServices;
namespace AntDeployAgent.MyApp.Service.Impl
{
    public class IIsPublisher : PublishProviderBasicAPI
    {
        private string _webSiteName;
        private string _sdkTypeName;
        private string _projectName;
        private string _port;
        private string _poolName;
        private string _dateTimeFolderName;
        private List<string> _backUpIgnoreList = new List<string>();

        private string _projectPublishFolder;
        private bool _isIncrement; //是否增量
        private bool _isNoStopWebSite; //是否需要停止website
        private string _physicalPath; //指定的创建的时候用的服务器路径
        private bool _useOfflineHtm = false; //指定用offline.htm
        private bool _useTempPhysicalPath = false; //是否采用新的模式
        private bool _poolAlwaysRunning = false; //新建site的时候支持让Pool一直running的配置

        public override string ProviderName => "iis";
        public override string ProjectName => _projectName;
        public override string ProjectPublishFolder => _projectPublishFolder;

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishIIsPathFolder, _projectName);
            _projectPublishFolder = Path.Combine(projectPath,
                !string.IsNullOrEmpty(_dateTimeFolderName) ? _dateTimeFolderName : DateTime.Now.ToString("yyyyMMddHHmmss"));
            EnsureProjectFolder(projectPath);
            EnsureProjectFolder(_projectPublishFolder);
            var deployFolder = string.Empty;
            try
            {
                var isNetcore = _sdkTypeName.ToLower().Equals("netcore");
                var _zipFile = Path.Combine(_projectPublishFolder, fileItem.FileName);
                using (var fs = new FileStream(_zipFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileItem.FileBody, 0, fileItem.FileBody.Length);
                }



                if (!File.Exists(_zipFile))
                {
                    return "publish file save fail";
                }
#if NETCORE
                Log("netcore agent version ==>" + Version.VERSION);
#else
                Log("netframework agent version ==>" + Version.VERSION);
#endif


                Log("upload success ==>" + _zipFile);
                //解压
                try
                {
                    Log("start unzip file");
                    ZipFile.ExtractToDirectory(_zipFile, _projectPublishFolder);
                }
                catch (Exception ex)
                {
                    return "unzip publish file error:" + ex.Message;
                }

                Log("unzip success ==>" + _projectPublishFolder);

                deployFolder = findUploadFolder(_projectPublishFolder, true);

                if (!Directory.Exists(deployFolder))
                {
                    return "unzip publish file error,Path not found:" + deployFolder;
                }

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

                var tempPhysicalPath = string.Empty;
                if (_useTempPhysicalPath)
                {
                    tempPhysicalPath = Path.Combine(_projectPublishFolder, "_deploy_");
                }

                var iisVersion = IISHelper.GetIISVersion();
                if (iisVersion > 0) Log($"IIS_Version : {iisVersion} ");
                if (!isSiteExistResult.Item1) //一级都不存在
                {
                    if (iisVersion <= 6)
                    {
                        return $"website : {_webSiteName} not found,start to create,but iis verison is too low!";
                    }

                    //准备好目录
                    if (string.IsNullOrEmpty(_port))
                    {
                        if (IISHelper.IsDefaultWebSite(level1))
                        {
                            _port = "80";
                        }
                        else
                        {
                            return $"website : {_webSiteName} not found,start to create,but port is required!";
                        }
                    }

                    Log($"website : {_webSiteName} not found,start to create!");

                    //创建发布目录
                    var firstDeployFolder = string.IsNullOrEmpty(_physicalPath) ? Path.Combine(projectPath, "deploy") : _physicalPath;
                    EnsureProjectFolder(firstDeployFolder);
                    if (Directory.Exists(firstDeployFolder))
                    {
                        Log($"deploy folder create success : {firstDeployFolder} ");
                    }
                    else
                    {
                        return $"DeployFolder : {firstDeployFolder} create error!";
                    }


                    var rt = IISHelper.InstallSite(level1, firstDeployFolder, _port, (string.IsNullOrEmpty(_poolName) ? _projectName : _poolName), isNetcore,
                        _poolAlwaysRunning);
                    if (string.IsNullOrEmpty(rt))
                    {
                        Log($"create website : {level1} success ");
                    }
                    else
                    {
                        return $"create website : {level1} error: {rt} ";
                    }

                    if (!string.IsNullOrEmpty(level2))
                    {
                        //创建一级 但是一级需要一个空的目录
                        //创建二级虚拟目录 二级的目录才是正常程序所在目录
                        var level2Folder = Path.Combine(firstDeployFolder, level2);
                        EnsureProjectFolder(level2Folder);

                        var rt2 = IISHelper.InstallVirtualSite(level1, level2, level2Folder, (string.IsNullOrEmpty(_poolName) ? _projectName : _poolName),
                            isNetcore, _poolAlwaysRunning);
                        if (string.IsNullOrEmpty(rt2))
                        {
                            Log($"create virtualSite :{level2} Of Website : {level1} success ");
                        }
                        else
                        {
                            return $"create virtualSite :{level2} website : {level1} error: {rt2} ";
                        }

                        //复制文件到发布目录
                        CopyHelper.ProcessXcopy(deployFolder, level2Folder, Log);
                        if (_useTempPhysicalPath)
                        {
                            EnsureProjectFolder(tempPhysicalPath);
                            CopyHelper.ProcessXcopy(deployFolder, tempPhysicalPath, Log);
                        }

                        Log($"copy files success from [{deployFolder}] to [{level2Folder}]");
                        return String.Empty;
                    }
                    else
                    {
                        //只需要一级 就是程序所在目录
                        //复制文件到发布目录
                        CopyHelper.ProcessXcopy(deployFolder, firstDeployFolder, Log);
                        if (_useTempPhysicalPath)
                        {
                            EnsureProjectFolder(tempPhysicalPath);
                            CopyHelper.ProcessXcopy(deployFolder, tempPhysicalPath, Log);
                        }

                        Log($"copy files success from [{deployFolder}] to [{firstDeployFolder}]");
                        return String.Empty;
                    }
                }
                else if (isSiteExistResult.Item1 && !isSiteExistResult.Item2 && !string.IsNullOrEmpty(level2))
                {
                    if (IISHelper.GetIISVersion() <= 6)
                    {
                        return $"website : {_webSiteName} not found,start to create,but iis verison is too low!";
                    }

                    //有一级 但是需要创建二级

                    Log($"website : {_webSiteName} not found,start to create!");
                    //创建发布目录
                    var firstDeployFolder = string.IsNullOrEmpty(_physicalPath) ? Path.Combine(projectPath, "deploy") : _physicalPath;
                    EnsureProjectFolder(firstDeployFolder);
                    Log($"deploy folder create success : {firstDeployFolder} ");

                    var level2Folder = Path.Combine(firstDeployFolder, level2);
                    EnsureProjectFolder(level2Folder);

                    var rt2 = IISHelper.InstallVirtualSite(level1, level2, level2Folder, (string.IsNullOrEmpty(_poolName) ? _projectName : _poolName),
                        isNetcore, _poolAlwaysRunning);
                    if (string.IsNullOrEmpty(rt2))
                    {
                        Log($"create virtualSite :{level2} Of Website : {level1} success ");
                    }
                    else
                    {
                        return $"create virtualSite :{level2} website : {level1} error: {rt2} ";
                    }

                    //复制文件到发布目录
                    CopyHelper.ProcessXcopy(deployFolder, level2Folder, Log);

                    if (_useTempPhysicalPath)
                    {
                        EnsureProjectFolder(tempPhysicalPath);
                        CopyHelper.ProcessXcopy(deployFolder, tempPhysicalPath, Log);
                    }

                    Log($"copy files success from [{deployFolder}] to [{level2Folder}]");
                    return String.Empty;
                }

                //下面的逻辑就是网站已经存在了 所以就有冲突的风险


                var projectLocation = IISHelper.GetWebSiteLocationInIIS(level1, level2, Log);
                if (projectLocation == null)
                {
                    return $"read info from iis error";
                }

                if (string.IsNullOrEmpty(projectLocation.Item1))
                {
                    return $"website : {_webSiteName} not found in iis";
                }

                if (!Directory.Exists(projectLocation.Item1))
                {
                    //如果目录不存在 那么就重新建立
                    EnsureProjectFolder(projectLocation.Item1);
                }

                Log("Start to deploy IIS:");
                Log("SiteName ===>" + _webSiteName + $"[{projectLocation.Item2}]");
                Log("SiteFolder ===> " + projectLocation.Item1);
                Log("SiteApplicationPoolName ===> " + projectLocation.Item3);

                Arguments args = new Arguments
                {
                    DeployType = "IIS",
                    BackupFolder = Setting.BackUpIIsPathFolder,
                    AppName = _projectName,
                    ApplicationPoolName = projectLocation.Item3,
                    AppFolder = projectLocation.Item1,
                    DeployFolder = deployFolder,
                    SiteName = projectLocation.Item2,
                    BackUpIgnoreList = this._backUpIgnoreList,
                    NoBackup = !Setting.NeedBackUp
                };

                //是否采用 每次都更换新的物理路径的方式
                if (_useTempPhysicalPath)
                {
                    args.NoStop = true;
                    args.NoStart = true;
                    args.UseTempPhysicalPath = true;
                    args.Site1 = level1;
                    args.Site2 = level2;
                    args.TempPhysicalPath = Path.Combine(_projectPublishFolder, "_deploy_");
                    EnsureProjectFolder(args.TempPhysicalPath);
                }
                else if (_useOfflineHtm)
                {
                    args.NoStop = true;
                    args.NoStart = true;
                    args.UseOfflineHtm = true;
                }
                else if (_isNoStopWebSite)
                {
                    args.NoStop = true;
                    args.NoStart = true;
                }

                var ops = new OperationsIIS(args, Log);

                try
                {
                    ops.Execute();
                    try
                    {
                        //如果是增量的话 要把复制过来
                        if (!args.UseTempPhysicalPath && _isIncrement)
                        {
                            Log("Increment deploy start to backup...");
                            //projectLocation.Item1 转到 increment 的目录
                            var incrementFolder = Path.Combine(_projectPublishFolder, "increment");
                            EnsureProjectFolder(incrementFolder);
                            DirectoryInfo directoryInfo = new DirectoryInfo(projectLocation.Item1);
                            string fullName = directoryInfo.FullName;
                            if (directoryInfo.Parent != null)
                                fullName = directoryInfo.Parent.FullName;
                            new Task(() =>
                            {
                                CopyHelper.DirectoryCopy(projectLocation.Item1, incrementFolder, true, fullName, directoryInfo.Name,
                                    this._backUpIgnoreList);
                            }).Start();

                            Log("Increment deploy backup success...");
                        }
                    }
                    catch (Exception ex3)
                    {
                        Log("Increment deploy folder backup fail:" + ex3.Message);
                    }

                    Log("Deploy IIS Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        //ops.Rollback();

                        return $"publish to iis err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"publish to iis err:{ex.Message},rollback fail:{ex2.Message}";
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(deployFolder) && Directory.Exists(deployFolder))
                {
                    new Task(() =>
                    {
                        try
                        {
                            Directory.Delete(deployFolder, true);
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }).Start();
                }
            }
        }


        public override string CheckData(FormHandler formHandler)
        {
            var sdkType = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("sdkType"));
            if (sdkType == null || string.IsNullOrEmpty(sdkType.TextValue))
            {
                return "sdkType required";
            }

            var sdkTypeValue = sdkType.TextValue.ToLower();

            if (!new string[] { "netframework", "netcore" }.Contains(sdkTypeValue))
            {
                return $"sdkType value :{sdkTypeValue} is not suppored";
            }

            _sdkTypeName = sdkTypeValue;

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

            var portItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("port"));
            if (portItem != null && !string.IsNullOrEmpty(portItem.TextValue))
            {
                _port = portItem.TextValue;
            }

            var poolNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("poolName"));
            if (poolNameItem != null && !string.IsNullOrEmpty(poolNameItem.TextValue))
            {
                _poolName = poolNameItem.TextValue;
            }

            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }

            var isIncrement = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isIncrement"));
            if (isIncrement != null && !string.IsNullOrEmpty(isIncrement.TextValue) && isIncrement.TextValue.ToLower().Equals("true"))
            {
                _isIncrement = true;
            }

            var useOfflineHtm = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("useOfflineHtm"));
            if (useOfflineHtm != null && !string.IsNullOrEmpty(useOfflineHtm.TextValue) && useOfflineHtm.TextValue.ToLower().Equals("true"))
            {
                _useOfflineHtm = true;
            }

            var poolAlwaysRunning = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("poolAlwaysRunning"));
            if (poolAlwaysRunning != null && !string.IsNullOrEmpty(poolAlwaysRunning.TextValue) && poolAlwaysRunning.TextValue.ToLower().Equals("true"))
            {
                _poolAlwaysRunning = true;
            }

            var useTempPhysicalPath = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("useTempPhysicalPath"));
            if (useTempPhysicalPath != null && !string.IsNullOrEmpty(useTempPhysicalPath.TextValue) && useTempPhysicalPath.TextValue.ToLower().Equals("true"))
            {
                _useTempPhysicalPath = true;
            }

            var isNoStopWebSite = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isNoStopWebSite"));
            if (isNoStopWebSite != null && !string.IsNullOrEmpty(isNoStopWebSite.TextValue) && isNoStopWebSite.TextValue.ToLower().Equals("true"))
            {
                _isNoStopWebSite = true;
            }

            var physicalPath = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("physicalPath"));
            if (physicalPath != null && !string.IsNullOrEmpty(physicalPath.TextValue))
            {
                _physicalPath = physicalPath.TextValue;
            }

            var backUpIgnoreList = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("backUpIgnore"));
            if (backUpIgnoreList != null && !string.IsNullOrEmpty(backUpIgnoreList.TextValue))
            {
                this._backUpIgnoreList = backUpIgnoreList.TextValue.Split(new string[] { "@_@" }, StringSplitOptions.None).ToList();
            }

            _projectName = IISHelper.GetCorrectFolderName(_webSiteName);
            return string.Empty;
        }
    }
}