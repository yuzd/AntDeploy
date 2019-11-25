using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LibGit2Sharp;

namespace AntDeployCommand.Utils
{
    public class CommandHelper
    {
        /// <summary>
        /// 是否启动完全日志
        /// </summary>
        public static bool IgnoreLazy =  false;

        /// <summary>
        /// 执行dotnet
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="arguments"></param>
        /// <param name="publishPath"></param>
        /// <returns></returns>
        public static bool RunDotnetExe(string projectPath,  string publishPath, string arguments)
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
                LogHelper.Info($"【Build】clear folder success:{publishPath}");
                arguments += " -o \"" + publishPath + "\"";
            }
            LogHelper.Info($"【Build】current project Path:{projectPath}");
            var dotnet = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)? "dotnet": System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX)? "/usr/local/share/dotnet/dotnet": "/usr/share/dotnet/dotnet";
            return RunDotnetExternalExe(dotnet, arguments);
        }
        private static bool RunCommand(string commandToRun, string workingDirectory = null)
        {
            try
            {
                if (string.IsNullOrEmpty(workingDirectory))
                {
                    workingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
                }

                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "bash"),
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    WorkingDirectory = workingDirectory
                };

                var process = Process.Start(processStartInfo);

                if (process == null)
                {
                    LogHelper.Error("Process should not be null.");
                    return false;
                }

                process.StandardInput.WriteLine($"{commandToRun} & exit");
                process.WaitForExit();

                var output = process.StandardOutput.ReadToEnd();
                if (process.ExitCode == 0)
                {
                    LogHelper.Info("【Command】" + output);
                }
                else
                {
                    LogHelper.Warn("【Command】" + output);
                }

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
                LogHelper.Error("【Command】" + ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="prefix"></param>
        /// <param name="workingFolder"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static bool RunDotnetExternalExe(string fileName, string arguments,string workingFolder = null,string prefix = "【Build】")
        {
            if (!string.IsNullOrEmpty(workingFolder))
            {
                return RunCommand(arguments, workingFolder);
            }
            Process process = null;
            BuildProgress pr = null;
            try
            {
                LogHelper.Info(prefix + fileName + " " + arguments);
                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                //执行dotnet命令如果 projectdir路径含有空格 或者 outDir 路径含有空格 都是没有问题的
                pr = new BuildProgress();

                process = new Process();

                process.StartInfo.WorkingDirectory = workingFolder??string.Empty;
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Verb = "runas";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                //process.StartInfo.StandardInputEncoding = Encoding.UTF8;

                process.Start();


                process.OutputDataReceived += (sender, args) =>
                {
                   
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        if (args.Data.StartsWith(" "))//有这个代表肯定build有出问题
                        {
                            LogHelper.Info(prefix + args.Data);
                        }
                        if (args.Data.Contains(": warning"))
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Warning,
                                message = prefix + args.Data
                            });
                        }
                        else if (args.Data.Contains(": error"))
                        {
                            LogHelper.Error(prefix + args.Data);
                        }
                        else
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Info,
                                message = prefix + args.Data
                            });
                        }
                    }
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Data))
                    {
                        LogHelper.Error(prefix + data.Data);
                       
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
                LogHelper.Error(prefix + ex.Message);
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
    }

    public class BuildProgress : IDisposable
    {
        public event EventHandler<BuildEventArgs> BuildEvent;


        private readonly IDisposable _subscribe;
        public BuildProgress()
        {
            if (!CommandHelper.IgnoreLazy)
            {
                _subscribe = System.Reactive.Linq.Observable
                    .FromEventPattern<BuildEventArgs>(this, "BuildEvent")
                    .Sample(TimeSpan.FromMilliseconds(100))
                    .Subscribe(arg => { OnBuildEvent(arg.Sender, arg.EventArgs); });
            }
        }

        private void OnBuildEvent(object objSender, BuildEventArgs objEventArgs)
        {
            if (objEventArgs.level == LogLevel.Warning)
            {
                LogHelper.Warn(objEventArgs.message);
                return;
            }

            if (objEventArgs.level == LogLevel.Error)
            {
                LogHelper.Error(objEventArgs.message);
                return;
            }

            LogHelper.Info(objEventArgs.message);
        }

        public void Log(BuildEventArgs ar)
        {
            if (_subscribe == null || BuildEvent == null)
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
