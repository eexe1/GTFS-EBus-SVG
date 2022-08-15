using System.Threading.Tasks;
using BusTripUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests
{
    [TestClass]
    public class StopInfoReaderTests
    {
        [TestMethod]
        public async Task TestRemoteStopInfoReaderAsync()
        {
            RemoteStopInfoReader readerA = new(IStopInfoReader.Route.Windward);
            RemoteStopInfoReader readerB = new(IStopInfoReader.Route.Leeward);
            var windwardList = await readerA.RetrieveStopInfoAsync();
            var leedwardList = await readerB.RetrieveStopInfoAsync();

            Assert.AreEqual(windwardList.Count > 0 && leedwardList.Count > 0, true);
        }

        [TestMethod]
        public void TestStopMapping()
        {
            IStopInfoReader readerA = new RemoteStopInfoReader(IStopInfoReader.Route.Windward);
            var windwardStopID = readerA.FindSIDBySeq("1", IStopInfoReader.Route.Windward);
            Assert.AreEqual(windwardStopID, "106_171");

            var leewardStopID = readerA.FindSIDBySeq("1", IStopInfoReader.Route.Leeward);
            Assert.AreEqual(leewardStopID, "107_166");
        }
    }
}

