using System.Text;

namespace TinyHealthCheck
{
    public class TinyHealthCheckConfig
    {
        public string ContentType { get; set; } = "application/json";
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 8080;
        public string UrlPath { get; set; } = "/healthz";
        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;
    }
}
