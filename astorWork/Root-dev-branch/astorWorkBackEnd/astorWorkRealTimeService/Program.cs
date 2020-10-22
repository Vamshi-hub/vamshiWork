using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace astorWorkRealTimeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;

            if (isDevelopment)
                builder.UseUrls("http://*:9003");

            return builder;
        }
    }
}
