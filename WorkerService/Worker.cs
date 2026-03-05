using Azure.Messaging.ServiceBus;

namespace WorkerService;

public partial class Worker(ILogger<Worker> logger, ServiceBusClient queueClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var processor = queueClient.CreateProcessor("queue");
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Start processing
        await processor.StartProcessingAsync(stoppingToken);

        await Task.Delay(-1, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        // Stop processing and dispose
        await processor.StopProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        LogReceivedMessageMessage(logger, args.Message.Body.ToString());
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing message");
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Received message: {message}")]
    static partial void LogReceivedMessageMessage(ILogger<Worker> logger, string message);
}
