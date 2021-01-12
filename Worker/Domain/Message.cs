using System;
using System.Collections.Generic;
using System.Text;
using Worker.Domain;

namespace Worker.Domain
{
    public class Message
    {
        public string Command { get; set; }
        public Individ Individ1 { get; set; }
        public Individ Individ2 { get; set; }
    }
}
