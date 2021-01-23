using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;
using System.Linq;
using TimetableEA.StaticData;
using System.Threading.Tasks;
using System.Threading;

namespace TimetableEA.EA.Extensions
{
    public static class IndividExtended
    {
        public static void ComputeFitness(this Individ individ)
        {
            var fitness = 0;

            fitness += individ.Genes.Count - individ.Genes.Distinct().Count();

            individ.Fitness = fitness;
        }

        public static void Mutate(this Individ individ)
        {
            individ.Genes.AsParallel().ForAll(gene => gene.Mutate());
        }

        public static Tuple<Individ, Individ> Crossover(this Individ fatherIndivid, Individ motherIndivid)
        {
            var brotherIndividGenes = new Gene[AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber];
            var stuckSisterIndividGenes = new Gene[AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber];

            for(var index = 0; index < AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber; index++)
            {
                var siblingsGenesTuple = fatherIndivid.Genes[index].Crossover(motherIndivid.Genes[index]);
                brotherIndividGenes[index] = siblingsGenesTuple.Item1;
                stuckSisterIndividGenes[index] = siblingsGenesTuple.Item2;
            }

            return new Tuple<Individ, Individ>
            (
                new Individ() { Genes = brotherIndividGenes.ToList() },
                new Individ() { Genes = stuckSisterIndividGenes.ToList() }
            );
        }
    }
}
