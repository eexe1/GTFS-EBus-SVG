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
            var isAll = route?.ToLower() == "all";

            StopInfoReader reader;

            IStopInfoReader.Route routeEnum = isLeeward ? IStopInfoReader.Route.Leeward : IStopInfoReader.Route.Windward;

            // if route is not provided, return an error
            if (!isWindward && !isLeeward && !isAll)
            {
                context.Response.StatusCode = 400;
                var error = "Incorrect parameters, contact developers";
                _logger.LogError(error);
                await context.Response.WriteAsync(error);
                return;
            }

            string output;
            if (isAll)
            {
                _logger.LogInformation("Start to gather stop info for both routes");
                // get data from both routes
                StopInfoReader readerA = new(IStopInfoReader.Route.Windward);
                StopInfoReader readerB = new(IStopInfoReader.Route.Leeward);
                MessageBuilder messageBuilder = new(_logger, readerA, readerB);
                output = await messageBuilder.GetEncodedStopInfoMessage();
            }
            else
            {
                reader = new(routeEnum);
                _logger.LogInformation("Start to gather stop info for route:{0}", routeEnum);
                MessageBuilder messageBuilder = new(_logger, reader);
                output = await messageBuilder.GetEncodedStopInfoMessage();
            }

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(output);

        }
    }
}
