using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RabbitMQ.Common.Extensions;

public static class ServiceCollectionExtensions
{
    private const string SECTION_NAME = "rabbitmq";

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqSettings>()
            .Configure<IConfiguration>((settings, _) =>
            {
                configuration.GetSection(SECTION_NAME).Bind(settings);
            });

        return services;
    }
}
