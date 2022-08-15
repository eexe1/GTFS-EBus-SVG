using System;
using static BusTripUpdate.TimeTableStopInfo;

namespace BusTripUpdate
{
    /// <summary>
    /// Represents an entry received from StopInfo API.
    /// Each StopInfo contains the arrival estimate and basic information.
    /// </summary>
    public class StopInfo
    {

        public class BusInfo
        {
            public string No { get; set; }
            public float Lat { get; set; }
            public float Lon { get; set; }
            public double? Tm { get; set; }
            /// <summary>
            /// Sid from the eBus system is not usable as our Stop ID is different
            /// </summary>
            public int Sid { get; set; }
            public int Seq { get; set; }
            public string Alias { get; set; }
        }

        /// <summary>
        /// Route Id: 4 is Windward, 5 is Leeward
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Stop sequence used to identity the bus stop.
        /// Sequence is unique in each route.
        /// </summary>
        public int Seq { get; set; }
        /// <summary>
        /// Estimate of arrival time in format of XhYmZs.
        /// </summary>
        public string Est { get; set; }

        /// <summary>
        /// Contains buses' basic info and coordinate
        /// </summary>
        public BusInfo[] Bno { get; set; }

        /// <summary>
        /// Indicates the direction of the upcoming bus.
        /// </summary>
        public Direction Direction
        {
            get
            {
                // Windward
                if (Id == Constants.windwardRouteId)
                {
                    if (Seq > Constants.windwardEoS)
                    {
                        return Direction.Inbound;
                    }

                    return Direction.Outbound;
                } else if (Id == Constants.leewardRouteId)
                // Leeward
                {
                    
                    if (Seq > Constants.leewardEoS)
                    {
                        return Direction.Inbound;
                    }
                    
                    return Direction.Outbound;
                }
                else
                {
                    throw new NotSupportedException(message: "Unable to get direction");
                }
            }
        }
    }

   
}

