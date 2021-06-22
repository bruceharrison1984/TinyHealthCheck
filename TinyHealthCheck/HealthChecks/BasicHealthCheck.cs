using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public class BasicHealthCheck : IHealthCheck
    {
        public async Task<string> Execute(CancellationToken cancellationToken)
        {
            return JsonSerializer.Serialize(new { Status = "Healthy!" });
        }
    }
}
