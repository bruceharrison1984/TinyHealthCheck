using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TinyHealthCheck.Models;

namespace TinyHealthCheck.HealthChecks
{
    /// <summary>
    /// This health check returns a simple 'Healthy' status, as well as an uptime counter.
    /// </summary>
    public class BasicHealthCheckWithUptime : IHealthCheck
    {
        private DateTimeOffset processStartTime = DateTimeOffset.Now;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <inheritdoc/>
        public async Task<IHealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var responseBody = new
            {
                Status = "Healthy!",
                Uptime = (DateTimeOffset.Now - processStartTime).ToString()
            };

            return new JsonHealthCheckResult(responseBody, HttpStatusCode.OK);
        }
    }
}
