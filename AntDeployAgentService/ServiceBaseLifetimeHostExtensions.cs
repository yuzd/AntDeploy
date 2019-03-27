using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace AntDeployAgentService
{
    public static class ServiceBaseLifetimeHostExtensions
    {
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static void RunAsService(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseServiceBaseLifetime().Build().Run();
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }
    }
}
