using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TimetableEA.EA.Logic.Distributed.Models;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;

namespace TimetableEA.EA.Logic.Distributed.Services
{
    public class ExecutorService : Base
    {
        private ConcurrentBag<RemoteWorker> workers = new ConcurrentBag<RemoteWorker>();
        private ConcurrentQueue<Tuple<Message, Action<Message>>> commandQueue = new ConcurrentQueue<Tuple<Message, Action<Message>>>();
        private bool isRunning = false;
        private bool isWorking = true;
        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public ExecutorService(Engine engine) : base(engine)
        {

        }

        public void Start()
        {
            isRunning = true;
            Task.Factory.StartNew(() =>
            {
                while (isRunning == true)
                {
                    if (isWorking == false)
                        continue;
                    if (!commandQueue.IsEmpty)
                    {
                        var worker = GetFirstIdleWorker();
                        if (worker == null)
                            continue;

                        Tuple<Message, Action<Message>> tuple = null;

                        if (commandQueue.TryDequeue(out tuple))
                        {
                            worker.Execute(tuple.Item1, tuple.Item2);
                        }
                    }

                    if (commandQueue.IsEmpty)
                        if (RunningWorkers() == 0)
                        {
                            isWorking = false;
                            autoResetEvent.Set();
                        }
                }
            });
        }

        public RemoteWorker GetFirstIdleWorker()
        {
            var workerList = workers.ToList();

            foreach (var worker in workerList)
                if (worker.Status == WorkerStatus.Idle)
                    return worker;

            return null;
        }

        public void ScheduleTask(Message message, Action<Message> callback)
        {
            isWorking = true;
            autoResetEvent.Reset();

            var tuple = Tuple.Create(message, callback);
            commandQueue.Enqueue(tuple);
        }

        public int RunningWorkers()
        {
            var count = 0;
            workers.ToList().ForEach(worker =>
            {
                if (worker.Status == WorkerStatus.Working)
                    count++;
            });

            return count;    
        }

        public void WaitToFinish()
        {
            if (commandQueue.IsEmpty)
                if (RunningWorkers() == 0)
                    return;
            autoResetEvent.WaitOne(-1);
        }

        public void AddWorker(RemoteWorker worker)
        {
            workers.Add(worker);
        }
    }
}
