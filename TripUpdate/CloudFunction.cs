using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BusTripUpdate.StopInfo;

namespace BusTripUpdate
{
    public class CloudFunction : IHttpFunction
    {
        private readonly ILogger _logger;


        public CloudFunction(ILogger<CloudFunction> logger) =>
            _logger = logger;

        public async Task HandleAsync(HttpContext context)
        {
            HttpRequest request = context.Request;
            // Check URL parameters for "route" field
            string route = request.Query["route"];

            var isWindward = route?.ToLower() == "windward";
            var isLeeward = route?.ToLower() == "leeward";

            IStopInfoReader.Route? routeEnum = isWindward ? IStopInfoReader.Route.Windward : (isLeeward ? IStopInfoReader.Route.Leeward : (IStopInfoReader.Route?)null);
            StopInfoReader reader;
            switch (routeEnum)
            {
                case IStopInfoReader.Route.Windward:
                    reader = new(IStopInfoReader.Route.Windward);
                    break;
                case IStopInfoReader.Route.Leeward:
                    reader = new(IStopInfoReader.Route.Leeward);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    var error = "Incorrect parameters, contact developers";
                    _logger.LogError(error);
                    await context.Response.WriteAsync(error);
                    return;
            }

            _logger.LogInformation("Start to gather stop info");
            MessageBuilder messageBuilder = new(_logger, reader);
            var output = await messageBuilder.GetStopInfoMessage();
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(output);

        }
    }
}
