using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TinyHealthCheck.Models;

namespace TinyHealthCheck.HealthChecks
{
    /// <summary>
    /// This health check returns a simple 'Healthy' status
    /// </summary>
    public class BasicHealthCheck : IHealthCheck
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <inheritdoc/>
        public async Task<IHealthCheckResult> ExecuteAsync(CancellationToken cancellationToken) =>
            new JsonHealthCheckResult(new { Status = "Healthy!" }, HttpStatusCode.OK);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
