using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
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

        #region 根据服务获取对应的进程id
        [StructLayout(LayoutKind.Sequential)]
        private sealed class SERVICE_STATUS_PROCESS
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint dwServiceType;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwCurrentState;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwControlsAccepted;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwWin32ExitCode;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwServiceSpecificExitCode;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwCheckPoint;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwWaitHint;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwProcessId;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwServiceFlags;
        }

        private const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        private const int SC_STATUS_PROCESS_INFO = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool QueryServiceStatusEx(SafeHandle hService, int infoLevel, IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

        public static int GetServiceProcessId(ServiceController sc)
        {
            if (sc == null)
                throw new ArgumentNullException("sc");

            IntPtr zero = IntPtr.Zero;

            try
            {
                UInt32 dwBytesNeeded;
                // Call once to figure the size of the output buffer.
                QueryServiceStatusEx(sc.ServiceHandle, SC_STATUS_PROCESS_INFO, zero, 0, out dwBytesNeeded);
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    // Allocate required buffer and call again.
                    zero = Marshal.AllocHGlobal((int)dwBytesNeeded);

                    if (QueryServiceStatusEx(sc.ServiceHandle, SC_STATUS_PROCESS_INFO, zero, dwBytesNeeded, out dwBytesNeeded))
                    {
                        var ssp = new SERVICE_STATUS_PROCESS();
                        Marshal.PtrToStructure(zero, ssp);
                        return (int)ssp.dwProcessId;
                    }
                }
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(zero);
                }
            }
            return -1;
        } 
        #endregion

        public static bool Kill(int pid)
        {
            try
            {
                var p = Process.GetProcessById(pid);
                p.Kill();
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
