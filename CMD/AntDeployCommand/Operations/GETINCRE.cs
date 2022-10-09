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

        public override async Task<bool> Run()
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

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public bool CommitIncrmentFileList(List<string> fileList = null)
        {
            //拿到gitchange所有的文件列表
            var lines = fileList ?? File.ReadAllLines(Arguments.PackageZipPath).ToList();
            if (lines.Count < 1)
            {
                Log("can not commit,selected fileList count = 0", LogLevel.Error);
                return false;
            }
            //先删除
            if (fileList == null)
            {
                File.Delete(Arguments.PackageZipPath);
                var zipPath = Path.Combine(Arguments.ProjectPath, "package.zip");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
            }

            var re = false;
            using (var gitModel = new GitClient(Arguments.ProjectPath, Log))
            {
                if (Arguments.IsSelectedDeploy)
                {
                    re = gitModel.SubmitSelectedChanges(lines, Arguments.ProjectPath);
                }
                else
                {
                    re = gitModel.SubmitChanges(lines.Count);
                }
            }

            return re;
        }

        /// <summary>
        /// 获取git增量
        /// </summary>
        public List<string> GetIncrmentFileList(bool notWrite = false)
        {
            var result = new List<string>();
            using (var gitModel = new GitClient(Arguments.ProjectPath, Log))
            {
                if (!gitModel.InitSuccess)
                {
                    return result;
                }

                var fileList = gitModel.GetChanges();
                if (fileList == null || fileList.Count < 1)
                {
                    Log("Increment package file count: 0" , LogLevel.Warning);
                    return result;
                }

                Log("Increment package file count:" + fileList.Count, LogLevel.Info);

                if (!notWrite)
                {
                    File.WriteAllLines(Arguments.PackageZipPath, fileList.ToArray(), Encoding.UTF8);
                }
                
                return fileList;
            }
        }


    }
}