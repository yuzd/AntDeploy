using AntDeployWinform.Models;
using AntDeployWinform.Winform;
using System;
using System.IO;
using System.Windows.Forms;

namespace AntDeployApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");

            if (args.Length == 2)
            {
                if (File.Exists(args[1]))
                {
                    var fileInfo = File.ReadAllText(args[1]);
                    if (string.IsNullOrEmpty(fileInfo))
                    {
                        Application.Run(new Deploy());
                    }
                    else
                    {
                        try
                        {
                            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectParam>(fileInfo);
                            Application.Run(model != null ? new Deploy(model.ProjectPath, model) : new Deploy());
                        }
                        catch (Exception)
                        {

                            Application.Run(new Deploy());
                        }
                    }
                }
                else
                {
                    Application.Run(new Deploy());
                }

            }
            else
            {
                Application.Run(new Deploy());
            }

            //@"d:\Users\zdyu\source\repos\WebApplication3\WebApplication3\WebApplication3.csproj", new ProjectParam()
            //Application.Run(new Deploy(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\Lito.APP.csproj", null));
            //Application.Run(new Deploy(@"E:\WorkSpace\github\Lito\Lito\Lito.APP\Lito.APP.csproj", null));
        }
    }
}
