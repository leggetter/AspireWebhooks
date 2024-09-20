var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.AspireWebhooks_WebhooksService>("webhooksservice")
    .WithExternalHttpEndpoints()
    .WithReference(cache);

var apiService = builder.AddProject<Projects.AspireWebhooks_ApiService>("apiservice")
    .WithReference(cache);

builder.AddProject<Projects.AspireWebhooks_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
