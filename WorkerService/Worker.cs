using Azure.Messaging.ServiceBus;

namespace WorkerService;

public partial class Worker(ILogger<Worker> logger, ServiceBusClient queueClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var processor = queueClient.CreateProcessor("queue");
        processor.ProcessMessageAsync += MessageHandler;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        LogReceivedMessageMessage(logger, args.Message.Body.ToString());
        await args.CompleteMessageAsync(args.Message);
    }

    [LoggerMessage(LogLevel.Information, "Received message: {message}")]
    static partial void LogReceivedMessageMessage(ILogger<Worker> logger, string message);
}
