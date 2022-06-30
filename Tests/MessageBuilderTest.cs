using System;
using System.Threading.Tasks;
using BusTripUpdate;
using BusTripUpdate.StopInfo;
using Google.Protobuf;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransitRealtime;

namespace Tests
{
    [TestClass]
    public class MessageBuilderTest
    {

        [TestMethod]
        public async Task TestMessageBuilder()
        {
            SampleStopInfoReader reader = new();
            var logger = NullLogger.Instance;
            MessageBuilder messageBuilder = new(logger, reader);
            var message = await messageBuilder.GetStopInfoMessage();
            var result = message.ToByteArray();
            // decode

            FeedMessage feedMessage = FeedMessage.Parser.ParseFrom(result);
            //foreach (FeedEntity entity in feedMessage.Entity)
            //{
            //    Assert.AreEqual(entity.TripUpdate.Trip.TripId, "WF_1");
            //    for (int i = 0; i < entity.TripUpdate.StopTimeUpdate.Count; i++)
            //    {
            //        var update = entity.TripUpdate.StopTimeUpdate[i];
            //        Assert.AreEqual(update.StopId, i == 0 ? "14" : "16");
            //    }

            //}
        }

        [TestMethod]
        public async Task TestLeewardStops()
        {
            SampleStopInfoReader reader = new();
            reader.fileUrl = @"./TestData/stopInfoLeeward.json";
            reader.route = IStopInfoReader.Route.Leeward;
            var logger = NullLogger.Instance;
            MessageBuilder messageBuilder = new(logger, reader);
            var message = await messageBuilder.GetStopInfoMessage();
            var result = message.ToByteArray();

            FeedMessage feedMessage = FeedMessage.Parser.ParseFrom(result);
            foreach (FeedEntity entity in feedMessage.Entity)
            {
                for (int i = 0; i < entity.TripUpdate.StopTimeUpdate.Count; i++)
                {
                    Console.WriteLine("TripID: {0}, SID: {1}, Arrival {2}", entity.TripUpdate.Trip.TripId, entity.TripUpdate.StopTimeUpdate[i].StopId, entity.TripUpdate.StopTimeUpdate[i].Arrival);
                }

            }
        }

    }
}

