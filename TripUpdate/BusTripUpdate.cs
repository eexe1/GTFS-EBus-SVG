using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BusTripUpdate.StopInfoReader; 
using Google.Protobuf;
using TransitRealtime;
namespace BusTripUpdate
{
    public class MessageBuilder
    {
        public MessageBuilder()
        {
        }

        public string GetSampleMessage()
        {

            FeedMessage message = new();

            IStopInfoReader stopInfoReader = new SampleStopInfoReader();
            List<StopInfo> stopList = stopInfoReader.RetrieveStopInfo();

            foreach (StopInfo stop in stopList)
            {

                IStopInfoReader.Route route = IStopInfoReader.Route.Windward;

                string sid = stopInfoReader.FindSIDBySeq(stop.Seq.ToString(), route);

                Console.WriteLine(stop.Id);
                Console.WriteLine(stop.Seq);
                Console.WriteLine(stop.Est);
                // build a FeedEntity

                TripDescriptor tripDescriptor = new()
                {
                    // use the first trip of each direction as a reference
                    TripId = route == IStopInfoReader.Route.Windward ? "WF_1" : "LF_1",
                    ScheduleRelationship = TripDescriptor.Types.ScheduleRelationship.Added
                };

                long arrivalTime = TimeParser.ToEpoch(TimeParser.ParseTime(stop.Est));
                if (arrivalTime == -1)
                {
                    // invalid estimate
                    break;
                }
                TripUpdate.Types.StopTimeEvent stopTimeEvent = new() {
                    Time = arrivalTime
                };
                TripUpdate tripUpdate = new() { Trip = tripDescriptor };
                TripUpdate.Types.StopTimeUpdate stopTimeUpdate = new() { StopId = sid, Arrival = stopTimeEvent };
                tripUpdate.StopTimeUpdate.Add(stopTimeUpdate);
                FeedEntity entity = new() { Id = Guid.NewGuid().ToString(), TripUpdate = tripUpdate };

                message.Entity.Add(entity);
            }


            return EncodeMessage(message);
        }

        private string EncodeMessage(IMessage message)
        {
            return message.ToByteString().ToBase64();
        }

    }
}

