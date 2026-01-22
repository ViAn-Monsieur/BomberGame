using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public class UdpServer
    {
        UdpClient udp;

        public UdpServer(int port)
        {
            udp = new UdpClient(port);
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

            PacketDispatcher.Dispatch(ep, data);

            udp.BeginReceive(OnReceive, null);
        }

        public void Send(IPEndPoint ep, byte[] data)
        {
            udp.Send(data, data.Length, ep);
        }
    }
}
