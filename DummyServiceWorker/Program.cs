using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TinyHealthCheck;
using TinyHealthCheck.HealthChecks;

namespace DummyServiceWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var processStartTime = DateTimeOffset.Now;
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddBasicTinyHealthCheck(config =>
                    {
                        config.Hostname = "*";
                        config.Port = 8080;
                        return config;
                    });
                    services.AddBasicTinyHealthCheckWithUptime(config =>
                    {
                        config.Hostname = "*";
                        config.Port = 8081;
                        return config;
                    });
                    services.AddCustomTinyHealthCheck<CustomHealthCheck>(config =>
                    {
                        config.Hostname = "*";
                        config.Port = 8082;
                        return config;
                    });
                });
        }

        public class CustomHealthCheck : IHealthCheck
        {
            private readonly ILogger<CustomHealthCheck> _logger;

            public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
            {
                _logger = logger;
            }

            public async Task<string> Execute(CancellationToken cancellationToken)
            {
                _logger.LogInformation("This is an example of accessing the DI containers for logging. You can access any service that is registered");
                return JsonSerializer.Serialize(new { Status = "Healthy!", CustomValue = "SomeValueFromServices" });
            }
        }
    }
}
