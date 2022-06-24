using static BusTripUpdate.TimeTableStopInformation;

namespace BusTripUpdate.StopInfo
{
    public class StopInfo
    {
        // Stop sequence
        public int Id { get; set; }
        public int Seq { get; set; }
        public string Est { get; set; }

        public Direction Direction
        {
            get
            {
                // Windward
                if (Id <= Constants.windwardEoSId || Id == Constants.windwardExceptionSId)
                {
                    if (Seq > Constants.windwardEoS)
                    {
                        return Direction.Inbound;
                    }

                    return Direction.Outbound;
                } else
                // Leeward
                {
                    if (Seq > Constants.leewardEoS)
                    {
                        return Direction.Inbound;
                    }

                    return Direction.Outbound;
                }
            }
        }
    }

   
}

