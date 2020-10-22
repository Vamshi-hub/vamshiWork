using System;
using System.Text;
using astorWorkShared.GlobalModels;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using astorWorkShared.Services;
using System.Threading;
using astorWorkShared.Middlewares;

namespace astorWorkGateWay
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            HostingEnvironment = env;
            Configuration = config;
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
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
            
            services.AddDistributedRedisCache(o =>
            {
                o.Configuration = AppConfiguration.GetRedisCacheConn();
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
                x.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {             
                    OnTokenValidated = ctx =>
                    {
                        var redisCache = ctx.HttpContext.RequestServices.GetService<IDistributedCache>();
                        var tenant = ctx.HttpContext.RequestServices.GetService<TenantInfo>();
                        var token = ctx.SecurityToken as JwtSecurityToken;

                        try
                        {
                            var tokenStr = redisCache.GetString(token.RawData);

                            if (string.IsNullOrEmpty(tokenStr))
                                ctx.Fail("Token does not exist");
                            else
                                ctx.Success();
                        }
                        catch(Exception exc)
                        {
                            ctx.Fail(exc.Message);
                            Console.WriteLine(exc.Message);
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSingleton<IAstorWorkTableStorage>(new AstorWorkTableStorage());
            //services.AddMultitenancy<TenantInfo, TenantResolver>();
            services.AddScoped<TenantInfo>();

            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Global exception handler must be first middleware
            // app.UseGlobalException();
            app.UseRequestTenant();
            app.UseGlobalResponse();
            
            
            //app.UseMultitenancy<TenantInfo>();

            /*
            var configuration = new OcelotPipelineConfiguration
            {
                PreAuthenticationMiddleware = async (ctx, next) =>
                {
                    if (ctx.DownstreamReRoute.IsAuthenticated)
                    {
                        var token = await ctx.HttpContext.GetTokenAsync("access_token");
                        var result = await redisCache.GetAsync(token);
                        ctx.HttpContext.Request.Headers.Clear();
                    }

                    await next.Invoke();
                }
            };
            app.UseOcelot(configuration);
            */
            app.UseOcelot();
            
            // Set minimum threads to avoid Redis cache Timeout
            int minWorker, minIOC;
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(250, minIOC);
        }
    }
}
