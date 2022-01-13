using System.Net;
using System.Text.Json;

namespace TinyHealthCheck.Models
{
    /// <summary>
    /// Container to hold the response data for an IHealthCheck. Automatically serializes the response object into JSON.
    /// </summary>
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

        /// <summary>
        /// JSON response body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// HTTP response code that will be returned to the client
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
}
