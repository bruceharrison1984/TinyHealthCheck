using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public class BasicHealthCheckWithUptime : IHealthCheck
    {
        private DateTimeOffset processStartTime = DateTimeOffset.Now;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var responseBody = new
            {
                Status = "Healthy!",
                Uptime = (DateTimeOffset.Now - processStartTime).ToString()
            };

            return new HealthCheckResult
            {
                Body = JsonSerializer.Serialize(responseBody),
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
    }
}
