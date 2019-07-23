using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using LibGit2Sharp;

namespace AntDeployCommand.Utils
{
    public class CommandHelper
    {
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
            return RunDotnetExternalExe($"dotnet", arguments);
        }

        /// <summary>
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static bool RunDotnetExternalExe(string fileName, string arguments)
        {
            Process process = null;
            BuildProgress pr = null;
            try
            {
                LogHelper.Info("【Build】" + fileName + " " + arguments);
                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                //执行dotnet命令如果 projectdir路径含有空格 或者 outDir 路径含有空格 都是没有问题的
                pr = new BuildProgress();

                process = new Process();

                process.StartInfo.WorkingDirectory = "";
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
                        if (args.Data.StartsWith(" "))//有这个代表肯定build有出问题
                        {
                            LogHelper.Info("【Build】" + args.Data);
                        }
                        if (args.Data.Contains(": warning"))
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Warning,
                                message = "【Build】" + args.Data
                            });
                        }
                        else if (args.Data.Contains(": error"))
                        {
                            LogHelper.Error("【Build】" + args.Data);
                        }
                        else
                        {
                            pr.Log(new BuildEventArgs
                            {
                                level = LogLevel.Info,
                                message = "【Build】" + args.Data
                            });
                        }
                    }
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Data))
                    {
                        LogHelper.Error("【Build】" + data.Data);
                       
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
                LogHelper.Error("【Build】" + ex.Message);
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

            _subscribe = System.Reactive.Linq.Observable
                .FromEventPattern<BuildEventArgs>(this, "BuildEvent")
                .Sample(TimeSpan.FromMilliseconds(100))
                .Subscribe(arg => { OnBuildEvent(arg.Sender, arg.EventArgs); });
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
