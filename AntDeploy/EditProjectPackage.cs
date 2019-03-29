using EnvDTE80;
using AntDeploy.Commands;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using AntDeployWinform;
using UIContextGuids = Microsoft.VisualStudio.Shell.Interop.UIContextGuids;

namespace AntDeploy
{
    [Guid(Ids.PACKAGE)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.VERSION, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(Ids.AUTO_LOAD_CONTEXT)]
    [ProvideUIContextRule(Ids.AUTO_LOAD_CONTEXT,
        "HasProject",
        "SingleProject | MultipleProjects",
        new[] { "SingleProject", "MultipleProjects" },
        new[] { UIContextGuids.SolutionHasSingleProject, UIContextGuids.SolutionHasMultipleProjects },
        delay: 1000)]
    public sealed class EditProjectPackage : Package
    {
        public static DTE2 DTE { get; private set; }

        internal OleMenuCommandService CommandService { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();

            DTE = GetService(typeof(EnvDTE.DTE)) as DTE2;
            CommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            EditProjectCommand.Initialize(this);
            EditProjectsCommand.Initialize(this);
        }
    }
}