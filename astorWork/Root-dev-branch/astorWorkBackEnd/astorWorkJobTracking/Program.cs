using astorWorkDAO.DBSchemaSync;
using astorWorkJobTracking;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace astorWorkJobTracking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;
            if (!isDevelopment)
                Task.Run(DBSchemaSync.MigrateAll);

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables();
            })
            .ConfigureKestrel((context, options) =>
            {
                // Set max request body size to 50 MB
                options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
            })
            .UseStartup<Startup>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;

            if (isDevelopment)
                builder.UseUrls("http://*:9003");

            return builder.Build();
        }
    }
}
