using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BusTripUpdate.StopInfoReader
{
    public class SampleStopInfoReader : IStopInfoReader
    {
        public SampleStopInfoReader()
        {
        }


        List<StopInfo> IStopInfoReader.RetrieveStopInfo()
        {

            string jsonString = File.ReadAllText(@"./TestData/stopinfo.json");

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            List<StopInfo> stopInfoList = JsonSerializer
                .Deserialize<List<StopInfo>>(jsonString, serializeOptions);

            return stopInfoList;

        }
    }
}

