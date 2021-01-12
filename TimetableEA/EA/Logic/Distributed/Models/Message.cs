using System;
using System.Collections.Generic;
using System.Text;
using TimetableEA.Domain;

namespace TimetableEA.EA.Logic.Distributed.Models
{
    public class Message
    {
        public string Command { get; set; }
        public Individ Individ1 { get; set; }
        public Individ Individ2 { get; set; }

    }
}
