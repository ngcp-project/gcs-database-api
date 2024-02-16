using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public void QueueDeclare(string queue) {
        _channel.QueueDeclarePassive(queue);
    }

    public void StartConsuming(string queue) {
        Consumer = new EventingBasicConsumer(_channel);

        _channel.BasicConsume(
            queue: queue,
            autoAck: true,
            consumer: Consumer
        );
    }

    public void StopListening() {
        _channel.Close();
    }
}
