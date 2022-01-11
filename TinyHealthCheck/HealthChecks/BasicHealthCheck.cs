using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public class BasicHealthCheck : IHealthCheck
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return new HealthCheckResult
            {
                Body = JsonSerializer.Serialize(new { Status = "Healthy!" }),
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
    }
}
