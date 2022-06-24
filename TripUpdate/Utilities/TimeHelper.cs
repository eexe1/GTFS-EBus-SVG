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

            try
            {
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/St_Vincent");
            }

            return atlanticTimeZone;
        }

        public static DateTime CurrentTimeInAST()
        {
            TimeZoneInfo atlanticTimeZone;

            try
            {
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                atlanticTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/St_Vincent");
            }

            return  TimeZoneInfo.ConvertTime(DateTime.Now, atlanticTimeZone);
        }

    }
}

