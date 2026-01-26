using System.Net;
using System.Net.Sockets;
using System.Text;
using BomberServer.Models;

namespace Networking
{
    public class ClientSession
    {
        public int Id { get; }
        public TcpClient Tcp { get; }

        public Player? Player { get; set; }

        public ClientSession(int id, TcpClient tcp)
        {
            Id = id;
            Tcp = tcp;
        }

        public void BindUdp(IPEndPoint ep)
        {
            if (Player == null)
                return;

            if (Player.RemoteEndPoint == null)
            {
                Player.RemoteEndPoint = ep;
                Console.WriteLine($"Client {Id} bind UDP {ep}");
            }
        }

        public void Send(string msg)
        {
            var stream = Tcp.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            stream.Write(data, 0, data.Length);
        }
    }
}
