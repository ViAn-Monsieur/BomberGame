using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public class ClientSession
    {
        public int Id { get; }
        public TcpClient Tcp { get; }
        public IPEndPoint UdpEndPoint { get; private set; }

        public ClientSession(int id, TcpClient tcp)
        {
            Id = id;
            Tcp = tcp;
        }

        public void BindUdp(IPEndPoint ep)
        {
            if (UdpEndPoint == null)
            {
                UdpEndPoint = ep;
                Console.WriteLine($"Client {Id} bind UDP {ep}");
            }
        }
    }
}
