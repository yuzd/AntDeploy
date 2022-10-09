using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Utils;

namespace AntDeployCommand.Operations
{
    public class PACKAGE : OperationsBase
    {
        public string PackagePath { get; set; }
        private string zipPath = string.Empty;
        public override string ValidateArgument()
        {
            if (string.IsNullOrEmpty(Arguments.PackagePath))
            {
                return $"【Package】{nameof(Arguments.PackagePath)} required!";
            }

            if (!Directory.Exists(Arguments.PackagePath))
            {
                return $"【Package】{nameof(Arguments.PackagePath)} not found!";
            }


            if (string.IsNullOrEmpty(Arguments.EnvType))
            {
                return $"【Package】{nameof(Arguments.EnvType)} required!";
            }

            zipPath = Path.Combine(Arguments.PackagePath, "package.zip");

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            return string.Empty;
        }

        public override async Task<bool> Run()
        {
            //打包分好几种情况
            //1 全部打包
            //2 指定打包文件列表

            LogHelper.Info($"【Package】start: {Arguments.PackagePath}");
            LogHelper.Info($"【Package】ignoreList Count:{Arguments.PackageIgnore.Count}");
            byte[] zipBytes = null;

            try
            {
                if (Arguments.SelectedFileList.Count < 1)
                {
                    zipBytes = ZipHelper.DoCreateFromDirectory(Arguments.PackagePath, CompressionLevel.Optimal,
                        !Arguments.EnvType.Equals("DOCKER"), Arguments.PackageIgnore,
                        (progressValue) =>
                        {
                            LogHelper.Info($"【Package progress】 {progressValue} %");
                            return false;
                        });
                }
                else
                {
                    LogHelper.Info($"【Package】selected fileList Count:{Arguments.SelectedFileList.Count}");
                    zipBytes = ZipHelper.DoCreateFromDirectory(Arguments.PackagePath, Arguments.SelectedFileList, CompressionLevel.Optimal,
                        !Arguments.EnvType.Equals("DOCKER"), Arguments.IsSelectedDeploy ? null : Arguments.PackageIgnore,
                        (progressValue) =>
                        {
                            LogHelper.Info($"【Package progress】 {progressValue} %");
                            return false;
                        }, Arguments.IsSelectedDeploy);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("【Package】package fail:" + ex.Message);
                return await Task.FromResult(false);
            }

            if (zipBytes == null || zipBytes.Length < 1)
            {
                LogHelper.Error("【Package】package fail");
                return await Task.FromResult(false);
            }

            //保存
            File.WriteAllBytes(zipPath, zipBytes);

            var packageSize = (zipBytes.Length / 1024 / 1024);
            LogHelper.Info($"【Package】size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
            LogHelper.Info($"【Package success】{zipPath}");
            PackagePath = zipPath;
            return await Task.FromResult(true);
        }
    }
}
