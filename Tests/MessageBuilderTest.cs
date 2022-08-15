using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusTripUpdate;
using BusTripUpdate.Utilities;
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
        public async Task TestTripUpdateMessage()
        {
            SampleStopInfoReader reader = new();
            var logger = NullLogger.Instance;
            IStopInfoReader[] readers = { reader };
            MessageBuilder messageBuilder = new(logger, readers);
            var message = await messageBuilder.GetTripUpdateMessage();
            var result = message.ToByteArray();

            FeedMessage _ = FeedMessage.Parser.ParseFrom(result);
        }

        [TestMethod]
        public async Task TestVehiclePositionMessage()
        {
            SampleStopInfoReader reader = new();
            var logger = NullLogger.Instance;
            IStopInfoReader[] readers = { reader };
            MessageBuilder messageBuilder = new(logger, readers)
            {
                CurrentTimestampOverride = TimeHelper.TimeFromEpoch(1654006760 + 60)
            };
            var message = await messageBuilder.GetVehiclePositionMessage();

            Assert.AreEqual(message.Entity[0].Vehicle.Trip.TripId, "WF_4");
            Assert.AreEqual(message.Entity[0].Vehicle.StopId, "106_171");
            Assert.AreEqual(message.Entity[0].Vehicle.Vehicle.Id, "HY789");

        }


        public struct StopResponse
        {
            public string StopId;
            public string ArrivalTime;

            public StopResponse(string stopId, string arrivalTime)
            {
                StopId = stopId;
                ArrivalTime = arrivalTime;
            }
        }

        [TestMethod]
        public async Task TestLeewardTripUpdateMessage()
        {

            string[] tripIds = { "LF_4", "LF_3", "FL_2" };
            StopResponse[] stops1 = {
                new StopResponse("107_166", "1660579295"),
                new StopResponse("109_164", "1660579290"),
                new StopResponse("110", "1660579289"),
                new StopResponse("111", "1660579288"),
                new StopResponse("112", "1660579287"),
                new StopResponse("113", "1660579286"),
                new StopResponse("114_160", "1660579285")};


            StopResponse[] stops2 = {
                new StopResponse("134_139", "1660580840"),
                new StopResponse("135_138", "1660580777"),
                new StopResponse("136", "1660580480")};

            StopResponse[] stops3 = {
                new StopResponse("128_146", "1660579290"),
                new StopResponse("127_147", "1660579289"),
                new StopResponse("126", "1660579288"),
                new StopResponse("148", "1660579287"),
                new StopResponse("149", "1660579286"),
                new StopResponse("124_150", "1660579285"),
                new StopResponse("156", "1660579284"),
                new StopResponse("151", "1660579283"),
                new StopResponse("121_152", "1660579282"),
                new StopResponse("153", "1660579281"),
                new StopResponse("118_155", "1660579280"),
                new StopResponse("154", "1660579279"),
                new StopResponse("117_178", "1660579278"),
                new StopResponse("116_177", "1660579277"),
                new StopResponse("159", "1660579276")};

            Dictionary<string, StopResponse[]> dict = new()
            {
                { tripIds[0], stops1 },
                { tripIds[1], stops2 },
                { tripIds[2], stops3 }
            };

            SampleStopInfoReader reader = new()
            {
                fileUrl = @"./TestData/stop_info_leeward.json",
                route = IStopInfoReader.Route.Leeward
            };
            var logger = NullLogger.Instance;
            IStopInfoReader[] readers = { reader };
            MessageBuilder messageBuilder = new(logger, readers)
            {
                CurrentTimestampOverride = TimeHelper.TimeFromEpoch(1660579275)
            };
            var message = await messageBuilder.GetTripUpdateMessage();

            int i = 0;
            foreach (var entity in message.Entity)
            {

                Assert.AreEqual(tripIds[i], entity.TripUpdate.Trip.TripId);

                var stops = dict[entity.TripUpdate.Trip.TripId];
                int j = 0;

                foreach (var update in entity.TripUpdate.StopTimeUpdate)
                {
                    Assert.AreEqual(stops[j].StopId, update.StopId);
                    Assert.AreEqual(stops[j].ArrivalTime, update.Arrival.Time.ToString());

                    j++;
                }
                i++;
            }
        }
    }
}

