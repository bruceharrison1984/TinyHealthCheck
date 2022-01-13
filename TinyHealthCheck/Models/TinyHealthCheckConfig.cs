using System.Text;

namespace TinyHealthCheck.Models
{
    public class TinyHealthCheckConfig
    {
        /// <summary>
        /// The ContentType that the response will be returned as
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// The hostname that will be listened on. 'localhost' by default, typically '*' is desired unless you know the hostname ahead of time.
        /// </summary>
        public string Hostname { get; set; } = "localhost";

        /// <summary>
        /// Port that this health check will be served on
        /// </summary>
        public int Port { get; set; } = 8080;

        /// <summary>
        /// The url path that the health check will be accessible on.
        /// </summary>
        public string UrlPath { get; set; } = "/healthz";

        /// <summary>
        /// The encoding of the response. The default(UTF8) should be fine except in special scenarios.
        /// </summary>
        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;
    }
}
