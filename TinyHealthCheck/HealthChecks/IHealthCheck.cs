using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public interface IHealthCheck
    {
        Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}