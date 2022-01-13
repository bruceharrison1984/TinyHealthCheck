using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TinyHealthCheck.HealthChecks;
using TinyHealthCheck.Models;

namespace TinyHealthCheck
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a simple health check endpoint that returns a JSON payload
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configFunc">Custom TinyHealthCheck configuration object</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddBasicTinyHealthCheck(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configFunc)
        {
            return services.AddCustomTinyHealthCheck<BasicHealthCheck>(configFunc);
        }

        /// <summary>
        /// Adds a simple health check endpoint that returns a JSON payload with an Uptime counter
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configFunc">Custom TinyHealthCheck configuration object</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddBasicTinyHealthCheckWithUptime(this IServiceCollection services, Func<TinyHealthCheckConfig, TinyHealthCheckConfig> configFunc)
        {
            return services.AddCustomTinyHealthCheck<BasicHealthCheckWithUptime>(configFunc);
        }

        /// <summary>
        /// Adds a custom health check endpoint that return any type of payload
        /// </summary>
        /// <typeparam name="T">The custom health check type</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="configFunc">Custom TinyHealthCheck configuration object</param>
        /// <returns>Service Collection</returns>
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
