using ParagonCodingExercise.Airports;
using System;
using System.Collections.Generic;
using System.Linq;
using ParagonCodingExercise.Events;
using System.IO;
using System.Text.Json;

namespace ParagonCodingExercise
{
    class Program
    {
        private static string AirportsFilePath = @"Resources/airports.json";

        // Location of ADS-B events
        private static string AdsbEventsFilePath = @"Resources/events.txt";

        // Write generated flights here
        private static string OutputFilePath = @"Resources/flights.txt";

        static void Main(string[] args)
        {
            Execute();

            Console.ReadKey();
        }

        private static void Execute()
        {
            var aircraftRecords = new Dictionary<string, List<AdsbEvent>>();
            
            // Load the airports
            var airports = AirportCollection.LoadFromFile(AirportsFilePath);
            
            // // Load the ADS-B events
            var events = AdsbEventCollection.LoadFromFile(AdsbEventsFilePath);
            
            // // Get all ADS-B records for each aircraft
            foreach (var e in events.Events)
            {
                if(aircraftRecords.ContainsKey(e.Identifier))
                {
                    aircraftRecords[e.Identifier].Add(e);
                }
                else
                {
                    aircraftRecords.Add(e.Identifier, new List<AdsbEvent>());
                }
            }
            
            var outFile = new StreamWriter(OutputFilePath);
            var status = "";
            var departureAirport = new Airport();
            var departureTime = DateTime.MinValue;
            // Loop through ADS-B events array for each aircraft identifier
            foreach (var log in aircraftRecords.Values.SelectMany(record => record))
            {
                const double latitude = double.NaN;
                const double longitude = double.NaN;


                var currLocation = new GeoCoordinate(log.Latitude ?? latitude, log.Longitude ?? longitude);
                if (currLocation.HasLocation())
                {
                    var closestAirport = airports.GetClosestAirport(currLocation);
                    var distanceToClosestAirport = currLocation.GetDistanceTo(new GeoCoordinate(closestAirport.Latitude, closestAirport.Longitude));
            
                    if (distanceToClosestAirport <= 2f && status == "not landed" && departureAirport.Identifier != closestAirport.Identifier)
                    {
                        status = "landed";
                        var flightRecord = new Flight
                        {
                            AircraftIdentifier = log.Identifier,
                            DepartureTime = departureTime,
                            DepartureAirport = new Airport().Identifier,
                            ArrivalTime = log.Timestamp,
                            ArrivalAirport = closestAirport.Identifier
                        };
                        var json = JsonSerializer.Serialize<Flight>(flightRecord);
                        // Console.WriteLine(json);
                        outFile.WriteLine(json);
                    } else if (status == "landed" || status == "")
                    {
                        status = "not landed";
                        departureAirport = closestAirport;
                        departureTime = log.Timestamp;
                    }

                }
            }

            outFile.Close();
            Console.WriteLine("Finished processing...");
        }
    }
}
