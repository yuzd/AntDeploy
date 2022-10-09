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
        
        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }
        
        /// <summary>
        /// 环境类型
        /// </summary>
        public string EnvType { get; set; }

        /// <summary>
        /// 打包目录
        /// </summary>
        public string PackagePath { get;  set; }

        /// <summary>
        /// 打包排除规则
        /// </summary>
        public List<string> PackageIgnore { get; set; } = new List<string>();
        
        /// <summary>
        /// 发布到windows服务器指定agent排除备份的目标
        /// </summary>
        public List<string> BackUpIgnore { get; set; } = new List<string>();

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
        
        /// <summary>
        /// iis发布的网址名称
        /// </summary>
        public string WebSiteName { get; set; }
        
        /// <summary>
        /// 第一次发布iis或者window服务指定的服务器物理路径
        /// </summary>
        public string PhysicalPath { get; set; }
        
        /// <summary>
        /// 发布iis指定程序池名称
        /// </summary>
        public string PoolName { get; set; }
        
        /// <summary>
        /// 发布国外的时候需要用到代理
        /// </summary>
        public string Proxy { get; set; }
        
        /// <summary>
        /// 发布loggerId
        /// </summary>
        public string LoggerId { get; set; }
        
        /// <summary>
        /// 回滚版本号
        /// </summary>
        public string DeployFolderName { get; set; }
        
        /// <summary>
        /// 发布备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// git提交人的email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// 发布windows服务的服务名称
        /// </summary>
        public string ServiceName { get; set; }
        
        /// <summary>
        /// 是否是选择指定文件发布
        /// </summary>
        public bool IsSelectedDeploy { get; set; }
        
        /// <summary>
        /// 选择指定文件的列表
        /// </summary>
        public List<string> SelectedFileList { get; set; } = new List<string>();
        
        /// <summary>
        /// 是否增量发布
        /// </summary>
        public bool IsIncrementDeploy { get; set; }
        
        /// <summary>
        /// 是否使用app_offline.htm发布
        /// </summary>
        public bool UseAppOffineHtm { get; set; }
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
