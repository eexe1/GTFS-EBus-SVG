using System.Threading.Tasks;
using BusTripUpdate;
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
            IStopInfoReader[] readers = { reader };
            MessageBuilder messageBuilder = new(logger, readers);
            var message = await messageBuilder.GetTripUpdateMessage();
            var result = message.ToByteArray();

            // decode protocol buffer message
            FeedMessage _ = FeedMessage.Parser.ParseFrom(result);
        }


    }
}

