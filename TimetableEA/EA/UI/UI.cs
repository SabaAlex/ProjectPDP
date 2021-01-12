using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;
using TimetableEA.EA.Logic;

namespace TimetableEA.EA.UI
{
    public static class UI
    {
        public static void StartParallel()
        {
            var EA = new ParallelAlgorithm(10);

            var timer = EA.StartAlgorithm(20, 5);

            Console.WriteLine($"Execution time: {timer}ms");

            var fittest = EA.Fittest();

            Console.WriteLine($"Fittest Individ:\n{fittest}\n\n Fitness: {fittest.Fitness}");
        }

        public static void StartDistributed()
        {
            var ea = new DistributedAlgorithm(10);
            var timer = ea.StartAlgorithm(800, 5);

            Console.WriteLine($"Execution time: {timer}ms");

            var fittest = ea.Fittest();

            Console.WriteLine($"Fittest Individ:\n{fittest}\n\n Fitness: {fittest.Fitness}");
        }


        public static void START()
        {
            StartDistributed();
        }
    }
}
