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

        enum FeedType
        {
            TripUpdate,
            VehiclePosition
        }

        private readonly ILogger _logger;
        private readonly IStopInfoReader[] _readers;

        public MessageBuilder(ILogger logger, IStopInfoReader[] readers)
        {
            _logger = logger;
            _readers = readers;
        }

        /// <summary>
        /// Build a feed update by doing the following procedures:
        /// 1. Retrieve StopInfo data and TimeTable.
        /// 2. Find each estimate a Trip ID by referencing TimeTable.
        /// 3. Build FeedEntity of either TripUpdate or VehiclePosition with the use of a Trip ID.
        /// </summary>
        /// <param name="reader">Reader is used to retrieve StopInfo(eBus System estimation)</param>
        /// <param name="type">Build an update for either TripUpdate or VehiclePosition</param>
        /// <returns>A list of FeedEntity</returns>
#nullable enable
        private async Task<FeedEntity[]?> BuildFeedEntity(IStopInfoReader reader, FeedType type)
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

            Dictionary<string, TripUpdate> tripPairs = new();
            Dictionary<string, VehiclePosition> busPairs = new();

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

                if (tripId is null)
                {
                    // invalid tripId
                    _logger.LogInformation("Unable to find a trip for sid:{0}, direction:{1}, estimateDateTime:{2}",
                        tripId, stop.Direction, estimateDateTime);
                    continue;
                }
                else
                {
                    _logger.LogInformation("Trip Id {3} for sid:{0}, direction:{1}, estimateDateTime:{2}",
                        tripId, stop.Direction, estimateDateTime, tripId);
                }


                switch (type)
                {
                    case FeedType.TripUpdate:

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

                        break;
                    case FeedType.VehiclePosition:
                        foreach (StopInfo.StopInfo.BusInfo bus in stop.Bno)
                        {
                            TripDescriptor tripDescriptor = new()
                            {
                                TripId = tripId,
                                ScheduleRelationship = TripDescriptor.Types.ScheduleRelationship.Scheduled
                            };
                            var busSId = reader.FindSIDBySeq(bus.Seq.ToString(), route);
                            if (bus.Tm == null)
                            {
                                _logger.LogInformation("Vehicle missing timestamp", busSId);
                                break;
                            }

                            var busTimestamp = (ulong)bus.Tm;

                            var currentTimestamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                            if (currentTimestamp - busTimestamp > 15 * 60)
                            {
                                // stale if older than 15 mins.
                                break;
                            }

                            if (bus.No == null || bus.Alias == null) {
                                _logger.LogInformation("Unknown bus No or Alias");
                                continue;
                            }

                            VehiclePosition p = new()
                            {
                                Trip = tripDescriptor,
                                Vehicle = new VehicleDescriptor
                                { LicensePlate = bus.No, Label = bus.Alias, Id = bus.No },
                                Position = new Position
                                {
                                    Latitude = bus.Lat,
                                    Longitude = bus.Lon
                                },
                                Timestamp = (ulong)bus.Tm,
                                StopId = busSId
                            };

                            _logger.LogInformation("Vehicle going to: sid {0}", busSId);

                            if (!busPairs.ContainsKey(bus.No))
                            {
                                busPairs.Add(bus.No, p);
                            }
                        }
                        break;

                }


            }

            List<FeedEntity> feedEntities = new();

            foreach (KeyValuePair<string, TripUpdate> item in tripPairs)
            {
                FeedEntity entity = new() { Id = Guid.NewGuid().ToString(), TripUpdate = item.Value };
                feedEntities.Add(entity);
            }

            foreach (KeyValuePair<string, VehiclePosition> item in busPairs)
            {
                FeedEntity entity = new() { Id = Guid.NewGuid().ToString(), Vehicle = item.Value };
                feedEntities.Add(entity);
            }

            return feedEntities.ToArray();
        }

        /// <summary>
        /// Retrieve TripUpdate Message
        /// </summary>
        /// <returns>FeedMessage</returns>
#nullable disable
        public async Task<FeedMessage> GetTripUpdateMessage()
        {
            var message = new FeedMessage();

            var header = new FeedHeader
            {
                GtfsRealtimeVersion = "2",
                Timestamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            };

            message.Header = header;

            foreach(var reader in _readers)
            {
                var entity = await BuildFeedEntity(reader, FeedType.TripUpdate);

                if (entity is FeedEntity[] valueOfEntity)
                {
                    message.Entity.Add(valueOfEntity);
                }
            }


            return message;
        }

        /// <summary>
        /// Retrieve VehiclePosition Message
        /// </summary>
        /// <returns>FeedMessage</returns>
        public async Task<FeedMessage> GetVehiclePositionMessage()
        {
            var message = new FeedMessage();

            var header = new FeedHeader
            {
                GtfsRealtimeVersion = "2",
                Timestamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            };

            message.Header = header;

            foreach (var reader in _readers)
            {
                var entity = await BuildFeedEntity(reader, FeedType.VehiclePosition);

                if (entity is FeedEntity[] valueOfEntity)
                {
                    message.Entity.Add(valueOfEntity);
                }
            }

            return message;
        }

    }
}

