using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AntDeployWinform.Util
{
    /// <summary>
    /// Extends the system menu of a window with additional commands.
    /// </summary>
    public class SystemMenu
    {
        private JumpList list;

        /// <summary>
        /// Creating a JumpList for the application
        /// </summary>
        /// <param name="windowHandle"></param>
        public SystemMenu(IntPtr windowHandle)
        {
            list = JumpList.CreateJumpListForIndividualWindow(windowHandle.ToString(), windowHandle);
            list.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
            BuildList();
        }

        public void AddToRecent(string destination)
        {
            list.AddToRecent(destination);
            list.Refresh();
        }

        public void AddToRecentList(List<string> projectPathList)
        {
            try
            {
                if (projectPathList == null || !projectPathList.Any()) return;
                projectPathList = projectPathList.Distinct().ToList();
                JumpListCustomCategory userActionsCategory = new JumpListCustomCategory("Recent");
                var rt = new List<JumpListLink>();
                foreach (string projectPath in projectPathList)
                {
                    if (string.IsNullOrEmpty(projectPath))
                    {
                        continue;
                    }

                    if (File.Exists(projectPath) || Directory.Exists(projectPath))
                    {
                        JumpListLink userActionLink = new JumpListLink(Assembly.GetEntryAssembly().Location, new FileInfo(projectPath).Name);
                        userActionLink.Arguments = projectPath;
                        rt.Add(userActionLink);
                    }

                }
                if (!rt.Any()) return;
                userActionsCategory.AddJumpListItems(rt.ToArray());
                list.AddCustomCategories(userActionsCategory);
                list.Refresh();
            }
            catch (Exception)
            {
                //有的系统不支持
            }

        }

        /// <summary>
        /// Builds the Jumplist
        /// </summary>
        private void BuildList()
        {
            try
            {
                JumpListCustomCategory userActionsCategory = new JumpListCustomCategory("Actions");
                JumpListLink userActionLink = new JumpListLink(Assembly.GetEntryAssembly().Location, "Help & Github");
                userActionLink.Arguments = "help";
                userActionsCategory.AddJumpListItems(userActionLink);
                list.AddCustomCategories(userActionsCategory);
                list.Refresh();
            }
            catch (Exception)
            {
                //有的系统不支持
            }
           
        }
    }
}
