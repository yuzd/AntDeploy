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
        /// <param name="arguments"></param>
        /// <param name="logAction"></param>
        /// <param name="errLogAction"></param>
        /// <returns></returns>
        public static bool RunDotnetExternalExe(string arguments,Action<string> logAction,Action<string> errLogAction)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                throw new ArgumentException(nameof(arguments));
            }

            if (!arguments.StartsWith(" "))
            {
                arguments += " ";
            }

            var process = new Process();

            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Verb = "runas";
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += (sender, args) =>
            {
                logAction(args.Data);
            };

            process.ErrorDataReceived += (sender, data) =>
            {
                errLogAction(data.Data);
            };

            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        private static string Format(string filename, string arguments)
        {
            return "'" + filename +
                   ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                   "'";
        }
    }
}
