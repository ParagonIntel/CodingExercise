using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ParagonCodingExercise.Airports
{
    public class AirportCollection
    {
        public List<Airport> Airports {get; set;}
        
        public AirportCollection(List<Airport> airports)
        {
            Airports = airports;
        }

        public static AirportCollection LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            using TextReader reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();

            var airports = JsonSerializer.Deserialize<List<Airport>>(json);
            return new AirportCollection(airports);
        }

        public Airport GetClosestAirport(GeoCoordinate coordinate)
        {
            var closestAirport = Airports[0];
            var closestAirportCoords = new GeoCoordinate(closestAirport.Latitude, closestAirport.Longitude);
            var minDistance = coordinate.GetDistanceTo(closestAirportCoords);

            // Loop through all airports to find the smallest distance
            foreach (var airport in Airports)
            {
                var currDistance = coordinate.GetDistanceTo(new GeoCoordinate(airport.Latitude, airport.Longitude));
                if (!(currDistance < minDistance)) continue;
                closestAirport = airport;
                minDistance = currDistance;
            }
            return closestAirport;
        }
    }
}