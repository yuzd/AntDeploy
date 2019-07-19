using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using AntDeployCommand.Utils;

namespace AntDeployCommand.Operations
{
    public class PACKAGE : OperationsBase
    {
        private string zipPath = string.Empty;
        public override string ValidateArgument()
        {
            if (string.IsNullOrEmpty(Arguments.PackagePath))
            {
                return $"{nameof(Arguments.PackagePath)} required!";
            }

            if (!Directory.Exists(Arguments.PackagePath))
            {
                return $"{nameof(Arguments.PackagePath)} not found!";
            }


            if (string.IsNullOrEmpty(Arguments.EnvType))
            {
                return $"{nameof(Arguments.EnvType)} required!";
            }

            if (!Arguments.EnvName.Equals("DOCKER"))
            {
                zipPath = Path.Combine(Arguments.PackagePath, "package.zip");
            }
            else
            {
                zipPath = Path.Combine(Arguments.PackagePath, "package.tar");
            }

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            return string.Empty;
        }

        public override void Run()
        {
            //打包分好几种情况
            //1 全部打包
            //2 指定打包文件列表

            LogHelper.Info($"package start: {Arguments.PackagePath}");
            LogHelper.Info($"package ignoreList Count:{Arguments.PackageIgnore.Count}");
            byte[] zipBytes = null;

            try
            {
                if (Arguments.EnvName.Equals("DOCKER"))
                {
                    if (Arguments.SelectedFileList.Count < 1)
                    {
                        using (var memeory = ZipHelper.DoCreateTarFromDirectory(Arguments.PackagePath,
                            Arguments.PackageIgnore,
                            (progressValue) =>
                            {
                                LogHelper.Info($"【package progress】 {progressValue} %");
                                return false;
                            }, LogHelper.Info))
                        {

                            zipBytes = memeory.GetBuffer();
                        }
                    }
                    else
                    {
                        LogHelper.Info($"selected fileList Count:{Arguments.SelectedFileList.Count}");
                        using (var memeory = ZipHelper.DoCreateTarFromDirectory(Arguments.PackagePath, Arguments.SelectedFileList,
                            Arguments.PackageIgnore,
                            (progressValue) =>
                            {
                                LogHelper.Info($"【package progress】 {progressValue} %");
                                return false;
                            }, LogHelper.Info, Arguments.IsSelectedDeploy))
                        {
                            zipBytes = memeory.GetBuffer();
                        }
                    }
                }
                else
                {
                    if (Arguments.SelectedFileList.Count < 1)
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory(Arguments.PackagePath, CompressionLevel.Optimal,
                            true, Arguments.PackageIgnore,
                            (progressValue) =>
                            {
                                LogHelper.Info($"【package progress】 {progressValue} %");
                                return false;
                            });
                    }
                    else
                    {
                        LogHelper.Info($"selected fileList Count:{Arguments.SelectedFileList.Count}");
                        zipBytes = ZipHelper.DoCreateFromDirectory(Arguments.PackagePath, Arguments.SelectedFileList, CompressionLevel.Optimal,
                            true, Arguments.PackageIgnore,
                            (progressValue) =>
                            {
                                LogHelper.Info($"【package progress】 {progressValue} %");
                                return false;
                            }, Arguments.IsSelectedDeploy);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("package fail:" + ex.Message);
                return;
            }

            if (zipBytes == null || zipBytes.Length < 1)
            {
                LogHelper.Error("package fail");
                return;
            }

            //保存
            File.WriteAllBytes(zipPath, zipBytes);

            var packageSize = (zipBytes.Length / 1024 / 1024);
            LogHelper.Info($"package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
            LogHelper.Info($"【package success】{zipPath}");

        }
    }
}
