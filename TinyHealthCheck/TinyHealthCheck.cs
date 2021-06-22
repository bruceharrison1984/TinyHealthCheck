﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyHealthCheck.HealthChecks;

namespace TinyHealthCheck
{
    public class HealthCheckService<T> : BackgroundService where T : IHealthCheck, new()
    {
        private readonly ILogger<HealthCheckService<T>> _logger;
        private readonly TinyHealthCheckConfig _config;
        private readonly T _healthCheck;
        private readonly HttpListener _listener = new HttpListener();

        public HealthCheckService(ILogger<HealthCheckService<T>> logger, TinyHealthCheckConfig config)
        {
            _logger = logger;
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _healthCheck = new T();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _listener.Prefixes.Add($"http://{_config.Hostname}:{_config.Port}/");
                _listener.Start();

                _logger.LogInformation($"TinyHealthCheck started on port '{_config.Port}'");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var httpContext = await _listener.GetContextAsync();
                    ThreadPool.QueueUserWorkItem(async x => await ProcessHealthCheck(x, cancellationToken), httpContext, false);
                }
            }
            catch (HttpListenerException e)
            {
                _logger.LogError(e, $"Port '{_config.Port}' is already occupied by another process");
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

            if (request.HttpMethod.ToUpper() != "GET" || request.Url.PathAndQuery != _config.UrlPath)
            {
                response.StatusCode = 404;
                response.Close();
                return;
            };

            var responseBody = await _healthCheck.Execute(cancellationToken);

            response.ContentType = _config.ContentType;
            response.ContentEncoding = Encoding.UTF8;
            byte[] data = Encoding.UTF8.GetBytes(responseBody);

            response.ContentLength64 = data.LongLength;
            await response.OutputStream.WriteAsync(data, cancellationToken);

            response.Close();
        }
    }
}
