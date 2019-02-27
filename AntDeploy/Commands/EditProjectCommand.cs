using AntDeploy.Models;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AntDeploy.Winform;
using EnvDTE;
using System;

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
        private Project _project;

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
            Deploy deploy = new Deploy(_projectFile,_project);
            deploy.ShowDialog();
        }

    }
}