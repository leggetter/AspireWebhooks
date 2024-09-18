using System.Text.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddRedisClient("cache");

// Add services to the container.
builder.Services.AddProblemDetails();
// builder.Services.AddRedisCacheService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

WeatherForecast[] GenerateExampleForecasts()
{
    return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
}

var exampleForecasts = GenerateExampleForecasts();
Console.WriteLine("Example forecasts {0}", JsonSerializer.Serialize(exampleForecasts));

app.MapGet("/weatherforecast", (IConnectionMultiplexer connection) =>
{
    WeatherForecast[]? forecast = null;
    var database = connection.GetDatabase();
    var forecastValue = database.StringGet("weatherforecast");
    if (forecastValue.HasValue && forecastValue != RedisValue.Null)
    {
        Console.WriteLine("Cache found, using cached forecast", forecastValue);
        forecast = JsonSerializer.Deserialize<WeatherForecast[]>(forecastValue.ToString())!;
    }
    else {
        Console.WriteLine("No cache found");
    }
    // else {
    //     Console.WriteLine("No cache found, generating new forecast");
    //     forecast = GenerateExampleForecasts();
    //     database.StringSet("weatherforecast", JsonSerializer.Serialize(forecast), TimeSpan.FromMinutes(1));
    // }
    
    return Task.FromResult(forecast);
});

app.MapPost("/weatherforecast", async (HttpContext context, IConnectionMultiplexer connection) =>
{
    var database = connection.GetDatabase();
    var forecast = await JsonSerializer.DeserializeAsync<WeatherForecast[]>(context.Request.Body);
    Console.WriteLine("Received JSON: {0}", forecast);
    
    if (forecast != null)
    {
        database.StringSet("weatherforecast", JsonSerializer.Serialize(forecast), TimeSpan.FromMinutes(1));
        Console.WriteLine("Stored forecast in cache");
        return Results.Ok(forecast);
    }
    
    return Results.BadRequest("Invalid payload");
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
