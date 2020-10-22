// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
            services.AddSignalR()
                    .AddAzureSignalR(options =>
                    {
                        options.ConnectionString = "Endpoint=https://astorwork.service.signalr.net;AccessKey=G9U2b4Vsq1ilXZH3jQRcoeBX1X/Ma395SacX7QXzob4=;Version=1.0;";
                        //options.ConnectionString = "Endpoint=https://astorworkqa.service.signalr.net;AccessKey=pxtuseVmjR6ac1mmEJzEt0yCdnbzuMyJDhDxu8Gmq2I=;Version=1.0;";
                        //options.ConnectionString = "Endpoint=https://astorworkdev.service.signalr.net;AccessKey=AEbxFU0lkYvbL3xoj+z/RuvvY7L/T2pWiKVohv+worw=;Version=1.0;";
                    });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.UseMvc();
            app.UseCors(cors =>
            {
                cors.AllowAnyHeader();
                cors.AllowAnyOrigin();
                cors.AllowAnyMethod();
            });
            app.UseFileServer();
            app.UseWebSockets();
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
                app.UseHsts();
            //}
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<Chat>("/chat");
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //   app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
