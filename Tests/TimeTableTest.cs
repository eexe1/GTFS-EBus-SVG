using System;
using System.Globalization;
using BusTripUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TimeTableTest
    {
        [TestMethod]
        public void TestFindNearestTripId1()
        {
            var timeTable = TimeTable.GetTimeTable();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var estimateTime = DateTime.ParseExact("12:00:05-4:00", "H:mm:sszzz", provider);
            var result = timeTable.FindNearestTripId("24", estimateTime, TimeTableStopInformation.Direction.Outbound);
            Assert.AreEqual("WF_5", result);
        }

        [TestMethod]
        public void TestFindNearestTripId2()
        {
            var timeTable = TimeTable.GetTimeTable();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var estimateTime = DateTime.ParseExact("16:57:35-4:00", "H:mm:sszzz", provider);

            var result = timeTable.FindNearestTripId("29_88", estimateTime, TimeTableStopInformation.Direction.Outbound);
            Assert.AreEqual("WF_10", result);
        }

    }
}

