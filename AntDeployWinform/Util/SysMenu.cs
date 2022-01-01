using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            list = JumpList.CreateJumpListForIndividualWindow(TaskbarManager.Instance.ApplicationId, windowHandle);
            list.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
            BuildList();
        }

        public void AddToRecent(string destination)
        {
            list.AddToRecent(destination);
            list.Refresh();
        }

        /// <summary>
        /// Builds the Jumplist
        /// </summary>
        private void BuildList()
        {
            JumpListCustomCategory userActionsCategory = new JumpListCustomCategory("Actions");
            JumpListLink userActionLink = new JumpListLink(Assembly.GetEntryAssembly().Location, "Help & Github");
            userActionLink.Arguments = "help";

            userActionsCategory.AddJumpListItems(userActionLink);
            list.AddCustomCategories(userActionsCategory);


            list.Refresh();
        }
    }
}
