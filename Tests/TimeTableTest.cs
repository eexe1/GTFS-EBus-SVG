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
        public void TestFindNearestTripIdWF_6()
        {
            var timeTable = TimeTable.GetTimeTable();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var estimateTime = DateTime.ParseExact("12:00:05-4:00", "H:mm:sszzz", provider);
            var result = timeTable.FindNearestTripId("24", estimateTime, TimeTableStopInformation.Direction.Outbound);
            Assert.AreEqual("WF_6", result);
        }

        [TestMethod]
        public void TestFindNearestTripIdWF_11()
        {
            var timeTable = TimeTable.GetTimeTable();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var estimateTime = DateTime.ParseExact("16:57:35-4:00", "H:mm:sszzz", provider);

            var result = timeTable.FindNearestTripId("29_88", estimateTime, TimeTableStopInformation.Direction.Outbound);
            Assert.AreEqual("WF_11", result);
        }

        [TestMethod]
        public void TestFindNearestTripIdForSundayNone()
        {
            var timeTable = TimeTable.GetTimeTable();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var estimateTime = DateTime.ParseExact("08/07 20:04:00-4:00", "MM/dd H:mm:sszzz", provider);
            var result = timeTable.FindNearestTripId("40_71", estimateTime, TimeTableStopInformation.Direction.Outbound);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestFindNearestTripIdForSundayWFS_4()
        {
            var timeTable = TimeTable.GetTimeTable();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var estimateTime = DateTime.ParseExact("08/07 14:18:00-4:00", "MM/dd H:mm:sszzz", provider);
            var result = timeTable.FindNearestTripId("40_71", estimateTime, TimeTableStopInformation.Direction.Outbound);
            Assert.AreEqual("WFS_4", result);
        }

    }
}

