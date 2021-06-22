# TinyHealthCheck
A very small library for adding health checks to C# ServiceWorkers. It can be used anywhere you want a health check endpoint, but
don't want to drag in the entire MVC ecosystem to support it. It has very few dependencies(2), and utilizes a low priority thread pool 
for low impact on your service worker processes.

[![Generic badge](https://img.shields.io/badge/Nuget-Download-Blue.svg)](https://www.nuget.org/packages/TinyHealthCheck/)

## Notes
 - This health check is meant to be used for internal/private health checks only
    - Expose it to the internet at your own peril
 - **Only GET operations are supported**
    - I have no plans to support other HttpMethods
 - Only one endpoint per port is allowed, as well as one UrlPath per port
    - This library was created for Service Workers that normally have *no* usable HTTP web server
    - This library allows endpoints without the full MVC package
        - No middleware, auth, validation, etc
    - You can run different HealthChecks on different ports

## Simple Usage
Simply add the TinyHealthCheck as a Hosted Service to have it run as a background process:
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
                config.Hostname = "*";
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
                config.Hostname = "*";
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

The return value of `Execute` is a string. It will be converted in to a byte[] and sent as the response. It can be any wire format you choose(json/xml/html/etc), 
just make sure to assign the appropriate `ContentType` for your health check client when you define the health check in CreateHostBuilder. The default ContentType 
is `application/json`.

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var processStartTime = DateTimeOffset.Now;
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
            services.AddCustomTinyHealthCheck<CustomHealthCheck>(config =>
            {
                config.Hostname = "*";
                config.Port = 8082;
                config.UrlPath = "/healthz";
                return config;
            });
        });
}

public class CustomHealthCheck : IHealthCheck
{
    private readonly ILogger<CustomHealthCheck> _logger;

    public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
    {
        _logger = logger;
    }

    public async Task<string> Execute(CancellationToken cancellationToken)
    {
        _logger.LogInformation("This is an example of accessing the DI containers for logging. You can access any service that is registered");
        return JsonSerializer.Serialize(new { Status = "Healthy!", CustomValue = "SomeValueFromServices" });
    }
}
```
This will return the following body:
```json
{
    "Status": "Healthy!",
    "CustomValue": "SomeValueFromServices"
}
```

## Hostname consideration
By default, the `hostname` parameter is set to `localhost`. This will work fine for local development, but will not work across the network.
To allow listening on all interfaces, you must set hostname to `*`. There are also security implications to doing this, which is why it is not
recommended to expose these health check endpoints to the internet.

**On windows, you must run the process as an administrator to use * as the hostname! Failure to do this will result in the TinyHealthCheck process failing**
