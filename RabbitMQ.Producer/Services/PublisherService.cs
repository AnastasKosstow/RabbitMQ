using RabbitMQ.Producer.Events;
using RabbitMQ.Producer.RabbitMQ.Client;

namespace RabbitMQ.Producer.Services;

internal class PublisherService : IPublisherService
{
    private readonly IRabbitMqClient client;

    public PublisherService(IRabbitMqClient client)
        =>
        this.client = client;

    public void Publish(Guid id)
    {
        var msg = new RabbitMqEvent(id);
        client.Send(msg);
    }
}
