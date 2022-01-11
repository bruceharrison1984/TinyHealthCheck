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
                    services.AddSingleton<WorkerStateService>();
                    services.AddHostedService<Worker>();
                    services.AddBasicTinyHealthCheck(config =>
                    {
                        config.Port = 8080;
                        return config;
                    });
                    services.AddBasicTinyHealthCheckWithUptime(config =>
                    {
                        config.Port = 8081;
                        return config;
                    });
                    services.AddCustomTinyHealthCheck<CustomHealthCheck>(config =>
                    {
                        config.Port = 8082;
                        return config;
                    });
                });
        }

        public class CustomHealthCheck : IHealthCheck
        {
            private readonly ILogger<CustomHealthCheck> _logger;
            private readonly WorkerStateService _workerStateService;
            //IHostedServices cannot be reliably retrieved from the DI collection
            //A secondary stateful service is required in order to get state information out of it
            //https://stackoverflow.com/a/52038409/889034

            public CustomHealthCheck(ILogger<CustomHealthCheck> logger, WorkerStateService workerStateService)
            {
                _logger = logger;
                _workerStateService = workerStateService;
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                _logger.LogInformation("This is an example of accessing the DI containers for logging. You can access any service that is registered");

                if (_workerStateService.IsRunning)
                    return new HealthCheckResult
                    {
                        Body = JsonSerializer.Serialize(new
                        {
                            Status = "Healthy!",
                            Iteration = _workerStateService.Iteration,
                            IsServiceRunning = _workerStateService.IsRunning,
                        }),
                        StatusCode = System.Net.HttpStatusCode.OK
                    };

                return new HealthCheckResult
                {
                    Body = JsonSerializer.Serialize(new
                    {
                        Status = "Unhealthy!",
                        Iteration = _workerStateService.Iteration,
                        IsServiceRunning = _workerStateService.IsRunning,
                        ErrorMessage = "We went over 10 iterations, so the service worker quit!"
                    }),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
