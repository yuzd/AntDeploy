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
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="arguments"></param>
        /// <param name="logAction"></param>
        /// <param name="errLogAction"></param>
        /// <returns></returns>
        public static bool RunDotnetExternalExe(string projectPath,string arguments,Action<string> logAction,Action<string> errLogAction)
        {
            try
            {
                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                //if (!arguments.StartsWith(" "))
                //{
                //    arguments = " " + arguments;
                //}

                var process = new Process();

                process.StartInfo.WorkingDirectory = projectPath;
                process.StartInfo.FileName = "dotnet";
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
                    if(!string.IsNullOrWhiteSpace(args.Data))logAction(args.Data);
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Data)) errLogAction(data.Data);
                };
                process.BeginErrorReadLine();

                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                errLogAction(ex.Message);
                return false;
            }
        }

        
    }
}
