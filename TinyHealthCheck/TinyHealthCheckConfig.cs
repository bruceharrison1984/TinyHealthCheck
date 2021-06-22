using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck
{
    public class TinyHealthCheckConfig
    {
        public string ContentType { get; set; } = "application/json";
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 8080;
        public string UrlPath { get; set; } = "/healthz";
        public Func<CancellationToken, Task<string>> HealthCheckFunction { get; set; } = async x => JsonSerializer.Serialize(new { Status = "Healthy!" });
    }
}
