using System.Net;
using System.Net.Sockets;
using BomberServer.Core;

namespace Networking
{
    public class UdpServer
    {
        UdpClient udp;
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
                gameServer.OnUdpPacket(ep, data);

            udp.BeginReceive(OnReceive, null);
        }


        public void Send(IPEndPoint ep, byte[] data)
        {
            udp.Send(data, data.Length, ep);
        }
    }
}
