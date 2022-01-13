using System.Net;

namespace TinyHealthCheck.Models
{
    /// <summary>
    /// Container to hold the response data for an IHealthCheck. Make sure to serialze the Body object in to the specified ContentType.
    /// </summary>
    public class HealthCheckResult : IHealthCheckResult
    {
        /// <summary>
        /// Container to hold the response data for an IHealthCheck. Make sure to serialze the Body object in to the specified ContentType.
        /// </summary>
        public HealthCheckResult() { }

        /// <summary>
        /// Container to hold the response data for an IHealthCheck. Make sure to serialze the Body object in to the specified ContentType.
        /// </summary>
        public HealthCheckResult(string body, HttpStatusCode statusCode)
        {
            Body = body;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Serialized response body. Make sure the serialization scheme matches the ContentType defined for the health-check.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// HTTP response code that will be returned to the client
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
}
