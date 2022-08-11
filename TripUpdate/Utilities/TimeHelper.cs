using System;
namespace BusTripUpdate.Utilities
{
    public class TimeHelper
    {
        public TimeHelper()
        {
        }

        public static TimeZoneInfo GetASTZone()
        {
            TimeZoneInfo atlanticTimeZone;

            // Linux & MacOS machine from ICU
            try
            {
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
            // Windows machines
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/St_Vincent");
            }

            return atlanticTimeZone;
        }

        public static DateTime CurrentTimeInAST()
        {
          return TimeToAST(DateTime.Now);
        }

        public static DateTime TimeToAST(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, GetASTZone());
        }

    }
}