using BusTripUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace Tests
{
    [TestClass]
    public class TimeParserTest
    {
        [TestMethod]
        public void TestHourMinuteSecond()
        {
            Console.WriteLine("Test Hour Min Sec");
            long interval = TimeParser.ParseTime("2h22m58s");
            Assert.AreEqual(8578, interval);
        }
        [TestMethod]
        public void TestMinuteSecond()
        {
            Console.WriteLine("Test Min Sec");
            long interval = TimeParser.ParseTime("19m58s");
            Assert.AreEqual(1198, interval);
        }
        [TestMethod]
        public void TestSecond()
        {
            Console.WriteLine("Sec");
            long interval = TimeParser.ParseTime("58s");
            Assert.AreEqual(58, interval);
        }
    }
}

