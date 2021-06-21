# TinyHealthCheck
A very small library for adding health checks to C# ServiceWorkers. It can be used anywhere you want a health check endpoint, but
don't want to drag in the entire MVC ecosystemt to support it.

## Usage
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

This will create a single endpoint that returns a Healthy response.