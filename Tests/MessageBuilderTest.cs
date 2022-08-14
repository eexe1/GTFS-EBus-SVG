using System;
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
            MessageBuilder messageBuilder = new(logger, readers);
            messageBuilder.CurrentTimestampOverride = TimeHelper.TimeFromEpoch(1654006760 + 60);
            var message = await messageBuilder.GetVehiclePositionMessage();

            Assert.AreEqual(message.Entity[0].Vehicle.Trip.TripId, "WF_4");
            Assert.AreEqual(message.Entity[0].Vehicle.StopId, "106_171");
            Assert.AreEqual(message.Entity[0].Vehicle.Vehicle.Id, "HY789");

        }
    }
}

