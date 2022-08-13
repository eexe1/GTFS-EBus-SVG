namespace BusTripUpdate
{
	public struct Constants
	{
		internal static string stopInfoWindwardUrl = "https://ebus.gov.vc/stopapi/webstopinfo?rid=4";
		internal static string stopInfoLeewardUrl = "https://ebus.gov.vc/stopapi/webstopinfo?rid=5";
		// seq, windward end of stop seq for the outbound direction
		internal static int windwardEoS = 41;
		internal static int leewardEoS = 24;

		// route id
		internal static int windwardRouteId = 4;
		internal static int leewardRouteId = 5;

	}
}

