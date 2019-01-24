using EnvDTE;
using AntDeploy.Models;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace AntDeploy.Commands
{
    internal abstract class BaseCommand : OleMenuCommand
    {
        // Editing File - Project File Watcher

        private static void OnBaseBeforeQueryStatus(object sender, EventArgs e) => (sender as BaseCommand).OnBeforeQueryStatus();

        private static void OnBaseCommandEventHandler(object sender, EventArgs e) => (sender as BaseCommand).OnExecute();





        protected BaseCommand(EditProjectPackage package, string cmdSet, int cmdId)
            : base(OnBaseCommandEventHandler, null, OnBaseBeforeQueryStatus, new CommandID(Guid.Parse(cmdSet), cmdId))
        {
            Package = package;
        }

        protected EditProjectPackage Package { get; }

        protected IEnumerable<Project> SelectedProjects => EditProjectPackage.DTE.SelectedItems.OfType<SelectedItem>().Where(x => x.Project != null).Select(x => x.Project);

        protected virtual void OnBeforeQueryStatus()
        {
            Visible = false;
        }

        protected abstract void OnExecute();

      
    }
}