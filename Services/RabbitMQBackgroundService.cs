using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQBackgroundService : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly ILogger<RabbitMQBackgroundService> _logger;

    public RabbitMQBackgroundService(RabbitMQService rabbitMQService, ILogger<RabbitMQBackgroundService> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ Background Service is starting.");

        _rabbitMQService.ReceiveMessages("example-queue", message =>
        {
            _logger.LogInformation($"Received message: {message}");
            // Xử lý tin nhắn tại đây
        });

        return Task.CompletedTask;
    }
}
