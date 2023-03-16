using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ;

IServiceCollection services = new ServiceCollection();
IServiceProvider serviceProvider = BuildServiceProvider(services);




static IServiceProvider BuildServiceProvider(IServiceCollection services)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    services.AddRabbitMq(configuration);

    return services.BuildServiceProvider();
}
