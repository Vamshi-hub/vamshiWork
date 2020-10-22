namespace astorWorkUserManage
{
    using astorWorkDAO;
    using astorWorkShared.MultiTenancy;
    using astorWorkShared.Services;
    using astorWorkShared.Utilities;
    using astorWorkUserManage.Common;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;
    using System.Threading;

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
            services.AddMultitenancy<TenantInfo, TenantResolver>();
            services.AddSingleton<IAstorWorkEmail>(new AstorWorkEmail());
            // Add table storage dependency service
            services.AddSingleton<IAstorWorkTableStorage>(new AstorWorkTableStorage());
            // Add blob storage dependency service
            services.AddSingleton<IAstorWorkBlobStorage>(new AstorWorkBlobStorage());
            // Add PowerBI service
            services.AddSingleton<IAstorWorkPowerBI>(new AstorWorkPowerBI());

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();
            ConfigureAuth(services);
            services.Configure<Models.Audience>(Configuration.GetSection("Audience"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMultitenancy<TenantInfo>();
            app.UseMvc();

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
