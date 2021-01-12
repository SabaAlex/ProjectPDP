using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;
using TimetableEA.EA.Logic.Distributed.Models;
using TimetableEA.EA.Logic.Distributed.Services;
using System.Linq;
using System.Diagnostics;

namespace TimetableEA.EA.Logic
{
    public class DistributedAlgorithm : IAlgorithm
    {
        private Population Population { get; set; }
        private int PopulationSize { get; set; }
        private Engine engine;
        private Individ bestIndivid;

        public DistributedAlgorithm(int popSize)
        {
            engine = Engine.Instance;
            engine.Initialize(); 
            
            Population = Population.Generate(popSize);
            CalculateFitness();
            PopulationSize = Population.Individs.Count;
        }

        public Individ Fittest()
        {
            return bestIndivid;
        }

        public int BestFitness()
        {
            var bestFitnessPop = Int32.MaxValue;
            Individ bestIndividPop = null;
            Population.Individs.ForEach(individ =>
            {
                

                if (bestFitnessPop > individ.Fitness)
                {
                    bestFitnessPop = individ.Fitness;
                    bestIndividPop = individ;
                }
            });

            if (bestIndivid == null)
                bestIndivid = bestIndividPop;
            else if(bestIndividPop != null && bestIndivid.Fitness > bestIndividPop.Fitness)
                bestIndivid = bestIndividPop;
            return bestFitnessPop;
        }

        public Tuple<Individ, Individ> ChoseTwo(int sample)
        {
            Random rand = new Random();
            var randList = new List<int>();
            for (var index = 0; index < sample; index++)
            {
                randList.Add(rand.Next(0, Population.Individs.Count));
            }

            var selection = randList.OrderBy(index => Population.Individs[index].Fitness).Select(index => Population.Individs[index]).ToList();
            return Tuple.Create(selection[0], selection[1]);
        }

        public long StartAlgorithm(int generations, int sampleSize)
        {
            bestIndivid = null;
            var generationNumber = 0;

            var watch = new Stopwatch();
            watch.Start();

            while (generations != generationNumber && BestFitness() != 0)
            {
                var newPopulation = new ConcurrentBag<Individ>();

                for (var i = 0; i < Population.Individs.Count / 2; i++)
                {
                    var parents = ChoseTwo(sampleSize);
                    var message = new Message()
                    {
                        Command = "childrens",
                        Individ1 = parents.Item1,
                        Individ2 = parents.Item2
                    };

                    engine.ExecutorService.ScheduleTask(message, result =>
                    {
                        newPopulation.Add(result.Individ1);
                        newPopulation.Add(result.Individ2);
                    });

                }

                engine.ExecutorService.WaitToFinish();
                var newPopList = newPopulation.ToList();
                if (newPopList.Count != 0)
                    Population.Individs = newPopulation.ToList();
                else
                {

                }

                generationNumber++;
            }

            var time = watch.ElapsedMilliseconds;
            return time;
        }

        private void CalculateFitness()
        {
            Population.Individs.ForEach(individ =>
            {
                var message = new Message()
                {
                    Command = "fitness",
                    Individ1 = individ
                };

                engine.ExecutorService.ScheduleTask(message, result =>
                {
                    individ.Fitness = result.Individ1.Fitness;
                });
            });

            engine.ExecutorService.WaitToFinish();
        }
    }
}
