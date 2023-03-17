using RabbitMQ.Consumer.Events.Abstractions;

namespace RabbitMQ.Consumer.Events;

public class RabbitMqEvent : IEvent
{
    public Guid Id { get; }

    public RabbitMqEvent(Guid id)
        => Id = id;
}

public class CustomerCreatedHandler : IEventHandler<RabbitMqEvent>
{
    public Task HandleAsync(RabbitMqEvent @event, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
