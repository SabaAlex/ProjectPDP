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

            Parallel.For(0, AlgorithmData.CoursesNumber, index =>
            {
                var groupsWithCourseI = individ.Genes.AsParallel().Where((element, index) =>
                {
                    return index % AlgorithmData.CoursesNumber == index;
                }).ToList();

                Interlocked.Add(ref fitness, groupsWithCourseI.Count - groupsWithCourseI.Distinct().Count());
            });

            //for (var i = 0; i < AlgorithmData.CoursesNumber; i++)
            //{
            //    var groupsWithCourseI = individ.Genes.Where((element, index) =>
            //    {
            //        return index % AlgorithmData.CoursesNumber == i;
            //    }).ToList();

            //    fitness += groupsWithCourseI.Count - groupsWithCourseI.Distinct().Count();
            //}

            individ.Fitness = fitness;
        }

        public static void Mutate(this Individ individ)
        {
            individ.Genes.ForEach(gene => gene.Mutate());
        }

        public static Tuple<Individ, Individ> Crossover(this Individ fatherIndivid, Individ motherIndivid)
        {
            var brotherIndividGenes = new Gene[AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber];
            var stuckSisterIndividGenes = new Gene[AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber];

            Parallel.For(0, AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber, index =>
            {
                var siblingsGenesTuple = fatherIndivid.Genes[index].Crossover(motherIndivid.Genes[index]);
                brotherIndividGenes[index] = siblingsGenesTuple.Item1;
                stuckSisterIndividGenes[index] = siblingsGenesTuple.Item2;
            });

            return new Tuple<Individ, Individ>
            (
                new Individ() { Genes = brotherIndividGenes.ToList() },
                new Individ() { Genes = stuckSisterIndividGenes.ToList() }
            );
        }
    }
}
