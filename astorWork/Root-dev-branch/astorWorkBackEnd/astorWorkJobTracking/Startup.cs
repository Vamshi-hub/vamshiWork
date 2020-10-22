using astorWorkDAO;
using astorWorkShared.GlobalModels;
using astorWorkShared.Middlewares;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag.AspNetCore;
using System;
using System.Text;
using System.Threading;

namespace astorWorkJobTracking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(o =>
            {
                o.Configuration = AppConfiguration.GetRedisCacheConn();
            });
            services.AddDbContext<astorWorkDbContext>();
            //services.AddMultitenancy<TenantInfo, TenantResolver>();
            services.AddScoped<TenantInfo>();
            // Add table storage dependency service
            services.AddSingleton<IAstorWorkTableStorage>(new AstorWorkTableStorage());
            // Add Blob storage dependency service
            services.AddSingleton<IAstorWorkBlobStorage>(new AstorWorkBlobStorage());
            services.AddSingleton<IAstorWorkEmail>(new AstorWorkEmail());
            services.AddSingleton<IAstorWorkImport>(new AstorWorkImport());
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            // Register the Swagger services
            services.AddSwaggerDocument(settings =>
            {
                settings.DocumentName = "v1";
                settings.Title = "astorWork Job Track API";
                settings.Description = "APIs in job tracking";
            });

            ConfigureAuth(services);
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //    app.UseDeveloperExceptionPage();
            // Global exception handler must be first middleware
            app.UseGlobalException();

            app.UseAuthentication();
            //app.UseMultitenancy<TenantInfo>();

            app.UseRequestTenant();
            app.UseMvc();

            // Set default files and static files
            app.UseFileServer();
            // Register the Swagger generator and the Swagger UI middlewares
            var swaggerBaseUrl = "/jt-doc";
            app.UseSwagger(c =>
            {
                c.Path = swaggerBaseUrl + "/{documentName}/swagger.json";
            });
            app.UseSwaggerUi3(c =>
            {
                c.Path = swaggerBaseUrl;
                c.DocumentPath = swaggerBaseUrl + "/{documentName}/swagger.json";
            });


            // Set minimum threads to avoid Redis cache Timeout
            int minWorker, minIOC;
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(250, minIOC);
        }
    }
}
