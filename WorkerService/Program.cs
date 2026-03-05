using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAzureServiceBusClient(connectionName: "queue");
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();