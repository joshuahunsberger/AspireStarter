using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

var serviceBus = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var queue = serviceBus.AddServiceBusQueue("queue");

var appInsights = builder.AddAzureApplicationInsights("appInsights");

var server = builder.AddProject<AspireStarter_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithReference(queue)
    .WithReference(appInsights);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

builder.AddYarp("bff")
    .WithConfiguration(c =>
    {
        c.AddRoute("/api/{**catch-all}", server);
        c.AddRoute("{**catch-all}", webfrontend);
    })
    .WithExternalHttpEndpoints();

var consumer = builder.AddProject<WorkerService>("queueconsumer")
    .WithReference(queue)
    .WithReference(appInsights);

builder.Build().Run();
