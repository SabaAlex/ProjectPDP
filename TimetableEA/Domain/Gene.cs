using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.StaticData;

namespace TimetableEA.Domain
{
    public class Gene
    {
        public int Time { get; set; }
        public int Location { get; set; }
        public int Day { get; set; }

        public static Gene Generate()
        {
            var randomSeed = new Random();
            return new Gene()
            {
                Time = randomSeed.Next(0, AlgorithmData.TimeDecoded.Count),
                Day = randomSeed.Next(0, AlgorithmData.DayDecoded.Count),
                Location = randomSeed.Next(0, AlgorithmData.LocationNumber),
            };
        }

        public override bool Equals(object obj)
        {
            return obj is Gene gene &&
                   Time == gene.Time &&
                   Location == gene.Location &&
                   Day == gene.Day;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Time, Location, Day);
        }

        public override string ToString()
        {
            return $"Time: {AlgorithmData.TimeDecoded[Time]}, Day: {AlgorithmData.DayDecoded[Day]}, Location: {Location}";
        }
    }
}
