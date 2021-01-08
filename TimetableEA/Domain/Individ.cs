using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.StaticData;
using System.Linq;

namespace TimetableEA.Domain
{
    public class Individ
    {
        public List<Gene> Genes { get; set; } = new List<Gene>();

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

        public override string ToString()
        {
            var index = -1;
            return Genes.Aggregate("", (acc, curr) =>
            {
                index++;
                return $"Group: {index % AlgorithmData.GroupsNumber}, Course: {index % AlgorithmData.CoursesNumber}, {curr}";
            });
        }
    }
}
