using AntDeployWinform.Winform;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp
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
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            Application.Run(new Deploy());
            //@"d:\Users\zdyu\source\repos\WebApplication3\WebApplication3\WebApplication3.csproj", new ProjectParam()
            //Application.Run(new Deploy(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\Lito.APP.csproj", null));
            //Application.Run(new Deploy(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\Lito.APP.csproj", null));
        }
    }
}
