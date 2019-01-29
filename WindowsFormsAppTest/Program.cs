using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntDeploy.Models;
using AntDeploy.Winform;

namespace WindowsFormsAppTest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Deploy("E:\\WorkSpace\\github\\AntDeploy\\AntDeployAgentWindowsService\\AntDeployAgentWindowsService.csproj",null));
        }
    }
}
