using WorkerService;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureServiceBusClient(connectionName: "queue");
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();