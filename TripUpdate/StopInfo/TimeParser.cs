using System;
namespace BusTripUpdate
{
	/// <summary>
    /// Parse custome time format
    /// </summary>
	public class TimeParser
	{
		/// <summary>
		/// Convert time in (xh:ym:zs) format to seconds
		/// </summary>
		/// <param name="time">time string in format of xh:ym:zs</param>
		/// <returns>seconds; return -1 if fails to parse</returns>
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

		/// <summary>
        /// Returns current time + offset in Epoch format
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns>seconds</returns>
		public static long ToEpoch(long offset)
        {

			long timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

			return timestamp + offset;

		}
	}
}

