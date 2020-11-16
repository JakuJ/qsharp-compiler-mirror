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
            // Note(JJ): This workaround makes the server work with our Docker configuration
            VisualStudioInstance vsi = MSBuildLocator.RegisterDefaults();

            // https://github.com/microsoft/qsharp-compiler/pull/566
            // We're using the installed version of the binaries to avoid a dependency between
            // the .NET Core SDK version and NuGet. This is a workaround due to the issue below:
            // https://github.com/microsoft/MSBuildLocator/issues/86
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
                     // This environment variable is set when running on Heroku
                     string port = Environment.GetEnvironmentVariable("PORT") ?? "8091";
                     webBuilder.UseStartup<Startup>().UseUrls($"http://*:{port}");
                 });
    }
}
