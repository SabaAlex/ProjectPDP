using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableEA.EA.Logic.Distributed.Services
{
    public class Base
    {
        protected Engine Engine { get; set; }

        public Base(Engine engine)
        {
            Engine = engine;
        }
    }
}
