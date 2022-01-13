using System.Net;
using System.Text;

namespace TinyHealthCheck.Models
{
    /// <inheritdoc/>
    public class HealthCheckResult : IHealthCheckResult
    {
        /// <summary>
        /// Container to hold the response data for an IHealthCheck. Make sure to serialize the Body object in to the specified ContentType.
        /// </summary>
        public HealthCheckResult(string body, HttpStatusCode statusCode)
        {
            Body = body;
            StatusCode = statusCode;
        }

        /// <inheritdoc/>
        public string Body { get; set; }

        /// <inheritdoc/>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <inheritdoc/>
        public string ContentType { get; } = "text/plain";

        /// <inheritdoc/>
        public Encoding ContentEncoding { get; } = Encoding.UTF8;
    }
}
