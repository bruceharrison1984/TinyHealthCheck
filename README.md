# TinyHealthCheck

A very small library for adding health checks to C# Service Workers or other headless processes. It can be used
anywhere you want a health check endpoint, but don't want to drag in the entire MVC ecosystem to support it. It has very few dependencies(3),
and utilizes a low priority thread pool for minimal impact on your service worker processes.

[![Nuget](https://img.shields.io/nuget/dt/tinyhealthcheck?color=blue&label=nuget%20downloads)](https://www.nuget.org/packages/TinyHealthCheck/)
![Build Badge](https://img.shields.io/github/actions/workflow/status/bruceharrison1984/TinyHealthCheck/devBuild.yml)

## Notes

- This health check is meant to be used for internal/private health checks only
  - Expose it to the internet at your own peril
- **Only GET operations are supported**
  - I have no plans to support other HttpMethods
- Only one endpoint per port is allowed, as well as one UrlPath per port
  - This library was created for Service Workers that normally have _no_ usable HTTP web server
  - This library creates endpoints without _any_ of the MVC packages
    - This means no middleware, auth, validation, etc
  - You can run different HealthChecks on different ports
- Docker containers may have trouble using `localhost` for the hostname, it's reccomended to use `*` or `+` instead, [see this stackoverflow for more information](https://stackoverflow.com/questions/75961828/c-sharp-net-core-7-http-listener-app-wrapped-in-docker-cannot-be-reached-from)

## Simple Usage

Simply add the TinyHealthCheck as a Hosted Service by calling the extension methods to have it run as a background process:

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var processStartTime = DateTimeOffset.Now;
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
            services.AddBasicTinyHealthCheck(config =>
            {
                config.Port = 8080;
                config.UrlPath = "/healthz";
                return config;
            });
        });
}
```

This will create an endpoint on `http://localhost:8080/healthz` with the following response body:

```json
{
  "Status": "Healthy!"
}
```

## Uptime Monitor Endpoint

Call `AddBasicTinyHealthCheckWithUptime` to add an uptime counter to the output:

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var processStartTime = DateTimeOffset.Now;
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
            services.AddBasicTinyHealthCheckWithUptime(config =>
            {
                config.Port = 8081;
                config.UrlPath = "/healthz";
                return config;
            });
        });
}
```

This will create an endpoint on `http://localhost:8081/healthz` with the following response body:

```json
{
  "Status": "Healthy!",
  "Uptime": "<ever increasing timespan>"
}
```

## Advanced Usage

Calling `AddCustomTinyHealthCheck` with a class that inheirits from IHealthCheck allows you to create whatever type of response you want.
It also allows you to leverage DI to gain access to values from your other DI service containers. You could use this to get queue lengths,
check if databases are accessible, etc.

The return value of `ExecuteAsync` is a `IHealthCheckResult`. The Body will be converted in to a byte[], and the StatusCode will be applied to the response. The Body
can be any wire format you choose(json/xml/html/etc), just make sure to assign the appropriate `ContentType` to your `IHealthCheckResult`. `ContentEncoding` should be left
as UTF-8 unless you have a specific reason to change it.

### IHostedService Note

If you are using a IHostedService, you will require a secondary service to hold the state of your IHostedService. This is because you cannot
reliably retrieve IHostedService from the `IServiceProvider` interface. [See this StackOverflow post](https://stackoverflow.com/a/52038409/889034).
There is a complete example of this in the `DummyServiceWorker` project, as well as a Dockerfile that demonstrates running TinyHealthCheck from
a Linux environment.

In the example you'll notice that while the HealthCheck on localhost:8082 fails after 10 iterations, the other HealthChecks still report success. A custom
health check like this allows you to monitor another process from out-of-band, and report when it has failed.

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var processStartTime = DateTimeOffset.Now;
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<WorkerStateService>();
            services.AddHostedService<Worker>();
            services.AddCustomTinyHealthCheck<CustomHealthCheck>(config =>
            {
                config.Port = 8082;
                config.UrlPath = "/healthz";
                return config;
            });
        });
}

public class CustomHealthCheck : IHealthCheck
{
    private readonly ILogger<CustomHealthCheck> _logger;
    private readonly WorkerStateService _workerStateService;
    //IHostedServices cannot be reliably retrieved from the DI collection
    //A secondary stateful service is required in order to get state information out of it
    //https://stackoverflow.com/a/52038409/889034

    public CustomHealthCheck(ILogger<CustomHealthCheck> logger, WorkerStateService workerStateService)
    {
        _logger = logger;
        _workerStateService = workerStateService;
    }

    public async Task<IHealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("This is an example of accessing the DI containers for logging. You can access any service that is registered");

        if (_workerStateService.IsRunning)
            return new JsonHealthCheckResult(
                new
                {
                    Status = "Healthy!",
                    Iteration = _workerStateService.Iteration,
                    IsServiceRunning = _workerStateService.IsRunning,
                },
                HttpStatusCode.OK);

        return new JsonHealthCheckResult(
            new
            {
                Status = "Unhealthy!",
                Iteration = _workerStateService.Iteration,
                IsServiceRunning = _workerStateService.IsRunning,
                ErrorMessage = "We went over 10 iterations, so the service worker quit!"
            },
            HttpStatusCode.InternalServerError);
    }
}
```

This will return the following body:

```json
//StatusCode 200
{
  "Status": "Healthy!",
  "Iteration": 3,
  "IsServiceRunning": true
}
```

As well as print a message in the application console:

```sh
info: DummyServiceWorker.Program.CustomHealthCheck[0]
      This is an example of accessing the DI containers for logging. You can access any service that is registered
```

Once 10 iterations have been exceeded, the response will change:

```json
//StatusCode 500
{
  "Status": "Unhealthy!",
  "Iteration": 10,
  "IsServiceRunning": false,
  "ErrorMessage": "We went over 10 iterations, so the service worker quit!"
}
```

## Example

A complete example can be found in the `DummyServiceWorker` directory.

## Response Interface

The `IHealthCheckResult` interface is used for returning the response data to the client. Two concrete result types are included:

- An open-ended `HealthCheckResult` that requires you to serialize the payload however you require
- The `JsonHealthCheckResult` accepts an object and automatically serializes it into JSON

Inheiriting from the `IHealthCheckResult` makes it easy to create a custom implementation to return a response body of any serialization scheme.

## Logging

All log messages happen with `Debug` log-level, so they will likely not appear in your logs. If you wish to see them, you can explictly change the log
level via `appsettings.json`.

```json
{
  "Logging": {
    "LogLevel": {
      ...
      "TinyHealthCheck": "Debug"
      ...
    }
  }
}
```

## Hostname consideration

By default, the `hostname` parameter is set to `localhost`. This will work fine for local development, but will not work across the network.
To allow listening on all interfaces, you must set hostname to `*`. There are also security implications to doing this, which is why it is not
recommended to expose these health check endpoints to the internet.

**On windows, you must run the process as an administrator to use \* as the hostname! Failure to do this will result in the TinyHealthCheck process failing**
