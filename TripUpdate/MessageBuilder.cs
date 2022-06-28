using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusTripUpdate.StopInfo;
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

#nullable enable
        private async Task<FeedEntity[]?> GetStopInfoFeedEntity(IStopInfoReader reader)
        {

            List<StopInfo.StopInfo> stopList = await reader.RetrieveStopInfoAsync();

            if (stopList.Count < 1)
            {
                _logger.LogDebug("Fail to retrieve any stop info");
                // no stops to proceed
                return null;
            }

            IStopInfoReader.Route route = reader.GetRoute();

            var timeTable = TimeTable.GetTimeTable();
            timeTable.logger = _logger;

            // keyed by tripId
            Dictionary<string, TripUpdate> tripPairs= new();

            // iterate the stop list to append a stopTimeUpdate event to a trip
            foreach (StopInfo.StopInfo stop in stopList)
            {
                long arrivalInterval = TimeParser.ParseTime(stop.Est);
                string sid = reader.FindSIDBySeq(stop.Seq.ToString(), route);
                _logger.LogInformation("System estimate: {0}, in {1} seconds for sid {2}", stop.Est, arrivalInterval, sid);
                if (arrivalInterval == -1)
                {
                    // invalid estimate
                    continue;
                }

                var estimateDateTime = DateTime.UtcNow.AddSeconds(arrivalInterval);

                var tripId = timeTable.FindNearestTripId(sid, estimateDateTime, stop.Direction);
                _logger.LogInformation("Trip Id: {0}", tripId);


                if (tripId is null)
                {
                    // invalid tripId
                    continue;
                }

                // add Trip Update to the dict
                if (!tripPairs.ContainsKey(tripId))
                {
                    TripDescriptor tripDescriptor = new()
                    {
                        TripId = tripId,
                        ScheduleRelationship = TripDescriptor.Types.ScheduleRelationship.Scheduled
                    };
                    TripUpdate newTripUpdate = new() { Trip = tripDescriptor };
                    tripPairs.Add(tripId, newTripUpdate);
                }

                long arrivalTime = TimeParser.ToEpoch(arrivalInterval);

                TripUpdate.Types.StopTimeEvent stopTimeEvent = new()
                {
                    Time = arrivalTime
                };

                TripUpdate.Types.StopTimeUpdate stopTimeUpdate = new() { StopId = sid, Arrival = stopTimeEvent };
                tripPairs[tripId].StopTimeUpdate.Add(stopTimeUpdate);
                _logger.LogInformation("With Stop Time Update stopId: {0}, arrival: {1}", stopTimeUpdate.StopId, stopTimeUpdate.Arrival);


            }

            List<FeedEntity> feedEntities = new();

            foreach (KeyValuePair<string, TripUpdate> item in tripPairs)
            {
                FeedEntity entity = new() { Id = Guid.NewGuid().ToString(), TripUpdate = item.Value };
                feedEntities.Add(entity);
            }

            return feedEntities.ToArray();
        }
#nullable disable
        public async Task<FeedMessage> GetStopInfoMessage()
        {
            var message = new FeedMessage();

            var header = new FeedHeader
            {
                GtfsRealtimeVersion = "2",
                Timestamp = (ulong)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            };

            message.Header = header;

            var entityA = await GetStopInfoFeedEntity(_readerA);

            if (entityA is FeedEntity[] valueOfEntityA)
            {
                message.Entity.Add(valueOfEntityA);
            }


            if (_readerB != null)
            {
                var entityB = await GetStopInfoFeedEntity(_readerB);
                if (entityB is FeedEntity[] valueOfEntityB)
                {
                    message.Entity.Add(valueOfEntityB);
                }
            }

            return message;
        }

    }
}

