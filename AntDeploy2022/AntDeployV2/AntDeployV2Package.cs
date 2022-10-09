using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using yuzd.AntDeploy;
using Task = System.Threading.Tasks.Task;
using Process = System.Diagnostics.Process;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using EnvDTE;

namespace AntDeployV2
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(AntDeployV2Package.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class AntDeployV2Package : AsyncPackage, IDisposable
    {
        private static DTE2 _dte;
        internal static DTE2 DTE => _dte;

        internal OleMenuCommandService CommandService { get; private set; }

        /// <summary>
        /// AntDeployV2Package GUID string.
        /// </summary>
        public const string PackageGuidString = "43472fa4-8016-4e2b-87ba-77c8ac5781d3";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
            {
                try
                {
                    CommandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

                    _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;

                    CommandID menuCommandID = new CommandID(GuidList.guidAntDeployCmdSet, (int)PkgCmdIDList.AntDeployCommand);
                    MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                    mcs.AddCommand(menuItem);
                }
                catch (Exception e)
                {

                }

            }
        }

        #endregion



        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var selectItems = DTE.SelectedItems.OfType<SelectedItem>().Where(x => x.Project != null).Select(x => x.Project).ToArray();
            if (selectItems.Length == 1)
            {
                var project = selectItems[0];
                var projectFile = project.FullName;
                try
                {

                    Saveall();

                    ProjectParam param = new ProjectParam();
                    param.IsWebProejct = ProjectHelper.IsWebProject(project);
                    param.IsNetcorePorject = ProjectHelper.IsDotNetCoreProject(project);
                    param.OutPutName = project.GetProjectProperty("OutputFileName");
                    param.VsVersion = ProjectHelper.GetVsVersion();
                    try
                    {

                        param.MsBuildPath = new MsBuildHelper().GetMsBuildPath();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    param.ProjectPath = projectFile;
                    if (!string.IsNullOrEmpty(param.MsBuildPath))
                    {
                        param.MsBuildPath = Path.Combine(param.MsBuildPath, "MSBuild.exe");
                    }


                    DoAntDeployProcess(projectFile, param);
                }
                catch (Exception ex)
                {
                    MessageBox(ex.ToString());
                }

            }
            else
            {
                MessageBox("multi projects not supported!");
            }
        }

        private void MessageBox(string msg)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                0,
                ref clsid,
                "AntDeploy",
                msg,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0,        // false
                out result));
        }

        private void Saveall()
        {
            try
            {
                DTE.Documents.SaveAll();
            }
            catch (Exception)
            {
                //ignore
            }
            try
            {
                // Get the current solution.
                var solution = DTE.Solution;

                // Save the solution file.
                if (!solution.Saved)
                    solution.SaveAs(solution.FullName);

                // Save the project files within the solution.
                for (int i = 1; i <= solution.Projects.Count; i++)
                {
                    var project = solution.Projects.Item(i);

                    // Check if this item is Solution Folder.
                    if (project.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
                        continue;

                    if (!project.Saved)
                        project.Save();

                    // Save all the files and items within the project.
                    //for (int j = 1; j <= project.ProjectItems.Count; j++)
                    //{
                    //    var item = project.ProjectItems.Item(j);
                    //    if (!item.Saved)
                    //        item.Save();
                    //}
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void DoAntDeployProcess(string projectPath, ProjectParam param)
        {
            var globalConfig = ProjectHelper.ReadGlobalConfig();
            try
            {
                if (!globalConfig.MultiInstance) StartListeningForWindowChanges();
                
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var codeBase = assembly.Location;
                var codeBaseDirectory = Path.GetDirectoryName(codeBase);
                param.DomainPath = codeBaseDirectory;
                var md5 = MD5(projectPath);
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var projectPram = JsonConvert.SerializeObject(param);
                var projectPramPath = Path.Combine(path, md5 + "_param.json");
                File.WriteAllText(projectPramPath, projectPram, Encoding.UTF8);


                var ant = Path.Combine(codeBaseDirectory, "AntDeployApp.exe");
                using (var process = new Process())
                {
                    process.StartInfo.FileName = ant;
                    process.StartInfo.Arguments = $"\"{projectPramPath}\"";
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.Verb = "runas";
                    process.Start();
                    if (!globalConfig.MultiInstance)
                    {
                        processHIntPtrGet = () =>
                        {
                            try
                            {
                                return process.MainWindowHandle;
                            }
                            catch (Exception)
                            {
                                return new IntPtr(0);
                            }
                        };
                    }
                    //var hwndMainWindow = (IntPtr)DTE.MainWindow.HWnd;
                    //SetParent(processHIntPtr, hwndMainWindow);
                    if (!globalConfig.MultiInstance) process.WaitForExit();

                }
            }
            finally
            {
                if (!globalConfig.MultiInstance) StopListeningForWindowChanges();
            }

        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = string.Empty;
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        //[DllImport("User32.dll")]
        //private static extern int SetParent(IntPtr hwndChild, IntPtr hwndParent);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, uint dwflags);
        [DllImport("user32.dll")]
        internal static extern int UnhookWinEvent(IntPtr hWinEventHook);
        internal delegate void WinEventProc(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        const uint WINEVENT_OUTOFCONTEXT = 0;
        const uint EVENT_SYSTEM_FOREGROUND = 3;
        private IntPtr winHook = new IntPtr(0);
        private static Func<IntPtr> processHIntPtrGet;
        private WinEventProc listener;

        public void StartListeningForWindowChanges()
        {
            try
            {
                listener = new WinEventProc(EventCallback);
                //setting the window hook
                winHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, listener, 0, 0, WINEVENT_OUTOFCONTEXT);
            }
            catch (Exception)
            {
                //Ignore
            }
        }

        public void StopListeningForWindowChanges()
        {
            try
            {
                if (winHook == new IntPtr(0)) return;
                UnhookWinEvent(winHook);
            }
            catch (Exception)
            {
                //Ignore
            }
        }

        private static void EventCallback(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime)
        {
            // handle active window changed!
            var handler = processHIntPtrGet();
            if (handler == new IntPtr(0)) return;
            var hwndMainWindow = (IntPtr)DTE.MainWindow.HWnd;
            if (hWnd == hwndMainWindow)
            {
                SetForegroundWindow(handler);
            }
        }

        public void Dispose()
        {
            StopListeningForWindowChanges();
            CommandService?.Dispose();
        }
    }

    [Serializable]
    public class ProjectParam
    {
        /// <summary>
        /// 经过vssdk判断是web项目的
        /// </summary>
        public bool IsWebProejct { get; set; }

        /// <summary>
        ///  经过vssdk判断是netcore项目的
        /// </summary>
        public bool IsNetcorePorject { get; set; }

        /// <summary>
        /// 经过vssdk获取到的outputname
        /// </summary>
        public string OutPutName { get; set; }
        public string VsVersion { get; set; }
        public string MsBuildPath { get; set; }
        public string ProjectPath { get; set; }
        public string NetCoreSDKVersion { get; set; }
        public string DomainPath { get; set; }
    }

    internal class GlobalConfig
    {
        public bool MultiInstance { get; set; }
    }
}
