using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck.HealthChecks
{
    public class BasicHealthCheckWithUptime : IHealthCheck
    {
        private DateTimeOffset processStartTime = DateTimeOffset.Now;

        public async Task<string> Execute(CancellationToken cancellationToken)
        {
            var responseBody = new
            {
                Status = "Healthy!",
                Uptime = (DateTimeOffset.Now - processStartTime).ToString()
            };

            return JsonSerializer.Serialize(responseBody);
        }
    }
}
