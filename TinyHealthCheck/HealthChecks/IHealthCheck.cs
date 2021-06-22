using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public interface IHealthCheck
    {
        Task<string> Execute(CancellationToken cancellationToken);
    }
}