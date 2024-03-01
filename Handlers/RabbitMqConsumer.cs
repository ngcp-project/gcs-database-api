using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Database.Handlers;

public class RabbitMqConsumer
{
    private readonly IModel _channel;
    public EventingBasicConsumer Consumer;

    public RabbitMqConsumer()
    {
        ConnectionFactory factory = new() { HostName = "localhost" };
        _channel = factory.CreateConnection().CreateModel();
        
    }

    public RabbitMqConsumer(string ipAddress)
    {
        ConnectionFactory factory = new() { HostName = ipAddress };
        _channel = factory.CreateConnection().CreateModel();
    }

    public void CreateConsumer(string queue) 
    {
        _channel.QueueDeclare(
            queue: queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        Consumer = new EventingBasicConsumer(_channel);
    }

    public void StartConsuming(string queue) 
    {
        _channel.BasicConsume(
            queue: queue,
            autoAck: true,
            consumer: Consumer
        );
    }

    public void StopConsuming() 
    {
        _channel.Close();
    }
}
