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
        public List<string> BackUpIgnoreList { get;  set; }
      

    }
}
