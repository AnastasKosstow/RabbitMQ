using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Common.Extensions;
using RabbitMQ.Consumer;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config
            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
            .AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(loggerBuilder =>
        {
            loggerBuilder.AddConsole();
        });

        services
            .AddRabbitMQ(hostContext.Configuration)
            .AddHostedService<RabbitMQConsumerService>();
    });

await builder.RunConsoleAsync();
