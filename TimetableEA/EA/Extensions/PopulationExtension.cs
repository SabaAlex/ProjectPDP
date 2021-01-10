using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;
using System.Linq;

namespace TimetableEA.EA.Extensions
{
    public static class PopulationExtension
    {
        public static void ComputeFitness(this Population population)
        {
            population.Individs.AsParallel().ForAll(element =>
            {
                element.ComputeFitness();
            });
        }

        public static void Mutate(this Population population)
        {
            population.Individs.AsParallel().ForAll(element =>
            {
                element.Mutate();
            });
        }

        public static Individ Fittest(this Population population)
        {
            return population.Individs.Min();
        }
    }
}
