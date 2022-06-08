using System;
using System.Text.Json.Serialization;

namespace BusTripUpdate.StopInfoReader
{
    public class StopInfo
    {
        // Stop sequence
        public int Id { get; set; }
        public int Seq { get; set; }
        public string Est { get; set; }
    }

   
}

