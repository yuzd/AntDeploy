using AntDeployAgentWindows;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TinyFox;

namespace AntDeployAgentService
{
    public class AntDeployAgentWindowsService : IHostedService, IDisposable
    {
        private Startup _startup;

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var port = System.Configuration.ConfigurationManager.AppSettings["Port"];
                TinyFoxService.Port = string.IsNullOrEmpty(port) ? 8088 : int.Parse(port);
                _startup = new AntDeployAgentWindows.Startup();
                TinyFoxService.Start(_startup.OwinMain);
            }
            catch (Exception ex)
            {
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
