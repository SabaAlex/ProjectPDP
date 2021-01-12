using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TimetableEA.EA.Logic.Distributed.Models;

namespace TimetableEA.EA.Logic.Distributed.Services
{
    public class NetworkService : Base
    {
        private bool connected = false;
        private IPEndPoint localEndPoint;
        private Socket listener;

        public NetworkService(Engine engine) : base(engine) { }

        public void Start()
        {
            try
            {
                var localIp = "127.0.0.1";

                localEndPoint = new IPEndPoint(IPAddress.Parse(localIp), 11194);
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                connected = true;
            }
            catch
            {
                Console.WriteLine("NetworkService: init network service failed");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                while (connected)
                {
                    if (listener != null)
                    {
                        listener.Listen(5);

                        var socket = listener.Accept();

                        //Console.WriteLine("Worker connected");

                        var worker = new RemoteWorker(socket);

                        Engine.ExecutorService.AddWorker(worker);
                    }
                }
            });
        }
    }
}
