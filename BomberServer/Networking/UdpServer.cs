using System.Net;
using System.Net.Sockets;
using BomberServer.Core;
using System.Collections.Concurrent;
using System.Text;

namespace Networking
{
    public class UdpServer
    {
        static UdpClient udp = default!;
        static ConcurrentDictionary<string, IPEndPoint> clients = new();

        GameServer gameServer;

        public UdpServer(int port, GameServer server)
        {
            udp = new UdpClient(port);
            gameServer = server;
        }

        public void Start()
        {
            udp.BeginReceive(OnReceive, null);
            Console.WriteLine("UDP Server started");
        }

        void OnReceive(IAsyncResult ar)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udp.EndReceive(ar, ref ep);

            if (ep != null)
            {
                string key = ep.ToString();

                // save endpoint
                clients.TryAdd(key, ep);

                gameServer.OnUdpPacket(ep, data);
            }

            udp.BeginReceive(OnReceive, null);
        }

        // Broadcast snapshot
        public static void Broadcast(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);

            foreach (var ep in clients.Values)
            {
                udp.Send(data, data.Length, ep);
            }
        }

        public void Send(IPEndPoint ep, byte[] data)
        {
            udp.Send(data, data.Length, ep);
        }
        public void SendToMany(IEnumerable<IPEndPoint> eps, byte[] data)
        {
            foreach (var ep in eps)
                udp.Send(data, data.Length, ep);
        }

    }
}
