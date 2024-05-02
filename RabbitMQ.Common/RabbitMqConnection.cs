using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using Polly;
using Polly.Retry;

namespace RabbitMQ.Common;

public class RabbitMqConnection : IDisposable
{
    private static readonly object SyncLocker = new();

    private bool disposed;
    private IConnection connection;
    private readonly RetryPolicy policy;
    private readonly ConnectionFactory connectionFactory;
    private readonly ILogger<RabbitMqConnection> logger;

    public RabbitMqConnection(RabbitMqSettings settings, ILogger<RabbitMqConnection> logger)
    {
        this.logger = logger;
        this.connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(CreateConnectionString(settings)),
        };

        this.policy = Policy
            .Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetry(settings.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) =>
                {
                    logger.LogWarning(exception, "Failed to connect to RabbitMQ. Retrying in {Delay} seconds.", span.TotalSeconds);
                });
    }

    public bool IsConnected => connection != null && connection.IsOpen && !disposed;

    public bool TryConnect()
    {
        if (IsConnected)
        {
            return true;
        }

        lock (SyncLocker)
        {
            connection = policy.Execute(() => connectionFactory.CreateConnection());

            if (IsConnected)
            {
                connection.ConnectionShutdown += OnConnectionShutdown;
                connection.ConnectionBlocked += OnConnectionBlocked;
                connection.CallbackException += OnException;
                return true;
            }

            logger.LogError("Failed to connect to RabbitMQ");
            return false;
        }
    }

    public IModel CreateModel()
    {
        if (!IsConnected)
        {
            logger.LogWarning("RabbitMQ connection is not established. Unable to create a channel.");
        }

        return connection.CreateModel();
    }

    private void OnException(object sender, CallbackExceptionEventArgs e)
    {
        if (disposed) return;

        logger.LogError(e.Exception, "RabbitMQ exception occurred: {Message}", e.Exception.Message);
        TryConnect();
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (disposed) return;

        logger.LogWarning("RabbitMQ connection is blocked: {Reason}", e.Reason);
        TryConnect();
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        if (disposed) return;

        logger.LogWarning("RabbitMQ connection was shut down: {ReplyText}", e.ReplyText);
        TryConnect();
    }

    private string CreateConnectionString(RabbitMqSettings settings)
    {
        void CheckNullOrInvalid(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(message, nameof(settings));
            }
        }

        CheckNullOrInvalid(settings.Username, "Username is required.");
        CheckNullOrInvalid(settings.Password, "Password is required.");
        CheckNullOrInvalid(settings.Host, "Host is required.");

        var connectionString = string.Format("amqp://{0}:{1}@{2}:{3}", settings.Username, settings.Password, settings.Host, settings.Port);
        return connectionString;
    }

    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            connection?.Dispose();
        }
    }
}
