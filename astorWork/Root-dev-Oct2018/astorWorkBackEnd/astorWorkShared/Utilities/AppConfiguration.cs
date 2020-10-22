using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace astorWorkShared.Utilities
{
    public class AppConfiguration
    {
        public static void InitEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("SENDGRID_KEY", "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");
            Environment.SetEnvironmentVariable("AW_REDIS_CACHE_CONNECTION","astorwork-dev.redis.cache.windows.net:6380,password=0EJ/NfXOQuCua+UgT8IXDTcGI5kz8MoDoUb05LcRifc=,ssl=True,abortConnect=False");
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("TENANT_INFO_TBL", "TenantInfoQA");
            Environment.SetEnvironmentVariable("IMAGE_CONTAINER", "dev-image-repo");
            Environment.SetEnvironmentVariable("IMAGE_CONTAINER", "dev-image-repo");
            Environment.SetEnvironmentVariable("FORGE_CLIENT_SECRET", "zVqsakG4x46EpD8V");
            Environment.SetEnvironmentVariable("FORGE_CLIENT_ID", "6AumZPf6sBW6pOvwpQkfXlq86I8l9JGl");
            Environment.SetEnvironmentVariable("POWERBI_TENANT_ID", "3156e991-a773-429e-a59a-df6faa02e474");
            Environment.SetEnvironmentVariable("POWERBI_CLIENT_ID", "1d67d1a2-09ba-4994-b019-a926d4150004");
            Environment.SetEnvironmentVariable("POWERBI_CLIENT_SECRET", "8qpSgFNpj/YFd4viPXqEzdklEuH8v6/mRl3VkP0nvfM=");
            Environment.SetEnvironmentVariable("POWERBI_USER_NAME", "powerbiadmin@astoriasolutions.com");
            Environment.SetEnvironmentVariable("POWERBI_USER_PASSWORD", "P@werbi@dmin123#");
        }

        public static readonly Dictionary<int,List<string>> RecuringNotification = new Dictionary<int, List<string>>
        {
            {2, new List<string>{"Delay In Delivery", "Daily notification for materials not delivered" } },
            {8, new List<string>{ "Expected Delivery", " Daily notification for expected materials" } }
        };
        private static IConfiguration Configuration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(buildDir, "SharedConfig.json");
            configurationBuilder.AddJsonFile(path, false);
            return configurationBuilder.Build();
        }

        public static string GetCloudStorageAccount()
        {
            return Configuration().GetConnectionString("CloudStorageAccount");
        }

        public static string GetDefaultSQLConn()
        {
            return Configuration().GetConnectionString("DevSQLConnectionstring");
        }

        public static string GetSendGridKey()
        {
            return Environment.GetEnvironmentVariable("SENDGRID_KEY");
        }

        public static string GetTenantTableName()
        {
            return Environment.GetEnvironmentVariable("TENANT_INFO_TBL");
           
        }
        public static bool IsDevEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;
            return isDevelopment;
        }
        public static string GetRedisCacheConn()
        {
            return Environment.GetEnvironmentVariable("AW_REDIS_CACHE_CONNECTION");
        }

        public static string GetForgeClientId()
        {
            return Environment.GetEnvironmentVariable("FORGE_CLIENT_ID");
        }

        public static string GetForgeClientSecret()
        {
            return Environment.GetEnvironmentVariable("FORGE_CLIENT_SECRET");
        }
        public static string GetQCContainerName()
        {
           return Environment.GetEnvironmentVariable("IMAGE_CONTAINER");
        }
        public static string GetVideoContainer()
        {
            return Environment.GetEnvironmentVariable("VIDEO_CONTAINER");
        }
        public static string GetHostName()
        {
            return Environment.GetEnvironmentVariable("HOST_NAME");
        }

        public static PowerBICredentials GetPowerBICredentials()
        {
            return new PowerBICredentials
            {
                ClientId = Environment.GetEnvironmentVariable("POWERBI_CLIENT_ID"),
                ClientSecret = Environment.GetEnvironmentVariable("POWERBI_CLIENT_SECRET"),
                UserName = Environment.GetEnvironmentVariable("POWERBI_USER_NAME"),
                Password = Environment.GetEnvironmentVariable("POWERBI_USER_PASSWORD"),
                TenantId = Environment.GetEnvironmentVariable("POWERBI_TENANT_ID")
            };
        }
    }

    public class PowerBICredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TenantId { get; set; }
    }
}
