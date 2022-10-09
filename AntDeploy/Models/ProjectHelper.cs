﻿using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using static AntDeploy.Models.ProjectKinds;

namespace AntDeploy.Models
{
    internal static class ProjectHelper
    {
        public static string GetMsBuildPath()
        {
            var getmS = ToolLocationHelper.GetPathToBuildTools(ToolLocationHelper.CurrentToolsVersion);
            return getmS;
        }
        
        public static bool IsWebProject(Project project)
        {
            try
            {
                switch (project.Kind)
                {
                    case Web_PROJECT_KIND:
                    case Web_PROJECT_KIND_MVC1:
                    case Web_PROJECT_KIND_MVC2:
                    case Web_PROJECT_KIND_MVC3:
                    case Web_PROJECT_KIND_MVC4:
                    case Web_Application:
                    case Web_ASPNET5:
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }



        public static bool IsDotNetCoreProject(Project project)
        {
            try
            {
                switch (project.Kind)
                {
                    case CS_CORE_PROJECT_KIND:
                    case FS_CORE_PROJECT_KIND:
                    case VB_CORE_PROJECT_KIND:
                        return true;

                    default:
                        var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                        if (solution.GetProjectOfUniqueName(project.UniqueName, out var hierarchy) == 0)
                        {
                            return IsCpsProject(hierarchy);
                        }

                        return false;
                }
            }
            catch (Exception)
            {

                return true;
            }
        }

        // https://developercommunity.visualstudio.com/content/problem/312523/envdteprojectkind-no-longer-differentiates-between.html
        // https://github.com/Microsoft/VSProjectSystem/blob/master/doc/automation/detect_whether_a_project_is_a_CPS_project.md
        // https://www.mztools.com/articles/2014/MZ2014006.aspx
        private static bool IsCpsProject(IVsHierarchy hierarchy)
        {
            Microsoft.Requires.NotNull(hierarchy, nameof(hierarchy));
            return hierarchy.IsCapabilityMatch("CPS");
        }


        public static string GetVsVersion()
        {
            try
            {
                return EditProjectPackage.DTE.Version;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

      
      

        public const string SolutionItemsFolder = "Solution Items";

        public static void AddError(Package package, string errorText)
        {
            var ivsSolution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            var errorListProvider = new ErrorListProvider(package);
            var errorCategory = TaskErrorCategory.Error;
            IVsHierarchy hierarchyItem;
            var proj = GetActiveProject();
            var projectUniqueName = proj.FileName;
            var firstFileInProject = GetSelectedItemPaths(EditProjectPackage.DTE).FirstOrDefault();
            ivsSolution.GetProjectOfUniqueName(projectUniqueName, out hierarchyItem);
            var newError = new ErrorTask()
            {
                ErrorCategory = errorCategory,
                Category = TaskCategory.BuildCompile,
                Text = errorText,
                Document = firstFileInProject,
                Line = 1,
                Column = 1,
                HierarchyItem = hierarchyItem
            };
            newError.Navigate += (sender, e) =>
            {
                //there are two Bugs in the errorListProvider.Navigate method:
                //    Line number needs adjusting
                //    Column is not shown
                newError.Line++;
                errorListProvider.Navigate(newError, new Guid(EnvDTE.Constants.vsViewKindCode));
                newError.Line--;
            };
            errorListProvider.Tasks.Clear();    // clear previously created
            errorListProvider.Tasks.Add(newError);  // add item
            errorListProvider.Show(); 		// make sure it is visible
        }

        ///<summary>Gets the Solution Items solution folder in the current solution, creating it if it doesn't exist.</summary>
        public static Project GetSolutionItemsProject()
        {
            Solution2 solution = EditProjectPackage.DTE.Solution as Solution2;
            return solution.Projects
                           .OfType<Project>()
                           .FirstOrDefault(p => p.Name.Equals(SolutionItemsFolder, StringComparison.OrdinalIgnoreCase))
                      ?? solution.AddSolutionFolder(SolutionItemsFolder);
        }






        ///<summary>Gets the base directory of a specific Project, or of the active project if no parameter is passed.</summary>
        public static string GetRootFolder(Project project = null)
        {
            try
            {
                project = project ?? GetActiveProject();

                if (project == null || project.Collection == null)
                {
                    var doc = EditProjectPackage.DTE.ActiveDocument;
                    if (doc != null && !string.IsNullOrEmpty(doc.FullName))
                        return GetProjectFolder(doc.FullName);
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(project.FullName))
                    return null;
                string fullPath;
                try
                {
                    fullPath = project.Properties.Item("FullPath").Value as string;
                }
                catch (ArgumentException)
                {
                    try
                    {
                        // MFC projects don't have FullPath, and there seems to be no way to query existence
                        fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                    }
                    catch (ArgumentException)
                    {
                        // Installer projects have a ProjectPath.
                        fullPath = project.Properties.Item("ProjectPath").Value as string;
                    }
                }

                if (String.IsNullOrEmpty(fullPath))
                    return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : "";

                if (Directory.Exists(fullPath))
                    return fullPath;
                if (File.Exists(fullPath))
                    return Path.GetDirectoryName(fullPath);

                return "";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetDefaultDestinationFilename(string fileName)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(fileName);
            string extension = "cs";
            return Path.ChangeExtension(baseFileName, extension);
        }

        public static string GetProjectProperty(this Project project, string key)
        {
            try
            {
                return (string)project.Properties.Item(key).Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public static void AddFileToActiveProject(string fileName, string itemType = null)
        {
            Project project = GetActiveProject();

            if (project == null)
                return;
            try
            {
                string projectFilePath = project.Properties.Item("FullPath").Value.ToString();
                string projectDirPath = Path.GetDirectoryName(projectFilePath);

                if (!fileName.StartsWith(projectDirPath, StringComparison.OrdinalIgnoreCase))
                    return;

                ProjectItem item = project.ProjectItems.AddFromFile(fileName);

                if (itemType == null || item == null || project.FullName.Contains("://"))
                    return;


                item.Properties.Item("ItemType").Value = itemType;
            }
            catch { }
        }

        ///<summary>Gets the currently active project (as reported by the Solution Explorer), if any.</summary>
        public static Project GetActiveProject()
        {
            try
            {
                Array activeSolutionProjects = EditProjectPackage.DTE.ActiveSolutionProjects as Array;

                if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
                    return activeSolutionProjects.GetValue(0) as Project;
            }
            catch (Exception ex)
            {
                Debug.Write("Error getting the active project" + ex);
            }

            return null;
        }

        #region ToAbsoluteFilePath()
        ///<summary>Converts a relative URL to an absolute path on disk, as resolved from the specified file.</summary>
        public static string ToAbsoluteFilePath(string relativeUrl, string relativeToFile)
        {
            var file = GetProjectItem(relativeToFile);
            if (file == null || file.Properties == null)
                return ToAbsoluteFilePath(relativeUrl, GetRootFolder(), Path.GetDirectoryName(relativeToFile));
            return ToAbsoluteFilePath(relativeUrl, GetProjectFolder(file), Path.GetDirectoryName(relativeToFile));
        }

        ///<summary>Converts a relative URL to an absolute path on disk, as resolved from the active file.</summary>
        public static string ToAbsoluteFilePathFromActiveFile(string relativeUrl)
        {
            return ToAbsoluteFilePath(relativeUrl, GetActiveFile());
        }

        ///<summary>Converts a relative URL to an absolute path on disk, as resolved from the specified file.</summary>
        private static string ToAbsoluteFilePath(string relativeUrl, ProjectItem file)
        {
            if (file == null)
                return null;

            var baseFolder = file.Properties == null ? null : GetProjectFolder(file);
            return ToAbsoluteFilePath(relativeUrl, GetProjectFolder(file), baseFolder);
        }

        ///<summary>Converts a relative URL to an absolute path on disk, as resolved from the specified relative or base directory.</summary>
        ///<param name="relativeUrl">The URL to resolve.</param>
        ///<param name="projectRoot">The root directory to resolve absolute URLs from.</param>
        ///<param name="baseFolder">The source directory to resolve relative URLs from.</param>
        public static string ToAbsoluteFilePath(string relativeUrl, string projectRoot, string baseFolder)
        {
            string imageUrl = relativeUrl.Trim(new[] { '\'', '"' });
            var relUri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);

            if (relUri.IsAbsoluteUri)
            {
                return relUri.LocalPath;
            }

            if (relUri.OriginalString.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).StartsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                baseFolder = null;
                relUri = new Uri(relUri.OriginalString.Substring(1), UriKind.Relative);
            }

            if (projectRoot == null && baseFolder == null)
                return "";

            var root = (baseFolder ?? projectRoot).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (File.Exists(root))
            {
                root = Path.GetDirectoryName(root);
            }

            if (!root.EndsWith(new string(Path.DirectorySeparatorChar, 1), StringComparison.OrdinalIgnoreCase))
            {
                root += Path.DirectorySeparatorChar;
            }

            try
            {
                var rootUri = new Uri(root, UriKind.Absolute);

                return FixAbsolutePath(new Uri(rootUri, relUri).LocalPath);
            }
            catch (UriFormatException)
            {
                return string.Empty;
            }
        }
        #endregion


        ///<summary>Gets the paths to all files included in the selection, including files within selected folders.</summary>
        public static IEnumerable<string> GetSelectedFilePaths()
        {
            return GetSelectedItemPaths()
                .SelectMany(p => Directory.Exists(p)
                                 ? Directory.EnumerateFiles(p, "*", SearchOption.AllDirectories)
                                 : new[] { p }
                           );
        }


        ///<summary>Gets the full paths to the currently selected item(s) in the Solution Explorer.</summary>
        public static IEnumerable<string> GetSelectedItemPaths(DTE2 dte = null)
        {
            var items = (Array)(dte ?? EditProjectPackage.DTE).ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;

                if (item != null && item.Properties != null)
                    yield return item.Properties.Item("FullPath").Value.ToString();
            }
        }

        ///<summary>Gets the the currently selected project(s) in the Solution Explorer.</summary>
        public static IEnumerable<Project> GetSelectedProjects()
        {
            var items = (Array)EditProjectPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as Project;

                if (item != null)
                    yield return item;
            }
        }

        ///<summary>Attempts to ensure that a file is writable.</summary>
        /// <returns>True if the file is not under source control or was checked out; false if the checkout failed or an error occurred.</returns>
        public static bool CheckOutFileFromSourceControl(string fileName)
        {
            try
            {
                var dte = EditProjectPackage.DTE;

                if (dte == null || !File.Exists(fileName) || dte.Solution.FindProjectItem(fileName) == null)
                    return true;

                if (dte.SourceControl.IsItemUnderSCC(fileName) && !dte.SourceControl.IsItemCheckedOut(fileName))
                    return dte.SourceControl.CheckOutItem(fileName);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Launch();
                return false;
            }
        }

        ///<summary>Gets the directory containing the active solution file.</summary>
        public static string GetSolutionFolderPath()
        {
            EnvDTE.Solution solution = EditProjectPackage.DTE.Solution;

            if (solution == null)
                return null;

            if (string.IsNullOrEmpty(solution.FullName))
                return GetRootFolder();

            return Path.GetDirectoryName(solution.FullName);
        }

        ///<summary>Gets the directory containing the project for the specified file.</summary>
        private static string GetProjectFolder(ProjectItem item)
        {
            if (item == null || item.ContainingProject == null || item.ContainingProject.Collection == null || string.IsNullOrEmpty(item.ContainingProject.FullName)) // Solution items
                return null;

            return GetRootFolder(item.ContainingProject);
        }

        ///<summary>Gets the directory containing the project for the specified file.</summary>
        public static string GetProjectFolder(string fileNameOrFolder)
        {
            if (string.IsNullOrEmpty(fileNameOrFolder))
                return GetRootFolder();

            ProjectItem item = GetProjectItem(fileNameOrFolder);
            string projectFolder = null;

            if (item != null)
                projectFolder = GetProjectFolder(item);

            return projectFolder;
        }

        ///<summary>Gets the the currently selected file(s) in the Solution Explorer.</summary>
        public static IEnumerable<ProjectItem> GetSelectedItems()
        {
            var items = (Array)EditProjectPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;

                if (item != null)
                    yield return item;
            }
        }

        public static string FixAbsolutePath(string absolutePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                return absolutePath;

            var uniformlySeparated = absolutePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var doubleSlash = new string(Path.DirectorySeparatorChar, 2);
            var prependSeparator = uniformlySeparated.StartsWith(doubleSlash, StringComparison.Ordinal);
            uniformlySeparated = uniformlySeparated.Replace(doubleSlash, new string(Path.DirectorySeparatorChar, 1));

            if (prependSeparator)
                uniformlySeparated = Path.DirectorySeparatorChar + uniformlySeparated;

            return uniformlySeparated;
        }

        ///<summary>Gets the Project containing the specified file.</summary>
        public static Project GetProject(string item)
        {
            var projectItem = GetProjectItem(item);

            if (projectItem == null)
                return null;

            return projectItem.ContainingProject;
        }

        public static ProjectItem GetActiveFile()
        {
            var doc = EditProjectPackage.DTE.ActiveDocument;

            if (doc == null)
                return null;

            if (GetProjectFolder(doc.ProjectItem) != null)
                return doc.ProjectItem;

            return null;
        }

        internal static ProjectItem GetProjectItem(string fileName)
        {
            try
            {
                return EditProjectPackage.DTE.Solution.FindProjectItem(fileName);
            }
            catch (Exception exception)
            {

                return null;
            }
        }

        public static ProjectItem AddFileToProject(string parentFileName, string fileName)
        {
            if (Path.GetFullPath(parentFileName) == Path.GetFullPath(fileName) || !File.Exists(fileName))
                return null;

            fileName = Path.GetFullPath(fileName);  // WAP projects don't like paths with forward slashes

            var item = GetProjectItem(parentFileName);

            if (item == null || item.ContainingProject == null || string.IsNullOrEmpty(item.ContainingProject.FullName))
                return null;

            var dependentItem = GetProjectItem(fileName);

            if (dependentItem != null && item.ContainingProject.GetType().Name == "OAProject" && item.ProjectItems != null)
            {
                // WinJS
                ProjectItem addedItem = null;

                try
                {
                    addedItem = dependentItem.ProjectItems.AddFromFile(fileName);

                    // create nesting
                    if (Path.GetDirectoryName(parentFileName) == Path.GetDirectoryName(fileName))
                        addedItem.Properties.Item("DependentUpon").Value = Path.GetFileName(parentFileName);
                }
                catch (COMException) { }
                catch { return dependentItem; }

                return addedItem;
            }

            if (dependentItem != null) // File already exists in the project
                return null;
            else if (item.ContainingProject.GetType().Name != "OAProject" && item.ProjectItems != null && Path.GetDirectoryName(parentFileName) == Path.GetDirectoryName(fileName))
            {   // WAP
                try
                {
                    return item.ProjectItems.AddFromFile(fileName);
                }
                catch (COMException) { }
            }
            else if (Path.GetFullPath(fileName).StartsWith(GetRootFolder(item.ContainingProject), StringComparison.OrdinalIgnoreCase))
            {   // Website
                try
                {
                    return item.ContainingProject.ProjectItems.AddFromFile(fileName);
                }
                catch (COMException) { }
            }

            return null;
        }

        public static bool CreateDirectoryInProject(string path)
        {
            if (!path.StartsWith(GetRootFolder(), StringComparison.OrdinalIgnoreCase))
                return false;

            // Assuming all the files would have extension
            // and all the paths without extensions are directory.
            string directory = string.IsNullOrEmpty(Path.GetExtension(path)) ? path : Path.GetDirectoryName(path);

            Directory.CreateDirectory(directory);

            return true;
        }

        public static string GetAbsolutePathFromSettings(string settingsPath, string filePath)
        {
            if (string.IsNullOrEmpty(settingsPath))
                return filePath;

            string targetFileName = Path.GetFileName(filePath);
            string sourceDir = Path.GetDirectoryName(filePath);

            // If the output path is not project-relative, combine it directly.
            if (!settingsPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase)
             && !settingsPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                return Path.GetFullPath(Path.Combine(sourceDir, settingsPath, targetFileName));

            string rootDir = GetRootFolder();

            if (string.IsNullOrEmpty(rootDir))
                // If no project is loaded, assume relative to file anyway
                rootDir = sourceDir;

            return Path.GetFullPath(Path.Combine(
                rootDir,
                settingsPath.TrimStart('~', '/'),
                targetFileName
            ));
        }

        public static IEnumerable<string> GetBundleConstituentFiles(IEnumerable<string> files, string root, string folder, string bundleFileName)
        {
            foreach (string file in files)
            {
                yield return Path.IsPathRooted(file) ?
                             ToAbsoluteFilePath(file, root, folder) :
                             ToAbsoluteFilePath(file, root, Path.GetDirectoryName(bundleFileName));
            }
        }
    }
}