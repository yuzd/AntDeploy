using AntDeployAgentWindows;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TinyFox;

namespace AntDeployAgentService
{
    public class AntDeployAgentWindowsService : IHostedService, IDisposable
    {
        private Startup _startup;
        private ILogger<AntDeployAgentWindowsService> _logger;

        public AntDeployAgentWindowsService(ILogger<AntDeployAgentWindowsService> logger)
        {
            _logger = logger;
        }
        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var port = System.Configuration.ConfigurationManager.AppSettings["Port"];
                TinyFoxService.IpAddress = TyeIpAddress.Any;
                TinyFoxService.Port = string.IsNullOrEmpty(port) ? 8088 : int.Parse(port);
                _startup = new AntDeployAgentWindows.Startup();
                TinyFoxService.Start(_startup.OwinMain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"StartError");
                try
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"startupError.txt"), ex.ToString());
                }
                catch (Exception)
                {

                }
            }

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _startup?.Stop();
            TinyFoxService.Stop();
            return Task.CompletedTask;
        }


    }
}
