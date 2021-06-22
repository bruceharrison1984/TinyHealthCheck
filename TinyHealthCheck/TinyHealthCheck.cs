using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyHealthCheck.HealthChecks;

namespace TinyHealthCheck
{
    public class HealthCheckService<T> : BackgroundService where T : IHealthCheck
    {
        private readonly ILogger<HealthCheckService<T>> _logger;
        private readonly TinyHealthCheckConfig _config;
        private readonly T _healthCheck;
        private readonly HttpListener _listener = new HttpListener();

        public HealthCheckService(ILogger<HealthCheckService<T>> logger, T healthCheck, TinyHealthCheckConfig config)
        {
            _logger = logger;
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _healthCheck = healthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _listener.Prefixes.Add($"http://{_config.Hostname}:{_config.Port}/");
                _listener.Start();

                _logger.LogInformation($"TinyHealthCheck<{typeof(T).Name}> started on port '{_config.Port}'");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var httpContext = await _listener.GetContextAsync().ConfigureAwait(false);
                    ThreadPool.QueueUserWorkItem(async x => await ProcessHealthCheck(x, cancellationToken).ConfigureAwait(false), httpContext, false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "TinyHealthCheck had an exception!");
            }
        }

        private async Task ProcessHealthCheck(HttpListenerContext client, CancellationToken cancellationToken)
        {
            var request = client.Request;

            _logger.LogInformation($"TinyHealthCheck recieved a request from {request.RemoteEndPoint}");

            using (var response = client.Response)
            {
                if (!request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
                    || !request.Url.PathAndQuery.Equals(_config.UrlPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    response.StatusCode = 404;
                    return;
                };

                var responseBody = await _healthCheck.Execute(cancellationToken).ConfigureAwait(false);

                response.ContentType = _config.ContentType;
                response.ContentEncoding = Encoding.UTF8;
                byte[] data = Encoding.UTF8.GetBytes(responseBody);

                response.ContentLength64 = data.LongLength;
                await response.OutputStream.WriteAsync(data, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
