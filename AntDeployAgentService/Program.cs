using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
#if NETSTANDARD
using System.Configuration;
#endif
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AntDeployAgentWindows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using TinyFox;

namespace AntDeployAgentService
{
    class Program
    {
        // P/Invoke declarations for Windows.
        [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hWnd);

        // Indicates if the current process is running:
        //  * on Windows: in a console window visible to the user.
        //  * on Unix-like platform: in a terminal window, which is assumed to imply
        //    user-visibility (given that hidden processes don't need terminals).
        public static bool HaveVisibleConsole()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                          IsWindowVisible(GetConsoleWindow())
                          :
                          Console.WindowHeight > 0;
        }

        private static  void Main(string[] args)
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);

#if NETSTANDARD
            Startup.RootPath = pathToContentRoot;
            TinyFoxService.WebRoot = Path.Combine(pathToContentRoot,"wwwroot");
            ConfigurationManager.Initialize(pathToExe);
#endif
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (HaveVisibleConsole()) isService = false;

           
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure => configure.AddConsole());
                    services.AddHostedService<AntDeployAgentWindowsService>();
                });

            if (isService)
            {
                 builder.UseWindowsService().Build().Run();
            }
            else
            {
                Console.WriteLine("Current Version：" + AntDeployAgentWindows.Version.VERSION); 
                builder.Build().Run();
            }
        }


    }


}
