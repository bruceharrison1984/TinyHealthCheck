using System.Net;

namespace TinyHealthCheck.HealthChecks
{
    public class HealthCheckResult
    {
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
}
