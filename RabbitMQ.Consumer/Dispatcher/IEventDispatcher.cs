using RabbitMQ.Consumer.Events.Abstractions;

namespace RabbitMQ.Consumer.Dispatcher;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent command, CancellationToken cancellationToken)
        where TEvent : class, IEvent;
}