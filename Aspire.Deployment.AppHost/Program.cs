var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").PublishAsAzurePostgresFlexibleServer();
var stocksDb = postgres.AddDatabase("stocks");

var cache = builder.AddRedis("local-redis");

var apiService = builder.AddProject<Projects.Aspire_Deployment_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithReference(stocksDb)
    .WaitFor(postgres);

builder.AddProject<Projects.Aspire_Deployment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
