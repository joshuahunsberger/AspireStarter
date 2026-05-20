using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

var serviceBus = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var queue = serviceBus.AddServiceBusQueue("queue");

var appInsights = builder.AddAzureApplicationInsights("appInsights");

var server = builder.AddProject<AspireStarter_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithReference(queue)
    .WithReference(appInsights);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

#pragma warning disable ASPIREJAVASCRIPT001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
webfrontend.PublishAsStaticWebsite(apiPath: "/api", apiTarget: server)
    .WithExternalHttpEndpoints();
#pragma warning restore ASPIREJAVASCRIPT001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var consumer = builder.AddProject<WorkerService>("queueconsumer")
    .WithReference(queue)
    .WithReference(appInsights);

builder.Build().Run();
