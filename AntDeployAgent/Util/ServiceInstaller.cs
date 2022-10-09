﻿using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using AntDeployAgent.Util;

namespace AntDeployAgentWindows.Util
{
    public class ServiceInstaller
    {
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        private const int SERVICE_CONFIG_DESCRIPTION = 1;
        internal const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        internal const int SC_STATUS_PROCESS_INFO = 0;
        [StructLayout(LayoutKind.Sequential)]
        private class SERVICE_STATUS
        {
            public int dwServiceType = 0;
            public ServiceState dwCurrentState = 0;
            public int dwControlsAccepted = 0;
            public int dwWin32ExitCode = 0;
            public int dwServiceSpecificExitCode = 0;
            public int dwCheckPoint = 0;
            public int dwWaitHint = 0;
        }

        #region OpenSCManager
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr OpenSCManager(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);
        #endregion

        #region OpenService
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);
        #endregion

        #region CreateService
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess, int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);
        #endregion
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int ChangeServiceConfig2(IntPtr hService, uint dwInfoLevel, ref SERVICE_DESCRIPTION info);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool QueryServiceStatusEx(SafeHandle hService, int infoLevel, IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

        #region CloseServiceHandle
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);
        #endregion

        #region QueryServiceStatus
        [DllImport("advapi32.dll")]
        private static extern int QueryServiceStatus(IntPtr hService, SERVICE_STATUS lpServiceStatus);
        #endregion

        #region DeleteService
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteService(IntPtr hService);
        #endregion

        #region ControlService
        [DllImport("advapi32.dll")]
        private static extern int ControlService(IntPtr hService, ServiceControl dwControl, SERVICE_STATUS lpServiceStatus);
        #endregion

        #region StartService
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);
        #endregion


        /// <summary>
        /// kill windows服务的进程
        /// </summary>
        /// <returns></returns>
        public static bool KillWindowsService(ServiceController sc, Action<string> logger)
        {
            var servicePid = GetServiceProcessId(sc, logger);
            if (servicePid < 1) return false;
            return KillProcessAndChildren(servicePid, logger);
        }



        /// <summary>
        /// Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        public static bool KillProcessAndChildren(int pid, Action<string> logger)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection moc = searcher.Get();
                foreach (var o in moc)
                {
                    var mo = o as ManagementObject;
                    if (mo == null) continue;
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]), logger);
                }
            }
            catch (Exception e)
            {
                //ignore
            }
            

            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
                return true;
            }
            catch (ArgumentException)
            {
                // Process already exited.
                return true;
            }
            catch (Exception e)
            {
                logger("【Warn】Windows Service Kill Fail :" + e.Message);
            }

            return false;
        }

        /// <summary>
        /// 找到这个windows服务对应的服务进程id
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static int GetServiceProcessId( ServiceController sc, Action<string> logger)
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
                        var pid = (int) ssp.dwProcessId;
                        logger("Windows Service get process pid : " + pid);
                        return pid;
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

            logger("【Warn】Windows Service get process pid Fail " );
            return -1;
        }

        public static void Uninstall(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
                if (service == IntPtr.Zero)
                    throw new ApplicationException("Service not installed.");

                try
                {
                    StopService(service);
                    if (!DeleteService(service))
                        throw new ApplicationException("Could not delete service " + Marshal.GetLastWin32Error());
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static bool ServiceIsInstalled(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);

                if (service == IntPtr.Zero)
                    return false;

                CloseServiceHandle(service);
                return true;
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void InstallAndStart(string serviceName, string displayName, string fileName,string startType,string desc)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
                ServiceBootFlag startTypeFlag = ServiceBootFlag.AutoStart;
                if (!string.IsNullOrEmpty(startType))
                {
                    if (!startType.ToLower().Equals("auto"))
                    {
                        startTypeFlag = ServiceBootFlag.DemandStart;
                    }
                }
                if (service == IntPtr.Zero)
                    service = CreateService(scm, serviceName, displayName, ServiceAccessRights.AllAccess, SERVICE_WIN32_OWN_PROCESS, startTypeFlag, ServiceError.Normal, fileName, null, IntPtr.Zero, null, null, null);

                if (service == IntPtr.Zero)
                    throw new ApplicationException("Failed to install service.");

                if (!string.IsNullOrEmpty(desc))
                {
                    var serviceDescription = new SERVICE_DESCRIPTION { lpDescription = Marshal.StringToHGlobalAnsi(desc) };
                    ChangeServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, ref serviceDescription);
                    Marshal.FreeHGlobal(serviceDescription.lpDescription);
                }

                try
                {
                    StartService(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void StartService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Start);
                if (service == IntPtr.Zero)
                    throw new ApplicationException("Could not open service.");

                try
                {
                    StartService(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void StopService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
                if (service == IntPtr.Zero)
                    throw new ApplicationException("Could not open service.");

                try
                {
                    StopService(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        private static void StartService(IntPtr service)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();
            StartService(service, 0, 0);
            var changedStatus = WaitForServiceStatus(service, ServiceState.StartPending, ServiceState.Running);
            if (!changedStatus)
                throw new ApplicationException("Unable to start service");
        }

        private static void StopService(IntPtr service)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();
            ControlService(service, ServiceControl.Stop, status);
            var changedStatus = WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
            if (!changedStatus)
                throw new ApplicationException("Unable to stop service");
        }

        public static ServiceState GetServiceStatus(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);
                if (service == IntPtr.Zero)
                    return ServiceState.NotFound;

                try
                {
                    return GetServiceStatus(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        private static ServiceState GetServiceStatus(IntPtr service)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();

            if (QueryServiceStatus(service, status) == 0)
                throw new ApplicationException("Failed to query service status.");

            return status.dwCurrentState;
        }

        private static bool WaitForServiceStatus(IntPtr service, ServiceState waitStatus, ServiceState desiredStatus)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();
            QueryServiceStatus(service, status);
            if (status.dwCurrentState == desiredStatus) return true;

            int dwStartTickCount = Environment.TickCount;
            int dwOldCheckPoint = status.dwCheckPoint;

            while (status.dwCurrentState == waitStatus)
            {
                // Do not wait longer than the wait hint. A good interval is
                // one tenth the wait hint, but no less than 1 second and no
                // more than 10 seconds.

                int dwWaitTime = status.dwWaitHint / 10;

                if (dwWaitTime < 1000) dwWaitTime = 1000;
                else if (dwWaitTime > 10000) dwWaitTime = 10000;

                Thread.Sleep(dwWaitTime);

                // Check the status again.

                if (QueryServiceStatus(service, status) == 0) break;

                if (status.dwCheckPoint > dwOldCheckPoint)
                {
                    // The service is making progress.
                    dwStartTickCount = Environment.TickCount;
                    dwOldCheckPoint = status.dwCheckPoint;
                }
                else
                {
                    if (Environment.TickCount - dwStartTickCount > status.dwWaitHint)
                    {
                        // No progress made within the wait hint
                        break;
                    }
                }
            }
            Thread.Sleep(2000);
            return (status.dwCurrentState == desiredStatus);
        }

        private static IntPtr OpenSCManager(ScmAccessRights rights)
        {
            IntPtr scm = OpenSCManager(null, null, rights);
            if (scm == IntPtr.Zero)
                throw new ApplicationException("Could not connect to service control manager.");

            return scm;
        }
        /**
         * 采用nssm来第一次安装服务
         */
        public static bool NssmInstallAndStart(string serviceName, string param, string execFullPath, string serviceStartType, string serviceDescription,Action<string> log)
        {
            string nssmPath = Path.Combine(Startup.RootPath, "nssm.exe");
            if (!File.Exists(nssmPath))
            {
                throw new FileNotFoundException(nssmPath);
            }

            log($"nssm.exe install {serviceName} \"{execFullPath}\"");
            var rt = ProcessHepler.RunExternalExe(nssmPath, $"install {serviceName} \"{execFullPath}\"", log);
            if (!rt)
            {
                return false;
            }

            log($"nssm.exe set {serviceName} AppDirectory \"{new FileInfo(execFullPath).DirectoryName}\"");
            rt = ProcessHepler.RunExternalExe(nssmPath, $"set {serviceName} AppDirectory \"{new FileInfo(execFullPath).DirectoryName}\"", log);
            if (!rt)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(serviceDescription))
            {
                log($"nssm.exe set {serviceName} Description \"{serviceDescription}\"");
                rt = ProcessHepler.RunExternalExe(nssmPath, $"set {serviceName} Description \"{serviceDescription}\"", log);
                if (!rt)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(param))
            {
                log($"nssm.exe set {serviceName} AppParameters \"{param}\"");
                rt = ProcessHepler.RunExternalExe(nssmPath, $"set {serviceName} AppParameters \"{param}\"", log);
                if (!rt)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(serviceStartType) && !serviceStartType.ToLower().Equals("auto"))
            {
                log($"nssm.exe set {serviceName} Start SERVICE_DEMAND_START");
                rt = ProcessHepler.RunExternalExe(nssmPath, $"set {serviceName} Start SERVICE_DEMAND_START", log);
                if (!rt)
                {
                    return false;
                }
            }

            ServiceInstaller.StartService(serviceName);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class SERVICE_STATUS_PROCESS
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
    public enum ServiceState
    {
        Unknown = -1, // The state cannot be (has not been) retrieved.
        NotFound = 0, // The service is not known on the host server.
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7
    }

    [Flags]
    public enum ScmAccessRights
    {
        Connect = 0x0001,
        CreateService = 0x0002,
        EnumerateService = 0x0004,
        Lock = 0x0008,
        QueryLockStatus = 0x0010,
        ModifyBootConfig = 0x0020,
        StandardRightsRequired = 0xF0000,
        AllAccess = (StandardRightsRequired | Connect | CreateService |
                     EnumerateService | Lock | QueryLockStatus | ModifyBootConfig)
    }

    [Flags]
    public enum ServiceAccessRights
    {
        QueryConfig = 0x1,
        ChangeConfig = 0x2,
        QueryStatus = 0x4,
        EnumerateDependants = 0x8,
        Start = 0x10,
        Stop = 0x20,
        PauseContinue = 0x40,
        Interrogate = 0x80,
        UserDefinedControl = 0x100,
        Delete = 0x00010000,
        StandardRightsRequired = 0xF0000,
        AllAccess = (StandardRightsRequired | QueryConfig | ChangeConfig |
                     QueryStatus | EnumerateDependants | Start | Stop | PauseContinue |
                     Interrogate | UserDefinedControl)
    }

    public enum ServiceBootFlag
    {
        Start = 0x00000000,
        SystemStart = 0x00000001,
        AutoStart = 0x00000002,
        DemandStart = 0x00000003,
        Disabled = 0x00000004
    }

    public enum ServiceControl
    {
        Stop = 0x00000001,
        Pause = 0x00000002,
        Continue = 0x00000003,
        Interrogate = 0x00000004,
        Shutdown = 0x00000005,
        ParamChange = 0x00000006,
        NetBindAdd = 0x00000007,
        NetBindRemove = 0x00000008,
        NetBindEnable = 0x00000009,
        NetBindDisable = 0x0000000A
    }

    public enum ServiceError
    {
        Ignore = 0x00000000,
        Normal = 0x00000001,
        Severe = 0x00000002,
        Critical = 0x00000003
    }
    public struct SERVICE_DESCRIPTION
    {
        public IntPtr lpDescription;
    }
}
