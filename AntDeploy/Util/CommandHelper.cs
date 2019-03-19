using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Utilities;

namespace AntDeploy.Util
{

    /// <summary>
    /// 执行command命令的Helper
    /// </summary>
    public class CommandHelper
    {
        

        public static bool RunMsbuild(string path,string publishPath,NLog.Logger logger)
        {
            var msBuild = GetMsBuildPath();
            if (string.IsNullOrEmpty(msBuild))
            {
                return false;
            }
            var path2 = publishPath.Replace("\\\\","\\");
            if (path2.EndsWith("\\"))
            {
                path2 =path2.Substring(0,path2.Length-1);
            }
            return RunDotnetExternalExe(string.Empty,msBuild+"\\MsBuild.exe",
                "\""+path.Replace("\\\\","\\") + "\" /t:Build /v:m /p:Configuration=Release;OutDir=\""+ path2 + "\"",
                 logger);
        }


        public static string GetMsBuildPath()
        {
            var getmS = ToolLocationHelper.GetPathToBuildTools(ToolLocationHelper.CurrentToolsVersion);
            return getmS;


            //try
            //{
               
                

              
           
            //    //var parms = new BuildParameters
            //    //{
            //    //    DetailedSummary = true,
                    
            //    //};

            //    //var projectInstance = new ProjectInstance(path);
            //    //projectInstance.SetProperty("Configuration", "Release");
            //    //projectInstance.SetProperty("Platform", "Any CPU");
            //    //var request = new BuildRequestData(projectInstance,  new string[] { "Rebuild" });
                
            //    //parms.Loggers = new List<Microsoft.Build.Framework.ILogger>
            //    //{
            //    //    new ConsoleLogger(LoggerVerbosity.Normal,
            //    //        message => { log(message); }, color => { }, () => { })
            //    //    {
            //    //        ShowSummary = true
            //    //    }
            //    //};

            //    //var result = BuildManager.DefaultBuildManager.Build(parms, request);
            //    //if (result.OverallResult == BuildResultCode.Success)
            //    //{
            //    //    return string.Empty;
            //    //}

            //    return getmS;
            //}
            //catch (Exception e)
            //{
            //    return e.Message;
            //}
        }


        /// <summary>
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="arguments"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool RunDotnetExternalExe(string projectPath,string fileName,string arguments, NLog.Logger logger)
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
                        if (!args.Data.StartsWith(" ")&&args.Data.Contains(": error"))
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

        
    }
}
