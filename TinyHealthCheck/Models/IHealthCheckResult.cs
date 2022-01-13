using System.Net;

namespace TinyHealthCheck.Models
{
    public interface IHealthCheckResult
    {
        string Body { get; set; }
        HttpStatusCode StatusCode { get; set; }
    }
}