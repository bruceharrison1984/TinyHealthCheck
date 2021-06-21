# TinyHealthCheck
A very small library for adding health checks to C# ServiceWorkers. It can be used anywhere you want a health check endpoint, but
don't want to drag in the entire MVC ecosystemt to support it.

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

## Hostname consideration
By default, the `hostname` parameter is set to `localhost`. This will work fine for local development, but will not work across the network.
To allow listening on all interfaces, you must set hostname to `*`.

**On windows, you must run the process as an administrator to use * as the hostname!**