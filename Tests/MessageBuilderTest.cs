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
        public async Task TestDecodingProtocolBufferMessage()
        {
            SampleStopInfoReader reader = new();
            var logger = NullLogger.Instance;
            MessageBuilder messageBuilder = new(logger, reader);
            var message = await messageBuilder.GetStopInfoMessage();
            var result = message.ToByteArray();

            // decode protocol buffer message
            FeedMessage _ = FeedMessage.Parser.ParseFrom(result);
        }


    }
}

