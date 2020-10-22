using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace astorWorkGateWay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseKestrel(options =>
                {
                    // Set max request body size to 50 MB
                    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile($"configuration.{hostingContext.HostingEnvironment.EnvironmentName}.json")
                    .AddEnvironmentVariables();

                    Console.WriteLine("Environment: " + hostingContext.HostingEnvironment.EnvironmentName);
                })
                .UseStartup<Startup>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;

            if (isDevelopment)
                builder.UseUrls("http://*:9000");


            return builder;
        }
    }
}
