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
    public class Algorithm
    {
        public Population Population { get; set; }
        public int PopulationSize { get; set; }
        public Algorithm(int popSize)
        {
            Population = Population.Generate(popSize);
            Population.ComputeFitness();
            PopulationSize = Population.Individs.Count;
        }

        public List<Individ> Sample(int sampleSize)
        {
            var randomSeed = new Random();
            return Population.Individs.AsParallel().OrderBy(x => randomSeed.Next()).Take(sampleSize).ToList();
        }

        public Tuple<Individ, Individ> Crossover(List<Individ> individs)
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
                var newPopulation = new ConcurrentBag<Individ>();

                Parallel.For(0, PopulationSize, index =>
                {
                    var siblings = Crossover(Sample(sampleSize));
                    newPopulation.Add(siblings.Item1);
                    newPopulation.Add(siblings.Item2);
                });

                Population.Individs = newPopulation.ToList();

                Population.Mutate();

                Population.ComputeFitness();

                generationNumber++;
            }

            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }
}
