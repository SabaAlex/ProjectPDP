using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableEA.EA.Logic.Distributed.Services
{
    public sealed class Engine
    {
        private static Engine instance = null;
        private static readonly object padlock = new object();
        public NetworkService NetworkService { get; set; }
        public ExecutorService ExecutorService { get; set; }

        public Engine()
        {
            NetworkService = new NetworkService(this);
            ExecutorService = new ExecutorService(this);
        }

        public void Initialize()
        {
            NetworkService.Start();
            ExecutorService.Start();
        }

        public static Engine Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ??= new Engine();
                }
            }
        }
    }
}
