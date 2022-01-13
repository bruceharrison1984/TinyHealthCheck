using System.Net;
using System.Text;
using System.Text.Json;

namespace TinyHealthCheck.Models
{
    /// <inheritdoc/>
    public class JsonHealthCheckResult : IHealthCheckResult
    {
        /// <summary>
        /// Container to hold the response data for an IHealthCheck. Automatically serializes the response object into JSON.
        /// </summary>
        public JsonHealthCheckResult(object responseObject, HttpStatusCode statusCode)
        {
            Body = JsonSerializer.Serialize(responseObject);
            StatusCode = statusCode;
        }

        /// <inheritdoc/>
        public string Body { get; set; }

        /// <inheritdoc/>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <inheritdoc/>
        public string ContentType { get; } = "application/json";

        /// <inheritdoc/>
        public Encoding ContentEncoding { get; } = Encoding.UTF8;
    }
}
