using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TransitRealtime;

namespace TripUpdateHttpFunction
{
    public class Function : IHttpFunction
    {
        private readonly ILogger _logger;


        public Function(ILogger<Function> logger) =>
            _logger = logger;

        public async Task HandleAsync(HttpContext context)
        {
            HttpRequest request = context.Request;
            // Check URL parameters for "message" field
            string message = request.Query["message"];

            // If there's a body, parse it as JSON and check for "message" field.
            using TextReader reader = new StreamReader(request.Body);
            string text = await reader.ReadToEndAsync();
            if (text.Length > 0)
            {
                try
                {
                    JsonElement json = JsonSerializer.Deserialize<JsonElement>(text);
                    if (json.TryGetProperty("message", out JsonElement messageElement) &&
                        messageElement.ValueKind == JsonValueKind.String)
                    {
                        message = messageElement.GetString();
                    }
                }
                catch (JsonException parseException)
                {
                    _logger.LogError(parseException, "Error parsing JSON request");
                }
            }

            BusTripUpdate.MessageBuilder messageBuilder = new();
            await context.Response.WriteAsync(messageBuilder.GetSampleMessage());
            //await context.Response.WriteAsync(message ?? "Hello World!");

        }
    }
}
