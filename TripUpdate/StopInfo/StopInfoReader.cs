using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Protobuf;
using TransitRealtime;

namespace BusTripUpdate.StopInfo
{
	public class StopInfoReader: IStopInfoReader
	{
        public readonly IStopInfoReader.Route _route;
        private readonly HttpClient client = new();

        public StopInfoReader(IStopInfoReader.Route route)
		{
            _route = route;
		}

        public IStopInfoReader.Route GetRoute()
        {
            return _route;
        }

        public async Task<List<StopInfo>> RetrieveStopInfoAsync()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                
                string url = "";

                switch (_route)
                {
                    case IStopInfoReader.Route.Windward:
                        url = Constants.stopInfoWindwardUrl;
                        break;
                    case IStopInfoReader.Route.Leeward:
                        url = Constants.stopInfoLeewardUrl;
                        break;
                }

                string jsonString = await client.GetStringAsync(url);

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                List<StopInfo> stopInfoList = JsonSerializer
                    .Deserialize<List<StopInfo>>(jsonString, serializeOptions);
                return stopInfoList;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return new List<StopInfo>();
            }
        }
    }
}

