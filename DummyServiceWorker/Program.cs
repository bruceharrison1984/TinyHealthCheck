using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

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
                    services.AddHostedService<TinyHealthCheck.TinyHealthCheck>(sp =>
                    {
                        return new TinyHealthCheck.TinyHealthCheck(
                            logger: sp.GetRequiredService<ILogger<TinyHealthCheck.TinyHealthCheck>>(),
                            hostname: "*",
                            port: 8081,
                            contentType: "application/json",
                            urlPath: "healthz",
                            healthCheckFunction: async cancellationToken => JsonSerializer.Serialize(new { Status = "Healthy!", Uptime = (DateTimeOffset.Now - processStartTime).ToString() }));
                    });
                });
        }
    }
}
