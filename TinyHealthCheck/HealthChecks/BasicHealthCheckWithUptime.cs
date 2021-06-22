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

        public async Task<HealthCheckResult> Execute(CancellationToken cancellationToken)
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
