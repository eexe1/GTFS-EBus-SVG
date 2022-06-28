using System;
using System.Collections.Generic;
using System.Globalization;
using BusTripUpdate.StopInfo;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

namespace BusTripUpdate
{

    public class TimeTableStopInformation
    {
        public enum Direction
        {
            Inbound,
            Outbound
        }
        public string tripId;
        public string arrivalTime;
        public Direction direction;

    }

    public class TimeTable
    {

        public TimeTable()
        {
            dictionary = new Dictionary<string, List<TimeTableStopInformation>>();
        }
#nullable enable
        public ILogger? logger;
#nullable disable

        // stopId as key
        public Dictionary<string, List<TimeTableStopInformation>> dictionary;

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

                if (tripId.Contains("L"))
                {
                    if (int.Parse(stopSequence) > Constants.leewardEoS)
                    {
                        direction = TimeTableStopInformation.Direction.Inbound;
                    }
                }
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
                    newList.Add(new TimeTableStopInformation { tripId = tripId, arrivalTime = arrivalTime });
                    timeTable.dictionary[stopId] = newList;

                }

            }

            return timeTable;
        }
#nullable enable
        public string? FindNearestTripId(string stopId, DateTime estimateTime, TimeTableStopInformation.Direction direction)
        {

            string? result = null;

            if (dictionary.TryGetValue(stopId, out List<TimeTableStopInformation>? list))
            {
                CultureInfo provider = CultureInfo.CurrentCulture;

                // arrival time must not be later than the fixed arrival time by x minutes.
                // otherwise, it is considered an inconceivable estimate.
                double min = 60;

                list.ForEach(delegate (TimeTableStopInformation info)
                {

                    if (direction == info.direction)
                    {
                        // Sunday schedule
                        if (estimateTime.DayOfWeek == DayOfWeek.Sunday && info.tripId.Contains("S")
                            || estimateTime.DayOfWeek != DayOfWeek.Sunday && !info.tripId.Contains("S"))
                        {
                            // This is in AST time
                            var value = String.Format("{0}-04", info.arrivalTime);
                            var fixedArrivalTime = DateTime.ParseExact(value, "H:mm:sszz", provider);
                            // compare with current time

                            var difference = estimateTime.Subtract(fixedArrivalTime).TotalMinutes;

                            if (difference < min && difference > 0)
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

