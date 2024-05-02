using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Common.Extensions;
using RabbitMQ.Producer.Services;

IServiceCollection services = new ServiceCollection();
IServiceProvider serviceProvider = BuildServiceProvider(services);

var clientFromConnection = serviceProvider.GetService<IRabbitMqPublisherWithConnection>();
var clientFromConnectionPool = serviceProvider.GetService<IRabbitMqPublisherWithConnectionPool>();
clientFromConnection.Publish(Guid.NewGuid(), "rabbit_routing_key");
clientFromConnectionPool.Publish(Guid.NewGuid(), "rabbit_routing_key");

Console.ReadKey();

static IServiceProvider BuildServiceProvider(IServiceCollection services)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    services.AddSingleton(configuration);
    services.AddLogging(loggerBuilder =>
    {
        loggerBuilder.AddConsole();
    });

    services.TryAddScoped<IRabbitMqPublisherWithConnection, RabbitMqPublisherWithConnection>();
    services.TryAddScoped<IRabbitMqPublisherWithConnectionPool, RabbitMqPublisherWithConnectionPool>();

    return services
        .AddRabbitMQ(configuration)
        .BuildServiceProvider();
}
