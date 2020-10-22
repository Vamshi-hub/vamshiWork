using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using astorWorkBackEnd;
using astorWorkShared.Utilities;

namespace astorWorkBackEndUnitTest
{
    class TestContext
    {
        private TestServer _server;
        public HttpClient Client { get; private set; }

        public TestContext()
        {
            SetUpClient();
        }

        private void SetUpClient()
        {
            /*
            var apiGateWayServer = new TestServer(GetGatewayHostBuilder());
            var apiGateWayClient = apiGateWayServer.CreateClient();
            var loginData = JsonConvert.SerializeObject(new
            {
                UserName = "admin",
                Password = "abc12345"
            });
            var loginBody = new StringContent(loginData, Encoding.UTF8, "application/json");
            var result = await apiGateWayClient.PostAsync("api/authentication/", loginBody)
            */
            //   new LaunchSettingsFixture();

            AppConfiguration.InitEnvironmentVariables();

            IWebHostBuilder webHostBuilder = GetWebHostBuilder();
            _server = new TestServer(webHostBuilder);
            Client = _server.CreateClient();
        }

        /*
        public static IWebHostBuilder GetGatewayHostBuilder() =>
            WebHost.CreateDefaultBuilder().ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables();
            })
            .UseStartup<APIGateway.Startup>();
            */
        public static IWebHostBuilder GetWebHostBuilder() =>
            WebHost.CreateDefaultBuilder().ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables();
            })
            .UseStartup<Startup>();
    }
   
}
