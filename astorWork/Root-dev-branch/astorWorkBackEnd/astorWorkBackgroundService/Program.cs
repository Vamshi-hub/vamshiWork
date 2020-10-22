using astorWorkShared.Services;
using astorWorkShared.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace astorWorkBackgroundService
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                })
            .ConfigureServices((IServiceCollection services) =>
            {
                services.AddDistributedRedisCache(o =>
                {
                    o.Configuration = AppConfiguration.GetRedisCacheConn();
                });

                services.AddLogging(builder => builder.AddConsole());

                services.AddSingleton<IAstorWorkEmail>(new AstorWorkEmail());
                services.AddSingleton<IHostedService, NotificationService>();
                services.AddSingleton<IAstorWorkImport>(new AstorWorkImport());
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            })
            .Build();

            // Set minimum threads to avoid Redis cache Timeout
            int minWorker, minIOC;
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(250, minIOC);

            await host.RunAsync();
        }
    }
}
