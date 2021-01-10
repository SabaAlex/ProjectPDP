using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.EA.Logic;

namespace TimetableEA.EA.UI
{
    public static class UI
    {
        public static void START()
        {
            var EA = new Algorithm(20);

            var timer = EA.StartAlgorithm(20, 5);

            Console.WriteLine($"Execution time: {timer}ms");

            var fittest = EA.Fittest();

            Console.WriteLine($"Fittest Individ:\n{fittest}\n\n Fitness: {fittest.Fitness}");
        }
    }
}
