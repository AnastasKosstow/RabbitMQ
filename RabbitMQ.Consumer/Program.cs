using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Consumer;
using RabbitMQ.Consumer.Dispatcher;
using RabbitMQ.Consumer.Events.Abstractions;
using RabbitMQ.Consumer.RabbitMQ;
using RabbitMQ.Consumer.RabbitMQ.Consumer;
using System.Reflection;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config
            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
            .AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<RabbitMqOptions>(hostContext.Configuration.GetSection("rabbitmq"));
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        services.AddScoped<IMessageConsumer, RabbitMQMessageConsumer>();

        Assembly.GetExecutingAssembly()
                .ExportedTypes
                .Where(type =>
                {
                    var implementType = type
                        .GetInterfaces()
                        .Any(@interface => @interface.IsGenericType &&
                                            @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>));

                    return implementType;
                })
                .ToList()
                .ForEach(commandHandler =>
                {
                    services.AddScoped(commandHandler.GetInterface("IEventHandler`1"), commandHandler);
                });

        services.AddRabbitMq(hostContext.Configuration);
        services.AddHostedService<RabbitMQConsumerBackgroundService>();
    });

await builder.RunConsoleAsync();
