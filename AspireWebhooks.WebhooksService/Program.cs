using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddRedisClient("cache");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

string webhookSecret = builder.Configuration["AspireWebhooks:HookdeckWebhookSecret"] ?? throw new InvalidOperationException("HookdeckWebhookSecret cannot be null or empty.");

WebApplication app = builder.Build();
app.UseExceptionHandler();

app.MapPost("/webhooks/weather", async (HttpContext context, IConnectionMultiplexer connection) =>
{
    using StreamReader reader = new StreamReader(context.Request.Body);
    string rawBody = await reader.ReadToEndAsync();

    if(String.IsNullOrEmpty(webhookSecret))
    {
        Console.WriteLine("WARNING: Missing Hookdeck Webhook Secret so we can't verify the request is coming from Hookdeck");
    }
    else
    {
        if(VerifyHmacWebhookSignature(context, "x-hookdeck-signature", webhookSecret, rawBody))
        {
            Console.WriteLine("Webhook originated from Hookdeck");
        }
        else
        {
            Console.WriteLine("Invalid signature");
            return Results.Unauthorized();
        }
    }

    using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(rawBody));
    WeatherForecast[]? forecast = await JsonSerializer.DeserializeAsync<WeatherForecast[]>(memoryStream);
    Console.WriteLine("Received JSON: {0}", forecast);
    
    if (forecast != null)
    {
        IDatabase database = connection.GetDatabase();
        database.StringSet("weatherforecast", JsonSerializer.Serialize(forecast), TimeSpan.FromMinutes(1));
        Console.WriteLine("Stored forecast in cache");
        return Results.Ok(forecast);
    }
    
    return Results.BadRequest("Invalid payload");
});

app.MapDefaultEndpoints();

app.Run();

static bool VerifyHmacWebhookSignature(HttpContext context, string headerName, string webhookSecret, string rawBody)
{
    string? hmacHeader = context.Request.Headers[headerName].FirstOrDefault();

    if (string.IsNullOrEmpty(hmacHeader))
    {
        Console.WriteLine("Missing HMAC headers");
        return false;
    }
    HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(webhookSecret));
    string hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody)));

    return hash.Equals(hmacHeader);
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}