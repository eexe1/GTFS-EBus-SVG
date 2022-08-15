using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusTripUpdate
{
    /// <summary>
    /// Interface of StopInfo reader
    /// </summary>
    public interface IStopInfoReader
    {

        /// <summary>
        /// An async task to retrieve StopInfo
        /// </summary>
        /// <returns>A list of StopInfo</returns>
        Task<List<StopInfo>> RetrieveStopInfoAsync();

        enum Route
        {
            Windward,
            Leeward
        }

        /// <summary>
        /// Gets the route that the reader is assigned to
        /// </summary>
        /// <returns></returns>
        Route GetRoute();

        /// <summary>
        /// Find Stop ID by giving a bus stop's sequence number and the route it belongs to
        /// </summary>
        /// <param name="seq">Bus stop's sequence number</param>
        /// <param name="route">Bus stop's route</param>
        /// <returns></returns>
        public string FindSIDBySeq(string seq, Route route)
        {
            // read map files
            string jsonString = null;
            switch (route)
            {
                case Route.Windward:
                    {
                        jsonString = File.ReadAllText(@"./ReferenceData/stop_windward_map.json");
                        break;
                    }
                case Route.Leeward:
                    {
                        jsonString = File.ReadAllText(@"./ReferenceData/stop_leeward_map.json");
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

