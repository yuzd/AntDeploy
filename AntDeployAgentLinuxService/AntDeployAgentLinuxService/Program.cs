using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using AntDeployAgentWindows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TinyFox;

namespace AntDeployAgentLinuxService
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);

#if !DEBUG
            Startup.RootPath = pathToContentRoot;
            TinyFoxService.WebRoot = Path.Combine(pathToContentRoot,"wwwroot");
            ConfigurationManager.Initialize(pathToExe);
#endif
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if !DEBUG
            .UseSystemd()
#endif

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AntDeployAgentService>();
                });
    }
}
