using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TimetableEA.Domain;
using TimetableEA.EA.Extensions;

namespace TimetableEA.EA.Logic
{
    public class ParallelAlgorithm : IAlgorithm
    {
        private Population Population { get; set; }
        private int PopulationSize { get; set; }
        public ParallelAlgorithm(int popSize)
        {
            Population = Population.Generate(popSize);
            Population.ComputeFitness();
            PopulationSize = Population.Individs.Count;
        }

        private List<Individ> Sample(int sampleSize)
        {
            var randomSeed = new Random();
            return Population.Individs.OrderBy(x => randomSeed.Next()).Take(sampleSize).ToList();
        }

        private Tuple<Individ, Individ> Crossover(List<Individ> individs)
        {
            var sortedSample = individs.OrderBy(individ => individ.Fitness);

            return individs[individs.Count - 1].Crossover(individs[individs.Count - 2]);
        }

        public Individ Fittest()
        {
            return Population.Fittest();
        }

        public long StartAlgorithm(int generations, int sampleSize)
        {
            var generationNumber = 0;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (generations != generationNumber && Population.Fittest().Fitness != 0) 
            {
                var newPopulation = new Individ[PopulationSize];

                for(var i = 0; i < PopulationSize; i += 2)
                {
                    var siblings = Crossover(Sample(sampleSize));
                    newPopulation[i] = siblings.Item1;
                    newPopulation[i + 1] = siblings.Item2;
                }

                Population.Individs = newPopulation.ToList();

                Population.Mutate();

                Population.ComputeFitness();

                generationNumber++;
            }

            Console.WriteLine($"Generation: {generationNumber}");

            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }
}
