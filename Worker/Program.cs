using System;
using System.Collections.Generic;
using Worker.Domain;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new NetworkService();
            service.Start();
        }
    }
}
