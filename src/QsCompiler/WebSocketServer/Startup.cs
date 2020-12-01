using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebSocketServer
{
    [SuppressMessage("Documentation", "SA1600", Justification = "Boilerplate")]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Heroku closes connections that are inactive for more than 55 seconds
                var webSocketOptions = new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(50),
                };

                app.UseWebSockets(webSocketOptions);
            }
            else
            {
                app.UseWebSockets();
            }

            app.UseRouting()
               .UseEndpoints(endpoints => { endpoints.MapStreamJsonRpc("/monaco-editor"); });
        }
    }
}
