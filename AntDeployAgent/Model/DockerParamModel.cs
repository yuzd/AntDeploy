using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AntDeployAgent.Model
{
    internal class DockerParamModel
    {
        public bool DockerServiceEnableUpload { get; set; }
        public bool DockerServiceBuildImageOnly { get; set; }
        public string RepositoryUrl { get; set; }
        public string RepositoryUserName { get; set; }
        public string RepositoryUserPwd { get; set; }
        public string RepositoryNameSpace { get; set; }
        public string RepositoryImageName { get; set; }
        public string ProjectDeployRoot { get; set; }
        public bool RollBack { get; set; }
        public string Volume { get; set; }
        public string Other { get; set; }
        public bool UseAsiaShanghai { get; set; }

        public string Sudo { get; set; }

        public string NetCorePort { get; set; }
        public string NetCoreENTRYPOINT { get; set; }
        public string NetCoreEnvironment { get; set; }
        public string NetCoreVersion { get; set; }
        public string RealPort { get; set; }
        public string RealServerPort { get; set; }

        public string PorjectName { get; set; }
        /// <summary>
        /// 宿主机开启的端口
        /// </summary>
        public string ServerPort
        {
            get
            {
                if (string.IsNullOrEmpty(this.NetCorePort)) return "5000";

                if (this.NetCorePort.Contains(":"))
                {
                    return this.NetCorePort.Split(':')[0];
                }
                else
                {
                    return NetCorePort;
                }
            }
        }

        /// <summary>
        /// 容器暴露的端口
        /// </summary>
        public string ContainerPort
        {
            get
            {
                if (string.IsNullOrEmpty(this.NetCorePort)) return "5000";

                if (this.NetCorePort.Contains(":"))
                {
                    return this.NetCorePort.Split(':')[1];
                }
                else
                {
                    return NetCorePort;
                }
            }
        }

        public  string GetSafeProjectName()
        {
            var projectName = PorjectName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                projectName = projectName.Replace(System.Char.ToString(c), "");
            }
            var aa = Regex.Replace(projectName, "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
            aa = aa.Replace(" ", "").Replace("　", "");
            aa = Regex.Replace(aa, @"[~!@#\$%\^&\*\(\)\+=\|\\\}\]\{\[:;<,>\?\/""]+", "");
            return aa.ToLower();
        }
    }
}
