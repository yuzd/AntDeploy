using AntDeploy.Models;
using AntDeployWinform.Models;
using AntDeployWinform.Util;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AntDeploy.Commands
{
    internal sealed class EditProjectCommand : BaseCommand
    {
        public static EditProjectCommand Instance { get; private set; }

        public static void Initialize(EditProjectPackage package)
        {
            Instance = new EditProjectCommand(package);
            package.CommandService.AddCommand(Instance);
        }

        private string _projectFile;
        private EnvDTE.Project _project;

        private EditProjectCommand(EditProjectPackage package)
            : base(package, Ids.CMD_SET, Ids.EDIT_PROJECT_MENU_COMMAND_ID)
        {
        }

        protected override void OnBeforeQueryStatus()
        {


            var projects = SelectedProjects.ToArray();
            if (projects.Length == 1)
            {
                _project = projects[0];

                _projectFile = _project.FullName;
                Text = "AntDeploy";
                Visible = true;
                return;

                //var project = projects[0];
                //if (ProjectHelper.IsDotNetCoreProject(project))
                //{
                //    _projectFile = project.FullName;
                //    Text = "AntDeploy";
                //    Visible = true;

                //}
                //else
                //{
                //    Visible = false;
                //}
            }
            else
            {
                Visible = false;
            }
        }

        protected override void OnExecute()
        {
            //RootNamespace Title Product OutputFileName
            //var friendlyName = "antDomain";
            //var assembly = Assembly.GetExecutingAssembly();
            //var codeBase = assembly.Location;
            //var codeBaseDirectory = Path.GetDirectoryName(codeBase);
            //var setup = new AppDomainSetup()
            //{
            //    ApplicationName = "AntDeployApplication",
            //    ApplicationBase = codeBaseDirectory,
            //    DynamicBase = codeBaseDirectory,
            //};
            //setup.CachePath = setup.ApplicationBase;
            //setup.ShadowCopyFiles = "true";
            //setup.ShadowCopyDirectories = setup.ApplicationBase;
            //AppDomain.CurrentDomain.SetShadowCopyFiles();
            //SecurityZone zone = SecurityZone.MyComputer;
            //Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
            //Evidence evidence = new Evidence(baseEvidence);
            //string assemblyName = Assembly.GetExecutingAssembly().FullName;
            //evidence.AddAssembly(assemblyName);
            //evidence.AddHost(new Zone(zone));

            //AppDomain otherDomain = AppDomain.CreateDomain(friendlyName,evidence, setup);

            try
            {

                EditProjectPackage.DTE.Documents.SaveAll();
                Saveall();

                ProjectParam param = new ProjectParam();
                param.IsWebProejct = ProjectHelper.IsWebProject(_project);
                param.IsNetcorePorject = ProjectHelper.IsDotNetCoreProject(_project);
                param.OutPutName = _project.GetProjectProperty("OutputFileName");
                param.VsVersion = ProjectHelper.GetVsVersion();
                param.MsBuildPath = ProjectHelper.GetMsBuildPath();
                param.ProjectPath = _projectFile;
                if (!string.IsNullOrEmpty(param.MsBuildPath))
                {
                    param.MsBuildPath = Path.Combine(param.MsBuildPath, "MSBuild.exe");
                }

                DoAntDeployProcess(_projectFile, param);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //AppDomain.Unload(otherDomain);
            }
        }

        private void Saveall()
        {
            try
            {
                // Get the current solution.
                var solution = EditProjectPackage.DTE.Solution;

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

            }
        }

        private void DoAntDeployProcess(string projectPath, ProjectParam param)
        {

            var md5 = CodingHelper.MD5(projectPath);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var projectPram = JsonConvert.SerializeObject(param);
            var projectPramPath = Path.Combine(path, md5 + "_param.json");
            File.WriteAllText(projectPramPath, projectPram, Encoding.UTF8);

            var assembly = Assembly.GetExecutingAssembly();
            var codeBase = assembly.Location;
            var codeBaseDirectory = Path.GetDirectoryName(codeBase);
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

                process.WaitForExit();
            }

        }

    }



}