using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkDAO.Data;
using astorWorkDAO.DBSchemaSync;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace astorWorkBackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            /*
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var hostingEnv = services.GetRequiredService<IHostingEnvironment>();
                    //if (hostingEnv != null)
                    //{
                    //    var context = services.GetRequiredService<astorWorkDbContext>();
                    //    DbInitializer.Initialize(context);
                    //}
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            */
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
            .UseKestrel(options =>
            {
                // Set max request body size to 50 MB
                options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
            })
            .UseStartup<Startup>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;

            if (isDevelopment)
                builder.UseUrls("http://*:9002");

            return builder.Build();
        }
    }
}
