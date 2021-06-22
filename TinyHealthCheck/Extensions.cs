using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TinyHealthCheck.HealthChecks;

namespace TinyHealthCheck
{
    public static class Extensions
    {
        public static IServiceCollection AddBasicTinyHealthCheck(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configAction)
        {
            return services.AddCustomTinyHealthCheck<BasicHealthCheck>(configAction);
        }

        public static IServiceCollection AddBasicTinyHealthCheckWithUptime(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configAction)
        {
            return services.AddCustomTinyHealthCheck<BasicHealthCheckWithUptime>(configAction);
        }

        public static IServiceCollection AddCustomTinyHealthCheck<T>(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configAction) where T : IHealthCheck, new()
        {
            return services.AddSingleton<IHostedService>(x => ActivatorUtilities.CreateInstance<HealthCheckService<T>>(x, configAction(new TinyHealthCheckConfig())));
        }
    }
}
