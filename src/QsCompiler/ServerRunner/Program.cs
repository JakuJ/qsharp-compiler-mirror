using System;
using System.IO;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Hosting;

namespace ServerRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // This workaround makes the server work with our Docker configuration
            // Taken from ../LanguageServer/Program.cs
            VisualStudioInstance vsi = MSBuildLocator.RegisterDefaults();

            AssemblyLoadContext.Default.Resolving += (assemblyLoadContext, assemblyName) =>
            {
                string path = Path.Combine(vsi.MSBuildPath, assemblyName.Name + ".dll");
                return File.Exists(path) ? assemblyLoadContext.LoadFromAssemblyPath(path) : null;
            };

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();

                     string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                     switch (environment)
                     {
                         case "Staging":
                             string port = Environment.GetEnvironmentVariable("PORT")!; // provided by the platform, never null
                             webBuilder.UseUrls($"http://*:{port}");                    // https is managed automatically
                             break;
                         case "Production":
                             webBuilder.UseUrls("http://*:80", "https://*:443"); // production defaults
                             break;
                         case "Development":
                             webBuilder.UseUrls("http://*:8091"); // default port for local development
                             break;
                     }
                 });
    }
}
