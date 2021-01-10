using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;
using TimetableEA.StaticData;

namespace TimetableEA.EA.Extensions
{
    public static class GeneExtension
    {
        public static double MUTATION_RATE { get; set; } = 0.05;

        public static double CROSSOVER_SELECTION_RATE { get; set; } = 0.5;

        public static void Mutate(this Gene gene)
        {
            var randomSeed = new Random();

            var timeProb = randomSeed.NextDouble();
            var dayProb = randomSeed.NextDouble();
            var locationProb = randomSeed.NextDouble();

            if(timeProb <= MUTATION_RATE)
                gene.Time = randomSeed.Next(0, AlgorithmData.TimeDecoded.Count);
               
            if(dayProb <= MUTATION_RATE)
                gene.Day = randomSeed.Next(0, AlgorithmData.DayDecoded.Count);

            if(locationProb <= MUTATION_RATE)
                gene.Location = randomSeed.Next(0, AlgorithmData.LocationNumber);
        }

        public static Tuple<Gene, Gene> Crossover(this Gene fatherGene, Gene motherGene)
        {
            var randomSeed = new Random();

            var timeProb = randomSeed.NextDouble();
            var dayProb = randomSeed.NextDouble();
            var locationProb = randomSeed.NextDouble();

            return new Tuple<Gene, Gene>(
                new Gene()
                {
                    Time = timeProb <= CROSSOVER_SELECTION_RATE ? fatherGene.Time : motherGene.Time,
                    Day = dayProb <= CROSSOVER_SELECTION_RATE ? fatherGene.Day : motherGene.Day,
                    Location = locationProb <= CROSSOVER_SELECTION_RATE ? fatherGene.Location : motherGene.Location
                },
                new Gene()
                {
                    Time = timeProb <= CROSSOVER_SELECTION_RATE ? motherGene.Time : fatherGene.Time,
                    Day = dayProb <= CROSSOVER_SELECTION_RATE ? motherGene.Day : fatherGene.Day,
                    Location = locationProb <= CROSSOVER_SELECTION_RATE ? motherGene.Location : fatherGene.Location
                });
        }
    }
}
