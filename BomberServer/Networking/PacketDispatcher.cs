using System.Net;
using System.Text;

namespace Networking
{
    public static class PacketDispatcher
    {
        public static void Dispatch(IPEndPoint ep, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);

            // UDP bind
            if (msg.StartsWith("bind:"))
            {
                int id = int.Parse(msg.Replace("bind:", ""));

                if (TcpServer.Clients.TryGetValue(id, out var session))
                {
                    session.BindUdp(ep);
                }

                return;
            }
        }
    }
}
