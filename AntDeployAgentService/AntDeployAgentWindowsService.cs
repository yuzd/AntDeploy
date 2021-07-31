using AntDeployAgentWindows;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TinyFox;

namespace AntDeployAgentService
{
    public class AntDeployAgentWindowsService : BackgroundService
    {
        private Startup _startup;
        private readonly ILogger<AntDeployAgentWindowsService> _logger;

        public AntDeployAgentWindowsService(ILogger<AntDeployAgentWindowsService> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _startup?.Stop();
            TinyFoxService.Stop();
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                ExposeNssmExe();
                var port = System.Configuration.ConfigurationManager.AppSettings["Port"];
                TinyFoxService.OwinOnly = true;
                TinyFoxService.IpAddress = TyeIpAddress.Any;
                TinyFoxService.Port = string.IsNullOrEmpty(port) ? 8088 : int.Parse(port);
                _startup = new AntDeployAgentWindows.Startup();
                TinyFoxService.Start(_startup.OwinMain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StartError");
                try
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startupError.txt"), ex.ToString());
                }
                catch (Exception)
                {

                }
            }

            return Task.CompletedTask;

        }


        protected void ExposeNssmExe()
        {
            var nssmSavePath = Path.Combine(Startup.RootPath, "nssm.exe");
            if (File.Exists(nssmSavePath))
            {
                return;
            }
            Assembly assembly = typeof(AntDeployAgentWindowsService).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployAgentService.nssm.exe"))
            {
                if (stream != null)
                {
                    using (var fileStream = File.Create(nssmSavePath))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                    }
                }
            }

            if (!File.Exists(nssmSavePath))
            {
                throw new FileNotFoundException(nssmSavePath + " not found");
            }
        }
    }
}
