using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusTripUpdate.StopInfo;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using TransitRealtime;
namespace BusTripUpdate
{
    public class MessageBuilder
    {
        private readonly ILogger _logger;
        private readonly IStopInfoReader _reader;

        public MessageBuilder(ILogger logger, IStopInfoReader reader)
        {
            _logger = logger;
            _reader = reader;
        }

        public async Task<string> GetStopInfoMessage()
        {

            FeedMessage message = new();

            List<StopInfo.StopInfo> stopList = await _reader.RetrieveStopInfoAsync();

            if (stopList.Count < 1)
            {
                _logger.LogDebug("Fail to retrieve any stop info");
                // no stops to proceed
                return EncodeMessage(message);
            }

            IStopInfoReader.Route route = _reader.GetRoute();
            // all stops share a single trip, hence, one TripUpdate to contain all stopTimeUpdate events
            TripDescriptor tripDescriptor = new()
            {
                // use the first trip of each direction as a reference
                TripId = route == IStopInfoReader.Route.Windward ? "WF_1" : "LF_1",
                ScheduleRelationship = TripDescriptor.Types.ScheduleRelationship.Added
            };
            TripUpdate tripUpdate = new() { Trip = tripDescriptor };

            // iterate the stop lists to append a stopTimeUpdate event to the trip
            foreach (StopInfo.StopInfo stop in stopList)
            {
                string sid = _reader.FindSIDBySeq(stop.Seq.ToString(), route);

                // build a FeedEntity

                long arrivalInterval = TimeParser.ParseTime(stop.Est);
                _logger.LogDebug("System estimate: {0}, in {1} seconds for sid {2}", stop.Est, arrivalInterval, sid);
                if (arrivalInterval == -1)
                {
                    // invalid estimate
                    continue;
                }

                long arrivalTime = TimeParser.ToEpoch(arrivalInterval);

                TripUpdate.Types.StopTimeEvent stopTimeEvent = new()
                {
                    Time = arrivalTime
                };

                TripUpdate.Types.StopTimeUpdate stopTimeUpdate = new() { StopId = sid, Arrival = stopTimeEvent };
                tripUpdate.StopTimeUpdate.Add(stopTimeUpdate);
            }

            FeedEntity entity = new() { Id = Guid.NewGuid().ToString(), TripUpdate = tripUpdate };

            message.Entity.Add(entity);

            return EncodeMessage(message);
        }

        private string EncodeMessage(IMessage message)
        {
            return message.ToByteString().ToBase64();
        }

    }
}

