using System;
using System.Collections.Generic;
using System.Text;

namespace AntDeployCommand.Model
{
    public class Arguments
    {
        /// <summary>
        /// IISDEPLOY IISROLL DOCKERDEPLOY DOCKERROLL SERVICEDEPLOY SERVICEROLL BUILD PACKAGE GETINCRE
        /// </summary>
        public string DeployType { get;  set; }

        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath { get;  set; }

        /// <summary>
        /// 编译类型
        /// </summary>
        public string BuildMode { get; set; }
        public string EnvName { get; set; }
        public string EnvType { get; set; }

        /// <summary>
        /// 打包目录
        /// </summary>
        public string PackagePath { get;  set; }

        /// <summary>
        /// 打包排除规则
        /// </summary>
        public List<string> PackageIgnore { get; set; }


        /// <summary>
        /// 打包成果物
        /// </summary>
        public string PackageZipPath { get; set; }

        /// <summary>
        /// 远程服务器
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// windows服务器发布Token
        /// </summary>
        public string Token { get; set; }
        public string WebSiteName { get; set; }
        public string ServiceName { get; set; }
        public bool IsSelectedDeploy { get; set; }
        public List<string> SelectedFileList { get; set; }
        public bool IsIncrementDeploy { get; set; }
        /// <summary>
        /// linux服务器账号
        /// </summary>
        public string Root { get; set; }
        /// <summary>
        /// linux服务器密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// Docker发布
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Docker发布
        /// </summary>
        public string AspEnv { get; set; }

        /// <summary>
        /// Docker发布
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// Docker发布
        /// </summary>
        public int RemoveDays { get; set; }

        /// <summary>
        /// 回滚版本号
        /// </summary>
        public string RollBackVersion { get; set; }


      
    }
}
