﻿using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace Aspire.Deployment.ApiService.EndPoints
{
    public static class AllEndpoints
    {
        private static readonly string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public static void SetUpAllEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            });

            app.MapGet("/stock-prices", async (NpgsqlDataSource dataSource) =>
            {
                await using var cnn = await dataSource.OpenConnectionAsync();
                var stockPrices = await cnn.QueryAsync("SELECT * FROM public.stock_prices");
                return Results.Ok(stockPrices);
            });
        }
    }

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
