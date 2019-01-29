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
            Application.Run(new Deploy(@"d:\Users\zdyu\source\repos\ConsoleApp2\ConsoleApp2\ConsoleApp2.csproj",null));
        }
    }
}
