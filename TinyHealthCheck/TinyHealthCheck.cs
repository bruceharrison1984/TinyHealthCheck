using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TinyHealthCheck
{
    public class TinyHealthCheck : BackgroundService
    {
        private readonly ILogger<TinyHealthCheck> _logger;
        private readonly string _contentType;
        private readonly string _hostname;
        private readonly int _port;
        private readonly string _urlPath;
        private readonly Func<CancellationToken, Task<string>> _healthCheckFunction;
        private readonly HttpListener _listener = new HttpListener();

        public TinyHealthCheck(ILogger<TinyHealthCheck> logger,
            string contentType = "application/json",
            string hostname = "localhost",
            int port = 8080,
            string urlPath = "healthz",
            Func<CancellationToken, Task<string>> healthCheckFunction = null)
        {
            if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException($"'{nameof(contentType)}' cannot be null or whitespace.", nameof(contentType));
            if (string.IsNullOrWhiteSpace(hostname)) throw new ArgumentException($"'{nameof(hostname)}' cannot be null or whitespace.", nameof(hostname));
            if (string.IsNullOrWhiteSpace(urlPath)) throw new ArgumentException($"'{nameof(urlPath)}' cannot be null or whitespace.", nameof(urlPath));

            _logger = logger ?? new NullLogger<TinyHealthCheck>();
            _contentType = contentType;
            _hostname = hostname;
            _port = port;
            _urlPath = urlPath;
            _healthCheckFunction = healthCheckFunction ?? DefaultHealthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _listener.Prefixes.Add($"http://{_hostname}:{_port}/");
                _listener.Start();

                _logger.LogInformation($"TinyHealthCheck started on port '{_port}'");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var httpContext = await _listener.GetContextAsync();
                    ThreadPool.QueueUserWorkItem(async x => await ProcessHealthCheck(x, cancellationToken), httpContext, false);
                }
            }
            catch (HttpListenerException e)
            {
                _logger.LogError(e, $"Port '{_port}' is already occupied by another process");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "TinyHealthCheck had an exception!");
            }
        }

        private async Task ProcessHealthCheck(HttpListenerContext client, CancellationToken cancellationToken)
        {
            var request = client.Request;
            var response = client.Response;

            _logger.LogInformation($"TinyHealthCheck recieved a request from {request.RemoteEndPoint}");

            if (request.HttpMethod.ToUpper() != "GET" || request.Url.PathAndQuery[1..] != _urlPath)
            {
                response.StatusCode = 404;
                response.Close();
                return;
            }

            var responseBody = await _healthCheckFunction(cancellationToken);

            response.ContentType = _contentType;
            response.ContentEncoding = Encoding.UTF8;
            byte[] data = Encoding.UTF8.GetBytes(responseBody);

            response.ContentLength64 = data.LongLength;
            await response.OutputStream.WriteAsync(data, cancellationToken);

            response.Close();
        }

        private async Task<string> DefaultHealthCheck(CancellationToken cancellationToken)
        {
            return JsonSerializer.Serialize(new { Status = "Healthy!" });
        }
    }
}
