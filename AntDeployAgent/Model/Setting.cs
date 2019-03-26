using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AntDeployAgentWindows.Model
{
    public class Setting
    {
        private static readonly System.Threading.Timer mDetectionTimer;
        private static readonly int _clearOldPublishFolderOverDays = 10;

        static Setting()
        {
#if DEBUG
             mDetectionTimer = new System.Threading.Timer(OnVerify, null, 1000 * 5, 1000 * 5);
#else
             mDetectionTimer = new System.Threading.Timer(OnVerify, null, 1000 * 60 * 30, 1000 * 60 * 30);
#endif

            var clearOldPublishFolderOverDaysStr = System.Configuration.ConfigurationManager.AppSettings["ClearOldPublishFolderOverDays"];
            if (int.TryParse(clearOldPublishFolderOverDaysStr, out int value) && value>0)
            {
                _clearOldPublishFolderOverDays = value;
            }
        }


        public static string WebRootPath = "";

        public static string PublishPathFolder = "";

        public static string PublishIIsPathFolder = "";
        public static string BackUpIIsPathFolder = "";


        public static string PublishWindowServicePathFolder = "";
        public static string BackUpWindowServicePathFolder = "";

        public static void InitWebRoot(string rootPath,bool useCustomer = false)
        {

            if (string.IsNullOrEmpty(rootPath))
            {
                return;
            }

            WebRootPath = rootPath;

            PublishPathFolder = (useCustomer)? rootPath: Path.Combine(WebRootPath, "antdeploy");

            if (!Directory.Exists(PublishPathFolder))
            {
                Directory.CreateDirectory(PublishPathFolder);
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
        }


        private static void OnVerify(object state)
        {
            mDetectionTimer.Change(-1, -1);
            try
            {
                CheckOldFolder(PublishIIsPathFolder);
                CheckOldFolder(BackUpIIsPathFolder);
                CheckOldFolder(PublishWindowServicePathFolder);
                CheckOldFolder(BackUpWindowServicePathFolder);

            }
            catch
            {
                // ignored
            }
            finally
            {
#if DEBUG
                mDetectionTimer.Change(1000 * 5, 1000 * 5);
#else
                mDetectionTimer.Change(1000 * 60 * 30, 1000 * 60 * 30);
#endif

            }
        }


        private static void CheckOldFolder(string path)
        {
            try
            {
                var now = DateTime.Now.Date;
                if (string.IsNullOrEmpty(path)) return;

                if (!Directory.Exists(path)) return;

                var applicationFolders = Directory.GetDirectories(path);
                if (applicationFolders.Length < 1) return;

                foreach (var applicationFolder in applicationFolders)
                {
                    var subFolders = Directory.GetDirectories(applicationFolder);
                    if (subFolders.Length < 10) continue;//保留10个
                    var oldFolderList = new List<OldFolder>();
                    foreach (var subFolder in subFolders)
                    {
                        var folder = new DirectoryInfo(subFolder);
                        var folderName = folder.Name;
                        folderName = folderName.Replace("Backup", "").Replace("_", "");
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
                            DiffDays = days
                        });
                    }


                    var targetList = oldFolderList.OrderByDescending(r => r.DateTime)
                        .Where(r => r.DiffDays >= _clearOldPublishFolderOverDays)
                        .ToList();

                    var diff = subFolders.Length - targetList.Count;

                    if (diff >= 0 && diff < 10)
                    {
                        targetList = targetList.Skip(10 - diff).ToList();
                    }

                    foreach (var target in targetList)
                    {
                        try
                        {
                            Directory.Delete(target.FullName, true);
                        }
                        catch
                        {
                            //ignore
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
            mDetectionTimer.Change(-1, -1);
            mDetectionTimer.Dispose();
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
