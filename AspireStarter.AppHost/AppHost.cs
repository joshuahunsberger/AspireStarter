using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

var serviceBus = builder.AddAzureServiceBus("messaging");
var queue = serviceBus.AddServiceBusQueue("queue");

var server = builder.AddProject<Projects.AspireStarter_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithReference(queue);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

var consumer = builder.AddProject<WorkerService>("queueconsumer");

builder.Build().Run();