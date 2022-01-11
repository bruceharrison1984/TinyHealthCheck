using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public class BasicHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> Execute(CancellationToken cancellationToken)
        {
            return new HealthCheckResult
            {
                Body = JsonSerializer.Serialize(new { Status = "Healthy!" }),
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
    }
}
