using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureServiceBusClient(connectionName: "queue");
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();