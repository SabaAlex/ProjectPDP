using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.StaticData;
using System.Linq;

namespace TimetableEA.Domain
{
    public class Individ : IComparable
    {
        public List<Gene> Genes { get; set; } = new List<Gene>();

        public int Fitness { get; set; }

        public static Individ Generate()
        {
            var genes = new List<Gene>();

            for (var i = 0; i < AlgorithmData.CoursesNumber * AlgorithmData.GroupsNumber; ++i)
                genes.Add(Gene.Generate());

            return new Individ()
            {
                Genes = genes
            };
        }

        public int CompareTo(object obj)
        {
            var objCopy = (Individ)obj;

            if (Fitness > objCopy.Fitness)
                return 1;
            if (Fitness < objCopy.Fitness)
                return -1;
            
            return 0;
        }

        public override string ToString()
        {
            var index = -1;
            return Genes.Aggregate("", (acc, curr) =>
            {
                index++;
                return acc + $"Course: {index / AlgorithmData.GroupsNumber}, Group: {index % AlgorithmData.GroupsNumber}, {curr}\n";
            });
        }
    }
}
