using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TransitRealtime;
using Google.Protobuf;
using System.Collections;
using Google.Protobuf.Collections;

namespace Reader
{
    class Program
    {
        static readonly HttpClient client = new();
        static async Task Main()
        {

            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string test = await client.GetStringAsync("http://127.0.0.1:8080");

                ByteString byteString = ByteString.FromBase64(test);
                FeedMessage feedMessage = FeedMessage.Parser.ParseFrom(byteString);

                foreach (FeedEntity entity in feedMessage.Entity)
                {
                    Console.WriteLine(entity.TripUpdate.Trip.TripId);
                    Console.WriteLine(entity.TripUpdate.StopTimeUpdate.Count);
                    foreach (TripUpdate.Types.StopTimeUpdate update in entity.TripUpdate.StopTimeUpdate)
                    {
                        Console.WriteLine(update.StopId);
                        Console.WriteLine(update.Arrival.Time);
                    }
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}

