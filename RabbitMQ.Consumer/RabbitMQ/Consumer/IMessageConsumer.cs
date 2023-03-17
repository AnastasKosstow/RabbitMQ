namespace RabbitMQ.Consumer.RabbitMQ.Consumer;

public interface IMessageConsumer
{
    void Consume(string queueName, Action<byte[]> onMessageReceived);
}