using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading.Tasks;

public class RabbitMQService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    public bool IsConnected { get; private set; }

    public RabbitMQService(string hostName, string userName, string password)
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }catch (BrokerUnreachableException ex)
        {
            Console.WriteLine("Could not reach the RabbitMQ broker. Please check the connection settings.");
            Console.WriteLine(ex.Message);
            IsConnected = false;
        }
        catch (ConnectFailureException ex)
        {
            Console.WriteLine("Failed to connect to the RabbitMQ broker. Please check if the broker is running.");
            Console.WriteLine(ex.Message);
            IsConnected = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred while trying to connect to RabbitMQ.");
            Console.WriteLine(ex.Message);
            IsConnected = false;
        }
    }

    public void SendMessage(string queueName, string message)
    {
        if (IsConnected)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }

    public void ReceiveMessages(string queueName, Action<string> handleMessage)
    {
        if (IsConnected)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                handleMessage(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
