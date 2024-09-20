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

app.MapPost("/webhooks/weather", async (HttpContext context, IConnectionMultiplexer connection) =>
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
