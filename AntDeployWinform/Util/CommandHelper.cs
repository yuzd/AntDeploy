﻿using Microsoft.Build.Locator;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using AntDeployWinform.Models;
using Newtonsoft.Json;
using NLog;

namespace AntDeployWinform.Util
{

    /// <summary>
    /// 执行command命令的Helper
    /// </summary>
    public class CommandHelper
    {



        public static string MsBuildPath = "";

        /// <summary>
        /// 启用msbuild
        /// </summary>
        /// <param name="path"></param>
        /// <param name="publishPath"></param>
        /// <param name="logger"></param>
        /// <param name="isWeb"></param>
        /// <returns></returns>
        public static bool RunMsbuild(string path, string publishPath, NLog.Logger logger, bool isWeb = false, Func<bool> checkCancel = null)
        {
            var msBuild = MsBuildPath;
            if (string.IsNullOrEmpty(msBuild))
            {
                logger.Error("Fail to get msbuild.exe path.please set it in Config tab page.");
                return false;
            }

            //针对netframework的webpublish尽量保证和vs的自带的publish一致
            if (isWeb)
            {
                //D:\Program Files (x86)\Vs2019\Enterprise\MSBuild\Current\Bin
                //查看当前的msbuild.exe的路径
                //查看是否 包含 vsvars32.bat
                //如果没有 查看是否 包含 VsDevCmd.bat
                var msbuildFolder = new FileInfo(msBuild);
                if (!msbuildFolder.Exists)
                {
                    logger.Error($"msbuild.exe path `{msBuild}` not found. please set it in Config tab page.");
                    goto CONTINUEWEB;
                }

                var vsfolder = msbuildFolder.Directory?.Parent?.Parent?.Parent;
                if (vsfolder == null)
                {
                    logger.Warn($"can not found visual stuidio install path by `{msBuild}` . please use `msbuild` in visual studio installed path");
                    goto CONTINUEWEB;
                }

                var vsfolderToolPath = Path.Combine(vsfolder.FullName, "Common7", "Tools");
                if (!Directory.Exists(vsfolderToolPath))
                {
                    logger.Warn($"can not found visual stuidio tool path  `{vsfolderToolPath}` . please use `msbuild` in visual studio installed path");
                    goto CONTINUEWEB;
                }

                var batPath = Path.Combine(vsfolderToolPath, "vsvars32.bat");
                if (!File.Exists(batPath))
                {
                    batPath = Path.Combine(vsfolderToolPath, "VsDevCmd.bat");
                }

                if (!File.Exists(batPath))
                {
                    logger.Warn($"can not found visual stuidio tool path  `{vsfolderToolPath}` . please use `msbuild` in visual studio installed path");
                    goto CONTINUEWEB;
                }

                var path22 = publishPath.Replace("\\\\", "\\");
                if (path22.EndsWith("\\"))
                {
                    path22 = path22.Substring(0, path22.Length - 1);
                }
                //执行特殊的方式
                var command =
                    $"\"{batPath}\" && msbuild /verbosity:minimal /t:Rebuild /p:Configuration=Release /t:WebPublish /p:WebPublishMethod=FileSystem /p:PublishProvider=FileSystem /p:DeleteExistingFiles=False /p:publishUrl=\"{path22}\" \"{path.Replace("\\\\", "\\")}\"";

                //先清空目录
                ClearPublishFolder(path22);
                logger.Info($"current project Path:{path}");
                logger.Info($"↓↓↓↓　msbuild ↓↓↓↓");
                logger.Info(command);

                return RunCommand(command, logger, checkCancel);
            }

            CONTINUEWEB:




            var path2 = publishPath.Replace("\\\\", "\\");
            if (path2.EndsWith("\\"))
            {
                path2 = path2.Substring(0, path2.Length - 1);
            }
            var msbuildPath = msBuild;
            var buildArg = "\"" + path.Replace("\\\\", "\\") + "\"";
            if (isWeb)
            {
                //如果没有bat就用这个模式
                buildArg += " /t:ResolveReferences /verbosity:minimal /p:BuildingProject=true /p:Configuration=Release /p:DeployOnBuild=true /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:PublishProvider=FileSystem /p:DeleteExistingFiles=False /p:publishUrl=\"" + path2 + "\"";
            }
            else
            {
                buildArg += " /t:Rebuild /v:m /p:Configuration=Release;OutDir=\"" + path2 + "\"";

            }

            //先清空目录
            ClearPublishFolder(path2);
            logger.Info($"current project Path:{path}");
            return RunDotnetExternalExe(string.Empty, msbuildPath, buildArg, logger, checkCancel);
        }

        /// <summary>
        /// 执行dotnet
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="publishPath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool RunDotnetExe(string projectPath, string fileName, string publishPath, string arguments,
            NLog.Logger logger, Func<bool> checkCancel = null)
        {
            if (!string.IsNullOrEmpty(publishPath))
            {

                if (publishPath.EndsWith("\\"))
                {
                    var path2 = publishPath.Substring(0, publishPath.Length - 1);
                    //先清空目录
                    ClearPublishFolder(path2);
                }
                else
                {
                    ClearPublishFolder(publishPath);
                }
                arguments += " -o \"" + publishPath + "\"";
            }
            logger.Info($"current project Path:{projectPath}");
            return RunDotnetExternalExe(string.Empty, $"dotnet", arguments, logger, checkCancel);
        }

        public static bool RunJibExe(string publishPath,string rootPath, DockerImageConfig config2,
            NLog.Logger logger, Func<bool> checkCancel = null)
        {

            var config = DockerImageConfigGo.get(config2);
            //判断当前运行目录有没有jib.exe文件
            var jibExe = Path.Combine(rootPath, "jib.exe");
            if (!File.Exists(jibExe))
            {
                logger.Error($"can not found :{jibExe}");
                return false;
            }

            config.ImageLayersFolder = publishPath;
            var dockerImageCache= Path.Combine(new DirectoryInfo(publishPath).Parent.FullName, "dockerImage_cache");
            config.ApplicationLayersCacheDirectory = dockerImageCache;
            //转成一个json文件
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string projectPramPath = "";
            string tarFolder = "";
            //通过tarimage判断是打包到本地还是publish到远程
            try
            {
                var bash = "";
                if (!config.TargetImage.StartsWith("http://") && ImageReference.Parse(config.TargetImage) == null)
                {
                    var output = config.TargetImage;
                    config.TargetImage = Path.GetFileNameWithoutExtension(config.TargetImage);
                    tarFolder = Path.GetDirectoryName(output);
                    var projectPram = JsonConvert.SerializeObject(config);
                    var md5 = CodingHelper.MD5(projectPram);
                    projectPramPath = Path.Combine(path, md5 + "_param.json");
                    File.WriteAllText(projectPramPath, projectPram, Encoding.UTF8);
                    //打包到本地文件
                    bash = $"tar --outputfile=\"{output}\" --configfile=\"{projectPramPath}\"";
                }
                else
                {
                    var projectPram = JsonConvert.SerializeObject(config);
                    var md5 = CodingHelper.MD5(projectPram);
                    projectPramPath = Path.Combine(path, md5 + "_param.json");
                    File.WriteAllText(projectPramPath, projectPram, Encoding.UTF8);
                    //发布到远程
                    bash = $"push --configfile=\"{projectPramPath}\"";
                }


                return RunDotnetExternalExe(rootPath, $"jib.exe", bash, logger, checkCancel,true);
            }
            finally
            {
                LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "global pull image cache folder  ==> ");
                publisEvent.Properties["ShowLink"] = "file://%LOCALAPPDATA%/fibdotnet";
                publisEvent.LoggerName = "rich_docker_image_log";
                logger.Log(publisEvent);

                LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local build image cache folder  ==> ");
                publisEvent2.Properties["ShowLink"] = "file://" + dockerImageCache.Replace("\\", "\\\\");
                publisEvent2.LoggerName = "rich_docker_image_log";
                logger.Log(publisEvent2);

                if (!string.IsNullOrEmpty(tarFolder))
                {
                    LogEventInfo publisEvent3 =
                        new LogEventInfo(LogLevel.Info, "", "local build image tar folder  ==> ");
                    publisEvent3.Properties["ShowLink"] = "file://" + tarFolder.Replace("\\", "\\\\");
                    publisEvent3.LoggerName = "rich_docker_image_log";
                    logger.Log(publisEvent3);
                }

                try
                {
                    File.Delete(projectPramPath);

                }
                catch (Exception e)
                {
                }
            }

        }
        /// <summary>
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool RunDotnetExternalExe(string projectPath, string fileName, string arguments, NLog.Logger logger, Func<bool> checkCancel = null,bool fullLog = false)
        {
            Process process = null;
            IlogProgress pr = null;
            try
            {
                try
                {
                    
                    logger.Info(fileName + " " + arguments);
                }
                catch (Exception)
                {

                }
                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                if (fullLog)
                {
                    pr = new FullLogProgress(logger);
                }
                else
                {
                    pr = new BuildProgress(logger);
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
                    if (checkCancel != null)
                    {
                        var r = checkCancel();
                        if (r)
                        {

                            try
                            {
                                process?.Dispose();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }

                            try
                            {
                                process?.Kill();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        if (args.Data.StartsWith(" "))//有这个代表肯定build有出问题
                        {
                            logger.Info(args.Data);
                        }
                        if (args.Data.Contains(": warning") || args.Data.Contains("[Warn]"))
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Warn,
                                message = args.Data
                            });
                        }
                        else if (args.Data.Contains(": error") || args.Data.Contains("[Error]"))
                        {
                            if (pr is BuildProgress)
                            {
                                pr.Dispose();
                                pr = new FullLogProgress(logger);//一旦有出错 后面就走全部日志
                            }
                            logger.Error(args.Data);
                        }
                        else
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Info,
                                message = args.Data
                            });
                        }
                    }
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (pr is BuildProgress)
                    {
                        pr.Dispose();
                        pr = new FullLogProgress(logger);//一旦有出错 后面就走全部日志
                    }
                    if (checkCancel != null)
                    {
                        var r = checkCancel();
                        if (r)
                        {

                            try
                            {
                                process?.Dispose();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }

                            try
                            {
                                process?.Kill();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(data.Data))
                    {
                        pr.Log(new BuildEventArgs
                        {
                            level = LogLevel.Error,
                            message = data.Data
                        });
                    }
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
                if (checkCancel != null)
                {
                    var r = checkCancel();
                    if (r)
                    {
                        logger?.Error("deploy task was canceled!");
                        return false;
                    }
                }

                logger?.Error(ex.Message);
                return false;
            }
            finally
            {
                try
                {
                    process?.Kill();
                }
                catch (Exception)
                {
                    //ignore
                }
                try
                {
                    pr?.Dispose();
                }
                catch (Exception)
                {
                    //ignore
                }
                try
                {
                    process?.Dispose();
                }
                catch (Exception)
                {
                    //ignore
                }

            }
        }


        /// <summary>
        /// 发布前清空
        /// </summary>
        /// <param name="srcDir"></param>
        private static void ClearPublishFolder(string srcDir)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcDir);

                if (!dir.Exists) return;

                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                var folderList = dir.GetDirectories();

                foreach (var folder in folderList)
                {
                    if (folder.Name.EndsWith(".git")) continue;
                    try
                    {
                        folder.Delete(true);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 运行bash命令
        /// </summary>
        public static bool RunCommand(string commandToRun, NLog.Logger logger, Func<bool> checkCancel = null, string workingDirectory = null)
        {
            Process process = null;
            IlogProgress pr = null;

            try
            {

                if (string.IsNullOrEmpty(workingDirectory))
                {
                    workingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
                }

                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    WorkingDirectory = workingDirectory
                };

                pr = new BuildProgress(logger);

                process = Process.Start(processStartInfo);


                process.OutputDataReceived += (sender, args) =>
                {
                    if (checkCancel != null)
                    {
                        var r = checkCancel();
                        if (r)
                        {

                            try
                            {
                                process?.Dispose();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }

                            try
                            {
                                process?.Kill();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        if (args.Data.StartsWith(" "))//有这个代表肯定build有出问题
                        {
                            logger.Info(args.Data);
                        }
                        if (args.Data.Contains(": warning"))
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Warn,
                                message = args.Data
                            });
                        }
                        else if (args.Data.Contains(": error"))
                        {
                            if (pr is BuildProgress)
                            {
                                pr.Dispose();
                                pr = new FullLogProgress(logger);//一旦有出错 后面就走全部日志
                            }
                            logger.Error(args.Data);
                        }
                        else
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Info,
                                message = args.Data
                            });
                        }
                    }
                };


                process.BeginOutputReadLine();


                process.ErrorDataReceived += (sender, data) =>
                {
                    if (pr is BuildProgress)
                    {
                        pr.Dispose();
                        pr = new FullLogProgress(logger);//一旦有出错 后面就走全部日志
                    }
                    if (checkCancel != null)
                    {
                        var r = checkCancel();
                        if (r)
                        {

                            try
                            {
                                process?.Dispose();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }

                            try
                            {
                                process?.Kill();
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(data.Data))
                    {
                        pr.Log(new BuildEventArgs
                        {
                            level = LogLevel.Error,
                            message = data.Data
                        });
                    }
                };
                process.BeginErrorReadLine();


                //录入命令
                process.StandardInput.WriteLine($"{commandToRun} & exit");





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
                if (checkCancel != null)
                {
                    var r = checkCancel();
                    if (r)
                    {
                        logger?.Error("deploy task was canceled!");
                        return false;
                    }
                }

                logger?.Error(ex.Message);
                return false;
            }
            finally
            {
                try
                {
                    process?.Kill();
                }
                catch (Exception)
                {
                    //ignore
                }
                try
                {
                    pr?.Dispose();
                }
                catch (Exception)
                {
                    //ignore
                }
                try
                {
                    process?.Dispose();
                }
                catch (Exception)
                {
                    //ignore
                }

            }
        }
        /// <summary>
        /// 获取Msbuild的路径
        /// </summary>
        /// <returns></returns>
        public static string GetMsBuildPath()
        {
            try
            {
                var instances = MSBuildLocator.QueryVisualStudioInstances().ToList();
                var instance_laster = instances.OrderByDescending(r => r.Version).FirstOrDefault();
                if (instance_laster != null && !string.IsNullOrEmpty(instance_laster.MSBuildPath))
                {
                    return Path.Combine(instance_laster.MSBuildPath, "MSBuild.exe");
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }

    }

    public interface IlogProgress: IDisposable
    {
        void Log(BuildEventArgs ar);
    }

    public class FullLogProgress : IlogProgress
    {
        private readonly Logger _logger;
        public FullLogProgress(Logger logger)
        {
            _logger = logger;
        }
        public void Dispose()
        {

        }

        public void Log(BuildEventArgs objEventArgs)
        {
            if (objEventArgs.level == LogLevel.Warn)
            {
                _logger.Warn(objEventArgs.message);
                return;
            }

            if (objEventArgs.level == LogLevel.Error)
            {
                _logger.Error(objEventArgs.message);
                return;
            }

            _logger.Info(objEventArgs.message);
        }
    }

    public class BuildProgress : IlogProgress
    {
        public event EventHandler<BuildEventArgs> BuildEvent;

        private readonly Logger _logger;

        private readonly IDisposable _subscribe;
        public BuildProgress(Logger logger)
        {
            _logger = logger;

            _subscribe = System.Reactive.Linq.Observable
                .FromEventPattern<BuildEventArgs>(this, "BuildEvent")
                .Sample(TimeSpan.FromMilliseconds(100))
                .Subscribe(arg => { OnBuildEvent(arg.Sender, arg.EventArgs); });
        }

        private void OnBuildEvent(object objSender, BuildEventArgs objEventArgs)
        {
            if (objEventArgs.level == LogLevel.Warn)
            {
                _logger.Warn(objEventArgs.message);
                return;
            }

            if (objEventArgs.level == LogLevel.Error)
            {
                _logger.Error(objEventArgs.message);
                return;
            }

            _logger.Info(objEventArgs.message);
        }

        public void Log(BuildEventArgs ar)
        {
            if (BuildEvent == null)
            {
                OnBuildEvent(null, ar);
            }
            else
            {
                BuildEvent(this, ar);
            }
        }

        public void Dispose()
        {
            _subscribe?.Dispose();
        }
    }

    public class BuildEventArgs : EventArgs
    {
        public LogLevel level { get; set; }
        public string message { get; set; }

    }
}
