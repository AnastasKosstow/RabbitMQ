using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Consumer.Events.Abstractions;

namespace RabbitMQ.Consumer.Dispatcher;

internal sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
        =>
        this.serviceProvider = serviceProvider;


    public async Task DispatchAsync<TEvent>(TEvent command, CancellationToken cancellationToken)
        where TEvent : class, IEvent
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();

        await handler.HandleAsync(command, cancellationToken);
    }
}