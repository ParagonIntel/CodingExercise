using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParagonCodingExercise.Events
{
    public class AdsbEventCollection
    {
        public List<AdsbEvent> Events { get; set; }

        public AdsbEventCollection(List<AdsbEvent> eventsList)
        {
            Events = eventsList;
        }

        public static AdsbEventCollection LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            var eventLog = File.ReadAllLines(filePath);
            List<AdsbEvent> events = eventLog.Select(AdsbEvent.FromJson).ToList();
            
            return new AdsbEventCollection(events);
        }
    }
}