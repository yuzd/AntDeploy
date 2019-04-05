using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgent.Util
{
    public class ProcessHepler
    {
        /// <summary>
        /// appcmd recycle apppool /apppool.name: string
        /// appcmd stop apppool /apppool.name: Marketing
        /// appcmd start apppool /apppool.name: Marketing
        ///  appcmd stop site /site.name: contoso
        ///  appcmd start site /site.name: contoso
        ///  IISHelper.RunAppCmd("start recycle /apppool.name:\"DefaultWebSitetestmvc\"");
        ///IISHelper.RunAppCmd("stop apppool /apppool.name:\"DefaultWebSitetestmvc\"");
        ///IISHelper.RunAppCmd("stop site /site.name:\"Default Web Site\"");
        ///IISHelper.RunAppCmd("start apppool /apppool.name:DefaultWebSitetestmvc");
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool RunAppCmd(string args, Action<string> log)
        {
            var expanded = Environment.ExpandEnvironmentVariables("%windir%\\System32\\inetsrv\\appcmd.exe");
            return ProcessHepler.RunExternalExe(expanded, args, log);
        }

        public static bool RuSCCmd(string args, Action<string> log)
        {
            var expanded = Environment.ExpandEnvironmentVariables("%windir%\\System32\\sc.exe");
            return ProcessHepler.RunExternalExe(expanded, args, log);
        }

        public static bool RunExternalExe(string projectPath, string arguments, Action<string> logger)
        {
            Process process = null;
            try
            {

                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                process = new Process();

                process.StartInfo.FileName = projectPath;
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
                        logger(args.Data);
                    }
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Data)) logger(data.Data);
                };
                process.BeginErrorReadLine();
                process.WaitForExit();
                //var err = process.StandardError.ReadToEnd();
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                process?.Dispose();
            }
        }

    }
}
