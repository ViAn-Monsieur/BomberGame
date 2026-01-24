using System.Net;

namespace Networking
{
    public static class PacketDispatcher
    {
        // packetId = data[0]
        public static void Dispatch(IPEndPoint ep, byte[] data)
        {
            byte packetId = data[0];

            switch (packetId)
            {
                case 1: // UDP HELLO
                    HandleHello(ep);
                    break;

                case 2: // MOVE
                    HandleMove(ep, data);
                    break;
            }
        }

        static void HandleHello(IPEndPoint ep)
        {
            foreach (var c in TcpServer.Clients.Values)
            {
                if (c.UdpEndPoint == null)
                {
                    c.BindUdp(ep);
                    break;
                }
            }
        }

        static void HandleMove(IPEndPoint ep, byte[] data)
        {
            int dx = (sbyte)data[1];
            int dy = (sbyte)data[2];
            Console.WriteLine($"Move from {ep} : {dx},{dy}");
        }
    }
}
