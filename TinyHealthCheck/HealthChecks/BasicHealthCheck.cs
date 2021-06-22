using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public class BasicHealthCheck : IHealthCheck
    {
        public BasicHealthCheck(IServiceProvider serviceProvider)
        {

        }

        public async Task<string> Execute(CancellationToken cancellationToken)
        {
            return JsonSerializer.Serialize(new { Status = "Healthy!" });
        }
    }
}
