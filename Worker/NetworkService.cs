using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading;
using Worker.Domain;
using Worker.StaticData;

namespace Worker
{
    public class NetworkService
    {
        public static double MUTATION_RATE { get; set; } = 0.05;
        public static double CROSSOVER_SELECTION_RATE { get; set; } = 0.5;

        private Socket socket;
        private byte[] bytes = new byte[1920 * 1080 * 4];

        private int CaculateFitness(Individ individ)
        {
            var intersections = new Dictionary<Gene, int>();

            foreach (var gene in individ.Genes)
            {
                if (!intersections.ContainsKey(gene))
                    intersections[gene] = 0;
                intersections[gene]++;
            }

            var fitness = 0;

            foreach (var keyValuePair in intersections)
                fitness += keyValuePair.Value - 1;

            return fitness;
        }

        private Tuple<Individ, Individ> Crossover(Individ pater, Individ mater)
        {
            var random = new Random();
            var child = new Individ();
            var antiChild = new Individ();

            for (var index = 0; index < pater.Genes.Count; index++)
            {
                var timeProb = random.NextDouble();
                var dayProb = random.NextDouble();
                var locationProb = random.NextDouble();

                var paterGene = pater.Genes[index];
                var materGene = mater.Genes[index];
                var childGene = new Gene()
                {
                    Time = timeProb <= CROSSOVER_SELECTION_RATE ? paterGene.Time : materGene.Time,
                    Day = dayProb <= CROSSOVER_SELECTION_RATE ? paterGene.Day : materGene.Day,
                    Location = locationProb <= CROSSOVER_SELECTION_RATE ? paterGene.Location : materGene.Location
                };
                var antiChildGene = new Gene()
                {
                    Time = timeProb <= CROSSOVER_SELECTION_RATE ? materGene.Time : paterGene.Time,
                    Day = dayProb <= CROSSOVER_SELECTION_RATE ? materGene.Day : paterGene.Day,
                    Location = locationProb <= CROSSOVER_SELECTION_RATE ? materGene.Location : paterGene.Location
                };

                child.Genes.Add(childGene);
                antiChild.Genes.Add(antiChildGene);
            }

            return Tuple.Create(child, antiChild);
        }

        private void Mutate(Individ individ)
        {
            var random = new Random();
            foreach (var gene in individ.Genes)
            {
                var timeProb = random.NextDouble();
                var dayProb = random.NextDouble();
                var locationProb = random.NextDouble();

                if (timeProb <= MUTATION_RATE)
                    gene.Time = random.Next(0, AlgorithmData.TimeDecoded.Count);

                if (dayProb <= MUTATION_RATE)
                    gene.Day = random.Next(0, AlgorithmData.DayDecoded.Count);

                if (locationProb <= MUTATION_RATE)
                    gene.Location = random.Next(0, AlgorithmData.LocationNumber);
            }
        }

        private Message HandleCommand(Message messgae)
        {
            var result = new Message();
            if (messgae.Command == "fitness")
            {
                var fitness = CaculateFitness(messgae.Individ1);
                result.Individ1 = new Individ();
                result.Individ1.Fitness = fitness;
                result.Command = "ok";
            }
            if (messgae.Command == "childrens")
            {
                var pater = messgae.Individ1;
                var mater = messgae.Individ2;

                var childrens = Crossover(pater, mater);

                var child = childrens.Item1;
                var antiChild = childrens.Item2;

                Mutate(child);
                Mutate(antiChild);

                child.Fitness = CaculateFitness(child);
                antiChild.Fitness = CaculateFitness(antiChild);

                result.Individ1 = child;
                result.Individ2 = antiChild;
                result.Command = "ok";
            }

            return result;
        }

        private byte[] Serialize(Message message)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };
            string jsonString = JsonSerializer.Serialize(message, options);
            return Encoding.Default.GetBytes(jsonString);
        }

        private Message Deserialize(byte[] data, int size)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };
            var strJson = System.Text.Encoding.Default.GetString(data, 0, size);
            return JsonSerializer.Deserialize<Message>(strJson, options);
        }

        private int ReceiveAll(int size)
        {
            try
            {
                var totalBytesReceived = 0;
                var numberOfBytes = size;
                while (totalBytesReceived < numberOfBytes)
                {
                    var bytesRemaining = numberOfBytes - totalBytesReceived;
                    var bytesReceived = 0;
                    if (socket != null)
                        bytesReceived = socket.Receive(bytes, totalBytesReceived, bytesRemaining, SocketFlags.None);
                    totalBytesReceived += bytesReceived;
                }
                return totalBytesReceived;
            }
            catch
            {
                return 0;
            }
        }

        private Message ReciveData()
        {
            var lengthBytes = new byte[4];
            socket.Receive(lengthBytes, lengthBytes.Length, 0);
            
            var length = BitConverter.ToInt32(lengthBytes);

            ReceiveAll(length);

            return Deserialize(bytes, length);
        }

        private void SendData(Message message)
        {
            var data = Serialize(message);
            var lengthBytes = BitConverter.GetBytes(data.Length);
            socket.Send(lengthBytes, lengthBytes.Length, 0);
            socket.Send(data, data.Length, 0);
        }

        public void Start()
        {
            var localIp = "127.0.0.1";
            var endPoint = new IPEndPoint(IPAddress.Parse(localIp), 11194);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var stopwatch = new Stopwatch();
            var disconnect = true;
            while (true)
            {
                try
                {
                    if (disconnect)
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(endPoint);
                        disconnect = false;
                    }

                    var message = ReciveData();
                    var result = HandleCommand(message);
                    SendData(result);
                    if (result.Individ1 != null)
                        Console.WriteLine($"i1: {result.Individ1.Fitness}");
                    if (result.Individ2 != null)
                        Console.WriteLine($"i2: {result.Individ2.Fitness}");

                }
                catch
                {
                    Console.WriteLine("Disconnect");
                    Thread.Sleep(1000);
                    Console.WriteLine("Trying to reconenct");
                    disconnect = true;
                }
                
            }
        }
    }
}
