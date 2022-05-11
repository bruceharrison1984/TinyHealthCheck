using System.Net;
using System.Text;

namespace TinyHealthCheck.Models
{
    /// <summary>
    /// Container to hold the response data for an IHealthCheck. 
    /// Make sure to serialze the Body object in to the ContentType that the health check is configured for.
    /// </summary>
    public interface IHealthCheckResult
    {
        /// <summary>
        /// Serialized response body. Make sure the serialization scheme matches the ContentType defined for the health-check.
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// HTTP response code that will be returned to the client
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The ContentType that the response will be returned as
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// The encoding of the response. The default(UTF8) should be fine except in special scenarios.
        /// </summary>
        Encoding ContentEncoding { get; }
    }
}