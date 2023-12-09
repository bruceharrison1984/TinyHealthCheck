namespace TinyHealthCheck.Models
{
    public class TinyHealthCheckConfig
    {
        /// <summary>
        /// The hostname that will be listened on. 'localhost' by default, typically '*' is desired unless you know the hostname ahead of time.
        /// If running inside a docker container, * or + may need to be used instead of 'localhost'.
        /// </summary>
        public string Hostname { get; set; } = "localhost";

        /// <summary>
        /// Port that this health check will be served on.
        /// </summary>
        public int Port { get; set; } = 8080;

        /// <summary>
        /// The url path that the health check will be accessible on.
        /// </summary>
        public string UrlPath { get; set; } = "/healthz";
    }
}
