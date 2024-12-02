using System.Text.Json;
using Aspire.Deployment.ApiService;
using Aspire.Deployment.ApiService.EndPoints;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("stocks");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddHostedService<DatabaseInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.SetUpAllEndpoints();
app.MapDefaultEndpoints();

app.Run();

