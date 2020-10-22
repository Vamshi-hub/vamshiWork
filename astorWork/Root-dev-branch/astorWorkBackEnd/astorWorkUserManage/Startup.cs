namespace astorWorkUserManage
{
    using astorWorkDAO;
    using astorWorkShared.Middlewares;
    using astorWorkShared.GlobalModels;
    using astorWorkShared.Services;
    using astorWorkShared.Utilities;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;
    using System.Threading;
    using NSwag.AspNetCore;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDistributedRedisCache(o =>
            {
                o.Configuration = AppConfiguration.GetRedisCacheConn();
            });
            services.AddDbContext<astorWorkDbContext>();
            //services.AddMultitenancy<TenantInfo, TenantResolver>();
            services.AddScoped<TenantInfo>();
            services.AddSingleton<IAstorWorkEmail>(new AstorWorkEmail());
            // Add table storage dependency service
            services.AddSingleton<IAstorWorkTableStorage>(new AstorWorkTableStorage());
            // Add blob storage dependency service
            services.AddSingleton<IAstorWorkBlobStorage>(new AstorWorkBlobStorage());
            // Add PowerBI service
            services.AddSingleton<IAstorWorkPowerBI>(new AstorWorkPowerBI());

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger services
            services.AddSwaggerDocument(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "astorWork User Manage API";
                    document.Info.Description = "APIs in user authentication, management, PowerBI";
                };
            });

            services.AddOptions();
            ConfigureAuth(services);
            services.Configure<Models.Audience>(Configuration.GetSection("Audience"));
        }

        public void Configure(IApplicationBuilder app)
        {
            // Global exception handler must be first middleware
            app.UseGlobalException();

            app.UseAuthentication();
            //app.UseMultitenancy<TenantInfo>();
            app.UseRequestTenant();
            app.UseMvc();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseSwagger();
            app.UseSwaggerUi3();

            // Set minimum threads to avoid Redis cache Timeout
            int minWorker, minIOC;
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(250, minIOC);
        }

        protected virtual void ConfigureAuth(IServiceCollection services)
        {
            var audienceConfig = Configuration.GetSection("Audience");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });
        }
    }
}
