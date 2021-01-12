using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Domain
{
    public class Population
    {
        public List<Individ> Individs { get; set; } = new List<Individ>();

        public static Population Generate(int populationSize)
        {
            var individuals = new List<Individ>();

            for (var i = 0; i < populationSize; ++i)
                individuals.Add(Individ.Generate());

            return new Population()
            {
                Individs = individuals,
            };
        }

    }
}
