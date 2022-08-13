using System;
namespace BusTripUpdate.Utilities
{
    public class TimeHelper
    {

        public static TimeZoneInfo GetASTZone()
        {
            TimeZoneInfo atlanticTimeZone;


            try
            {
                // Windows machines
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Linux & MacOS machine from ICU
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