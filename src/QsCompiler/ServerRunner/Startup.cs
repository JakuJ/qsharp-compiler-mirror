using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServerRunner
{
    [SuppressMessage("Documentation", "SA1600", Justification = "Boilerplate")]
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Heroku closes connections that are inactive for more than 55 seconds
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(45)
            };

            app.UseWebSockets(webSocketOptions)
               .UseRouting()
               .UseEndpoints(endpoints => { endpoints.MapStreamJsonRpc("/monaco-editor"); })
               .Run(async context =>
                {
                    await context.Response.WriteAsync("-- You're looking for the WebSocket endpoint for the Q# Language Server on port 8091 --");
                });
        }
    }
}
