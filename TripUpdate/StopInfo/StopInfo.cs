using System;
using static BusTripUpdate.TimeTableStopInformation;

namespace BusTripUpdate.StopInfo
{
    public class StopInfo
    {
        // Route Id: 4 is Windward, 5 is Leeward
        public int Id { get; set; }
        // Stop sequence
        public int Seq { get; set; }
        public string Est { get; set; }

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

