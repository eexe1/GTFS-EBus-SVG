using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BusTripUpdate.StopInfo;
using TransitRealtime;
using Google.Protobuf;
using Microsoft.AspNetCore.Http.Features;

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
            FeedMessage message;
            if (isAll)
            {
                _logger.LogInformation("Start to gather stop info for both routes");
                // get data from both routes
                StopInfoReader readerA = new(IStopInfoReader.Route.Windward);
                StopInfoReader readerB = new(IStopInfoReader.Route.Leeward);
                MessageBuilder messageBuilder = new(_logger, readerA, readerB);
                message = await messageBuilder.GetStopInfoMessage();
            }
            else
            {
                reader = new(routeEnum);
                _logger.LogInformation("Start to gather stop info for route:{0}", routeEnum);
                MessageBuilder messageBuilder = new(_logger, reader);
                message = await messageBuilder.GetStopInfoMessage();
            }

            context.Response.StatusCode = 200;

            var syncIOFeature = context.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            message.WriteTo(context.Response.Body);

            }
    }
}