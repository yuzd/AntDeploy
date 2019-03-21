using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;

namespace AntDeploy.Util
{

    /// <summary>
    /// 执行command命令的Helper
    /// </summary>
    public class CommandHelper
    {

        /// <summary>
        /// 启用msbuild
        /// </summary>
        /// <param name="path"></param>
        /// <param name="publishPath"></param>
        /// <param name="logger"></param>
        /// <param name="isWeb"></param>
        /// <returns></returns>
        public static bool RunMsbuild(string path, string publishPath, NLog.Logger logger, bool isWeb = false)
        {
            var msBuild = GetMsBuildPath();
            if (string.IsNullOrEmpty(msBuild))
            {
                return false;
            }
            var path2 = publishPath.Replace("\\\\", "\\");
            if (path2.EndsWith("\\"))
            {
                path2 = path2.Substring(0, path2.Length - 1);
            }
            var msbuildPath = msBuild + "\\MsBuild.exe";
            var buildArg = "\"" + path.Replace("\\\\", "\\") + "\"";
            if (isWeb)
            {
                buildArg += " /verbosity:minimal /p:Configuration=Release /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=True /p:publishUrl=\"" + path2 + "\"";
            }
            else
            {
                buildArg += " /t:Rebuild /v:m /p:Configuration=Release;OutDir=\"" + path2 + "\"";

            }
            return RunDotnetExternalExe(string.Empty, msbuildPath, buildArg, logger);
        }



        /// <summary>
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="arguments"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool RunDotnetExternalExe(string projectPath, string fileName, string arguments, NLog.Logger logger)
        {
            Process process = null;
            try
            {
                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                //执行dotnet命令如果 projectdir路径含有空格 或者 outDir 路径含有空格 都是没有问题的

                process = new Process();

                process.StartInfo.WorkingDirectory = projectPath;
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Verb = "runas";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;



                process.Start();


                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        if (!args.Data.StartsWith(" ") && args.Data.Contains(": error"))
                        {
                            logger?.Warn(args.Data);
                        }
                        else if (args.Data.Contains(".csproj : error"))
                        {
                            logger?.Error(args.Data);
                        }
                        else
                        {
                            logger?.Info(args.Data);
                        }
                    }
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Data)) logger?.Error(data.Data);
                };
                process.BeginErrorReadLine();

                process.WaitForExit();

                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    //ignore
                }
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
                return false;
            }
            finally
            {
                process?.Dispose();
            }
        }


        /// <summary>
        /// 获取Msbuild的路径
        /// </summary>
        /// <returns></returns>
        private static string GetMsBuildPath()
        {
            var getmS = ToolLocationHelper.GetPathToBuildTools(ToolLocationHelper.CurrentToolsVersion);
            return getmS;
        }

    }
}
