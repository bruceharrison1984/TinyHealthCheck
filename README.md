# TinyHealthCheck
A very small library for adding health checks to C# ServiceWorkers. It can be used anywhere you want a health check endpoint, but
don't want to drag in the entire MVC ecosystemt to support it. 

*Only GET operations are supported*

## Simple Usage
Simply add the TinyHealthCheck as a Hosted Service to have it run as a background process:
```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
            services.AddHostedService<TinyHealthCheck.TinyHealthCheck>();
        });
}
```

This will create a single endpoint on `http://localhost:8080/healthz` with the following response body:
```json
{
    "Status": "Healthy!"
}
```

## Advanced Usage
The function that returns the response body can be completely overridden to allow for any response:
```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    var processStartTime = DateTimeOffset.Now;
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
            services.AddHostedService<TinyHealthCheck.TinyHealthCheck>(sp =>
            {
                return new TinyHealthCheck.TinyHealthCheck(
                    logger: sp.GetRequiredService<ILogger<TinyHealthCheck.TinyHealthCheck>>(),
                    hostname: "*",
                    port: 8081,
                    contentType: "application/json",
                    urlPath: "healthz",
                    healthCheckFunction: async cancellationToken => JsonSerializer.Serialize(new { Status = "Healthy!", Uptime = (DateTimeOffset.Now - processStartTime).ToString() }));
            });
        });
}
```
This will return the following body:
```json
{
    "Status": "Healthy!",
    "Uptime": "00:00:04.5997430"
}
```

## Hostname consideration
By default, the `hostname` parameter is set to `localhost`. This will work fine for local development, but will not work across the network.
To allow listening on all interfaces, you must set hostname to `*`. There are also security implications to doing this.

**On windows, you must run the process as an administrator to use * as the hostname! Failure to do this will result in the TinyHealthCheck process failing**