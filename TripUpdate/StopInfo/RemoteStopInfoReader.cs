using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusTripUpdate
{
    /// <summary>
    /// Read StopInfo data from the StopInfo API.
    /// </summary>
	public class RemoteStopInfoReader: IStopInfoReader
	{
        private readonly IStopInfoReader.Route _route;
        private readonly HttpClient _client = new();

        public RemoteStopInfoReader(IStopInfoReader.Route route)
		{
            _route = route;
		}

        public IStopInfoReader.Route GetRoute()
        {
            return _route;
        }

        public async Task<List<StopInfo>> RetrieveStopInfoAsync()
        {
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

                string jsonString = await _client.GetStringAsync(url);

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

