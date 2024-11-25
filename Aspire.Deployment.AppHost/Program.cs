var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("local-redis");

var apiService = builder.AddProject<Projects.Aspire_Deployment_ApiService>("apiservice");

builder.AddProject<Projects.Aspire_Deployment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
