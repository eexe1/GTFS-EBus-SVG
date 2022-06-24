using System;
using System.Net.Http;
using System.Threading.Tasks;
using TransitRealtime;
using Google.Protobuf;

namespace Reader
{
    class Program
    {
        static readonly HttpClient client = new();
        static async Task Main(string[] args)
        {

            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                var byteArray = await client.GetByteArrayAsync("https://us-east1-svg-ebus-realtime-gtfs.cloudfunctions.net/gtfs-trip-update?route=" + args[0]);
                
                FeedMessage feedMessage = FeedMessage.Parser.ParseFrom(byteArray);

                foreach (FeedEntity entity in feedMessage.Entity)
                {
                    Console.WriteLine("Trip ID: {0}", entity.TripUpdate.Trip.TripId);
                    Console.WriteLine("Total update in this trip: {0}", entity.TripUpdate.StopTimeUpdate.Count);
                    foreach (TripUpdate.Types.StopTimeUpdate update in entity.TripUpdate.StopTimeUpdate)
                    {
                        Console.WriteLine("Stop ID: {0}", update.StopId);
                        Console.WriteLine("Arrival Timie: {0}", update.Arrival.Time);
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

