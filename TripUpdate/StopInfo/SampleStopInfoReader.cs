using System;
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

        public IStopInfoReader.Route GetRoute()
        {
            return IStopInfoReader.Route.Windward;
        }

        Task<List<StopInfo>> IStopInfoReader.RetrieveStopInfoAsync()
        {

            var task = new Task<List<StopInfo>>(() =>
            {
                string jsonString = File.ReadAllText(@"./TestData/stopinfo.json");

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

