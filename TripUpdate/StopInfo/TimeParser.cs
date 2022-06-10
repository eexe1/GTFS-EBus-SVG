using System;
namespace BusTripUpdate.StopInfo
{
	public class TimeParser
	{
		public TimeParser()
		{
		}

		// return -1 if invalid.
		// time in seconds
		public static long ParseTime(string time)
        {
			char[] delimiterChars = { 'h', 'm', 's'};
			if (time.Contains("NO BUS"))
            {
				return -1;
            }

			if (time.Contains("ARRIVING SOON"))
            {
				return 60;
            }

			string[] values = time.Split(delimiterChars);
			if (values.Length > 3)
            {
				long hour = int.Parse(values[0]) * 3600;
				long min = int.Parse(values[1]) * 60;
				long sec = int.Parse(values[2]);

				return hour + min + sec;
			} else if (values.Length > 2)
            {
				long min = int.Parse(values[0]) * 60;
				long sec = int.Parse(values[1]);
				return min + sec;
			} else if (values.Length > 1)
            {
				long sec = int.Parse(values[0]);
				return sec;
            } else
            {
				return -1;
            }
		}

		public static long ToEpoch(long seconds)
        {

			long timestamp = (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

			return timestamp + seconds;


		}
	}
}

