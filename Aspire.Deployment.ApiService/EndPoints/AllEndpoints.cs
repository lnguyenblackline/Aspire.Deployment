using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

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
            app.MapGet("/weatherforecast", async (IDistributedCache cache) =>
            {
                var cacheName = "redisCache";
                var cacheData = await cache.GetAsync(cacheName);
                if (cacheData is not null)
                {
                    using var ms = new MemoryStream(cacheData);
                    return JsonSerializer.Deserialize<WeatherForecast[]>(ms);
                }
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
        }
    }
}
