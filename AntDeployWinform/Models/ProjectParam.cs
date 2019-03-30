using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployWinform.Models
{
    [Serializable]
    public class ProjectParam
    {
        /// <summary>
        /// 经过vssdk判断是web项目的
        /// </summary>
        public bool IsWebProejct { get; set; }

        /// <summary>
        ///  经过vssdk判断是netcore项目的
        /// </summary>
        public bool IsNetcorePorject { get; set; }

        /// <summary>
        /// 经过vssdk获取到的outputname
        /// </summary>
        public string OutPutName { get; set; }
        public string VsVersion { get; set; }
        public string MsBuildPath { get; set; }
        public string ProjectPath { get; set; }
        public string NetCoreSDKVersion { get; set; }
    }
}
