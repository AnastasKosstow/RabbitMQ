namespace RabbitMQ.Producer.Services;

public interface IPublisherService
{
    void Publish(Guid id);
}
