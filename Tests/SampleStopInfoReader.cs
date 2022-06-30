using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusTripUpdate.StopInfo
{
    public class SampleStopInfoReader : IStopInfoReader
    {
        public SampleStopInfoReader()
        {
        }

        public string fileUrl = @"./TestData/stopinfo.json";

        public IStopInfoReader.Route route = IStopInfoReader.Route.Windward;

        public IStopInfoReader.Route GetRoute()
        {
            return route;
        }

        Task<List<StopInfo>> IStopInfoReader.RetrieveStopInfoAsync()
        {

            var task = new Task<List<StopInfo>>(() =>
            {
                string jsonString = File.ReadAllText(fileUrl);

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                List<StopInfo> stopInfoList = JsonSerializer
                    .Deserialize<List<StopInfo>>(jsonString, serializeOptions);
                return stopInfoList;

            });

            task.RunSynchronously();
            return task;
        }

    }
}

