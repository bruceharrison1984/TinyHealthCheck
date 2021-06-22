using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TinyHealthCheck.HealthChecks;

namespace TinyHealthCheck
{
    public static class Extensions
    {
        public static IServiceCollection AddBasicTinyHealthCheck(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configFunc)
        {
            return services.AddCustomTinyHealthCheck<BasicHealthCheck>(configFunc);
        }

        public static IServiceCollection AddBasicTinyHealthCheckWithUptime(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configFunc)
        {
            return services.AddCustomTinyHealthCheck<BasicHealthCheckWithUptime>(configFunc);
        }

        public static IServiceCollection AddCustomTinyHealthCheck<T>(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configFunc) where T : IHealthCheck
        {
            return services.AddSingleton<IHostedService>(x =>
            {
                var healthCheck = ActivatorUtilities.CreateInstance<T>(x);
                return ActivatorUtilities.CreateInstance<HealthCheckService<T>>(x, healthCheck, configFunc(new TinyHealthCheckConfig()));
            });
        }
    }
}
