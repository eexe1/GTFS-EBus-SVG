using System;
using System.IO;
using System.Text;
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
            TripDescriptor tripDescriptor = new() { TripId = "WF_1" };
            TripUpdate.Types.StopTimeEvent stopTimeEvent = new() { Time = 1653958416 };
            TripUpdate tripUpdate = new() { Trip = tripDescriptor };
            TripUpdate.Types.StopTimeUpdate stopTimeUpdate = new() { StopId = "106_171", Arrival = stopTimeEvent };
            tripUpdate.StopTimeUpdate.Add(stopTimeUpdate);
            FeedEntity entity = new () { Id = Guid.NewGuid().ToString(), TripUpdate = tripUpdate };
            FeedMessage message = new();
            message.Entity.Add(entity);


            return EncodeMessage(message);
        }

        private string EncodeMessage(IMessage message)
        {
            return message.ToByteString().ToBase64();
        }

    }
}

