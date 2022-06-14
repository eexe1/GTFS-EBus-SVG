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
        private readonly IStopInfoReader _readerA;
        private readonly IStopInfoReader _readerB;

        public MessageBuilder(ILogger logger, IStopInfoReader reader)
        {
            _logger = logger;
            _readerA = reader;
        }

        public MessageBuilder(ILogger logger, IStopInfoReader readerA, IStopInfoReader readerB)
        {
            _logger = logger;
            _readerA = readerA;
            _readerB = readerB;
        }

        private async Task<FeedEntity> GetStopInfoFeedEntity(IStopInfoReader reader)
        {

            List<StopInfo.StopInfo> stopList = await reader.RetrieveStopInfoAsync();

            if (stopList.Count < 1)
            {
                _logger.LogDebug("Fail to retrieve any stop info");
                // no stops to proceed
                return new FeedEntity();
            }

            IStopInfoReader.Route route = reader.GetRoute();
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
                string sid = reader.FindSIDBySeq(stop.Seq.ToString(), route);

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
            

            return entity;
        }

        private string EncodeMessage(IMessage message)
        {
            return message.ToByteString().ToBase64();
        }

        public async Task<string> GetEncodedStopInfoMessage()
        {
            var message = new FeedMessage();
            var entity = await GetStopInfoFeedEntity(_readerA);

            message.Entity.Add(entity);

            if (_readerB != null)
            {
                var entityB = await GetStopInfoFeedEntity(_readerB);
                message.Entity.Add(entityB);
            }

            return EncodeMessage(message);
        }

    }
}

