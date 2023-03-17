using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Producer.RabbitMQ;
using RabbitMQ.Producer.Services;

IServiceCollection services = new ServiceCollection();
IServiceProvider serviceProvider = BuildServiceProvider(services);

var client = serviceProvider.GetService<IPublisherService>();
client.Publish(Guid.NewGuid());

Console.ReadKey();

static IServiceProvider BuildServiceProvider(IServiceCollection services)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    services.AddSingleton(configuration);

    services.AddOptions<RabbitMqOptions>()
        .Configure<IConfiguration>((settings, _) =>
        {
            configuration.GetSection("rabbitMq").Bind(settings);
        });

    services
        .AddScoped<IPublisherService, PublisherService>()
        .AddRabbitMq(configuration);

    return services.BuildServiceProvider();
}
