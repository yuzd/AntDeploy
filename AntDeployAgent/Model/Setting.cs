using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Model
{
    public class Setting
    {
        //private static readonly System.Threading.Timer mDetectionTimer;
        private static readonly int _clearOldPublishFolderOverDays = 10;
        private static readonly int _oldPulishLimit = 10;
        public static readonly int BackUpLimit = 10;
        private static readonly List<string> MacWhiteList = new List<string>();
        /// <summary>
        /// 是否开启备份
        /// </summary>
        /// <returns></returns>
        public static readonly bool NeedBackUp = true;
        static Setting()
        {
            //#if DEBUG
            //             mDetectionTimer = new System.Threading.Timer(OnVerify, null, 1000 * 5, 1000 * 5);
            //#else
            //             mDetectionTimer = new System.Threading.Timer(OnVerify, null, 1000 * 60 * 30, 1000 * 60 * 30);
            //#endif

            var oldPulishLimit = System.Configuration.ConfigurationManager.AppSettings["OldPulishLimit"];
            if (int.TryParse(oldPulishLimit, out int value1))
            {
                _oldPulishLimit = value1 < 1 ? 1 : value1;
            }

            var clearOldPublishFolderOverDaysStr = System.Configuration.ConfigurationManager.AppSettings["ClearOldPublishFolderOverDays"];
            if (int.TryParse(clearOldPublishFolderOverDaysStr, out int value))
            {
                _clearOldPublishFolderOverDays = value;
            }

            var _whiteMacList = System.Configuration.ConfigurationManager.AppSettings["MacWhiteList"];
            if (!string.IsNullOrEmpty(_whiteMacList))
            {
                MacWhiteList = _whiteMacList.Split(',').Distinct().ToList();
            }

            var needBackUp = System.Configuration.ConfigurationManager.AppSettings["NeedBackUp"];
            if (!string.IsNullOrEmpty(needBackUp) && needBackUp.ToLower().Equals("false"))
            {
                NeedBackUp = false;
            }

            var backuplimit = System.Configuration.ConfigurationManager.AppSettings["BackUpLimit"];
            if (!string.IsNullOrEmpty(backuplimit))
            {
                int.TryParse(backuplimit, out BackUpLimit);
            }
            else
            {
                BackUpLimit = _oldPulishLimit;
            }
        }


        public static string WebRootPath = "";

        public static string PublishPathFolder = "";

        public static string PublishIIsPathFolder = "";
        public static string BackUpIIsPathFolder = "";


        public static string PublishLinuxPathFolder = "";
        public static string BackUpLinuxPathFolder = "";

        public static string PublishWindowServicePathFolder = "";
        public static string BackUpWindowServicePathFolder = "";


        public static string PublishDockerPathFolder = "";
        public static string BackUpDockerPathFolder = "";

        public static void InitWebRoot(string rootPath, bool useCustomer = false)
        {

            if (string.IsNullOrEmpty(rootPath))
            {
                return;
            }

            WebRootPath = rootPath;

            PublishPathFolder = (useCustomer) ? rootPath : Path.Combine(WebRootPath, "antdeploy");

            if (!Directory.Exists(PublishPathFolder))
            {
                Directory.CreateDirectory(PublishPathFolder);
            }


            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //linux环境下
                PublishLinuxPathFolder = Path.Combine(PublishPathFolder, "linux");
                if (!Directory.Exists(PublishLinuxPathFolder))
                {
                    Directory.CreateDirectory(PublishLinuxPathFolder);
                }
                BackUpLinuxPathFolder = Path.Combine(PublishPathFolder, "linux_backup");
                if (!Directory.Exists(BackUpLinuxPathFolder))
                {
                    Directory.CreateDirectory(BackUpLinuxPathFolder);
                }


                PublishDockerPathFolder = Path.Combine(PublishPathFolder, "docker");
                if (!Directory.Exists(PublishDockerPathFolder))
                {
                    Directory.CreateDirectory(PublishDockerPathFolder);
                }
                BackUpDockerPathFolder = Path.Combine(PublishPathFolder, "docker_backup");
                if (!Directory.Exists(BackUpDockerPathFolder))
                {
                    Directory.CreateDirectory(BackUpDockerPathFolder);
                }

                return;
            }


            PublishIIsPathFolder = Path.Combine(PublishPathFolder, "iis");

            if (!Directory.Exists(PublishIIsPathFolder))
            {
                Directory.CreateDirectory(PublishIIsPathFolder);
            }

            BackUpIIsPathFolder = Path.Combine(PublishPathFolder, "iis_backup");

            if (!Directory.Exists(BackUpIIsPathFolder))
            {
                Directory.CreateDirectory(BackUpIIsPathFolder);
            }

            PublishWindowServicePathFolder = Path.Combine(PublishPathFolder, "window_service");

            if (!Directory.Exists(PublishWindowServicePathFolder))
            {
                Directory.CreateDirectory(PublishWindowServicePathFolder);
            }

            BackUpWindowServicePathFolder = Path.Combine(PublishPathFolder, "window_service_backup");

            if (!Directory.Exists(BackUpWindowServicePathFolder))
            {
                Directory.CreateDirectory(BackUpWindowServicePathFolder);
            }
            
            // windows 也可能支持docker
            PublishDockerPathFolder = Path.Combine(PublishPathFolder, "docker");
            if (!Directory.Exists(PublishDockerPathFolder))
            {
                Directory.CreateDirectory(PublishDockerPathFolder);
            }
            BackUpDockerPathFolder = Path.Combine(PublishPathFolder, "docker_backup");
            if (!Directory.Exists(BackUpDockerPathFolder))
            {
                Directory.CreateDirectory(BackUpDockerPathFolder);
            }
        }

        public static void ClearOldFolders(string type, string projectFolderName, Action<string> logger = null)
        {
            logger?.Invoke($"start check old published folder :{projectFolderName}");
            if (type == "linux")
            {
                CheckOldFolder(PublishLinuxPathFolder, projectFolderName, logger);
                CheckOldFolder(BackUpLinuxPathFolder, projectFolderName, logger);
       
            }
            else if (type == "docker")
            {
                CheckOldFolder(PublishDockerPathFolder, projectFolderName, logger);
                CheckOldFolder(BackUpDockerPathFolder, projectFolderName, logger);
            }
            else if (type == "iis")
            {
                CheckOldFolder(PublishIIsPathFolder, projectFolderName, logger);
                CheckOldFolder(BackUpIIsPathFolder, projectFolderName, logger);
            }
            else
            {

                CheckOldFolder(PublishWindowServicePathFolder, projectFolderName);
                CheckOldFolder(BackUpWindowServicePathFolder, projectFolderName);
            }
        }

        //        private static void OnVerify(object state)
        //        {
        //            mDetectionTimer.Change(-1, -1);
        //            try
        //            {
        //                ClearOldFolders();
        //            }
        //            catch
        //            {
        //                // ignored
        //            }
        //            finally
        //            {
        //#if DEBUG
        //                mDetectionTimer.Change(1000 * 5, 1000 * 5);
        //#else
        //                mDetectionTimer.Change(1000 * 60 * 30, 1000 * 60 * 30);
        //#endif

        //            }
        //        }


        /// <summary>
        /// 清除老的发布版本记录 (不会删除正在使用的版本 即使已经到期)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="projectFolder"></param>
        private static void CheckOldFolder(string path, string projectFolder, Action<string> logger = null)
        {
            try
            {
                var now = DateTime.Now.Date;
                if (string.IsNullOrEmpty(path)) return;

                if (!Directory.Exists(path)) return;

                var applicationFolders = !string.IsNullOrEmpty(projectFolder) ? new List<string> { Path.Combine(path, projectFolder) }.ToArray() : Directory.GetDirectories(path);
                if (applicationFolders.Length < 1) return;
                foreach (var applicationFolder in applicationFolders)
                {
                    var subFolders = Directory.GetDirectories(applicationFolder);
                    logger?.Invoke($"found deploy folders:{subFolders.Length}->{applicationFolder}");
                    var limits = (path.EndsWith("_backup") ? BackUpLimit : _oldPulishLimit);
                    if (subFolders.Length < limits)
                    {
                        logger?.Invoke($"config limit:{limits},ignore delete");
                        continue;//还没超过最低的保留记录数
                    }
                    //找到current.txt文件 记录着当前正在使用的版本
                    var currentText = Path.Combine(applicationFolder, "current.txt");
                    var currentVersion = "";
                    if (File.Exists(currentText))
                    {
                        currentVersion = File.ReadAllText(currentText);
                    }
                    //超过了就要比对文件夹的日期 把超过了x天数的就要删掉
                    var oldFolderList = new List<OldFolder>();
                    foreach (var subFolder in subFolders)
                    {
                        var folder = new DirectoryInfo(subFolder);
                        var folderName = folder.Name;
                        folderName = folderName.Replace("Backup", "").Replace("Err", "").Replace("_", "");
                        if (!DateTime.TryParseExact(folderName, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime createDate))
                        {
                            continue;
                        }

                        var days = (now - createDate.Date).TotalDays;

                        oldFolderList.Add(new OldFolder
                        {
                            Name = folder.Name,
                            FullName = subFolder,
                            DateTime = createDate,
                            DiffDays = days//这个是当前日期和文件夹日期(也就是发布日期)进行相比的天数
                        });
                    }


                    var targetList = oldFolderList.OrderByDescending(r => r.DateTime)
                        .Where(r => r.DiffDays >= _clearOldPublishFolderOverDays)
                        .ToList();
                    
                    logger?.Invoke($"deploy folders count:{oldFolderList.Count}, overDays({_clearOldPublishFolderOverDays}) folders count:{targetList.Count}, limit:{limits}");
                    var diff = subFolders.Length - targetList.Count;
                    var oldLimit = (path.EndsWith("_backup") ? BackUpLimit : _oldPulishLimit);
                    if (diff >= 0 && diff < oldLimit)
                    {
                        targetList = targetList.Skip(oldLimit - diff).ToList();
                    }

                    foreach (var target in targetList)
                    {
                        //如果是当前正在使用的版本的话 不能删除
                        if (!string.IsNullOrEmpty(currentVersion) && target.Name.Equals(currentVersion))
                        {
                            continue;
                        }
                        try
                        {
                            logger?.Invoke($"delete old folder:{target.FullName}");
                            Directory.Delete(target.FullName, true);
                        }
                        catch(Exception e)
                        {
                            //ignore
                            logger?.Invoke($"delete old folder fail:{e.Message}");
                        }
                    }


                }

            }
            catch
            {
                // ignored
            }
        }

        public static void StopWatchFolderTask()
        {
            //mDetectionTimer.Change(-1, -1);
            //mDetectionTimer.Dispose();
        }

        /// <summary>
        /// 检查是否mac地址白名单
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        public static bool CheckIsInWhiteMacList(string macAddress)
        {
            if (!MacWhiteList.Any()) return true;

            return MacWhiteList.Contains(macAddress);
        }

    }

    class OldFolder
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime DateTime { get; set; }
        public double DiffDays { get; set; }
    }
}
