using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Operation
{
    class Arguments
    {
        public string DeployType { get;  set; }
        public string BackupFolder { get;  set; }
        public string AppName { get;  set; }
        public int WaitForWindowsServiceStopTimeOut { get;  set; }
        public string AppFolder { get;  set; }
        public string DeployFolder { get;  set; }
        public string CategoryName { get;  set; }
        public string FireDaemonPath { get;  set; }
        public bool Restore { get; set; }
        public string RestorePath { get; set; }
        public string ApplicationPoolName { get;  set; }
        public string SiteName { get;  set; }
        public bool NoBackup { get;  set; }
        public bool NoStop { get;  set; }
        public bool NoStart { get;  set; }
        public bool UseOfflineHtm { get;  set; }

        /// <summary>
        /// 是否采用AntDeploy的Agent的工作目录的方式，彻底解决资源占用的问题
        /// 日期文件夹下创建 deploy
        /// 从iis现有的路径中先把文件给复制过来
        /// 然后进行替换覆盖
        /// 然后设置pool 和 重新设置新的虚拟路径
        /// pool进行回收
        /// </summary>
        public bool UseTempPhysicalPath { get;  set; }
        public string TempPhysicalPath { get;  set; }
        public string Site1 { get;  set; }
        public string Site2 { get;  set; }
        public object Extention { get;  set; }

        public List<string> BackUpIgnoreList { get;  set; }
      

    }
}
