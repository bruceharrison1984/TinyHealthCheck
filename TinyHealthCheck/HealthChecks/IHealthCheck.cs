using System.Threading;
using System.Threading.Tasks;
using TinyHealthCheck.Models;

namespace TinyHealthCheck.HealthChecks
{
    /// <summary>
    /// Custom health check classes must inheirit from this interface
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// This method will be invoked when a client accesses the health check endpoint
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to stop the listener</param>
        /// <returns><see cref="Task{T}"/> with a result of <see cref="HealthCheckResult"/></returns>
        Task<IHealthCheckResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}