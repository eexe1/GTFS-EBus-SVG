using System;
namespace BusTripUpdate
{
	public struct Constants
	{
		internal static string stopInfoWindwardUrl = "https://ebus.gov.vc/stopapi/webstopinfo?rid=4";
		internal static string stopInfoLeewardUrl = "https://ebus.gov.vc/stopapi/webstopinfo?rid=5";
		// seq
		internal static int windwardEoS = 41;
		internal static int leewardEoS = 24;
		// sid
		internal static int windwardEoSId = 106;
		internal static int windwardExceptionSId = 171;

	}
}

