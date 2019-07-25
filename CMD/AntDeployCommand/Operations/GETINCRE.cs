using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;
using LibGit2Sharp;

namespace AntDeployCommand.Operations
{
    public class GETINCRE : OperationsBase
    {
        public override string ValidateArgument()
        {
            //这个就是发布成果物的目录
            if (string.IsNullOrEmpty(Arguments.ProjectPath))
            {
                return $"{Name}{nameof(Arguments.ProjectPath)} required!";
            }

            if (string.IsNullOrEmpty(Arguments.EnvType))
            {
                return $"{Name}{nameof(Arguments.EnvType)} required!";
            }

            if (string.IsNullOrEmpty(Arguments.PackageZipPath))
            {
                return $"{Name}{nameof(Arguments.PackageZipPath)} required!";
            }

            return string.Empty;
        }

        public override string Name => "【Git】";

        public override async Task Run()
        {
            //判断是否已经创建了git
            if (Arguments.EnvType.Equals("get"))
            {
                GetIncrmentFileList();
            }
            else if (Arguments.EnvType.Equals("commit"))
            {
                CommitIncrmentFileList();
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 提交
        /// </summary>
        private void CommitIncrmentFileList()
        {
            var lines = File.ReadAllLines(Arguments.PackageZipPath);
            if (lines.Length < 1)
            {
                Log("can not commit,selected fileList count = 0", LogLevel.Error);
                return;
            }

            using (var gitModel = new GitClient(Arguments.ProjectPath, Log))
            {
                if (Arguments.IsSelectedDeploy)
                {
                    gitModel.SubmitSelectedChanges(lines.ToList(), Arguments.ProjectPath);
                }
                else
                {
                    gitModel.SubmitChanges(lines.Length);
                }
            }
        }

        /// <summary>
        /// 获取git增量
        /// </summary>
        private void GetIncrmentFileList()
        {
            using (var gitModel = new GitClient(Arguments.ProjectPath, Log))
            {
                if (!gitModel.InitSuccess)
                {
                    Log("can not init git,please cancel Increment Deploy", LogLevel.Error);
                    return;
                }

                var fileList = gitModel.GetChanges();
                if (fileList == null || fileList.Count < 1)
                {
                    return;
                }

                Log("Increment package file count:" + fileList.Count, LogLevel.Info);

                File.WriteAllLines(Arguments.PackageZipPath, fileList.ToArray(), Encoding.UTF8);
            }
        }


    }
}