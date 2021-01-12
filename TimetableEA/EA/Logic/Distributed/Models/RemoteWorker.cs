using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TimetableEA.EA.Logic.Distributed.Models
{
    public enum WorkerStatus
    {
        Idle,
        Working,
        Disconnected,
        None
    }

    public class RemoteWorker
    {
        public WorkerStatus Status = WorkerStatus.None;
        private Socket socket;
        private byte[] bytes = new byte[1920 * 1080 * 4];

        public RemoteWorker(Socket workerSocket)
        {
            socket = workerSocket;
            Status = WorkerStatus.Idle;
        }

        public void Execute(Message message, Action<Message> callback)
        {
            Status = WorkerStatus.Working;

            Task.Factory.StartNew(() =>
            {
                SendData(message);
                var result = ReciveData();
                callback?.Invoke(result);

                Status = WorkerStatus.Idle;
            }); 
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
    }
}
