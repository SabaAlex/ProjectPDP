using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;

namespace TimetableEA.EA.Logic
{
    public interface IAlgorithm
    {
        public Individ Fittest();
        public long StartAlgorithm(int generations, int sampleSize);
    }
}
