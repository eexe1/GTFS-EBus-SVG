using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BusTripUpdate.StopInfoReader
{
    public interface IStopInfoReader
    {
        List<StopInfo> RetrieveStopInfo();

        enum Route
        {
            Windward,
            Leeward
        }

        string FindSIDBySeq(string seq, Route route)
        {
            // read map files
            string jsonString = null;
            switch (route)
            {
                case Route.Windward:
                    {
                        jsonString = File.ReadAllText(@"./ReferenceData/stop-windward-map.json");
                        break;
                    }
                case Route.Leeward:
                    {
                        jsonString = File.ReadAllText(@"./ReferenceData/stop-leeward-map.json");
                        break;
                    }
            }


            Dictionary<string, string> keyValuePairs =
                JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            string value = keyValuePairs.ContainsKey(seq) ? keyValuePairs[seq] : "";

            return value;
        }
    }
}

