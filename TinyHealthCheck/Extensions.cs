using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;

namespace TinyHealthCheck
{
    public static class Extensions
    {
        public static IServiceCollection AddBasicHealthCheck(this IServiceCollection services)
        {
            return services.AddSingleton<IHostedService>(x => ActivatorUtilities.CreateInstance<HealthCheckService>(x, new object[] { new TinyHealthCheckConfig() }));
        }

        public static IServiceCollection AddBasicHealthCheckWithUptime(this IServiceCollection services)
        {
            var processStartTime = DateTimeOffset.Now;
            var config = new TinyHealthCheckConfig();
            config.HealthCheckFunction = async x => JsonSerializer.Serialize(new { Status = "Healthy!", Uptime = (DateTimeOffset.Now - processStartTime).ToString() });

            return services.AddSingleton<IHostedService>(x => ActivatorUtilities.CreateInstance<HealthCheckService>(x, config));
        }
    }
}
