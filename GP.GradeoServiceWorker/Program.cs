using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace GP.GradeoServiceWorker
{
    public class Program
    {
        public IConfiguration Configuration { get; }

        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                                .WriteTo.RollingFile("Log_{Date}.txt")
                                .MinimumLevel.Debug()                                
                                .Enrich.FromLogContext()
                                .CreateLogger();

            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
                .ConfigureServices((hostContext, services) =>
                {                   
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
    }
}
