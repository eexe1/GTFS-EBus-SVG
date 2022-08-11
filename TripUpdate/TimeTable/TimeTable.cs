using System;
using System.Collections.Generic;
using System.Globalization;
using BusTripUpdate.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

namespace BusTripUpdate
{
    /// <summary>
    /// Models an arriving bus at a stop.
    /// </summary>
    public class TimeTableStopInformation
    {
        /// <summary>
        /// Inbound (returning to Kingstown);
        /// Outbound (leaving Kingstown)
        /// </summary>
        public enum Direction
        {
            Inbound,
            Outbound
        }
        /// <summary>
        /// Trip ID attached to the bus
        /// </summary>
        public string tripId;
        /// <summary>
        /// The scheduled arrival time
        /// </summary>
        public string arrivalTime;
        /// <summary>
        /// The direction of the bus
        /// </summary>
        public Direction direction;

    }

    /// <summary>
    /// Models the bus timetable.
    /// </summary>
    public class TimeTable
    {

        public TimeTable()
        {
            dictionary = new Dictionary<string, List<TimeTableStopInformation>>();
        }
#nullable enable
        public ILogger? logger;
#nullable disable

        /// <value>Stores TimeTable information keyed by StopId. Each stop has a list of bus arriving information</value> 
        public Dictionary<string, List<TimeTableStopInformation>> dictionary;

        /// <summary>
        /// Gets a TimieTable instance
        /// </summary>
        /// <returns>TimeTable</returns>
        public static TimeTable GetTimeTable()
        {

            var path = @"./ReferenceData/stop_times.txt";
            using TextFieldParser csvParser = new(path);
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            csvParser.ReadLine();

            TimeTable timeTable = new();


            while (!csvParser.EndOfData)
            {
                // Read current line fields, pointer moves to the next line.
                string[] fields = csvParser.ReadFields();
                string tripId = fields[0];
                string arrivalTime = fields[1];
                string departureTime = fields[2];
                string stopId = fields[3];
                string stopSequence = fields[4];

                var direction = TimeTableStopInformation.Direction.Outbound;

                // Leeward side
                if (tripId.Contains("L"))
                {
                    if (int.Parse(stopSequence) > Constants.leewardEoS)
                    {
                        direction = TimeTableStopInformation.Direction.Inbound;
                    }
                }
                // Windward side
                else
                {
                    if (int.Parse(stopSequence) > Constants.windwardEoS)
                    {
                        direction = TimeTableStopInformation.Direction.Inbound;
                    }
                }

                if (timeTable.dictionary.TryGetValue(stopId, out List<TimeTableStopInformation> list))
                {
                    list.Add(new TimeTableStopInformation { tripId = tripId, arrivalTime = arrivalTime, direction = direction });
                    timeTable.dictionary[stopId] = list;
                }
                else
                {
                    List<TimeTableStopInformation> newList = new();
                    newList.Add(new TimeTableStopInformation { tripId = tripId, arrivalTime = arrivalTime, direction = direction });
                    timeTable.dictionary[stopId] = newList;

                }

            }

            return timeTable;
        }

        /** 
        <summary>
        Find the nearest Trip based on the given parameters.
        </summary>
        <param name="stopId">Stop ID</param>
        <param name="estimateTime">The estimate arriving time from the eBus system</param>
        <param name="direction">The direction the Trip belongs to</param>
        */
#nullable enable
        public string? FindNearestTripId(string stopId, DateTime estimateTime, TimeTableStopInformation.Direction direction)
        {

            string? result = null;

            if (dictionary.TryGetValue(stopId, out List<TimeTableStopInformation>? list))
            {
                CultureInfo provider = CultureInfo.CurrentCulture;

                // arrival time must not be later/earlier than the fixed arrival time by x minutes.
                // otherwise, it is considered an inconceivable estimate.
                double min = 60;

                list.ForEach(delegate (TimeTableStopInformation info)
                {

                    if (direction == info.direction)
                    {
                        // convert to AST so Sunday trip can be identified.
                        var astTime = TimeHelper.TimeToAST(estimateTime);
                        var utcTime = estimateTime.ToUniversalTime();

                        /* Only 2 types of Schedule: Mon-Sat & Sun
                         * The condition enforces which schedule to apply
                         */
                        if (astTime.DayOfWeek == DayOfWeek.Sunday && info.tripId.Contains("S")
                            || astTime.DayOfWeek != DayOfWeek.Sunday && !info.tripId.Contains("S"))
                        {

                            /* Compare time only (date component is not relevant as every day has the same timetable)
                            */
                            var hour = astTime.Hour.ToString("D2");
                            var minute = astTime.Minute.ToString("D2");
                            var second = astTime.Second.ToString("D2");

                            var value = String.Format("{0}-04:00", info.arrivalTime);
                            var estimateValue = String.Format("{0}:{1}:{2}-04:00", hour, minute, second);

                            var fixedArrivalTime = DateTime.ParseExact(value, "H:mm:sszzz", provider).ToUniversalTime();
                            var finalEstimateTime = DateTime.ParseExact(estimateValue, "H:mm:sszzz", provider).ToUniversalTime();

                            // Ensure times are in UTC so subtract works correctly
                            // find the closet time no matter whether it is before or ahead
                            var difference = Math.Abs(finalEstimateTime.Subtract(fixedArrivalTime).TotalMinutes);

                            if (difference < min)
                            {
                                min = difference;
                                result = info.tripId;
                            }

                        }

                    }

                });
            }

            return result;

        }
#nullable disable

    }
}

